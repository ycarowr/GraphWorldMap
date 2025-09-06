using System;
using UnityEngine;

namespace Tools.WorldMapCreation
{
    public class Node : IComparable<Node>
    {
        public readonly Vector2 Size;
        public readonly Vector2 WorldPosition;

        public Node(Vector2 worldPosition, Vector2 size)
        {
            WorldPosition = worldPosition;
            Size = size;
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