using infrastructure.factories.blocks;
using infrastructure.factories.enemies;
using infrastructure.factories.towers;
using infrastructure.services.abilityService;
using infrastructure.services.combatService;
using infrastructure.services.effectService;
using infrastructure.services.gameStateService;
using infrastructure.services.inputService;
using infrastructure.services.pathService;
using infrastructure.services.playerService;
using infrastructure.services.towerService;
using infrastructure.services.waveService;
using level.builder;
using level.path;
using Zenject;

namespace infrastructure.installers
{
    public class SceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Bind simple dependencies first (no constructor dependencies)
            BindInputActions();
            BindPathService();
            BindTowerService();
            BindPlayerService();
            BindCombatService();

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

            // Bind GameStateService (depends on IPathService, IWaveService, ILevelBuilder)
            BindGameStateService();

            // Bind PathDrawer (depends on IPathService, IInputService)
            BindPathDrawer();

            // Initialize circular dependency: LevelBuilder needs IGameStateService
            InitializeCircularDependencies();
        }

        private void InitializeCircularDependencies()
        {
            // Resolve and initialize LevelBuilder with GameStateService
            var levelBuilder = Container.Resolve<ILevelBuilder>();
            var gameStateService = Container.Resolve<IGameStateService>();
            levelBuilder.Initialize(gameStateService);
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

        private void BindEffectService()
        {
            Container.Bind<IEffectService>().To<EffectService>().AsSingle().NonLazy();
        }

        private void BindAbilityService()
        {
            Container.Bind<IAbilityService>().To<AbilityService>().AsSingle().NonLazy();
        }
    }
}
