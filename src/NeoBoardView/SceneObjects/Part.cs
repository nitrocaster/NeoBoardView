using System;
using System.Drawing;
using Core.Math;

namespace NeoBoardView.SceneObjects
{
    internal class Part : OrientedRect
    {
        public string Name = string.Empty;
        public int FirstPin;
        public int PinCount;
        public bool HasThroughHolePins = false;

        public Part() : base()
        { Color = Root.Scene.Colors.Part; }

        public Part(Box2 bbox, OBB obb) : base(bbox, obb)
        { Color = Root.Scene.Colors.Part; }

        public void Update(Box2 bbox, OBB box)
        {
            BBox = bbox;
            OBBox = box;
        }

        public override void Draw(Renderer renderer, Graphics graphics)
        {
            base.Draw(renderer, graphics);
            var center = OBBox.WorldCenter;
            //var invScale = 1/renderer.Scale;
            //var d = (float)(2*Radius*invScale);
            //var r = d/2;
            if ((Selected || Highlighted || Root.Scene.Options.ShowNames) && !string.IsNullOrEmpty(Name))
            {
                var gTransform = graphics.Transform;
                graphics.ResetTransform();
                var myTransform = Matrix23.FromColumns(gTransform.Elements);
                var pos = myTransform*center;
                var str = Name;
                graphics.DrawString(str, renderer.Font, Brushes.Yellow, (float)pos.X, (float)pos.Y);
                graphics.Flush();
                graphics.Transform = gTransform;
            }
        }
    }
}
