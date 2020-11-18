using kSlovnik.Board;
using kSlovnik.Controls;
using kSlovnik.Generic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace kSlovnik.Player
{
    public class HandSlot : Slot
    {
        public int Index { get; set; }

        public bool IsInHand { get; set; } = true;

        public BoardSlot CurrentBoardSlot { get; set; }

        public bool IsPlaced { get => CurrentBoardSlot != null; }

        public override string ToString()
        {
            string result = $"{base.ToString()} | Hand({Tag})";
            if (IsPlaced)
                result += $" | In: {CurrentBoardSlot.Position}";

            return result;
        }

        public void SetPiece(char? piece, bool setColor = true)
        {
            if (piece == null || piece == '\0')
            {
                Clear();
            }
            else
            {
                Letter = piece.Value;
                Color = setColor ? Constants.DeckInfo.PieceColors[piece.Value] : Color;
                Image = Resources.ImageController.LetterImagesActive[piece.Value];
                Visible = true;
            }
        }

        public void Clear()
        {
            Letter = '\0';
            Color = Constants.Colors.TileColors.None;
            Image = null;
            Visible = false;
        }

        public void PlaceOnBoard(BoardSlot boardSlot)
        {
            if (boardSlot == null) ReturnToHand();

            if(this.Color == Constants.Colors.TileColors.Grey && this.Letter == '~')
            {
                if (Prompt.ChooseLetter(out var newLetter))
                {
                    Letter = newLetter;
                    Image = Resources.ImageController.LetterImagesActive[newLetter];
                }
                else
                {
                    ReturnToHand();
                    return;
                }
            }

            boardSlot.PendingPiece = this;
            this.CurrentBoardSlot = boardSlot;
            this.Location = boardSlot.GetLocationOnForm();
        }

        public void ReturnToHand()
        {
            if (IsPlaced)
            {
                CurrentBoardSlot.PendingPiece = null;
                CurrentBoardSlot = null;
            }
            ReturnToOriginalPosition();
            IsInHand = true;

            if (Color == Constants.Colors.TileColors.Grey)
            {
                SetPiece('~', setColor: false);
            }
        }

        public void ReturnToOriginalPosition()
        {
            var originalPosition = this.GetLocationFromTag();
            if (originalPosition != Point.Empty)
                this.Location = originalPosition;
        }
    }
}
