using kSlovnik.Controls;
using kSlovnik.Generic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace kSlovnik.Sidebar
{
    public static class Sidebar
    {
        public static ShadowedPanel SidebarContainer;
        public static Panel SidebarPanel;
        public static ShadowedPanel MenuContainer;
        public static ShadowedPanel ScoreboardContainer;
        public static Grid<Player.Player> ScoreboardGrid;
        public static Label TurnPlayerLabel;
        public static Panel HandPanelContainer;
        public static ShadowedPanel HandPanelOuter;
        public static Panel HandPanelInner;
        public static Label PiecesInDeckLabel;
        public static Panel ButtonsContainer;
        public static ShadowedButton ButtonConfirm;
        public static ShadowedButton ButtonReset;
        public static Label TurnPointsLabel;
        public static ShadowedPanel WordsContainer;
        public static Grid<Word> WordsGrid;

        public static Point HandLocation;
    }
}
