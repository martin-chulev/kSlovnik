using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kSlovnik.Controls
{
    public class Menu : MenuStrip
    {
        public Menu() : base()
        {
            this.Renderer = new MenuRenderer();
        }
    }
}
