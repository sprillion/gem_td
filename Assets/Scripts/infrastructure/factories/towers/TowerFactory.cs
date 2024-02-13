using System.Collections.Generic;
using System.Linq;
using infrastructure.services.resourceProvider;
using towers;
using UnityEngine;
using Zenject;

namespace infrastructure.factories.towers
{
    public class TowerFactory : ITowerFactory
    {
        private const string TowersDataPath = "ScriptableObjects/Towers/";
        private const string BasicTowerModelsPath = "Prefabs/Towers/Basic/";
        private const string StoneModelsPath = "Prefabs/Towers/Stone";
        private const string TowerPath = "Prefabs/Towers/Tower";
        private const string TowerSettingsPath = "ScriptableObjects/Towers/TowerSettings";
        
        private readonly IResourceProvider _resourceProvider;
        private readonly DiContainer _diContainer;

        private List<TowerData> _towersData;
        private List<TowerModel> _basicTowerModels;
        private TowerModel _stoneModel;
        private Tower _tower;
        private TowerSettings _towerSettings;
        private Transform _towerParent;

        public TowerFactory(IResourceProvider resourceProvider, DiContainer diContainer)
        {
            _resourceProvider = resourceProvider;
            _diContainer = diContainer;
            Load();
        }

        public Tower CreateTower(TowerType towerType, int towerLevel)
        {
            var tower = _diContainer.InstantiatePrefabForComponent<Tower>(_tower);
            tower.transform.SetParent(_towerParent);
            tower.name = $"{towerType}{towerLevel + 1}";
            var towerData = _towersData.FirstOrDefault(data => data.TowerType == towerType && data.Level == towerLevel);
            tower.Initialize(towerData);
            CreateTowerModel(tower);
            return tower;
        }

        private void Load()
        {
            _towersData = _resourceProvider.LoadList<TowerData>(TowersDataPath);
            _basicTowerModels = _resourceProvider.LoadList<TowerModel>(BasicTowerModelsPath);
            _stoneModel = _resourceProvider.Load<TowerModel>(StoneModelsPath);
            _tower = _resourceProvider.Load<Tower>(TowerPath);
            _towerSettings = _resourceProvider.Load<TowerSettings>(TowerSettingsPath);
            CreateTowerParent();
        }

        private void CreateTowerParent()
        {
            var parent = new GameObject("Towers");
            _towerParent = parent.transform;
        }

        private void CreateTowerModel(Tower tower)
        {
            var modelPrefab = _basicTowerModels.FirstOrDefault(model => model.TowerType == tower.TowerData.TowerType);
            var model = _diContainer.InstantiatePrefabForComponent<TowerModel>(modelPrefab);
            model.transform.SetParent(tower.ScaledObject);
            tower.ScaledObject.localScale = Vector3.one * _towerSettings.ScaleFromLevel[tower.TowerData.Level];

        }
    }
}