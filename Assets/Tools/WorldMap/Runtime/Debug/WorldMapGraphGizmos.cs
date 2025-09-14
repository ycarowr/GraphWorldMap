#if UNITY_EDITOR

using System.Collections.Generic;
using System.Globalization;
using Tools.Graphs;
using Tools.WorldMapCore.Database;
using UnityEditor;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static class WorldMapGraphGizmos
    {
        public static void DrawGizmos(List<Graph<WorldMapNode>> mapGraphs, WorldMapStaticData data)
        {
            if (data.DebugData.Mode != WorldMapParameters.DebugData.DrawMode.All &&
                data.DebugData.Mode != WorldMapParameters.DebugData.DrawMode.Graph)
            {
                return;
            }

            if (mapGraphs == null || mapGraphs.Count == 0)
            {
                return;
            }

            Dictionary<Graph<WorldMapNode>, Color> Colors = new();
            var CurrentGraph = mapGraphs[0];
            List<Vector3> lines = new();

            {
                // Draw Graph
                if (CurrentGraph == null)
                {
                    return;
                }

                Gizmos.color = Color.cyan;
                lines.Clear();
                foreach (var connection in CurrentGraph.Connections)
                {
                    var nodeA = connection.Key;
                    var targets = connection.Value;
                    foreach (var nodeB in targets)
                    {
                        lines.Add(nodeA.WorldPosition);
                        lines.Add(nodeB.Key.WorldPosition);
                        var midpointX = (nodeB.Key.WorldPosition.x + nodeA.WorldPosition.x) / 2;
                        var midpointY = (nodeB.Key.WorldPosition.y + nodeA.WorldPosition.y) / 2;
                        var distance = nodeB.Value.ToString(CultureInfo.InvariantCulture)[..4];
                        Handles.Label(new Vector3(midpointX, midpointY, 0), distance);
                    }
                }

                Gizmos.DrawLineList(lines.ToArray());
            }
        }
    }
}

#endif