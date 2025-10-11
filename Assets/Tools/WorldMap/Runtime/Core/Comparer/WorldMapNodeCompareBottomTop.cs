using System.Collections.Generic;

namespace Tools.WorldMapCore.Runtime
{
    public class WorldMapNodeCompareBottomTop : IComparer<WorldMapNode>
    {
        public static readonly WorldMapNodeCompareBottomTop Static = new();

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

            if (lhs.Bound.center.y > rhs.Bound.center.y)
            {
                return 1;
            }

            if (lhs.Bound.center.y < rhs.Bound.center.y)
            {
                return -1;
            }

            if (lhs.Bound.center.x > rhs.Bound.center.x)
            {
                return 1;
            }

            if (lhs.Bound.center.x < rhs.Bound.center.x)
            {
                return -1;
            }

            return lhs.ID.CompareTo(rhs.ID);
        }
    }
}