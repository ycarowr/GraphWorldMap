using System.Collections.Generic;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static partial class WorldMapHelper
    {
        public static void CheckIsolationDistance(List<WorldMapNode> nodes, WorldMapStaticData data,
            ref List<WorldMapNode> isolationNodes)
        {
            if (data.IsolationDistance <= 0)
            {
                return;
            }

            var nodesCount = nodes.Count;
            for (var distanceIndexA = 0; distanceIndexA < nodesCount; distanceIndexA++)
            {
                var nodeA = nodes[distanceIndexA];
                var isolationDistance = float.MaxValue;
                for (var distanceIndexB = 0; distanceIndexB < nodesCount; distanceIndexB++)
                {
                    var nodeB = nodes[distanceIndexB];
                    if (nodeA == nodeB)
                    {
                        continue;
                    }

                    var distance = Vector2.Distance(nodeA.WorldPosition, nodeB.WorldPosition);
                    if (distance < isolationDistance)
                    {
                        isolationDistance = distance;
                    }
                }

                if (isolationDistance > data.IsolationDistance)
                {
                    isolationNodes.Add(nodeA);
                }
            }

            for (var indexRemoval = 0; indexRemoval < isolationNodes.Count; indexRemoval++)
            {
                var node = isolationNodes[indexRemoval];
                nodes.Remove(node);
            }
        }
    }
}