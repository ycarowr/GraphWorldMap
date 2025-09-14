using System.Collections.Generic;

namespace Tools.Graphs
{
    public class Graph<TNode> where TNode : BaseGraphNode
    {
        public readonly Dictionary<TNode, Dictionary<TNode, float>> Connections;

        public readonly List<TNode> Nodes;

        public Graph()
        {
            Nodes = new List<TNode>();
            Connections = new Dictionary<TNode, Dictionary<TNode, float>>();
        }

        public int Count => Nodes.Count;

        public void Register(TNode graphNode)
        {
            if (!IsRegistered(graphNode))
            {
                Nodes.Add(graphNode);
            }
        }

        public void Unregister(TNode graphNode)
        {
            if (IsRegistered(graphNode))
            {
                Nodes.Remove(graphNode);
            }
        }

        public bool IsRegistered(TNode graphNode)
        {
            return Nodes.Contains(graphNode);
        }

        public bool AreConnected(TNode graphNodeA, TNode nodeB)
        {
            if (!Connections.ContainsKey(graphNodeA))
            {
                return false;
            }

            return Connections[graphNodeA].ContainsKey(nodeB);
        }

        public void Connect(TNode graphNodeA, TNode nodeB, float cost = 0, bool isTwoWay = true)
        {
            if (!IsRegistered(graphNodeA) || !IsRegistered(nodeB))
            {
                return;
            }

            if (!AreConnected(graphNodeA, nodeB))
            {
                if (!Connections.ContainsKey(graphNodeA))
                {
                    Connections.Add(graphNodeA, new Dictionary<TNode, float>
                    {
                        { nodeB, cost },
                    });
                }
                else
                {
                    Connections[graphNodeA].Add(nodeB, cost);
                }
            }

            // reverse connection
            if (isTwoWay)
            {
                if (!AreConnected(nodeB, graphNodeA))
                {
                    if (!Connections.ContainsKey(nodeB))
                    {
                        Connections.Add(nodeB, new Dictionary<TNode, float>
                        {
                            { graphNodeA, cost },
                        });
                    }
                    else
                    {
                        Connections[nodeB].Add(graphNodeA, cost);
                    }
                }
            }
        }

        public int FindAmountOfConnections(TNode graphNode)
        {
            if (!IsRegistered(graphNode))
            {
                return 0;
            }

            if (!Connections.ContainsKey(graphNode))
            {
                return 0;
            }

            return Connections[graphNode].Count;
        }

        public float FindConnectionCost(TNode graphNodeA, TNode nodeB)
        {
            if (!AreConnected(graphNodeA, nodeB))
            {
                return float.MaxValue;
            }

            return Connections[graphNodeA][nodeB];
        }
    }
}