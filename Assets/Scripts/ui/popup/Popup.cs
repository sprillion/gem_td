using UnityEngine;

namespace ui
{
    public class Popup : MonoBehaviour
    {
        private Popup _backPopup;
        
        public bool IsActive => gameObject.activeSelf;
        
        
        public virtual void Initialize() { }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Show(Popup backPopup)
        {
            _backPopup = backPopup;
            _backPopup.Hide();
            Show();
        }

        public virtual void Show(params object[] args)
        {
            Show();
        }
        
        public virtual void Show(Popup backPopup, params object[] args)
        {
            Show(backPopup);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Back()
        {
            if (_backPopup)
            {
                _backPopup.Show();
                _backPopup = null;
            }
            Hide();
        }
    }
}