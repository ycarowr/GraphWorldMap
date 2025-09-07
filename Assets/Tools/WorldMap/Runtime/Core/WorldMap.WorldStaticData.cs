using Tools.WorldMapCore.Database;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public struct WorldMapStaticData
    {
        public readonly int Amount;
        public readonly int Seed;
        public readonly bool HasRandomSeed;
        public readonly float IsolationDistance;
        public readonly Vector2 NodeWorldSize;
        public readonly Rect WorldBounds;
        public readonly WorldMapParameters.DebugData DebugData;
        public readonly int Iterations;
        public readonly int ParallelIterations;
        public readonly int Timeout;


        public WorldMapStaticData(int amount,
            Vector2 nodeWorldSize,
            float isolationDistance,
            Rect worldBounds,
            int seed,
            int iterations,
            int parallelIterations,
            int timeout,
            bool hasRandomSeed,
            WorldMapParameters.DebugData debugData)
        {
            Amount = amount;
            NodeWorldSize = nodeWorldSize;
            IsolationDistance = isolationDistance;
            WorldBounds = worldBounds;
            Seed = seed;
            HasRandomSeed = hasRandomSeed;
            DebugData = debugData;
            Iterations = iterations;
            ParallelIterations = parallelIterations;
            Timeout = timeout;
        }

        public bool ValidateTotalArea()
        {
            var totalArea = WorldBounds.size.x * WorldBounds.size.y;
            var nodeArea = NodeWorldSize.x * NodeWorldSize.y;
            var totalNodeArea = nodeArea * Amount;
            return totalNodeArea > totalArea;
        }
    }
}