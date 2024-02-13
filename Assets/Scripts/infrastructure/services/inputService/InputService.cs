using System;
using infrastructure.services.updateService;
using UnityEngine;
using UnityEngine.InputSystem;

namespace infrastructure.services.inputService
{
    public class InputService : IInputService
    {
        private const float MaxDurationClick = 0.5f;

        private readonly InputActions _inputActions;
        private readonly IUpdateService _updateService;
        private readonly Camera _camera;

        private float _clickDuration;

        public event Action OnSpacePressed;

        public InputService(InputActions inputActions, IUpdateService updateService)
        {
            _inputActions = inputActions;
            _updateService = updateService;
            _camera = Camera.main;
            _inputActions.Enable();
            _updateService.OnUpdate += Update;
            _inputActions.Map.MouseLeftClick.started += OnMouseClickStart;
            _inputActions.Map.MouseLeftClick.canceled += OnMouseClickEnd;
            _inputActions.Map.Space.performed += _ => OnSpacePressed?.Invoke();
        }

        public Vector2 MousePosition()
        {
            return Mouse.current.position.value;
        }

        private void Update()
        {
            _clickDuration += Time.deltaTime;
        }

        private void OnMouseClickStart(InputAction.CallbackContext context)
        {
            _clickDuration = 0;
        }

        private void OnMouseClickEnd(InputAction.CallbackContext context)
        {
            if (_clickDuration < MaxDurationClick)
            {
                ClickOnClickableObject();
            }
        }

        private void ClickOnClickableObject()
        {
            var ray = _camera.ScreenPointToRay(Mouse.current.position.value);
            var isHit = Physics.Raycast(ray, out var raycastHit);
            if (!isHit) return;
            if (!raycastHit.collider.transform.parent.TryGetComponent(out IClickable clickable)) return;
            clickable.OnClick();
        }
    }
}