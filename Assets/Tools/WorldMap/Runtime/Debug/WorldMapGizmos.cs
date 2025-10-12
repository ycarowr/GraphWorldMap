using System;
using System.Collections.Generic;
using Tools.WorldMapCore.Database;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static class WorldMapGizmos
    {
        public const float ZPOSITION_REGIONBOUNDS = 0.1f;
        public const float ZPOSITION_REMOVED_NODES = 0.05f;
        public const float ZPOSITION_NODES = 0f;
        public const float ZPOSITION_STARTEND_NODES = -0.1f;
        public const float ZPOSITION_DISTANCE = -0.17f;
        public static readonly Vector3 ZPOSITION_LINES = new(0, 0, -0.15f);

        public static void DrawGizmos(WorldMapStaticData data,
            List<WorldMapNode> nodes,
            List<WorldMapNode> start,
            List<WorldMapNode> end,
            List<WorldMapRegion> regions,
            Dictionary<EDeletionReason, List<WorldMapNode>> deletions)
        {
            if (data.Parameters.DebugValues.Mode != WorldMapParameters.DebugData.EDrawMode.All &&
                data.Parameters.DebugValues.Mode != WorldMapParameters.DebugData.EDrawMode.Nodes)
            {
                return;
            }

            {
                var generatedRegions = regions;
                for (var i = 0; i < generatedRegions.Count; i++)
                {
                    var region = generatedRegions[i].Bound;
                    ReadOnlySpan<Vector3> points = new[]
                    {
                        new Vector3(region.xMin, region.yMin, ZPOSITION_REGIONBOUNDS),
                        new Vector3(region.xMin, region.yMax, ZPOSITION_REGIONBOUNDS),
                        new Vector3(region.xMax, region.yMax, ZPOSITION_REGIONBOUNDS),
                        new Vector3(region.xMax, region.yMin, ZPOSITION_REGIONBOUNDS),
                    };
                    Lines.DrawLineStrip(points, Color.red);
                }
                
                Lines.DrawLineStrip( new Vector3[]
                {
                    data.WorldBounds.min,
                    data.WorldBounds.min + new Vector2(data.WorldBounds.xMax, 0),
                    data.WorldBounds.max,
                    data.WorldBounds.min + new Vector2(0, data.WorldBounds.yMax),
                }, new Color(200f / 255f, 90f / 255f, 96f / 255f));
            }

            {
                for (var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    if (start.Contains(node) || end.Contains(node))
                    {
                        continue;
                    }

                    var bound = node.Bound;
                    ReadOnlySpan<Vector3> points = new[]
                    {
                        new Vector3(bound.xMin, bound.yMin, ZPOSITION_NODES),
                        new Vector3(bound.xMin, bound.yMax, ZPOSITION_NODES),
                        new Vector3(bound.xMax, bound.yMax, ZPOSITION_NODES),
                        new Vector3(bound.xMax, bound.yMin, ZPOSITION_NODES),
                    };
                    Lines.DrawLineStrip(points, new Color(75f / 255f, 175f / 255f, 40f / 255f));
                }
            }

            {
                // Start
                for (var i = 0; i < start.Count; i++)
                {
                    var node = start[i];
                    var bound = node.Bound;
                    ReadOnlySpan<Vector3> points = new[]
                    {
                        new Vector3(bound.xMin, bound.yMin, ZPOSITION_STARTEND_NODES),
                        new Vector3(bound.xMin, bound.yMax, ZPOSITION_STARTEND_NODES),
                        new Vector3(bound.xMax, bound.yMax, ZPOSITION_STARTEND_NODES),
                        new Vector3(bound.xMax, bound.yMin, ZPOSITION_STARTEND_NODES),
                    };
                    Lines.DrawLineStrip(points, new Color(60f / 255f, 179f / 255f, 113 / 255f));
                }
            }

            {
                // End
                for (var i = 0; i < end.Count; i++)
                {
                    var node = end[i];
                    var bound = node.Bound;
                    ReadOnlySpan<Vector3> points = new[]
                    {
                        new Vector3(bound.xMin, bound.yMin, ZPOSITION_STARTEND_NODES),
                        new Vector3(bound.xMin, bound.yMax, ZPOSITION_STARTEND_NODES),
                        new Vector3(bound.xMax, bound.yMax, ZPOSITION_STARTEND_NODES),
                        new Vector3(bound.xMax, bound.yMin, ZPOSITION_STARTEND_NODES),
                    };
                    Lines.DrawLineStrip(points, new Color(106f / 255f, 90f / 255f, 205 / 255f));
                }
            }

            var isAll = data.Parameters.DebugValues.DeletionReason == EDeletionReason.All;
            {
                if (isAll || data.Parameters.DebugValues.DeletionReason == EDeletionReason.Overlap)
                {
                    // Draw Overlap
                    var deleted = deletions[EDeletionReason.Overlap];
                    for (var i = 0; i < deleted.Count; i++)
                    {
                        var node = deleted[i];
                        var bound = node.Bound;
                        ReadOnlySpan<Vector3> points = new[]
                        {
                            new Vector3(bound.xMin, bound.yMin, ZPOSITION_REMOVED_NODES),
                            new Vector3(bound.xMin, bound.yMax, ZPOSITION_REMOVED_NODES),
                            new Vector3(bound.xMax, bound.yMax, ZPOSITION_REMOVED_NODES),
                            new Vector3(bound.xMax, bound.yMin, ZPOSITION_REMOVED_NODES),
                        };
                        Lines.DrawLineStrip(points, Color.red);
                    }
                }
            }

            {
                if (isAll || data.Parameters.DebugValues.DeletionReason ==
                    EDeletionReason.OutOfWorldBounds)
                {
                    // Draw Out of World Bounds
                    var deleted = deletions[EDeletionReason.OutOfWorldBounds];
                    for (var i = 0; i < deleted.Count; i++)
                    {
                        var node = deleted[i];
                        var bound = node.Bound;
                        ReadOnlySpan<Vector3> points = new[]
                        {
                            new Vector3(bound.xMin, bound.yMin, ZPOSITION_REMOVED_NODES),
                            new Vector3(bound.xMin, bound.yMax, ZPOSITION_REMOVED_NODES),
                            new Vector3(bound.xMax, bound.yMax, ZPOSITION_REMOVED_NODES),
                            new Vector3(bound.xMax, bound.yMin, ZPOSITION_REMOVED_NODES),
                        };
                        Lines.DrawLineStrip(points, Color.yellow);
                    }
                }
            }

            {
                if (isAll || data.Parameters.DebugValues.DeletionReason ==
                    EDeletionReason.OutOfRegionBounds)
                {
                    // Draw Out of Region Bounds
                    var deleted = deletions[EDeletionReason.OutOfRegionBounds];
                    for (var i = 0; i < deleted.Count; i++)
                    {
                        var node = deleted[i];
                        var bound = node.Bound;
                        ReadOnlySpan<Vector3> points = new[]
                        {
                            new Vector3(bound.xMin, bound.yMin, ZPOSITION_REMOVED_NODES),
                            new Vector3(bound.xMin, bound.yMax, ZPOSITION_REMOVED_NODES),
                            new Vector3(bound.xMax, bound.yMax, ZPOSITION_REMOVED_NODES),
                            new Vector3(bound.xMax, bound.yMin, ZPOSITION_REMOVED_NODES),
                        };
                        Lines.DrawLineStrip(points, Color.yellow);
                    }
                }
            }
        }
    }
}