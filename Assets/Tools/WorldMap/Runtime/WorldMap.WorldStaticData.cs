using Tools.WorldMapCore.Database;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public struct WorldMapStaticData
    {
        public readonly int Amount;
        public readonly int Seed;
        public readonly int MaxSeed;
        public readonly bool HasRandomSeed;
        public readonly float IsolationDistance;
        public readonly Vector2 NodeWorldSize;
        public readonly Rect WorldBounds;
        public readonly WorldMapParameters.DebugData DebugData;

        public WorldMapStaticData(int amount,
            Vector2 nodeWorldSize,
            float isolationDistance,
            Rect worldBounds,
            int seed,
            bool hasRandomSeed,
            WorldMapParameters.DebugData debugData)
        {
            Amount = amount;
            NodeWorldSize = nodeWorldSize;
            IsolationDistance = isolationDistance;
            WorldBounds = worldBounds;
            Seed = seed;
            HasRandomSeed = hasRandomSeed;
            MaxSeed = int.MaxValue;
            DebugData = debugData;
        }
    }
}