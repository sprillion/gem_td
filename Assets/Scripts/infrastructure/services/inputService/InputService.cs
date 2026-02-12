using System;
using infrastructure.services.selectionService;
using infrastructure.services.updateService;
using towers;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace infrastructure.services.inputService
{
    public class InputService : IInputService
    {
        private const float MaxDurationClick = 0.5f;
        private const float MaxDragDistanceForClick = 5f; // pixels

        private readonly InputActions _inputActions;
        private readonly IUpdateService _updateService;
        private readonly ISelectionService _selectionService;
        private readonly Camera _camera;

        private float _clickDuration;
        private bool _isMouseButtonHeld;
        private Vector2 _mouseDownPosition;
        private float _totalMouseMovement;

        public event Action OnSpacePressed;
        public event Action OnPPressed;

        [Inject]
        public InputService(InputActions inputActions, IUpdateService updateService, ISelectionService selectionService)
        {
            _inputActions = inputActions;
            _updateService = updateService;
            _selectionService = selectionService;
            _camera = Camera.main;
            _inputActions.Enable();
            _updateService.OnUpdate += Update;
            _inputActions.Map.MouseLeftClick.started += OnMouseClickStart;
            _inputActions.Map.MouseLeftClick.canceled += OnMouseClickEnd;
            _inputActions.Map.Space.performed += _ => OnSpacePressed?.Invoke();
            _inputActions.Map.P.canceled += _ => OnPPressed?.Invoke();
        }

        public Vector2 MousePosition()
        {
            return Mouse.current.position.value;
        }

        private void Update()
        {
            _clickDuration += Time.deltaTime;

            // Track mouse movement while button is held
            if (_isMouseButtonHeld)
            {
                var mouseDelta = GetMouseDelta();
                _totalMouseMovement += mouseDelta.magnitude;
            }

            // Check hover for range circle preview
            CheckHover();
        }

        private void CheckHover()
        {
            var ray = _camera.ScreenPointToRay(Mouse.current.position.value);
            var isHit = Physics.Raycast(ray, out var raycastHit, Mathf.Infinity,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            if (isHit && raycastHit.collider.transform.parent != null &&
                raycastHit.collider.transform.parent.TryGetComponent(out Tower tower))
            {
                _selectionService.SetHoveredTower(tower);
            }
            else
            {
                _selectionService.ClearHover();
            }
        }

        private void OnMouseClickStart(InputAction.CallbackContext context)
        {
            _clickDuration = 0;
            _isMouseButtonHeld = true;
            _mouseDownPosition = Mouse.current.position.value;
            _totalMouseMovement = 0f;
        }

        private void OnMouseClickEnd(InputAction.CallbackContext context)
        {
            _isMouseButtonHeld = false;

            // Only treat as a click if:
            // 1. Duration was short enough (< 0.5s)
            // 2. Mouse didn't move much (< 5 pixels total movement)
            if (_clickDuration < MaxDurationClick && _totalMouseMovement < MaxDragDistanceForClick)
            {
                ClickOnClickableObject();
            }
        }

        private void ClickOnClickableObject()
        {
            var ray = _camera.ScreenPointToRay(Mouse.current.position.value);
            var isHit = Physics.Raycast(ray, out var raycastHit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            if (!isHit || raycastHit.collider.transform.parent == null ||
                !raycastHit.collider.transform.parent.TryGetComponent(out IClickable clickable))
            {
                // Clicked on empty space - clear selection
                _selectionService.ClearSelection();
                return;
            }

            clickable.OnClick();
        }

        public Vector2 GetMouseDelta()
        {
            return _inputActions.Map.MouseDelta.ReadValue<Vector2>();
        }

        public Vector2 GetMouseScroll()
        {
            return _inputActions.Map.MouseScroll.ReadValue<Vector2>();
        }

        public bool IsMouseButtonHeld()
        {
            return _isMouseButtonHeld;
        }
    }
}