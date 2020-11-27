using System;
using System.Collections.Generic;
using System.Text;

namespace kSlovnik.Generic
{
    public class Word
    {
        private int points = 0;

        public string Text { get; set; } = string.Empty;

        public int Points
        {
            get
            {
                var totalPoints = points;
                foreach (var boardSlot in LetterPositions)
                {
                    if(boardSlot.Color == Constants.Colors.TileColors.x2)
                    {
                        totalPoints *= 2;
                    }
                    else if (boardSlot.Color == Constants.Colors.TileColors.x3)
                    {
                        totalPoints *= 3;
                    }
                }
                return totalPoints;
            }
            set
            {
                points = value;
            }
        }

        public List<Board.BoardSlot> LetterPositions { get; set; } = new List<Board.BoardSlot>();

        public bool IsValid { get; set; } = false;
    }
}
