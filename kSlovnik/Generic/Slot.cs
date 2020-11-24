using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace kSlovnik.Generic
{
    public abstract class Slot : PictureBox
    {
        public char Letter { get; set; } = '\0';

        public bool IsFilled { get => Letter != '\0'; }

        public Constants.Colors.TileColors Color { get; set; } = Constants.Colors.TileColors.None;

        public override string ToString()
        {
            return $"Letter: {Letter} | Color: {Color}";
        }
    }
}
