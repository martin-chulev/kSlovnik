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
                Image = Color == Constants.Colors.TileColors.Grey ?
                    Resources.ImageController.LetterImagesActiveBlank[piece.Value] :
                    Resources.ImageController.LetterImagesActive[piece.Value];
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

        public void PlaceOnBoard(int boardSlotRow, int boardSlotColumn, bool changeVisualPosition = true)
        {
            PlaceOnBoard((boardSlotRow * Board.Board.Columns) + boardSlotColumn, changeVisualPosition);
        }
        public void PlaceOnBoard(int boardSlotIndex, bool changeVisualPosition = true)
        {
            PlaceOnBoard(BoardController.GetSlotAtIndex(boardSlotIndex), changeVisualPosition);
        }
        public void PlaceOnBoard(BoardSlot boardSlot, bool changeVisualPosition = true)
        {
            if (boardSlot == null) ReturnToHand();

            if(this.Color == Constants.Colors.TileColors.Grey && this.Letter == '~')
            {
                if (Prompt.ChooseLetter(out var newLetter))
                {
                    //Letter = newLetter;
                    //Image = Resources.ImageController.LetterImagesActiveBlank[newLetter];
                    SetPiece(newLetter, false);
                }
                else
                {
                    ReturnToHand(changeVisualPosition);
                    return;
                }
            }

            boardSlot.PendingPiece = this;
            this.CurrentBoardSlot = boardSlot;
            if (changeVisualPosition) this.Location = boardSlot.GetLocationOnForm();
        }

        public void ReturnToHand(bool changeVisualPosition = true)
        {
            if (IsPlaced)
            {
                CurrentBoardSlot.PendingPiece = null;
                CurrentBoardSlot = null;
            }
            if (changeVisualPosition) ReturnToOriginalPosition();
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
