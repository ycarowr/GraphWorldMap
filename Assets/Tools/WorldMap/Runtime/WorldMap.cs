using System.Collections.Generic;

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

        public readonly WorldMapStaticData Data;
        private readonly Dictionary<EDeletionReason, List<Node>> Deletions;
        public readonly List<Node> Nodes;

        public WorldMap(WorldMapStaticData data)
        {
            Data = data;
            WorldMapHelper.ResetID();
            WorldMapHelper.GenerateSeed(Data);
            Nodes = new List<Node>();
            Deletions = new Dictionary<EDeletionReason, List<Node>>
            {
                { EDeletionReason.OutOfBounds, new List<Node>() },
                { EDeletionReason.Overlap, new List<Node>() },
                { EDeletionReason.Isolation, new List<Node>() },
            };
        }

        public bool IsValid()
        {
            return Nodes != null && Nodes.Count != 0;
        }

        public void GenerateNodes()
        {
            var amountToCreate = Data.Amount;
            var worldSize = Data.NodeWorldSize;
            for (var index = 0; index < amountToCreate; ++index)
            {
                var worldPosition = WorldMapHelper.GenerateRandomPosition(Data);
                var newNode = new Node(WorldMapHelper.GenerateID(), worldPosition, worldSize);

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