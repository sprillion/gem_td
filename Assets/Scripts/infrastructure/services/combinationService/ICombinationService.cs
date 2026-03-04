using System.Collections.Generic;
using towers;

namespace infrastructure.services.combinationService
{
    public interface ICombinationService
    {
        List<(CombinationRecipe recipe, List<Tower> ingredients)> GetAvailableCombinations(Tower selectedTower);
        void ApplyCombination(Tower sourceTower, CombinationRecipe recipe, List<Tower> ingredients);
    }
}
