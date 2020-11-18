using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace kSlovnik.Controls
{
    public class ShadowedButton : Button, IShadowedControl
    {
        private Panel shadowPanel;

        public ShadowedButton() : base()
        {
            shadowPanel = new Panel();
        }

        public void DropShadow()
        {
            shadowPanel.BackColor = Color.Black;
            shadowPanel.Left = this.Left + Constants.Shadows.DropShadowHOffset;
            shadowPanel.Top = this.Top + Constants.Shadows.DropShadowVOffset;
            shadowPanel.Width = this.Width;
            shadowPanel.Height = this.Height;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            this.Parent.Controls.Add(shadowPanel);
            this.Parent.Controls.SetChildIndex(shadowPanel, this.Parent.Controls.GetChildIndex(this) + 1);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            DropShadow();
        }
    }
}
