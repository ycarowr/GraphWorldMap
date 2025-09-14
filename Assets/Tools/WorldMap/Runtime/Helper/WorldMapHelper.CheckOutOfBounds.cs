using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public static partial class WorldMapHelper
    {
        public static bool CheckBounds(WorldMapNode node, WorldMapStaticData data)
        {
            var worldBounds = data.WorldBounds;
            var nodeBounds = node.Bounds;
            return CheckRectContains(worldBounds, nodeBounds);
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
    }
}