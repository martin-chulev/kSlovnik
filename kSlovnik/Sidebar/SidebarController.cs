using kSlovnik.Controls;
using kSlovnik.Game;
using kSlovnik.Generic;
using kSlovnik.Piece;
using kSlovnik.Player;
using kSlovnik.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            #region Menu bar
            #region Container
            Sidebar.MenuContainer = new ShadowedPanel
            {
                Location = new Point(Constants.Padding.Left, currentStartingY),
                Width = sidebarContentWidth,
                Height = (int)(sidebarTotalContentHeight * 0.05),
                BackColor = Color.Black,
            };
            Sidebar.MenuContainer.DropShadow();
            Sidebar.SidebarPanel.Controls.Add(Sidebar.MenuContainer);
            Sidebar.MenuPanel = new Panel
            {
                Location = new Point(Constants.BorderThickness, Constants.BorderThickness),
                Width = Sidebar.MenuContainer.Width - 2 * Constants.BorderThickness,
                Height = Sidebar.MenuContainer.Height - 2 * Constants.BorderThickness,
            };
            Sidebar.MenuContainer.Controls.Add(Sidebar.MenuPanel);
            Sidebar.Menu = new Menu
            {
                Padding = new Padding(0),
                Dock = DockStyle.Fill,
                BackColor = Constants.Colors.MenuBackColor
            };
            Sidebar.MenuPanel.Controls.Add(Sidebar.Menu);
            #endregion
            #region Items
            #region File
            var fileButton = new MenuItem("Файл", ImageController.LetterImagesActive['~'])
            {
                Width = Sidebar.Menu.Width / 3,
                Height = Sidebar.Menu.Height
            };
            #region File dropdown items
            fileButton.DropDown = new MenuDropDown();
            var loadSubButton = new MenuDropDownItem("Отвори...", enabled: true);
            loadSubButton.Click += LoadSubButton_Click;
            fileButton.DropDownItems.Add(loadSubButton);

            var saveSubButton = new MenuDropDownItem("Запиши", enabled: true);
            saveSubButton.Click += SaveSubButton_Click;
            fileButton.DropDownItems.Add(saveSubButton);

            fileButton.DropDownItems.Add(new MenuDropDownItem("Запиши като...", withSeparator: true, enabled: false));

            fileButton.DropDownItems.Add(new MenuDropDownItem("Печат...", enabled: false));
            fileButton.DropDownItems.Add(new MenuDropDownItem("Печат Инициализация...", withSeparator: true, enabled: false));

            fileButton.DropDownItems.Add(new MenuDropDownItem("Последни Игри", withSeparator: true, enabled: false));

            var exitSubButton = new MenuDropDownItem("Изход", enabled: true);
            exitSubButton.Click += ExitSubButton_Click;
            fileButton.DropDownItems.Add(exitSubButton);
            #endregion
            Sidebar.Menu.Items.Add(fileButton);
            #endregion
            #region Game
            var gameButton = new MenuItem("Игра", ImageController.LetterImagesActive['~'])
            {
                Width = Sidebar.Menu.Width / 3,
                Height = Sidebar.Menu.Height
            };
            #region Game dropdown items
            gameButton.DropDown = new MenuDropDown();
            var newGameSubButton = new MenuDropDownItem("Нова Игра", enabled: true);
            newGameSubButton.Click += NewGameSubButton_Click;
            gameButton.DropDownItems.Add(newGameSubButton);
            gameButton.DropDownItems.Add(new MenuDropDownItem("Нова Мрежова Игра...", withSeparator: true, enabled: false));
            gameButton.DropDownItems.Add(new MenuDropDownItem("Играчи...", enabled: true));
            gameButton.DropDownItems.Add(new MenuDropDownItem("Постижения", withSeparator: true, enabled: true));
            gameButton.DropDownItems.Add(new MenuDropDownItem("Речник", enabled: true));
            gameButton.DropDownItems.Add(new MenuDropDownItem("Трудност", withSeparator: true, enabled: true));
            gameButton.DropDownItems.Add(new MenuDropDownItem("Изглед", enabled: true));
            var soundsSubButton = new MenuDropDownItem("Звуци", isToggled: UserSettings.SoundsOn, enabled: true);
            soundsSubButton.Click += SoundsSubButton_Click;
            gameButton.DropDownItems.Add(soundsSubButton);
            #endregion
            Sidebar.Menu.Items.Add(gameButton);
            #endregion
            #region Help
            var helpButton = new MenuItem("Помощ", ImageController.LetterImagesActive['~'])
            {
                Width = Sidebar.Menu.Width / 3,
                Height = Sidebar.Menu.Height
            };
            #region Help dropdown items
            helpButton.DropDown = new MenuDropDown();
            helpButton.DropDownItems.Add(new MenuDropDownItem("Индекс...", enabled: true));
            helpButton.DropDownItems.Add(new MenuDropDownItem("Правила...", withSeparator: true, enabled: true));
            helpButton.DropDownItems.Add(new MenuDropDownItem("Относно КръстоСловник...", enabled: true));
            #endregion
            Sidebar.Menu.Items.Add(helpButton);
            #endregion
            #endregion
            #endregion

            currentStartingY += Sidebar.MenuContainer.Height + (int)(1.5 * Constants.SidebarSeparatorHeight);

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
            Sidebar.ScoreboardGrid = new Grid<Player.Player>(Constants.Fonts.ScoreboardGrid)
            {
                Location = new Point(0, 0),
                Width = sidebarContentWidth,
                Height = Sidebar.ScoreboardContainer.Height,
                BackColor = Constants.Colors.GridBackColor
            };
            Sidebar.ScoreboardGrid.StylesApplied += new Grid<Player.Player>.ExtraStyleFilter((Grid<Player.Player>.Row row, Player.Player player) =>
            {
                if (player != null && player.IsInTurn)
                {
                    row.Font = new Font(row.Font, FontStyle.Bold);
                    row.ForeColor = Constants.Colors.FontBlue;
                }
                else
                {
                    row.Font = new Font(row.Font, FontStyle.Regular);
                    row.ForeColor = Color.Black;
                }
            });
            Sidebar.ScoreboardGrid.Columns.Add(new Grid<Player.Player>.Column(nameof(Player.Player.Avatar), null, 8, ContentAlignment.MiddleCenter, new Func<Player.Player, object, bool>((player, data) => player != null ? player.IsInTurn : false)));
            Sidebar.ScoreboardGrid.Columns.Add(new Grid<Player.Player>.Column(nameof(Player.Player.Name), "Играчи", 42, ContentAlignment.MiddleLeft));
            Sidebar.ScoreboardGrid.Columns.Add(new Grid<Player.Player>.Column(nameof(Player.Player.TurnsPlayed), "Ходове", 20, ContentAlignment.MiddleRight));
            Sidebar.ScoreboardGrid.Columns.Add(new Grid<Player.Player>.Column(nameof(Player.Player.Score), "Точки", 20, ContentAlignment.MiddleRight));
            Sidebar.ScoreboardGrid.Columns.Add(new Grid<Player.Player>.Column(null, null, 10, ContentAlignment.MiddleRight));
            Sidebar.ScoreboardGrid.Init(5, stretchRows: true);
            Sidebar.ScoreboardContainer.Controls.Add(Sidebar.ScoreboardGrid);
            #endregion

            currentStartingY += Sidebar.ScoreboardContainer.Height + (Constants.SidebarSeparatorHeight / 2);

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
            #endregion

            currentStartingY += Sidebar.TurnPlayerLabel.Height + (Constants.SidebarSeparatorHeight * 2);

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
            #endregion

            currentStartingY += Sidebar.HandPanelContainer.Height;

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
            #endregion

            currentStartingY += Sidebar.PiecesInDeckLabel.Height + (Constants.SidebarSeparatorHeight * 2);

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
                BackColor = Constants.Colors.ButtonColorBackActive,
                ForeColor = Constants.Colors.ButtonColorForeActive,
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
                BackColor = Constants.Colors.ButtonColorBackActive,
                ForeColor = Constants.Colors.ButtonColorForeActive,
                FlatStyle = FlatStyle.Flat,
                Text = "Прибери пуловете"
            };
            Sidebar.ButtonReset.DropShadow();
            Sidebar.ButtonReset.Click += ButtonResetClick;
            Sidebar.ButtonsContainer.Controls.Add(Sidebar.ButtonReset);
            #endregion

            currentStartingY += Sidebar.ButtonsContainer.Height + Constants.SidebarSeparatorHeight;

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
            #endregion

            currentStartingY += Sidebar.TurnPointsLabel.Height + Constants.SidebarSeparatorHeight / 2;

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
            Sidebar.WordsGrid.StylesApplied += new Grid<Word>.ExtraStyleFilter((Grid<Word>.Row row, Word word) =>
            {
                row.Font = new Font(row.Font, FontStyle.Bold);
                if (word != null)
                {
                    row.ForeColor = word.IsValid ? Constants.Colors.FontBlue : Constants.Colors.FontRed;
                }
            });
            Sidebar.WordsGrid.Columns.Add(new Grid<Word>.Column(nameof(Word.IsValid), null, 10, ContentAlignment.MiddleCenter));
            Sidebar.WordsGrid.Columns.Add(new Grid<Word>.Column(nameof(Word.Text), "Думи", 55, ContentAlignment.MiddleLeft));
            Sidebar.WordsGrid.Columns.Add(new Grid<Word>.Column(nameof(Word.Points), "Точки", 25, ContentAlignment.MiddleRight));
            Sidebar.WordsGrid.Columns.Add(new Grid<Word>.Column(null, null, 10, ContentAlignment.MiddleRight));
            Sidebar.WordsGrid.Init(12, stretchRows: true);
            Sidebar.WordsContainer.Controls.Add(Sidebar.WordsGrid);
            #endregion
        }

        #region Menu button functions
        private static void ExitSubButton_Click(object sender, EventArgs e)
        {
            var confirmed = MessageBox.Show("Сигурни ли сте?", "Изход", MessageBoxButtons.YesNo) == DialogResult.Yes;
            if (confirmed)
            {
                Application.Exit();
            }
        }

        private static void NewGameSubButton_Click(object sender, EventArgs e)
        {
            Game.GameController.NewGame();
            SidebarController.RenderSidebar();
            Game.Game.Save(autosave: true);
            Task.Run(() => GameController.ContinueFromLoadedTurn());
        }

        private static void LoadSubButton_Click(object sender, EventArgs e)
        {
            if (Prompt.ChooseGame())
            {
                var loadedGamePath = Prompt.SelectedGamePath;
                var confirmed = MessageBox.Show("Сигурни ли сте?", "Зареждане на игра", MessageBoxButtons.YesNo) == DialogResult.Yes;
                if (confirmed)
                {
                    Game.Game.Current = Game.Game.LoadSaveFileGame(loadedGamePath);
                    SidebarController.RenderSidebar();
                    Task.Run(() => GameController.ContinueFromLoadedTurn());
                }
            }
            /*var saveFilePath = LoadSavedGameDialog.SelectFile();
            if (string.IsNullOrEmpty(saveFilePath) == false)
            {
                if(Game.Game.Load(saveFilePath, true))
                {
                    SidebarController.RenderSidebar();
                    Task.Run(() => GameController.ContinueFromLoadedTurn());
                }
            }*/
        }

        private static void SaveSubButton_Click(object sender, EventArgs e)
        {
            if (Game.Game.Save(autosave: false))
                MessageBox.Show("Играта е запазена");
        }

        private static void SoundsSubButton_Click(object sender, EventArgs e)
        {
            if(sender is MenuDropDownItem toggle)
            {
                var soundsOn = UserSettings.SoundsOn;

                toggle.IsToggled = !soundsOn;
                UserSettings.SoundsOn = !soundsOn;
                UserSettings.Save();
            }
        }
        #endregion

        public static void RenderSidebar()
        {
            RenderScoreboard();
            RenderTurnPlayerLabel();
            RenderPiecesInDeckLabel();
            RenderWords();
        }

        public static void RenderScoreboard()
        {
            if (Sidebar.ScoreboardGrid != null)
            {
                Sidebar.ScoreboardGrid.Invoke((MethodInvoker)delegate
                {
                    Sidebar.ScoreboardGrid.DataSource = Game.Game.Current.Players;
                });
            }
        }

        public static void RenderTurnPlayerLabel()
        {
            if (Sidebar.TurnPlayerLabel != null)
            {
                Sidebar.TurnPlayerLabel.Invoke((MethodInvoker)delegate
                {
                    Sidebar.TurnPlayerLabel.Text = string.Format(Constants.Texts.TurnPlayer, Game.Game.Current.Players[Game.Game.Current.CurrentPlayerIndex].Name);
                });
            }
        }

        public static void RenderPiecesInDeckLabel()
        {
            if (Sidebar.PiecesInDeckLabel != null)
            {
                Sidebar.PiecesInDeckLabel.Invoke((MethodInvoker)delegate
                {
                    Sidebar.PiecesInDeckLabel.Text = string.Format(Constants.Texts.PiecesInDeck, Deck.Pieces.Count);
                });
            }
        }

        private static async void ButtonConfirmClick(object sender, EventArgs e)
        {
            await GameController.EndTurn();
        }

        private static void ButtonResetClick(object sender, EventArgs e)
        {
            HandController.ReturnAllToHand();
            SidebarController.RenderWords();
        }

        public static void ToggleUserButtons(bool enabled)
        {
            Sidebar.ButtonConfirm.Invoke((MethodInvoker)delegate
            {
                Sidebar.ButtonConfirm.Enabled = enabled;
                Sidebar.ButtonConfirm.BackColor = enabled ? Constants.Colors.ButtonColorBackActive : Constants.Colors.ButtonColorBackInactive;
                Sidebar.ButtonConfirm.ForeColor = enabled ? Constants.Colors.ButtonColorForeActive : Constants.Colors.ButtonColorForeInactive;
                //Sidebar.ButtonConfirm.Visible = enabled;
                //Sidebar.ButtonConfirm.Parent.Visible = enabled;
            });
            Sidebar.ButtonReset.Invoke((MethodInvoker)delegate
            {
                Sidebar.ButtonReset.Enabled = enabled;
                Sidebar.ButtonReset.BackColor = enabled ? Constants.Colors.ButtonColorBackActive : Constants.Colors.ButtonColorBackInactive;
                Sidebar.ButtonReset.ForeColor = enabled ? Constants.Colors.ButtonColorForeActive : Constants.Colors.ButtonColorForeInactive;
                //Sidebar.ButtonReset.Visible = enabled;
                //Sidebar.ButtonReset.Parent.Visible = enabled;
            });
            Sidebar.Menu.Invoke((MethodInvoker)delegate
            {
                Sidebar.Menu.Enabled = enabled;
            });
        }

        public static void RenderTurnPointsLabel(int points)
        {
            Sidebar.TurnPointsLabel.Invoke((MethodInvoker)delegate
            {
                if (Sidebar.TurnPointsLabel != null)
                    Sidebar.TurnPointsLabel.Text = string.Format(Constants.Texts.TurnPoints, points);
            });
        }

        public static void RenderWords()
        {
            var placedPieces = HandController.HandSlots.Where(s => s.IsPlaced).ToList();

            if (GameController.PiecePlacementIsValid(placedPieces))
            {
                var words = GameController.GetNewWords(placedPieces);
                var points = GameController.CalculatePoints(placedPieces);

                if (Sidebar.WordsGrid != null)
                {
                    if (words.Count > 0)
                    {
                        List<Word> bonuses = new List<Word>() { new Word() { Text = "Общо:", Points = points, IsValid = true } };

                        var separatorRowsCount = Sidebar.WordsGrid.DataRows.Count - words.Count - bonuses.Count;
                        for (int i = 0; i < separatorRowsCount; i++)
                        {
                            words.Add(null);
                        }
                        words.AddRange(bonuses);
                    }
                    Sidebar.WordsGrid.DataSource = words;
                }
                RenderTurnPointsLabel(points);
            }
            else
            {
                if (Sidebar.WordsGrid != null)
                {
                    Sidebar.WordsGrid.DataSource = null;
                }
                RenderTurnPointsLabel(0);
            }
        }
    }
}
