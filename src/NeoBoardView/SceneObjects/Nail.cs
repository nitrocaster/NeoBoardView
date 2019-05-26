using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace NeoBoardView.SceneObjects
{
    internal class Nail : Circle
    {
        public int Net;
        public string Name = string.Empty;

        public Nail()
        {
            Color = Root.Scene.Colors.Nail;
            Radius = Root.Scene.Options.NailRadius;
        }

        public override void Draw(Renderer renderer, Graphics graphics)
        {
            base.Draw(renderer, graphics);
            if (Selected)
                DrawTag(renderer, graphics, Name);
        }
    }
}
