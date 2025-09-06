using System.Collections.Generic;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static partial class WorldMapHelper
    {
        public static bool CheckOverlap(Node node, List<Node> nodes)
        {
            var count = nodes.Count;
            for (var index = 0; index < count; index++)
            {
                var nodeOverlap = nodes[index];
                var rectA = new Rect(node.WorldPosition, node.Size);
                var rectB = new Rect(nodeOverlap.WorldPosition, nodeOverlap.Size);
                if (!CheckRectOverlap(rectA, rectB))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CheckRectOverlap(Rect rectA, Rect rectB)
        {
            return !(rectA.x < rectB.x + rectB.size.x) ||
                   !(rectA.y < rectB.y + rectB.size.y) ||
                   !(rectB.x < rectA.x + rectA.size.x) ||
                   !(rectB.y < rectA.y + rectA.size.y);
        }

        public static bool CheckRectContains(Rect rectA, Rect rectB)
        {
            var point0 = new Vector3(rectB.xMin, rectB.yMin, 0);
            var point1 = new Vector3(rectB.xMin, rectB.yMax, 0);
            var point2 = new Vector3(rectB.xMax, rectB.yMax, 0);
            var point3 = new Vector3(rectB.xMax, rectB.yMin, 0);

            return rectA.Contains(point0) &&
                   rectA.Contains(point1) &&
                   rectA.Contains(point2) &&
                   rectA.Contains(point3);
        }
    }
}