using System.Collections.Generic;
using Tools.WorldMapCore.Database;
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
        public readonly List<WorldMapNode> Start;
        public readonly List<WorldMapNode> End;
        public readonly WorldMapRandom Random;

        public WorldMap(WorldMapStaticData data)
        {
            Data = data;
            WorldMapHelper.ResetID();
            Random = new WorldMapRandom(Data);
            Nodes = new List<WorldMapNode>();
            Start = new List<WorldMapNode>();
            End = new List<WorldMapNode>();
            Deletions = new Dictionary<EDeletionReason, List<WorldMapNode>>
            {
                { EDeletionReason.OutOfBounds, new List<WorldMapNode>() },
                { EDeletionReason.Overlap, new List<WorldMapNode>() },
                { EDeletionReason.Isolation, new List<WorldMapNode>() },
            };

            // Generate Starting
            for (var index = 0; index < Data.AmountStart; index++)
            {
                var worldPosition = Vector2.zero;
                if (Data.Orientation == WorldMapParameters.Orientation.Horizontal)
                {
                    var segment = Data.WorldBounds.size.y / (Data.AmountStart + 1);
                    worldPosition.x = Data.WorldBounds.min.x - Data.NodeWorldSize.x / 2 -
                                      WorldMapParameters.SMALL_NUMBER;
                    worldPosition.y = segment * (index + 1);
                }

                if (Data.Orientation == WorldMapParameters.Orientation.Vertical)
                {
                    var segment = Data.WorldBounds.size.x / (Data.AmountStart + 1);
                    worldPosition.x = segment * (index + 1);
                    worldPosition.y = Data.WorldBounds.min.y - Data.NodeWorldSize.y / 2 -
                                      WorldMapParameters.SMALL_NUMBER;
                }

                if (worldPosition != Vector2.zero)
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
            }

            // Generate Ending
            for (var index = 0; index < Data.AmountEnd; index++)
            {
                var worldPosition = Vector2.zero;
                if (Data.Orientation == WorldMapParameters.Orientation.Horizontal)
                {
                    var segment = Data.WorldBounds.size.y / (Data.AmountEnd + 1);
                    worldPosition.x = Data.WorldBounds.max.x + Data.NodeWorldSize.x / 2 +
                                      WorldMapParameters.SMALL_NUMBER;
                    worldPosition.y = segment * (index + 1);
                }

                if (Data.Orientation == WorldMapParameters.Orientation.Vertical)
                {
                    var segment = Data.WorldBounds.size.x / (Data.AmountEnd + 1);
                    worldPosition.x = segment * (index + 1);
                    worldPosition.y = Data.WorldBounds.max.y + Data.NodeWorldSize.y / 2 +
                                      WorldMapParameters.SMALL_NUMBER;
                }

                if (worldPosition != Vector2.zero)
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
            }
        }

        public WorldMap(List<WorldMapNode> start)
        {
            Start = start;
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
            var newNode = new WorldMapNode(WorldMapHelper.GenerateID(), worldPosition, worldSize);

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
        }
#endif
    }
}