using System.Collections.Generic;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static partial class WorldMapHelper
    {
        /// <summary>
        ///     Checks whether a point overlaps with a polygon.
        ///     See more: https://wrfranklin.org/Research/Short_Notes/pnpoly.html
        /// </summary>
        private static bool CheckPointOverlapPolygon(Vector3[] vert, Vector3 test)
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
                var current = nodes[index];
                if (!CheckRectOverlap(node.Bounds, current.Bounds))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Checks whether two rectangles overlap.
        ///     Lots of things can be optimized.
        /// </summary>
        private static bool CheckRectOverlap(Rect rectA, Rect rectB)
        {
            var point0B = new Vector2(rectB.xMin, rectB.yMin);
            var point1B = new Vector2(rectB.xMin, rectB.yMax);
            var point2B = new Vector2(rectB.xMax, rectB.yMax);
            var point3B = new Vector2(rectB.xMax, rectB.yMin);

            var overlap0B = rectA.Contains(point0B);
            var overlap1B = rectA.Contains(point1B);
            var overlap2B = rectA.Contains(point2B);
            var overlap3B = rectA.Contains(point3B);

            var point0A = new Vector2(rectA.xMin, rectA.yMin);
            var point1A = new Vector2(rectA.xMin, rectA.yMax);
            var point2A = new Vector2(rectA.xMax, rectA.yMax);
            var point3A = new Vector2(rectA.xMax, rectA.yMin);

            var overlap0A = rectB.Contains(point0A);
            var overlap1A = rectB.Contains(point1A);
            var overlap2A = rectB.Contains(point2A);
            var overlap3A = rectB.Contains(point3A);

            var overlapInA = overlap0B || overlap1B || overlap2B || overlap3B;
            var overlapInB = overlap0A || overlap1A || overlap2A || overlap3A;
            return !(overlapInA || overlapInB);
        }

        private static bool CheckOverlapX(Rect lhs, Rect rhs)
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

        private static bool CheckOverlapY(Rect lhs, Rect rhs)
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

        public static Rect[] FindAdjacentRects(float distance, Rect target, Rect[] rects)
        {
            var adjacent = new List<Rect>();
            var overlapsX = new List<Rect>();
            var overlapsY = new List<Rect>();
            foreach (var current in rects)
            {
                if (Vector2.Distance(current.center, target.center) > distance)
                {
                    continue;
                }

                if (CheckOverlapX(target, current))
                {
                    overlapsX.Add(current);
                }

                if (CheckOverlapY(target, current))
                {
                    overlapsY.Add(current);
                }
            }

            foreach (var right in overlapsX)
            {
                var overlapRect = FindRectIntersection(target, right);
                var isAdjacent = true;
                foreach (var middle in overlapsX)
                {
                    if (middle == right)
                    {
                        continue;
                    }

                    if (!CheckRectOverlap(overlapRect, middle))
                    {
                        isAdjacent = false;
                    }
                }

                if (isAdjacent)
                {
                    adjacent.Add(right);
                }
            }

            foreach (var right in overlapsY)
            {
                var overlapRect = FindRectIntersection(target, right);
                var isAdjacent = true;
                foreach (var middle in overlapsY)
                {
                    if (middle == right)
                    {
                        continue;
                    }

                    if (!CheckRectOverlap(overlapRect, middle))
                    {
                        isAdjacent = false;
                    }
                }

                if (isAdjacent)
                {
                    adjacent.Add(right);
                }
            }

            return adjacent.ToArray();
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
            return intersection.Sanitize();
        }

        private static Rect Sanitize(this Rect rect)
        {
            var newRect = new Rect(rect);
            if (newRect.width < 0)
            {
                newRect.x += newRect.width;
                newRect.width = Mathf.Abs(newRect.width);
            }

            if (newRect.height < 0)
            {
                newRect.y += newRect.height;
                newRect.height = Mathf.Abs(newRect.height);
            }

            return newRect;
        }
    }
}