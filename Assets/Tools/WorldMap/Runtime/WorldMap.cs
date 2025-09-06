using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tools.WorldMapCore.Runtime
{
    public class WorldMap
    {
        private static int GenerateID() => ++NodeID;
        private static int NodeID = 0;
        
        public readonly WorldMapStaticData Data;
        public readonly List<Node> Nodes;
        public readonly List<Node> OverlapNodes;
        public bool IsValid() => Nodes != null && Nodes.Count != 0;

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
        private float isolationDistance;
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
            NodeID = 0;
            Data = data;
            var seed = 0;
            if (data.HasRandomSeed)
            {
                seed = Random.Range(0, data.MaxSeed);
                Debug.Log("Random Seed: " + seed);
            }
            else
            {
                seed = data.Seed;
                Debug.Log("Collected Seed: " + seed);
            }
            Random.InitState(seed);
            Nodes = new List<Node>();
            OverlapNodes = new List<Node>();
            remove = new List<Node>();
        }

        public void GenerateNodes()
        {
            amount = Data.Amount;
            size = Data.NodeWorldSize;
            for (index = 0; index < amount; ++index)
            {
                worldPosition = GenerateRandomPosition();
                node = new Node(GenerateID(), worldPosition, size);

                if (CheckOverlap(node))
                {
                    Nodes.Add(node);
                }
                else
                {
                    OverlapNodes.Add(node);
                }
            }

            Nodes.Sort();
            CheckIsolationDistance();
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

        private void CheckIsolationDistance()
        {
            if (Data.IsolationDistance <= 0)
            {
                return;
            }
            
            remove.Clear();
            for (distanceIndexA = 0; distanceIndexA < Nodes.Count; distanceIndexA++)
            {
                var nodeA = Nodes[distanceIndexA];
                isolationDistance = float.MaxValue;
                for (distanceIndexB = 0; distanceIndexB < Nodes.Count; distanceIndexB++)
                {
                    var nodeB = Nodes[distanceIndexB];
                    if (nodeA == nodeB)
                    {
                        continue;
                    }

                    distance = Vector2.Distance(nodeA.WorldPosition, nodeB.WorldPosition);
                    if (distance < isolationDistance)
                    {
                        isolationDistance = distance;
                    }
                }

                if (isolationDistance > Data.IsolationDistance)
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

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            {
                // Draw center 
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(Data.WorldBounds.center, 0.3f);
            }

            {
                // Draw borders
                Gizmos.color = Color.magenta;
                ReadOnlySpan<Vector3> points = new Vector3[]
                {
                    new Vector3(bounds.xMin, bounds.yMin, 0),
                    new Vector3(bounds.xMin, bounds.yMax, 0),
                    new Vector3(bounds.xMax, bounds.yMax, 0),
                    new Vector3(bounds.xMax, bounds.yMin, 0),
                };
                Gizmos.DrawLineStrip(points, true);
            }

            {
                Gizmos.color = Color.blue;
                for (var i = 0; i < Nodes.Count; i++)
                {
                    node = Nodes[i];
                    ReadOnlySpan<Vector3> points = new Vector3[]
                    {
                        new Vector3(node.WorldRect.xMin, node.WorldRect.yMin, 0),
                        new Vector3(node.WorldRect.xMin, node.WorldRect.yMax, 0),
                        new Vector3(node.WorldRect.xMax, node.WorldRect.yMax, 0),
                        new Vector3(node.WorldRect.xMax, node.WorldRect.yMin, 0),
                    };
                    Gizmos.DrawLineStrip(points, true);
                }
            }
            
            {
                // Draw collisions
                Gizmos.color = Color.red;
                for (var i = 0; i < OverlapNodes.Count; i++)
                {
                    node = OverlapNodes[i];
                    ReadOnlySpan<Vector3> points = new Vector3[]
                    {
                        new Vector3(node.WorldRect.xMin, node.WorldRect.yMin, 0),
                        new Vector3(node.WorldRect.xMin, node.WorldRect.yMax, 0),
                        new Vector3(node.WorldRect.xMax, node.WorldRect.yMax, 0),
                        new Vector3(node.WorldRect.xMax, node.WorldRect.yMin, 0),
                    };
                    Gizmos.DrawLineStrip(points, true);
                }
            }
        }
#endif
        
    }
}
