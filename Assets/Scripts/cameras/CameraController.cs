using infrastructure.services.inputService;
using UnityEngine;
using Zenject;

namespace cameras
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _boardSize = 20;
        [SerializeField] private float _moveSpeed = 20;
        
        private IInputService _inputService;
        
        [Inject]
        public void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }

        private void Update()
        {
            MoveCamera();
        }

        private void MoveCamera()
        {
            var direction = new Vector3();
            var mouseX = _inputService.MousePosition().x;
            var mouseY = _inputService.MousePosition().y;
            
            if (mouseX < _boardSize && mouseX >= 0)
            {
                direction.x -= 1f;
            }
            if (mouseX > Screen.width - _boardSize && mouseX <= Screen.width)
            {
                direction.x += 1f;
            }
            if (mouseY < _boardSize && mouseY >= 0)
            {
                direction.z -= 1f;
            }
            if (mouseY > Screen.height - _boardSize && mouseY <= Screen.height)
            {
                direction.z += 1f;
            }
            
            _target.transform.position += direction.normalized * _moveSpeed * Time.deltaTime;
        }
    }
}