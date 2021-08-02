using kSlovnik.Generic;
using kSlovnik.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace kSlovnik.Controls
{
    public class Grid<T> : Panel
    {
        private List<T> dataSource;
        public List<T> DataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                dataSource = value ?? new List<T>();
                RefreshData(0);
                SelectedIndex = null;
                UpdateSelectionHighlight();
            }
        }

        public int CurrentPage { get; private set; } = 0;

        public bool IsClickable { get; private set; }

        // TODO: Add SelectionChangedEventArgs which contains the previous item
        public delegate void SelectionChangedEventHandler(Grid<T> sender);
        public event SelectionChangedEventHandler SelectionChanged;

        public bool HasSelectedItem { get => selectedIndex != null; }

        private int? selectedIndex;
        public int? SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                selectedIndex = value;
                UpdateSelectionHighlight();
                SelectionChanged?.Invoke(this);
            }
        }

        public T SelectedItem => SelectedIndex.HasValue ? DataRows[SelectedIndex.Value].Item : default;

        new public Color DefaultBackColor { get; set; } = Constants.Colors.GridBackColor;
        new public Color DefaultForeColor { get; set; } = Color.Black;
        public Color SelectionBackColor { get; set; } = Color.DarkGray;
        public Color SelectionForeColor { get; set; } = Color.Black;

        public bool IsInitialized { get; private set; }

        public Row HeaderRow { get; private set; }
        public List<Row> DataRows { get; }

        public List<Column> Columns { get; }

        // TODO: Add setter which updates font for each Row
        public new Font Font { get; }

        public delegate void ExtraStyleFilter(Row row, T item);
        public event ExtraStyleFilter StylesApplied;
        private void OnStylesApplied(Row row, T item)
        {
            StylesApplied?.Invoke(row, item);
        }

        public Grid(Font font)
        {
            this.IsInitialized = false;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Font = font;
            this.DataRows = new List<Row>();
            this.Columns = new List<Column>();
            this.AutoScroll = false;
        }

        /// <summary>
        /// Initialize the grid.
        /// </summary>
        /// <param name="rowCount">-1 for auto-calculated number.</param>
        /// <param name="hasHeader">Determines whether the first row is filled with column names.</param>
        public void Init(int rowCount = -1, int desiredRowHeight = -1, bool stretchRows = false, bool hasHeader = true, bool clickable = false)
        {
            this.IsClickable = clickable;

            var currentStartingY = 0;

            if (hasHeader)
            {
                this.HeaderRow = new Row(Columns, Width - 2 * SystemInformation.BorderSize.Width, Font);
                this.Controls.Add(this.HeaderRow);
                currentStartingY = this.HeaderRow.Bottom;
            }

            int rowHeight;
            if (rowCount == -1)
            {
                // Calculate from font height
                rowHeight = desiredRowHeight == -1 ? Font.Height + Constants.GridRowPadding : desiredRowHeight;
                rowCount = (this.Height - currentStartingY) / rowHeight;
            }
            else
            {
                // Calculate from control height
                rowHeight = (this.Height - currentStartingY) / rowCount;

                if (stretchRows == false)
                {
                    var rowHeightFromFont = Font.Height + Constants.GridRowPadding;
                    if(rowHeightFromFont < rowHeight)
                    {
                        rowHeight = rowHeightFromFont;
                        rowCount = (this.Height - currentStartingY) / rowHeight;
                    }
                }
            }

            for (int i = 0; i < rowCount; i++)
            {
                if (i == rowCount - 1) rowHeight = this.Height - currentStartingY - 2 * SystemInformation.BorderSize.Height;
                var row = new Row(i, currentStartingY, Columns, default(T), Width - 2 * SystemInformation.BorderSize.Width, Font, rowHeight, clickable);
                this.DataRows.Add(row);
                this.Controls.Add(row);
                currentStartingY = row.Bottom;
            }

            if (this.HeaderRow != null) this.HeaderRow.BackColor = this.DefaultBackColor;
            UpdateSelectionHighlight();

            this.IsInitialized = true;
        }

        public void NextPage()
        {
            RefreshData(this.CurrentPage+1);
        }

        public void PreviousPage()
        {
            RefreshData(this.CurrentPage-1);
        }

        public void RefreshData()
        {
            RefreshData(this.CurrentPage);
        }

        private void RefreshData(int page)
        {
            if (this.IsInitialized == false) throw new Exception($"Grid hasn't been initialized yet. Call {nameof(Init)} first.");

            this.SelectedIndex = null;

            int pageCount = 1;
            if (this.dataSource.Count > this.DataRows.Count)
            {
                pageCount = this.dataSource.Count / (this.DataRows.Count - 1); // Get full pages, considering arrows row
                if (this.dataSource.Count % (this.DataRows.Count - 1) > 0) pageCount++; // Add extra page for remainder (if any), considering arrows row
            }

            if (page >= pageCount)
            {
                this.CurrentPage = 0;
            }
            else if (page < 0)
            {
                this.CurrentPage = pageCount - 1;
            }
            else
            {
                this.CurrentPage = page;
            }

            var dataRowCount = this.DataRows.Count;
            if (pageCount > 1) dataRowCount--; // Leave last row empty for buttons

            if (this.dataSource.Count > 0)
            {
                var startIndex = dataRowCount * this.CurrentPage;
                var endIndex = startIndex + Math.Min(dataRowCount, this.dataSource.Count - startIndex) - 1;
                for (int i = 0; i < dataRowCount; i++)
                {
                    if (startIndex + i <= endIndex)
                        this.DataRows[i].SetItem(this.dataSource[startIndex + i]);
                    else
                        this.DataRows[i].SetItem(default(T));
                }
                
                if (pageCount > 1)
                {
                    // Add page arrows
                    var cellValues = new object[this.DataRows.Last().Columns.Count];
                    var contentAlignments = new ContentAlignment?[this.DataRows.Last().Columns.Count];

                    if (cellValues.Length > 0)
                    {
                        if (this.CurrentPage > 0)
                        {
                            var leftArrow = new Button() { Text = "⮜" /*Image = ImageController.LetterImagesActive['х']*/ };
                            leftArrow.Click += (sender, args) => RefreshData(this.CurrentPage - 1);
                            cellValues[0] = leftArrow;
                            contentAlignments[0] = ContentAlignment.MiddleCenter;
                        }

                        var centerIndex = Math.Max(0, cellValues.Length / 2 - (1 - cellValues.Length % 2));
                        cellValues[centerIndex] = $"страница {this.CurrentPage + 1} от {pageCount}";
                        contentAlignments[centerIndex] = ContentAlignment.MiddleCenter;

                        if (this.CurrentPage < pageCount - 1)
                        {
                            var rightArrow = new Button() { Text = "⮞" /*Image = ImageController.LetterImagesActive['а']*/ };
                            rightArrow.Click += (sender, args) => RefreshData(this.CurrentPage + 1);
                            cellValues[cellValues.Length - 1] = rightArrow;
                            contentAlignments[cellValues.Length - 1] = ContentAlignment.MiddleCenter;
                        }

                        this.DataRows.Last().SetItem(cellValues, contentAlignments);
                    }
                }
            }
            else
            {
                for (int i = 0; i < this.DataRows.Count; i++)
                {
                    this.DataRows[i].SetItem(default(T));
                }
            }
        }

        protected void UpdateSelectionHighlight()
        {
            for (int i = 0; i < this.DataRows.Count; i++)
            {
                if (this.IsClickable && i == SelectedIndex)
                {
                    this.DataRows[i].BackColor = this.SelectionBackColor;
                    this.DataRows[i].ForeColor = this.SelectionForeColor;
                }
                else
                {
                    this.DataRows[i].BackColor = this.DefaultBackColor;
                    this.DataRows[i].ForeColor = this.DefaultForeColor;
                }
                this.OnStylesApplied(this.DataRows[i], this.DataRows[i].Item);
            }
        }

        public class Column
        {
            public string PropertyName { get; set; }
            public string Header { get; set; }
            public Func<T, object, bool> ShowValue { get; set; }
            public Func<T, string> GetValue { get; set; }
            public int WidthPercent { get; set; } // TODO: Add checks or scale percentages of all headers down if >100
            public ContentAlignment TextAlign { get; set; }

            public Column(string propertyName, string text, int widthPercent, ContentAlignment textAlign) :
                this(propertyName, text, widthPercent, textAlign, new Func<T, object, bool>((item, data) => true))
            {
            }

            public Column(string propertyName, string text, int widthPercent, ContentAlignment textAlign, Func<T, object, bool> showValue, Func<T, string> getValue = null)
            {
                this.PropertyName = propertyName;
                this.Header = text;
                this.WidthPercent = widthPercent;
                this.TextAlign = textAlign;
                this.ShowValue = showValue;
                this.GetValue = getValue;
            }
        }

        public class Row : Panel
        {
            public int RowIndex { get; set; }
            public T Item { get; private set; }
            public bool IsHeader { get; }
            public List<Column> Columns { get; }

            /// <summary>
            /// Create a header row.
            /// </summary>
            /// <param name="columns"></param>
            public Row(List<Column> columns, int width, Font font, int? height = null)
            {
                this.IsHeader = true;
                this.Width = width;
                this.Height = height ?? (int)(font.Height * 1.5) + Constants.GridRowPadding;
                this.BorderStyle = BorderStyle.FixedSingle;
                this.Font = font;

                var currentStartingX = 0;
                for (int i = 0; i < columns.Count; i++)
                {
                    var cell = CreateCell(i, currentStartingX, columns[i].WidthPercent, columns[i].TextAlign, columns[i].Header);
                    this.Controls.Add(cell);
                    currentStartingX += cell.Width;
                }
            }

            /// <summary>
            /// Create a data row.
            /// </summary>
            /// <param name="columns"></param>
            /// <param name="item"></param>
            public Row(int rowIndex, int offsetY, List<Column> columns, T item, int width, Font font, int? height = null, bool clickable = false)
            {
                this.Item = item;
                this.RowIndex = rowIndex;
                this.IsHeader = false;
                this.Columns = new List<Column>(columns);
                this.Width = width;
                this.Height = height ?? font.Height + Constants.GridRowPadding;
                this.Top = offsetY;
                this.BorderStyle = BorderStyle.FixedSingle;
                this.Font = font;

                var currentStartingX = 0;
                for (int i = 0; i < this.Columns.Count; i++)
                {
                    var data = this.Columns[i].PropertyName != null && item != null ? typeof(T).GetProperty(this.Columns[i].PropertyName)?.GetValue(item) : null;
                    var cell = CreateCell(i, currentStartingX, this.Columns[i].WidthPercent, this.Columns[i].TextAlign, data);
                    this.Controls.Add(cell);
                    currentStartingX = cell.Right;
                }

                if (clickable)
                {
                    this.Click += Row_Click;
                }
            }

            private void Row_Click(object sender, EventArgs e)
            {
                if (this.Parent is Grid<T> grid && grid.IsClickable)
                {
                    grid.SelectedIndex = grid.SelectedIndex != this.RowIndex && this.Item != null && this.Item.Equals(default(T)) == false ?
                                         this.RowIndex :
                                         null;
                }
            }

            public void OnClick()
            {
                Row_Click(null, null);
            }

            public void SetItem(T item)
            {
                this.Item = item;

                foreach (var control in this.Controls)
                {
                    if (control is Cell cell)
                    {
                        var i = cell.Index;
                        var data = this.Columns[i].GetValue != null ?
                                    this.Columns[i].GetValue.Invoke(item) :
                                    this.Columns[i].PropertyName != null && item != null ? typeof(T).GetProperty(this.Columns[i].PropertyName)?.GetValue(item) : null;
                        cell.Value = this.Columns[i].ShowValue(item, data) == true ? data : null;
                        cell.TextAlign = cell.DefaultContentAlign;
                        cell.ImageAlign = cell.DefaultContentAlign;
                    }
                }

                if (this.Parent is Grid<T> grid)
                {
                    grid.OnStylesApplied(this, this.Item);
                }
            }

            public void SetItem(object[] values, ContentAlignment?[] overrideAlignments = null)
            {
                foreach (var control in this.Controls)
                {
                    if (control is Cell cell)
                    {
                        var i = cell.Index;
                        if (values.Length > i)
                        {
                            cell.Value = values[i];

                            if (overrideAlignments != null && overrideAlignments.Length > i && overrideAlignments[i] != null)
                            {
                                cell.TextAlign = overrideAlignments[i].Value;
                                cell.ImageAlign = overrideAlignments[i].Value;
                            }
                            else
                            {
                                cell.TextAlign = cell.DefaultContentAlign;
                                cell.ImageAlign = cell.DefaultContentAlign;
                            }
                        }
                        else
                        {
                            cell.Value = null;
                        }
                    }
                }
            }

            protected override void OnParentChanged(EventArgs e)
            {
                if (this.Parent is Grid<T> grid && this.IsHeader == false)
                {
                    grid.OnStylesApplied(this, this.Item);
                }
            }

            /// <summary>
            /// Add a cell with data.
            /// </summary>
            /// <param name="offsetX">Starting relative X.</param>
            /// <param name="widthPercent">Percent of the row width.</param>
            /// <param name="data">The data to display in the cell.</param>
            /// <returns>The new cell</returns>
            private Control CreateCell(int index, int offsetX, int widthPercent, ContentAlignment contentAlign, object data)
            {
                var cell = new Cell
                {
                    Index = index,
                    Location = new Point(offsetX, 0),
                    Width = this.Width * widthPercent / 100,
                    Height = this.Height,
                    TextAlign = contentAlign,
                    ImageAlign = contentAlign,
                    DefaultContentAlign = contentAlign,
                    Value = data
                };

                return cell;
            }
        }

        public class Cell : Label
        {
            private object value = null;
            private bool isButton = false;
            private bool hasClickEvent = false;

            public int Index { get; set; }

            public ContentAlignment DefaultContentAlign { get; set; }

            public object Value
            {
                get
                {
                    if (this.value is Image)
                    {
                        return this.Image;
                    }
                    else
                    {
                        return this.Text;
                    }
                }
                set
                {
                    try
                    {
                        if (value is Image image)
                        {
                            this.Text = string.Empty;
                            this.Image = image.ToSize((int)(this.Height * 0.7));
                            this.value = this.Image;
                            this.isButton = false;
                        }
                        else if (value is Button button)
                        {
                            this.Text = button.Text;
                            this.Image = button.Image?.ToSize((int)(this.Height * 0.7));
                            this.value = this.Image;

                            if (this.isButton == false && hasClickEvent == false)
                            {
                                this.Click += (sender, args) =>
                                {
                                    if (sender is Cell cell && cell.isButton == true)
                                    {
                                        button?.PerformClick();
                                    }
                                };
                                this.hasClickEvent = true;
                            }
                            this.isButton = true;
                        }
                        else
                        {
                            this.Text = value switch
                            {
                                true => "✓",
                                false => "✗",
                                _ => value?.ToString()
                            };
                            this.Image = null;
                            this.value = this.Text;
                            this.isButton = false;
                        }
                    }
                    catch
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (value is Image image)
                            {
                                this.Text = string.Empty;
                                this.Image = image.ToSize((int)(this.Height * 0.7));
                                this.value = this.Image;
                            }
                            else
                            {
                                this.Text = value switch
                                {
                                    true => "✓",
                                    false => "✗",
                                    _ => value?.ToString()
                                };
                                this.Image = null;
                                this.value = this.Text;
                            }
                        });
                    }
                }
            }

            public Cell() : base()
            {
                this.MouseDown += (sender, args) =>
                {
                    if (this.Parent is Row row)
                    {
                        row.OnClick();
                    }
                };
            }
        }
    }
}
