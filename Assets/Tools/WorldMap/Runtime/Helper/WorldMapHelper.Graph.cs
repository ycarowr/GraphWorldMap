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
            List<WorldMapNode> nodes, List<WorldMapNode> starting, List<WorldMapNode> ending,
            List<WorldMapRegion> regions)
        {
            RegisterRegionGraphs(graphRegistry, data, regions);
            IndexNodesByRegion(graphRegistry, data, nodes, starting, ending, regions);
            SanitizeRegions(graphRegistry, data);
            RegisterStartNodes(graphRegistry, data, starting, regions);
            RegisterEndNodes(graphRegistry, data, ending, regions);
            SortNodes(graphRegistry, data);
            ConnectAllNodes(graphRegistry, data, starting, ending);

            /*
            RegisterStartNodes(graphRegistry, starting, data);
            IndexNodesByLanes(graphRegistry, data, nodes, starting, ending);
            RegisterEndNodes(graphRegistry, ending);
            SortNodes(graphRegistry, data);
            RegistryConnections(graphRegistry, regionConnectionsRegistry, data, starting, ending);
            ConnectAllNodes(graphRegistry, data, starting, ending);
            ConnectMissingStartNodes(graphRegistry, nodes, starting);
            ConnectMissingEndNodes(graphRegistry, nodes, ending);
            */
        }

        private static void RegisterRegionGraphs(
            List<Graph<WorldMapNode>> graphRegistry,
            WorldMapStaticData data,
            List<WorldMapRegion> regions)
        {
            foreach (var region in regions)
            {
                graphRegistry.Add(new Graph<WorldMapNode>());
            }
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

        private static void SanitizeRegions(List<Graph<WorldMapNode>> graphRegistry, WorldMapStaticData data)
        {
            data.SanitizedRegions.Clear();
            for (var index = 0; index < graphRegistry.Count; index++)
            {
                var graph = graphRegistry[index];
                if (graph.Count < 1)
                {
                    continue;
                }

                var minX = float.MaxValue;
                var minY = float.MaxValue;
                var maxX = float.MinValue;
                var maxY = float.MinValue;

                foreach (var node in graph.Nodes)
                {
                    minX = Mathf.Min(node.Bounds.xMin, minX);
                    minY = Mathf.Min(node.Bounds.yMin, minY);
                    maxX = Mathf.Max(node.Bounds.xMax, maxX);
                    maxY = Mathf.Max(node.Bounds.yMax, maxY);
                }

                var sanitizedRect = new Rect
                {
                    xMin = minX,
                    yMin = minY,
                    xMax = maxX,
                    yMax = maxY,
                };
                sanitizedRect.Sanitize();
                data.SanitizedRegions.Add(sanitizedRect);
            }

            ;
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
            if (data.Parameters.AmountOfRegionConnections > 0)
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
                    if (data.Parameters.Orientation == EOrientationGraph.BottomTop)
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
                         connectionCount < data.Parameters.AmountOfRegionConnections;
                         connectionCount++)
                    {
                        if (sort.Count <= 0)
                        {
                            // The list is empty
                            continue;
                        }

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
                if (data.Parameters.SortingMethod == ESortMethod.Distance)
                {
                    graph.Nodes.Sort(new WorldMapNodeComparePointDistance(end.Bounds.center));
                }
                else
                {
                    if (data.Parameters.Orientation == EOrientationGraph.LeftRight)
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

        private static void IndexNodesByRegion(List<Graph<WorldMapNode>> graphRegistry, WorldMapStaticData data,
            List<WorldMapNode> nodes,
            List<WorldMapNode> starting,
            List<WorldMapNode> ending, List<WorldMapRegion> regions)
        {
            // Register nodes according to their regions
            for (var index = 0; index < nodes.Count; index++)
            {
                var node = nodes[index];
                if (starting.Contains(node) || ending.Contains(node))
                {
                    continue;
                }

                var regionIndex = FindNodeRegionIndex(node, data, regions);
                if (regionIndex == -1)
                {
                    continue;
                }

                graphRegistry[regionIndex].Register(node);
            }
        }

        private static void RegisterStartNodes(List<Graph<WorldMapNode>> graphRegistry,
            WorldMapStaticData data,
            List<WorldMapNode> starting,
            List<WorldMapRegion> regions)
        {
            var isVertical = data.Parameters.Orientation == EOrientationGraph.BottomTop;
            for (var index = 0; index < graphRegistry.Count; index++)
            {
                var graph = graphRegistry[index];
                var currentRegion = regions[index];
                var shouldConnectWithStart = true;

                foreach (var region in regions)
                {
                    if (currentRegion == region)
                    {
                        continue;
                    }

                    if (isVertical)
                    {
                        shouldConnectWithStart &= !region.Bounds.IsOnBottom(currentRegion.Bounds);
                    }
                    else
                    {
                        shouldConnectWithStart &= !region.Bounds.IsOnLeft(currentRegion.Bounds);
                    }
                }

                if (!shouldConnectWithStart)
                {
                    continue;
                }

                var nearestIndex = FindNearestIndex(graph, starting);
                if (nearestIndex == -1)
                {
                    continue;
                }

                var start = starting[nearestIndex];
                graph.Register(start);
            }
        }

        private static void RegisterEndNodes(List<Graph<WorldMapNode>> graphRegistry,
            WorldMapStaticData data,
            List<WorldMapNode> ending,
            List<WorldMapRegion> regions)
        {
            var isVertical = data.Parameters.Orientation == EOrientationGraph.BottomTop;
            for (var index = 0; index < graphRegistry.Count; index++)
            {
                var graph = graphRegistry[index];
                var currentRegion = regions[index];
                var shouldConnectWithEnd = true;

                foreach (var region in regions)
                {
                    if (currentRegion == region)
                    {
                        continue;
                    }

                    if (isVertical)
                    {
                        shouldConnectWithEnd &= !region.Bounds.IsOnTop(currentRegion.Bounds);
                    }
                    else
                    {
                        shouldConnectWithEnd &= !region.Bounds.IsOnRight(currentRegion.Bounds);
                    }
                }

                if (!shouldConnectWithEnd)
                {
                    continue;
                }

                var endIndex = FindNearestIndex(graph, ending);
                if (endIndex == -1)
                {
                    continue;
                }

                var end = ending[endIndex];
                graph.Register(end);
            }
        }

        private static int FindNearestRegionIndex(WorldMapNode node, WorldMapStaticData data,
            List<WorldMapRegion> regions)
        {
            var nearest = float.MaxValue;
            var nearestEdge = float.MaxValue;
            var nearestIndex = -1;
            for (var index = 0; index < regions.Count; index++)
            {
                var region = regions[index];
                var distance = Vector3.Distance(node.Bounds.center, region.Bounds.center);
                var distanceEdge =
                    data.Parameters.Orientation == EOrientationGraph.BottomTop
                        ? Mathf.Abs(region.Bounds.yMin - node.Bounds.center.y)
                        : Mathf.Abs(region.Bounds.xMin - node.Bounds.center.x);
                if (distance < nearest && distanceEdge < nearestEdge)
                {
                    nearest = distance;
                    nearestEdge = distanceEdge;
                    nearestIndex = index;
                }
            }

            return nearestIndex;
        }

        private static int FindNearestRegionIndex(WorldMapStaticData data,
            List<WorldMapNode> nodes,
            WorldMapNode start,
            List<WorldMapNode> exceptions,
            List<WorldMapRegion> regions)
        {
            WorldMapNode nearestNode = null;
            var nearestNodeDistance = float.MaxValue;
            foreach (var node in nodes)
            {
                if (exceptions.Contains(node))
                {
                    continue;
                }

                var distance = Vector3.Distance(start.Bounds.center, node.Bounds.center);
                if (distance < nearestNodeDistance)
                {
                    nearestNode = node;
                    nearestNodeDistance = distance;
                }
            }

            return FindNodeRegionIndex(nearestNode, data, regions);
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

            if (nearestIndex >= 0 && nearestIndex < nodes.Count)
            {
                return nodes[nearestIndex];
            }

            // Can't find a connection
            return null;
        }

        private static List<WorldMapNode> FindBorderNodes(Graph<WorldMapNode> graphNodes,
            IComparer<WorldMapNode> comparer)
        {
            var borderNodes = new List<WorldMapNode>();
            borderNodes.AddRange(graphNodes.Nodes);
            borderNodes.Sort(comparer);
            return borderNodes;
        }

        private static int FindNearestIndex(Graph<WorldMapNode> graphNodes, List<WorldMapNode> nodes)
        {
            var nearest = float.MaxValue;
            var nearestIndex = -1;
            for (var index = 0; index < nodes.Count; index++)
            {
                var node = nodes[index];
                var distance = Vector3.Distance(node.Bounds.center, graphNodes.Nodes[^1].Bounds.center);
                if (distance < nearest)
                {
                    nearest = distance;
                    nearestIndex = index;
                }
            }

            return nearestIndex;
        }

        private static int FindNodeRegionIndex(WorldMapNode node, WorldMapStaticData data, List<WorldMapRegion> regions)
        {
            for (var index = 0; index < regions.Count; index++)
            {
                var region = regions[index];
                if (CheckRectContains(region.Bounds, node.Bounds))
                {
                    return index;
                }
            }

            return -1;
        }
    }
}