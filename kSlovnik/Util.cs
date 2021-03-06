﻿using Ionic.Zip;
using kSlovnik.Board;
using kSlovnik.Generic;
using kSlovnik.Player;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace kSlovnik
{
    public static class Util
    {
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

        public static bool Between(this int value, int minInclusive, int maxExclusive)
        {
            return value >= minInclusive && value < maxExclusive;
        }

        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        public static Dictionary<string, object> GetFieldValues(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Static)
                       .ToDictionary(f => f.Name,
                                     f => f.GetValue(null));
        }

        public static void SetFieldValues(Type type, Dictionary<string, object> newValues)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static).ToList();

            for (int i = 0; i < fields.Count; i++)
            {
                if (newValues.ContainsKey(fields[i].Name))
                {
                    if (fields[i].FieldType == typeof(bool))
                    {
                        fields[i].SetValue(null, ((JsonElement)newValues[fields[i].Name]).GetBoolean());
                    }
                    else
                    {
                        try
                        {
                            fields[i].SetValue(null, Convert.ChangeType(newValues[fields[i].Name], fields[i].FieldType));
                        }
                        catch
                        {
                            var listResult = new List<Player.Player>();
                            var arr = ((JsonElement)newValues[fields[i].Name]).EnumerateArray();
                            foreach (var item in arr)
                            {
                                listResult.Add(JsonSerializer.Deserialize<Player.Player>(item.GetRawText()));
                            }
                            fields[i].SetValue(null, listResult);
                        }
                    }
                }
            }
        }

        public static List<T> Shuffle<T>(this List<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public static void CaptureScreenshot(Form form, int gameId, int turnNumber)
        {
            //Creating a new Bitmap object
            Bitmap captureBitmap = new Bitmap(form.Width - 16, form.Height - 8, PixelFormat.Format32bppArgb);

            //Creating a New Graphics Object
            Graphics captureGraphics = Graphics.FromImage(captureBitmap);

            //Copying Image from The Screen
            captureGraphics.CopyFromScreen(form.Location.X + 8,
                                           form.Location.Y,
                                           0, 0,
                                           new Size(form.Size.Width, form.Size.Height));

            var screenshotsFolderPath = @"Screenshots";
            var timeStamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var fileName = $"{timeStamp}_{gameId}_{turnNumber}.png";

            if (!Directory.Exists(screenshotsFolderPath))
                Directory.CreateDirectory(screenshotsFolderPath);

            captureBitmap.Save(Path.Combine(screenshotsFolderPath, fileName), ImageFormat.Png);
        }

        public static void CaptureScreenshot(Form form, string filePath)
        {
            //Creating a new Bitmap object
            Bitmap captureBitmap = new Bitmap(form.Width - 16, form.Height - 8, PixelFormat.Format32bppArgb);

            //Creating a New Graphics Object
            Graphics captureGraphics = Graphics.FromImage(captureBitmap);

            //Copying Image from The Screen
            captureGraphics.CopyFromScreen(form.Location.X + 8,
                                           form.Location.Y,
                                           0, 0,
                                           new Size(form.Size.Width, form.Size.Height));

            captureBitmap.Save(filePath, ImageFormat.Png);
        }

        public static byte[] ToByteArray(this Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public static string ToBase64String(this Image image)
        {
            return Convert.ToBase64String(image.ToByteArray());
        }

        public static string ToBase64String(this byte[] imageByteArray)
        {
            return Convert.ToBase64String(imageByteArray);
        }

        public static Image ImageFromBase64String(string base64String)
        {
            return Image.FromStream(new MemoryStream(Convert.FromBase64String(base64String)));
        }

        public static void CreatePackagedFile(string packagedFileName, params string[] files)
        {
            using (ZipFile zip = new ZipFile(packagedFileName))
            {
                zip.AddFiles(files, false, "");
                zip.Save();
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
        }

        public static Queue<int> CreateShuffledQueue(int start, int end)
        {
            var numberList = new List<int>();
            for (int number = start; number < end; number++)
                numberList.Add(number);

            return new Queue<int>(numberList.Shuffle());
        }
    }
}
