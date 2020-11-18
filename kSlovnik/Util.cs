using kSlovnik.Board;
using kSlovnik.Generic;
using kSlovnik.Player;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace kSlovnik
{
    public static class Util
    {
        public static void DropShadow(object sender, PaintEventArgs e)
        {
            Panel panel = (Panel)sender;
            Color[] shadow = new Color[3];
            shadow[0] = Color.FromArgb(181, 181, 181);
            shadow[1] = Color.FromArgb(195, 195, 195);
            shadow[2] = Color.FromArgb(211, 211, 211);
            Pen pen = new Pen(shadow[0]);
            using (pen)
            {
                Point pt = new Point(panel.Location.X + Constants.Padding.Left, panel.Location.Y + Constants.Padding.Top);
                pt.Y += panel.Height;
                for (var sp = 0; sp < 3; sp++)
                {
                    pen.Color = shadow[sp];
                    e.Graphics.DrawLine(pen, pt.X, pt.Y, pt.X + panel.Width - 1, pt.Y);
                    pt.Y++;
                }
            }
        }

        public static void DropShadowRecursive(object sender, PaintEventArgs e)
        {
            Panel panel = (Panel)sender;
            Color[] shadow = new Color[3];
            shadow[0] = Color.FromArgb(181, 181, 181);
            shadow[1] = Color.FromArgb(195, 195, 195);
            shadow[2] = Color.FromArgb(211, 211, 211);
            Pen pen = new Pen(shadow[0]);
            using (pen)
            {
                foreach (Panel p in panel.Controls.OfType<Panel>())
                {
                    Point pt = p.Location;
                    pt.Y += p.Height;
                    for (var sp = 0; sp < 3; sp++)
                    {
                        pen.Color = shadow[sp];
                        e.Graphics.DrawLine(pen, pt.X, pt.Y, pt.X + p.Width - 1, pt.Y);
                        pt.Y++;
                    }
                }
            }
        }

        public static Point Plus(this Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }
        public static Point Minus(this Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }
        public static Point PlusX(this Point a, int x)
        {
            return new Point(a.X + x, a.Y);
        }
        public static Point PlusY(this Point a, int y)
        {
            return new Point(a.X, a.Y + y);
        }
        public static Point MinusX(this Point a, int x)
        {
            return new Point(a.X - x, a.Y);
        }
        public static Point MinusY(this Point a, int y)
        {
            return new Point(a.X, a.Y - y);
        }

        public static Point GetLocationFromTag(this Slot slot)
        {
            try
            {
                if (slot?.Tag != null)
                {
                    int index = (int)slot.Tag;
                    if (slot is HandSlot)
                    {
                        return HandController.CalculateLocation(index);
                    }
                    else if (slot is BoardSlot boardSlot)
                    {
                        return new Point(boardSlot.Position.PositionX,
                                         boardSlot.Position.PositionY);
                    }
                }
            }
            catch
            {
            }

            return Point.Empty;
        }

        public static Point GetLocationOnForm(this Control control)
        {
            var location = Point.Empty;
            do
            {
                location.Offset(control.Location);
                control = control.Parent;
            }
            while (control != null && !control.Name.Equals("content"));
            return location;
        }

        public static Point GetCenterLocation(this Control control)
        {
            return control.Location.PlusX((int)(control.Width / 2)).PlusY((int)(control.Height / 2));
        }
    }
}
