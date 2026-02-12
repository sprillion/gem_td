using System.Collections.Generic;
using enemies;
using infrastructure.services.effectService;
using infrastructure.services.selectionService;
using infrastructure.services.updateService;
using TMPro;
using towers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ui.selection
{
    public class SelectionInfoPanel : MonoBehaviour
    {
        [Header("Main Panels")]
        [SerializeField] private GameObject _towerPanel;
        [SerializeField] private GameObject _enemyPanel;
        [SerializeField] private GameObject _emptyPanel;

        [Header("Tower Info")]
        [SerializeField] private Image _towerIcon;
        [SerializeField] private TMP_Text _towerName;
        [SerializeField] private TMP_Text _towerLevel;
        [SerializeField] private TMP_Text _towerDamage;
        [SerializeField] private TMP_Text _towerAttackSpeed;
        [SerializeField] private TMP_Text _towerAttackRange;
        [SerializeField] private Transform _abilitiesContainer;
        [SerializeField] private GameObject _abilityItemPrefab;

        [Header("Enemy Info")]
        [SerializeField] private Image _enemyIcon;
        [SerializeField] private TMP_Text _enemyName;
        [SerializeField] private Slider _enemyHealthBar;
        [SerializeField] private TMP_Text _enemyHealthText;
        [SerializeField] private TMP_Text _enemyDamage;
        [SerializeField] private TMP_Text _enemyMoveSpeed;
        [SerializeField] private TMP_Text _enemyArmor;
        [SerializeField] private TMP_Text _enemyMagicResist;
        [SerializeField] private TMP_Text _enemyEvasion;
        [SerializeField] private Transform _effectsContainer;
        [SerializeField] private GameObject _effectItemPrefab;

        private ISelectionService _selectionService;
        private IEffectService _effectService;
        private IUpdateService _updateService;

        private List<GameObject> _activeAbilityItems = new List<GameObject>();
        private List<GameObject> _activeEffectItems = new List<GameObject>();

        [Inject]
        public void Construct(ISelectionService selectionService, IEffectService effectService, IUpdateService updateService)
        {
            _selectionService = selectionService;
            _effectService = effectService;
            _updateService = updateService;
        }

        private void Start()
        {
            _selectionService.OnTowerSelected += HandleTowerSelected;
            _selectionService.OnEnemySelected += HandleEnemySelected;
            _selectionService.OnSelectionCleared += HandleSelectionCleared;

            _updateService.OnUpdate += OnUpdate;

            ShowEmptyPanel();
        }

        private void OnDestroy()
        {
            if (_selectionService != null)
            {
                _selectionService.OnTowerSelected -= HandleTowerSelected;
                _selectionService.OnEnemySelected -= HandleEnemySelected;
                _selectionService.OnSelectionCleared -= HandleSelectionCleared;
            }

            if (_updateService != null)
            {
                _updateService.OnUpdate -= OnUpdate;
            }
        }

        private void OnUpdate()
        {
            // Real-time update for enemy info
            if (_selectionService.CurrentSelectionType == SelectionType.Enemy && _selectionService.SelectedEnemy != null)
            {
                UpdateEnemyHealth();
                UpdateEnemyEffects();
            }
        }

        private void HandleTowerSelected(Tower tower)
        {
            ShowTowerPanel(tower);
        }

        private void HandleEnemySelected(Enemy enemy)
        {
            ShowEnemyPanel(enemy);
        }

        private void HandleSelectionCleared()
        {
            ShowEmptyPanel();
        }

        private void ShowTowerPanel(Tower tower)
        {
            if (tower == null || tower.TowerData == null) return;

            _towerPanel.SetActive(true);
            _enemyPanel.SetActive(false);
            _emptyPanel.SetActive(false);

            PopulateTowerInfo(tower);
        }

        private void ShowEnemyPanel(Enemy enemy)
        {
            if (enemy == null || enemy.EnemyData == null) return;

            _towerPanel.SetActive(false);
            _enemyPanel.SetActive(true);
            _emptyPanel.SetActive(false);

            PopulateEnemyInfo(enemy);
        }

        private void ShowEmptyPanel()
        {
            _towerPanel.SetActive(false);
            _enemyPanel.SetActive(false);
            _emptyPanel.SetActive(true);
        }

        private void PopulateTowerInfo(Tower tower)
        {
            var data = tower.TowerData;

            // Icon
            if (_towerIcon != null && data.Icon != null)
            {
                _towerIcon.sprite = data.Icon;
                _towerIcon.enabled = true;
            }
            else if (_towerIcon != null)
            {
                _towerIcon.enabled = false;
            }

            // Name
            if (_towerName != null)
            {
                _towerName.text = GetTowerDisplayName(data.TowerType);
            }

            // Level
            if (_towerLevel != null)
            {
                _towerLevel.text = $"Level {data.Level + 1}";
            }

            // Stats
            if (_towerDamage != null)
            {
                _towerDamage.text = $"Damage: {data.Damage}";
            }

            if (_towerAttackSpeed != null)
            {
                _towerAttackSpeed.text = $"Attack Speed: {data.AttackSpeed}";
            }

            if (_towerAttackRange != null)
            {
                _towerAttackRange.text = $"Range: {data.AttackRange:F1}";
            }

            // Abilities
            ClearAbilityItems();
            if (data.Abilities != null && data.Abilities.Count > 0)
            {
                foreach (var ability in data.Abilities)
                {
                    CreateAbilityItem(ability, data.Level);
                }
            }
        }

        private void PopulateEnemyInfo(Enemy enemy)
        {
            var data = enemy.EnemyData;

            // Icon
            if (_enemyIcon != null && data.Icon != null)
            {
                _enemyIcon.sprite = data.Icon;
                _enemyIcon.enabled = true;
            }
            else if (_enemyIcon != null)
            {
                _enemyIcon.enabled = false;
            }

            // Name
            if (_enemyName != null)
            {
                _enemyName.text = enemy.name;
            }

            // Health
            UpdateEnemyHealth();

            // Stats (use current modifiers)
            if (_enemyDamage != null)
            {
                _enemyDamage.text = $"Damage: {data.Damage}";
            }

            if (_enemyMoveSpeed != null)
            {
                _enemyMoveSpeed.text = $"Speed: {enemy.CurrentMoveSpeed:F1}";
            }

            if (_enemyArmor != null)
            {
                _enemyArmor.text = $"Armor: {enemy.CurrentArmor}";
            }

            if (_enemyMagicResist != null)
            {
                _enemyMagicResist.text = $"Magic Resist: {data.MagicResist}";
            }

            if (_enemyEvasion != null)
            {
                _enemyEvasion.text = $"Evasion: {data.Evasion}%";
            }

            // Effects
            UpdateEnemyEffects();
        }

        private void UpdateEnemyHealth()
        {
            var enemy = _selectionService.SelectedEnemy;
            if (enemy == null || enemy.EnemyData == null) return;

            if (_enemyHealthBar != null)
            {
                _enemyHealthBar.maxValue = enemy.EnemyData.Health;
                _enemyHealthBar.value = enemy.CurrentHealth;
            }

            if (_enemyHealthText != null)
            {
                _enemyHealthText.text = $"{enemy.CurrentHealth} / {enemy.EnemyData.Health}";
            }
        }

        private void UpdateEnemyEffects()
        {
            var enemy = _selectionService.SelectedEnemy;
            if (enemy == null) return;

            ClearEffectItems();

            var effects = _effectService.GetActiveEffects(enemy);
            if (effects != null && effects.Count > 0)
            {
                foreach (var effect in effects)
                {
                    CreateEffectItem(effect);
                }
            }
        }

        private void CreateAbilityItem(towers.abilities.AbilityData ability, int towerLevel)
        {
            if (_abilityItemPrefab == null || _abilitiesContainer == null) return;

            var itemObj = Instantiate(_abilityItemPrefab, _abilitiesContainer);
            var itemUI = itemObj.GetComponent<AbilityItemUI>();
            if (itemUI != null)
            {
                itemUI.Setup(ability, towerLevel);
            }

            _activeAbilityItems.Add(itemObj);
        }

        private void CreateEffectItem(towers.abilities.effects.Effect effect)
        {
            if (_effectItemPrefab == null || _effectsContainer == null) return;

            var itemObj = Instantiate(_effectItemPrefab, _effectsContainer);
            var itemUI = itemObj.GetComponent<EffectItemUI>();
            if (itemUI != null)
            {
                itemUI.Setup(effect);
            }

            _activeEffectItems.Add(itemObj);
        }

        private void ClearAbilityItems()
        {
            foreach (var item in _activeAbilityItems)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }
            _activeAbilityItems.Clear();
        }

        private void ClearEffectItems()
        {
            foreach (var item in _activeEffectItems)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }
            _activeEffectItems.Clear();
        }

        private string GetTowerDisplayName(TowerType towerType)
        {
            switch (towerType)
            {
                case TowerType.P: return "Armor Breaker (P)";
                case TowerType.Q: return "Rapid Fire (Q)";
                case TowerType.D: return "Heavy Hitter (D)";
                case TowerType.G: return "Poison Tower (G)";
                case TowerType.E: return "Support Aura (E)";
                case TowerType.R: return "Splash Tower (R)";
                case TowerType.B: return "Frost Tower (B)";
                case TowerType.Y: return "Multi-Shot (Y)";
                case TowerType.Stone: return "Stone Obstacle";
                default: return towerType.ToString();
            }
        }
    }
}
