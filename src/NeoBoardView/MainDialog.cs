using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Core.Math;

namespace NeoBoardView
{
    public partial class MainDialog : Form
    {
        private bool trackingOffset;
        private Vector2 prevPos;
        private const double ScaleDelta = 1.25;
        private const double PanScale = 40.0;
        private SearchDialog searchDlg = new SearchDialog();
        private readonly string defaultText;
        private ToolTip ttDesc;
        private AboutDialog aboutDlg = new AboutDialog();

        public MainDialog()
        {
            KeyPreview = true;
            InitializeComponent();
            Icon = Properties.Resources.AppIcon1;
            //if (!License.Status.Licensed)
            //    Text += " (evaluation)";
            defaultText = Text;
            lStatus.Text = string.Empty;
#if DEBUG
            lDebug.Visible = true;
#else
            lDebug.Visible = false;
#endif
            ttDesc = new ToolTip();
            ttDesc.AutoPopDelay = 2500;
            ttDesc.InitialDelay = 1000;
            ttDesc.ReshowDelay = 500;
            InitializeToolButtons();
            MouseWheel += OnMouseWheel;
            Root.Renderer.RenderingOutput = pbDrawingSurface;
        }
        
        private void InitializeToolButtons()
        {
            InitializeToolButton(btnOpen, ToolId.Open, Properties.Resources.Open);
            InitializeToolButton(btnRotateCCW, ToolId.RotateCCW, Properties.Resources.Rotate);
            InitializeToolButton(btnRotateCW, ToolId.RotateCW, Properties.Resources.Rotate, 1);
            InitializeToolButton(btnSide, ToolId.ChangeSide, Properties.Resources.Side);
            InitializeToolButton(btnHome, ToolId.Home, Properties.Resources.Home);
            InitializeToolButton(btnNames, ToolId.ToggleNames, Properties.Resources.Names, 1);
            InitializeToolButton(btnHeurShapes, ToolId.ToggleHeurShapes, Properties.Resources.Shapes, 1);
            InitializeToolButton(btnFindPart, ToolId.FindPart, Properties.Resources.Part);
            InitializeToolButton(btnFindPin, ToolId.FindPin, Properties.Resources.Pin);
            InitializeToolButton(btnFindNet, ToolId.FindNet, Properties.Resources.Net);
            InitializeToolButton(btnFindNail, ToolId.FindNail, Properties.Resources.Nail);
            InitializeToolButton(btnAbout, ToolId.About, Properties.Resources.About);
        }
        
        private string GetToolDesc(ToolId id)
        {
            switch (id)
            {
            case ToolId.Open: return "Load board view (Ctrl+O)";
            case ToolId.RotateCCW: return "Rotate left (Q)";
            case ToolId.RotateCW: return "Rotate right (E)";
            case ToolId.ChangeSide: return "Toggle side (Space)";
            case ToolId.Home: return "Fit in view (H)";
            case ToolId.ToggleNames: return "Toggle names (T)";
            case ToolId.ToggleHeurShapes: return "Toggle heuristically guessed shapes (G)";
            case ToolId.FindPart: return "Find component (C)";
            case ToolId.FindPin: return "Find pin (P)";
            case ToolId.FindNet: return "Find net (N)";
            case ToolId.FindNail: return "Find nail (L)";
            case ToolId.ZoomIn: return "";
            case ToolId.ZoomOut: return "";
            case ToolId.PanLeft: return "";
            case ToolId.PanRight: return "";
            case ToolId.PanUp: return "";
            case ToolId.PanDown: return "";
            case ToolId.About: return "About (F1)";
            default: return "";
            }
        }

        private void InitializeToolButton(Button btn, ToolId id, Image iconStrip, int initialIndex = 0)
        {
            var list = new ImageList();
            if (iconStrip!=null)
            {
                list.Images.AddStrip(iconStrip);
                btn.ImageList = list;
                if (initialIndex>=0)
                    btn.ImageIndex = initialIndex;
                btn.Text = string.Empty;
                var desc = GetToolDesc(id);
                if (!string.IsNullOrEmpty(desc))
                    ttDesc.SetToolTip(btn, desc);
            }
            btn.Click += (sender, e) => HandleToolAction(id);
        }

        private void SetCurrentFilename(string name)
        {
            if (string.IsNullOrEmpty(name))
                Text = defaultText;
            else
                Text = string.Format("{0} - {1}", defaultText, name);
        }

        private bool HandleToolAction(ToolId id)
        {
            switch (id)
            {
            case ToolId.Open:
                if (LoadFile())
                {
                    searchDlg.InitializeAutocomplete();
                    goto case ToolId.Home;
                }
                break;
            case ToolId.RotateCCW:
                Root.Renderer.Turn += Math.PI/2;
                RedrawScene();
                break;
            case ToolId.RotateCW:
                Root.Renderer.Turn -= Math.PI/2;
                RedrawScene();
                break;
            case ToolId.ChangeSide:
            {
                btnSide.ImageIndex = (btnSide.ImageIndex+1)%2;
                bool topSide = btnSide.ImageIndex==0;
                Root.Scene.Side = topSide ? BoardSide.Top : BoardSide.Bottom;
                if (Root.Scene.TopContour!=null)
                {
                    double yOffset = Root.Scene.TopContour.GetBBox().Height;
                    if (topSide)
                        yOffset *= -1;
                    var vec = new Vector2(0, yOffset);
                    vec.Rotate(Root.Renderer.Turn.Radians);
                    ApplyPan(vec, false);
                }
                break;
            }
            case ToolId.Home:
                var contour = Root.Scene.Side==BoardSide.Top ? Root.Scene.TopContour : Root.Scene.BottomContour;
                if (contour==null)
                    break;
                Root.Renderer.FitToScreenTransform(contour.GetBBox());
                RedrawScene();
                break;
            case ToolId.ToggleNames:
                btnNames.ImageIndex = (btnNames.ImageIndex+1)%2;
                Root.Scene.Options.ShowNames = btnNames.ImageIndex==0;
                RedrawScene();
                break;
            case ToolId.ToggleHeurShapes:
                btnHeurShapes.ImageIndex = (btnHeurShapes.ImageIndex+1)%2;
                Root.Scene.Options.HeurShapes = btnHeurShapes.ImageIndex==0;
                RedrawScene();
                break;
            // XXX: refactor copypasted code
            case ToolId.FindPart:
                searchDlg.Setup(QuerySubject.Part, 3, (string[] q, out bool found) =>
                {
                    Root.Scene.ResetSearchResults();
                    foreach (var query in q)
                    {
                        if (!string.IsNullOrEmpty(query))
                        {
                            var fp = Root.Scene.Parts.Find(p => string.Equals(p.Name, query, StringComparison.OrdinalIgnoreCase));
                            Root.Scene.UpdateSearchResults(fp);
                        }
                    }
                    found = Root.Scene.SearchResults.Count>0;
                    if (found && Root.Scene.SearchResults.Find(obj => obj.Side==Root.Scene.Side)==null)
                        HandleToolAction(ToolId.ChangeSide);
                    else
                        RedrawScene();
                });
                searchDlg.ShowDialog(this);
                break;
            case ToolId.FindPin:
                searchDlg.Setup(QuerySubject.Pin, 1, (string[] q, out bool found) =>
                {
                    Root.Scene.ResetSearchResults();
                    found = false;
                    if (string.IsNullOrEmpty(q[0]))
                        return;
                    char[] splitChars = {'.'};
                    var sq = q[0].Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                    if (sq.Length!=2)
                        return;
                    var pin = int.Parse(sq[1]);
                    if (pin<=0)
                        return;
                    var fp = Root.Scene.Parts.Find(p => string.Equals(p.Name, sq[0], StringComparison.OrdinalIgnoreCase));
                    if (fp==null)
                        return;
                    if (fp.PinCount<pin)
                        return;
                    Root.Scene.SearchResults.Add(fp);
                    Root.Scene.SearchResults.Add(Root.Scene.Pins[fp.FirstPin+pin-1]);
                    found = true;
                    if (fp.HasSide && fp.Side!=Root.Scene.Side)
                        HandleToolAction(ToolId.ChangeSide);
                    else
                        RedrawScene();
                    //Root.Renderer.FitToScreenTransform(fp.GetBBox());
                });
                searchDlg.ShowDialog(this);
                break;
            case ToolId.FindNet:
                searchDlg.Setup(QuerySubject.Net, 1, (string[] q, out bool found) =>
                {
                    Root.Scene.ResetSearchResults();
                    foreach (var query in q)
                    {
                        if (!string.IsNullOrEmpty(query))
                        {
                            var fp = Root.Scene.Pins.FindAll(p => string.Equals(p.Name, query, StringComparison.OrdinalIgnoreCase));
                            var fn = Root.Scene.Nails.FindAll(n => string.Equals(n.Name, query, StringComparison.OrdinalIgnoreCase));
                            foreach (var p in fp)
                                Root.Scene.UpdateSearchResults(p);
                            foreach (var n in fn)
                                Root.Scene.UpdateSearchResults(n);
                        }
                    }
                    found = Root.Scene.SearchResults.Count>0;
                    if (found && Root.Scene.SearchResults.Find(obj => obj.Side==Root.Scene.Side)==null)
                        HandleToolAction(ToolId.ChangeSide);
                    else
                        RedrawScene();
                });
                searchDlg.ShowDialog(this);
                break;
            case ToolId.FindNail:
                searchDlg.Setup(QuerySubject.Nail, 3, (string[] q, out bool found) =>
                {
                    Root.Scene.ResetSearchResults();
                    foreach (var query in q)
                    {
                        if (!string.IsNullOrEmpty(query))
                        {
                            var fn = Root.Scene.Nails.FindAll(n => string.Equals(n.Name, query, StringComparison.OrdinalIgnoreCase));
                            foreach (var n in fn)
                                Root.Scene.UpdateSearchResults(n);
                        }
                    }
                    found = Root.Scene.SearchResults.Count>0;
                    if (found && Root.Scene.SearchResults.Find(obj => obj.Side==Root.Scene.Side)==null)
                        HandleToolAction(ToolId.ChangeSide);
                    else
                        RedrawScene();
                });
                searchDlg.ShowDialog(this);
                break;
            case ToolId.ZoomIn:
                ApplyZoom(1);
                break;
            case ToolId.ZoomOut:
                ApplyZoom(-1);
                break;
            case ToolId.PanLeft:
                ApplyPan(new Vector2(+1, 0)*PanScale);
                break;
            case ToolId.PanRight:
                ApplyPan(new Vector2(-1, 0)*PanScale);
                break;
            case ToolId.PanUp:
                ApplyPan(new Vector2(0, -1)*PanScale);
                break;
            case ToolId.PanDown:
                ApplyPan(new Vector2(0, +1)*PanScale);
                break;
            case ToolId.About:
                if (aboutDlg.IsDisposed) // just in case...
                    aboutDlg = new AboutDialog();
                aboutDlg.ShowDialog(this);
                break;
            }
            return true;
        }

        private void ApplyZoom(int delta)
        {
            var coef = delta>0 ? ScaleDelta : 1/ScaleDelta;
            var newFactor = (Root.Renderer.Scale*coef).Clamp(0.01, 1000);
            Root.Renderer.Scale = newFactor;
            RedrawScene();
        }

        private void ApplyPan(Vector2 offset, bool invScale = true)
        {
            if (invScale)
                offset /= Root.Renderer.Scale;
            var newOffset = Root.Renderer.Offset+Vector2.Rotate(offset, -Root.Renderer.Turn);
            Root.Renderer.Offset = newOffset;
            RedrawScene();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
            case Keys.Left:
                HandleToolAction(ToolId.PanLeft);
                return true;
            case Keys.Right:
                HandleToolAction(ToolId.PanRight);
                return true;
            case Keys.Up:
                HandleToolAction(ToolId.PanUp);
                return true;
            case Keys.Down:
                HandleToolAction(ToolId.PanDown);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void OnMouseWheel(object sender, MouseEventArgs args)
        { ApplyZoom(args.Delta); }

        private void RedrawScene()
        { pbDrawingSurface.Invalidate(); }

        private Vector2 ScreenToWorld(Point screenPos)
        {
            var invScale = 1/Root.Renderer.Scale;
            var invOffset = -Root.Renderer.Offset;
            var clSize = Root.Renderer.RenderingOutput.ClientSize;
            screenPos.X -= clSize.Width/2;
            screenPos.Y -= clSize.Height/2;
            var pos = new Vector2(screenPos.X, -screenPos.Y);
            pos.Rotate(-Root.Renderer.Turn);
            pos *= invScale;
            pos += invOffset;
            return pos;
        }

        private bool LoadFile()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "BRD (*.brd)|*.brd|BDV (*.bdv)|*.bdv|All boardview files|*.brd;*.bdv;";
                ofd.FilterIndex = 3;
                ofd.Multiselect = false;
                ofd.RestoreDirectory = true;
                var result = ofd.ShowDialog(this);
                if (result!=DialogResult.OK)
                    return false;
                var ext = Path.GetExtension(ofd.FileName).ToLowerInvariant();
                switch (ext)
                {
                case ".brd": return LoadBrd(ofd.FileName);
                case ".bdv": return LoadBdv(ofd.FileName);
                }
                return false;
            }
        }

        private bool LoadBrd(string fileName)
        {
            if (TestLinkBrdFile.Detect(fileName))
                return LoadTestLinkBrd(fileName);
            if (ToptestBrdFile.Detect(fileName))
                return LoadToptestBrd(fileName);
            BoardUtil.OnUnknownFormat(Path.GetFileName(fileName), -1);
            return false;
        }

        private bool LoadTestLinkBrd(string fileName)
        {
            return LoadBoardView(fileName, new TestLinkBrdFile());
        }

        private bool LoadToptestBrd(string fileName)
        {
            return LoadBoardView(fileName, new ToptestBrdFile());
        }

        private bool LoadBdv(string fileName)
        {
            if (!HonhanBdvFile.Detect(fileName))
            {
                BoardUtil.OnUnknownFormat(Path.GetFileName(fileName), -1);
                return false;
            }
            return LoadBoardView(fileName, new HonhanBdvFile());
        }

        private PartTurner aligner = new PartTurner();

        private void AdjustPart(SceneObjects.Part src, ref OBB obb,
            ref double aspect, ref double growR)
        {
            var match = false;
            var originAngle = aligner.CalculatePartShape(src, ref aspect, ref growR, out match);
            if (match)
            {
                obb.Turn = originAngle;
                return;
            }
            if (!aligner.Check90degAlignment(originAngle))
                obb.Turn = aligner.AdjustPartAngle(originAngle, true);
        }

        private OBB CalculatePartOBB(SceneObjects.Part src)
        {
            char c = '\0';
            if (src.Name.Length>0)
                c = src.Name[0];
            double growR = 1.0;
            double adjustAspect = 1.0/2; // 1.1/2
            var obb = OBB.Empty;
            AdjustPart(src, ref obb, ref adjustAspect, ref growR);
            for (int j = src.FirstPin; j<src.FirstPin+src.PinCount; j++)
            {
                var p = Root.Scene.Pins[j];
                if (p.Side==src.Side)
                    obb.Merge(p.Location);
                if (p.Inverse!=null && p.Inverse.Side==src.Side)
                    obb.Merge(p.Inverse.Location);
            }
            if (growR>0) // outer box (+radius)
                obb.Grow(Root.Scene.Options.PinBoxRadius*growR);
            if (adjustAspect!=0)
            {
                var center = obb.Center;
                var w = obb.Width;
                var h = obb.Height;
                var lowRefAspect = w/h<Math.Abs(adjustAspect) || h/w<Math.Abs(adjustAspect);
                if (lowRefAspect || adjustAspect<0)
                {
                    var turn = obb.Turn;
                    Vector2 boxOffset;
                    if (adjustAspect>0)
                    {
                        if (w>h)
                            boxOffset = new Vector2(w/2, w*adjustAspect/2);
                        else
                            boxOffset = new Vector2(h*adjustAspect/2, h/2);
                    }
                    else
                    {
                        adjustAspect *= -1;
                        if (lowRefAspect)
                        {
                            if (w>h)
                                boxOffset = new Vector2(w/2, w*adjustAspect/2);
                            else
                                boxOffset = new Vector2(h*adjustAspect/2, h/2);
                        }
                        else
                        {
                            var minRefSz = Math.Min(w, h);
                            if (w>h)
                                boxOffset = new Vector2(minRefSz/adjustAspect/2, minRefSz/2);
                            else
                                boxOffset = new Vector2(minRefSz/2, minRefSz/adjustAspect/2);
                        }
                    }
                    obb.Set(center-boxOffset, center+boxOffset, turn);
                }
            }
            return obb;
        }

        private Box2 CalculatePartBBox(SceneObjects.Part src)
        {
            var bbox = Box2.Empty;
            for (int i = src.FirstPin; i<src.FirstPin+src.PinCount; i++)
                bbox.Merge(Root.Scene.Pins[i].Location);
            var c = '\0';
            if (src.Name.Length>=2)
                c = src.Name[0];
            var growR = 1.0;
            switch (c)
            {
            case 'U':
                growR = 0.0;
                break;
            }
            if (growR>0) // outer box (+radius)
                bbox.Grow(Root.Scene.Options.PinBoxRadius*growR);
            return bbox;
        }

        private bool LoadBoardView(string fileName, IBoardViewFile bdv)
        {
            if (!bdv.Load(fileName))
            {
                GC.Collect();
                GC.WaitForFullGCComplete();
                return false;
            }
            SetCurrentFilename(Path.GetFileName(fileName));
            Root.Scene.Reset();
            GC.Collect();
            GC.WaitForFullGCComplete();
            if (bdv.InchUnits) // convert to mils
            {
                for (int i = 0; i<bdv.TopContour.Count; i++)
                    bdv.TopContour[i] *= 1000;
                for (int i = 0; i<bdv.BottomContour.Count; i++)
                    bdv.BottomContour[i] *= 1000;
                for (int i = 0; i<bdv.Nails.Count; i++)
                    bdv.Nails[i].Location *= 1000;
                for (int i = 0; i<bdv.Pins.Count; i++)
                    bdv.Pins[i].Location *= 1000;
                // ?
                //bdv.InchUnits = false;
            }
            Root.Scene.TopContour = new SceneObjects.Contour(bdv.TopContour);
            Root.Scene.BottomContour = new SceneObjects.Contour(bdv.BottomContour);
            for (int i = 0; i<bdv.Parts.Count; i++)
            {
                var src = bdv.Parts[i];
                // process part pins and update bbox
                int flipScale = src.Side==BoardSide.Top || !bdv.FlipBottomY ? +1 : -1;
                var p = new SceneObjects.Part();
                p.Side = src.Side;
                p.FirstPin = src.FirstPin;
                p.PinCount = src.PinCount;
                p.Name = src.Name;
                var pinSides = p.Side;
                for (int j = src.FirstPin; j<src.FirstPin+src.PinCount; j++)
                {
                    var loc = bdv.Pins[j].Location;
                    loc.Y *= flipScale;
                    bdv.Pins[j].Location = loc;
                    var pin = new SceneObjects.Pin(p);
                    pin.SelfIndex = j;
                    pin.Net = bdv.Pins[j].Net;
                    pin.Name = bdv.Pins[j].Name;
                    pin.Location = bdv.Pins[j].Location;
                    pin.Side = bdv.Pins[j].Side;
                    if (bdv.Pins[j].Inverse!=null)
                    {
                        var invSrc = bdv.Pins[j].Inverse;
                        var ipin = new SceneObjects.Pin(p);
                        ipin.SelfIndex = j;
                        ipin.Net = invSrc.Net;
                        ipin.Name = invSrc.Name;
                        ipin.Location = invSrc.Location;
                        ipin.Side = invSrc.Side;
                        ipin.Inverse = pin;
                        pin.Inverse = ipin;
                        pinSides |= ipin.Side;
                        if (ipin.Side==BoardSide.Top)
                            Root.Scene.TopObjectSpace.Insert(ipin);
                        if (ipin.Side==BoardSide.Bottom)
                            Root.Scene.BottomObjectSpace.Insert(ipin);
                    }
                    pinSides |= pin.Side;
                    Root.Scene.Pins.Add(pin);
                    if (pin.Side==BoardSide.Top)
                        Root.Scene.TopObjectSpace.Insert(pin);
                    if (pin.Side==BoardSide.Bottom)
                        Root.Scene.BottomObjectSpace.Insert(pin);
                }
                p.HasThroughHolePins = pinSides!=p.Side;
                p.Update(CalculatePartBBox(p), CalculatePartOBB(p));
                Root.Scene.Parts.Add(p);
            }
            // process through-hole pins
            int pinCount = Root.Scene.Pins.Count;
            for (int i = 0; i<pinCount; i++)
            {
                if (Root.Scene.Pins[i].Inverse!=null)
                    Root.Scene.Pins.Add(Root.Scene.Pins[i].Inverse);
            }
            for (int i = 0; i<bdv.Nails.Count; i++)
            {
                var src = bdv.Nails[i];
                var nail = new SceneObjects.Nail();
                nail.Side = src.Side;
                nail.Net = src.Net;
                nail.Name = src.Name;
                int flipScale = src.Side==BoardSide.Top || !bdv.FlipBottomY ? +1 : -1;
                var loc = src.Location;
                loc.Y *= flipScale;
                src.Location = loc;
                nail.Location = loc;
                Root.Scene.Nails.Add(nail);
                if (src.Side==BoardSide.Top)
                    Root.Scene.TopObjectSpace.Insert(nail);
                if (src.Side==BoardSide.Bottom)
                    Root.Scene.BottomObjectSpace.Insert(nail);
            }
            // ?
            //Root.Scene.Options.FlipBottomY = bdv.FlipBottomY;
            return true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Modifiers.HasFlag(Keys.Control))
            {
                if (e.KeyCode==Keys.O)
                {
                    e.Handled = true;
                    HandleToolAction(ToolId.Open);
                }
                base.OnKeyDown(e);
                return;
            }
            switch (e.KeyCode)
            {
            case Keys.H:
            case Keys.Home:
                e.Handled = true;
                HandleToolAction(ToolId.Home);
                break;
            case Keys.Add:
                e.Handled = true;
                HandleToolAction(ToolId.ZoomIn);
                break;
            case Keys.Subtract:
                e.Handled = true;
                HandleToolAction(ToolId.ZoomOut);
                break;
            case Keys.T:
                e.Handled = true;
                HandleToolAction(ToolId.ToggleNames);
                break;
            case Keys.G:
                e.Handled = true;
                HandleToolAction(ToolId.ToggleHeurShapes);
                break;
            case Keys.Space:
                e.Handled = true;
                HandleToolAction(ToolId.ChangeSide);
                break;
            case Keys.Q:
                e.Handled = true;
                HandleToolAction(ToolId.RotateCCW);
                break;
            case Keys.E:
                e.Handled = true;
                HandleToolAction(ToolId.RotateCW);
                break;
            case Keys.L:
                e.Handled = true;
                HandleToolAction(ToolId.FindNail);
                break;
            case Keys.N:
                e.Handled = true;
                HandleToolAction(ToolId.FindNet);
                break;
            case Keys.C:
                e.Handled = true;
                HandleToolAction(ToolId.FindPart);
                break;
            case Keys.P:
                e.Handled = true;
                HandleToolAction(ToolId.FindPin);
                break;
            case Keys.F1:
                e.Handled = true;
                HandleToolAction(ToolId.About);
                break;
            }
            base.OnKeyDown(e);
        }

        private void pbDrawingSurface_MouseDown(object sender, MouseEventArgs args)
        {
            switch (args.Button)
            {
            case MouseButtons.Right:
            case MouseButtons.Middle:
                trackingOffset = true;
                prevPos = new Vector2(args.Location.X, -args.Location.Y);
                break;
            case MouseButtons.Left:
            {
#if DEBUG
                lDebug.Text = string.Empty;
#endif
                var wPos = ScreenToWorld(args.Location);
                var space = Root.Scene.Side==BoardSide.Top ? Root.Scene.TopObjectSpace : Root.Scene.BottomObjectSpace;
                var qbox = new Box2(wPos, Root.Scene.Options.PinRadius*1.5);
                var qresult = space.Query(qbox);
                if (qresult.Count==0)
                {
                    lStatus.Text = string.Empty;
                    if (Root.Scene.SetSelected(null))
                        RedrawScene();
                    break;
                }
                var objects = new List<SceneObject>(qresult.Count);
                foreach (var obj in qresult)
                {
                    if (!obj.HasSide)
                    {
                        objects.Add(obj);
                        continue;
                    }
                    if (obj.Side==Root.Scene.Side)
                    {
                        objects.Add(obj);
                        continue;
                    }
                    var pinObj = obj as SceneObjects.Pin;
                    if (pinObj!=null && pinObj.Inverse!=null && pinObj.Inverse.Side==Root.Scene.Side)
                        objects.Add(obj);
                }
                objects.Sort((a, b) => { return (int)Math.Ceiling((a.GetBBox().Center-wPos).SqrLength-(b.GetBBox().Center-wPos).SqrLength); });
                if (objects[0] is SceneObjects.Pin)
                {
                    var pin = (SceneObjects.Pin)objects[0];
                    string status;
                    if (string.IsNullOrEmpty(pin.Parent.Name))
                        status = string.Format("Nail: {0}, Probe: {1}", pin.Name, pin.Net);
                    else
                    {
                        status = string.Format("Part: {0}, Pin: {1}, Net: {2}, Probe: {3}",
                            pin.Parent.Name, pin.SelfIndex-pin.Parent.FirstPin+1, pin.Name, pin.Net);
#if DEBUG
                        if (pin.Parent.PinCount==2)
                        {
                            var p1 = Root.Scene.Pins[pin.Parent.FirstPin+0].Location;
                            var p2 = Root.Scene.Pins[pin.Parent.FirstPin+1].Location;
                            lDebug.Text = "diam = "+p1.Distance(p2);
                        }
#endif
                        }
                    lStatus.Text = status;
                    if (Root.Scene.SetSelected(pin))
                        RedrawScene();
                }
                else if (objects[0] is SceneObjects.Nail)
                {
                    var nail = (SceneObjects.Nail)objects[0];
                    lStatus.Text = string.Format("Nail: {0}, Probe: {1}", nail.Name, nail.Net);
                    if (Root.Scene.SetSelected(nail))
                        RedrawScene();
                }
                break;
            }
            }
        }

        private void pbDrawingSurface_MouseMove(object sender, MouseEventArgs args)
        {
            switch (args.Button)
            {
            case MouseButtons.Right:
            case MouseButtons.Middle:
                if (trackingOffset)
                {
                    var newPos = new Vector2(args.Location.X, -args.Location.Y);
                    var offset = (newPos-prevPos)/Root.Renderer.Scale;
                    prevPos = newPos;
                    var newOffset = Root.Renderer.Offset+Vector2.Rotate(offset, -Root.Renderer.Turn);
                    Root.Renderer.Offset = newOffset;
                    RedrawScene();
                }
                break;
            case MouseButtons.Left:
                break;
            }
        }

        private void pbDrawingSurface_MouseUp(object sender, MouseEventArgs args)
        {
            switch (args.Button)
            {
            case MouseButtons.Right:
            case MouseButtons.Middle:
                trackingOffset = false;
                break;
            case MouseButtons.Left:
                break;
            }
        }
    }
}
