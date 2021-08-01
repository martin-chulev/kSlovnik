using kSlovnik.Generic;
using kSlovnik.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kSlovnik.Controls
{
    public class DictionaryWordPrompt : Form
    {
        private string originalWord;
        public DictionaryWord Word;
        public TextBox PrefixTextBox;
        public TextBox RootTextBox;
        public TextBox SuffixTextBox;
        public TextBox EndingTextBox;
        public TextBox DefiniteArticleTextBox;
        public Label FullWordLabel;

        public DictionaryWordPrompt(DictionaryWord word = null, bool advanced = true)
        {
            this.Word = word ?? new DictionaryWord();
            this.originalWord = this.Word.FullWord;

            this.StartPosition = FormStartPosition.CenterParent;

            this.PrefixTextBox = new TextBox()
            {
                Text = string.Join(null, this.Word.Prefixes),
                Top = Constants.Padding.Top / 2,
                Left = Constants.Padding.Left / 2,
                TextAlign = HorizontalAlignment.Right,
                PlaceholderText = "представки",
                Visible = advanced,
            };
            if (!advanced) this.PrefixTextBox.Width = 0;
            this.PrefixTextBox.Width = (int)(this.PrefixTextBox.Width * 1.5);
            this.PrefixTextBox.TextChanged += (sender, args) =>
            {
                this.Word.Prefixes = this.PrefixTextBox.Text?.ToUpperInvariant().Split('-', StringSplitOptions.RemoveEmptyEntries).ToList();
                this.FullWordLabel.Text = this.Word.FullWord;
            };
            this.Controls.Add(this.PrefixTextBox);

            this.RootTextBox = new TextBox()
            {
                Text = this.Word.Root,
                Top = this.PrefixTextBox.Top,
                Left = advanced ? this.PrefixTextBox.Right : Constants.Padding.Left / 2,
                TextAlign = HorizontalAlignment.Center,
                PlaceholderText = advanced ? "корен" : "дума..."
            };
            this.RootTextBox.Width = advanced ? this.RootTextBox.Width : (int)(this.RootTextBox.Width * 5.5);
            this.RootTextBox.TextChanged += (sender, args) =>
            {
                this.Word.Root = this.RootTextBox.Text.ToUpperInvariant();
                this.FullWordLabel.Text = this.Word.FullWord;
            };
            this.Controls.Add(this.RootTextBox);

            this.SuffixTextBox = new TextBox()
            {
                Text = string.Join(null, this.Word.Suffixes),
                Top = this.PrefixTextBox.Top,
                Left = this.RootTextBox.Right,
                TextAlign = HorizontalAlignment.Left,
                PlaceholderText = "наставки",
                Visible = advanced
            };
            if (!advanced) this.SuffixTextBox.Width = 0;
            this.SuffixTextBox.TextChanged += (sender, args) =>
            {
                this.Word.Suffixes = this.SuffixTextBox.Text?.ToUpperInvariant().Split('-', StringSplitOptions.RemoveEmptyEntries).ToList();
                this.FullWordLabel.Text = this.Word.FullWord;
            };
            this.Controls.Add(this.SuffixTextBox);

            this.EndingTextBox = new TextBox()
            {
                Text = this.Word.Ending,
                Top = this.PrefixTextBox.Top,
                Left = this.SuffixTextBox.Right,
                TextAlign = HorizontalAlignment.Left,
                PlaceholderText = "окончание",
                Visible = advanced
            };
            if (!advanced) this.EndingTextBox.Width = 0;
            this.EndingTextBox.TextChanged += (sender, args) =>
            {
                this.Word.Ending = this.EndingTextBox.Text?.ToUpperInvariant();
                this.FullWordLabel.Text = this.Word.FullWord;
            };
            this.Controls.Add(this.EndingTextBox);

            this.DefiniteArticleTextBox = new TextBox()
            {
                Text = this.Word.DefiniteArticle,
                Top = this.PrefixTextBox.Top,
                Left = this.EndingTextBox.Right,
                TextAlign = HorizontalAlignment.Left,
                PlaceholderText = "опр. член",
                Visible = advanced
            };
            if (!advanced) this.DefiniteArticleTextBox.Width = 0;
            this.DefiniteArticleTextBox.TextChanged += (sender, args) =>
            {
                this.Word.DefiniteArticle = this.DefiniteArticleTextBox.Text?.ToUpperInvariant();
                this.FullWordLabel.Text = this.Word.FullWord;
            };
            this.Controls.Add(this.DefiniteArticleTextBox);

            this.FullWordLabel = new Label()
            {
                Top = this.PrefixTextBox.Bottom,
                Height = this.Font.Height + 6,
                Width = this.DefiniteArticleTextBox.Right + Constants.Padding.Right / 2,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = this.Word.FullWord
            };
            this.Controls.Add(this.FullWordLabel);

            var acceptButton = new Button()
            {
                Text = "Запази",
                Top = this.FullWordLabel.Bottom,
                Left = this.FullWordLabel.Width / 16 * 3,
                Width = this.FullWordLabel.Width / 4,
                Height = this.PrefixTextBox.Height,
                DialogResult = DialogResult.OK
            };
            acceptButton.Click += AcceptButton_Click;
            this.Controls.Add(acceptButton);

            var cancelButton = new Button()
            {
                Text = "Откажи",
                Top = this.FullWordLabel.Bottom,
                Left = this.FullWordLabel.Width / 16 * 9,
                Width = this.FullWordLabel.Width / 4,
                Height = this.PrefixTextBox.Height,
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(cancelButton);

            this.ClientSize = new Size(this.FullWordLabel.Width, acceptButton.Bottom + Constants.Padding.Bottom / 2);
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            bool tooShort = this.Word.FullWord.Length < Constants.MinimumWordLength;
            if (tooShort)
            {
                MessageBox.Show($"Думата е прекалено къса (трябва да е поне {Constants.MinimumWordLength} букви).");
                DialogResult = DialogResult.None;
                return;
            }

            bool tooLong = this.Word.FullWord.Length > Math.Max(Board.Board.Rows, Board.Board.Columns);
            if (tooShort)
            {
                MessageBox.Show($"Думата е прекалено дълга (трябва да е най-много {Math.Max(Board.Board.Rows, Board.Board.Columns)} букви).");
                DialogResult = DialogResult.None;
                return;
            }

            bool hasInvalidCharacters = this.Word.FullWord.Any(c => c < 'А' || c > 'Я');
            if (hasInvalidCharacters)
            {
                MessageBox.Show($"Думата съдържа невалидни символи (трябва да съдържа само български букви).");
                DialogResult = DialogResult.None;
                return;
            }

            WordController.Approve(this.Word);
            this.Close();
        }
    }
}
