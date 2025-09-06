#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static class WorldMapGizmos
    {
        public static void DrawGizmos(WorldMapStaticData data, List<Node> nodes,
            Dictionary<WorldMap.EDeletionReason, List<Node>> deletions)
        {
            {
                // Draw center 
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(data.WorldBounds.center, 0.3f);
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
                Gizmos.color = Color.blue;
                for (var i = 0; i < nodes.Count; i++)
                {
                    var node = nodes[i];
                    ReadOnlySpan<Vector3> points = new[]
                    {
                        new Vector3(node.WorldRect.xMin, node.WorldRect.yMin, 0),
                        new Vector3(node.WorldRect.xMin, node.WorldRect.yMax, 0),
                        new Vector3(node.WorldRect.xMax, node.WorldRect.yMax, 0),
                        new Vector3(node.WorldRect.xMax, node.WorldRect.yMin, 0),
                    };
                    Gizmos.DrawLineStrip(points, true);
                }
            }

            {
                // Draw collisions
                Gizmos.color = Color.red;
                var deleted = deletions[WorldMap.EDeletionReason.Overlap];
                for (var i = 0; i < deleted.Count; i++)
                {
                    var node = deleted[i];
                    ReadOnlySpan<Vector3> points = new[]
                    {
                        new Vector3(node.WorldRect.xMin, node.WorldRect.yMin, 0),
                        new Vector3(node.WorldRect.xMin, node.WorldRect.yMax, 0),
                        new Vector3(node.WorldRect.xMax, node.WorldRect.yMax, 0),
                        new Vector3(node.WorldRect.xMax, node.WorldRect.yMin, 0),
                    };
                    Gizmos.DrawLineStrip(points, true);
                }
            }

            {
                // Draw isolation
                Gizmos.color = Color.white;
                var deleted = deletions[WorldMap.EDeletionReason.Isolation];
                for (var i = 0; i < deleted.Count; i++)
                {
                    var node = deleted[i];
                    ReadOnlySpan<Vector3> points = new[]
                    {
                        new Vector3(node.WorldRect.xMin, node.WorldRect.yMin, 0),
                        new Vector3(node.WorldRect.xMin, node.WorldRect.yMax, 0),
                        new Vector3(node.WorldRect.xMax, node.WorldRect.yMax, 0),
                        new Vector3(node.WorldRect.xMax, node.WorldRect.yMin, 0),
                    };
                    Gizmos.DrawLineStrip(points, true);
                }
            }
        }
    }
}
#endif