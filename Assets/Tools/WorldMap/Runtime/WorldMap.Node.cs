using System;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public class Node : IComparable<Node>
    {
        public readonly int ID;
        public readonly Rect WorldRect;
        public Vector2 WorldPosition => WorldRect.center;
        public Vector2 Size => WorldRect.size;

        public Node(int id, Vector2 worldPosition, Vector2 size)
        {
            ID = id;
            WorldRect = new Rect(worldPosition, size);
            WorldRect.center = worldPosition;
        }

        public int CompareTo(Node other)
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