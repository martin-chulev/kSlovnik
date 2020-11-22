using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kSlovnik.Generic
{
    public class PotentialWord : Word
    {
        public int StartRow { get; set; }
        public int StartColumn { get; set; }
        public bool IsVertical { get; set; }

        public PotentialWord(string text, int startRow, int startColumn, bool isVertical)
        {
            this.Text = text;
            this.StartRow = startRow;
            this.StartColumn = startColumn;
            this.IsVertical = isVertical;
        }
    }
}
