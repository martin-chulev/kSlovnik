using System;
using System.Collections.Generic;
using System.Text;

namespace kSlovnik.Board
{
    public struct Position
    {
        public int Column { get; set; }
        public int Row { get; set; }

        public int PositionX { get => Column * Board.SlotSize; }
        public int PositionY { get => Row * Board.SlotSize; }

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public override string ToString()
        {
            return $"R:{Row} C:{Column} (X:{PositionX} Y:{PositionY})";
        }

        public static bool operator ==(Position a, Position b)
        {
            return a.Row == b.Row && a.Column == b.Column;
        }

        public static bool operator !=(Position a, Position b)
        {
            return a.Row != b.Row || a.Column != b.Column;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
