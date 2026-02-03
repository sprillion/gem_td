using System.Collections.Generic;
using System.Linq;
using enemies;
using infrastructure.services.abilityService;
using infrastructure.services.combatService;
using infrastructure.services.gameStateService;
using infrastructure.services.inputService;
using infrastructure.services.updateService;
using towers.abilities;
using UnityEngine;
using Zenject;

namespace towers
{
    public class Tower : MonoBehaviour, IClickable
    {
        [SerializeField] private EnemyTrigger _enemyTrigger;

        private readonly List<Enemy> _enemiesInside = new List<Enemy>();
        private Enemy _currentTarget;

        private IUpdateService _updateService;
        private ICombatService _combatService;
        private IGameStateService _gameStateService;
        private IAbilityService _abilityService;

        public TowerData TowerData { get; private set; }
        public Transform ScaledObject;

        [Inject]
        public void Construct(IUpdateService updateService, ICombatService combatService,
                            IGameStateService gameStateService, IAbilityService abilityService)
        {
            _updateService = updateService;
            _combatService = combatService;
            _gameStateService = gameStateService;
            _abilityService = abilityService;

            _updateService.OnUpdate += OnUpdate;
        }

        public void Initialize(TowerData towerData)
        {
            TowerData = towerData;
            Subscribes();

            // Set enemy trigger radius based on attack range
            if (_enemyTrigger != null)
            {
                _enemyTrigger.SetRadius(towerData.AttackRange);
            }

            // Register if this tower has aura ability (E-type)
            if (towerData.Abilities != null &&
                towerData.Abilities.Any(a => a.AbilityType == AbilityType.AttackSpeedAura))
            {
                _abilityService.RegisterAuraTower(this);
            }
        }

        public void OnClick()
        {
            if (_gameStateService.CurrentPhase == GamePhase.SELECTING_TOWER)
            {
                _gameStateService.SelectTower(this);
            }
        }

        private void OnUpdate()
        {
            if (_gameStateService.CurrentPhase != GamePhase.COMBAT) return;
            if (!enabled) return; // Don't attack if disabled (stone tower)
            if (_currentTarget == null || !_currentTarget.IsAlive)
            {
                _currentTarget = _enemiesInside.FirstOrDefault(e => e != null && e.IsAlive);
                return;
            }

            // Get attack speed multiplier from passive/aura buffs
            float attackSpeedMultiplier = _abilityService.GetTowerAttackSpeedMultiplier(this);

            if (_combatService.CanTowerAttack(this, attackSpeedMultiplier))
            {
                _combatService.RegisterAttack(this);

                // Execute OnAttack abilities BEFORE damage (D, R, Y)
                _abilityService.ExecuteAbilities(this, _currentTarget, _enemiesInside);

                // Deal primary damage with armor reduction
                _combatService.DealDamage(_currentTarget, TowerData.Damage);

                Debug.Log($"Tower {TowerData.TowerType} attacked for {TowerData.Damage} damage");
            }
        }

        private void OnDestroy()
        {
            if (_updateService != null)
            {
                _updateService.OnUpdate -= OnUpdate;
            }

            if (_abilityService != null)
            {
                _abilityService.UnregisterAuraTower(this);
            }
        }
        
        private void Subscribes()
        {
            _enemyTrigger.OnEnemyEnter += AddEnemy;
            _enemyTrigger.OnEnemyExit += RemoveEnemy;
        }

        private void AddEnemy(Enemy enemy)
        {
            if (enemy == null) return;
            _enemiesInside.Add(enemy);
            if (_currentTarget == null || !_currentTarget.IsAlive)
            {
                _currentTarget = enemy;
            }
        }

        private void RemoveEnemy(Enemy enemy)
        {
            if (enemy == null) return;
            _enemiesInside.Remove(enemy);
            if (_currentTarget == enemy)
            {
                _currentTarget = _enemiesInside.FirstOrDefault(e => e != null && e.IsAlive);
            }
        }
    }
}