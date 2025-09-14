using System.Collections.Generic;
using Tools.Graphs;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static partial class WorldMapHelper
    {
        public static void CreateGraph(Dictionary<WorldMapNode, Graph<WorldMapNode>> graphs, WorldMapStaticData data,
            List<WorldMapNode> nodes, List<WorldMapNode> starting, List<WorldMapNode> ending)
        {
            foreach (var start in starting)
            {
                var graph = new Graph<WorldMapNode>();
                graphs.Add(start, graph);
                graph.Register(start);
            }

            foreach (var node in nodes)
            {
                if (starting.Contains(node) || ending.Contains(node))
                {
                    continue;
                }

                var start = FindLaneStart(node.Bounds, starting, data);
                if (start != null)
                {
                    graphs[start].Register(node);
                }
            }

            foreach (var pair in graphs)
            {
                var graph = pair.Value;
                for (var index = 0; index < graph.Nodes.Count - 1; index++)
                {
                    var nextIndex = index + 1;
                    var node = graph.Nodes[index];
                    var nodeNext = graph.Nodes[nextIndex];
                    var distance = Vector3.Distance(node.WorldPosition, nodeNext.WorldPosition);
                    graph.Connect(node, nodeNext, distance);
                }
            }
        }

        private static WorldMapNode FindLaneStart(Rect nodeBounds, List<WorldMapNode> starting, WorldMapStaticData data)
        {
            var lanes = data.Lanes;
            var laneIndex = -1;
            for (var index = 0; index < lanes.Count; index++)
            {
                var lane = lanes[index];
                if (CheckRectContains(lane, nodeBounds))
                {
                    laneIndex = index;
                    break;
                }
            }

            if (laneIndex == -1)
            {
                return null;
            }

            return starting[laneIndex];
        }

        // private static WorldMapNode FindLaneStart(Rect nodeBounds, List<WorldMapNode> starting)
        // {
        //     var nearest = float.MaxValue;
        //     WorldMapNode nearestNode = null;
        //     foreach (var node in starting)
        //     {
        //         var distance = Vector3.Distance(node.WorldPosition, nodeBounds.position);
        //         if (distance < nearest)
        //         {
        //             nearest = distance;
        //             nearestNode = node;
        //         }
        //     }
        //     
        //     return nearestNode;
        // }
    }
}