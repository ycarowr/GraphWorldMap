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
            // Register Start Nodes
            foreach (var start in starting)
            {
                var graph = new Graph<WorldMapNode>();
                graphs.Add(start, graph);
                graph.Register(start);
            }

            // Register middle Nodes
            foreach (var node in nodes)
            {
                if (starting.Contains(node) || ending.Contains(node))
                {
                    continue;
                }

                var startIndex = FindNodeLaneIndex(node.Bounds, data);
                if (startIndex == -1)
                {
                    continue;
                }

                var startNode = starting[startIndex];
                graphs[startNode].Register(node);
            }

            // Register End nodes
            foreach (var pair in graphs)
            {
                var graphNodes = pair.Value;
                var endIndex = FindNearestEndIndex(graphNodes, ending);
                if (endIndex == -1)
                {
                    continue;
                }

                var end = ending[endIndex];
                pair.Value.Register(end);
            }

            
            // Connect everything
            foreach (var pair in graphs)
            {
                var graph = pair.Value;
                for (var index = 0; index < graph.Nodes.Count - 1; index++)
                {
                    var nextIndex = index + 1;
                    var node = graph.Nodes[index];
                    var nodeNext = graph.Nodes[nextIndex];
                    var distance = Vector3.Distance(node.Bounds.center, nodeNext.Bounds.center);
                    graph.Connect(node, nodeNext, distance);
                }
            }
        }

        private static int FindNearestEndIndex(Graph<WorldMapNode> graphNodes, List<WorldMapNode> ending)
        {
            var nearest = float.MaxValue;
            var nearestIndex = -1;
            for (var index = 0; index < ending.Count; index++)
            {
                var node = ending[index];
                var distance = Vector3.Distance(node.Bounds.center, graphNodes.Nodes[^1].Bounds.center);
                if (distance < nearest)
                {
                    nearest = distance;
                    nearestIndex = index;
                }
            }

            return nearestIndex;
        }

        private static int FindNodeLaneIndex(Rect nodeBounds, WorldMapStaticData data)
        {
            var lanes = data.Lanes;
            var nearest = float.MaxValue;
            var nearestIndex = -1;
            for (var index = 0; index < lanes.Count; index++)
            {
                var lane = lanes[index];
                var distance = Vector3.Distance(lane.center, nodeBounds.center);
                if (distance < nearest)
                {
                    nearest = distance;
                    nearestIndex = index;
                }
            }

            return nearestIndex;
        }
    }
}