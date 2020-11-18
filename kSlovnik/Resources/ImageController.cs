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
        public static Dictionary<string, Image> TileImages = new Dictionary<string, Image>();

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
        private static Bitmap ToSize(this Image image, int size)
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
        private static Bitmap ToSize(this Image image, int width, int height)
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
    }
}
