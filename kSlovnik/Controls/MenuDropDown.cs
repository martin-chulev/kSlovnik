using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kSlovnik.Controls
{
    public class MenuDropDown : ToolStripDropDown
    {
        public MenuDropDown() : base()
        {
            LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
            BackColor = Color.Black;
            this.Renderer = new MenuRenderer();
        }

        public class MenuDropDownRenderer : ToolStripProfessionalRenderer
        {
            public MenuDropDownRenderer() : base(new MenuDropDownColorTable()) { }
        }

        public class MenuDropDownColorTable : MenuColorTable
        {
            public override Color MenuBorder => Color.Transparent;
        }
    }
}
