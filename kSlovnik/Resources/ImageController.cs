using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace kSlovnik.Resources
{
    public static class ImageController
    {
        public const string LettersFolderPath = @"Resources\Images\Letters";
        public const string TilesFolderPath = @"Resources\Images\Tiles";

        public static Dictionary<char, Image> LetterImagesActive = new Dictionary<char, Image>();
        public static Dictionary<char, Image> LetterImagesInactive = new Dictionary<char, Image>();
        public static Dictionary<char, Image> LetterImagesActiveBlank = new Dictionary<char, Image>();
        public static Dictionary<char, Image> LetterImagesInactiveBlank = new Dictionary<char, Image>();
        public static Dictionary<string, Image> TileImages = new Dictionary<string, Image>();

        public static Image MenuItemToggleOnImage = CreateImage(Color.Transparent, 30, 30, Color.Transparent, text: "✓", font: new Font(Constants.Fonts.Default, FontStyle.Bold), fontColor: Color.Black);
        public static Image MenuItemToggleOffImage = CreateImage(Color.Transparent, 30, 30, Color.Transparent);

        public static void LoadImages()
        {
            // Active pieces (colored)
            var letterImagePathsActive = Directory.GetFiles($@"{LettersFolderPath}\Active");

            LetterImagesActive.Clear();
            foreach (var letterImagePath in letterImagePathsActive)
            {
                LetterImagesActive.Add(Path.GetFileNameWithoutExtension(letterImagePath)[0], Image.FromFile(letterImagePath).ToSize(Board.Board.SlotSize - Board.Board.SlotBorderSize));
            }

            // Inactive pieces (greyed out)
            var letterImagePathsInactive = Directory.GetFiles($@"{LettersFolderPath}\Inactive");

            LetterImagesInactive.Clear();
            foreach (var letterImagePath in letterImagePathsInactive)
            {
                LetterImagesInactive.Add(Path.GetFileNameWithoutExtension(letterImagePath)[0], Image.FromFile(letterImagePath).ToSize(Board.Board.SlotSize - Board.Board.SlotBorderSize));
            }

            // Active pieces (colored) for blank piece
            var letterImagePathsActiveBlank = Directory.GetFiles($@"{LettersFolderPath}\ActiveBlank");

            LetterImagesActiveBlank.Clear();
            foreach (var letterImagePath in letterImagePathsActiveBlank)
            {
                LetterImagesActiveBlank.Add(Path.GetFileNameWithoutExtension(letterImagePath)[0], Image.FromFile(letterImagePath).ToSize(Board.Board.SlotSize - Board.Board.SlotBorderSize));
            }

            // Inactive pieces (greyed out) for blank piece
            var letterImagePathsInactiveBlank = Directory.GetFiles($@"{LettersFolderPath}\InactiveBlank");

            LetterImagesInactiveBlank.Clear();
            foreach (var letterImagePath in letterImagePathsInactiveBlank)
            {
                LetterImagesInactiveBlank.Add(Path.GetFileNameWithoutExtension(letterImagePath)[0], Image.FromFile(letterImagePath).ToSize(Board.Board.SlotSize - Board.Board.SlotBorderSize));
            }

            // Board tiles
            var tileImagePaths = Directory.GetFiles(TilesFolderPath);

            TileImages.Clear();
            foreach (var tileImagePath in tileImagePaths)
            {
                TileImages.Add(Path.GetFileNameWithoutExtension(tileImagePath), Image.FromFile(tileImagePath).ToSize(Board.Board.SlotSize - Board.Board.SlotBorderSize));
            }
        }

        /// <summary>
        /// Resize the image to the specified size.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="size">The width and height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ToSize(this Image image, int size)
        {
            return image.ToSize(size, size);
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ToSize(this Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static Image CreateImage(Color color, int width, int height, Color borderColor, int borderLeft = 0, int borderRight = 0, int borderTop = 0, int borderBottom = 0, string text = null, Font font = null, Color? fontColor = null)
        {
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(bitmap))
            using (SolidBrush borderBrush = new SolidBrush(borderColor))
            using (SolidBrush brush = new SolidBrush(color))
            using (SolidBrush fontBrush = new SolidBrush(fontColor ?? Color.Black))
            {
                gfx.FillRectangle(borderBrush, 0, 0, width, height);

                var contentRectangle = new RectangleF(borderLeft, borderTop, width - (borderLeft + borderRight), height - (borderTop + borderBottom));
                gfx.FillRectangle(brush, contentRectangle);

                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                float emSize = height;
                font = font ?? Constants.Fonts.Default;
                font = new Font(font.FontFamily, emSize, font.Style);
                font = FindBestFitFont(gfx, text, font, contentRectangle.Size);

                gfx.DrawString(text, font, fontBrush, contentRectangle, sf);

                return bitmap;
            }
        }

        private static Font FindBestFitFont(Graphics gfx, string text, Font font, SizeF proposedSize)
        {
            // Compute actual size, shrink if needed
            while (true)
            {
                SizeF size = gfx.MeasureString(text, font);

                // It fits, back out
                if (size.Height <= proposedSize.Height &&
                     size.Width <= proposedSize.Width) { return font; }

                // Try a smaller font (90% of old size)
                Font oldFont = font;
                font = new Font(font.Name, (float)(font.Size * .9), font.Style);
                oldFont.Dispose();
            }
        }
    }
}
