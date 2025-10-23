using System.Collections.Generic;
using UnityEngine;

namespace Code.Features.Layout.Services
{
    public static class PositionCalculator
    {
        private static readonly List<Vector3> Positions = new List<Vector3>(32);

        public static IReadOnlyList<Vector3> CalculateGridPositions(GridLayoutParams parameters)
        {
            Positions.Clear();
            float totalWidth = (parameters.Columns - 1) * parameters.Spacing.x;
            float totalHeight = (parameters.Rows - 1) * parameters.Spacing.y;

            Vector3 startOffset = new Vector3(-totalWidth / 2f, 0, -totalHeight / 2f);

            for (int row = 0; row < parameters.Rows; row++)
            {
                for (int col = 0; col < parameters.Columns; col++)
                {
                    float x = col * parameters.Spacing.x;
                    float z = row * parameters.Spacing.y;
                    Vector3 position = parameters.Origin + startOffset + new Vector3(x, 0, z);
                    Positions.Add(position);
                }
            }
            
            return Positions;
        }

        public static IReadOnlyList<Vector3> CalculateHorizontalLayoutPositions(HorizontalLayoutParams parameters)
        {
            Positions.Clear();
            float totalWidth = (parameters.Count - 1) * parameters.Spacing;
            Vector3 startOffset = new Vector3(-totalWidth / 2f, 0, 0);

            for (int i = 0; i < parameters.Count; i++)
            {
                float x = i * parameters.Spacing;
                Vector3 position = parameters.Origin + startOffset + new Vector3(x, 0, 0);
                Positions.Add(position);
            }

            return Positions;
        }

        public static IReadOnlyList<Vector3> CalculateVerticalLayoutPositions(VerticalLayoutParams parameters)
        {
            Positions.Clear();
            float totalHeight = (parameters.Count - 1) * parameters.Spacing;
            Vector3 startOffset = new Vector3(0, -totalHeight / 2f, 0);

            for (int i = 0; i < parameters.Count; i++)
            {
                float y = i * parameters.Spacing;
                Vector3 position = parameters.Origin + startOffset + new Vector3(0, y, 0);
                Positions.Add(position);
            }

            return Positions;
        }
    }
    
    public struct GridLayoutParams
    {
        public int Rows;
        public int Columns;
        public Vector2 Spacing;
        public Vector3 Origin;
    }

    public struct HorizontalLayoutParams
    {
        public int Count;
        public float Spacing;
        public Vector3 Origin;
    }
    
    public struct VerticalLayoutParams
    {
        public int Count;
        public float Spacing;
        public Vector3 Origin;
    }
}
