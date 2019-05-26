using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Core.Math;

namespace NeoBoardView
{
    internal sealed class Renderer : IDisposable
    {
        public bool AntialiasingEnabled;
        // camera parameters
        public Vector2 Offset;
        public Vector2 TurnOrigin;
        public Angle Turn;
        public double Scale;
        // ~
        public readonly Font Font;
        public readonly Brush FontBackgroundBrush;
        public event Action Rendered;
        private Func<Scene> getScene;
        private Pen redLinePen;
        private Pen greenLinePen;
        private Control drawingSurface;
        private readonly Matrix transform;
        private readonly Stopwatch timer;
        private const float FontBackgroundAlpha = 0.8f;
        private const int FontSize = 8;

        public Renderer(Func<Scene> sceneGetter)
        {
            timer = new Stopwatch();
            Font = new Font("Tahoma", FontSize, GraphicsUnit.Point);
            FontBackgroundBrush = new SolidBrush(Color.FromArgb((int)(255*FontBackgroundAlpha), Color.White));
            AntialiasingEnabled = false;
            Offset = Vector2.Origin;
            TurnOrigin = Vector2.Origin;
            Turn = 0.0f;
            Scale = 1.0f;
            transform = new Matrix();
            getScene = sceneGetter;
            redLinePen = new Pen(Color.Red);
            greenLinePen = new Pen(Color.Lime);
        }

        public void Dispose()
        {
            redLinePen.Dispose();
            redLinePen = null;
            greenLinePen.Dispose();
            greenLinePen = null;
        }
        
        public Control RenderingOutput
        {
            get { return drawingSurface; }
            set
            {
                if (drawingSurface!=null)
                    drawingSurface.Paint -= OnRender;
                drawingSurface = value;
                drawingSurface.Paint += OnRender;
            }
        }

        public Matrix Transform
        {
            get
            {
                transform.Reset();
                // 1. scale relative to world origin
                transform.Scale((float)Scale, (float)Scale);
                // 2. translate to turn origin and turn
                transform.Translate((float)(TurnOrigin.X), (float)(TurnOrigin.Y));
                transform.Rotate((float)Turn.Degrees);
                transform.Translate((float)(-TurnOrigin.X), (float)(-TurnOrigin.Y));
                // 3. apply translation
                transform.Translate((float)(Offset.X), (float)(Offset.Y));
                return transform;
            }
        }
        
        private void DefaultTransform(Graphics graphics, PointF center)
        {
            var matrix = graphics.Transform;
            DefaultTransform(matrix, center);
            graphics.Transform = matrix;
        }

        private void DefaultTransform(Matrix m, PointF center)
        {
            // invert Y axis and center world origin
            m.Reset();
            m.Scale(+1.0F, -1.0F);
            m.Translate(center.X, center.Y);
        }

        private void OnRendered()
        {
            if (Rendered != null)
                Rendered();
        }

        private void OnRender(object sender, PaintEventArgs args)
        {
            Render(getScene(), args.Graphics);
            OnRendered();
        }

        private PointF GetDefaultOrigin()
        {
            var clSize = drawingSurface.ClientSize;
            return new PointF(+clSize.Width/2.0f, -clSize.Height/2.0f);
        }

        public void FitToScreenTransform(Box2 bbox)
        {
            Offset.Set(0, 0);
            TurnOrigin.Set(0, 0);
            Scale = 1;
            // применяем к собранным вершинам преобразование координат
            // инвертирование оси Y, перенос базиса в центр экрана
            // + изменяемый масштаб, поворот и перенос
            var transformedVertices = bbox.ToArray();
            var center = GetDefaultOrigin();
            var mx = new Matrix();
            DefaultTransform(mx, center);
            mx.Multiply(Transform);
            mx.TransformPoints(transformedVertices);
            // получили экранные координаты вершин
            var tbbox = Box2.Empty;
            // ищем мин/макс координаты группы вершин
            for (var i = 0; i<transformedVertices.Length; i++)
                tbbox.Merge(transformedVertices[i]);
            // получили bounding box в экранных координатах.
            // (ось Y направлена вниз)
            var tbbDiag = tbbox.Max-tbbox.Min;
            var tbbCenter = tbbox.Min+tbbDiag/2;
            var cbbox = RenderingOutput.ClientRectangle;
            var cbboxCenter = new Vector2(cbbox.Width, cbbox.Height)/2;
            var addOffset = cbboxCenter-tbbCenter;
            addOffset.X /= +Scale;
            addOffset.Y /= -Scale;
            addOffset.Rotate(-Turn.Radians);
            var boundingRect = new System.Drawing.Rectangle(0, 0,
                (int)Math.Ceiling(Math.Abs(tbbDiag.X)), (int)Math.Ceiling(Math.Abs(tbbDiag.Y)));
            var newScale = Math.Min((double)cbbox.Width/boundingRect.Width, (double)cbbox.Height/boundingRect.Height);
            Offset += addOffset;
            Scale = newScale*Scale*0.8;
        }

        // cache last successful render params here
        struct TransformData
        {
            public Vector2 Offset;
            public Vector2 TurnOrigin;
            public Angle Turn;
            public double Scale;
        }

        TransformData goodTransform;

        private void SaveTransform()
        {
            goodTransform.Offset = Offset;
            goodTransform.TurnOrigin = TurnOrigin;
            goodTransform.Turn = Turn;
            goodTransform.Scale = Scale;
        }

        private void RestoreTransform()
        {
            Offset = goodTransform.Offset;
            TurnOrigin = goodTransform.TurnOrigin;
            Turn = goodTransform.Turn;
            Scale = goodTransform.Scale;
        }

        private void DrawPin(Scene scene, Graphics g, SceneObjects.Pin p)
        {
            if (p.Side==scene.Side)
                p.Draw(this, g);
            else if (p.Inverse!=null && p.Inverse.Side==scene.Side)
                p.Inverse.Draw(this, g);
        }

        private void Render(Scene scene, Graphics graphics)
        {
            timer.Restart();
            var center = GetDefaultOrigin();
            DefaultTransform(graphics, center);
            graphics.MultiplyTransform(Transform);
            graphics.SmoothingMode = AntialiasingEnabled ? SmoothingMode.AntiAlias : SmoothingMode.Default;
            scene.Background.Draw(this, graphics);
            if (scene.Side==BoardSide.Top)
            {
                if (scene.TopContour!=null && scene.TopContour.Visible)
                    scene.TopContour.Draw(this, graphics);
            }
            else if (scene.Side==BoardSide.Bottom)
            {
                if (scene.BottomContour!=null && scene.BottomContour.Visible)
                    scene.BottomContour.Draw(this, graphics);
            }
            foreach (var part in scene.Parts)
            {
                if (!(part.Side==scene.Side || part.HasThroughHolePins) || !part.Visible)
                    continue;
                // draw pins
                for (int i = part.FirstPin; i<part.FirstPin+part.PinCount; i++)
                    DrawPin(scene, graphics, scene.Pins[i]);
                // draw part itself
                if (part.Side==scene.Side)
                    part.Draw(this, graphics);
            }
            foreach (var nail in scene.Nails)
            {
                if (nail.Side!=scene.Side || !nail.Visible)
                    continue;
                nail.Draw(this, graphics);
            }
            // draw search results
            foreach (var obj in scene.SearchResults)
            {
                if (obj.HasSide && obj.Side!=scene.Side)
                    continue;
                obj.Visible = true;
                obj.Highlighted = true;
                var part = obj as SceneObjects.Part;
                if (part!=null) // draw pins first
                {
                    for (int i = part.FirstPin; i<part.FirstPin+part.PinCount; i++)
                        DrawPin(scene, graphics, scene.Pins[i]);
                }
                obj.Draw(this, graphics);
                obj.Highlighted = false;
                obj.Visible = false;
            }
            // redraw selected object
            // XXX: draw selected object once
            if (scene.SelectedObject!=null)
            {
                var obj = scene.SelectedObject;
                if (!(obj.HasSide && obj.Side!=scene.Side))
                    scene.SelectedObject.Draw(this, graphics);
            }
            if (scene.Options.DrawOrigin)
                DrawCartesianOrigin(graphics);
            graphics.ResetTransform();
            timer.Stop();
            //DrawStats(graphics, timer.Elapsed.TotalMilliseconds);
        }

        private void DrawStats(Graphics graphics, double frameMilliseconds)
        {
            const float statsOffset = 8;
            var info = string.Format("Frame time: {0} ms", frameMilliseconds.ToString("#0.00"));
            graphics.CompositingMode = CompositingMode.SourceOver;
            var infoSize = graphics.MeasureString(info, Font);
            graphics.FillRectangle(FontBackgroundBrush, statsOffset, statsOffset, infoSize.Width, infoSize.Height);
            graphics.DrawString(info, Font, Brushes.Black, statsOffset, statsOffset);
        }

        private void DrawCartesianOrigin(Graphics graphics)
        {
            const float axisLength = 32;
            const float axisArrowLength = 8;
            var invScale = (float)(1/Scale);
            var len = axisLength*invScale;
            var arrow = axisArrowLength*invScale;
            redLinePen.Width = invScale;
            graphics.DrawLine(redLinePen, 0, 0, len, 0);
            graphics.DrawLine(redLinePen, len-arrow, +arrow/4, len, 0);
            graphics.DrawLine(redLinePen, len-arrow, -arrow/4, len, 0);
            greenLinePen.Width = invScale;
            graphics.DrawLine(greenLinePen, 0, 0, 0, len);
            graphics.DrawLine(greenLinePen, +arrow/4, len-arrow, 0, len);
            graphics.DrawLine(greenLinePen, -arrow/4, len-arrow, 0, len);
        }
    }
}
