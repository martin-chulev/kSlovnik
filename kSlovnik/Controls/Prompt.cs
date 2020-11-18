using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace kSlovnik.Controls
{
    public static class Prompt
    {
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
