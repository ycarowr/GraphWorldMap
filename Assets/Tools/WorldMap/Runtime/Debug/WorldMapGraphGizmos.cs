#if UNITY_EDITOR

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tools.Graphs;
using Tools.WorldMapCore.Database;
using UnityEditor;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static class WorldMapGraphGizmos
    {
        private static readonly List<Color> colors = new();

        public static void DrawGizmos(Dictionary<WorldMapNode, Graph<WorldMapNode>> graphs, WorldMapStaticData data)
        {
            if (data.DebugData.Mode != WorldMapParameters.DebugData.DrawMode.All &&
                data.DebugData.Mode != WorldMapParameters.DebugData.DrawMode.Graph)
            {
                return;
            }

            if (graphs == null || graphs.Count == 0)
            {
                return;
            }

            // Draw Graph
            if (colors.Count != graphs.Count)
            {
                colors.Clear();
                foreach (var graph in graphs)
                {
                    colors.Add(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
                }
            }

            List<Vector3> lines = new();
            var mapGraphs = graphs.Values.ToList();
            for (var index = 0; index < mapGraphs.Count; index++)
            {
                var currentGraph = mapGraphs[index];
                Gizmos.color = colors[index];
                lines.Clear();
                foreach (var connection in currentGraph.Connections)
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