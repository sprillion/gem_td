using TMPro;
using towers;
using UnityEngine;
using UnityEngine.UI;

namespace ui.combinations
{
    public enum IngredientPresence { None, Permanent, NewlyPlaced, Both }

    public class IngredientSlotUI : MonoBehaviour
    {
        private static readonly Color MissingColor    = new Color(0.35f, 0.35f, 0.35f, 0.9f);  // grey
        private static readonly Color PermanentColor  = new Color(0.2f,  0.75f, 0.25f, 1f);    // green – old tower
        private static readonly Color NewlyPlacedColor = new Color(0.95f, 0.75f, 0.1f,  1f);   // gold  – selection pool
        private static readonly Color BothColor       = new Color(0.2f,  0.65f, 0.9f,  1f);    // blue  – present in both

        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _background;

        public void Setup(TowerIngredient ingredient, IngredientPresence presence)
        {
            if (_label != null)
                _label.text = FormatIngredient(ingredient);

            if (_background != null)
            {
                switch (presence)
                {
                    case IngredientPresence.Permanent:   _background.color = PermanentColor;   break;
                    case IngredientPresence.NewlyPlaced: _background.color = NewlyPlacedColor; break;
                    case IngredientPresence.Both:        _background.color = BothColor;        break;
                    default:                             _background.color = MissingColor;     break;
                }
            }
        }

        private static string FormatIngredient(TowerIngredient ingredient)
        {
            if (ingredient.Level < 0)
                return ingredient.TowerType.ToString();

            return $"{ingredient.TowerType}{ingredient.Level + 1}";
        }
    }
}
