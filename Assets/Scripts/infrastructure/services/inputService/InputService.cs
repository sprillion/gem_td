using System;
using System.Collections.Generic;
using infrastructure.services.selectionService;
using infrastructure.services.updateService;
using UnityEngine;
using UnityEngine.EventSystems;
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
            if (IsPointerOverUI())
                return;

            var ray = _camera.ScreenPointToRay(Mouse.current.position.value);
            var isHit = Physics.Raycast(ray, out var raycastHit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            if (!isHit || raycastHit.collider.transform.parent == null ||
                !raycastHit.collider.transform.parent.TryGetComponent(out IClickable clickable))
            {
                _selectionService.ClearSelection();
                return;
            }

            _selectionService.ClearSelection();
            clickable.OnClick();
        }

        private bool IsPointerOverUI()
        {
            if (EventSystem.current == null)
                return false;

            var eventData = new PointerEventData(EventSystem.current)
            {
                position = Mouse.current.position.ReadValue()
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
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