using TMPro;
using UnityEngine;

namespace ui.combinations
{
    public class CategoryHeaderUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;

        public void Setup(string categoryName)
        {
            if (_label != null)
                _label.text = categoryName;
        }
    }
}
