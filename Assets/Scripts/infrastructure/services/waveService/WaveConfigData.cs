using enemies;
using UnityEngine;

namespace infrastructure.services.waveService
{
    [CreateAssetMenu(fileName = "WaveConfig", menuName = "Data/WaveConfig", order = 3)]
    public class WaveConfigData : ScriptableObject
    {
        public int WaveNumber;
        public int EnemyCount;
        public float SpawnInterval;
        // EnemyData is now loaded dynamically by wave number in WaveService
    }
}
