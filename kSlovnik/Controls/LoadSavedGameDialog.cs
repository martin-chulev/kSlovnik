using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kSlovnik.Controls
{
    public static class LoadSavedGameDialog
    {
        public static string SelectFile()
        {
            var dialog = new OpenFileDialog()
            {
                InitialDirectory = Constants.SavesFolder,
                RestoreDirectory = true,
                Title = "Изберете игра",
                DefaultExt = "game",
                Filter = "Saved games (*.game)|*.game",
                FilterIndex = 0,
                CheckPathExists = true,
                CheckFileExists = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                return dialog.FileName;

            return null;
        }
    }
}
