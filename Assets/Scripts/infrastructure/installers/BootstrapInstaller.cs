using infrastructure.services.resourceProvider;
using Zenject;
using ResourceProvider = infrastructure.services.resourceProvider.ResourceProvider;

namespace infrastructure.installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindResourceProvider();
        }

        private void BindResourceProvider()
        {
            Container.Bind<IResourceProvider>().To<ResourceProvider>().AsSingle();
        }
    }
}
