using System;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public class WorldMapNode : IComparable<WorldMapNode>
    {
        public readonly Rect Bounds;
        public readonly int ID;

        public WorldMapNode(int id, Vector2 worldPosition, Vector2 size)
        {
            ID = id;
            Bounds = new Rect(worldPosition, size);
            Bounds.center = worldPosition;
        }

        public Vector2 WorldPosition => Bounds.center;
        public Vector2 Size => Bounds.size;

        public int CompareTo(WorldMapNode other)
        {
            if (WorldPosition.x > other.WorldPosition.x)
            {
                return 1;
            }

            if (WorldPosition.x < other.WorldPosition.x)
            {
                return -1;
            }

            if (WorldPosition.y < other.WorldPosition.y)
            {
                return 1;
            }

            if (WorldPosition.y < other.WorldPosition.y)
            {
                return -1;
            }

            return 0;
        }
    }
}