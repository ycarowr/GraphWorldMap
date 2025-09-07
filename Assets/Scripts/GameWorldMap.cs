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
        private Graph<GameGraphWorldMapNode> MapGraph;

        [Button]
        public void Graph()
        {
            if (WorldMap == null)
            {
                return;
            }
            
            MapGraph = new Graph<GameGraphWorldMapNode>();

            foreach (var node in WorldMap.Nodes)
            {
                var nodeGraph = new GameGraphWorldMapNode(node);
                MapGraph.Register(nodeGraph);
            }

            foreach (var nodeB in MapGraph.Nodes)
            {
                foreach (var nodeA in MapGraph.Nodes)
                {
                    if (nodeA == nodeB)
                    {
                        // won't do for itself
                        continue;
                    }
                    var worldPositionA = nodeA.Data.WorldPosition;
                    var worldPositionB = nodeB.Data.WorldPosition;
                    
                    var distance = Vector2.Distance(worldPositionA, worldPositionB);
                    var amount = MapGraph.FindAmountOfConnections(nodeA);
                    if (distance < maxDistance && amount < maxConnections)
                    {
                        MapGraph.Connect(nodeA, nodeB, distance);
                    }
                }
            }
        }

#if UNITY_EDITOR
        List<Vector3> lines = new List<Vector3>();
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (MapGraph == null)
            {
                return;
            }

            Gizmos.color = Color.white;
            lines.Clear();
            foreach (var connection in MapGraph.Connections)
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
                    UnityEditor.Handles.Label( new Vector3(midpointX, midpointY, 0), cost);
                }
            }
            Gizmos.DrawLineList(lines.ToArray());
        }
#endif
    }
}