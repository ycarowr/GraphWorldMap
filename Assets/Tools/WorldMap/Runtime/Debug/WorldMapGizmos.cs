#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using Tools.WorldMapCore.Database;
using UnityEditor;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static class WorldMapGizmos
    {
        public static void DrawGizmos(WorldMapStaticData data,
            List<WorldMapNode> nodes,
            List<WorldMapNode> start,
            List<WorldMapNode> end,
            Dictionary<WorldMap.EDeletionReason, List<WorldMapNode>> deletions)
        {
            if (data.Parameters.DebugValues.Mode != WorldMapParameters.DebugData.DrawMode.All &&
                data.Parameters.DebugValues.Mode != WorldMapParameters.DebugData.DrawMode.Nodes)
            {
                return;
            }

            {
                // Draw lanes
                Gizmos.color = Color.yellow;
                foreach (var lane in data.Lanes)
                {
                    Gizmos.DrawWireCube(lane.center, lane.size);
                }
            }

            {
                // Draw bounders
                var bottomLeft = data.WorldBounds.min.ToString();
                var bottomRight = data.WorldBounds.min + new Vector2(data.WorldBounds.xMax, 0);
                var topLeft = data.WorldBounds.min + new Vector2(0, data.WorldBounds.yMax);
                var topRight = data.WorldBounds.max.ToString();
                Handles.Label(data.WorldBounds.min, bottomLeft);
                Handles.Label(bottomRight, bottomRight.ToString());
                Handles.Label(topLeft, topLeft.ToString());
                Handles.Label(data.WorldBounds.max, topRight);
            }

            {
                // Draw center 
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(data.WorldBounds.center, 0.2f);
            }

            {
                // Draw borders isn't necessary because the lanes are just enough
                // Gizmos.color = Color.magenta;
                // ReadOnlySpan<Vector3> points = new[]
                // {
                //     new Vector3(data.WorldBounds.xMin - SMALL_NUMBER_DRAW, data.WorldBounds.yMin - SMALL_NUMBER_DRAW, 0),
                //     new Vector3(data.WorldBounds.xMin - SMALL_NUMBER_DRAW, data.WorldBounds.yMax + SMALL_NUMBER_DRAW, 0),
                //     new Vector3(data.WorldBounds.xMax + SMALL_NUMBER_DRAW, data.WorldBounds.yMax + SMALL_NUMBER_DRAW, 0),
                //     new Vector3(data.WorldBounds.xMax + SMALL_NUMBER_DRAW, data.WorldBounds.yMin - SMALL_NUMBER_DRAW, 0),
                // };
                // Gizmos.DrawLineStrip(points, true);
            }

            {
                Gizmos.color = Color.green;
                for (var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    ReadOnlySpan<Vector3> points = new[]
                    {
                        new Vector3(node.Bounds.xMin, node.Bounds.yMin, 0),
                        new Vector3(node.Bounds.xMin, node.Bounds.yMax, 0),
                        new Vector3(node.Bounds.xMax, node.Bounds.yMax, 0),
                        new Vector3(node.Bounds.xMax, node.Bounds.yMin, 0),
                    };
                    Gizmos.DrawLineStrip(points, true);
                }
            }

            {
                // Start
                Gizmos.color = new Color(60f / 255f, 179f / 255f, 113 / 255f);
                for (var i = 0; i < start.Count; i++)
                {
                    var node = start[i];
                    ReadOnlySpan<Vector3> points = new[]
                    {
                        new Vector3(node.Bounds.xMin, node.Bounds.yMin, 0),
                        new Vector3(node.Bounds.xMin, node.Bounds.yMax, 0),
                        new Vector3(node.Bounds.xMax, node.Bounds.yMax, 0),
                        new Vector3(node.Bounds.xMax, node.Bounds.yMin, 0),
                    };
                    Gizmos.DrawLineStrip(points, true);
                }
            }

            {
                // End
                Gizmos.color = new Color(106f / 255f, 90f / 255f, 205 / 255f);
                for (var i = 0; i < end.Count; i++)
                {
                    var node = end[i];
                    ReadOnlySpan<Vector3> points = new[]
                    {
                        new Vector3(node.Bounds.xMin, node.Bounds.yMin, 0),
                        new Vector3(node.Bounds.xMin, node.Bounds.yMax, 0),
                        new Vector3(node.Bounds.xMax, node.Bounds.yMax, 0),
                        new Vector3(node.Bounds.xMax, node.Bounds.yMin, 0),
                    };
                    Gizmos.DrawLineStrip(points, true);
                }
            }

            var isAll = data.Parameters.DebugValues.DeletionReason == WorldMap.EDeletionReason.All;
            {
                if (isAll || data.Parameters.DebugValues.DeletionReason == WorldMap.EDeletionReason.Overlap)
                {
                    // Draw Overlap
                    Gizmos.color = Color.red;
                    var deleted = deletions[WorldMap.EDeletionReason.Overlap];
                    for (var i = 0; i < deleted.Count; i++)
                    {
                        var node = deleted[i];
                        ReadOnlySpan<Vector3> points = new[]
                        {
                            new Vector3(node.Bounds.xMin, node.Bounds.yMin, 0),
                            new Vector3(node.Bounds.xMin, node.Bounds.yMax, 0),
                            new Vector3(node.Bounds.xMax, node.Bounds.yMax, 0),
                            new Vector3(node.Bounds.xMax, node.Bounds.yMin, 0),
                        };
                        Gizmos.DrawLineStrip(points, true);
                    }
                }
            }

            {
                if (isAll || data.Parameters.DebugValues.DeletionReason == WorldMap.EDeletionReason.OutOfBounds)
                {
                    // Draw Bounds
                    Gizmos.color = Color.yellow;
                    var deleted = deletions[WorldMap.EDeletionReason.OutOfBounds];
                    for (var i = 0; i < deleted.Count; i++)
                    {
                        var node = deleted[i];
                        ReadOnlySpan<Vector3> points = new[]
                        {
                            new Vector3(node.Bounds.xMin, node.Bounds.yMin, 0),
                            new Vector3(node.Bounds.xMin, node.Bounds.yMax, 0),
                            new Vector3(node.Bounds.xMax, node.Bounds.yMax, 0),
                            new Vector3(node.Bounds.xMax, node.Bounds.yMin, 0),
                        };
                        Gizmos.DrawLineStrip(points, true);
                    }
                }
            }
        }
    }
}
#endif