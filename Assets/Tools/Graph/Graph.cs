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

        public void Register(TNode node)
        {
            if (!IsRegistered(node))
            {
                Nodes.Add(node);
            }
        }

        public void Unregister(TNode node)
        {
            if (IsRegistered(node))
            {
                Nodes.Remove(node);
            }
        }

        public bool IsRegistered(TNode node)
        {
            return Nodes.Contains(node);
        }

        public bool AreConnected(TNode parametersA, TNode parametersB)
        {
            if (!Connections.ContainsKey(parametersA))
            {
                return false;
            }

            return Connections[parametersA].ContainsKey(parametersB);
        }

        public bool HasConnection(TNode parameters)
        {
            return Connections.ContainsKey(parameters);
        }

        public void Connect(TNode nodeA, TNode parametersB, float cost = 0, bool isTwoWay = true)
        {
            if (!IsRegistered(nodeA) || !IsRegistered(parametersB))
            {
                return;
            }

            if (!AreConnected(nodeA, parametersB))
            {
                if (!Connections.ContainsKey(nodeA))
                {
                    Connections.Add(nodeA, new Dictionary<TNode, float>
                    {
                        { parametersB, cost },
                    });
                }
                else
                {
                    Connections[nodeA].Add(parametersB, cost);
                }
            }

            // reverse connection
            if (isTwoWay)
            {
                if (!AreConnected(parametersB, nodeA))
                {
                    if (!Connections.ContainsKey(parametersB))
                    {
                        Connections.Add(parametersB, new Dictionary<TNode, float>
                        {
                            { nodeA, cost },
                        });
                    }
                    else
                    {
                        Connections[parametersB].Add(nodeA, cost);
                    }
                }
            }
        }

        public int FindAmountOfConnections(TNode node)
        {
            if (!IsRegistered(node))
            {
                return 0;
            }

            if (!Connections.ContainsKey(node))
            {
                return 0;
            }

            return Connections[node].Count;
        }

        public float FindConnectionCost(TNode nodeA, TNode parametersB)
        {
            if (!AreConnected(nodeA, parametersB))
            {
                return float.MaxValue;
            }

            return Connections[nodeA][parametersB];
        }
    }
}