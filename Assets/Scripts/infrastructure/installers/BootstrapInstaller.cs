using infrastructure.services.resourceProvider;
using infrastructure.services.updateService;
using UnityEngine;
using Zenject;
using ResourceProvider = infrastructure.services.resourceProvider.ResourceProvider;

namespace infrastructure.installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private UpdateService _updateService;
        
        public override void InstallBindings()
        {
            BindResourceProvider();
            BindUpdateService();
        }

        private void BindUpdateService()
        {
            Container.Bind<IUpdateService>().FromInstance(_updateService).AsSingle();
        }

        private void BindResourceProvider()
        {
            Container.Bind<IResourceProvider>().To<ResourceProvider>().AsSingle();
        }
    }
}
