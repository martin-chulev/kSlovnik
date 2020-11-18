using System;
using System.Collections.Generic;
using System.Text;

namespace kSlovnik.Piece
{
    public static class Deck
    {
        public static Queue<char> Pieces = new Queue<char>();

        public static bool HasPieces() => Pieces.TryPeek(out var piece);
    }
}
