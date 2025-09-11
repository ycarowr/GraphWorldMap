using Tools.WorldMapCore.Database;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public readonly struct WorldMapStaticData
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
        public readonly WorldMapParameters.Orientation Orientation;
        public readonly int AmountStart;
        public readonly int AmountEnd;

        public WorldMapStaticData(int amount,
            Vector2 nodeWorldSize,
            float isolationDistance,
            Rect worldBounds,
            int seed,
            int iterations,
            int parallelIterations,
            int timeout,
            bool hasRandomSeed,
            WorldMapParameters.DebugData debugData,
            WorldMapParameters.Orientation orientation,
            int amountStart,
            int amountEnd)
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
            Orientation = orientation;
            AmountEnd = amountEnd;
            AmountStart = amountStart;
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