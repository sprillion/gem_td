using TMPro;
using towers;
using UnityEngine;

namespace ui.combinations
{
    public class RecipeRowUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _resultName;
        [SerializeField] private IngredientSlotUI _slot1;
        [SerializeField] private IngredientSlotUI _slot2;
        [SerializeField] private IngredientSlotUI _slot3;

        public void Setup(CombinationRecipe recipe, IngredientPresence[] presences)
        {
            if (_resultName != null)
                _resultName.text = recipe.ResultType.ToString();

            var slots = new[] { _slot1, _slot2, _slot3 };
            for (int i = 0; i < 3 && i < recipe.Ingredients.Length; i++)
            {
                if (slots[i] != null)
                    slots[i].Setup(recipe.Ingredients[i], i < presences.Length ? presences[i] : IngredientPresence.None);
            }
        }
    }
}
