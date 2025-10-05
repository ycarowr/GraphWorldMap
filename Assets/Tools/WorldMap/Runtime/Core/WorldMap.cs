using System.Collections.Generic;
using Tools.Graphs;
using Tools.WorldMapCore.Database;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public partial class WorldMap
    {
        public readonly List<Graph<WorldMapNode>> ConnectionsRegistry;
        public readonly WorldMapStaticData Data;
        private readonly Dictionary<EDeletionReason, List<WorldMapNode>> Deletions;
        public readonly List<WorldMapNode> End;
        public readonly List<Graph<WorldMapNode>> GraphsRegistry;
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
            GraphsRegistry = new List<Graph<WorldMapNode>>();
            ConnectionsRegistry = new List<Graph<WorldMapNode>>();
            Deletions = new Dictionary<EDeletionReason, List<WorldMapNode>>
            {
                { EDeletionReason.Overlap, new List<WorldMapNode>() },
                { EDeletionReason.OutOfWorldBounds, new List<WorldMapNode>() },
                { EDeletionReason.OutOfRegionBounds, new List<WorldMapNode>() },
            };

            // Starting
            var starting = Data.Start;
            foreach (var worldPosition in starting)
            {
                var generated = GenerateNodeAt(worldPosition, true);
                if (generated != null)
                {
                    Start.Add(generated);
                    Nodes.Add(generated);
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
                    Nodes.Add(generated);
                }
            }
        }

        public void GenerateNodes()
        {
            var amountToCreate = Data.Parameters.Amount;
            var count = 0;
            var maxCount = Mathf.Max(Data.Parameters.Iterations, amountToCreate);
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
            
            WorldMapHelper.CreateGraph(GraphsRegistry, ConnectionsRegistry, Data, Nodes, Start, End);
        }

        private WorldMapNode GenerateNodeAt(Vector2 worldPosition, bool skipChecks = false)
        {
            var worldSize = Data.Parameters.NodeWorldSize;
            var nodeID = WorldMapHelper.GenerateID();
            var newNode = new WorldMapNode(nodeID, worldPosition, worldSize);

            if (skipChecks)
            {
                return newNode;
            }

            if (WorldMapHelper.CheckOverlap(newNode, Nodes))
            {
                if (WorldMapHelper.CheckWorldBounds(newNode, Data))
                {
                    if (WorldMapHelper.CheckRegionBounds(newNode, Data))
                    {
                        return newNode;
                    }

                    Deletions[EDeletionReason.OutOfRegionBounds].Add(newNode);
                }
                else
                {
                    Deletions[EDeletionReason.OutOfWorldBounds].Add(newNode);
                }
            }
            else
            {
                Deletions[EDeletionReason.Overlap].Add(newNode);
            }

            return null;
        }

        public void OnDrawGizmos()
        {
            WorldMapGizmos.DrawGizmos(Data, Nodes, Start, End, Deletions);
            WorldMapGraphGizmos.DrawGizmos(GraphsRegistry, ConnectionsRegistry, Data);
        }
    }
}