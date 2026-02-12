using System;
using enemies;
using UnityEngine;

namespace towers.projectiles
{
    public class Projectile : PooledObject
    {
        [SerializeField] private Renderer _sphereRenderer;
        [SerializeField] private TrailRenderer _trailRenderer;

        private Enemy _target;
        private float _speed;
        private Action _onArrival;

        private const float ArrivalDistance = 0.3f;

        public void Initialize(Enemy target, float speed, Material sphereMaterial, Material trailMaterial, Action onArrival)
        {
            _trailRenderer.Clear();
            
            _target = target;
            _speed = speed;
            _onArrival = onArrival;

            _sphereRenderer.sharedMaterial = sphereMaterial;
            _trailRenderer.sharedMaterial = trailMaterial;
        }

        public override void OnGetted()
        {
            base.OnGetted();
            _trailRenderer.Clear();
        }

        public override void Release()
        {
            _target = null;
            _onArrival = null;
            _trailRenderer.Clear();
            base.Release();
        }

        private void Update()
        {
            if (_target == null || !_target.IsAlive)
            {
                Release();
                return;
            }

            Vector3 targetPos = _target.transform.position + Vector3.up * 0.5f;
            Vector3 direction = (targetPos - transform.position).normalized;
            transform.position += direction * _speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetPos) < ArrivalDistance)
            {
                _onArrival?.Invoke();
                Release();
            }
        }
    }
}
