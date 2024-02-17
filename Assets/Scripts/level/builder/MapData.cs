using System;
using System.Collections.Generic;
using infrastructure.factories.blocks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace level.builder
{
    [CreateAssetMenu(fileName = "Map", menuName = "Data/Map", order = 0)]
    public class MapData : SerializedScriptableObject
    {
        [Min(1)] public int Width;
        [Min(1)] public int Height;

        public float BlockSize = 2f;
        
        [SerializeField] private BlockType _currentBlock;
        
        [OdinSerialize]
        [ShowInInspector]
        [TableMatrix(HorizontalTitle = "Custom Cell Drawing", DrawElementMethod = "DrawColoredBlockElement",
            ResizableColumns = false, SquareCells = true)]
        public BlockType[,] BlocksMap = new BlockType[1, 1];

        public List<Vector2Int> Points = new List<Vector2Int>();

        [Button]
        private void Refresh()
        {
            BlocksMap = new BlockType[Width, Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    BlocksMap[i, j] = (i + j % 2) % 2 == 0 ? BlockType.Light : BlockType.Dark;
                }
            }
        }

        [Button]
        private void SetListPoints()
        {
            Points.Clear();
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (BlocksMap[i, j] is not (BlockType.Point or BlockType.Start or BlockType.End)) continue;
                    Points.Add(new Vector2Int(i, j));
                }
            }
        }
        
        private BlockType DrawColoredBlockElement(Rect rect, BlockType value)
        {
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                value = value == _currentBlock ? BlockType.None : _currentBlock;
                GUI.changed = true;
                Event.current.Use();
            }
            var style = new GUIStyle
            {
                normal =
                {
                    textColor = Color.white
                }
            };
            EditorGUI.TextArea(rect.Padding(1), value.ToString(), style);
            EditorGUI.DrawRect(rect.Padding(1), GetColor(value));

            return value;
        }

        private Color GetColor(BlockType blockType)
        {
            return blockType switch
            {
                BlockType.None => new Color(0f, 0f, 0f, 0.38f),
                BlockType.Dark => new Color(0f, 0.75f, 0.02f, 0.58f),
                BlockType.Light => new Color(0f, 0.58f, 0.02f, 0.3f),
                BlockType.Point => new Color(0.84f, 0.8f, 0.03f, 0.33f),
                BlockType.Way => new Color(0.84f, 0.14f, 0f, 0.24f),
                BlockType.NoPut => new Color(0.35f, 0f, 0.84f, 0.24f),
                BlockType.Start => new Color(0.84f, 0.83f, 0.78f, 0.24f),
                BlockType.End => new Color(0.01f, 0.01f, 0.01f, 0.66f),
                _ => throw new ArgumentOutOfRangeException(nameof(blockType), blockType, null)
            };
        }
        
        
    }
}
