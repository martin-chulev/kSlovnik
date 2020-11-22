using kSlovnik.Piece;
using kSlovnik.Sidebar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace kSlovnik.Player
{
    public static class HandController
    {
        public static HandSlot[] HandSlots = new HandSlot[Hand.PieceLimit];

        public static void CreateHand(Panel container)
        {
            for (int i = 0; i < Hand.PieceLimit; i++)
            {
                // Add to visuals
                var slot = new HandSlot
                {
                    Width = Board.Board.SlotSize,
                    Height = Board.Board.SlotSize,
                    Location = CalculateLocation(i),
                    Visible = true,
                    
                    Tag = i
                };
                HandSlots[i] = slot;
                container.Controls.Add(slot);
                container.Controls.SetChildIndex(slot, 0);
                slot.MakeMovable();
            }
        }

        public static Point CalculateLocation(int index)
        {
            var handLocation = Sidebar.Sidebar.HandLocation;
            return handLocation.PlusX(Constants.HandTileSeparatorWidth +
                                      index * (Board.Board.SlotSize + Constants.HandTileSeparatorWidth));
        }
        public static int CalculateIndex(Point location)
        {
            var handLocation = Sidebar.Sidebar.HandLocation;
            var relativeLocation = location.Minus(handLocation);

            var y = relativeLocation.Y;
            if (y >= 0 && y <= Board.Board.SlotSize)
            {
                var x = relativeLocation.X;
                return (x - Constants.HandTileSeparatorWidth) / (Board.Board.SlotSize + Constants.HandTileSeparatorWidth);
            }

            return -1;
        }

        public static void LoadHand(Player player)
        {
            // Draw until hand is full
            var freeSlotIndex = player.Hand.GetFirstFreeSlotIndex();
            while (freeSlotIndex >= 0 && Deck.HasPieces())
            {
                char? newPiece = DeckController.DrawPiece();
                player.Hand.Pieces[freeSlotIndex] = newPiece;
                freeSlotIndex = player.Hand.GetFirstFreeSlotIndex();
            }
            SidebarController.RenderPiecesInDeckLabel();

            // Set player's hand in hand slots
            for (int i = 0; i < player.Hand.Pieces.Length; i++)
            {
                HandSlots[i].SetPiece(player.Hand.Pieces[i]);
            }
        }
        public static void SaveHand(Player player)
        {
            for (int i = 0; i < HandSlots.Length; i++)
            {
                player.Hand.Pieces[i] = HandSlots[i].Letter;
            }
        }

        public static HandSlot GetSlotAtPoint(int x, int y)
            => GetSlotAtPoint(new Point(x, y));
        public static HandSlot GetSlotAtPoint(Point point)
        {
            var index = HandController.CalculateIndex(point);
            return (index >= 0 && index < HandSlots.Length) ? HandSlots[index] : null;
        }

        public static void ReturnAllToHand(bool changeVisualPosition = true)
        {
            foreach (var slot in HandSlots)
            {
                slot.ReturnToHand(changeVisualPosition);
            }
        }

        public static void SlidePieces(HandSlot insertedPiece, HandSlot pieceUnderIt)
        {
            int oldIndex = (int)insertedPiece.Tag;
            int newIndex = (int)pieceUnderIt.Tag;

            // If pieces are adjacent
            if (Math.Abs(newIndex - oldIndex) == 1)
            {
                SwapPieces(HandSlots[oldIndex], HandSlots[newIndex]);
                return;
            }

            // If moving to the right
            if (oldIndex < newIndex)
            {
                // Find empty space to the left
                if (TryToSlideLeft(oldIndex, newIndex)) return;
                // Find empty space to the right
                if (TryToSlideRight(oldIndex, newIndex)) return;
            }
            else
            {
                // Find empty space to the right
                if (TryToSlideRight(oldIndex, newIndex)) return;
                // Find empty space to the left
                if (TryToSlideLeft(oldIndex, newIndex)) return;
            }
        }

        private static bool TryToSlideRight(int oldIndex, int newIndex)
        {
            for (int i = newIndex + 1; i < HandSlots.Length; i++)
            {
                bool isPlaced = HandSlots[i].IsPlaced;
                if ((i == oldIndex || HandSlots[i].IsFilled == false) || isPlaced)
                {
                    for (int j = i; j > newIndex; j--)
                    {
                        SwapPieces(HandSlots[j], HandSlots[j - 1]);
                    }

                    if (isPlaced)
                    {
                        SwapPieces(HandSlots[oldIndex], HandSlots[newIndex]);
                    }

                    return true;
                }
            }
            return false;
        }

        private static bool TryToSlideLeft(int oldIndex, int newIndex)
        {
            for (int i = newIndex - 1; i >= 0; i--)
            {
                bool isPlaced = HandSlots[i].IsPlaced;
                if ((i == oldIndex || HandSlots[i].IsFilled == false) || isPlaced)
                {
                    for (int j = i; j < newIndex; j++)
                    {
                        SwapPieces(HandSlots[j], HandSlots[j + 1]);
                    }

                    if (isPlaced)
                    {
                        SwapPieces(HandSlots[oldIndex], HandSlots[newIndex]);
                    }

                    return true;
                }
            }
            return false;
        }

        public static void SwapPieces(HandSlot a, HandSlot b)
        {
            var aIndex = (int)a.Tag;
            var bIndex = (int)b.Tag;
            a.Tag = bIndex;
            b.Tag = aIndex;
            HandSlots[aIndex] = b;
            HandSlots[bIndex] = a;
            if (a.IsPlaced == false) a.ReturnToOriginalPosition();
            if (b.IsPlaced == false) b.ReturnToOriginalPosition();

            /*var handSlotLetter = b.Letter;
            b.SetPiece(a.Letter);
            a.SetPiece(handSlotLetter);*/
        }

        public static void ConfirmAll()
        {
            foreach (var slot in HandSlots)
            {
                if (slot.IsPlaced)
                {
                    slot.CurrentBoardSlot.ConfirmPiece();
                }
            }
        }
    }
}
