using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kSlovnik.Controls
{
    public class MenuItem : ToolStripMenuItem
    {
        public MenuItem(string text, Image image) : base($"{text}    ▾", image)
        {
            ImageAlign = ContentAlignment.MiddleCenter;
            TextAlign = ContentAlignment.MiddleLeft;
            AutoSize = false;
        }
    }
}
