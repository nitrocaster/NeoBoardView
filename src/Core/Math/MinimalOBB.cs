using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Math
{
    using SysMath = System.Math;

    public static class ConvexHull
    {
        private static double Cross(Vector2 O, Vector2 A, Vector2 B)
        { return (A.X - O.X) * (B.Y - O.Y) - (A.Y - O.Y) * (B.X - O.X); }

        private static int CompareVec2(Vector2 a, Vector2 b)
        {
            if (a.X==b.X)
                return SysMath.Sign(a.Y-b.Y);
            else
                return SysMath.Sign(a.X-b.X);
        }

        public static void Calculate(List<Vector2> verts, List<Vector2> hull)
        {
            if (verts.Count==0)
                return;
            if (verts.Count==1)
            {
                hull.Add(verts[0]);
                return;
            }
            int n = verts.Count, k = 0;
            hull.Capacity = 2*n;
            for (int i = 0; i<2*n; i++)
                hull.Add(Vector2.Origin);
            verts.Sort(CompareVec2);
            // Build lower hull
            for (int i = 0; i<n; i++)
            {
                while (k>=2 && Cross(hull[k-2], hull[k-1], verts[i]) <= 0)
                    k--;
                hull[k++] = verts[i];
            }
            // Build upper hull
            for (int i = n-2, t = k+1; i>=0; i--)
            {
                while (k>=t && Cross(hull[k-2], hull[k-1], verts[i]) <= 0)
                    k--;
                hull[k++] = verts[i];
            }
            if (k>1) // remove non-hull vertices after k; remove k - 1 which is a duplicate
                hull.RemoveRange(k-1, hull.Count-k+1);
        }
    }

    public static class MinimalOBB
    {
        public static OBB Calculate(List<Vector2> points)
        {
            //calculate the convex hull
            var hullPoints = new List<Vector2>();
            ConvexHull.Calculate(points, hullPoints);
            //check if no bounding box available
            if (hullPoints.Count==0)
                return OBB.Empty;
            if (hullPoints.Count==1)
                return new OBB(new Box2(hullPoints[0], 0), 0);
            var obb = OBB.Empty;
            var minObb = OBB.Empty;
            var minArea = double.MaxValue;
            var minAngle = 0d;
            //foreach edge of the convex hull
            for (int i = 0; i < hullPoints.Count; i++)
            {
                var nextIndex = i+1;
                var current = hullPoints[i];
                var next = hullPoints[nextIndex % hullPoints.Count];
                var angle = (next-current).OriginAngle();
                obb = OBB.Empty;
                obb.Turn = angle;
                foreach (var p in hullPoints)
                    obb.Merge(p);
                var area = obb.Width*obb.Height;
                if (area<minArea)
                {
                    minArea = area;
                    minAngle = angle;
                    minObb = obb;
                }
            }
            return minObb;
        }
    }
}
