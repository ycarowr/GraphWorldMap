using System.Collections.Generic;
using System.Globalization;
using Tools.Graphs;
using Tools.WorldMapCore.Database;
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
            new Color(46f / 255f, 255f / 255f, 87f / 255f),
            new Color(244f / 255f, 164f / 255f, 96f / 255f),
            new Color(151f / 255f, 170f / 255f, 27f / 255f),
        };

        public static void DrawTextDistance(List<Graph<WorldMapNode>> graphs, WorldMapStaticData data,
            GameObject worldMapRoot)
        {
            if (data.Parameters.DebugValues.Mode != WorldMapParameters.DebugData.EDrawMode.All &&
                data.Parameters.DebugValues.Mode != WorldMapParameters.DebugData.EDrawMode.Distances)
            {
                return;
            }
            
            DrawBounders(data, worldMapRoot);

            for (var index = 0; index < graphs.Count; index++)
            {
                var currentGraph = graphs[index];
                foreach (var connection in currentGraph.Connections)
                {
                    var nodeA = connection.Key;
                    var targets = connection.Value;
                    foreach (var nodeB in targets)
                    {
                        var midpointX = (nodeB.Key.Center.x + nodeA.Center.x) / 2;
                        var midpointY = (nodeB.Key.Center.y + nodeA.Center.y) / 2;
                        var text = ((int)nodeB.Value).ToString();
                        var position = new Vector3(midpointX, midpointY, WorldMapGizmos.ZPOSITION_DISTANCE);
                        CreateText(position, data, worldMapRoot, text);
                    }
                }
            }
        }

        private static void DrawBounders(WorldMapStaticData data, GameObject worldMapRoot)
        {
            var bottomLeft = data.WorldBounds.min;
            var bottomRight = data.WorldBounds.min + new Vector2(data.WorldBounds.xMax, 0);
            var topLeft = data.WorldBounds.min + new Vector2(0, data.WorldBounds.yMax);
            var topRight = data.WorldBounds.max;
            CreateText(bottomLeft, data, worldMapRoot, bottomLeft.ToString());
            CreateText(bottomRight, data, worldMapRoot, bottomRight.ToString());
            CreateText(topLeft, data, worldMapRoot, topLeft.ToString());
            CreateText(topRight, data, worldMapRoot, topRight.ToString());
        }

        private static void CreateText(Vector3 position, WorldMapStaticData data, GameObject worldMapRoot, string text)
        {
            var tmpText = Object.Instantiate(data.Parameters.DebugDistanceText, worldMapRoot.transform);
            tmpText.transform.position = position;
            tmpText.text = text;
            tmpText.fontSize = data.Parameters.FontSize;
        }

        public static void DrawGizmos(List<Graph<WorldMapNode>> graphs,
            List<Graph<WorldMapNode>> regionConnectionsRegistry, WorldMapStaticData data)
        {
            if (data.Parameters.DebugValues.Mode != WorldMapParameters.DebugData.EDrawMode.All &&
                data.Parameters.DebugValues.Mode != WorldMapParameters.DebugData.EDrawMode.Graph &&
                data.Parameters.DebugValues.Mode != WorldMapParameters.DebugData.EDrawMode.Distances)
            {
                return;
            }

            if (graphs == null || graphs.Count == 0)
            {
                return;
            }

            // Setup colors
            if (colors.Count < graphs.Count + regionConnectionsRegistry.Count)
            {
                var delta = graphs.Count + regionConnectionsRegistry.Count - colors.Count;
                for (var index = 0; index < delta; index++)
                {
                    colors.Add(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
                }
            }

            // Draw Graph
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
                        lines.Add(nodeA.Center + WorldMapGizmos.ZPOSITION_LINES);
                        lines.Add(nodeB.Key.Center + WorldMapGizmos.ZPOSITION_LINES);
                    }
                }

                Lines.DrawLineStrip(lines.ToArray(), colors[index], false);
            }

            for (var index = 0; index < regionConnectionsRegistry.Count; index++)
            {
                var currentGraph = regionConnectionsRegistry[index];
                lines.Clear();
                foreach (var connection in currentGraph.Connections)
                {
                    var nodeA = connection.Key;
                    var targets = connection.Value;
                    foreach (var nodeB in targets)
                    {
                        lines.Add(nodeA.Center + WorldMapGizmos.ZPOSITION_LINES);
                        lines.Add(nodeB.Key.Center + WorldMapGizmos.ZPOSITION_LINES);
                    }
                }

                Lines.DrawLineList(lines.ToArray(), colors[graphs.Count + index]);
            }
        }
    }
}