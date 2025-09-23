using System.Collections.Generic;
using System.Globalization;
using Tools.Graphs;
using Tools.WorldMapCore.Database;
using UGizmo;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static class WorldMapGraphGizmos
    {
        public static readonly List<Color> colors = new()
        {
            Color.blue,
            Color.magenta,
            Color.red,
            Color.cyan,
            new Color(46/255f,255/255f,87f/255f),
            new Color(244f/255f,164f/255f,96f/255f),
            new Color(151f/255f,170f/255f,27f/255f),
        };
        
        
        public static void DrawGizmos(List<Graph<WorldMapNode>> graphs, WorldMapStaticData data)
        {
            if (data.Parameters.DebugValues.Mode != WorldMapParameters.DebugData.DrawMode.All &&
                data.Parameters.DebugValues.Mode != WorldMapParameters.DebugData.DrawMode.Graph &&
                data.Parameters.DebugValues.Mode != WorldMapParameters.DebugData.DrawMode.Distances)
            {
                return;
            }

            if (graphs == null || graphs.Count == 0)
            {
                return;
            }

            // Draw Graph
            if (colors.Count < graphs.Count)
            {
                var delta = graphs.Count - colors.Count;
                for (var index = 0; index < delta; index++)
                {
                    colors.Add(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
                }
            }

            List<Vector3> lines = new();
            for (var index = 0; index < graphs.Count; index++)
            {
                var currentGraph = graphs[index];
                lines.Clear();
                foreach (var connection in currentGraph.Connections)
                {
                    var nodeA = connection.Key;
                    var targets = connection.Value;
                    foreach (var nodeB in targets)
                    {
                        lines.Add(nodeA.Center);
                        lines.Add(nodeB.Key.Center);
// #if UNITY_EDITOR
//                         if (data.Parameters.DebugValues.Mode != WorldMapParameters.DebugData.DrawMode.All &&
//                             data.Parameters.DebugValues.Mode != WorldMapParameters.DebugData.DrawMode.Distances)
//                         {
//                             continue;
//                         }
//
//                         var midpointX = (nodeB.Key.Center.x + nodeA.Center.x) / 2;
//                         var midpointY = (nodeB.Key.Center.y + nodeA.Center.y) / 2;
//                         var text = nodeB.Value.ToString(CultureInfo.InvariantCulture);
//                         if (text.Length < 4)
//                         {
//                             text = text[..1];
//                         }
//                         else
//                         {
//                             text = text[..4];
//                         }
//                         UnityEditor.Handles.Label(new Vector3(midpointX, midpointY, 0), text);
// #endif
                    }
                }
                UGizmos.DrawLineList(lines.ToArray(), colors[index]);
            }
        }
    }
}