using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tools.WorldMapCore.Runtime
{
    public class WorldMap
    {
        private readonly WorldMapStaticData Data;
        public readonly List<Node> Nodes;

        // Begin optimization
        private int amount;
        private int index;
        private Vector2 size;
        private Vector3 worldPosition;
        private Node node;
        private Rect bounds;
        private float randX;
        private float randY;
        private Rect rectA;
        private Rect rectB;
        private readonly List<Node> remove;
        private float minDistance;
        private float distance;
        private int distanceIndexA;
        private int distanceIndexB;
        private int indexRemoval;
        private int indexOverlap;
        private Node nodeOverlap;
        private int nodesCount;
        // End optimization
        
        public WorldMap(WorldMapStaticData data)
        {
            Data = data;
            if (!data.UseRandomSeed)
            {
                Random.InitState(data.Seed);
            }
            Nodes = new List<Node>();
            remove = new List<Node>();
        }

        public void GenerateNodes()
        {
            amount = Data.Amount;
            size = Data.NodeWorldSize;
            for (index = 0; index < amount; ++index)
            {
                worldPosition = GenerateRandomPosition();
                node = new Node(worldPosition, size);

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
            for (indexOverlap = 0; indexOverlap < Nodes.Count; indexOverlap++)
            {
                nodeOverlap = Nodes[indexOverlap];
                rectA = new Rect(nodeA.WorldPosition, nodeA.Size);
                rectB = new Rect(nodeOverlap.WorldPosition, nodeOverlap.Size);
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
            remove.Clear();
            for (distanceIndexA = 0; distanceIndexA < Nodes.Count; distanceIndexA++)
            {
                var nodeA = Nodes[distanceIndexA];
                minDistance = float.MaxValue;
                for (distanceIndexB = 0; distanceIndexB < Nodes.Count; distanceIndexB++)
                {
                    var nodeB = Nodes[distanceIndexB];
                    if (nodeA == nodeB)
                    {
                        continue;
                    }

                    distance = Vector2.Distance(nodeA.WorldPosition, nodeB.WorldPosition);
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

            for (indexRemoval = 0; indexRemoval < remove.Count; indexRemoval++)
            {
                node = remove[indexRemoval];
                Nodes.Remove(node);
            }
        }

        private Vector2 GenerateRandomPosition()
        {
            bounds = Data.WorldBounds;
            randX = Random.Range(bounds.xMin, bounds.xMax);
            randY = Random.Range(bounds.yMin, bounds.yMax);
            return new Vector2(randX, randY);
        }
    }
}