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
        private static bool IsPointOverlapPolygon(Vector3[] vert, Vector3 test)
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

        public static bool IsOverlap<T>(T instance, List<T> list, float offset = 0) where T : IBound
        {
            var count = list.Count;
            for (var index = 0; index < count; index++)
            {
                var current = list[index];
                if (IsRectOverlap(instance.Bound, current.Bound, offset))
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
        public static bool IsRectOverlap(Rect rectA, Rect rectB,  float offset = 0)
        {
            return rectA.x < rectB.x + rectB.size.x+ offset &&
                   rectA.y < rectB.y + rectB.size.y+ offset &&
                   rectB.x < rectA.x + rectA.size.x+ offset &&
                   rectB.y < rectA.y + rectA.size.y+ offset;
        }

        private static bool IsOverlapX(Rect lhs, Rect rhs)
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

        private static bool IsOverlapY(Rect lhs, Rect rhs)
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
                if (target == current)
                {
                    continue;
                }

                if (Vector2.Distance(current.center, target.center) > distance)
                {
                    continue;
                }

                if (IsOverlapX(target, current))
                {
                    overlapsX.Add(current);
                }

                if (IsOverlapY(target, current))
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

                    if (IsRectOverlap(overlapRect, middle))
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

                    if (IsRectOverlap(overlapRect, middle))
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

        public static Rect Sanitize(this Rect rect)
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