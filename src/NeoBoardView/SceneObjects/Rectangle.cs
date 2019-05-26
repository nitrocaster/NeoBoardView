using System;
using System.Drawing;
using System.Globalization;
using Core.Math;

namespace NeoBoardView.SceneObjects
{
    internal class Rectangle : SceneObject
    {
        public Vector2 Min;
        public Vector2 Max;
        private static Pen pen;
        private static long refCount = 0;
        
        public Rectangle()
        {
            if (refCount++==0)
                pen = new Pen(Color.White);
            Color = Root.Scene.Colors.Nail;
            ZOrder = SceneObjectConstants.VertexZOrder;
        }

        public Rectangle(Box2 bbox) : this()
        {
            Min = bbox.Min;
            Max = bbox.Max;
        }

        public override void Draw(Renderer renderer, Graphics graphics)
        {
            pen.Color = Highlighted || Selected ? Root.Scene.Colors.SelectedObject : Color;
            double x = Min.X;
            double y = Min.Y;
            double w = Max.X-x;
            double h = Max.Y-y;
            float prevWidth = pen.Width;
            if (Highlighted || Selected)
                pen.Width *= 2;
            graphics.DrawRectangle(pen, (float)x, (float)y, (float)w, (float)h);
            if (Highlighted || Selected)
                pen.Width = prevWidth;
        }

        public override Box2 GetBBox()
        { return new Box2(Min, Max); }

        protected override void PureDispose()
        {
            if (--refCount==0)
            {
                pen.Dispose();
                pen = null;
            }
            base.PureDispose();
        }
    }

    internal class OrientedRect : SceneObject
    {
        public OBB OBBox;
        public Box2 BBox;
        private static Pen pen;
        private static long refCount = 0;
        private static PointF[] vertexCache = new PointF[4];
        public bool DrawAsCircle = false;

        public OrientedRect()
        {
            if (refCount++==0)
                pen = new Pen(Color.White);
            Color = Root.Scene.Colors.Nail;
            ZOrder = SceneObjectConstants.VertexZOrder;
        }

        public OrientedRect(Box2 bbox, OBB obb) : this()
        {
            BBox = bbox;
            OBBox = obb;
        }

        public override void Draw(Renderer renderer, Graphics graphics)
        {
            pen.Color = Highlighted || Selected ? Root.Scene.Colors.SelectedObject : Color;
            float prevWidth = pen.Width;
            if (Highlighted || Selected)
                pen.Width *= 2;
            if (!Root.Scene.Options.HeurShapes)
            {
                graphics.DrawRectangle(pen, (float)BBox.Min.X, (float)BBox.Min.Y,
                    (float)BBox.Width, (float)BBox.Height);
            }
            else if (DrawAsCircle)
            {
                var wcenter = OBBox.WorldCenter;
                var diam = (float)Math.Max(OBBox.Width, OBBox.Height);
                graphics.DrawEllipse(pen, (float)wcenter.X-diam/2, (float)wcenter.Y-diam/2, diam, diam);
            }
            else
            {
                OBBox.ToPointArray(vertexCache);
                graphics.DrawPolygon(pen, vertexCache);
            }
            if (Highlighted || Selected)
                pen.Width = prevWidth;
        }

        public override Box2 GetBBox()
        {
            if (Root.Scene.Options.HeurShapes)
                return OBBox.EnclosingBBox;
            else
                return BBox;
        }

        protected override void PureDispose()
        {
            if (--refCount==0)
            {
                pen.Dispose();
                pen = null;
            }
            base.PureDispose();
        }
    }
}
