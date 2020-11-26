using kSlovnik.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace kSlovnik.Controls
{
    public static class Prompt
    {
        public static bool ChooseGame(out Game.Game selectedGame)
        {
            selectedGame = null;

            var promptWidth = (int)(Program.MainView.ClientSize.Width / 1.5);
            var promptHeight = (int)(Program.MainView.ClientSize.Height / 2);
            var aspectRatio = (double)Program.MainView.ClientSize.Width / Program.MainView.ClientSize.Height;
            Form prompt = new Form()
            {
                Width = promptWidth,
                Height = promptHeight,
                MaximizeBox = false,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent
            };

            var gamePreviewPanel = new Panel()
            {
                Left = Constants.Padding.Left / 2,
                Top = Constants.Padding.Top / 2,
                Height = prompt.ClientSize.Height - Constants.Padding.Vertical / 2
            };
            gamePreviewPanel.Width = (int)(gamePreviewPanel.Height * aspectRatio);
            prompt.Controls.Add(gamePreviewPanel);
            var placeHolderLabel = new Label()
            {
                Name = "PlaceholderLabel",
                Width = gamePreviewPanel.Width,
                Height = gamePreviewPanel.Height,
                Text = "Не сте избрали игра.",
                TextAlign = ContentAlignment.MiddleCenter
            };
            gamePreviewPanel.Controls.Add(placeHolderLabel);
            var imagePreview = new PictureBox()
            {
                Name = "ImagePreview",
                Width = gamePreviewPanel.Width,
                Height = gamePreviewPanel.Height,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            gamePreviewPanel.Controls.Add(imagePreview);

            var gameList = new ListView()
            {
                Left = gamePreviewPanel.Right + Constants.Padding.Left / 2,
                Top = Constants.Padding.Top / 2,
                Height = (int)((prompt.ClientSize.Height - Constants.Padding.Vertical / 2) * 0.82),
                View = View.List,
                MultiSelect = false,
                ShowItemToolTips = true
            };
            gameList.Width = prompt.ClientSize.Width - gamePreviewPanel.Right - Constants.Padding.Horizontal / 2;
            prompt.Controls.Add(gameList);

            Panel buttonPanel = new Panel()
            {
                Left = gameList.Left,
                Top = gameList.Bottom,
                Width = gameList.Width,
                Height = (int)((prompt.ClientSize.Height - Constants.Padding.Vertical / 2) * 0.18),
            };
            prompt.Controls.Add(buttonPanel);
            Button confirmButton = new Button()
            {
                Left = 0,
                Top = Constants.Padding.Top / 2,
                Width = (buttonPanel.Width - Constants.Padding.Left / 2) / 2,
                Height = buttonPanel.Height - Constants.Padding.Vertical / 2,
                Text = "ОК",
                DialogResult = DialogResult.OK
            };
            buttonPanel.Controls.Add(confirmButton);
            Button cancelButton = new Button()
            {
                Left = confirmButton.Right + Constants.Padding.Left / 2,
                Top = confirmButton.Top,
                Width = confirmButton.Width,
                Height = confirmButton.Height,
                Text = "Отказ",
                DialogResult = DialogResult.Cancel
            };
            buttonPanel.Controls.Add(cancelButton);


            if (Directory.Exists(Constants.SavesFolder))
            {
                // Get all saves
                var saveFilePaths = Directory.GetFiles(Constants.SavesFolder).Where(p => Path.GetFileName(p).EndsWith(".save")).ToList();

                gameList.LargeImageList = new ImageList();
                gameList.LargeImageList.ImageSize = new Size(Math.Min(gameList.Size.Width, 320), Math.Min(gameList.Size.Height, 320));
                foreach (var saveFilePath in saveFilePaths)
                {
                    var fileName = Path.GetFileName(saveFilePath);
                    gameList.LargeImageList.Images.Add(key: saveFilePath, image: Game.Game.LoadSaveFileThumbnail(saveFilePath));

                    var item = new ListViewItem(text: Path.GetFileNameWithoutExtension(saveFilePath), imageKey: saveFilePath);
                    item.ToolTipText = item.Text;
                    gameList.Items.Add(item);
                }
                gameList.SelectedIndexChanged += GameList_SelectedIndexChanged;
                if (gameList.Items.Count > 0)
                {
                    gameList.Items[0].Selected = true;
                    gameList.Select();
                }
            }

            var response = prompt.ShowDialog();
            if (response == DialogResult.OK && gameList.SelectedItems.Count > 0)
            {
                selectedGame = Game.Game.LoadSaveFileGame(gameList.SelectedItems[0].ImageKey);
                return true;
            }

            return false;
        }

        private static void GameList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(sender is ListView gameList && gameList.Parent is Form prompt)
            {
                var placeholderLabel = prompt.Controls.Find("PlaceholderLabel", true).FirstOrDefault();
                var imagePreviewList = prompt.Controls.Find("ImagePreview", true).Select(c => c as PictureBox).Where(c => c != null).ToList();
                if(imagePreviewList != null && imagePreviewList.Count > 0)
                {
                    if (gameList.SelectedItems.Count > 0)
                    {
                        imagePreviewList[0].Image = gameList.LargeImageList.Images[gameList.SelectedItems[0].ImageKey];
                        imagePreviewList[0].Visible = true;
                        if (placeholderLabel != null) placeholderLabel.Visible = false;
                    }
                    else
                    {
                        imagePreviewList[0].Image = null;
                        imagePreviewList[0].Visible = false;
                        if (placeholderLabel != null) placeholderLabel.Visible = true;
                    }
                }
            }
        }

        public static bool ChooseLetter(out char letter)
        {
            var promptWidth = Program.MainView.ClientSize.Width / 8;
            var promptHeight = Program.MainView.ClientSize.Height / 8;

            Form prompt = new Form()
            {
                MaximizeBox = false,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                //Text = Constants.Texts.ChooseLetterPromptTitle,
                StartPosition = FormStartPosition.CenterParent
            };

            int currentStartingY = Constants.Padding.Top / 2;

            Label textLabel = new Label()
            {
                Left = Constants.Padding.Left / 2,
                Top = currentStartingY,
                Width = (int)(promptWidth * 0.8),
                Height = (int)(promptHeight * 0.2),
                Text = Constants.Texts.ChooseLetterPromptText
            };
            textLabel.Left = Constants.Padding.Left / 2 + ((promptWidth - textLabel.Width) / 2);

            currentStartingY += textLabel.Height + Constants.SidebarSeparatorHeight / 2;

            TextBox textBox = new TextBox()
            {
                Top = currentStartingY,
                Width = (int)(promptWidth * 0.8),
                Height = (int)(promptHeight * 0.3),
                MaxLength = 1
            };
            textBox.Font = new Font(Constants.Fonts.Default.FontFamily, Constants.Fonts.Default.Size * 2, FontStyle.Bold);
            textBox.Left = Constants.Padding.Left / 2 + ((promptWidth - textBox.Width) / 2);

            currentStartingY += textBox.Height + Constants.SidebarSeparatorHeight;

            Button confirmButton = new Button()
            {
                Left = Constants.Padding.Left / 2,
                Top = currentStartingY,
                Width = (int)(promptWidth * 0.4),
                Height = (int)(promptHeight * 0.3),
                Text = "ОК",
                DialogResult = DialogResult.OK
            };
            confirmButton.Left = Constants.Padding.Left / 2 + ((promptWidth - confirmButton.Width) / 2);
            confirmButton.Click += (sender, e) => prompt.Close();

            currentStartingY += confirmButton.Height;

            prompt.ClientSize = new Size(promptWidth + Constants.Padding.Horizontal / 2, currentStartingY + Constants.Padding.Bottom / 2);

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmButton);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmButton;

            var response = prompt.ShowDialog();
            if (response == DialogResult.OK && !string.IsNullOrEmpty(textBox.Text))
            {
                letter = textBox.Text.ToLowerInvariant()[0];

                // Check if letter is valid
                if (Constants.DeckInfo.PieceColors.ContainsKey(letter))
                    return true;
            }
            
            letter = '~';
            return false;
        }
    }
}
