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
        private Grid<DictionaryWord> WordList;
        private Grid<DictionaryWord> WordFormsList;

        public DictionaryEditor(Form parent)
        {
            InitializeComponent();

            this.Owner = parent;
            this.Width = (int)(parent.Width * 0.8);
            this.Height = (int)(parent.Height * 0.8);

            this.WordList = new Grid<DictionaryWord>(Constants.Fonts.Default)
            {
                Width = (int)(this.ClientSize.Width * 0.4),
                Height = (int)(this.ClientSize.Height * 1.0)
            };
            this.WordList.Columns.Add(new Grid<DictionaryWord>.Column(null, null, 10, ContentAlignment.MiddleCenter));
            this.WordList.Columns.Add(new Grid<DictionaryWord>.Column(nameof(DictionaryWord.FullWord), null, 80, ContentAlignment.MiddleLeft));
            this.WordList.Columns.Add(new Grid<DictionaryWord>.Column(null, null, 10, ContentAlignment.MiddleCenter));
            this.WordList.Init(hasHeader: false, clickable: true);
            this.WordList.SelectionChanged += (sender) => { this.Text = sender.SelectedItem?.FullWord; };
            this.WordList.DataSource = WordController.AllWords.Select(w => new DictionaryWord { Root = w }).ToList();
            this.Controls.Add(this.WordList);

            /*this.WordFormsList = new ListView()
            {
                Name = "WordFormsList",
                Left = (int)(this.Width * 0.6),
                Width = (int)(this.Width * 0.4)
            };
            this.Controls.Add(this.WordFormsList);*/
        }

        private void WordList_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*this.WordFormsList.Items.Clear();
            if (this.WordList.SelectedItems.Count > 0)
            {
                this.WordFormsList.Items.Add(new ListViewItem(this.WordList.SelectedItems[0].Text));
                this.WordFormsList.Items.Add(new ListViewItem(this.WordList.SelectedItems[0].Text));
                this.WordFormsList.Items.Add(new ListViewItem(this.WordList.SelectedItems[0].Text));
                this.WordFormsList.Items.Add(new ListViewItem(this.WordList.SelectedItems[0].Text));
                this.WordFormsList.Items.Add(new ListViewItem(this.WordList.SelectedItems[0].Text));
            }*/
        }

        protected override void OnShown(EventArgs e)
        {
            this.Location = new Point(this.Owner.Left + (this.Owner.Width - this.Width) / 2,
                                      this.Owner.Top + (this.Owner.Height - this.Height) / 2);
            base.OnShown(e);
        }
    }
}
