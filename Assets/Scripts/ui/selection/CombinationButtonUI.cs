using System.Collections.Generic;
using infrastructure.services.combinationService;
using TMPro;
using towers;
using UnityEngine;
using UnityEngine.UI;

namespace ui.selection
{
    public class CombinationButtonUI : MonoBehaviour
    {
        [SerializeField] private Image _resultIcon;
        [SerializeField] private TMP_Text _resultName;
        [SerializeField] private Button _combineButton;

        public void Setup(CombinationRecipe recipe, List<Tower> ingredients, Tower sourceTower, ICombinationService svc)
        {
            if (_resultName != null)
                _resultName.text = recipe.ResultType.ToString();

            if (_resultIcon != null)
                _resultIcon.enabled = false;

            if (_combineButton != null)
            {
                _combineButton.onClick.RemoveAllListeners();
                _combineButton.onClick.AddListener(() => svc.ApplyCombination(sourceTower, recipe, ingredients));
            }
        }
    }
}
