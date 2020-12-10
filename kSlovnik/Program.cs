using kSlovnik.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kSlovnik
{
    static class Program
    {
        public static MainView MainView;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var loadingScreen = new Form()
            {
                FormBorderStyle = FormBorderStyle.None,
                BackgroundImage = Resources.ImageController.LoadingScreenImage,
                BackgroundImageLayout = ImageLayout.Zoom
            };

            Screen screen = Screen.FromControl(loadingScreen);
            Rectangle screenArea = screen.WorkingArea;

            var aspectRatio = (double)Resources.ImageController.LoadingScreenImage.Width / Resources.ImageController.LoadingScreenImage.Height;
            loadingScreen.Height = (int)(screenArea.Height * 0.5);
            loadingScreen.Width = (int)(loadingScreen.Height * aspectRatio);
            loadingScreen.Location = new Point(screenArea.Width / 2 - loadingScreen.Width / 2, screenArea.Height / 2 - loadingScreen.Height / 2);
            loadingScreen.Show();

            MainView = new MainView();
            loadingScreen.Close();
            Application.Run(MainView);
        }
    }
}
