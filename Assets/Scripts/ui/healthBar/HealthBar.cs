using UnityEngine;
using UnityEngine.UI;

namespace ui.healthBar
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private float _heightOffset = 1.5f;

        private Transform _target;
        private Camera _mainCamera;
        private int _maxHealth;
        private int _currentHealth;

        public void Initialize(Transform target, int maxHealth)
        {
            _target = target;
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
            _mainCamera = Camera.main;

            UpdatePosition();
            UpdateFill();
        }

        public void SetHealth(int currentHealth)
        {
            _currentHealth = currentHealth;
            UpdateFill();
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                Destroy(gameObject);
                return;
            }

            UpdatePosition();
            LookAtCamera();
        }

        private void UpdatePosition()
        {
            if (_target != null)
            {
                transform.position = _target.position + Vector3.up * _heightOffset;
            }
        }

        private void LookAtCamera()
        {
            if (_mainCamera != null)
            {
                transform.forward = _mainCamera.transform.forward;
            }
        }

        private void UpdateFill()
        {
            if (_fillImage != null && _maxHealth > 0)
            {
                _fillImage.fillAmount = (float)_currentHealth / _maxHealth;
            }
        }
    }
}
