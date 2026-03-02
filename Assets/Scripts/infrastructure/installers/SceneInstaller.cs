using infrastructure.factories.blocks;
using infrastructure.factories.enemies;
using infrastructure.factories.towers;
using infrastructure.services.abilityService;
using infrastructure.services.combatService;
using infrastructure.services.effectService;
using infrastructure.services.gameStateService;
using infrastructure.services.projectileService;
using infrastructure.services.inputService;
using infrastructure.services.pathService;
using infrastructure.services.playerService;
using infrastructure.services.saveService;
using infrastructure.services.playerSkillService;
using infrastructure.services.selectionService;
using infrastructure.services.timerService;
using infrastructure.services.towerService;
using infrastructure.services.waveService;
using skills;
using level.builder;
using level.path;
using ui.skillSelection;
using UnityEngine;
using Zenject;

namespace infrastructure.installers
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] private SkillSelectionView _skillSelectionView;
        public override void InstallBindings()
        {
            Pool.Initialize(Container);

            // Bind simple dependencies first (no constructor dependencies)
            BindInputActions();
            BindPathService();
            BindTowerService();
            BindPlayerService();
            BindCombatService();
            BindProjectileService();
            BindTimerService();
            BindSelectionService();
            BindPlayerSkillService();

            // Bind Effect and Ability services
            BindEffectService();      // Depends on: IUpdateService (from Bootstrap), ICombatService
            BindAbilityService();     // Depends on: IEffectService, ICombatService, IUpdateService

            // Bind factories (depend only on IResourceProvider from Bootstrap)
            BindBlockFactory();
            BindTowerFactory();
            BindEnemyFactory();

            // Bind InputService (depends on InputActions)
            BindInputService();

            // Bind LevelBuilder WITHOUT .NonLazy() to prevent immediate instantiation
            BindLevelBuilder();

            // Bind WaveService (depends on IEnemyFactory, ILevelBuilder, IPlayerService)
            BindWaveService();

            // Bind SaveService (depends on IPlayerService, IWaveService, ILevelBuilder)
            BindSaveService();

            // Bind GameStateService (depends on IPathService, IWaveService, ILevelBuilder)
            BindGameStateService();

            // Bind PathDrawer (depends on IPathService, IInputService)
            BindPathDrawer();

            // Bind RangeCircleManager (depends on ISelectionService)
            BindRangeCircleManager();

            // Initialize circular dependency: LevelBuilder needs IGameStateService
            InitializeCircularDependencies();
        }

        private void InitializeCircularDependencies()
        {
            // Resolve and initialize LevelBuilder with GameStateService
            var levelBuilder = Container.Resolve<ILevelBuilder>();
            var gameStateService = Container.Resolve<IGameStateService>();
            var saveService = Container.Resolve<ISaveService>();
            var playerService = Container.Resolve<IPlayerService>();
            var waveService = Container.Resolve<IWaveService>();
            var pathService = Container.Resolve<IPathService>();
            var playerSkillService = Container.Resolve<IPlayerSkillService>() as PlayerSkillService;

            gameStateService.Initialize(levelBuilder);
            waveService.Initialize(levelBuilder);
            levelBuilder.Initialize(gameStateService, Container.Resolve<IPlayerSkillService>());
            playerSkillService?.Initialize(levelBuilder);

            // Automatically build the map when scene loads
            levelBuilder.Build();

            // Setup auto-save: subscribe to wave completion event
            waveService.OnWaveComplete += () =>
            {
                UnityEngine.Debug.Log("Wave complete - triggering auto-save");
                saveService.SaveGame();
            };

            // Load all skills and set them as available for selection
            var skillService = Container.Resolve<IPlayerSkillService>();
            var allSkills = UnityEngine.Resources.LoadAll<PlayerSkillData>("ScriptableObjects/Skills");
            skillService.SetAvailableSkills(new System.Collections.Generic.List<PlayerSkillData>(allSkills));

            // Try to load saved game
            if (saveService.TryLoadGame(out GameSaveData saveData))
            {
                UnityEngine.Debug.Log($"Loading saved game: Wave {saveData.lastCompletedWave}");

                // Restore player data
                playerService.LoadPlayerData(
                    saveData.playerData.playerLevel,
                    saveData.playerData.currentXP,
                    saveData.playerData.lives,
                    saveData.playerData.gold
                );

                // Restore wave number
                waveService.SetWaveNumber(saveData.lastCompletedWave);

                // Restore towers
                if (saveData.placedTowers != null)
                {
                    foreach (var towerData in saveData.placedTowers)
                    {
                        // Validate grid bounds
                        if (towerData.gridX >= 0 && towerData.gridX < levelBuilder.MapData.Width &&
                            towerData.gridY >= 0 && towerData.gridY < levelBuilder.MapData.Height)
                        {
                            levelBuilder.RestoreTower(towerData.towerType, towerData.level, towerData.gridX, towerData.gridY);
                        }
                        else
                        {
                            UnityEngine.Debug.LogWarning($"Invalid tower position in save data: ({towerData.gridX}, {towerData.gridY})");
                        }
                    }

                    // Validate path after restoring all towers
                    var path = pathService.FindPath(levelBuilder.MapData, levelBuilder.TowerMap);
                    if (path == null)
                    {
                        UnityEngine.Debug.LogError("Loaded tower configuration blocks enemy path! Starting fresh game.");
                        saveService.DeleteSave();
                        // Towers are already placed, but game will be unplayable - should reload scene ideally
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"Game loaded successfully: {saveData.placedTowers.Count} towers restored");
                    }
                }

                // Save game found: auto-equip first 4 skills and skip skill selection
                for (int i = 0; i < UnityEngine.Mathf.Min(4, allSkills.Length); i++)
                    skillService.EquipSkill(i, allSkills[i], 0);
                gameStateService.StartGame();
            }
            else
            {
                UnityEngine.Debug.Log("No save found, starting new game with skill selection");
                // Activate the skill selection screen — its Start() will populate skills
                _skillSelectionView?.Show();
            }
        }

        private void BindPathDrawer()
        {
            Container.Bind<IPathDrawer>().To<PathDrawer>().AsSingle().NonLazy();
        }

        private void BindPathService()
        {
            Container.Bind<IPathService>().To<PathService>().AsSingle().NonLazy();
        }

        private void BindTowerService()
        {
            Container.Bind<ITowerService>().To<TowerService>().AsSingle();
        }

        private void BindTowerFactory()
        {
            Container.Bind<ITowerFactory>().To<TowerFactory>().AsSingle();
        }

        private void BindInputService()
        {
            Container.Bind<IInputService>().To<InputService>().AsSingle().NonLazy();
        }

        private void BindInputActions()
        {
            Container.Bind<InputActions>().To<InputActions>().AsSingle();
        }

        private void BindLevelBuilder()
        {
            // Don't use NonLazy() - will be manually resolved after all bindings
            Container.Bind<ILevelBuilder>().To<LevelBuilder>().AsSingle();
        }

        private void BindBlockFactory()
        {
            Container.Bind<IBlockFactory>().To<BlockFactory>().AsSingle();
        }

        private void BindEnemyFactory()
        {
            Container.Bind<IEnemyFactory>().To<EnemyFactory>().AsSingle();
        }

        private void BindGameStateService()
        {
            Container.Bind<IGameStateService>().To<GameStateService>().AsSingle().NonLazy();
        }

        private void BindWaveService()
        {
            Container.Bind<IWaveService>().To<WaveService>().AsSingle().NonLazy();
        }

        private void BindCombatService()
        {
            Container.Bind<ICombatService>().To<CombatService>().AsSingle();
        }

        private void BindPlayerService()
        {
            Container.Bind<IPlayerService>().To<PlayerService>().AsSingle().NonLazy();
        }

        private void BindProjectileService()
        {
            Container.Bind<IProjectileService>().To<ProjectileService>().AsSingle();
        }

        private void BindEffectService()
        {
            Container.Bind<IEffectService>().To<EffectService>().AsSingle().NonLazy();
        }

        private void BindAbilityService()
        {
            Container.Bind<IAbilityService>().To<AbilityService>().AsSingle().NonLazy();
        }

        private void BindTimerService()
        {
            Container.Bind<ITimerService>().To<TimerService>().AsSingle().NonLazy();
        }

        private void BindSaveService()
        {
            Container.Bind<ISaveService>().To<SaveService>().AsSingle();
        }

        private void BindSelectionService()
        {
            Container.Bind<ISelectionService>().To<SelectionService>().AsSingle().NonLazy();
        }

        private void BindPlayerSkillService()
        {
            Container.Bind<IPlayerSkillService>().To<PlayerSkillService>().AsSingle();
        }

        private void BindRangeCircleManager()
        {
            Container.BindInterfacesTo<RangeCircleManager>().AsSingle().NonLazy();
        }
    }
}
