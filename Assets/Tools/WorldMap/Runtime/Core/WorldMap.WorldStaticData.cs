using System.Collections.Generic;
using Tools.WorldMapCore.Database;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public readonly struct WorldMapStaticData
    {
        public readonly WorldMapParameters Parameters;
        public readonly Rect WorldBounds;
        public readonly List<Rect> Lanes;
        public readonly List<Vector3> Start;
        public readonly List<Vector3> End;

        public WorldMapStaticData(WorldMapParameters parameters, Rect worldBounds)
        {
            Parameters = parameters;
            WorldBounds = worldBounds;

            // Generate Lanes
            Lanes = new List<Rect>();
            var amountStart = parameters.AmountStart;
            var amountEnd = parameters.AmountEnd;
            if (Parameters.Orientation == WorldMapParameters.OrientationGraph.LeftRight)
            {
                var laneSize = new Vector2(worldBounds.size.x, worldBounds.size.y / amountStart);
                var worldMinX = WorldBounds.xMin;
                var worldMaxY = WorldBounds.yMax / amountStart;
                for (var index = 0; index < amountStart; index++)
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
                var laneSize = new Vector2(worldBounds.size.x / amountStart, worldBounds.size.y);
                var worldMinY = WorldBounds.yMin;
                var worldMaxX = WorldBounds.xMax / amountStart;
                for (var index = 0; index < amountStart; index++)
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
            for (var index = 0; index < amountStart; index++)
            {
                var worldPosition = Vector2.zero;
                if (parameters.Orientation == WorldMapParameters.OrientationGraph.LeftRight)
                {
                    if (!parameters.IsPerfectSegmentLane)
                    {
                        var segment = WorldBounds.size.y / (amountStart + 1);
                        worldPosition.y = segment * (index + 1);
                    }
                    else
                    {
                        var segment = WorldBounds.size.y / amountStart;
                        worldPosition.y = segment / 2 + segment * index;
                    }

                    worldPosition.x = WorldBounds.min.x - parameters.NodeWorldSize.x / 2 -
                                      WorldMapParameters.SMALL_NUMBER;
                }

                if (parameters.Orientation == WorldMapParameters.OrientationGraph.BottomTop)
                {
                    if (!parameters.IsPerfectSegmentLane)
                    {
                        var segment = WorldBounds.size.x / (amountStart + 1);
                        worldPosition.x = segment * (index + 1);
                    }
                    else
                    {
                        var segment = WorldBounds.size.x / amountStart;
                        worldPosition.x = segment / 2 + segment * index;
                    }

                    worldPosition.y = WorldBounds.min.y - parameters.NodeWorldSize.y / 2 -
                                      WorldMapParameters.SMALL_NUMBER;
                }

                Start.Add(worldPosition);
            }

            // Generate Ending
            End = new List<Vector3>();
            for (var index = 0; index < amountEnd; index++)
            {
                var worldPosition = Vector2.zero;
                if (parameters.Orientation == WorldMapParameters.OrientationGraph.LeftRight)
                {
                    var segment = WorldBounds.size.y / (amountEnd + 1);
                    worldPosition.x = WorldBounds.max.x + parameters.NodeWorldSize.x / 2 +
                                      WorldMapParameters.SMALL_NUMBER;
                    worldPosition.y = segment * (index + 1);
                }

                if (parameters.Orientation == WorldMapParameters.OrientationGraph.BottomTop)
                {
                    var segment = WorldBounds.size.x / (amountEnd + 1);
                    worldPosition.x = segment * (index + 1);
                    worldPosition.y = WorldBounds.max.y + parameters.NodeWorldSize.y / 2 +
                                      WorldMapParameters.SMALL_NUMBER;
                }

                End.Add(worldPosition);
            }
        }

        public bool ValidateTotalArea()
        {
            var totalArea = WorldBounds.size.x * WorldBounds.size.y;
            var nodeArea = Parameters.NodeWorldSize.x * Parameters.NodeWorldSize.y;
            var totalNodeArea = nodeArea * Parameters.Amount;
            return totalNodeArea > totalArea;
        }
        
        public bool ValidateIsolation()
        {
            if (Parameters.IsolationDistance < 0)
            {
                // it should be ignored
                return false;
            }
            
            var isolationArea = Parameters.IsolationDistance * Parameters.IsolationDistance;
            return isolationArea < Parameters.NodeWorldSize.x * Parameters.NodeWorldSize.y;
        }
    }
}