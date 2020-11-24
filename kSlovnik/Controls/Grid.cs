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
        private int currentPage = 0;

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
                RefreshData();
            }
        }

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

        public void Init(bool hasHeader = true)
        {
            var currentStartingY = 0;

            if (hasHeader)
            {
                this.HeaderRow = new Row(Columns, Width - 2 * SystemInformation.BorderSize.Width, Font);
                this.Controls.Add(this.HeaderRow);
                currentStartingY = this.HeaderRow.Bottom;
            }

            var rowHeight = Font.Height + Constants.GridRowPadding;
            var rowCount = (Height - currentStartingY) / rowHeight;

            for (int i = 0; i < rowCount; i++)
            {
                if (i == rowCount - 1) rowHeight = this.Height - currentStartingY;
                var row = new Row(currentStartingY, Columns, default(T), Width - 2 * SystemInformation.BorderSize.Width, Font, rowHeight);
                this.DataRows.Add(row);
                this.Controls.Add(row);
                currentStartingY = row.Bottom;
            }

            this.IsInitialized = true;
        }

        // TODO: Add a way to iterate pages
        private void RefreshData(int page = 0)
        {
            if (this.IsInitialized == false) throw new Exception($"Grid hasn't been initialized yet. Call {nameof(Init)} first.");

            this.currentPage = 0;

            if (this.dataSource.Count > 0)
            {
                var startIndex =  (this.dataSource.Count / this.DataRows.Count) * page;
                var endIndex = startIndex + this.dataSource.Count % this.DataRows.Count - 1;
                for (int i = 0; i < this.DataRows.Count; i++)
                {
                    if (startIndex + i <= endIndex)
                        this.DataRows[i].SetItem(this.dataSource[startIndex + i]);
                    else
                        this.DataRows[i].SetItem(default(T));
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

        public class Column
        {
            public string PropertyName { get; set; }
            public string Header { get; set; }
            public Func<T, object, bool> ShowValue { get; set; }
            public int WidthPercent { get; set; } // TODO: Add checks or scale percentages of all headers down if >100
            public ContentAlignment TextAlign { get; set; }

            public Column(string propertyName, string text, int widthPercent, ContentAlignment textAlign) :
                this(propertyName, text, widthPercent, textAlign, new Func<T, object, bool>((item, data) => true))
            {
            }

            public Column(string propertyName, string text, int widthPercent, ContentAlignment textAlign, Func<T, object, bool> showValue)
            {
                this.PropertyName = propertyName;
                this.Header = text;
                this.WidthPercent = widthPercent;
                this.TextAlign = textAlign;
                this.ShowValue = showValue;
            }
        }

        public class Row : Panel
        {
            private T item;
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
            public Row(int offsetY, List<Column> columns, T item, int width, Font font, int? height = null)
            {
                this.item = item;
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
            }

            public void SetItem(T item)
            {
                this.item = item;

                foreach (var control in this.Controls)
                {
                    if (control is Cell cell)
                    {
                        var i = cell.Index;
                        var data = this.Columns[i].PropertyName != null && item != null ? typeof(T).GetProperty(this.Columns[i].PropertyName)?.GetValue(item) : null;
                        cell.Value = this.Columns[i].ShowValue(item, data) == true ? data : null;
                    }
                }

                if (this.Parent is Grid<T> grid)
                {
                    grid.OnStylesApplied(this, this.item);
                }
            }

            protected override void OnParentChanged(EventArgs e)
            {
                if (this.Parent is Grid<T> grid && this != grid.HeaderRow)
                {
                    grid.OnStylesApplied(this, this.item);
                }
            }

            /// <summary>
            /// Add a cell with data.
            /// </summary>
            /// <param name="offsetX">Starting relative X.</param>
            /// <param name="widthPercent">Percent of the row width.</param>
            /// <param name="data">The data to display in the cell.</param>
            /// <returns>The new cell</returns>
            private Control CreateCell(int index, int offsetX, int widthPercent, ContentAlignment textAlign, object data)
            {
                var cell = new Cell
                {
                    Index = index,
                    Location = new Point(offsetX, 0),
                    Width = this.Width * widthPercent / 100,
                    Height = this.Height,
                    TextAlign = textAlign,
                    ImageAlign = textAlign,
                    Value = data
                };

                return cell;
            }
        }

        public class Cell : Label
        {
            public object value;

            public int Index { get; set; }

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
                        }
                        else
                        {
                            this.Text = value switch
                            {
                                true => "✓",
                                false => value is Word ? "✗" : null,
                                _ => value?.ToString()
                            };
                            this.Image = null;
                            this.value = this.Text;
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
                                    false => value is Word ? "✗" : null,
                                    _ => value?.ToString()
                                };
                                this.Image = null;
                                this.value = this.Text;
                            }
                        });
                    }
                }
            }
        }
    }
}
