using infrastructure.services.inputService;
using level.builder;
using UnityEngine;
using Zenject;
using Cinemachine;

namespace cameras
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Camera _camera;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private float _dragSpeed = 0.5f;
        [SerializeField] private float _zoomSpeed = 1f;
        [SerializeField] private float _minZoom = 5f;
        [SerializeField] private float _maxZoom = 50f;

        private IInputService _inputService;
        private ILevelBuilder _levelBuilder;

        [Inject]
        public void Construct(IInputService inputService, ILevelBuilder levelBuilder)
        {
            _inputService = inputService;
            _levelBuilder = levelBuilder;
        }

        private void Start()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_virtualCamera == null)
            {
                _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            }
        }

        private void Update()
        {
            HandleCameraDrag();
            HandleCameraZoom();
        }

        private void HandleCameraDrag()
        {
            if (_inputService.IsMouseButtonHeld())
            {
                var mouseDelta = _inputService.GetMouseDelta();
                var direction = new Vector3(-mouseDelta.x, 0, -mouseDelta.y);
                _target.position += direction * _dragSpeed * Time.deltaTime;
                ClampCameraPosition();
            }
        }

        private void HandleCameraZoom()
        {
            var scrollDelta = _inputService.GetMouseScroll();

            if (scrollDelta.y == 0) return;
            if (_virtualCamera == null) return;

            // Get the transposer component to access Follow Offset
            var transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer == null) return;

            // Apply zoom speed to scroll delta (scroll values are typically 120 or -120 per notch)
            var zoomAmount = scrollDelta.y * _zoomSpeed * 0.01f;

            // Get current follow offset
            var currentOffset = transposer.m_FollowOffset;
            var newY = Mathf.Clamp(currentOffset.y - zoomAmount, _minZoom, _maxZoom);

            // Update the follow offset with new Y value
            transposer.m_FollowOffset = new Vector3(currentOffset.x, newY, currentOffset.z);
        }

        private void ClampCameraPosition()
        {
            var mapData = _levelBuilder.MapData;
            if (mapData == null) return;

            var pos = _target.position;
            pos.x = Mathf.Clamp(pos.x, 0f, (mapData.Width - 1) * mapData.BlockSize);
            pos.z = Mathf.Clamp(pos.z, -(mapData.Height - 1) * mapData.BlockSize, 0f);
            _target.position = pos;
        }
    }
}