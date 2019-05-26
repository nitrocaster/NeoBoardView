using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NeoBoardView
{
    public interface MessageBoxInterface
    {
        string MessageBoxCaption { get; set; }
        string MessageBoxText { get; set; }
        System.Drawing.Color MessageBoxGradientBegin { get; set; }
        System.Drawing.Color MessageBoxGradientEnd { get; set; }
    }
}
