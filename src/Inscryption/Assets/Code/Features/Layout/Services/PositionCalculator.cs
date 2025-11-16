using System.Collections.Generic;
using UnityEngine;

namespace Code.Features.Layout.Services
{
    public struct CardLayoutData
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }

    public static class PositionCalculator
    {
        public static IReadOnlyList<Vector3> CalculateGridPositions(GridLayoutParams parameters)
        {
            var positions = new List<Vector3>(parameters.Rows * parameters.Columns);
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
                    positions.Add(position);
                }
            }

            return positions;
        }

        public static IReadOnlyList<Vector3> CalculateHorizontalLayoutPositions(HorizontalLayoutParams parameters)
        {
            var positions = new List<Vector3>(parameters.Count);
            float totalWidth = (parameters.Count - 1) * parameters.Spacing;
            Vector3 startOffset = new Vector3(-totalWidth / 2f, 0, 0);
            for (int i = 0; i < parameters.Count; i++)
            {
                float x = i * parameters.Spacing;
                Vector3 position = parameters.Origin + startOffset + new Vector3(x, 0, 0);
                positions.Add(position);
            }

            return positions;
        }

        public static IReadOnlyList<Vector3> CalculateVerticalLayoutPositions(VerticalLayoutParams parameters)
        {
            var positions = new List<Vector3>(parameters.Count);
            float totalHeight = (parameters.Count - 1) * parameters.Spacing;
            Vector3 startOffset = new Vector3(0, -totalHeight / 2f, 0);
            for (int i = 0; i < parameters.Count; i++)
            {
                float y = i * parameters.Spacing;
                Vector3 position = parameters.Origin + startOffset + new Vector3(0, y, 0);
                positions.Add(position);
            }

            return positions;
        }

        public static CardLayoutData[] CalculateArcLayout(ArcLayoutParams parameters)
        {
            var layoutData = new CardLayoutData[parameters.Count];
            if (parameters.Count == 0)
                return layoutData;
            if (parameters.Count == 1)
            {
                layoutData[0] = new CardLayoutData
                {
                    Position = parameters.Origin,
                    Rotation = Quaternion.identity
                };
                return layoutData;
            }

            float centerIndex = (parameters.Count - 1) / 2f;
            for (int i = 0; i < parameters.Count; i++)
            {
                float normalizedIndex = i - centerIndex;
                float rotationAngle = normalizedIndex * parameters.AnglePerCard;
                float angleRad = rotationAngle * Mathf.Deg2Rad;
                float x = normalizedIndex * parameters.HorizontalSpacing;
                float y = -Mathf.Abs(normalizedIndex) * parameters.VerticalCurve;
                float z = -Mathf.Abs(normalizedIndex) * parameters.DepthSpacing;
                Vector3 localPosition = new Vector3(x, y, z);
                Vector3 worldPosition = parameters.Origin + localPosition;
                Quaternion rotation = Quaternion.Euler(0, 0, -rotationAngle);
                layoutData[i] = new CardLayoutData
                {
                    Position = worldPosition,
                    Rotation = rotation
                };
            }

            return layoutData;
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

    public struct ArcLayoutParams
    {
        public int Count;
        public Vector3 Origin;
        public float HorizontalSpacing;
        public float VerticalCurve;
        public float DepthSpacing;
        public float AnglePerCard;
    }
}