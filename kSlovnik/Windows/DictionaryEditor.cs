using kSlovnik.Controls;
using kSlovnik.Generic;
using kSlovnik.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kSlovnik.Windows
{
    public partial class DictionaryEditor : Form
    {
        public Label WordLabel;
        public string CurrentQuery;
        public TextBox SearchBar;
        public Grid<DictionaryWord> WordList;
        public Grid<DictionaryWord> SameRootList;
        public Grid<DictionaryWord> SimilarWordsList;

        public RadioButton SearchEverywhere;
        public RadioButton SearchAtStart;
        public RadioButton SearchAtEnd;
        public RadioButton SearchWholeWord;

        public CheckBox IncludeAccepted;
        public CheckBox IncludePending;
        public CheckBox IncludeInvalidMisc;
        public CheckBox IncludeInvalidLengthAndCharacters;

        public Button ButtonAdd;
        public Button ButtonEdit;
        public Button ButtonRemove;
        public Button ButtonApprove;
        public Button ButtonReject;
        public Button ButtonSetPending;

        private ToolTip ToolTip = new ToolTip();

        public DictionaryEditor(Form parent)
        {
            InitializeComponent();

            this.Owner = parent;
            this.Width = (int)(parent.Width * 0.9);
            this.Height = (int)(parent.Height * 0.9);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.BackColor = Color.LightGray;

            var searchBarLabel = new Label()
            {
                Top = Constants.Padding.Top / 2,
                Left = Constants.Padding.Left / 2,
                Width = (int)(this.ClientSize.Width * 1),
                Font = Constants.Fonts.TurnPointsLabel,
                Text = "Въведете част от думата"
            };
            this.Controls.Add(searchBarLabel);

            this.SearchBar = new TextBox()
            {
                Top = searchBarLabel.Bottom + 2,
                Left = searchBarLabel.Left,
                Width = (int)(this.ClientSize.Width * 0.5),
                Font = Constants.Fonts.TurnPointsLabel,
                BackColor = Color.White,
                PlaceholderText = "Търсене на дума..."
            };
            this.SearchBar.TextChanged += (sender, args) =>
            {
                bool fromCurrent = this.SearchBar?.Text != null && this.CurrentQuery != null && this.SearchBar.Text.Contains(this.CurrentQuery);
                FilterWords(fromCurrent);
                this.CurrentQuery = this.SearchBar.Text;
            };
            this.Controls.Add(this.SearchBar);

            this.WordList = new Grid<DictionaryWord>(Constants.Fonts.Default)
            {
                Left = this.SearchBar.Left,
                Top = this.SearchBar.Bottom + 6,
                Width = this.SearchBar.Width,
                Height = (int)(this.ClientSize.Height * 0.9) - this.SearchBar.Bottom - Constants.Padding.Bottom / 2,
                SelectionBackColor = Constants.Colors.GridBackColor,
                DefaultBackColor = Color.White
            };
            this.WordList.Columns.Add(new Grid<DictionaryWord>.Column(nameof(DictionaryWord.IsApproved), null, 10, ContentAlignment.MiddleCenter));
            this.WordList.Columns.Add(new Grid<DictionaryWord>.Column(nameof(DictionaryWord.FullWord), null, 80, ContentAlignment.MiddleLeft));
            this.WordList.Columns.Add(new Grid<DictionaryWord>.Column(null, null, 10, ContentAlignment.MiddleCenter));
            this.WordList.Init(hasHeader: false, clickable: true);

            this.WordList.StylesApplied += WordList_StylesApplied;
            this.WordList.MouseWheel += WordList_MouseWheel;
            this.WordList.SelectionChanged += (sender) =>
            {
                this.SameRootList.DataSource = null;
                this.SimilarWordsList.DataSource = null;
                this.WordLabel.Text = "Зарежда се...";
                Task.Run(() => WordList_SelectedIndexChanged(sender));
            };
            this.Controls.Add(this.WordList);

            this.ButtonAdd = new Button()
            {
                Top = this.WordList.Bottom + Constants.Padding.Bottom / 2,
                Left = this.WordList.Left,
                Text = "Добави...",
                Width = (this.WordList.Width - 3 * (Constants.Padding.Left / 2)) / 5,
                Height = this.ClientSize.Height - this.WordList.Bottom - Constants.Padding.Bottom,
                BackColor = Color.White,
                Enabled = true
            };
            this.ButtonAdd.Click += ButtonAdd_Click;
            this.Controls.Add(this.ButtonAdd);
            this.ButtonEdit = new Button()
            {
                Top = this.WordList.Bottom + Constants.Padding.Bottom / 2,
                Left = this.ButtonAdd.Right + Constants.Padding.Left / 2,
                Text = "Промени...",
                Width = (this.WordList.Width - 3 * (Constants.Padding.Left / 2)) / 5,
                Height = this.ClientSize.Height - this.WordList.Bottom - Constants.Padding.Bottom,
                BackColor = Color.White,
                Enabled = false
            };
            this.ButtonEdit.Click += ButtonEdit_Click;
            this.Controls.Add(this.ButtonEdit);
            this.ButtonApprove = new Button()
            {
                Top = this.WordList.Bottom + Constants.Padding.Bottom / 2,
                Left = this.ButtonEdit.Right + Constants.Padding.Left / 2,
                Text = "Валидна",
                Width = this.ButtonAdd.Width,
                Height = this.ClientSize.Height - this.WordList.Bottom - Constants.Padding.Bottom,
                ForeColor = Color.White,
                BackColor = Constants.Colors.FontGreen,
                Enabled = false
            };
            this.ButtonApprove.Click += (sender, args) =>
            { 
                this.WordList.SelectedItem.Approve();
                this.WordList.RefreshData();
            };
            this.Controls.Add(this.ButtonApprove);
            this.ButtonReject = new Button()
            {
                Top = this.WordList.Bottom + Constants.Padding.Bottom / 2,
                Left = this.ButtonApprove.Right + Constants.Padding.Left / 2,
                Text = "Невалидна",
                Width = this.ButtonAdd.Width,
                Height = this.ClientSize.Height - this.WordList.Bottom - Constants.Padding.Bottom,
                ForeColor = Color.White,
                BackColor = Constants.Colors.FontRed,
                Enabled = false
            };
            this.ButtonReject.Click += (sender, args) =>
            {
                this.WordList.SelectedItem.Reject();
                this.WordList.RefreshData();
            };
            this.Controls.Add(this.ButtonReject);
            this.ButtonSetPending = new Button()
            {
                Top = this.WordList.Bottom + Constants.Padding.Bottom / 2,
                Left = this.ButtonReject.Right + Constants.Padding.Left / 2,
                Text = "Неопр.",
                Width = this.ButtonAdd.Width,
                Height = this.ClientSize.Height - this.WordList.Bottom - Constants.Padding.Bottom,
                ForeColor = Color.White,
                BackColor = Constants.Colors.FontBlue,
                Enabled = false
            };
            this.ButtonSetPending.Click += (sender, args) =>
            {
                this.WordList.SelectedItem.SetAsPending();
                this.WordList.RefreshData();
            };
            this.Controls.Add(this.ButtonSetPending);

            var searchPanel = new Panel()
            {
                Top = this.SearchBar.Top,
                Left = this.SearchBar.Right + Constants.Padding.Left,
                AutoSize = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            searchPanel.Width = this.ClientSize.Width - searchPanel.Left - Constants.Padding.Right;
            this.Controls.Add(searchPanel);

            var searchTypeLabel = new Label()
            {
                Left = 3,
                Text = "Вид търсене:",
                Font = Constants.Fonts.TurnPointsLabel,
                AutoSize = true
            };
            searchPanel.Controls.Add(searchTypeLabel);

            this.SearchEverywhere = new RadioButton()
            {
                Top = searchTypeLabel.Bottom,
                Left = searchTypeLabel.Left,
                Text = "Където и да е в думата",
                Font = Constants.Fonts.TurnPointsLabel,
                AutoSize = true,
                Checked = true
            };
            this.SearchEverywhere.CheckedChanged += (sender, args) => FilterWords();
            searchPanel.Controls.Add(this.SearchEverywhere);
            this.SearchAtStart = new RadioButton()
            {
                Top = this.SearchEverywhere.Bottom,
                Left = this.SearchEverywhere.Left,
                Text = "В началото думата",
                Font = Constants.Fonts.TurnPointsLabel,
                AutoSize = true
            };
            this.SearchAtStart.CheckedChanged += (sender, args) => FilterWords();
            searchPanel.Controls.Add(this.SearchAtStart);
            this.SearchAtEnd = new RadioButton()
            {
                Top = this.SearchAtStart.Bottom,
                Left = this.SearchAtStart.Left,
                Text = "В края на думата",
                Font = Constants.Fonts.TurnPointsLabel,
                AutoSize = true
            };
            this.SearchAtEnd.CheckedChanged += (sender, args) => FilterWords();
            searchPanel.Controls.Add(this.SearchAtEnd);
            this.SearchWholeWord = new RadioButton()
            {
                Top = this.SearchAtEnd.Bottom,
                Left = this.SearchAtEnd.Left,
                Text = "Цяла дума",
                Font = Constants.Fonts.TurnPointsLabel,
                AutoSize = true
            };
            this.SearchWholeWord.CheckedChanged += (sender, args) => FilterWords();
            searchPanel.Controls.Add(this.SearchWholeWord);

            var includesLabel = new Label()
            {
                Top = this.SearchWholeWord.Bottom,
                Left = this.SearchWholeWord.Left,
                Text = "Показване на:",
                Font = Constants.Fonts.TurnPointsLabel,
                AutoSize = true
            };
            searchPanel.Controls.Add(includesLabel);

            this.IncludeAccepted = new CheckBox()
            {
                Top = includesLabel.Bottom,
                Left = includesLabel.Left,
                Text = "Приети думи",
                Font = Constants.Fonts.TurnPointsLabel,
                AutoSize = true,
                Checked = true,
            };
            this.IncludeAccepted.CheckedChanged += (sender, args) => FilterWords();
            searchPanel.Controls.Add(this.IncludeAccepted);
            this.IncludePending = new CheckBox()
            {
                Top = this.IncludeAccepted.Bottom,
                Left = this.IncludeAccepted.Left,
                Text = "Неопределени думи",
                Font = Constants.Fonts.TurnPointsLabel,
                AutoSize = true,
                Checked = true
            };
            this.IncludePending.CheckedChanged += (sender, args) => FilterWords();
            searchPanel.Controls.Add(this.IncludePending);
            this.IncludeInvalidMisc = new CheckBox()
            {
                Top = this.IncludePending.Bottom,
                Left = this.IncludePending.Left,
                Text = "Отхвърлени думи",
                Font = Constants.Fonts.TurnPointsLabel,
                AutoSize = true
            };
            this.IncludeInvalidMisc.CheckedChanged += (sender, args) => FilterWords();
            searchPanel.Controls.Add(this.IncludeInvalidMisc);
            this.IncludeInvalidLengthAndCharacters = new CheckBox()
            {
                Top = this.IncludeInvalidMisc.Bottom,
                Left = this.IncludeInvalidMisc.Left,
                Text = "Неподходящи за играта думи",
                Font = Constants.Fonts.TurnPointsLabel,
                AutoSize = true
            };
            this.IncludeInvalidLengthAndCharacters.CheckedChanged += (sender, args) =>
            {
                if (this.IncludeInvalidLengthAndCharacters.Checked)
                {
                    if (MessageBox.Show("Този списък е голям и може да се зарежда дълго време. Сигурни ли сте?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        FilterWords();
                    else
                        this.IncludeInvalidLengthAndCharacters.Checked = false;
                }
                else
                    FilterWords();
            };
            searchPanel.Controls.Add(this.IncludeInvalidLengthAndCharacters);


            this.WordLabel = new Label()
            {
                Top = searchPanel.Bottom + Constants.Padding.Bottom / 2,
                Left = searchPanel.Left,
                Width = searchPanel.Width,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = Constants.Fonts.TurnPointsLabel,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.WordLabel.Height = this.WordLabel.Font.Height + 6;
            this.Controls.Add(this.WordLabel);


            this.SameRootList = new Grid<DictionaryWord>(Constants.Fonts.Default)
            {
                Top = this.WordLabel.Bottom + Constants.Padding.Bottom / 2,
                Left = this.SearchBar.Right + Constants.Padding.Left,
                DefaultBackColor = Color.White
            };
            this.SameRootList.Height = (this.ClientSize.Height - this.SameRootList.Top - Constants.Padding.Bottom / 2) / 2;
            this.SameRootList.Width = this.ClientSize.Width - this.SameRootList.Left - Constants.Padding.Right;

            this.SameRootList.Columns.Add(new Grid<DictionaryWord>.Column(nameof(DictionaryWord.IsApproved), null, 10, ContentAlignment.MiddleCenter));
            this.SameRootList.Columns.Add(new Grid<DictionaryWord>.Column(nameof(DictionaryWord.FullWord), "С този корен:", 80, ContentAlignment.MiddleLeft));
            this.SameRootList.Columns.Add(new Grid<DictionaryWord>.Column(null, null, 10, ContentAlignment.MiddleCenter));
            this.SameRootList.Init(hasHeader: true, clickable: false);

            this.SameRootList.StylesApplied += WordList_StylesApplied;
            this.SameRootList.MouseWheel += WordList_MouseWheel;
            this.Controls.Add(this.SameRootList);

            this.SimilarWordsList = new Grid<DictionaryWord>(Constants.Fonts.Default)
            {
                Top = this.SameRootList.Bottom + Constants.Padding.Bottom / 2,
                Left = this.SearchBar.Right + Constants.Padding.Left,
                DefaultBackColor = Color.White
            };
            this.SimilarWordsList.Height = this.ClientSize.Height - this.SimilarWordsList.Top - Constants.Padding.Bottom / 2;
            this.SimilarWordsList.Width = this.ClientSize.Width - this.SimilarWordsList.Left - Constants.Padding.Right;

            this.SimilarWordsList.Columns.Add(new Grid<DictionaryWord>.Column(nameof(DictionaryWord.IsApproved), null, 10, ContentAlignment.MiddleCenter));
            this.SimilarWordsList.Columns.Add(new Grid<DictionaryWord>.Column(nameof(DictionaryWord.FullWord), "Съдържа се в:", 80, ContentAlignment.MiddleLeft));
            this.SimilarWordsList.Columns.Add(new Grid<DictionaryWord>.Column(null, null, 10, ContentAlignment.MiddleCenter));
            this.SimilarWordsList.Init(hasHeader: true, clickable: false);

            this.SimilarWordsList.StylesApplied += WordList_StylesApplied;
            this.SimilarWordsList.MouseWheel += WordList_MouseWheel;
            this.Controls.Add(this.SimilarWordsList);

            FilterWords();
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            var prompt = new DictionaryWordPrompt() { Text = "Добавяне на дума в речника" };
            var wordAdded = prompt.ShowDialog() == DialogResult.OK;
            if (wordAdded)
            {
                this.WordList.RefreshData();
            }
        }

        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            var prompt = new DictionaryWordPrompt(this.WordList?.SelectedItem) { Text = "Редактиране на дума" };
            var wordModified = prompt.ShowDialog() == DialogResult.OK;
            if (wordModified)
            {
                this.WordList.RefreshData();
            }
        }

        private void WordList_StylesApplied(Grid<DictionaryWord>.Row row, DictionaryWord word)
        {
            foreach (Grid<DictionaryWord>.Cell cell in row.Controls)
            {
                if (cell.Index == 0)
                {
                    cell.ForeColor = word?.IsApproved switch
                    {
                        true => Constants.Colors.FontGreen,
                        false => Constants.Colors.FontRed,
                        null => (row.Parent as Grid<DictionaryWord>).DefaultForeColor
                    };

                    switch (word?.IsApproved)
                    {
                        case true:
                            cell.Invoke((MethodInvoker)delegate
                            {
                                cell.ForeColor = Constants.Colors.FontGreen;
                                this.ToolTip.SetToolTip(cell, "Валидна");
                            });
                            break;
                        case false:
                            cell.Invoke((MethodInvoker)delegate
                            {
                                cell.ForeColor = Constants.Colors.FontRed;
                                ToolTip.SetToolTip(cell, "Невалидна");
                            });
                            break;
                        default:
                            cell.Invoke((MethodInvoker)delegate
                            {
                                cell.ForeColor = (row.Parent as Grid<DictionaryWord>).DefaultForeColor;
                                ToolTip.SetToolTip(cell, null);

                            });
                            break;
                    }

                    return;
                }
            }
        }

        private void WordList_MouseWheel(object sender, MouseEventArgs e)
        {
            if (sender is Grid<DictionaryWord> wordList)
            {
                if (e.Delta < 0)
                    wordList.NextPage();
                else if (e.Delta > 0)
                    wordList.PreviousPage();
            }
        }

        private void FilterWords(bool fromCurrent = false)
        {
            this.WordLabel.Text = "Зарежда се...";
            Task.Run(() =>
            {
                List<DictionaryWord> includedWords;

                if (fromCurrent == true)
                {
                    includedWords = this.WordList.DataSource;
                }
                else
                {
                    includedWords = new List<DictionaryWord>();

                    if (this.IncludeAccepted?.Checked == true)
                    {
                        includedWords.AddRange(WordController.ApprovedWords);
                    }

                    if (this.IncludeInvalidMisc?.Checked == true)
                    {
                        includedWords.AddRange(WordController.InvalidMiscWords);
                    }

                    if (this.IncludeInvalidLengthAndCharacters?.Checked == true)
                    {
                        includedWords.AddRange(WordController.InvalidLengthWords);
                        includedWords.AddRange(WordController.InvalidCharactersWords);
                    }

                    if (this.IncludePending?.Checked == true)
                    {
                        includedWords.AddRange(WordController.PendingWords);
                    }

                    includedWords = includedWords.Distinct().OrderBy(w => w.FullWord).ToList();
                }

                var query = this.SearchBar.Text?.Trim().ToUpperInvariant().Replace(" ", "");
                if (string.IsNullOrEmpty(query))
                {
                    this.WordList.DataSource = includedWords.ToList();
                }
                else
                {
                    if (this.SearchEverywhere?.Checked == true)
                        this.WordList.DataSource = this.WordList.DataSource = includedWords.Where(w => w.FullWord.Contains(query)).ToList();
                    else if (this.SearchAtStart?.Checked == true)
                        this.WordList.DataSource = this.WordList.DataSource = includedWords.Where(w => w.FullWord.StartsWith(query)).ToList();
                    else if (this.SearchAtEnd?.Checked == true)
                        this.WordList.DataSource = this.WordList.DataSource = includedWords.Where(w => w.FullWord.EndsWith(query)).ToList();
                    else if (this.SearchWholeWord?.Checked == true)
                        this.WordList.DataSource = this.WordList.DataSource = includedWords.Where(w => w.FullWord.Equals(query)).ToList();
                    else
                        this.WordList.DataSource = includedWords.ToList();
                }
            });
        }

        private void WordList_SelectedIndexChanged(object sender)
        {
            if (this.WordList != null && this.SimilarWordsList != null)
            {
                if (this.WordList.HasSelectedItem)
                {
                    var includedWords = new List<DictionaryWord>();
                    if (this.IncludeAccepted?.Checked == true)
                    {
                        includedWords.AddRange(WordController.ApprovedWords);
                    }

                    if (this.IncludeInvalidMisc?.Checked == true)
                    {
                        includedWords.AddRange(WordController.InvalidMiscWords);
                    }

                    if (this.IncludeInvalidLengthAndCharacters?.Checked == true)
                    {
                        includedWords.AddRange(WordController.InvalidLengthWords);
                        includedWords.AddRange(WordController.InvalidCharactersWords);
                    }

                    if (this.IncludePending?.Checked == true)
                    {
                        includedWords.AddRange(WordController.PendingWords);
                    }

                    this.SameRootList.DataSource = includedWords.Where(w => w.FullWord.Equals(this.WordList.SelectedItem.FullWord) == false &&
                                                                            w.Root.Equals(this.WordList.SelectedItem.Root)).ToList();

                    this.SimilarWordsList.DataSource = includedWords.Where(w => w.FullWord.Equals(this.WordList.SelectedItem.FullWord) == false &&
                                                                                w.FullWord.Contains(this.WordList.SelectedItem.Root)).ToList();

                    this.WordLabel.Text = this.WordList.SelectedItem.FullWordWithMarks;
                    ToggleButtons(true);
                }
                else
                {
                    this.SameRootList.DataSource = null;
                    this.SimilarWordsList.DataSource = null;
                    this.WordLabel.Text = null;
                    ToggleButtons(false);
                }
            }
        }

        private void ToggleButtons(bool enabled)
        {
            //if (ButtonAdd != null) ButtonAdd.Enabled = enabled;
            if (ButtonEdit != null) ButtonEdit.Enabled = enabled;
            if (ButtonRemove != null) ButtonRemove.Enabled = enabled;
            if (ButtonApprove != null) ButtonApprove.Enabled = enabled;
            if (ButtonReject != null) ButtonReject.Enabled = enabled;
            if (ButtonSetPending != null) ButtonSetPending.Enabled = enabled;
        }
    }
}
