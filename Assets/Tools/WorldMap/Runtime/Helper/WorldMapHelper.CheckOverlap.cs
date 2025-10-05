using System.Collections.Generic;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static partial class WorldMapHelper
    {
        //https://wrfranklin.org/Research/Short_Notes/pnpoly.html
        public static bool CheckPointOverlapPolygon(Vector3[] vert, Vector3 test)
        {
            var nvert = vert.Length;
            var isHit = false;
            int i, j;
            for (i = 0, j = nvert - 1; i < nvert; j = i++)
            {
                if (vert[i].y > test.y != vert[j].y > test.y &&
                    test.x < (vert[j].x - vert[i].x) * (test.y - vert[i].y)
                    / (vert[j].y - vert[i].y) + vert[i].x)
                {
                    isHit = !isHit;
                }
            }

            return isHit;
        }

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

        public static bool CheckOverlapX(Rect lhs, Rect rhs)
        {
            var left = lhs.center.x < rhs.center.x ? lhs : rhs;
            var right = lhs.center.x < rhs.center.x ? rhs : lhs;

            var lRight = left.center.x + left.width / 2;
            var rLeft = right.center.x - right.width / 2;

            if (lRight > rLeft)
            {
                return true;
            }

            return false;
        }

        public static bool CheckOverlapY(Rect lhs, Rect rhs)
        {
            var bottom = lhs.center.y < rhs.center.y ? lhs : rhs;
            var top = lhs.center.y < rhs.center.y ? rhs : lhs;

            var bTop = bottom.center.y + bottom.height / 2;
            var tBottom = top.center.y - top.height / 2;

            if (bTop > tBottom)
            {
                return true;
            }

            return false;
        }

        public static Rect FindRectIntersection(Rect lhs, Rect rhs)
        {
            var lhsRight = lhs.center.x + lhs.width / 2;
            var rhsRight = rhs.center.x + rhs.width / 2;
            var lhsLeft = lhs.center.x - lhs.width / 2;
            var rhsLeft = rhs.center.x - rhs.width / 2;
            var lhsTop = lhs.center.y + lhs.height / 2;
            var rhsTop = rhs.center.y + rhs.height / 2;
            var lhsBottom = lhs.center.y - lhs.height / 2;
            var rhsBottom = rhs.center.y - rhs.height / 2;
            var intersection = new Rect
            {
                xMin = Mathf.Min(lhsRight, rhsRight),
                xMax = Mathf.Max(lhsLeft, rhsLeft),
                yMin = Mathf.Min(lhsTop, rhsTop),
                yMax = Mathf.Max(lhsBottom, rhsBottom),
            };
            return intersection;
        }
    }
}