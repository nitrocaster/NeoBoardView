using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///     Represents a Windows label control that can display hyperlinks.
    /// </summary>
    /// <remarks>Fixed incorrect hand cursor.</remarks>
    [ToolboxBitmap(typeof(LinkLabel))]
    public class LinkLabelEx : LinkLabel
    {
        private readonly IntPtr hCursorHand;

        [DllImport("user32", SetLastError = true)]
        private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);
        [DllImport("user32", SetLastError = true)]
        private static extern IntPtr SetCursor(IntPtr hCursor);

        private const int IDC_HAND = 0x7F89;
        private const uint WM_SETCURSOR = 0x0020;

        public LinkLabelEx()
        { hCursorHand = LoadCursor(IntPtr.Zero, IDC_HAND); }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg==WM_SETCURSOR)
            {
                if (OverrideCursor==Cursors.Hand)
                    SetCursor(hCursorHand);
                else
                    SetCursor(Cursors.Arrow.Handle);
                m.Result = IntPtr.Zero;
                return;
            }
            base.WndProc(ref m);
        }
    }
}
