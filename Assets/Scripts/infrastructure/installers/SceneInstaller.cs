﻿using infrastructure.factories.blocks;
using infrastructure.factories.towers;
using infrastructure.services.inputService;
using infrastructure.services.towerService;
using level.builder;
using Zenject;

namespace infrastructure.installers
{
    public class SceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindInputService();
            BindTowerFactory();
            BindTowerService();
            BindBlockFactory();
            BindLevelBuilder();
            BindInputActions();
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
            Container.Bind<ILevelBuilder>().To<LevelBuilder>().AsSingle().NonLazy();
        }

        private void BindBlockFactory()
        {
            Container.Bind<IBlockFactory>().To<BlockFactory>().AsSingle();
        }
    }
}
