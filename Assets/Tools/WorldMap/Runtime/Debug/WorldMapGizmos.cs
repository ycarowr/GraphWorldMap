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
            Dictionary<WorldMapParameters.EDeletionReason, List<WorldMapNode>> deletions)
        {
            if (data.Parameters.DebugValues.Mode != WorldMapParameters.DebugData.EDrawMode.All &&
                data.Parameters.DebugValues.Mode != WorldMapParameters.DebugData.EDrawMode.Nodes)
            {
                return;
            }

            {
                var regions = data.Parameters.Regions;
                for (var i = 0; i < regions.Length; i++)
                {
                    var region = regions[i];
                    ReadOnlySpan<Vector3> points = new[]
                    {
                        new Vector3(region.Bounds.xMin, region.Bounds.yMin, 0),
                        new Vector3(region.Bounds.xMin, region.Bounds.yMax, 0),
                        new Vector3(region.Bounds.xMax, region.Bounds.yMax, 0),
                        new Vector3(region.Bounds.xMax, region.Bounds.yMin, 0),
                    };
                    Lines.DrawLineStrip(points, Color.yellow);
                }
            }

            {
                // Draw center 
                UGizmos.DrawWireSphere(data.WorldBounds.center, 0.2f, Color.magenta);
            }

            {
                for (var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    if (start.Contains(node) || end.Contains(node))
                    {
                        continue;
                    }
                    
                    ReadOnlySpan<Vector3> points = new[]
                    {
                        new Vector3(node.Bounds.xMin, node.Bounds.yMin, 0),
                        new Vector3(node.Bounds.xMin, node.Bounds.yMax, 0),
                        new Vector3(node.Bounds.xMax, node.Bounds.yMax, 0),
                        new Vector3(node.Bounds.xMax, node.Bounds.yMin, 0),
                    };
                    Lines.DrawLineStrip(points, Color.green);
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
                    Lines.DrawLineStrip(points, new Color(60f / 255f, 179f / 255f, 113 / 255f));
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
                    Lines.DrawLineStrip(points, new Color(106f / 255f, 90f / 255f, 205 / 255f));
                }
            }

            var isAll = data.Parameters.DebugValues.DeletionReason == WorldMapParameters.EDeletionReason.All;
            {
                if (isAll || data.Parameters.DebugValues.DeletionReason == WorldMapParameters.EDeletionReason.Overlap)
                {
                    // Draw Overlap
                    var deleted = deletions[WorldMapParameters.EDeletionReason.Overlap];
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
                        Lines.DrawLineStrip(points, Color.red);
                    }
                }
            }

            {
                if (isAll || data.Parameters.DebugValues.DeletionReason ==
                    WorldMapParameters.EDeletionReason.OutOfBounds)
                {
                    // Draw Bounds
                    var deleted = deletions[WorldMapParameters.EDeletionReason.OutOfBounds];
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
                        Lines.DrawLineStrip(points, Color.yellow);
                    }
                }
            }
        }
    }
}