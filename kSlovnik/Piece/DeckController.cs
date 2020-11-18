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

        private static Random rng = new Random();
        private static List<T> Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }
    }
}
