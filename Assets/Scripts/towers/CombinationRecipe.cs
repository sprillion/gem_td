using System;
using UnityEngine;

namespace towers
{
    [Serializable]
    public struct TowerIngredient
    {
        public TowerType TowerType;
        public int Level; // 0-indexed; -1 = any level (for combined tower types)
    }

    [CreateAssetMenu(fileName = "Recipe", menuName = "Game/CombinationRecipe")]
    public class CombinationRecipe : ScriptableObject
    {
        public TowerIngredient[] Ingredients; // length = 3
        public TowerType ResultType;
        public int ResultLevel; // always 0 for combined towers
    }
}
