using System;
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
            RegisterRegionGraphs(graphRegistry, regions);
            IndexNodesByRegion(graphRegistry, data, nodes, starting, ending, regions);
            RegisterStartNodes(graphRegistry, data, starting, regions);
            RegisterEndNodes(graphRegistry, data, ending, regions);
            SortNodes(graphRegistry, data);
            RegistryConnections(graphRegistry, regionConnectionsRegistry, data, starting, ending, regions);
            ConnectAllNodes(graphRegistry, starting, ending);
            ConnectMissingStartNodes(graphRegistry, nodes, starting);
            ConnectMissingEndNodes(graphRegistry, nodes, ending);
        }

        private static void RegisterRegionGraphs(
            List<Graph<WorldMapNode>> graphRegistry,
            List<WorldMapRegion> regions)
        {
            for (var index = 0; index < regions.Count; index++)
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
                    graph.Connect(nearest, end, Vector3.Distance(nearest.Bound.center, end.Bound.center));
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
                    graph.Connect(start, nearest, Vector3.Distance(start.Bound.center, nearest.Bound.center));
                }
            }
        }

        private static void ConnectAllNodes(
            List<Graph<WorldMapNode>> graphRegistry,
            List<WorldMapNode> starting,
            List<WorldMapNode> ending)
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

                    var distance = Vector3.Distance(node.Bound.center, nodeNext.Bound.center);
                    graph.Connect(node, nodeNext, distance);
                }
            }
        }

        private static void RegistryConnections(List<Graph<WorldMapNode>> graphRegistry,
            List<Graph<WorldMapNode>> regionConnectionsRegistry,
            WorldMapStaticData data,
            List<WorldMapNode> starting,
            List<WorldMapNode> ending,
            List<WorldMapRegion> regions)
        {
            // Create regions connections
            if (data.Parameters.AmountOfRegionConnections > 0)
            {
                var connections = new List<Graph<WorldMapNode>>();
                var regionBound = new Rect[regions.Count];
                for (var index = 0; index < regions.Count; index++)
                {
                    var region = regions[index];
                    regionBound[index] = region.Bound;
                }

                for (var index = 0; index < regions.Count; index++)
                {
                    var region = regions[index];
                    var adjacent = FindAdjacentRects(int.MaxValue, region.Bound, regionBound);
                    foreach (var adjacentRegionRect in adjacent)
                    {
                        if (IsOnRight(adjacentRegionRect, region.Bound))
                        {
                            continue;
                        }

                        if (IsOnTop(adjacentRegionRect, region.Bound))
                        {
                            continue;
                        }

                        var connection = new Graph<WorldMapNode>();
                        var indexAdjacent = Array.IndexOf(regionBound, adjacentRegionRect);
                        var current = new List<WorldMapNode>(graphRegistry[index].Nodes);
                        var currentAdjacent = new List<WorldMapNode>(graphRegistry[indexAdjacent].Nodes);


                        foreach (var node in starting)
                        {
                            current.Remove(node);
                            currentAdjacent.Remove(node);
                        }

                        foreach (var node in ending)
                        {
                            current.Remove(node);
                            currentAdjacent.Remove(node);
                        }

                        var amountOfConnections = data.Parameters.AmountOfRegionConnections;
                        for (var connectionCount = 0;
                             connectionCount < amountOfConnections;
                             connectionCount++)
                        {
                            if (current.Count <= 0)
                            {
                                // The list is empty
                                continue;
                            }

                            if (currentAdjacent.Count <= 0)
                            {
                                continue;
                            }

                            var pair = new WorldMapNode[2];
                            FindBorderNodes(current, currentAdjacent, ref pair);
                            var left = pair[0];
                            var right = pair[1];

                            current.Remove(left);
                            currentAdjacent.Remove(right);

                            connection.Register(left);
                            connection.Register(right);
                            var distance = Vector3.Distance(left.Bound.center, right.Bound.center);
                            connection.Connect(left, right, distance);
                        }

                        connections.Add(connection);
                    }
                }

                // Lane connections are separated graphs
                foreach (var connection in connections)
                {
                    regionConnectionsRegistry.Add(connection);
                }
            }
        }

        private static void FindBorderNodes(List<WorldMapNode> listA, List<WorldMapNode> listB, ref WorldMapNode[] pair)
        {
            var minDistance = float.MaxValue;
            foreach (var nodeA in listA)
            {
                foreach (var nodeB in listB)
                {
                    var distance = Vector3.Distance(nodeA.Bound.center, nodeB.Bound.center);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        pair[0] = nodeA;
                        pair[1] = nodeB;
                    }
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
                    graph.Nodes.Sort(new WorldMapNodeComparePointDistance(end.Bound.center));
                }
                else
                {
                    if (data.Parameters.Orientation == EOrientationGraph.LeftRight)
                    {
                        graph.Nodes.Sort(WorldMapNodeCompareLeftRight.Static);
                    }
                    else
                    {
                        graph.Nodes.Sort(WorldMapNodeCompareBottomTop.Static);
                    }
                }
            }
        }

        private static void IndexNodesByRegion(List<Graph<WorldMapNode>> graphRegistry, WorldMapStaticData data,
            List<WorldMapNode> nodes,
            List<WorldMapNode> starting,
            List<WorldMapNode> ending,
            List<WorldMapRegion> regions)
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
                node.SetRegion(regionIndex);
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
                        shouldConnectWithStart &= !region.Bound.IsOnBottom(currentRegion.Bound);
                    }
                    else
                    {
                        shouldConnectWithStart &= !region.Bound.IsOnLeft(currentRegion.Bound);
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
                        shouldConnectWithEnd &= !region.Bound.IsOnTop(currentRegion.Bound);
                    }
                    else
                    {
                        shouldConnectWithEnd &= !region.Bound.IsOnRight(currentRegion.Bound);
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

        private static WorldMapNode FindNearest(List<WorldMapNode> nodes, WorldMapNode target,
            List<WorldMapNode> exceptions = null)
        {
            var nearest = float.MaxValue;
            var nearestIndex = -1;
            for (var index = 0; index < nodes.Count; index++)
            {
                var node = nodes[index];
                if (exceptions != null && exceptions.Contains(node))
                {
                    continue;
                }

                var distance = Vector3.Distance(target.Bound.center, node.Bound.center);
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

            // Can't nearest
            return null;
        }

        private static int FindNearestIndex(Graph<WorldMapNode> graphNodes, List<WorldMapNode> nodes)
        {
            var nearest = float.MaxValue;
            var nearestIndex = -1;
            for (var index = 0; index < nodes.Count; index++)
            {
                var node = nodes[index];
                var distance = Vector3.Distance(node.Bound.center, graphNodes.Nodes[^1].Bound.center);
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
                if (CheckRectContains(region.Bound, node.Bound))
                {
                    return index;
                }
            }

            return -1;
        }
    }
}