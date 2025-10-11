using System.Collections.Generic;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static partial class WorldMapHelper
    {
        public static bool CheckWorldBounds(Rect rect, WorldMapStaticData data)
        {
            var worldBounds = data.WorldBounds;
            return CheckRectContains(worldBounds, rect);
        }

        private static bool CheckRectContains(Rect rectA, Rect rectB)
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

        public static bool CheckRegionBounds(WorldMapNode newNode, List<WorldMapRegion> regions,
            WorldMapStaticData data)
        {
            var nodeBounds = newNode.Bound;

            var isOutOfBounds = false;
            foreach (var region in regions)
            {
                isOutOfBounds |= CheckRectContains(region.Bounds, nodeBounds);
            }

            return isOutOfBounds;
        }

        public static bool IsOnLeft(this Rect rectA, Rect rectB)
        {
            return rectA.xMax <= rectB.xMin;
        }

        public static bool IsOnRight(this Rect rectA, Rect rectB)
        {
            return rectA.xMin >= rectB.xMax;
        }

        public static bool IsOnTop(this Rect rectA, Rect rectB)
        {
            return rectA.yMin >= rectB.yMax;
        }

        public static bool IsOnBottom(this Rect rectA, Rect rectB)
        {
            return rectA.yMax <= rectB.yMin;
        }
    }
}