using System;
using System.Drawing;

namespace NeoBoardView.SceneObjects
{
    internal class Pin : Circle
    {
        public Part Parent { get; private set; }
        public int SelfIndex;
        public int Net;
        public string Name = string.Empty;
        public Pin Inverse = null; // for through-hole pins

        public Pin(Part parent)
        {
            Color = Root.Scene.Colors.Pin;
            Radius = Root.Scene.Options.PinRadius;
            Parent = parent;
            Side = parent.Side;
        }

        public override BoardSide Side
        { get; set; }

        public override void Draw(Renderer renderer, Graphics graphics)
        {
            base.Draw(renderer, graphics);
            if (Selected)
                DrawTag(renderer, graphics, Name);
        }
    }
}
