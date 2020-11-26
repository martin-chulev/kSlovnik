using kSlovnik.Controls;
using kSlovnik.Piece;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace kSlovnik.Board
{
    public static class BoardController
    {
        public static ShadowedPanel BoardContainer;
        public static Panel BoardControl;

        public static void LoadBoard(Control contentPanel)
        {
            if (BoardContainer != null && contentPanel.Controls.Contains(BoardContainer))
                contentPanel.Controls.Remove(BoardContainer);

            BoardContainer = new ShadowedPanel
            {
                Location = new Point(0, 0),
                Width = Board.Columns * Board.SlotSize + Constants.BorderThickness,
                Height = Board.Rows * Board.SlotSize + Constants.BorderThickness,
                BackColor = Color.Black,
                BorderStyle = BorderStyle.None
            };
            BoardContainer.DropShadow();
            contentPanel.Controls.Add(BoardContainer);

            if (BoardControl != null && BoardControl.HasChildren)
                BoardControl.Controls.Clear();

            BoardControl = new Panel
            {
                Location = new Point(Constants.BorderThickness / 2, Constants.BorderThickness / 2),
                Width = Board.Columns * Board.SlotSize,
                Height = Board.Rows * Board.SlotSize,
                BorderStyle = BorderStyle.None
            };
            BoardContainer.Controls.Add(BoardControl);

            var unfilledPositions = new List<Position>();
            for (int r = 0; r < Board.Rows; r++)
            {
                for (int c = 0; c < Board.Columns; c++)
                {
                    unfilledPositions.Add(new Position(r, c)); 
                }
            }

            foreach (var slotPositionProperty in typeof(Constants.SlotPositions).GetFields(BindingFlags.Public | BindingFlags.Static).OrderByDescending(f => !f.Name.Equals("Green")))
            {
                var slotPositionArray = slotPositionProperty.GetValue(null) as Position[];
                foreach (var slotPosition in slotPositionArray)
                {
                    CreateSlot(slotPosition, slotPositionProperty.Name);   
                    unfilledPositions.Remove(slotPosition);
                }
            }

            // Fill the rest of the board with Green tiles
            foreach (var unfilledPosition in unfilledPositions)
            {
                CreateSlot(unfilledPosition, "Green");
            }
        }

        private static void CreateSlot(Position position, string colorName)
        {
            // Add to visuals
            var slot = new BoardSlot
            {
                Width = Board.SlotSize,
                Height = Board.SlotSize,
                Location = new Point(position.PositionX, position.PositionY),

                Position = position,
                Color = (Constants.Colors.TileColors)Enum.Parse(typeof(Constants.Colors.TileColors), colorName),

                Image = position != Constants.CenterPosition ?
                        Resources.ImageController.TileImages[colorName] :
                        Resources.ImageController.TileImages["Center"],

                Tag = (position.Row * Board.Columns + position.Column)
            };
            BoardControl.Controls.Add(slot);

            // Add to logic
            Board.Slots[position.Row, position.Column] = slot;
        }

        public static BoardSlot GetSlotAtPoint(int x, int y)
            => GetSlotAtPoint(new Point(x, y));
        public static BoardSlot GetSlotAtPoint(Point point)
        {
            return BoardControl.GetChildAtPoint(point) as BoardSlot;
        }

        public static BoardSlot GetSlotAtIndex(int index)
        {
            if (index >= 0 && index < Board.Slots.Length)
                return Board.Slots[index / Board.Columns, index % Board.Columns];

            return null;
        }
    }
}
