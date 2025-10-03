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
        public static readonly Vector3 ZPOSITION_LINES = new Vector3(0, 0, -0.15f);
        public const float ZPOSITION_DISTANCE = -0.17f;
        
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
                        new Vector3(region.Bounds.xMin, region.Bounds.yMin, ZPOSITION_REGIONBOUNDS),
                        new Vector3(region.Bounds.xMin, region.Bounds.yMax, ZPOSITION_REGIONBOUNDS),
                        new Vector3(region.Bounds.xMax, region.Bounds.yMax, ZPOSITION_REGIONBOUNDS),
                        new Vector3(region.Bounds.xMax, region.Bounds.yMin, ZPOSITION_REGIONBOUNDS),
                    };
                    Lines.DrawLineStrip(points, new Color(111f/255f, 29f/255f, 27f/255f));
                }
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
                        new Vector3(node.Bounds.xMin, node.Bounds.yMin, ZPOSITION_NODES),
                        new Vector3(node.Bounds.xMin, node.Bounds.yMax, ZPOSITION_NODES),
                        new Vector3(node.Bounds.xMax, node.Bounds.yMax, ZPOSITION_NODES),
                        new Vector3(node.Bounds.xMax, node.Bounds.yMin, ZPOSITION_NODES),
                    };
                    Lines.DrawLineStrip(points, new Color(75f/255f, 175f/255f, 40f/255f));
                }
            }

            {
                // Start
                for (var i = 0; i < start.Count; i++)
                {
                    var node = start[i];
                    ReadOnlySpan<Vector3> points = new[]
                    {
                        new Vector3(node.Bounds.xMin, node.Bounds.yMin, ZPOSITION_STARTEND_NODES),
                        new Vector3(node.Bounds.xMin, node.Bounds.yMax, ZPOSITION_STARTEND_NODES),
                        new Vector3(node.Bounds.xMax, node.Bounds.yMax, ZPOSITION_STARTEND_NODES),
                        new Vector3(node.Bounds.xMax, node.Bounds.yMin, ZPOSITION_STARTEND_NODES),
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
                        new Vector3(node.Bounds.xMin, node.Bounds.yMin, ZPOSITION_STARTEND_NODES),
                        new Vector3(node.Bounds.xMin, node.Bounds.yMax, ZPOSITION_STARTEND_NODES),
                        new Vector3(node.Bounds.xMax, node.Bounds.yMax, ZPOSITION_STARTEND_NODES),
                        new Vector3(node.Bounds.xMax, node.Bounds.yMin, ZPOSITION_STARTEND_NODES),
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
                            new Vector3(node.Bounds.xMin, node.Bounds.yMin, ZPOSITION_REMOVED_NODES),
                            new Vector3(node.Bounds.xMin, node.Bounds.yMax, ZPOSITION_REMOVED_NODES),
                            new Vector3(node.Bounds.xMax, node.Bounds.yMax, ZPOSITION_REMOVED_NODES),
                            new Vector3(node.Bounds.xMax, node.Bounds.yMin, ZPOSITION_REMOVED_NODES),
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
                            new Vector3(node.Bounds.xMin, node.Bounds.yMin, ZPOSITION_REMOVED_NODES),
                            new Vector3(node.Bounds.xMin, node.Bounds.yMax, ZPOSITION_REMOVED_NODES),
                            new Vector3(node.Bounds.xMax, node.Bounds.yMax, ZPOSITION_REMOVED_NODES),
                            new Vector3(node.Bounds.xMax, node.Bounds.yMin, ZPOSITION_REMOVED_NODES),
                        };
                        Lines.DrawLineStrip(points, Color.yellow);
                    }
                }
            }
        }
    }
}