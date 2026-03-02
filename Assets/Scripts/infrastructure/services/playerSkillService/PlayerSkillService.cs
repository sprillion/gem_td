using System;
using System.Collections.Generic;
using System.Linq;
using infrastructure.services.gameStateService;
using infrastructure.services.playerService;
using infrastructure.services.updateService;
using level.builder;
using skills;
using skills.data;
using towers;
using UnityEngine;
using Zenject;

namespace infrastructure.services.playerSkillService
{
    public class PlayerSkillService : IPlayerSkillService
    {
        private readonly IPlayerService _playerService;
        private readonly IGameStateService _gameStateService;
        private readonly IUpdateService _updateService;

        private ILevelBuilder _levelBuilder;

        private readonly PlayerSkillInstance[] _equippedSkills = new PlayerSkillInstance[4];
        private readonly List<TowerBuff> _activeBuffs = new List<TowerBuff>();
        private readonly List<PlayerSkillData> _availableSkills = new List<PlayerSkillData>();

        private bool _isTargeting;
        private int _targetingSlotIndex = -1;
        private Tower _firstTargetTower;

        // Chance boosts (consumed after next tower placement)
        private TowerType? _activeTypeBoost;
        private float _typeChanceBonusPercent;
        private int? _activeLevelBoost;
        private float _levelChanceBonusPercent;

        public IReadOnlyList<PlayerSkillInstance> EquippedSkills => _equippedSkills;
        public IReadOnlyList<PlayerSkillData> AvailableSkills => _availableSkills;
        public bool IsTargeting => _isTargeting;

        public event Action<int> OnSkillActivated;
        public event Action<int> OnSkillCooldownUpdated;
        public event Action OnSkillsChanged;
        public event Action<int, SkillTargetMode> OnTargetingStarted;
        public event Action OnTargetingCancelled;

        [Inject]
        public PlayerSkillService(IPlayerService playerService, IGameStateService gameStateService,
            IUpdateService updateService)
        {
            _playerService = playerService;
            _gameStateService = gameStateService;
            _updateService = updateService;

            _updateService.OnUpdate += OnUpdate;
        }

        public void Initialize(ILevelBuilder levelBuilder)
        {
            _levelBuilder = levelBuilder;
        }

        public void SetAvailableSkills(IReadOnlyList<PlayerSkillData> skills)
        {
            _availableSkills.Clear();
            foreach (var skill in skills)
                _availableSkills.Add(skill);
        }

        public void EquipSkill(int slotIndex, PlayerSkillData data, int upgradeLevel)
        {
            if (slotIndex < 0 || slotIndex >= 4) return;
            _equippedSkills[slotIndex] = new PlayerSkillInstance(data, upgradeLevel);
            OnSkillsChanged?.Invoke();
        }

        public bool CanActivateSkill(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= 4) return false;
            var skill = _equippedSkills[slotIndex];
            if (skill == null) return false;
            if (skill.IsOnCooldown) return false;
            if (_playerService.Gold < skill.GoldCost) return false;
            if (_isTargeting) return false;

            // Check phase restriction
            var currentPhase = _gameStateService.CurrentPhase;
            if (skill.Data.AvailableInPhases != null && skill.Data.AvailableInPhases.Length > 0)
            {
                if (!skill.Data.AvailableInPhases.Contains(currentPhase))
                    return false;
            }

            return true;
        }

        public void ActivateSkill(int slotIndex)
        {
            if (!CanActivateSkill(slotIndex)) return;

            var skill = _equippedSkills[slotIndex];

            if (skill.Data.TargetMode == SkillTargetMode.None)
            {
                ExecuteSkill(slotIndex, null, null);
            }
            else
            {
                _isTargeting = true;
                _targetingSlotIndex = slotIndex;
                _firstTargetTower = null;
                OnTargetingStarted?.Invoke(slotIndex, skill.Data.TargetMode);
            }
        }

        public void CancelTargeting()
        {
            if (!_isTargeting) return;
            _isTargeting = false;
            _targetingSlotIndex = -1;
            _firstTargetTower = null;
            OnTargetingCancelled?.Invoke();
        }

        public void OnTargetSelected(Tower tower)
        {
            if (!_isTargeting || tower == null) return;

            var skill = _equippedSkills[_targetingSlotIndex];
            if (skill == null) return;

            if (skill.Data.TargetMode == SkillTargetMode.SingleTower)
            {
                var slotIndex = _targetingSlotIndex;
                _isTargeting = false;
                _targetingSlotIndex = -1;
                ExecuteSkill(slotIndex, tower, null);
            }
            else if (skill.Data.TargetMode == SkillTargetMode.TwoTowers)
            {
                if (_firstTargetTower == null)
                {
                    _firstTargetTower = tower;
                }
                else
                {
                    if (_firstTargetTower == tower)
                    {
                        Debug.Log("Cannot swap a tower with itself");
                        return;
                    }
                    var slotIndex = _targetingSlotIndex;
                    var first = _firstTargetTower;
                    _isTargeting = false;
                    _targetingSlotIndex = -1;
                    _firstTargetTower = null;
                    ExecuteSkill(slotIndex, first, tower);
                }
            }
        }

        public void ConsumeChanceBoost()
        {
            _activeTypeBoost = null;
            _typeChanceBonusPercent = 0f;
            _activeLevelBoost = null;
            _levelChanceBonusPercent = 0f;
        }

        public TowerType? GetActiveTypeChanceBoost() => _activeTypeBoost;
        public float GetTypeChanceBonusPercent() => _typeChanceBonusPercent;
        public int? GetActiveLevelChanceBoost() => _activeLevelBoost;
        public float GetLevelChanceBonusPercent() => _levelChanceBonusPercent;

        private void ExecuteSkill(int slotIndex, Tower target1, Tower target2)
        {
            var skill = _equippedSkills[slotIndex];
            if (skill == null) return;

            // Deduct gold
            _playerService.AddGold(-skill.GoldCost);

            // Start cooldown
            skill.CooldownRemaining = skill.Cooldown;

            switch (skill.Data)
            {
                case SwapTowersSkillData _:
                    ExecuteSwapTowers(target1, target2);
                    break;
                case RestoreHealthSkillData healData:
                    ExecuteRestoreHealth(healData, skill.UpgradeLevel);
                    break;
                case TowerBuffSkillData buffData:
                    ExecuteTowerBuff(target1, buffData, skill);
                    break;
                case TowerTypeChanceSkillData typeChanceData:
                    ExecuteTypeChance(typeChanceData, skill.UpgradeLevel);
                    break;
                case TowerLevelChanceSkillData levelChanceData:
                    ExecuteLevelChance(levelChanceData, skill.UpgradeLevel);
                    break;
            }

            OnSkillActivated?.Invoke(slotIndex);
        }

        private void ExecuteSwapTowers(Tower tower1, Tower tower2)
        {
            if (tower1 == null || tower2 == null || _levelBuilder == null) return;

            // Find grid positions
            var mapData = _levelBuilder.MapData;
            Vector2Int pos1 = FindTowerGridPosition(tower1);
            Vector2Int pos2 = FindTowerGridPosition(tower2);

            if (pos1.x < 0 || pos2.x < 0)
            {
                Debug.LogWarning("Could not find tower grid positions for swap");
                return;
            }

            // Swap in tower map
            var towerMap = _levelBuilder.TowerMap;
            var tempType = towerMap[pos1.x, pos1.y];
            towerMap[pos1.x, pos1.y] = towerMap[pos2.x, pos2.y];
            towerMap[pos2.x, pos2.y] = tempType;

            // Swap world positions
            var tempPos = tower1.transform.position;
            tower1.transform.position = tower2.transform.position;
            tower2.transform.position = tempPos;

            // Swap in level builder dictionary
            _levelBuilder.SwapTowers(pos1.x, pos1.y, pos2.x, pos2.y);

            Debug.Log($"Swapped towers at ({pos1.x},{pos1.y}) and ({pos2.x},{pos2.y})");
        }

        private void ExecuteRestoreHealth(RestoreHealthSkillData data, int level)
        {
            int heal = UnityEngine.Random.Range(data.MinHealByLevel[level], data.MaxHealByLevel[level] + 1);
            _playerService.AddLives(heal);
            Debug.Log($"Restored {heal} HP");
        }

        private void ExecuteTowerBuff(Tower tower, TowerBuffSkillData data, PlayerSkillInstance skill)
        {
            if (tower == null) return;

            float value = data.BuffValueByLevel[skill.UpgradeLevel];
            float duration = data.BuffDurationByLevel[skill.UpgradeLevel];

            var buff = new TowerBuff(tower, skill.Data.SkillType, value, duration);
            buff.Apply();
            _activeBuffs.Add(buff);

            Debug.Log($"Applied {skill.Data.SkillType} buff ({value}) to tower for {duration}s");
        }

        private void ExecuteTypeChance(TowerTypeChanceSkillData data, int level)
        {
            _activeTypeBoost = data.TargetTowerType;
            _typeChanceBonusPercent = data.BonusChanceByLevel[level];
            Debug.Log($"Type chance boost active: {data.TargetTowerType} +{_typeChanceBonusPercent}%");
        }

        private void ExecuteLevelChance(TowerLevelChanceSkillData data, int level)
        {
            _activeLevelBoost = data.TargetTowerLevel;
            _levelChanceBonusPercent = data.BonusChanceByLevel[level];
            Debug.Log($"Level chance boost active: Level {data.TargetTowerLevel} +{_levelChanceBonusPercent}%");
        }

        private Vector2Int FindTowerGridPosition(Tower tower)
        {
            if (_levelBuilder == null) return new Vector2Int(-1, -1);
            var mapData = _levelBuilder.MapData;
            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    if (_levelBuilder.GetTowerAtPosition(x, y) == tower)
                        return new Vector2Int(x, y);
                }
            }
            return new Vector2Int(-1, -1);
        }

        private void OnUpdate()
        {
            float dt = Time.deltaTime;

            // Update cooldowns
            for (int i = 0; i < _equippedSkills.Length; i++)
            {
                var skill = _equippedSkills[i];
                if (skill == null || !skill.IsOnCooldown) continue;

                skill.CooldownRemaining -= dt;
                if (skill.CooldownRemaining < 0f)
                    skill.CooldownRemaining = 0f;

                OnSkillCooldownUpdated?.Invoke(i);
            }

            // Update active buffs
            for (int i = _activeBuffs.Count - 1; i >= 0; i--)
            {
                _activeBuffs[i].Update(dt);
                if (_activeBuffs[i].IsExpired)
                {
                    _activeBuffs[i].Remove();
                    _activeBuffs.RemoveAt(i);
                }
            }
        }
    }
}
