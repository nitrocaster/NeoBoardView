using System;
using System.Drawing;
using System.Collections.Generic;
using Core.Math;

namespace NeoBoardView
{
    public sealed class SceneOptions
    {
        public float PinRadius = 9.0f; // XXX: tweak
        public float PinBoxRadius = 10.0f;
        public float NailRadius = 9.0f;
        public float NailBoxRadius = 10.0f;
        public bool ShowNames = false;
        public bool DrawOrigin = false;
        public bool HeurShapes = false;
    }

    public sealed class SceneColors
    {
        public Color Contour = Color.Yellow;
        public Color Part = Color.Gray;
        public Color Pin = Color.CornflowerBlue;
        public Color Nail = Color.DarkMagenta;
        public Color SelectedObject = Color.LimeGreen;
    }

    public sealed class SceneBrushes
    {
        public Brush SelectedName = Brushes.LimeGreen;
        public Brush Name = Brushes.Gray;
    }
    
    internal sealed class Scene : IDisposable
    {
        private sealed class SceneObjectComparer : IComparer<SceneObject>
        {
            public int Compare(SceneObject x, SceneObject y)
            {
                if (x.ZOrder>y.ZOrder)
                    return 1;
                if (x.ZOrder<y.ZOrder)
                    return -1;
                if (x.Id>y.Id)
                    return 1;
                if (x.Id<y.Id)
                    return -1;
                return 0;
            }
        }

        public readonly SceneOptions Options;
        public readonly SceneColors Colors;
        public readonly SceneBrushes Brushes;

        // content comes here
        public SceneObjects.Background Background;
        public SceneObjects.Contour TopContour;
        public SceneObjects.Contour BottomContour;
        public List<SceneObjects.Part> Parts;
        public List<SceneObjects.Pin> Pins;
        public List<SceneObjects.Nail> Nails;
        public BoardSide Side;
        public Core.QuadTree<SceneObject> TopObjectSpace;
        public Core.QuadTree<SceneObject> BottomObjectSpace;
        public List<SceneObject> SearchResults;
        public SceneObject SelectedObject;

        private bool disposed = false;

        public Scene()
        {
            Options = new SceneOptions();
            Colors = new SceneColors();
            Brushes = new SceneBrushes();
            Background = new SceneObjects.Background(Color.Black);
            Parts = new List<SceneObjects.Part>();
            Pins = new List<SceneObjects.Pin>();
            Nails = new List<SceneObjects.Nail>();
            Side = BoardSide.Top;
            TopObjectSpace = new Core.QuadTree<SceneObject>(new Vector2(40, 40), 8);
            BottomObjectSpace = new Core.QuadTree<SceneObject>(new Vector2(40, 40), 8);
            SearchResults = new List<SceneObject>();
            SelectedObject = null;
        }

        public bool SetSelected(SceneObject obj)
        {
            if (SelectedObject==obj)
                return false;
            if (SelectedObject!=null)
                SelectedObject.Selected = false;
            SelectedObject = obj;
            if (obj!=null)
                obj.Selected = true;
            return true;
        }
        
        public void UpdateSearchResults(SceneObject obj)
        {
            if (obj==null)
                return;
            SearchResults.Add(obj);
            obj.Visible = false;
        }

        public void ResetSearchResults()
        {
            foreach (var obj in SearchResults)
                obj.Visible = true;
            SearchResults.Clear();
        }

        public void Reset()
        {
            if (TopContour!=null)
            {
                TopContour.Dispose();
                TopContour = null;
            }
            if (BottomContour!=null)
            {
                BottomContour.Dispose();
                BottomContour = null;
            }
            foreach (var part in Parts)
                part.Dispose();
            Parts.Clear();
            foreach (var pin in Pins)
                pin.Dispose();
            Pins.Clear();
            foreach (var nail in Nails)
                nail.Dispose();
            Nails.Clear();
            TopObjectSpace.Clear();
            BottomObjectSpace.Clear();
            SearchResults.Clear();
            SelectedObject = null;
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Background.Dispose();
                    Background = null;
                    if (TopContour!=null)
                        TopContour.Dispose();
                    TopContour = null;
                    if (BottomContour!=null)
                        BottomContour.Dispose();
                    BottomContour = null;
                    foreach (var nail in Nails)
                        nail.Dispose();
                }
                DisposeHelper.OnDispose<Scene>(disposing);
                disposed = true;
            }
        }

        ~Scene()
        { Dispose(false); }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
