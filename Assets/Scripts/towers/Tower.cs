using System.Collections.Generic;
using System.Linq;
using enemies;
using infrastructure.services.abilityService;
using infrastructure.services.combatService;
using infrastructure.services.gameStateService;
using infrastructure.services.inputService;
using infrastructure.services.projectileService;
using infrastructure.services.selectionService;
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
        private ISelectionService _selectionService;
        private IAbilityService _abilityService;
        private IProjectileService _projectileService;

        public TowerData TowerData { get; private set; }
        public Transform ScaledObject;

        private GameObject _highlightEffect;
        private ParticleSystem _highlightParticles;

        public List<Enemy> EnemiesInRange => _enemiesInside;

        [Inject]
        public void Construct(IUpdateService updateService, ICombatService combatService,
                            IGameStateService gameStateService, ISelectionService selectionService,
                            IAbilityService abilityService, IProjectileService projectileService)
        {
            _updateService = updateService;
            _combatService = combatService;
            _gameStateService = gameStateService;
            _selectionService = selectionService;
            _abilityService = abilityService;
            _projectileService = projectileService;

            _updateService.OnUpdate += OnUpdate;

            CreateClickCollider();
        }

        private void CreateClickCollider()
        {
            var clickTarget = new GameObject("ClickTarget");
            clickTarget.transform.SetParent(transform);
            clickTarget.transform.localPosition = Vector3.zero;
            clickTarget.AddComponent<BoxCollider>();
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
            // Always use SelectionService to open info panel
            _selectionService?.SelectTower(this);
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

                var targets = _abilityService.CollectAttackTargets(this, _currentTarget, _enemiesInside);

                foreach (var projectileTarget in targets)
                {
                    var target = projectileTarget.Target;
                    var damage = projectileTarget.Damage;
                    var isPrimary = projectileTarget.IsPrimary;

                    _projectileService.SpawnProjectile(this, target, () =>
                    {
                        if (target == null || !target.IsAlive) return;

                        _combatService.DealDamage(target, damage);

                        if (isPrimary)
                        {
                            _abilityService.ExecuteSplashOnArrival(this, target, _enemiesInside);
                            _abilityService.ExecuteOnHitAbilities(this, target);
                        }
                    });
                }
            }
        }

        public void SetHighlightEffect(GameObject highlightPrefab)
        {
            if (_highlightEffect != null)
            {
                Destroy(_highlightEffect);
            }

            _highlightEffect = Instantiate(highlightPrefab, transform);
            _highlightEffect.transform.localPosition = Vector3.zero;
            _highlightParticles = _highlightEffect.GetComponent<ParticleSystem>();

            // Don't play immediately - wait for SetHighlight call
            if (_highlightParticles != null)
            {
                _highlightParticles.Stop();
            }
        }

        public void SetHighlight(bool enabled)
        {
            if (_highlightParticles == null) return;

            if (enabled)
            {
                _highlightParticles.Play();
            }
            else
            {
                _highlightParticles.Stop();
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

            if (_highlightEffect != null)
            {
                Destroy(_highlightEffect);
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