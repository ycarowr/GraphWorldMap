using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tools.WorldMapCore.Runtime
{
    public class WorldMap
    {
        private readonly WorldMapStaticData Data;
        public readonly List<Node> Nodes;

        public WorldMap(WorldMapStaticData data)
        {
            Data = data;
            if (!data.UseRandomSeed)
            {
                Random.InitState(data.Seed);
            }
            Nodes = new List<Node>();
        }

        public void Create()
        {
            var amount = Data.Amount;
            var size = Data.Size;
            for (var index = 0; index < amount; ++index)
            {
                var worldPosition = GenerateRandomPosition();
                var node = new Node(worldPosition, size);

                if (CheckOverlap(node))
                {
                    Nodes.Add(node);
                }
            }

            Nodes.Sort();
            CheckDistance();
        }

        private bool CheckOverlap(Node nodeA)
        {
            foreach (var nodeB in Nodes)
            {
                var rectA = new Rect(nodeA.WorldPosition, nodeA.Size);
                var rectB = new Rect(nodeB.WorldPosition, nodeB.Size);
                if (rectA.x < rectB.x + rectB.size.x &&
                    rectA.y < rectB.y + rectB.size.y &&
                    rectB.x < rectA.x + rectA.size.x &&
                    rectB.y < rectA.y + rectA.size.y)
                {
                    return false;
                }
            }

            return true;
        }

        private void CheckDistance()
        {
            var remove = new List<Node>();
            foreach (var nodeA in Nodes)
            {
                var minDistance = float.MaxValue;
                foreach (var nodeB in Nodes)
                {
                    if (nodeA == nodeB)
                    {
                        continue;
                    }

                    var distance = Vector2.Distance(nodeA.WorldPosition, nodeB.WorldPosition);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                }

                if (minDistance > Data.MinDistance)
                {
                    remove.Add(nodeA);
                }
            }

            foreach (var node in remove)
            {
                Nodes.Remove(node);
            }
        }

        private Vector2 GenerateRandomPosition()
        {
            var bounds = Data.Bounds;
            var x = Random.Range(bounds.xMin, bounds.xMax);
            var y = Random.Range(bounds.yMin, bounds.yMax);
            return new Vector2(x, y);
        }
    }
}