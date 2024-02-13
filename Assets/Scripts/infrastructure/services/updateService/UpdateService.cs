using System;
using UnityEngine;

namespace infrastructure.services.updateService
{
    public class UpdateService : MonoBehaviour, IUpdateService
    {
        public event Action OnUpdate;
        
        private void Update()
        {
            OnUpdate?.Invoke();
        }
    }
}