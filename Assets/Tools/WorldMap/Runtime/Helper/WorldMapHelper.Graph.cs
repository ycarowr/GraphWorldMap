using System.Collections.Generic;
using System.Linq;
using Tools.Graphs;
using Tools.WorldMapCore.Database;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static partial class WorldMapHelper
    {
        public static void CreateGraph(List<Graph<WorldMapNode>> graphRegistry,
            List<Graph<WorldMapNode>> regionConnectionsRegistry, WorldMapStaticData data,
            List<WorldMapNode> nodes, List<WorldMapNode> starting, List<WorldMapNode> ending)
        {
            RegisterStartNodes(graphRegistry, starting, data);
            IndexNodesByLanes(graphRegistry, data, nodes, starting, ending);
            RegisterEndNodes(graphRegistry, ending);
            SortNodes(graphRegistry, data);
            RegistryConnections(graphRegistry, regionConnectionsRegistry, data, starting, ending);
            ConnectAllNodes(graphRegistry, data, starting, ending);
            ConnectMissingStartNodes(graphRegistry, nodes, starting);
            ConnectMissingEndNodes(graphRegistry, nodes, ending);
        }

        private static void ConnectMissingEndNodes(List<Graph<WorldMapNode>> graphRegistry, List<WorldMapNode> nodes,
            List<WorldMapNode> ending)
        {
            // Connect end leftovers
            foreach (var end in ending)
            {
                var hasConnection = false;
                foreach (var graph in graphRegistry)
                {
                    hasConnection |= graph.HasConnection(end);
                }

                if (!hasConnection)
                {
                    var graph = new Graph<WorldMapNode>();
                    graphRegistry.Add(graph);
                    var nearest = FindNearest(nodes, end, ending);
                    graph.Register(nearest);
                    graph.Register(end);
                    graph.Connect(nearest, end, Vector3.Distance(nearest.Bounds.center, end.Bounds.center));
                }
            }
        }

        private static void ConnectMissingStartNodes(List<Graph<WorldMapNode>> graphRegistry, List<WorldMapNode> nodes,
            List<WorldMapNode> starting)
        {
            // Connect start leftovers
            foreach (var start in starting)
            {
                var hasConnection = false;
                foreach (var graph in graphRegistry)
                {
                    hasConnection |= graph.HasConnection(start);
                }

                if (!hasConnection)
                {
                    var graph = new Graph<WorldMapNode>();
                    graphRegistry.Add(graph);
                    var nearest = FindNearest(nodes, start, starting);
                    graph.Register(start);
                    graph.Register(nearest);
                    graph.Connect(start, nearest, Vector3.Distance(start.Bounds.center, nearest.Bounds.center));
                }
            }
        }

        private static void ConnectAllNodes(List<Graph<WorldMapNode>> graphRegistry, WorldMapStaticData data,
            List<WorldMapNode> starting, List<WorldMapNode> ending)
        {
            // Connect all registered nodes from each graph sequentially
            for (var graphIndex = 0; graphIndex < graphRegistry.Count; graphIndex++)
            {
                var graph = graphRegistry[graphIndex];
                for (var index = 0; index < graph.Nodes.Count - 1; index++)
                {
                    var nextIndex = index + 1;
                    var node = graph.Nodes[index];
                    var nodeNext = graph.Nodes[nextIndex];
                    if ((starting.Contains(node) && ending.Contains(nodeNext))
                        || (starting.Contains(nodeNext) && ending.Contains(node)))
                    {
                        // skip straight connection start->end
                        continue;
                    }

                    var distance = Vector3.Distance(node.Bounds.center, nodeNext.Bounds.center);
                    graph.Connect(node, nodeNext, distance);
                }
            }
        }

        private static void RegistryConnections(List<Graph<WorldMapNode>> graphRegistry,
            List<Graph<WorldMapNode>> regionConnectionsRegistry, WorldMapStaticData data,
            List<WorldMapNode> starting, List<WorldMapNode> ending)
        {
            // Create regions connections
            if (data.Parameters.HasConnections)
            {
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
                        sortNext.Remove(node);
                    }

                    foreach (var node in ending)
                    {
                        sort.Remove(node);
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

                // Lane connections are separated graphs
                foreach (var connection in connections)
                {
                    regionConnectionsRegistry.Add(connection);
                }
            }
        }

        private static void SortNodes(List<Graph<WorldMapNode>> graphRegistry, WorldMapStaticData data)
        {
            // Sort nodes according to the parameters, so we can have a proper direction to go to
            for (var index = 0; index < graphRegistry.Count; index++)
            {
                var end = graphRegistry[index].Nodes.Last();
                var graph = graphRegistry[index];
                if (data.Parameters.SortingMethod == WorldMapParameters.SortMethod.Distance)
                {
                    graph.Nodes.Sort(new WorldMapNodeComparePointDistance(end.Bounds.center));
                }
                else
                {
                    if (data.Parameters.Orientation == WorldMapParameters.OrientationGraph.LeftRight)
                    {
                        graph.Nodes.Sort(new WorldMapNodeCompareLeftRight());
                    }
                    else
                    {
                        graph.Nodes.Sort(new WorldMapNodeCompareBottomTop());
                    }
                }
            }
        }

        private static void RegisterEndNodes(List<Graph<WorldMapNode>> graphRegistry, List<WorldMapNode> ending)
        {
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
        }

        private static void IndexNodesByLanes(List<Graph<WorldMapNode>> graphRegistry, WorldMapStaticData data,
            List<WorldMapNode> nodes, List<WorldMapNode> starting,
            List<WorldMapNode> ending)
        {
            // Register nodes according to their regions
            for (var index = 0; index < nodes.Count; index++)
            {
                var node = nodes[index];
                if (starting.Contains(node) || ending.Contains(node))
                {
                    continue;
                }

                var startIndex = FindNodeRegionIndex(node, data);
                if (startIndex == -1)
                {
                    continue;
                }

                graphRegistry[startIndex].Register(node);
            }
        }

        private static void RegisterStartNodes(List<Graph<WorldMapNode>> graphRegistry, List<WorldMapNode> starting,
            WorldMapStaticData data)
        {
            // Register Start Nodes
            for (var index = 0; index < starting.Count; index++)
            {
                var start = starting[index];
                var foundIndex = FindNodeLaneIndex(start, data);
                if (graphRegistry.Count - 1 >= foundIndex)
                {
                    continue;
                }

                var graph = new Graph<WorldMapNode>();
                graphRegistry.Add(graph);
                graph.Register(start);
            }
        }

        private static WorldMapNode FindNearest(List<WorldMapNode> nodes, WorldMapNode node,
            List<WorldMapNode> exceptions = null)
        {
            var nearest = float.MaxValue;
            var nearestIndex = -1;
            for (var index = 0; index < nodes.Count; index++)
            {
                var worldMapNode = nodes[index];
                if (exceptions != null && exceptions.Contains(worldMapNode))
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

        private static int FindNodeLaneIndex(WorldMapNode node, WorldMapStaticData data)
        {
            var regions = data.Parameters.Regions;
            var nearest = float.MaxValue;
            var nearestIndex = -1;
            for (var index = 0; index < regions.Length; index++)
            {
                var region = regions[index];
                var distance = Vector3.Distance(region.Bounds.center, node.Bounds.center);
                if (distance < nearest)
                {
                    nearest = distance;
                    nearestIndex = index;
                }
            }

            return nearestIndex;
        }

        private static int FindNodeRegionIndex(WorldMapNode node, WorldMapStaticData data)
        {
            var regions = data.Parameters.Regions;
            for (var index = 0; index < regions.Length; index++)
            {
                var region = regions[index];
                var vertex = new[]
                {
                    new Vector3(region.Bounds.xMin, region.Bounds.yMin, 0),
                    new Vector3(region.Bounds.xMin, region.Bounds.yMax, 0),
                    new Vector3(region.Bounds.xMax, region.Bounds.yMax, 0),
                    new Vector3(region.Bounds.xMax, region.Bounds.yMin, 0),
                };

                if (CheckPointOverlapPolygon(vertex, node.Bounds.center))
                {
                    return index;
                }
            }

            return -1;
        }
    }
}