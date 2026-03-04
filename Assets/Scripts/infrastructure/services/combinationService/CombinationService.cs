using System.Collections.Generic;
using System.Linq;
using infrastructure.factories.towers;
using infrastructure.services.gameStateService;
using infrastructure.services.selectionService;
using level.builder;
using towers;
using UnityEngine;
using Zenject;

namespace infrastructure.services.combinationService
{
    public class CombinationService : ICombinationService
    {
        private const string RecipesPath = "ScriptableObjects/Combinations/";

        private readonly ILevelBuilder _levelBuilder;
        private readonly IGameStateService _gameStateService;
        private readonly ITowerFactory _towerFactory;
        private readonly ISelectionService _selectionService;

        private List<CombinationRecipe> _recipes;

        [Inject]
        public CombinationService(
            ILevelBuilder levelBuilder,
            IGameStateService gameStateService,
            ITowerFactory towerFactory,
            ISelectionService selectionService)
        {
            _levelBuilder = levelBuilder;
            _gameStateService = gameStateService;
            _towerFactory = towerFactory;
            _selectionService = selectionService;
        }

        public List<(CombinationRecipe recipe, List<Tower> ingredients)> GetAvailableCombinations(Tower selectedTower)
        {
            var result = new List<(CombinationRecipe, List<Tower>)>();

            if (selectedTower == null || selectedTower.TowerData == null) return result;

            EnsureRecipesLoaded();
            if (_recipes == null || _recipes.Count == 0) return result;

            var pool = BuildEligiblePool(selectedTower);

            foreach (var recipe in _recipes)
            {
                if (recipe.Ingredients == null || recipe.Ingredients.Length != 3) continue;

                var match = TryMatchRecipe(recipe, selectedTower, pool);
                if (match != null)
                    result.Add((recipe, match));
            }

            return result;
        }

        public void ApplyCombination(Tower sourceTower, CombinationRecipe recipe, List<Tower> ingredients)
        {
            if (sourceTower == null || recipe == null || ingredients == null || ingredients.Count < 3)
            {
                Debug.LogError("ApplyCombination: invalid arguments");
                return;
            }

            // The other 2 ingredients (not the source tower)
            var otherIngredients = ingredients.Where(t => t != sourceTower).ToList();
            if (otherIngredients.Count != 2)
            {
                Debug.LogError("ApplyCombination: expected exactly 2 non-source ingredients");
                return;
            }

            // Remove other ingredients from placed-this-round tracking and convert to stone
            foreach (var ingredient in otherIngredients)
            {
                _gameStateService.RemovePlacedThisRound(ingredient);
                _gameStateService.ConvertTowerToStone(ingredient);
            }

            // Replace source tower with combined tower at same position
            var combined = _levelBuilder.CreateCombinedTower(sourceTower, recipe.ResultType, recipe.ResultLevel);

            if (combined == null)
            {
                Debug.LogError("ApplyCombination: failed to create combined tower");
                return;
            }

            // Handle phase-specific logic
            if (_gameStateService.CurrentPhase == GamePhase.SELECTING_TOWER)
            {
                // During selection: this combined tower IS the selected one
                // RemovePlacedThisRound for sourceTower (it's being replaced)
                _gameStateService.RemovePlacedThisRound(sourceTower);
                _gameStateService.SelectCombinedTower(combined);
            }

            _selectionService.ClearSelection();
        }

        private void EnsureRecipesLoaded()
        {
            if (_recipes != null) return;

            var loaded = Resources.LoadAll<CombinationRecipe>(RecipesPath);
            _recipes = loaded != null ? new List<CombinationRecipe>(loaded) : new List<CombinationRecipe>();

            Debug.Log($"CombinationService: loaded {_recipes.Count} recipes from {RecipesPath}");
        }

        private List<Tower> BuildEligiblePool(Tower selectedTower)
        {
            var placedThisRound = _gameStateService.GetPlacedThisRound();

            if (placedThisRound.Contains(selectedTower))
            {
                // Placed-this-round towers can only combine with each other
                // Filter out any that have already been converted to stone
                return new List<Tower>(placedThisRound.Where(t => t != null && t.enabled));
            }
            else
            {
                // Old towers combine with other old towers (not newly placed)
                // tower.enabled == false means it was converted to stone
                var allTowers = _levelBuilder.GetAllTowers();
                return allTowers
                    .Where(t => t != null && t.TowerData != null
                                && t.enabled
                                && !placedThisRound.Contains(t))
                    .ToList();
            }
        }

        /// <summary>
        /// Returns the list of 3 ingredient towers if recipe matches, null otherwise.
        /// The list always has sourceTower as the first element.
        /// </summary>
        private List<Tower> TryMatchRecipe(CombinationRecipe recipe, Tower sourceTower, List<Tower> pool)
        {
            // Try each ingredient slot as the source tower
            for (int sourceSlot = 0; sourceSlot < recipe.Ingredients.Length; sourceSlot++)
            {
                if (!IngredientMatches(recipe.Ingredients[sourceSlot], sourceTower)) continue;

                // Build remaining ingredient requirements
                var remaining = new List<TowerIngredient>();
                for (int i = 0; i < recipe.Ingredients.Length; i++)
                {
                    if (i != sourceSlot)
                        remaining.Add(recipe.Ingredients[i]);
                }

                // Try to satisfy remaining 2 from the pool (excluding sourceTower)
                var poolWithoutSource = pool.Where(t => t != sourceTower).ToList();
                var matched = TryFillIngredients(remaining, poolWithoutSource);
                if (matched != null)
                {
                    var result = new List<Tower> { sourceTower };
                    result.AddRange(matched);
                    return result;
                }
            }

            return null;
        }

        private List<Tower> TryFillIngredients(List<TowerIngredient> required, List<Tower> available)
        {
            if (required.Count == 0) return new List<Tower>();

            var usedIndices = new List<int>();
            var result = new List<Tower>();

            foreach (var ingredient in required)
            {
                bool found = false;
                for (int i = 0; i < available.Count; i++)
                {
                    if (usedIndices.Contains(i)) continue;
                    if (IngredientMatches(ingredient, available[i]))
                    {
                        usedIndices.Add(i);
                        result.Add(available[i]);
                        found = true;
                        break;
                    }
                }
                if (!found) return null;
            }

            return result;
        }

        private bool IngredientMatches(TowerIngredient ingredient, Tower tower)
        {
            if (tower == null || tower.TowerData == null) return false;
            if (tower.TowerData.TowerType != ingredient.TowerType) return false;
            // Level -1 means any level (used for combined tower ingredients)
            if (ingredient.Level >= 0 && tower.TowerData.Level != ingredient.Level) return false;
            return true;
        }
    }
}
