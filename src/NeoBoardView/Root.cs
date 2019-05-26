using System;
using System.Reflection;
using System.Windows.Forms;

namespace NeoBoardView
{
    internal static class Root
    {
        public static readonly Scene Scene;
        public static readonly Renderer Renderer;
        public static readonly Version Version = Assembly.GetExecutingAssembly().GetName().Version;
        public static readonly string BuildString = string.Format("v.{0}.{1}.{2}",
            Version.Major, Version.Minor, Version.Build);

        static Root()
        {
            Scene = new Scene();
            Renderer = new Renderer(() => { return Scene; });
            Renderer.AntialiasingEnabled = true;
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);            
            Application.Run(new MainDialog());
            Scene.Dispose();
            Renderer.Dispose();
        }
    }
}
