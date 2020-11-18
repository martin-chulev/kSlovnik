using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kSlovnik.Player
{
    public class Hand
    {
        public const int PieceLimit = 7;

        public char?[] Pieces = new char?[PieceLimit];

        public int GetFirstFreeSlotIndex()
        {
            var firstNull = Array.IndexOf(Pieces, null);
            var firstZero = Array.IndexOf(Pieces, '\0');

            if (firstNull == -1)
                return firstZero;
            else if (firstZero == -1)
                return firstNull;
            else
                return Math.Min(firstNull, firstZero);
        }
    }
}
