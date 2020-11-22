using kSlovnik.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kSlovnik.Controls
{
    public class MenuDropDownItem : ToolStripMenuItem
    {
        private bool isToggled;
        public bool IsToggled
        {
            get => isToggled;
            set
            {
                isToggled = value;
                Image = isToggled ? ImageController.MenuItemToggleOnImage : ImageController.MenuItemToggleOffImage;
            }
        }

        public MenuDropDownItem(string text, bool withSeparator = false, bool enabled = true, bool isToggled = false) : base($"{text}    ")
        {
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            ImageAlign = ContentAlignment.MiddleLeft;
            TextAlign = ContentAlignment.MiddleLeft;
            BackColor = Constants.Colors.MenuBackColor;
            Margin = withSeparator ? new Padding(0, 0, 0, 1) : new Padding(0, 0, 0, 0);
            Enabled = enabled;

            this.IsToggled = isToggled;
        }

        public enum Type
        {
            Top,
            Middle,
            WithSeparator
        }
    }
}
