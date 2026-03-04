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
        private const string AdvancedTowerModelsPath = "Prefabs/Towers/Advanced/";
        private const string StoneModelsPath = "Prefabs/Towers/Stone";
        private const string TowerPath = "Prefabs/Towers/Tower";
        private const string TowerSettingsPath = "ScriptableObjects/Towers/TowerSettings";
        private const string TowerHighlightPath = "Prefabs/Effects/TowerHighlight";
        
        private readonly IResourceProvider _resourceProvider;
        private readonly DiContainer _diContainer;

        private List<TowerData> _towersData;
        private List<TowerModel> _basicTowerModels;
        private List<TowerModel> _advancedTowerModels;
        private TowerModel _stoneModel;
        private Tower _tower;
        private TowerSettings _towerSettings;
        private Transform _towerParent;
        private GameObject _towerHighlightPrefab;

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

            var rangeDrawer = tower.gameObject.AddComponent<RangeCircleDrawer>();
            rangeDrawer.Initialize(towerData.AttackRange);

            if (_towerHighlightPrefab != null)
            {
                tower.SetHighlightEffect(_towerHighlightPrefab);
            }

            return tower;
        }

        private void Load()
        {
            _towersData = _resourceProvider.LoadList<TowerData>(TowersDataPath);
            _basicTowerModels = _resourceProvider.LoadList<TowerModel>(BasicTowerModelsPath);
            _advancedTowerModels = _resourceProvider.LoadList<TowerModel>(AdvancedTowerModelsPath);
            _stoneModel = _resourceProvider.Load<TowerModel>(StoneModelsPath);
            _tower = _resourceProvider.Load<Tower>(TowerPath);
            _towerSettings = _resourceProvider.Load<TowerSettings>(TowerSettingsPath);
            _towerHighlightPrefab = _resourceProvider.Load<GameObject>(TowerHighlightPath);
            CreateTowerParent();
        }

        private void CreateTowerParent()
        {
            var parent = new GameObject("Towers");
            _towerParent = parent.transform;
        }

        private void CreateTowerModel(Tower tower)
        {
            if (tower == null || tower.TowerData == null)
            {
                Debug.LogError("Cannot create tower model: Tower or TowerData is null");
                return;
            }

            var modelPrefab = _basicTowerModels.FirstOrDefault(model => model.TowerType == tower.TowerData.TowerType);

            if (modelPrefab == null && _advancedTowerModels != null)
                modelPrefab = _advancedTowerModels.FirstOrDefault(model => model.TowerType == tower.TowerData.TowerType);

            if (modelPrefab == null)
            {
                Debug.LogError($"No model prefab found for tower type: {tower.TowerData.TowerType}. Check Resources/{BasicTowerModelsPath} or {AdvancedTowerModelsPath}");
                return;
            }

            var model = _diContainer.InstantiatePrefabForComponent<TowerModel>(modelPrefab);
            model.transform.SetParent(tower.ScaledObject);
            model.transform.localRotation = Quaternion.identity;

            if (tower.TowerData.TowerType.IsBasicTower() && _towerSettings != null && _towerSettings.ScaleFromLevel != null &&
                tower.TowerData.Level < _towerSettings.ScaleFromLevel.Count)
            {
                tower.ScaledObject.localScale = Vector3.one * _towerSettings.ScaleFromLevel[tower.TowerData.Level];
            }
            else
            {
                tower.ScaledObject.localScale = Vector3.one;
            }
        }

        public void ReplaceWithStoneModel(Tower tower)
        {
            foreach (Transform child in tower.ScaledObject)
            {
                Object.Destroy(child.gameObject);
            }
            
            tower.ScaledObject.localScale = Vector3.one;

            var stoneModel = _diContainer.InstantiatePrefabForComponent<TowerModel>(_stoneModel);
            stoneModel.transform.SetParent(tower.ScaledObject);
            stoneModel.transform.localPosition = Vector3.zero;
            stoneModel.transform.localRotation = Quaternion.identity;
        }
    }
}