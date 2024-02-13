using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace infrastructure.services.resourceProvider
{
    public class ResourceProvider : IResourceProvider
    {
        public T Load<T>(string path) where T : Object
        {
            T prefab = Resources.Load<T>(path);
            return prefab;
        }
    
        public T LoadInstance<T>(string path) where T : Object
        {
            T instance = Object.Instantiate(Load<T>(path));
            return instance;
        }

        public T LoadInstance<T>(string path, Vector3 position, Quaternion rotation) where T : Object
        {
            T instance = Object.Instantiate(Load<T>(path), position, rotation);
            return instance;
        }
    
        public Dictionary<Type, T> LoadTypeDictionary<T>(string path) where T : Object =>
            Resources
                .LoadAll<T>(path)
                .ToDictionary(x => x.GetType(), x => x);

        public List<T> LoadList<T>(string path) where T : Object => 
            Resources
                .LoadAll<T>(path)
                .ToList();
    }
}
