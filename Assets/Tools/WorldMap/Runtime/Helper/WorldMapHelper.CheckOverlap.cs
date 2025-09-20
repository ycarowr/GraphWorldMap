using System.Collections.Generic;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static partial class WorldMapHelper
    {
        public static bool CheckOverlap(WorldMapNode node, List<WorldMapNode> nodes)
        {
            var count = nodes.Count;
            for (var index = 0; index < count; index++)
            {
                var nodeOverlap = nodes[index];
                var rectA = new Rect(node.Center, node.Size);
                var rectB = new Rect(nodeOverlap.Center, nodeOverlap.Size);
                if (!CheckRectOverlap(rectA, rectB))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CheckRectOverlap(Rect rectA, Rect rectB)
        {
            return !(rectA.x < rectB.x + rectB.size.x) ||
                   !(rectA.y < rectB.y + rectB.size.y) ||
                   !(rectB.x < rectA.x + rectA.size.x) ||
                   !(rectB.y < rectA.y + rectA.size.y);
        }
    }
}