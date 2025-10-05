using System.Collections.Generic;
using UnityEngine;

namespace Tools.WorldMapCore.Runtime
{
    public class RectCompareLeftRight : IComparer<Rect>
    {
        public int Compare(Rect lhs, Rect rhs)
        {
            if (lhs.center.x > rhs.center.x)
            {
                return 1;
            }

            if (lhs.center.x < rhs.center.x)
            {
                return -1;
            }

            if (lhs.center.y < rhs.center.y)
            {
                return 1;
            }

            if (lhs.center.y < rhs.center.y)
            {
                return -1;
            }

            return 0;
        }
    }
}