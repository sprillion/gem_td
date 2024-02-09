using infrastructure.factories.blocks;
using level.builder;
using Zenject;

namespace infrastructure.installers
{
    public class SceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindBlockFactory();
            BindLevelBuilder();
        }

        private void BindLevelBuilder()
        {
            Container.Bind<ILevelBuilder>().To<LevelBuilder>().AsSingle().NonLazy();
        }

        private void BindBlockFactory()
        {
            Container.Bind<IBlockFactory>().To<BlockFactory>().AsSingle();
        }
    }
}
