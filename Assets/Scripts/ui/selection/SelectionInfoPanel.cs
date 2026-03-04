using System.Collections.Generic;
using enemies;
using infrastructure.services.combinationService;
using infrastructure.services.effectService;
using infrastructure.services.gameStateService;
using infrastructure.services.selectionService;
using infrastructure.services.updateService;
using level.builder;
using TMPro;
using towers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ui.selection
{
    public class SelectionInfoPanel : MonoBehaviour
    {
        [Header("Main Panels")]
        [SerializeField] private GameObject _towerPanel;
        [SerializeField] private GameObject _enemyPanel;
        [SerializeField] private GameObject _emptyPanel;

        [Header("Tower Info")]
        [SerializeField] private Image _towerIcon;
        [SerializeField] private TMP_Text _towerName;
        [SerializeField] private TMP_Text _towerLevel;
        [SerializeField] private TMP_Text _towerDamage;
        [SerializeField] private TMP_Text _towerAttackSpeed;
        [SerializeField] private TMP_Text _towerAttackRange;
        [SerializeField] private Transform _abilitiesContainer;
        [SerializeField] private GameObject _abilityItemPrefab;

        [Header("Select Tower Button")]
        [SerializeField] private Button _selectTowerButton;

        [Header("Delete Button")]
        [SerializeField] private Button _deleteTowerButton;

        [Header("Combinations")]
        [SerializeField] private Transform _combinationsContainer;
        [SerializeField] private GameObject _combinationButtonPrefab;

        [Header("Enemy Info")]
        [SerializeField] private Image _enemyIcon;
        [SerializeField] private TMP_Text _enemyName;
        [SerializeField] private Slider _enemyHealthBar;
        [SerializeField] private TMP_Text _enemyHealthText;
        [SerializeField] private TMP_Text _enemyDamage;
        [SerializeField] private TMP_Text _enemyMoveSpeed;
        [SerializeField] private TMP_Text _enemyArmor;
        [SerializeField] private TMP_Text _enemyMagicResist;
        [SerializeField] private TMP_Text _enemyEvasion;
        [SerializeField] private Transform _effectsContainer;
        [SerializeField] private GameObject _effectItemPrefab;

        private ISelectionService _selectionService;
        private IEffectService _effectService;
        private IUpdateService _updateService;
        private IGameStateService _gameStateService;
        private ICombinationService _combinationService;
        private ILevelBuilder _levelBuilder;

        private readonly List<GameObject> _activeAbilityItems = new List<GameObject>();
        private readonly List<GameObject> _activeEffectItems = new List<GameObject>();
        private readonly List<GameObject> _activeCombinationButtons = new List<GameObject>();

        private Tower _currentTower;

        [Inject]
        public void Construct(ISelectionService selectionService, IEffectService effectService,
            IUpdateService updateService, IGameStateService gameStateService,
            ICombinationService combinationService, ILevelBuilder levelBuilder)
        {
            _selectionService = selectionService;
            _effectService = effectService;
            _updateService = updateService;
            _gameStateService = gameStateService;
            _combinationService = combinationService;
            _levelBuilder = levelBuilder;

            _selectionService.OnTowerSelected += HandleTowerSelected;
            _selectionService.OnEnemySelected += HandleEnemySelected;
            _selectionService.OnSelectionCleared += HandleSelectionCleared;
            _updateService.OnUpdate += OnUpdate;

            if (_selectTowerButton != null)
            {
                _selectTowerButton.onClick.AddListener(OnSelectTowerClicked);
            }

            if (_deleteTowerButton != null)
            {
                _deleteTowerButton.onClick.AddListener(OnDeleteTowerClicked);
            }

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_selectionService != null)
            {
                _selectionService.OnTowerSelected -= HandleTowerSelected;
                _selectionService.OnEnemySelected -= HandleEnemySelected;
                _selectionService.OnSelectionCleared -= HandleSelectionCleared;
            }

            if (_updateService != null)
            {
                _updateService.OnUpdate -= OnUpdate;
            }

            if (_selectTowerButton != null)
            {
                _selectTowerButton.onClick.RemoveListener(OnSelectTowerClicked);
            }

            if (_deleteTowerButton != null)
            {
                _deleteTowerButton.onClick.RemoveListener(OnDeleteTowerClicked);
            }
        }

        private void OnUpdate()
        {
            if (_selectionService.CurrentSelectionType == SelectionType.Enemy && _selectionService.SelectedEnemy != null)
            {
                UpdateEnemyHealth();
                UpdateEnemyEffects();
            }
        }

        private void HandleTowerSelected(Tower tower)
        {
            gameObject.SetActive(true);
            _currentTower = tower;
            ShowTowerPanel(tower);
            // Stone towers (disabled) don't participate in combinations
            if (tower.enabled)
                UpdateCombinationButtons(tower);
            else
                ClearCombinationButtons();
        }

        private void HandleEnemySelected(Enemy enemy)
        {
            gameObject.SetActive(true);
            _currentTower = null;
            ShowEnemyPanel(enemy);
        }

        private void HandleSelectionCleared()
        {
            _currentTower = null;
            ClearCombinationButtons();
            gameObject.SetActive(false);
        }

        private void OnSelectTowerClicked()
        {
            if (_currentTower == null) return;
            if (_gameStateService.CurrentPhase != GamePhase.SELECTING_TOWER) return;
            if (!_gameStateService.IsPlacedThisRound(_currentTower)) return;

            _gameStateService.SelectTower(_currentTower);
            _selectionService.ClearSelection();
        }

        private void ShowTowerPanel(Tower tower)
        {
            if (tower == null) return;

            _towerPanel.SetActive(true);
            _enemyPanel.SetActive(false);
            if (_emptyPanel != null) _emptyPanel.SetActive(false);

            PopulateTowerInfo(tower);
        }

        private void ShowEnemyPanel(Enemy enemy)
        {
            if (enemy == null || enemy.EnemyData == null) return;

            _towerPanel.SetActive(false);
            _enemyPanel.SetActive(true);
            if (_emptyPanel != null) _emptyPanel.SetActive(false);

            PopulateEnemyInfo(enemy);
        }

        private void PopulateTowerInfo(Tower tower)
        {
            bool isStone = !tower.enabled;

            if (_towerIcon != null)
            {
                var icon = tower.TowerData?.Icon;
                _towerIcon.sprite = icon;
                _towerIcon.enabled = !isStone && icon != null;
            }

            if (_towerName != null)
                _towerName.text = isStone ? "Камень" : GetTowerDisplayName(tower.TowerData.TowerType);

            if (_towerLevel != null)
                _towerLevel.text = isStone ? "" : $"Уровень {tower.TowerData.Level + 1}";

            if (_towerDamage != null)
                _towerDamage.text = isStone ? "" : $"Урон: {tower.TowerData.Damage}";

            if (_towerAttackSpeed != null)
                _towerAttackSpeed.text = isStone ? "" : $"Скорость атаки: {tower.TowerData.AttackSpeed}";

            if (_towerAttackRange != null)
                _towerAttackRange.text = isStone ? "" : $"Дальность: {tower.TowerData.AttackRange:F1}";

            ClearAbilityItems();
            if (!isStone && tower.TowerData?.Abilities != null)
            {
                foreach (var ability in tower.TowerData.Abilities)
                    CreateAbilityItem(ability, tower.TowerData.Level);
            }

            UpdateSelectButton(tower);
            UpdateDeleteButton();
        }

        private void UpdateSelectButton(Tower tower)
        {
            if (_selectTowerButton == null) return;

            bool showButton = _gameStateService.CurrentPhase == GamePhase.SELECTING_TOWER
                              && _gameStateService.IsPlacedThisRound(tower);
            _selectTowerButton.gameObject.SetActive(showButton);
        }

        private void PopulateEnemyInfo(Enemy enemy)
        {
            var data = enemy.EnemyData;

            if (_enemyIcon != null && data.Icon != null)
            {
                _enemyIcon.sprite = data.Icon;
                _enemyIcon.enabled = true;
            }
            else if (_enemyIcon != null)
            {
                _enemyIcon.enabled = false;
            }

            if (_enemyName != null)
            {
                _enemyName.text = enemy.name;
            }

            UpdateEnemyHealth();

            if (_enemyDamage != null)
            {
                _enemyDamage.text = $"Урон: {data.Damage}";
            }

            if (_enemyMoveSpeed != null)
            {
                _enemyMoveSpeed.text = $"Скорость: {enemy.CurrentMoveSpeed:F1}";
            }

            if (_enemyArmor != null)
            {
                _enemyArmor.text = $"Броня: {enemy.CurrentArmor}";
            }

            if (_enemyMagicResist != null)
            {
                _enemyMagicResist.text = $"Сопротивление магии: {data.MagicResist}";
            }

            if (_enemyEvasion != null)
            {
                _enemyEvasion.text = $"Уклонение: {data.Evasion}%";
            }

            UpdateEnemyEffects();
        }

        private void UpdateEnemyHealth()
        {
            var enemy = _selectionService.SelectedEnemy;
            if (enemy == null || enemy.EnemyData == null) return;

            if (_enemyHealthBar != null)
            {
                _enemyHealthBar.maxValue = enemy.EnemyData.Health;
                _enemyHealthBar.value = enemy.CurrentHealth;
            }

            if (_enemyHealthText != null)
            {
                _enemyHealthText.text = $"{enemy.CurrentHealth} / {enemy.EnemyData.Health}";
            }
        }

        private void UpdateEnemyEffects()
        {
            var enemy = _selectionService.SelectedEnemy;
            if (enemy == null) return;

            ClearEffectItems();

            var effects = _effectService.GetActiveEffects(enemy);
            if (effects != null && effects.Count > 0)
            {
                foreach (var effect in effects)
                {
                    CreateEffectItem(effect);
                }
            }
        }

        private void CreateAbilityItem(towers.abilities.AbilityData ability, int towerLevel)
        {
            if (_abilityItemPrefab == null || _abilitiesContainer == null) return;

            var itemObj = Instantiate(_abilityItemPrefab, _abilitiesContainer);
            var itemUI = itemObj.GetComponent<AbilityItemUI>();
            if (itemUI != null)
            {
                itemUI.Setup(ability, towerLevel);
            }

            _activeAbilityItems.Add(itemObj);
        }

        private void CreateEffectItem(towers.abilities.effects.Effect effect)
        {
            if (_effectItemPrefab == null || _effectsContainer == null) return;

            var itemObj = Instantiate(_effectItemPrefab, _effectsContainer);
            var itemUI = itemObj.GetComponent<EffectItemUI>();
            if (itemUI != null)
            {
                itemUI.Setup(effect);
            }

            _activeEffectItems.Add(itemObj);
        }

        private void ClearAbilityItems()
        {
            foreach (var item in _activeAbilityItems)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }
            _activeAbilityItems.Clear();
        }

        private void ClearEffectItems()
        {
            foreach (var item in _activeEffectItems)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }
            _activeEffectItems.Clear();
        }

        private void OnDeleteTowerClicked()
        {
            if (_currentTower == null) return;
            var tower = _currentTower;
            // Remove from placed-this-round list if applicable (safe to call even if not in list)
            _gameStateService.RemovePlacedThisRound(tower);
            _selectionService.ClearSelection();
            _levelBuilder.DeleteTower(tower);
        }

        private void UpdateDeleteButton()
        {
            if (_deleteTowerButton == null) return;
            // Hide delete for newly placed towers during selection phase
            bool isPlacedThisRound = _gameStateService.IsPlacedThisRound(_currentTower);
            _deleteTowerButton.gameObject.SetActive(_currentTower != null && !isPlacedThisRound);
        }

        private void UpdateCombinationButtons(Tower tower)
        {
            ClearCombinationButtons();
            if (_combinationService == null || _combinationButtonPrefab == null || _combinationsContainer == null)
                return;

            var available = _combinationService.GetAvailableCombinations(tower);
            foreach (var (recipe, ingredients) in available)
            {
                var itemObj = Instantiate(_combinationButtonPrefab, _combinationsContainer);
                var itemUI = itemObj.GetComponent<CombinationButtonUI>();
                if (itemUI != null)
                    itemUI.Setup(recipe, ingredients, tower, _combinationService);
                _activeCombinationButtons.Add(itemObj);
            }
        }

        private void ClearCombinationButtons()
        {
            foreach (var item in _activeCombinationButtons)
            {
                if (item != null)
                    Destroy(item);
            }
            _activeCombinationButtons.Clear();
        }

        private string GetTowerDisplayName(TowerType towerType)
        {
            switch (towerType)
            {
                case TowerType.P: return "Ломатель брони (P)";
                case TowerType.Q: return "Скорострел (Q)";
                case TowerType.D: return "Мощный (D)";
                case TowerType.G: return "Ядовитый (G)";
                case TowerType.E: return "Помощник (E)";
                case TowerType.R: return "Сплеш башня (R)";
                case TowerType.B: return "Ледяной (B)";
                case TowerType.Y: return "Мультивыстрел (Y)";
                case TowerType.Stone: return "Камень";
                // Basic combinations
                case TowerType.Silver: return "Серебро";
                case TowerType.Malachite: return "Малахит";
                case TowerType.AsteriatedRuby: return "Астерированный рубин";
                case TowerType.Jade: return "Нефрит";
                case TowerType.Quartz: return "Кварц";
                // Intermediate
                case TowerType.SilverKnight: return "Серебряный рыцарь";
                case TowerType.PinkDiamond: return "Розовый бриллиант";
                case TowerType.VividMalachite: return "Яркий малахит";
                case TowerType.Uranium238: return "Уран-238";
                case TowerType.Volcano: return "Вулкан";
                case TowerType.Bloodstone: return "Кровавый камень";
                case TowerType.GreyJade: return "Серый нефрит";
                case TowerType.Gold: return "Золото";
                case TowerType.DarkEmerald: return "Тёмный изумруд";
                case TowerType.ParaibaTourmaline: return "Параибский турмалин";
                case TowerType.DeepseaPearl: return "Глубоководная жемчужина";
                case TowerType.ChrysoBerylCatsEye: return "Хризоберилл-кошачий глаз";
                case TowerType.YellowSaphire: return "Жёлтый сапфир";
                case TowerType.MonkeyKingJade: return "Нефрит царя обезьян";
                case TowerType.EgyptGold: return "Египетское золото";
                case TowerType.LuckyChinese: return "Китайская удача";
                case TowerType.CharmingLazurite: return "Чарующий лазурит";
                case TowerType.NorthernSabersEye: return "Глаз северной сабли";
                // Advanced
                case TowerType.HugePinkDiamond: return "Огромный розовый бриллиант";
                case TowerType.Uranium235: return "Уран-235";
                case TowerType.AntiqueBloodstone: return "Античный кровавый камень";
                case TowerType.EmeraldGolem: return "Изумрудный голем";
                case TowerType.ElaboratelyCarvedTourmaline: return "Резной турмалин";
                case TowerType.RedCoral: return "Красный коралл";
                case TowerType.DiamondCullinan: return "Алмаз Куллинан";
                // Top
                case TowerType.KohINoorDiamond: return "Кохинур";
                case TowerType.DepletedKyparium: return "Обеднённый кипарий";
                case TowerType.TheCrownPrince: return "Принц-наследник";
                case TowerType.GoldenJubilee: return "Золотой юбилей";
                case TowerType.CarmenLucia: return "Кармен Лусия";
                case TowerType.StarSapphire: return "Звёздный сапфир";
                case TowerType.SapphireStarOfAdam: return "Сапфировая звезда Адама";
                // Secret
                case TowerType.Agate: return "Агат";
                case TowerType.Obsidian: return "Обсидиан";
                case TowerType.FantasticMissShrimp: return "Фантастическая мисс Креветка";
                default: return towerType.ToString();
            }
        }
    }
}
