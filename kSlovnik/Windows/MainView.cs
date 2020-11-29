using kSlovnik.Board;
using kSlovnik.Controls;
using kSlovnik.Game;
using kSlovnik.Piece;
using kSlovnik.Player;
using kSlovnik.Resources;
using kSlovnik.Sidebar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kSlovnik
{
    public partial class MainView : Form
    {
        public MainView()
        {
            InitializeComponent();

            this.MaximizeBox = false;
            this.BackColor = Constants.Colors.BackgroundColor;

            int borderWidth = (this.Width - this.ClientSize.Width) / 2;
            int titlebarHeight = this.Height - this.ClientSize.Height - 2 * borderWidth;
            
            Screen screen = Screen.FromControl(this);
            Rectangle screenArea = screen.WorkingArea;

            Constants.Padding = new Padding(screenArea.Height / Constants.PaddingDivider);

            // Board size
            Board.Board.SlotSize = (int)((screenArea.Height * 0.8 - Constants.Padding.Vertical) / Board.Board.Rows);
            int boardWidth = Board.Board.Columns * Board.Board.SlotSize;
            int boardHeight = Board.Board.Rows * Board.Board.SlotSize;

            // Form height based on contents
            int formHeight = boardHeight +
                             Constants.Padding.Vertical +
                             Constants.Shadows.DropShadowHeight;
            
            // Sidebar size
            int sidePanelWidth = Constants.HandTileSeparatorWidth +
                                 (Board.Board.SlotSize + Constants.HandTileSeparatorWidth) * Player.Hand.PieceLimit +
                                 Constants.Padding.Horizontal +
                                 2 * Constants.Shadows.DropShadowWidth;
            int sidePanelHeight = boardHeight;

            // Form width based on contents
            int formWidth = boardWidth +
                            Constants.SeparatorWidth +
                            sidePanelWidth +
                            Constants.Padding.Horizontal +
                            Constants.Shadows.DropShadowWidth;

            var contentContainer = new Panel
            {
                Name = "content",
                Location = new Point(Constants.Padding.Left, Constants.Padding.Top),
                Height = formHeight - Constants.Padding.Vertical + Constants.Shadows.DropShadowHeight,
                Width = formWidth - Constants.Padding.Horizontal + Constants.Shadows.DropShadowWidth
            };
            this.Controls.Add(contentContainer);

            // Set form size (and consequently window size) and position on the screen
            this.ClientSize = new Size(formWidth, formHeight);
            this.Location = new Point(screenArea.Width / 2 - this.Width / 2, screenArea.Height / 2 - this.Height / 2);

            ImageController.LoadImages();
            UserSettings.Load();
            SoundController.LoadSounds();
            WordController.LoadWords();
            BoardController.LoadBoard(contentContainer);
            SidebarController.LoadSidebar(contentContainer, positionX: boardWidth + Constants.SeparatorWidth, width: sidePanelWidth, height: sidePanelHeight);
            HandController.CreateHand(contentContainer);
            
            if (Game.Game.Load(null))
            {
                SidebarController.RenderSidebar();
                Task.Run(() => GameController.ContinueFromLoadedTurn());
            }
            else
            {
                GameController.NewGame(contentContainer);
                SidebarController.RenderSidebar();
            }
        }
    }
}
