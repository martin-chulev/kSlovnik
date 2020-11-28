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
using System.Windows.Forms;

namespace kSlovnik.Windows
{
    public partial class DictionaryEditor : Form
    {
        private enum SearchTypes
        {
            [Description("Търсене където и да е в думата")]
            Everywhere,

            [Description("Търсене в началото на думата")]
            Start,

            [Description("Търсене в края на думата")]
            End,

            [Description("Търсене на изцяло съвпадаща думи")]
            WholeWord
        }

        private ComboBox SearchType;
        private TextBox SearchBar;
        private Grid<DictionaryWord> WordList;
        private Grid<DictionaryWord> WordFormsList;
        private Grid<DictionaryWord> SimilarWordsList;

        public DictionaryEditor(Form parent)
        {
            InitializeComponent();

            this.Owner = parent;
            this.Width = (int)(parent.Width * 0.8);
            this.Height = (int)(parent.Height * 0.8);
            this.MaximizeBox = false;
            this.BackColor = Color.LightGray;

            var searchBarLabel = new Label()
            {
                Top = Constants.Padding.Top / 2,
                Left = Constants.Padding.Left / 2,
                Width = (int)(this.ClientSize.Width * 1),
                Font = Constants.Fonts.TurnPointsLabel,
                Text = "Изберете тип на търсенето и въведете част от думата"
            };
            this.Controls.Add(searchBarLabel);

            this.SearchType = new ComboBox()
            {
                Top = searchBarLabel.Bottom,
                Left = searchBarLabel.Left,
                Width = (int)(this.ClientSize.Width * 0.5),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            var enumValues = Enum.GetValues(typeof(SearchTypes));
            foreach (var enumValue in enumValues)
            {
                this.SearchType.Items.Add(((SearchTypes)enumValue).GetDescription());
            }
            this.SearchType.SelectedIndex = 0;
            this.SearchType.SelectedIndexChanged += (sender, args) => FilterWords();
            this.Controls.Add(this.SearchType);

            this.SearchBar = new TextBox()
            {
                Top = this.SearchType.Bottom + 3,
                Left = this.SearchType.Left,
                Width = (int)(this.ClientSize.Width * 0.5),
                Font = Constants.Fonts.TurnPointsLabel,
                BackColor = Color.White,
                PlaceholderText = "Търсене на дума..."
            };
            this.SearchBar.TextChanged += (sender, args) => FilterWords();
            this.Controls.Add(this.SearchBar);

            this.WordList = new Grid<DictionaryWord>(Constants.Fonts.Default)
            {
                Left = this.SearchBar.Left,
                Top = this.SearchBar.Bottom + 2,
                Width = this.SearchBar.Width,
                Height = (int)(this.ClientSize.Height * 1.0) - this.SearchBar.Bottom - Constants.Padding.Bottom / 2,
                SelectionBackColor = Constants.Colors.GridBackColor,
                DefaultBackColor = Color.White
            };
            this.WordList.Columns.Add(new Grid<DictionaryWord>.Column(null, null, 10, ContentAlignment.MiddleCenter));
            this.WordList.Columns.Add(new Grid<DictionaryWord>.Column(nameof(DictionaryWord.FullWord), null, 80, ContentAlignment.MiddleLeft));
            this.WordList.Columns.Add(new Grid<DictionaryWord>.Column(null, null, 10, ContentAlignment.MiddleCenter));
            this.WordList.Init(hasHeader: false, clickable: true);

            this.WordList.MouseWheel += WordList_MouseWheel;
            this.WordList.SelectionChanged += WordList_SelectedIndexChanged;

            this.WordList.DataSource = WordController.AllWords.Select(w => new DictionaryWord { Root = w }).ToList();
            this.Controls.Add(this.WordList);

            this.SimilarWordsList = new Grid<DictionaryWord>(Constants.Fonts.Default)
            {
                Left = this.SearchBar.Right + Constants.Padding.Left,
                Height = (int)(this.ClientSize.Height * 0.6) - this.SearchBar.Bottom - Constants.Padding.Bottom / 2,
                DefaultBackColor = Color.White
            };
            this.SimilarWordsList.Top = this.ClientSize.Height - this.SimilarWordsList.Height - Constants.Padding.Bottom / 2;
            this.SimilarWordsList.Width = this.ClientSize.Width - this.SimilarWordsList.Left - Constants.Padding.Right;

            this.SimilarWordsList.Columns.Add(new Grid<DictionaryWord>.Column(null, null, 10, ContentAlignment.MiddleCenter));
            this.SimilarWordsList.Columns.Add(new Grid<DictionaryWord>.Column(nameof(DictionaryWord.FullWord), "Подобни думи", 80, ContentAlignment.MiddleLeft));
            this.SimilarWordsList.Columns.Add(new Grid<DictionaryWord>.Column(null, null, 10, ContentAlignment.MiddleCenter));
            this.SimilarWordsList.Init(hasHeader: true, clickable: false);

            this.SimilarWordsList.MouseWheel += WordList_MouseWheel;

            this.Controls.Add(this.SimilarWordsList);
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

        private void FilterWords()
        {
            var query = this.SearchBar.Text?.Trim().ToUpperInvariant().Replace(" ", "");
            if (string.IsNullOrEmpty(query))
            {
                this.WordList.DataSource = WordController.AllWords
                                                         .Select(w => new DictionaryWord { Root = w }).ToList();
            }
            else
            {
                this.WordList.DataSource = (SearchTypes)this.SearchType.SelectedIndex switch
                {
                    SearchTypes.Everywhere => this.WordList.DataSource = WordController.AllWords
                                                         .Where(w => w.Contains(query))
                                                         .Select(w => new DictionaryWord { Root = w }).ToList(),

                    SearchTypes.Start => this.WordList.DataSource = WordController.AllWords
                                                         .Where(w => w.StartsWith(query))
                                                         .Select(w => new DictionaryWord { Root = w }).ToList(),

                    SearchTypes.End => this.WordList.DataSource = WordController.AllWords
                                                         .Where(w => w.EndsWith(query))
                                                         .Select(w => new DictionaryWord { Root = w }).ToList(),

                    SearchTypes.WholeWord => this.WordList.DataSource = WordController.AllWords
                                                         .Where(w => w.Equals(query))
                                                         .Select(w => new DictionaryWord { Root = w }).ToList(),

                    _ => this.WordList.DataSource = WordController.AllWords
                                                         .Select(w => new DictionaryWord { Root = w }).ToList(),
                };
            }
        }

        private void WordList_SelectedIndexChanged(object sender)
        {
            if (this.WordList != null && this.SimilarWordsList != null)
            {
                if (this.WordList.HasSelectedItem)
                {
                    this.SimilarWordsList.DataSource = WordController.AllWords
                                                             .Where(w => w.Equals(this.WordList.SelectedItem.Root) == false &&
                                                                         w.Contains(this.WordList.SelectedItem.Root)) // TODO: Change to Root Equals Root after fixing dictionary contents
                                                             .Select(w => new DictionaryWord { Root = w }).ToList();
                }
                else
                {
                    this.SimilarWordsList.DataSource = null;
                }
            }
        }

        protected override void OnShown(EventArgs e)
        {
            this.Location = new Point(this.Owner.Left + (this.Owner.Width - this.Width) / 2,
                                      this.Owner.Top + (this.Owner.Height - this.Height) / 2);
            base.OnShown(e);
        }
    }
}
