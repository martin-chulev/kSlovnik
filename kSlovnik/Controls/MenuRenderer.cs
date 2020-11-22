using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kSlovnik.Controls
{
    public class MenuRenderer : ToolStripProfessionalRenderer
    {
        public MenuRenderer() : base(new MenuColorTable()) { }
    }

    public class MenuColorTable : ProfessionalColorTable
    {
        // Hover
        public override Color MenuItemSelectedGradientBegin => Constants.Colors.MenuHoverColor;
        public override Color MenuItemSelectedGradientEnd => Constants.Colors.MenuHoverColor;
        public override Color MenuItemBorder => Color.Black;

        // Select
        public override Color MenuItemPressedGradientBegin => Constants.Colors.MenuHoverColor;
        public override Color MenuItemPressedGradientMiddle => Constants.Colors.MenuHoverColor;
        public override Color MenuItemPressedGradientEnd => Constants.Colors.MenuHoverColor;
        /*public override Color ImageMarginGradientBegin => Constants.Colors.MenuHoverColor;
        public override Color ImageMarginGradientMiddle => Constants.Colors.MenuHoverColor;
        public override Color ImageMarginGradientEnd => Constants.Colors.MenuHoverColor;*/
        public override Color ButtonPressedGradientBegin => Constants.Colors.MenuHoverColor;
        public override Color ButtonPressedGradientEnd => Constants.Colors.MenuHoverColor;
        public override Color ButtonPressedBorder => Color.Black;
        public override Color ToolStripDropDownBackground => Color.Transparent;

        // Other overridable fields
        /*
        public override Color ButtonCheckedGradientBegin => Color.Red;//base.ButtonCheckedGradientBegin;
        public override Color ButtonCheckedGradientEnd => Color.Red;//base.ButtonCheckedGradientEnd;
        public override Color ButtonCheckedGradientMiddle => Color.Red;//base.ButtonCheckedGradientMiddle;
        public override Color ButtonCheckedHighlight => Color.Red;//base.ButtonCheckedHighlight;
        public override Color ButtonCheckedHighlightBorder => Color.Red;//base.ButtonCheckedHighlightBorder;
        public override Color ButtonPressedGradientMiddle => Color.Red;//base.ButtonPressedGradientMiddle;
        public override Color ButtonPressedHighlight => Color.Red;// base.ButtonPressedHighlight;
        public override Color ButtonPressedHighlightBorder => Color.Red;//base.ButtonPressedHighlightBorder;
        public override Color ButtonSelectedBorder => Color.Red;//base.ButtonSelectedBorder;
        public override Color ButtonSelectedGradientBegin => Color.Red;// base.ButtonSelectedGradientBegin;
        public override Color ButtonSelectedGradientEnd => Color.Red;//base.ButtonSelectedGradientEnd;
        public override Color ButtonSelectedGradientMiddle => Color.Red;//base.ButtonSelectedGradientMiddle;
        public override Color ButtonSelectedHighlight => Color.Red;//base.ButtonSelectedHighlight;
        public override Color ButtonSelectedHighlightBorder => Color.Red;// base.ButtonSelectedHighlightBorder;
        public override Color CheckBackground => Color.Red;//base.CheckBackground;
        public override Color CheckPressedBackground => Color.Red;// base.CheckPressedBackground;
        public override Color CheckSelectedBackground => Color.Red;// base.CheckSelectedBackground;
        public override Color GripDark => Color.Red;//base.GripDark;
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override Color GripLight => Color.Red;//base.GripLight;
        public override Color ImageMarginRevealedGradientBegin => Color.Red;// base.ImageMarginRevealedGradientBegin;
        public override Color ImageMarginRevealedGradientEnd => Color.Red;//base.ImageMarginRevealedGradientEnd;
        public override Color ImageMarginRevealedGradientMiddle => Color.Red;// base.ImageMarginRevealedGradientMiddle;
        public override Color MenuBorder => Color.Black;
        public override Color MenuItemSelected => Color.Red;//base.MenuItemSelected;
        public override Color MenuStripGradientBegin => Color.Red;//base.MenuStripGradientBegin;
        public override Color MenuStripGradientEnd => Color.Red;// base.MenuStripGradientEnd;
        public override Color OverflowButtonGradientBegin => Color.Red;//base.OverflowButtonGradientBegin;
        public override Color OverflowButtonGradientEnd => Color.Red;//base.OverflowButtonGradientEnd;
        public override Color OverflowButtonGradientMiddle => Color.Red;// base.OverflowButtonGradientMiddle;
        public override Color RaftingContainerGradientBegin => Color.Red;// base.RaftingContainerGradientBegin;
        public override Color RaftingContainerGradientEnd => Color.Red;//base.RaftingContainerGradientEnd;
        public override Color StatusStripGradientBegin => Color.Red;// base.StatusStripGradientBegin;
        public override Color StatusStripGradientEnd => Color.Red;//base.StatusStripGradientEnd;
        public override Color ToolStripContentPanelGradientBegin => Color.Red;//base.ToolStripContentPanelGradientBegin;
        public override Color ToolStripContentPanelGradientEnd => Color.Red;// base.ToolStripContentPanelGradientEnd;
        public override Color ToolStripGradientBegin => Color.Red;//base.ToolStripGradientBegin;
        public override Color ToolStripGradientEnd => Color.Red;//base.ToolStripGradientEnd;
        public override Color ToolStripGradientMiddle => Color.Red;//base.ToolStripGradientMiddle;
        public override Color ToolStripPanelGradientBegin => Color.Red;// base.ToolStripPanelGradientBegin;
        public override Color ToolStripPanelGradientEnd => Color.Red;//base.ToolStripPanelGradientEnd;
        public override string ToString()
        {
            return base.ToString();
        }
        public override Color SeparatorDark => base.SeparatorDark;
        public override Color SeparatorLight => base.SeparatorLight;
        public override Color ToolStripBorder => base.ToolStripBorder;
        */
    }
}
