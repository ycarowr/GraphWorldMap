using System;
using System.Collections.Generic;
using System.Globalization;
using Tools.Attributes;
using Tools.Graphs;
using Tools.Line;
using Tools.WorldMapCore.Database;
using Tools.WorldMapCore.Runtime;
using UnityEngine;

namespace Game
{
    public class GameWorldMap : BaseWorldMapController<GameWorldMapNode, WorldMapParameters>
    {
        [SerializeField] private float maxDistance;
        [SerializeField] private float maxConnections;
        private List<Graph<GameGraphWorldMapNode>> MapGraphs = new List<Graph<GameGraphWorldMapNode>>();
        private Dictionary<Graph<GameGraphWorldMapNode>, Color> Colors = new Dictionary<Graph<GameGraphWorldMapNode>, Color>();
        private Graph<GameGraphWorldMapNode> CurrentGraph;

        [Button]
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
        
        [Button]
        public void Graph()
        {
            if (WorldMap == null)
            {
                return;
            }
            MapGraphs.Clear();
            Colors.Clear();
            foreach (var start in WorldMap.Start)
            {
                foreach (var end in WorldMap.End)
                {
                    var mapGraph = new Graph<GameGraphWorldMapNode>();
                    var nodeGraphStart = new GameGraphWorldMapNode(start);
                    var nodeGraphEnd = new GameGraphWorldMapNode(end);
                    mapGraph.Register(nodeGraphStart);
                    mapGraph.Register(nodeGraphEnd);
                    
                    foreach (var node in WorldMap.Nodes)
                    {
                        var nodeGraph = new GameGraphWorldMapNode(node);
                        mapGraph.Register(nodeGraph);
                    }
                    MapGraphs.Add(mapGraph);
                    Colors.Add(mapGraph, new Color(
                        UnityEngine.Random.Range(0f, 1f),
                        UnityEngine.Random.Range(0f, 1f),
                        UnityEngine.Random.Range(0f, 1f)));
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

                        var worldPositionA = nodeA.Data.WorldPosition;
                        var worldPositionB = nodeB.Data.WorldPosition;

                        var distance = Vector2.Distance(worldPositionA, worldPositionB);
                        var amount = mapGraph.FindAmountOfConnections(nodeA);

                        mapGraph.Connect(nodeA, nodeB, distance);
                    }
                }
            }
        }

#if UNITY_EDITOR
        List<Vector3> lines = new List<Vector3>();
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
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
                    lines.Add(nodeA.Data.WorldPosition);
                    lines.Add(nodeB.Key.Data.WorldPosition);
                    var midpointX = (nodeB.Key.Data.WorldPosition.x + nodeA.Data.WorldPosition.x) / 2;
                    var midpointY = (nodeB.Key.Data.WorldPosition.y + nodeA.Data.WorldPosition.y) / 2;
                    var cost = nodeB.Value.ToString(CultureInfo.InvariantCulture)[..3];
                    UnityEditor.Handles.Label(new Vector3(midpointX, midpointY, 0), cost);
                }
            }

            Gizmos.DrawLineList(lines.ToArray());
        }
#endif
    }
}
