using UnityEngine;

namespace Tools.WorldMapCreation
{
    public struct WorldMapStaticData
    {
        public readonly int Amount;
        public readonly Vector2 Size;
        public readonly float MinDistance;
        public readonly Rect Bounds;

        public WorldMapStaticData(int amount, Vector2 size, float minDistance, Rect bounds)
        {
            Amount = amount;
            Size = size;
            MinDistance = minDistance;
            Bounds = bounds;
        }
    }
}