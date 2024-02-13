using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace infrastructure.services.resourceProvider
{
    public interface IResourceProvider
    {
        public T Load<T>(string path) where T : Object;
        public T LoadInstance<T>(string path) where T : Object;
        public T LoadInstance<T>(string path, Vector3 position, Quaternion rotation) where T : Object;
        public Dictionary<Type, T> LoadTypeDictionary<T>(string path) where T : Object;
        public List<T> LoadList<T>(string path) where T : Object;
    }
}
