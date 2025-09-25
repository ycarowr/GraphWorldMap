using System.Collections.Generic;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public class WorldMapNodeComparePointDistance : IComparer<WorldMapNode>
    {
        public readonly Vector3 Point;

        public WorldMapNodeComparePointDistance(Vector3 point)
        {
            Point = point;
        }

        public int Compare(WorldMapNode lhs, WorldMapNode rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return 0;
            }

            if (rhs is null)
            {
                return 1;
            }

            if (lhs is null)
            {
                return -1;
            }

            var distanceLhs = Vector3.Distance(lhs.Bounds.center, Point);
            var distanceRhs = Vector3.Distance(rhs.Bounds.center, Point);
            if (distanceLhs > distanceRhs)
            {
                return 1;
            }

            if (distanceLhs < distanceRhs)
            {
                return -1;
            }

            return lhs.ID.CompareTo(rhs.ID);
        }
    }
}