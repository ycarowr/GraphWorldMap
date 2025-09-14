//https://discussions.unity.com/t/two-intersecting-lines/916519/2

using UnityEngine;
using static System.MathF;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Tools.Line
{
    public static class LineHelper
    {
        // returns true if the intersection exists, false otherwise
        // the actual point is returned through the 'out' parameter
        public static bool Intersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersect,
          Mode mode = Mode.Segments)
        {
          // default intersection point in case none exists
          intersect = new Vector2(float.NaN, float.NaN);

          // check the bounding rectangles (optimization for segments only)
          if (mode == Mode.Segments)
          {
            var ar = rectFromSeg(a1, a2);
            var br = rectFromSeg(b1, b2);
            if (!ar.Overlaps(br)) return false;
          }

          // compute point of intersection using homogeneous coordinates
          var aa = Vector3.Cross(hp(a1), hp(a2));
          var bb = Vector3.Cross(hp(b1), hp(b2));
          var cc = Vector3.Cross(aa, bb);

          // if Z is close to 0, no point of intersection exists
          if (Abs(cc.z) < 1E-6f) return false;

          // otherwise, compute the actual point in 2D
          var x = ((Vector2)cc) * (1f / cc.z);

          // test the intersection interval for rays and segments
          if (mode switch
              {
                Mode.Rays => !test(x, a1, a2) || !test(x, b1, b2),
                Mode.RayLine => !test(x, a1, a2),
                Mode.RaySegment => !test(x, a1, a2) || !test(x, b1, b2, bidi: true),
                Mode.Segments => !test(x, a1, a2, bidi: true) || !test(x, b1, b2, bidi: true),
                _ => false // lines can't fail at this
              }) return false;

          // adopt the 2D solution and return it
          intersect = x;
          return true;

          // -- local functions --
          // conversion to homogeneous coordinates
          static Vector3 hp(Vector2 p) => new Vector3(p.x, p.y, 1f);

          // interval test; segments are bidi(rectional), rays are not
          static bool test(Vector2 p, Vector2 a, Vector2 b, bool bidi = false)
          {
            int i = Abs(b.x - a.x) < Abs(b.y - a.y) ? 1 : 0;
            float n = p[i] - a[i], d = b[i] - a[i]; // numerator and denominator of inverse lerp
            if (bidi && Abs(n) > Abs(d)) return false; // interval test for segments
            return n >= 0f == d >= 0f; // interval test for rays and segments
          }

          // produces a rectangle that encapsulates two arbitrary points
          static Rect rectFromSeg(Vector2 a, Vector2 b)
          {
            var min = leastOf(a, b);
            return new Rect(min, mostOf(a, b) - min);
          }

          static Vector2 leastOf(Vector2 a, Vector2 b) => new Vector2(Min(a.x, b.x), Min(a.y, b.y));
          static Vector2 mostOf(Vector2 a, Vector2 b) => new Vector2(Max(a.x, b.x), Max(a.y, b.y));

        }

        public enum Mode
        {
          Lines,
          Rays,
          RayLine,
          RaySegment,
          Segments
        }
    }
}
