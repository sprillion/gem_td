using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace towers
{
    [CreateAssetMenu(fileName = "TowerSettings", menuName = "Data/TowerSettings", order = 4)]
    public class TowerSettings : SerializedScriptableObject
    {
        public Dictionary<int, float> ScaleFromLevel;
    }
}