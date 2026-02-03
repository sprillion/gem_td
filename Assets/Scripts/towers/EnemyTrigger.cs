using System;
using enemies;
using UnityEngine;

namespace towers
{
    public class EnemyTrigger : MonoBehaviour
    {
        private const string EnemyTag = "Enemy";

        public event Action<Enemy> OnEnemyEnter;
        public event Action<Enemy> OnEnemyExit;

        public void SetRadius(float radius)
        {
            var sphereCollider = GetComponent<SphereCollider>();
            if (sphereCollider != null)
            {
                sphereCollider.radius = radius;
            }
            else
            {
                Debug.LogWarning("EnemyTrigger: No SphereCollider found to set radius");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(EnemyTag)) return;
            if (!other.TryGetComponent(out Enemy enemy)) return;
            OnEnemyEnter?.Invoke(enemy);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(EnemyTag)) return;
            if (!other.TryGetComponent(out Enemy enemy)) return;
            OnEnemyExit?.Invoke(enemy);
        }
    }
}