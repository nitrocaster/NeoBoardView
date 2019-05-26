using System;
using System.Collections.Generic;
using System.Drawing;
using Core.Math;

namespace NeoBoardView.SceneObjects
{
    internal class Contour : SceneObject
    {
        public readonly PointF[] Vertices;
        public float Thickness;

        private Box2 bbox = Box2.Empty;
        private static Pen pen;
        private static long refCount = 0;

        public Contour(IList<Vector2> src)
        {
            if (refCount++==0)
                pen = new Pen(Color.Yellow);
            Color = Root.Scene.Colors.Contour;
            Thickness = 1.0f;
            Vertices = new PointF[src.Count];
            for (int i = 0; i<Vertices.Length; i++)
            {
                Vertices[i] = src[i];
                bbox.Merge(Vertices[i]);
            }
        }

        public override void Draw(Renderer r, Graphics g)
        {
            var invScale = 1/r.Scale;
            pen.Color = Selected ? Root.Scene.Colors.SelectedObject : Color;
            pen.Width = (float)(Thickness*invScale);
            g.DrawPolygon(pen, Vertices);
        }

        public override Box2 GetBBox()
        { return bbox; }

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
