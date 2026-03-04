using System.Collections.Generic;
using System.Linq;
using infrastructure.services.gameStateService;
using level.builder;
using towers;
using UnityEngine;
using Zenject;

namespace ui.combinations
{
    public class CombinationsReferencePanel : Popup
    {
        private const string RecipesPath = "ScriptableObjects/Combinations";

        [SerializeField] private Transform _recipesContainer;
        [SerializeField] private GameObject _recipeRowPrefab;
        [SerializeField] private GameObject _categoryHeaderPrefab;

        private ILevelBuilder _levelBuilder;
        private IGameStateService _gameStateService;
        private CombinationRecipe[] _allRecipes;
        private readonly List<GameObject> _activeItems = new List<GameObject>();

        [Inject]
        public void Construct(ILevelBuilder levelBuilder, IGameStateService gameStateService)
        {
            _levelBuilder = levelBuilder;
            _gameStateService = gameStateService;
            Hide();
        }

        public void Toggle()
        {
            if (gameObject.activeSelf)
                Hide();
            else
                Show();
        }

        public override void Show()
        {
            base.Show();
            Refresh();
        }
        
        private void Refresh()
        {
            try
            {
                for (int i = _activeItems.Count - 1; i >= 0; i--)
                    if (_activeItems[i] != null) Destroy(_activeItems[i]);
                _activeItems.Clear();

                if (_allRecipes == null)
                {
                    _allRecipes = Resources.LoadAll<CombinationRecipe>(RecipesPath);
                    if (_allRecipes != null && _allRecipes.Length > 0)
                        System.Array.Sort(_allRecipes, (a, b) => ((int)a.ResultType).CompareTo((int)b.ResultType));
                }

                Debug.Log($"[CombPanel] Refresh: recipes={_allRecipes?.Length}, rowPrefab={_recipeRowPrefab != null}, container={_recipesContainer != null}, levelBuilder={_levelBuilder != null}");

                if (_allRecipes == null || _allRecipes.Length == 0) { Debug.LogWarning("[CombPanel] No recipes loaded!"); return; }
                if (_recipeRowPrefab == null) { Debug.LogWarning("[CombPanel] _recipeRowPrefab is null!"); return; }
                if (_recipesContainer == null) { Debug.LogWarning("[CombPanel] _recipesContainer is null!"); return; }

                List<Tower> permanentTowers;
                List<Tower> newlyPlacedTowers;
                if (_levelBuilder != null)
                {
                    var allTowers = _levelBuilder.GetAllTowers()
                        .Where(t => t != null && t.enabled && t.TowerData != null)
                        .ToList();
                    var placedThisRound = _gameStateService?.GetPlacedThisRound() ?? (IReadOnlyList<Tower>)new List<Tower>();
                    permanentTowers  = allTowers.Where(t => !placedThisRound.Contains(t)).ToList();
                    newlyPlacedTowers = allTowers.Where(t =>  placedThisRound.Contains(t)).ToList();
                }
                else
                {
                    permanentTowers  = new List<Tower>();
                    newlyPlacedTowers = new List<Tower>();
                }

                var lastCategory = -1;

                foreach (var recipe in _allRecipes)
                {
                    if (recipe == null || recipe.Ingredients == null) { Debug.LogWarning("[CombPanel] null recipe or ingredients!"); continue; }
                    int category = GetCategory((int)recipe.ResultType);
                    if (category != lastCategory)
                    {
                        lastCategory = category;
                        SpawnCategoryHeader(GetCategoryName(category));
                    }
                    SpawnRecipeRow(recipe, permanentTowers, newlyPlacedTowers);
                }

                Debug.Log($"[CombPanel] Done: spawned {_activeItems.Count} items");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[CombPanel] Exception in Refresh: {ex}");
            }
        }

        private void SpawnCategoryHeader(string categoryName)
        {
            if (_categoryHeaderPrefab == null) return;
            var obj = Instantiate(_categoryHeaderPrefab, _recipesContainer);
            var header = obj.GetComponent<CategoryHeaderUI>();
            header?.Setup(categoryName);
            _activeItems.Add(obj);
        }

        private void SpawnRecipeRow(CombinationRecipe recipe, List<Tower> permanentTowers, List<Tower> newlyPlacedTowers)
        {
            var obj = Instantiate(_recipeRowPrefab, _recipesContainer);
            var row = obj.GetComponent<RecipeRowUI>();
            if (row != null)
            {
                var presences = new IngredientPresence[3];
                for (int i = 0; i < recipe.Ingredients.Length && i < 3; i++)
                    presences[i] = GetIngredientPresence(recipe.Ingredients[i], permanentTowers, newlyPlacedTowers);
                row.Setup(recipe, presences);
            }
            _activeItems.Add(obj);
        }

        private static IngredientPresence GetIngredientPresence(TowerIngredient ingredient, List<Tower> permanentTowers, List<Tower> newlyPlacedTowers)
        {
            bool inPermanent = Matches(ingredient, permanentTowers);
            bool inNew       = Matches(ingredient, newlyPlacedTowers);
            if (inPermanent && inNew) return IngredientPresence.Both;
            if (inPermanent)          return IngredientPresence.Permanent;
            if (inNew)                return IngredientPresence.NewlyPlaced;
            return IngredientPresence.None;
        }

        private static bool Matches(TowerIngredient ingredient, List<Tower> towers)
        {
            return towers.Any(t =>
                t.TowerData.TowerType == ingredient.TowerType &&
                (ingredient.Level < 0 || t.TowerData.Level == ingredient.Level));
        }

        private static int GetCategory(int typeValue)
        {
            if (typeValue <= 14) return 0;  // Basic: 10–14
            if (typeValue <= 32) return 1;  // Intermediate: 15–32
            if (typeValue <= 39) return 2;  // Advanced: 33–39
            if (typeValue <= 46) return 3;  // Top: 40–46
            return 4;                       // Secret: 47+
        }

        private static string GetCategoryName(int category)
        {
            switch (category)
            {
                case 0: return "— Базовые —";
                case 1: return "— Промежуточные —";
                case 2: return "— Продвинутые —";
                case 3: return "— Топовые —";
                case 4: return "— Секретные —";
                default: return "—";
            }
        }
    }
}
