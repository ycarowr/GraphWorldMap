using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public struct WorldMapStaticData
    {
        public readonly int Amount;
        public readonly int Seed;
        public readonly bool UseRandomSeed;
        public readonly float MinDistance;
        public readonly Vector2 NodeWorldSize;
        public readonly Rect WorldBounds;

        public WorldMapStaticData(int amount, Vector2 nodeWorldSize, float minDistance, Rect worldBounds, int seed, bool useRandomSeed)
        {
            Amount = amount;
            NodeWorldSize = nodeWorldSize;
            MinDistance = minDistance;
            WorldBounds = worldBounds;
            Seed = seed;
            UseRandomSeed = useRandomSeed;
        }
    }
}