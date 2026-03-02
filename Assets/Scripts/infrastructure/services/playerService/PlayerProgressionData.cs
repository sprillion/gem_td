using System;
using UnityEngine;

namespace infrastructure.services.playerService
{
    [CreateAssetMenu(fileName = "PlayerProgressionData", menuName = "Game/PlayerProgressionData")]
    public class PlayerProgressionData : ScriptableObject
    {
        [Tooltip("XP required to advance from each level. Length defines max player level (e.g., 4 entries = max level 5).")]
        public int[] XPPerLevel = { 100, 250, 500, 1000 };

        [Tooltip("Tower level drop weights per player level. Add one row per player level.")]
        public TowerLevelWeightsRow[] TowerLevelWeights =
        {
            new TowerLevelWeightsRow { Weights = new float[] { 1.00f, 0.00f, 0.00f, 0.00f, 0.00f } },
            new TowerLevelWeightsRow { Weights = new float[] { 0.65f, 0.35f, 0.00f, 0.00f, 0.00f } },
            new TowerLevelWeightsRow { Weights = new float[] { 0.50f, 0.30f, 0.20f, 0.00f, 0.00f } },
            new TowerLevelWeightsRow { Weights = new float[] { 0.45f, 0.25f, 0.20f, 0.10f, 0.00f } },
            new TowerLevelWeightsRow { Weights = new float[] { 0.35f, 0.25f, 0.20f, 0.15f, 0.05f } },
        };

        public int MaxPlayerLevel => XPPerLevel.Length + 1;
    }

    [Serializable]
    public class TowerLevelWeightsRow
    {
        [Tooltip("Weights for each tower level at this player level. Must sum to 1 (or will be normalized).")]
        public float[] Weights = { 1f, 0f, 0f, 0f, 0f };
    }
}
