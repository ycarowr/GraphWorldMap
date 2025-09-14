using System.Collections.Generic;

namespace Tools.WorldMapCore.Runtime
{
    public class WorldMapNodeCompareLeftRight : IComparer<WorldMapNode>
    {
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

            if (lhs.Bounds.center.x > rhs.Bounds.center.x)
            {
                return 1;
            }

            if (lhs.Bounds.center.x < rhs.Bounds.center.x)
            {
                return -1;
            }

            if (lhs.Bounds.center.y < rhs.Bounds.center.y)
            {
                return 1;
            }

            if (lhs.Bounds.center.y < rhs.Bounds.center.y)
            {
                return -1;
            }

            return lhs.ID.CompareTo(rhs.ID);
        }
    }
}