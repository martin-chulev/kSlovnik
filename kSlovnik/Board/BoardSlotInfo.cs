using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kSlovnik.Board
{
    public class BoardSlotInfo
    {
        public int Tag { get; set; }

        public string ImageRef { get; set; }

        public char Letter { get; set; } = '\0';

        public Constants.Colors.TileColors Color { get; set; } = Constants.Colors.TileColors.None;

        public Position Position { get; set; }

        public static BoardSlotInfo FromBoardSlot(BoardSlot boardSlot)
        {
            return new BoardSlotInfo
            {
                ImageRef = boardSlot.Image.ToBase64String(),
                Letter = boardSlot.Letter,
                Position = boardSlot.Position,
                Color = boardSlot.Color,
                Tag = (int)boardSlot.Tag
            };
        }

        public static void ApplyBoardInfoToSlot(BoardSlotInfo boardSlotInfo, BoardSlot targetBoardSlot)
        {
            targetBoardSlot.Image = Util.ImageFromBase64String(boardSlotInfo.ImageRef);
            targetBoardSlot.Letter = boardSlotInfo.Letter;
            targetBoardSlot.Position = boardSlotInfo.Position;
            targetBoardSlot.Color = boardSlotInfo.Color;
            targetBoardSlot.Tag = boardSlotInfo.Tag;
        }
    }
}
