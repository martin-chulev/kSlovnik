using System;
using System.Collections.Generic;
using System.Text;

namespace kSlovnik.Board
{
    public class Board
    {
        public const int Columns = 15;
        public const int Rows = 15;
        public static int SlotSize = 50;
        public const int SlotBorderSize = 0;

        public static BoardSlot[,] Slots = new BoardSlot[Rows, Columns];

        public static BoardSlot CenterSlot { get => Slots[Constants.CenterPosition.Row, Constants.CenterPosition.Column]; }
    }
}
