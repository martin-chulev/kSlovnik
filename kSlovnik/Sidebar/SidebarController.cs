using kSlovnik.Controls;
using kSlovnik.Game;
using kSlovnik.Generic;
using kSlovnik.Piece;
using kSlovnik.Player;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace kSlovnik.Sidebar
{
    public static class SidebarController
    {
        public static void LoadSidebar(Control contentPanel, int positionX, int width, int height)
        {
            #region Sidebar
            Sidebar.SidebarContainer = new ShadowedPanel
            {
                Location = new Point(positionX, 0),
                Width = width + 2 * Constants.BorderThickness,
                Height = height + 2 * Constants.BorderThickness,
                BackColor = Color.Black,
            };
            Sidebar.SidebarContainer.DropShadow();
            contentPanel.Controls.Add(Sidebar.SidebarContainer);

            Sidebar.SidebarPanel = new Panel
            {
                Location = new Point(Constants.BorderThickness, Constants.BorderThickness),
                Width = width,
                Height = height,
                BackColor = Constants.Colors.SidebarColor,
            };
            Sidebar.SidebarContainer.Controls.Add(Sidebar.SidebarPanel);
            #endregion

            int sidebarContentWidth = width - Constants.Padding.Horizontal;
            int sidebarTotalContentHeight = Sidebar.SidebarPanel.Height - (int)(Constants.SidebarSeparatorHeight * 6.0) - Constants.Padding.Vertical;

            int currentStartingY = Constants.Padding.Top;

            #region Menu
            Sidebar.MenuContainer = new ShadowedPanel
            {
                Location = new Point(Constants.Padding.Left, currentStartingY),
                Width = sidebarContentWidth,
                Height = (int)(sidebarTotalContentHeight * 0.05),
                BackColor = Constants.Colors.HandInnerColor,
            };
            Sidebar.MenuContainer.DropShadow();
            Sidebar.SidebarPanel.Controls.Add(Sidebar.MenuContainer);
            // menupanel

            currentStartingY += Sidebar.MenuContainer.Height + (int)(1.5 * Constants.SidebarSeparatorHeight);
            #endregion

            #region Scoreboard
            Sidebar.ScoreboardContainer = new ShadowedPanel
            {
                Location = new Point(Constants.Padding.Left, currentStartingY),
                Width = sidebarContentWidth,
                Height = (int)(sidebarTotalContentHeight * 0.25),
                BackColor = Constants.Colors.HandInnerColor,
            };
            Sidebar.ScoreboardContainer.DropShadow();
            Sidebar.SidebarPanel.Controls.Add(Sidebar.ScoreboardContainer);
            /*var highscorePanel = new Panel
            {
                ////
                Location = new Point(0, 0),
                Width = width,
                Height = Board.Board.Rows * Board.Board.SlotSize,
                BackColor = Constants.Colors.SidebarColor,
            };
            highscoreContainer.Controls.Add(highscorePanel);*/

            currentStartingY += Sidebar.ScoreboardContainer.Height + (Constants.SidebarSeparatorHeight / 2);
            #endregion

            #region Turn player label
            // Center vertically while taking lack of shadow in consideration
            currentStartingY += Constants.Shadows.DropShadowHeight / 2;
            Sidebar.TurnPlayerLabel = new Label
            {
                Location = new Point(Constants.Padding.Left, currentStartingY),
                Width = sidebarContentWidth + Constants.Shadows.DropShadowWidth,
                BackColor = Color.Transparent,
                ForeColor = Constants.Colors.FontBlack,
                Font = new Font(Constants.Fonts.Default, FontStyle.Bold),
                TextAlign = ContentAlignment.TopLeft,
                Text = Constants.Texts.TurnPlayer
            };
            Sidebar.TurnPlayerLabel.Height = Sidebar.TurnPlayerLabel.Font.Height;
            Sidebar.SidebarPanel.Controls.Add(Sidebar.TurnPlayerLabel);

            currentStartingY += Sidebar.TurnPlayerLabel.Height + (Constants.SidebarSeparatorHeight * 2);
            #endregion

            #region Hand
            Sidebar.HandPanelContainer = new Panel
            {
                Location = new Point(Constants.Padding.Left - Constants.Shadows.DropShadowWidth, currentStartingY),
                Width = sidebarContentWidth + 3 * Constants.Shadows.DropShadowWidth,
                Height = (int)(Board.Board.SlotSize * 1.1) + Constants.Shadows.DropShadowWidth,
                BackColor = Color.Transparent
            };
            Sidebar.SidebarPanel.Controls.Add(Sidebar.HandPanelContainer);
            Sidebar.HandPanelOuter = new ShadowedPanel
            {
                Location = new Point(0, (int)(Sidebar.HandPanelContainer.Height * 0.35) - Constants.Shadows.DropShadowWidth),
                Width = Sidebar.HandPanelContainer.Width - Constants.Shadows.DropShadowWidth,
                Height = Sidebar.HandPanelContainer.Height - (int)(Sidebar.HandPanelContainer.Height * 0.35),
                BackColor = Constants.Colors.HandOuterColor,
            };
            Sidebar.HandPanelOuter.DropShadow();
            Sidebar.HandPanelContainer.Controls.Add(Sidebar.HandPanelOuter);
            Sidebar.HandPanelInner = new Panel
            {
                Location = new Point(4, 0),
                Width = Sidebar.HandPanelOuter.Width - 8,
                Height = (int)(Sidebar.HandPanelOuter.Height * 0.9),
                BackColor = Constants.Colors.HandInnerColor,
            };
            Sidebar.HandPanelOuter.Controls.Add(Sidebar.HandPanelInner);
            Sidebar.HandLocation = Sidebar.HandPanelContainer.GetLocationOnForm().PlusX(Sidebar.HandPanelInner.Location.X);

            currentStartingY += Sidebar.HandPanelContainer.Height;
            #endregion

            #region Pieces in deck label
            Sidebar.PiecesInDeckLabel = new Label
            {
                Location = new Point(Constants.Padding.Left, currentStartingY),
                Width = sidebarContentWidth + Constants.Shadows.DropShadowWidth,
                BackColor = Color.Transparent,
                ForeColor = Constants.Colors.FontBlack,
                Font = Constants.Fonts.Default,
                TextAlign = ContentAlignment.TopLeft,
                Text = Constants.Texts.PiecesInDeck
            };
            Sidebar.PiecesInDeckLabel.Height = Sidebar.PiecesInDeckLabel.Font.Height;
            Sidebar.SidebarPanel.Controls.Add(Sidebar.PiecesInDeckLabel);

            currentStartingY += Sidebar.PiecesInDeckLabel.Height + (Constants.SidebarSeparatorHeight * 2);
            #endregion

            #region Buttons
            Sidebar.ButtonsContainer = new Panel
            {
                Location = new Point(Constants.Padding.Left, currentStartingY),
                Width = sidebarContentWidth + Constants.Shadows.DropShadowWidth,
                Height = (int)(sidebarTotalContentHeight * 0.06),
            };
            Sidebar.SidebarPanel.Controls.Add(Sidebar.ButtonsContainer);
            Sidebar.ButtonConfirm = new ShadowedButton
            {
                Location = new Point(0, 0),
                Width = (int)(Sidebar.ButtonsContainer.Width * 0.35),
                Height = Sidebar.ButtonsContainer.Height - Constants.Shadows.DropShadowWidth,
                BackColor = Constants.Colors.ButtonColor,
                FlatStyle = FlatStyle.Flat,
                Text = "Играй"
            };
            Sidebar.ButtonConfirm.DropShadow();
            Sidebar.ButtonConfirm.Click += ButtonConfirmClick;
            Sidebar.ButtonsContainer.Controls.Add(Sidebar.ButtonConfirm);
            Sidebar.ButtonReset = new ShadowedButton
            {
                Location = new Point((int)(Sidebar.ButtonsContainer.Width * 0.45) - Constants.Shadows.DropShadowWidth, 0),
                Width = (int)(Sidebar.ButtonsContainer.Width * 0.55),
                Height = Sidebar.ButtonsContainer.Height - Constants.Shadows.DropShadowWidth,
                BackColor = Constants.Colors.ButtonColor,
                FlatStyle = FlatStyle.Flat,
                Text = "Прибери пуловете"
            };
            Sidebar.ButtonReset.DropShadow();
            Sidebar.ButtonReset.Click += ButtonResetClick;
            Sidebar.ButtonsContainer.Controls.Add(Sidebar.ButtonReset);

            currentStartingY += Sidebar.ButtonsContainer.Height + Constants.SidebarSeparatorHeight;
            #endregion

            #region Turn points label
            // Center vertically while taking lack of shadow in consideration
            currentStartingY += Constants.Shadows.DropShadowHeight / 2;

            Sidebar.TurnPointsLabel = new Label
            {
                Location = new Point(Constants.Padding.Left, currentStartingY),
                Width = sidebarContentWidth + Constants.Shadows.DropShadowWidth,
                BackColor = Color.Transparent,
                ForeColor = Constants.Colors.FontBlue,
                Font = Constants.Fonts.Default,
                TextAlign = ContentAlignment.BottomLeft,
                Text = Constants.Texts.TurnPoints
            };
            Sidebar.TurnPointsLabel.Height = Sidebar.TurnPointsLabel.Font.Height;
            Sidebar.SidebarPanel.Controls.Add(Sidebar.TurnPointsLabel);

            currentStartingY += Sidebar.TurnPointsLabel.Height + Constants.SidebarSeparatorHeight / 2;
            #endregion

            #region Current words
            Sidebar.WordsContainer = new ShadowedPanel
            {
                Location = new Point(Constants.Padding.Left, currentStartingY),
                Width = sidebarContentWidth,
                Height = Sidebar.SidebarPanel.Height - currentStartingY - Constants.Padding.Bottom, // Fill up remainder of the sidebar
                BackColor = Color.Transparent,
            };
            Sidebar.WordsContainer.DropShadow();
            Sidebar.SidebarPanel.Controls.Add(Sidebar.WordsContainer);
            Sidebar.WordsGrid = new Grid<Word>(Constants.Fonts.WordsGrid)
            {
                Location = new Point(0, 0),
                Width = sidebarContentWidth,
                Height = Sidebar.WordsContainer.Height,
                BackColor = Constants.Colors.GridBackColor
            };
            Sidebar.WordsGrid.Columns.Add(new Grid<Word>.Column(nameof(Word.IsValid), null, 10, ContentAlignment.MiddleCenter));
            Sidebar.WordsGrid.Columns.Add(new Grid<Word>.Column(nameof(Word.Text), "Думи", 55, ContentAlignment.MiddleLeft));
            Sidebar.WordsGrid.Columns.Add(new Grid<Word>.Column(nameof(Word.Points), "Точки", 25, ContentAlignment.MiddleRight));
            Sidebar.WordsGrid.Columns.Add(new Grid<Word>.Column(null, null, 10, ContentAlignment.MiddleRight));
            Sidebar.WordsGrid.Init();
            Sidebar.WordsContainer.Controls.Add(Sidebar.WordsGrid);
            #endregion
        }

        public static void RenderSidebar()
        {
            RenderHighscores();
            RenderTurnPlayerLabel();
            RenderPiecesInDeckLabel();
            RenderWords();
        }

        public static void RenderHighscores()
        {

        }

        public static void RenderTurnPlayerLabel()
        {
            if (Sidebar.TurnPlayerLabel != null)
                Sidebar.TurnPlayerLabel.Text = string.Format(Constants.Texts.TurnPlayer, Game.Game.Current.Players[Game.Game.Current.CurrentPlayerIndex].Name);
        }

        public static void RenderPiecesInDeckLabel()
        {
            if (Sidebar.PiecesInDeckLabel != null)
                Sidebar.PiecesInDeckLabel.Text = string.Format(Constants.Texts.PiecesInDeck, Deck.Pieces.Count);
        }

        private static void ButtonConfirmClick(object sender, EventArgs e)
        {
            GameController.EndTurn();
        }

        private static void ButtonResetClick(object sender, EventArgs e)
        {
            HandController.ReturnAllToHand();
            SidebarController.RenderWords();
        }

        public static void RenderTurnPointsLabel(int points)
        {
            if (Sidebar.TurnPointsLabel != null)
                Sidebar.TurnPointsLabel.Text = string.Format(Constants.Texts.TurnPoints, points);
        }

        public static void RenderWords()
        {
            var placedPieces = HandController.HandSlots.Where(s => s.IsPlaced).ToList();

            if (GameController.PiecePlacementIsValid(placedPieces))
            {
                var words = GameController.GetNewWords(placedPieces);
                var points = GameController.CalculatePoints(placedPieces);

                if (Sidebar.WordsGrid != null)
                    Sidebar.WordsGrid.DataSource = words;
                RenderTurnPointsLabel(points);
            }
            else
            {
                if (Sidebar.WordsGrid != null)
                    Sidebar.WordsGrid.DataSource = null;
                RenderTurnPointsLabel(0);
            }
        }
    }
}
