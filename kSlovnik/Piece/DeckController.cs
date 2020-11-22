using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kSlovnik.Piece
{
    public static class DeckController
    {
        public static void LoadDeck()
        {
            Constants.DeckInfo.Pieces.Shuffle().ForEach(p => Deck.Pieces.Enqueue(p));
        }

        public static char DrawPiece()
        {
            return Deck.Pieces.Dequeue();
        }
    }
}
