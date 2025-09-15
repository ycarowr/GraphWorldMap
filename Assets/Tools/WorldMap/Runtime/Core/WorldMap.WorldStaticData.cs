using System.Collections.Generic;
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
        public readonly bool IsPerfectSegmentLane;
        public readonly List<Rect> Lanes;
        public readonly List<Vector3> Start;
        public readonly List<Vector3> End;
        public readonly int AmountOfLaneConnections;

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
            int amountEnd,
            bool isPerfectSegmentLane,
            int amountOfLaneConnections)
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
            IsPerfectSegmentLane = isPerfectSegmentLane;
            AmountOfLaneConnections = amountOfLaneConnections;

            // Generate Lanes
            Lanes = new List<Rect>();
            if (orientation == WorldMapParameters.Orientation.LeftRight)
            {
                var laneSize = new Vector2(worldBounds.size.x, worldBounds.size.y / AmountStart);
                var worldMinX = WorldBounds.xMin;
                var worldMaxY = WorldBounds.yMax / AmountStart;
                for (var index = 0; index < AmountStart; index++)
                {
                    var lane = new Rect
                    {
                        position = new Vector2(worldMinX, worldMaxY * index),
                        size = new Vector2(laneSize.x, laneSize.y),
                    };
                    Lanes.Add(lane);
                }
            }
            else
            {
                var laneSize = new Vector2(worldBounds.size.x / AmountStart, worldBounds.size.y);
                var worldMinY = WorldBounds.yMin;
                var worldMaxX = WorldBounds.xMax / AmountStart;
                for (var index = 0; index < AmountStart; index++)
                {
                    var lane = new Rect
                    {
                        position = new Vector2(worldMaxX * index, worldMinY),
                        size = new Vector2(laneSize.x, laneSize.y),
                    };
                    Lanes.Add(lane);
                }
            }

            // Generate Starting
            Start = new List<Vector3>();
            for (var index = 0; index < AmountStart; index++)
            {
                var worldPosition = Vector2.zero;
                if (Orientation == WorldMapParameters.Orientation.LeftRight)
                {
                    if (!IsPerfectSegmentLane)
                    {
                        var segment = WorldBounds.size.y / (AmountStart + 1);
                        worldPosition.y = segment * (index + 1);
                    }
                    else
                    {
                        var segment = WorldBounds.size.y / AmountStart;
                        worldPosition.y = segment / 2 + segment * index;
                    }

                    worldPosition.x = WorldBounds.min.x - NodeWorldSize.x / 2 -
                                      WorldMapParameters.SMALL_NUMBER;
                }

                if (Orientation == WorldMapParameters.Orientation.BottomTop)
                {
                    if (!IsPerfectSegmentLane)
                    {
                        var segment = WorldBounds.size.x / (AmountStart + 1);
                        worldPosition.x = segment * (index + 1);
                    }
                    else
                    {
                        var segment = WorldBounds.size.x / AmountStart;
                        worldPosition.x = segment / 2 + segment * index;
                    }

                    worldPosition.y = WorldBounds.min.y - NodeWorldSize.y / 2 -
                                      WorldMapParameters.SMALL_NUMBER;
                }

                Start.Add(worldPosition);
            }

            // Generate Ending
            End = new List<Vector3>();
            for (var index = 0; index < AmountEnd; index++)
            {
                var worldPosition = Vector2.zero;
                if (Orientation == WorldMapParameters.Orientation.LeftRight)
                {
                    var segment = WorldBounds.size.y / (AmountEnd + 1);
                    worldPosition.x = WorldBounds.max.x + NodeWorldSize.x / 2 +
                                      WorldMapParameters.SMALL_NUMBER;
                    worldPosition.y = segment * (index + 1);
                }

                if (Orientation == WorldMapParameters.Orientation.BottomTop)
                {
                    var segment = WorldBounds.size.x / (AmountEnd + 1);
                    worldPosition.x = segment * (index + 1);
                    worldPosition.y = WorldBounds.max.y + NodeWorldSize.y / 2 +
                                      WorldMapParameters.SMALL_NUMBER;
                }

                End.Add(worldPosition);
            }
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