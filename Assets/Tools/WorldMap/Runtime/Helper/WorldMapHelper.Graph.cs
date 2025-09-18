using System.Collections.Generic;
using System.Linq;
using Tools.Graphs;
using Tools.WorldMapCore.Database;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static partial class WorldMapHelper
    {
        public static void CreateGraph(List<Graph<WorldMapNode>> graphRegistry, WorldMapStaticData data,
            List<WorldMapNode> nodes, List<WorldMapNode> starting, List<WorldMapNode> ending)
        {
            // Register Start Nodes
            for (var index = 0; index < starting.Count; index++)
            {
                var start = starting[index];
                var graph = new Graph<WorldMapNode>();
                graphRegistry.Add(graph);
                graph.Register(start);
            }


            // Register middle Nodes
            for (var index = 0; index < nodes.Count; index++)
            {
                var node = nodes[index];
                if (starting.Contains(node) || ending.Contains(node))
                {
                    continue;
                }

                var startIndex = FindNodeLaneIndex(node.Bounds, data);
                if (startIndex == -1)
                {
                    continue;
                }

                graphRegistry[startIndex].Register(node);
            }

            // Register End nodes
            foreach (var graph in graphRegistry)
            {
                var endIndex = FindNearestEndIndex(graph, ending);
                if (endIndex == -1)
                {
                    continue;
                }

                var end = ending[endIndex];
                graph.Register(end);
            }


            for (var index = 0; index < graphRegistry.Count; index++)
            {
                var end = graphRegistry[index].Nodes.Last();
                var graph = graphRegistry[index];
                if (data.Parameters.test)
                {
                    graph.Nodes.Sort(new WorldMapNodeComparePointDistance(end.Bounds.center));
                }
                else
                {
                    graph.Nodes.Sort(new WorldMapNodeCompareBottomTop());
                }
            }

            // Create connections
            var connections = new List<Graph<WorldMapNode>>();

            for (var index = 0; index < graphRegistry.Count - 1; index++)
            {
                var connection = new Graph<WorldMapNode>();
                var indexNext = index + 1;

                var graph = graphRegistry[index];
                var graphNext = graphRegistry[indexNext];

                List<WorldMapNode> sort;
                List<WorldMapNode> sortNext;
                if (data.Parameters.Orientation == WorldMapParameters.OrientationGraph.BottomTop)
                {
                    sort = FindBorderNodes(graph, new WorldMapNodeCompareLeftRight());
                    sortNext = FindBorderNodes(graphNext, new WorldMapNodeCompareLeftRight());
                }
                else
                {
                    sort = FindBorderNodes(graph, new WorldMapNodeCompareBottomTop());
                    sortNext = FindBorderNodes(graphNext, new WorldMapNodeCompareBottomTop());
                }

                foreach (var node in starting)
                {
                    sort.Remove(node);
                }

                foreach (var node in ending)
                {
                    sort.Remove(node);
                }

                foreach (var node in starting)
                {
                    sortNext.Remove(node);
                }

                foreach (var node in ending)
                {
                    sortNext.Remove(node);
                }

                for (var connectionCount = 0;
                     connectionCount < data.Parameters.AmountOfLaneConnections;
                     connectionCount++)
                {
                    var rightMost = sort.Last();
                    sort.Remove(rightMost);
                    var nearest = FindNearest(sortNext, rightMost, connection.Nodes);
                    if (nearest != null)
                    {
                        connection.Register(rightMost);
                        connection.Register(nearest);
                        var distance = Vector3.Distance(rightMost.Bounds.center, nearest.Bounds.center);
                        connection.Connect(rightMost, nearest, distance);
                    }
                }

                connections.Add(connection);
            }

            foreach (var connection in connections)
            {
                graphRegistry.Add(connection);
            }

            // Connect everything
            for (var startingIndex = 0; startingIndex < data.Parameters.AmountStart; startingIndex++)
            {
                var graph = graphRegistry[startingIndex];
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

        private static WorldMapNode FindNearest(List<WorldMapNode> nodes, WorldMapNode node,
            List<WorldMapNode> exceptions)
        {
            var nearest = float.MaxValue;
            var nearestIndex = -1;
            for (var index = 0; index < nodes.Count; index++)
            {
                var worldMapNode = nodes[index];
                if (exceptions.Contains(worldMapNode))
                {
                    continue;
                }

                var distance = Vector3.Distance(node.Bounds.center, worldMapNode.Bounds.center);
                if (distance < nearest)
                {
                    nearest = distance;
                    nearestIndex = index;
                }
            }

            return nodes[nearestIndex];
        }

        private static List<WorldMapNode> FindBorderNodes(Graph<WorldMapNode> graphNodes,
            IComparer<WorldMapNode> comparer)
        {
            var borderNodes = new List<WorldMapNode>();
            borderNodes.AddRange(graphNodes.Nodes);
            borderNodes.Sort(comparer);
            return borderNodes;
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