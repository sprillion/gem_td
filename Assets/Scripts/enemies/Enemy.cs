using System.Collections.Generic;
using infrastructure.services.pathService;
using level.builder;
using UnityEngine;
using Zenject;

namespace enemies
{
    public class Enemy : MonoBehaviour
    {
        private IPathService _pathService;
        private ILevelBuilder _levelBuilder;
        
        private EnemyData _enemyData;
        private List<Vector3> _path;
        private int _currentPoint;

        [Inject]
        private void Construct(IPathService pathService, ILevelBuilder levelBuilder)
        {
            _pathService = pathService;
            _levelBuilder = levelBuilder;
            SetPath();
        }

        private void Update()
        {
            if (Direction() != NeededDirection())
            {
                Rotate();
            }
            else
            {
                Move();
                CheckPoint();
            }
        }

        private void Move()
        {
            transform.position += Direction() * _enemyData.MoveSpeed * Time.deltaTime;
        }

        private void Rotate()
        {
            transform.Rotate(Vector3.up, _enemyData.RotateSpeed * Time.deltaTime);
        }

        private Vector3 Direction()
        {
            return (_path[_currentPoint] - transform.position).normalized;
        }
        
        private Vector3 NeededDirection()
        {
            return (_path[_currentPoint + 1] - _path[_currentPoint]).normalized;
        }

        private void CheckPoint()
        {
            if (transform.position == _path[_currentPoint])
            {
                _currentPoint++;
            }
        }

        private void SetPath()
        {
            _path = _pathService.CurrentPath
                .ConvertAll(point => new Vector3(point.x, 0, point.y) * _levelBuilder.MapData.BlockSize);
        }
    }
}