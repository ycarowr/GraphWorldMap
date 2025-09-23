using System;
using System.Collections.Generic;
using Tools.WorldMapCore.Database;
using UGizmo;
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
                foreach (var lane in data.Lanes)
                {
                    UGizmos.DrawWireCube(lane.center, Quaternion.identity, lane.size, Color.yellow);
                }
            }

// #if UNITY_EDITOR
//             {
//                 // Draw bounders
//                 var bottomLeft = data.WorldBounds.min.ToString();
//                 var bottomRight = data.WorldBounds.min + new Vector2(data.WorldBounds.xMax, 0);
//                 var topLeft = data.WorldBounds.min + new Vector2(0, data.WorldBounds.yMax);
//                 var topRight = data.WorldBounds.max.ToString();
//                 UnityEditor.Handles.Label(data.WorldBounds.min, bottomLeft);
//                 UnityEditor.Handles.Label(bottomRight, bottomRight.ToString());
//                 UnityEditor.Handles.Label(topLeft, topLeft.ToString());
//                 UnityEditor.Handles.Label(data.WorldBounds.max, topRight);
//             }
// #endif

            {
                // Draw center 
                UGizmos.DrawWireSphere(data.WorldBounds.center, 0.2f, Color.magenta);
            }

            {
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
                    UGizmos.DrawLineStrip(points, true, Color.green);
                }
            }

            {
                // Start
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
                    UGizmos.DrawLineStrip(points, true, new Color(60f / 255f, 179f / 255f, 113 / 255f));
                }
            }

            {
                // End
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
                    UGizmos.DrawLineStrip(points, true, new Color(106f / 255f, 90f / 255f, 205 / 255f));
                }
            }

            var isAll = data.Parameters.DebugValues.DeletionReason == WorldMap.EDeletionReason.All;
            {
                if (isAll || data.Parameters.DebugValues.DeletionReason == WorldMap.EDeletionReason.Overlap)
                {
                    // Draw Overlap
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
                        UGizmos.DrawLineStrip(points, true, Color.red);
                    }
                }
            }

            {
                if (isAll || data.Parameters.DebugValues.DeletionReason == WorldMap.EDeletionReason.OutOfBounds)
                {
                    // Draw Bounds
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
                        UGizmos.DrawLineStrip(points, true, Color.yellow);
                    }
                }
            }
        }
    }
}