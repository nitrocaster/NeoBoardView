using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Math
{
    using System.Drawing;
    using SysMath = System.Math;

    public struct OBB
    {
        //private Box2 box;
        //public Vector2 Center;
        //public Vector2 HalfSize;
        public Box2 Box; // bbox in rotated world origin centered basis
        public Angle Turn;

        /*
        required operations:
        - change size
        - convert to world verts
        - shrink/grow
        - merge point
        */

        public double Width
        {
            //get { return 2*HalfSize.X; }
            get { return Box.Width; }
        }
        public double Height
        {
            //get { return 2*HalfSize.Y; }
            get { return Box.Height; }
        }
        public Vector2 Center
        {
            get { return Box.Center; }
        }
        public Vector2 WorldCenter
        {
            get { return BoxToWorld(Box.Center); }
        }
        //public Vector2 Min
        //{
        //    get { return BoxToWorld(box.Min); }
        //}
        //public Vector2 Max
        //{
        //    get { return BoxToWorld(box.Max); }
        //}

        public Box2 EnclosingBBox
        {
            get
            {
                if (Turn<Angle.EqualityTolerence)
                    //return new Box2(Center-HalfSize, Center+HalfSize);
                    return Box;
                Vector2 p0, p1, p2, p3;
                ToPoints(out p0, out p1, out p2, out p3);
                var box = Box2.Empty;
                box.Merge(p0);
                box.Merge(p1);
                box.Merge(p2);
                box.Merge(p3);
                return box;
            }
        }
        
        public OBB(Angle turn)
        {
            //Center = Vector2.Origin;
            //HalfSize = Vector2.MinValue;
            Box = Box2.Empty;
            Turn = turn;
        }

        public OBB(Box2 box, Angle turn)
        {
            //if (!box.IsEmpty)
            //{
            //    Center = box.Center;
            //    HalfSize.X = box.Width;
            //    HalfSize.Y = box.Height;
            //}
            //else
            //{
            //    Center = Vector2.Origin;
            //    HalfSize = Vector2.MinValue;
            //}
            Box = box;
            Turn = turn;
        }

        public void Set(Vector2 min, Vector2 max, Angle turn)
        {
            // var box = new Box2(min, max);
            // Center = box.Center;
            // HalfSize.X = box.Width;
            // HalfSize.Y = box.Height;
            Box.Set(min, max);
            Turn = turn;
        }

        public bool Contains(double x, double y)
        { return Contains(new Vector2(x, y)); }

        private void WorldToBox(ref Vector2 p)
        {
            // // translate p to obb center basis
            // var offset = p-Center;
            // // rotate around center and translate back
            // p = Matrix23.Rotation(-Turn)*offset+Center;
            p = Matrix23.Rotation(-Turn)*p;
        }

        private Vector2 BoxToWorld(Vector2 p)
        {
            // // translate p to obb center basis
            // var offset = p-Center;
            // // rotate around center and translate back
            // return Matrix23.Rotation(Turn)*offset+Center;
            return Matrix23.Rotation(Turn)*p;
        }

        public bool Contains(Vector2 v)
        {
            WorldToBox(ref v);
            //return (v.Y > Center.Y - HalfSize.Y) && (v.Y < Center.Y + HalfSize.Y) &&
            //    (v.X > Center.X - HalfSize.X) && (v.X < Center.X + HalfSize.X);
            return Box.Contains(v);
        }

        // XXX: implement
        //public bool Contains(Box2 b)
        //{ return Contains(b.Min) && Contains(b.Max); }

        public void LocalMerge(Vector2 p)
        {
            // var box = new Box2(-HalfSize, +HalfSize);
            // box.Merge(p);
            // Center += box.Center; // adjust center after merge
            // HalfSize = box.Size/2;
            Box.Merge(p);
        }

        public void Merge(Vector2 p)
        {
            WorldToBox(ref p);
            LocalMerge(p);
        }

        public void Shrink(double s)
        {
            // HalfSize -= s;
            Box.Shrink(s);
        }

        public void Grow(double s)
        {
            // HalfSize += s;
            Box.Grow(s);
        }

        public void ToPoints(out Vector2 dst0, out Vector2 dst1, out Vector2 dst2, out Vector2 dst3)
        {
            var halfSize = new Vector2(Box.Width/2, Box.Height/2);
            var invHalfSize = new Vector2(Box.Width/2, -Box.Height/2);
            dst0 = BoxToWorld(Box.Center+halfSize);
            dst1 = BoxToWorld(Box.Center+invHalfSize);
            dst2 = BoxToWorld(Box.Center-halfSize);
            dst3 = BoxToWorld(Box.Center-invHalfSize);
        }

        public void ToPoints(out PointF dst0, out PointF dst1, out PointF dst2, out PointF dst3)
        {
            var halfSize = new Vector2(Box.Width/2, Box.Height/2);
            var invHalfSize = new Vector2(Box.Width/2, -Box.Height/2);
            dst0 = BoxToWorld(Box.Center+halfSize);
            dst1 = BoxToWorld(Box.Center+invHalfSize);
            dst2 = BoxToWorld(Box.Center-halfSize);
            dst3 = BoxToWorld(Box.Center-invHalfSize);
        }

        public void ToPointArray(PointF[] dst)
        { ToPoints(out dst[0], out dst[1], out dst[2], out dst[3]); }

        public void ToPointArray(Vector2[] dst)
        { ToPoints(out dst[0], out dst[1], out dst[2], out dst[3]); }

        //public Box2 Box { get { return box; } }

        //public bool IsEmpty { get { return HalfSize.X<0 || HalfSize.Y<0; } }
        public bool IsEmpty { get { return Box.IsEmpty; } }

        public static readonly OBB Empty = new OBB(Box2.Empty, Angle.FromRadians(0));
    }
}
