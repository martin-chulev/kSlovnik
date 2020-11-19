using kSlovnik.Generic;
using kSlovnik.Player;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace kSlovnik.Board
{
    public class BoardSlot : Slot
    {
        public char CurrentLetter { get => PendingPiece != null ? PendingPiece.Letter : this.Letter; }

        public Position Position { get; set; }

        public int Points
        {
            get
            {
                if (PendingPiece == null)
                {
                    return (int)this.Color;
                }
                else
                {
                    if(PendingPiece.Color == this.Color)
                    {
                        return (int)this.Color * 3;
                    }
                    else
                    {
                        return (int)PendingPiece.Color;
                    }
                }
            }
        }

        public HandSlot PendingPiece { get; set; }

        public bool IsPending { get => PendingPiece != null; }

        public override string ToString()
        {
            string result = $"{base.ToString()} | Board({Tag})";
            if (IsPending)
                result += $" | Pending: {PendingPiece.Letter}({PendingPiece.Tag})";

            return result;
        }

        public void ConfirmPiece()
        {
            if (this.IsPending)
            {
                Letter = PendingPiece.Letter;
                Image = PendingPiece.Color == Constants.Colors.TileColors.Grey ?
                    Resources.ImageController.LetterImagesInactiveBlank[PendingPiece.Letter] :
                        Resources.ImageController.LetterImagesInactive[PendingPiece.Letter];
                Color = PendingPiece.Color;

                PendingPiece.Clear();
                PendingPiece.ReturnToHand();
            }
        }
    }
}
