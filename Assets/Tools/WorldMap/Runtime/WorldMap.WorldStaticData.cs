using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public struct WorldMapStaticData
    {
        public readonly int Amount;
        public readonly Vector2 Size;
        public readonly float MinDistance;
        public readonly Rect Bounds;
        public readonly int Seed;
        public readonly bool UseRandomSeed;

        public WorldMapStaticData(int amount, Vector2 size, float minDistance, Rect bounds, int seed, bool useRandomSeed)
        {
            Amount = amount;
            Size = size;
            MinDistance = minDistance;
            Bounds = bounds;
            Seed = seed;
            UseRandomSeed = useRandomSeed;
        }
    }
}