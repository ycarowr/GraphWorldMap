using System.Collections.Generic;
using System.Globalization;
using Tools.Graphs;
using Tools.WorldMapCore.Database;
using UnityEditor;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public class WorldMapGraph
    {
        private readonly WorldMapStaticData Data;
        private readonly List<Graph<WorldMapNode>> MapGraphs = new();
        private readonly WorldMapParameters Parameters;
        private readonly WorldMap WorldMap;

        public WorldMapGraph(WorldMap worldMap, WorldMapParameters parameters)
        {
            WorldMap = worldMap;
            Parameters = parameters;
            Data = parameters.CreateData();
        }

        public void Create()
        {
            if (WorldMap == null)
            {
                return;
            }

            MapGraphs.Clear();
#if UNITY_EDITOR
            CurrentGraph = null;
            Colors.Clear();
#endif
            foreach (var start in WorldMap.Start)
            {
                foreach (var end in WorldMap.End)
                {
                    var mapGraph = new Graph<WorldMapNode>();
                    mapGraph.Register(start);
                    mapGraph.Register(end);

                    foreach (var node in WorldMap.Nodes)
                    {
                        mapGraph.Register(node);
                    }

                    MapGraphs.Add(mapGraph);
#if UNITY_EDITOR
                    Colors.Add(mapGraph, new Color(
                        Random.Range(0f, 1f),
                        Random.Range(0f, 1f),
                        Random.Range(0f, 1f)));
#endif
                }
            }

            foreach (var mapGraph in MapGraphs)
            {
                foreach (var nodeB in mapGraph.Nodes)
                {
                    foreach (var nodeA in mapGraph.Nodes)
                    {
                        if (nodeA == nodeB)
                        {
                            // won't do for itself
                            continue;
                        }

                        var worldPositionA = nodeA.WorldPosition;
                        var worldPositionB = nodeB.WorldPosition;
                        var distance = Vector2.Distance(worldPositionA, worldPositionB);
                        mapGraph.Connect(nodeA, nodeB, distance);
                    }
                }
            }
#if UNITY_EDITOR
            Next();
#endif
        }

#if UNITY_EDITOR
        private readonly Dictionary<Graph<WorldMapNode>, Color> Colors = new();
        private Graph<WorldMapNode> CurrentGraph;
        private readonly List<Vector3> lines = new();

        public void Next()
        {
            if (CurrentGraph == null)
            {
                CurrentGraph = MapGraphs[0];
            }
            else
            {
                var index = MapGraphs.IndexOf(CurrentGraph) + 1;
                if (index >= MapGraphs.Count)
                {
                    index = 0;
                }

                CurrentGraph = MapGraphs[index];
            }
        }

        public void OnDrawGizmos()
        {
            if (Data.DebugData.Mode != WorldMapParameters.DebugData.DrawMode.All && Data.DebugData.Mode != WorldMapParameters.DebugData.DrawMode.Graph)
            {
                return;
            }
            
            {
                // Draw Graph
                if (CurrentGraph == null)
                {
                    return;
                }

                if (!Colors.ContainsKey(CurrentGraph))
                {
                    return;
                }

                Gizmos.color = Colors[CurrentGraph];
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
#endif
    }
}