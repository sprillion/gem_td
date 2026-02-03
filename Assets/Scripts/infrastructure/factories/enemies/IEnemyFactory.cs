using enemies;
using UnityEngine;

namespace infrastructure.factories.enemies
{
    public interface IEnemyFactory
    {
        Enemy CreateEnemy(EnemyData enemyData, Vector3 position, int waveNumber);
    }
}
