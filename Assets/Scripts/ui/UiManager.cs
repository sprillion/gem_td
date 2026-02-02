using ui.menu;
using UnityEngine;

namespace ui
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private MenuView _menuView;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            _menuView.Initialize();
        }
    }
}