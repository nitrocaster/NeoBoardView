using System;
using System.Drawing;
using System.Globalization;
using Core.Math;

namespace NeoBoardView.SceneObjects
{
    internal class Circle : SceneObject
    {
        public Vector2 Location;
        public float Radius;

        public bool ShowName;
        private static Pen pen;
        private static long refCount = 0;
        
        public Circle()
        {
            if (refCount++==0)
                pen = new Pen(Color.White);
            Radius = 2.0f;
            Color = Root.Scene.Colors.Nail;
            ZOrder = SceneObjectConstants.VertexZOrder;
            ShowName = false;
        }

        public override void Draw(Renderer renderer, Graphics graphics)
        {
            //var invScale = 1/renderer.Scale;
            //var d = (float)(2*Radius*invScale);
            var d = (float)(2*Radius);
            var r = d/2;
            pen.Color = Selected || Highlighted ? Root.Scene.Colors.SelectedObject : Color;
            float prevWidth = pen.Width;
            if (Highlighted || Selected)
                pen.Width *= 2;
            //graphics.SmoothingMode
            graphics.DrawEllipse(pen, (float)Location.X-r, (float)Location.Y-r, d, d);
            if (Highlighted || Selected)
                pen.Width = prevWidth;
        }

        protected void DrawTag(Renderer renderer, Graphics graphics, string tag)
        {
            var d = 2*Radius;
            var gTransform = graphics.Transform;
            graphics.ResetTransform();
            var pos = Location;
            var offset = new Vector2(d, -d);
            offset.Rotate(-renderer.Turn);
            pos += offset;
            pos = Matrix23.FromColumns(gTransform.Elements)*pos;
            graphics.DrawString(tag, renderer.Font,
                Selected ? Root.Scene.Brushes.SelectedName : Root.Scene.Brushes.Name, (float)pos.X, (float)pos.Y);
            graphics.Flush();
            graphics.Transform = gTransform;
        }

        public override Box2 GetBBox()
        { return new Box2(Location, Radius); }

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
