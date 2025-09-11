#if UNITY_EDITOR

using System;
using System.Collections.Generic;
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
            if (!data.DebugData.DrawGizmos)
            {
                return;
            }

            {
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
                // Draw borders
                Gizmos.color = Color.magenta;
                ReadOnlySpan<Vector3> points = new[]
                {
                    new Vector3(data.WorldBounds.xMin, data.WorldBounds.yMin, 0),
                    new Vector3(data.WorldBounds.xMin, data.WorldBounds.yMax, 0),
                    new Vector3(data.WorldBounds.xMax, data.WorldBounds.yMax, 0),
                    new Vector3(data.WorldBounds.xMax, data.WorldBounds.yMin, 0),
                };
                Gizmos.DrawLineStrip(points, true);
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
                Gizmos.color = new Color(60f/255f, 179f/255f, 113/255f);
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
                Gizmos.color = new Color(106f/255f, 90f/255f, 205/255f);
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

            var isAll = data.DebugData.DeletionReason == WorldMap.EDeletionReason.All;
            {
                if (isAll || data.DebugData.DeletionReason == WorldMap.EDeletionReason.Overlap)
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
                if (isAll || data.DebugData.DeletionReason == WorldMap.EDeletionReason.Isolation)
                {
                    // Draw isolation
                    Gizmos.color = Color.white;
                    var deleted = deletions[WorldMap.EDeletionReason.Isolation];
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
                if (isAll || data.DebugData.DeletionReason == WorldMap.EDeletionReason.OutOfBounds)
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