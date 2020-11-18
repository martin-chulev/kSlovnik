using kSlovnik.Board;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace kSlovnik
{
    public static class Constants
    {
        public static class Colors
        {
            public enum TileColors
            {
                None = -1,
                Grey = 0,
                Melrose = 1, // Purple
                Yellow = 2,
                DarkBlue = 3,
                DarkRed = 5,
                Green,
                x2,
                x3
            }

            public static readonly Color BackgroundColor = Color.FromArgb(205, 255, 204);
            public static readonly Color SidebarColor = Color.FromArgb(154, 203, 156);

            public static readonly Color GridBackColor = Color.FromArgb(222, 222, 222);

            public static readonly Color HandOuterColor = Color.FromArgb(61, 197, 65);
            public static readonly Color HandInnerColor = Color.FromArgb(0, 64, 4);

            public static readonly Color ButtonColor = Color.FromArgb(211, 209, 197);

            public static readonly Color FontBlack = Color.FromArgb(0, 0, 0);
            public static readonly Color FontBlue = Color.FromArgb(0, 0, 190);
            public static readonly Color FontRed = Color.FromArgb(190, 0, 0);
        }

        public enum GameEndReason
        {
            Forced,
            NoMoreTurns,
            NoMorePieces
        }
        
        public const int MinimumWordLength = 2;
        public const int BonusPointsAllPiecesUsed = 50;

        public static class Fonts
        {
            public static readonly Font Default = new Font("Microsoft Sans Serif", 11, FontStyle.Regular);
            public static readonly Font WordsGrid = new Font("Microsoft Sans Serif", 10, FontStyle.Regular);
        }

        public static class Texts
        {
            public const string TurnPlayer = "Играе: {0}";
            public const string PiecesInDeck = "Налични пулове: {0}";
            public const string TurnPoints = "Успешен ход - {0} точки";

            public const string ChooseLetterPromptText = "Изберете буква:";
        }

        public static Padding Padding;// = new Padding(18);
        public const int BorderThickness = 2;
        public const int PaddingDivider = 50;
        public const int SidebarSeparatorHeight = 10;
        public const int HandTileSeparatorWidth = 0;
        public const int SeparatorWidth = 24;

        public const int TileBorderWidth = 1;

        public const int FPS = 60;

        public static class Shadows
        {
            public const int DropShadowHOffset = 6;
            public const int DropShadowVOffset = 8;
            public const int DropShadowBlur = 0;
            public const int DropShadowSpread = 0;
            public static readonly int DropShadowWidth = DropShadowHOffset + DropShadowSpread;
            public static readonly int DropShadowHeight = DropShadowVOffset + DropShadowSpread;

            /// <summary>
            /// H-offset, V-offset, Blur, Spread, Color, Inset
            /// </summary>
            public static readonly string DropShadowString = $"DropShadow:" +
                                                             $"{DropShadowHOffset}," +
                                                             $"{DropShadowVOffset}," +
                                                             $"{DropShadowBlur}," +
                                                             $"{DropShadowSpread + 1}," +
                                                             $"#000000,noinset";
        }

        public static class SlotPositions
        {
            public static readonly Position[] Melrose = new Position[]
            {
                new Board.Position(5, 7),

                new Board.Position(6, 6),
                new Board.Position(6, 8),

                new Board.Position(7, 5),
                new Board.Position(7, 9),

                new Board.Position(8, 6),
                new Board.Position(8, 8),

                new Board.Position(9, 7),
            };

            public static readonly Position[] Yellow = new Position[]
            {
                new Board.Position(2, 7),

                new Board.Position(3, 6),
                new Board.Position(3, 8),

                new Board.Position(4, 5),
                new Board.Position(4, 9),

                new Board.Position(5, 4),
                new Board.Position(5, 10),

                new Board.Position(6, 3),
                new Board.Position(6, 11),

                new Board.Position(7, 2),
                new Board.Position(7, 12),

                new Board.Position(8, 3),
                new Board.Position(8, 11),

                new Board.Position(9, 4),
                new Board.Position(9, 10),

                new Board.Position(10, 5),
                new Board.Position(10, 9),

                new Board.Position(11, 6),
                new Board.Position(11, 8),

                new Board.Position(12, 7),

            };

            public static readonly Position[] DarkBlue = new Position[]
            {
                new Board.Position(0, 5),
                new Board.Position(0, 9),

                new Board.Position(1, 4),
                new Board.Position(1, 10),

                new Board.Position(2, 3),
                new Board.Position(2, 11),

                new Board.Position(3, 2),
                new Board.Position(3, 12),

                new Board.Position(4, 1),
                new Board.Position(4, 13),

                new Board.Position(5, 0),
                new Board.Position(5, 14),

                new Board.Position(9, 0),
                new Board.Position(9, 14),

                new Board.Position(10, 1),
                new Board.Position(10, 13),

                new Board.Position(11, 2),
                new Board.Position(11, 12),

                new Board.Position(12, 3),
                new Board.Position(12, 11),

                new Board.Position(13, 4),
                new Board.Position(13, 10),

                new Board.Position(14, 5),
                new Board.Position(14, 9),
            };

            public static readonly Position[] DarkRed = new Position[]
            {
                new Board.Position(0, 0),
                new Board.Position(0, 7),
                new Board.Position(0, 14),

                new Board.Position(1, 6),
                new Board.Position(1, 8),

                new Board.Position(6, 1),
                new Board.Position(6, 13),

                new Board.Position(7, 0),
                new Board.Position(7, 7),
                new Board.Position(7, 14),

                new Board.Position(8, 1),
                new Board.Position(8, 13),

                new Board.Position(13, 6),
                new Board.Position(13, 8),

                new Board.Position(14, 0),
                new Board.Position(14, 7),
                new Board.Position(14, 14)
            };

            public static readonly Position[] x2 = new Position[]
            {
                new Board.Position(2, 5),
                new Board.Position(2, 9),

                new Board.Position(3, 4),
                new Board.Position(3, 10),

                new Board.Position(4, 3),
                new Board.Position(4, 11),

                new Board.Position(5, 2),
                new Board.Position(5, 12),

                new Board.Position(9, 2),
                new Board.Position(9, 12),

                new Board.Position(10, 3),
                new Board.Position(10, 11),

                new Board.Position(11, 4),
                new Board.Position(11, 10),

                new Board.Position(12, 5),
                new Board.Position(12, 9),

            };

            public static readonly Position[] x3 = new Position[]
            {
                new Board.Position(0, 2),
                new Board.Position(0, 12),

                new Board.Position(2, 0),
                new Board.Position(2, 14),

                new Board.Position(12, 0),
                new Board.Position(12, 14),

                new Board.Position(14, 2),
                new Board.Position(14, 12),

            };
        }
        public static readonly Position CenterPosition = new Position(7, 7);

        public static class DeckInfo
        {
            public static readonly Dictionary<char, Colors.TileColors> PieceColors = new Dictionary<char, Colors.TileColors>
            {
                { 'а', Colors.TileColors.Melrose },
                { 'б', Colors.TileColors.Yellow },
                { 'в', Colors.TileColors.Yellow },
                { 'г', Colors.TileColors.DarkBlue },
                { 'д', Colors.TileColors.Yellow },
                { 'е', Colors.TileColors.Melrose },
                { 'ж', Colors.TileColors.DarkBlue },
                { 'з', Colors.TileColors.DarkBlue },
                { 'и', Colors.TileColors.Melrose },
                { 'й', Colors.TileColors.DarkBlue },
                { 'к', Colors.TileColors.Yellow },
                { 'л', Colors.TileColors.Yellow },
                { 'м', Colors.TileColors.Yellow },
                { 'н', Colors.TileColors.Melrose },
                { 'о', Colors.TileColors.Melrose },
                { 'п', Colors.TileColors.Melrose },
                { 'р', Colors.TileColors.Melrose },
                { 'с', Colors.TileColors.Melrose },
                { 'т', Colors.TileColors.Melrose },
                { 'у', Colors.TileColors.DarkBlue },
                { 'ф', Colors.TileColors.DarkRed },
                { 'х', Colors.TileColors.DarkRed },
                { 'ц', Colors.TileColors.DarkRed },
                { 'ч', Colors.TileColors.DarkRed },
                { 'ш', Colors.TileColors.DarkRed },
                { 'щ', Colors.TileColors.DarkRed },
                { 'ъ', Colors.TileColors.DarkBlue },
                { 'ь', Colors.TileColors.DarkRed },
                { 'ю', Colors.TileColors.DarkRed },
                { 'я', Colors.TileColors.DarkBlue },
                { '~', Colors.TileColors.Grey }
            };

            public static readonly List<char> Pieces = new List<char>()
            {
                'а','а','а','а','а','а','а','а','а', //9
                'б','б','б', //3
                'в','в','в','в', //4
                'г','г','г', //3
                'д','д','д','д', //4
                'е','е','е','е','е','е','е','е', //8
                'ж','ж', //2
                'з','з', //2
                'и','и','и','и','и','и','и','и', //8
                'й', //1
                'к','к','к', //3
                'л','л','л', //3
                'м','м','м','м', //4
                'н','н','н','н', //4
                'о','о','о','о','о','о','о','о','о', //9
                'п','п','п','п', //4
                'р','р','р','р', //4
                'с','с','с','с', //4
                'т','т','т','т','т', //5
                'у','у','у', //3
                'ф', //1
                'х', //1
                'ц', //1
                'ч','ч', //2
                'ш', //1
                'щ', //1
                'ъ','ъ', //2
                'ь', //1
                'ю', //1
                'я','я', //2
                '~','~', //2 //blank
            };
        }
    }
}
