using System.Collections.Generic;
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
        public readonly List<WorldMapNode> Nodes;
        public readonly WorldMapRandom Random;

        public WorldMap(WorldMapStaticData data)
        {
            Data = data;
            WorldMapHelper.ResetID();
            Random = new WorldMapRandom(Data);
            Nodes = new List<WorldMapNode>();
            Deletions = new Dictionary<EDeletionReason, List<WorldMapNode>>
            {
                { EDeletionReason.OutOfBounds, new List<WorldMapNode>() },
                { EDeletionReason.Overlap, new List<WorldMapNode>() },
                { EDeletionReason.Isolation, new List<WorldMapNode>() },
            };
        }


        public bool IsValid()
        {
            return Nodes != null && Nodes.Count != 0;
        }

        public bool IsPerfect()
        {
            return Nodes.Count == Data.Amount;
        }

        public void GenerateNodes()
        {
            var amountToCreate = Data.Amount;
            var worldSize = Data.NodeWorldSize;
            var count = 0;
            var maxCount = Mathf.Max(Data.Iterations, amountToCreate);
            while (Nodes.Count != amountToCreate && count < maxCount)
            {
                count++;
                var worldPosition = Random.GenerateRandomPosition(Data);
                var newNode = new WorldMapNode(WorldMapHelper.GenerateID(), worldPosition, worldSize);

                if (WorldMapHelper.CheckOverlap(newNode, Nodes))
                {
                    if (WorldMapHelper.CheckBounds(newNode, Data))
                    {
                        Nodes.Add(newNode);
                    }
                    else
                    {
                        Deletions[EDeletionReason.OutOfBounds].Add(newNode);
                    }
                }
                else
                {
                    Deletions[EDeletionReason.Overlap].Add(newNode);
                }
            }

            Nodes.Sort();
            var isolationNodes = Deletions[EDeletionReason.Isolation];
            WorldMapHelper.CheckIsolationDistance(Nodes, Data, ref isolationNodes);
        }

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            WorldMapGizmos.DrawGizmos(Data, Nodes, Deletions);
        }
#endif
    }
}