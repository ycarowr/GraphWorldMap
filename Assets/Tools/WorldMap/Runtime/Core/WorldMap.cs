using System.Collections.Generic;
using Tools.Graphs;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public class WorldMap
    {
        public enum EDeletionReason
        {
            None = 0,
            Overlap = 1,
            OutOfBounds = 2,
            Isolation = 3,

            All = int.MaxValue,
        }

        private readonly WorldMapStaticData Data;
        private readonly Dictionary<EDeletionReason, List<WorldMapNode>> Deletions;
        public readonly List<WorldMapNode> End;
        public readonly List<Graph<WorldMapNode>> Graphs = new();
        public readonly List<WorldMapNode> Nodes;
        public readonly WorldMapRandom Random;
        public readonly List<WorldMapNode> Start;

        public WorldMap(WorldMapStaticData data)
        {
            Data = data;
            WorldMapHelper.ResetID();
            Random = new WorldMapRandom(Data);
            Nodes = new List<WorldMapNode>();
            Start = new List<WorldMapNode>();
            End = new List<WorldMapNode>();
            Graphs = new List<Graph<WorldMapNode>>();
            Deletions = new Dictionary<EDeletionReason, List<WorldMapNode>>
            {
                { EDeletionReason.OutOfBounds, new List<WorldMapNode>() },
                { EDeletionReason.Overlap, new List<WorldMapNode>() },
                { EDeletionReason.Isolation, new List<WorldMapNode>() },
            };

            // Starting
            var starting = Data.Start;
            foreach (var worldPosition in starting)
            {
                var generated = GenerateNodeAt(worldPosition, true);
                if (generated != null)
                {
                    Start.Add(generated);
                    if (Data.IsStartPartOfMainPath)
                    {
                        Nodes.Add(generated);
                    }
                }
            }

            // Ending
            var ending = Data.End;
            foreach (var worldPosition in ending)
            {
                var generated = GenerateNodeAt(worldPosition, true);
                if (generated != null)
                {
                    End.Add(generated);
                    if (Data.IsEndPartOfMainPath)
                    {
                        Nodes.Add(generated);
                    }
                }
            }

            // Graph
            foreach (var start in Start)
            {
                foreach (var end in End)
                {
                    var mapGraph = new Graph<WorldMapNode>();
                    mapGraph.Register(start);
                    mapGraph.Register(end);

                    foreach (var node in Nodes)
                    {
                        mapGraph.Register(node);
                    }

                    Graphs.Add(mapGraph);
                }
            }

            // foreach (var mapGraph in Graphs)
            // {
            //     foreach (var nodeB in mapGraph.Nodes)
            //     {
            //         foreach (var nodeA in mapGraph.Nodes)
            //         {
            //             if (nodeA == nodeB)
            //             {
            //                 // won't do for itself
            //                 continue;
            //             }
            //
            //             var worldPositionA = nodeA.WorldPosition;
            //             var worldPositionB = nodeB.WorldPosition;
            //             var distance = Vector2.Distance(worldPositionA, worldPositionB);
            //             mapGraph.Connect(nodeA, nodeB, distance);
            //         }
            //     }
            // }
        }

        public void GenerateNodes()
        {
            var amountToCreate = Data.Amount;
            var count = 0;
            var maxCount = Mathf.Max(Data.Iterations, amountToCreate);
            while (Nodes.Count != amountToCreate && count < maxCount)
            {
                count++;
                var worldPosition = Random.GenerateRandomPosition(Data);
                var generated = GenerateNodeAt(worldPosition);
                if (generated != null)
                {
                    Nodes.Add(generated);
                }
            }

            Nodes.Sort();
            var isolationNodes = Deletions[EDeletionReason.Isolation];
            WorldMapHelper.CheckIsolationDistance(Nodes, Data, ref isolationNodes);
        }

        private WorldMapNode GenerateNodeAt(Vector2 worldPosition, bool skipChecks = false)
        {
            var worldSize = Data.NodeWorldSize;
            var nodeID = WorldMapHelper.GenerateID();
            var newNode = new WorldMapNode(nodeID, worldPosition, worldSize);

            if (skipChecks)
            {
                return newNode;
            }

            if (WorldMapHelper.CheckOverlap(newNode, Nodes))
            {
                if (WorldMapHelper.CheckBounds(newNode, Data))
                {
                    return newNode;
                }

                Deletions[EDeletionReason.OutOfBounds].Add(newNode);
            }
            else
            {
                Deletions[EDeletionReason.Overlap].Add(newNode);
            }

            return null;
        }

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            WorldMapGizmos.DrawGizmos(Data, Nodes, Start, End, Deletions);
            WorldMapGraphGizmos.DrawGizmos(Graphs, Data);
        }
#endif
    }
}