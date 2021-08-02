using kSlovnik.Board;
using kSlovnik.Generic;
using kSlovnik.Piece;
using kSlovnik.Player;
using kSlovnik.Resources;
using kSlovnik.Sidebar;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kSlovnik.Game
{
    public static class GameController
    {
        public static void NewGame(Panel contentContainer = null)
        {
            Game.Current = new Game() { Id = DateTime.Now.ToString("yyyyMMddHHmmss") };

            foreach (var player in UserSettings.Players)
            {
                Game.Current.Players.Add(new Player.Player(player.Name, player.Avatar, player.AI));
            }
            Game.Current.CurrentPlayerIndex = 0;
            SidebarController.RenderTurnPlayerLabel();

            DeckController.LoadDeck();
            HandController.ReturnAllToHand(changeVisualPosition: true);
            HandController.LoadHand(Game.Current.CurrentPlayer);
            HandController.SaveHand(Game.Current.CurrentPlayer);
            BoardController.LoadBoard(contentContainer ?? Program.MainView.Controls.Find("content", true).First());
        }

        public static async Task ContinueFromLoadedTurn()
        {
            await Task.Run(() => StartNextTurn(false));
        }

        public static async Task EndTurn()
        {
            await Task.Run(() =>
            {
                if (EndCurrentTurn())
                    StartNextTurn();
            });
        }

        private static bool EndCurrentTurn()
        {
            // Get all placed pieces
            var placedPieces = HandController.HandSlots.Where(s => s.IsPlaced).ToList();

            if (PiecePlacementIsValid(placedPieces) == false)
                return false;

            if (placedPieces.Any())
            {
                var words = GetNewWords(placedPieces);
                if (WordsAreValid(words) == false)
                {
                    Game.Current.TurnErrors.Add(Constants.InvalidTurnReason.InvalidWord);
                    return false;
                }

                Game.Current.TurnsWithoutPlacement = 0;
                Game.Current.CurrentPlayer.TurnsPlayed++;

                var turnPoints = CalculatePoints(placedPieces);
                Game.Current.CurrentPlayer.Score += turnPoints;

                if (Game.Current.CurrentPlayer.IsAI == false && turnPoints >= Constants.ScreenshotMinimumPoints)
                {
                    // TODO: Save points in a highscore board as well
                    Util.CaptureScreenshot(Program.MainView, 0, Game.Current.Players.Sum(p => p.TurnsPlayed));
                }

                HandController.ConfirmAll();
                SidebarController.RenderWords();

                HandController.SaveHand(Game.Current.CurrentPlayer);
            }
            else
            {
                Game.Current.TurnsWithoutPlacement++;
                Game.Current.CurrentPlayer.TurnsPlayed++;
                if (Game.Current.TurnsWithoutPlacement == 2 * Game.Current.Players.Count)
                {
                    EndGame(Constants.GameEndReason.NoMoreTurns);
                    return false;
                }
            }

            return true;
        }

        private static void StartNextTurn(bool nextPlayer = true)
        {
            if (nextPlayer)
            {
                Game.Current.CurrentPlayerIndex++;
                if (Game.Current.CurrentPlayerIndex >= Game.Current.Players.Count)
                {
                    Game.Current.CurrentPlayerIndex = 0;
                }
            }
            SidebarController.ToggleUserButtons(Game.Current.CurrentPlayer.IsAI == false);
            SidebarController.RenderTurnPlayerLabel();
            SidebarController.RenderTurnPointsLabel(0);
            SidebarController.RenderScoreboard();
            HandController.LoadHand(Game.Current.CurrentPlayer);
            if (Game.Current.CurrentPlayer.IsAI)
            {
                Game.Current.CurrentPlayer.AI.PlayTurn();
            }
            else
            {
                if(nextPlayer) Game.Save(autosave: true);
            }
        }

        public static bool PiecePlacementIsValid(List<HandSlot> placedPieces = null)
        {
            placedPieces ??= HandController.HandSlots.Where(s => s.IsPlaced).ToList();

            // If no pieces were placed - nothing to check
            if (placedPieces.Any() == false) return true;

            // Check if there is a piece in the center slot
            var centerSlot = Board.Board.CenterSlot;
            if (centerSlot.IsPending == false && centerSlot.IsFilled == false)
            {
                Game.Current.TurnErrors.Add(Constants.InvalidTurnReason.CenterNotFilled);
                return false;
            }

            // Check if pieces are in the same horizontal line
            bool allInTheSameRow = !placedPieces.Select(p => p.CurrentBoardSlot.Position.Row)
                                                .Distinct()
                                                .Skip(1)
                                                .Any();
            // Check if pieces are in the same vertical line
            bool allInTheSameColumn = !placedPieces.Select(p => p.CurrentBoardSlot.Position.Column)
                                                   .Distinct()
                                                   .Skip(1)
                                                   .Any();
            // If pieces are not in 1 line - placement is invalid
            if (allInTheSameRow == false && allInTheSameColumn == false)
            {
                Game.Current.TurnErrors.Add(Constants.InvalidTurnReason.NotInALine);
                return false;
            }

            // Check if the pieces are in the same horizontal word
            if (allInTheSameRow)
            {
                if (HorizontalWordIsWhole(placedPieces) == false)
                {
                    Game.Current.TurnErrors.Add(Constants.InvalidTurnReason.NotInALine);
                    return false;
                }
            }

            // Check if the pieces are in the same vertical word
            if (allInTheSameColumn)
            {
                if (VerticalWordIsWhole(placedPieces) == false)
                {
                    Game.Current.TurnErrors.Add(Constants.InvalidTurnReason.NotInALine);
                    return false;
                }
            }

            // Check if any of the pieces is next to an already filled slot or in the center
            if (WordIsAttachedToFilledSlot(placedPieces) == false)
            {
                Game.Current.TurnErrors.Add(Constants.InvalidTurnReason.NotAttached);
                return false;
            }

            return true;
        }

        public static int CalculatePoints(List<HandSlot> placedPieces = null)
        {
            placedPieces ??= HandController.HandSlots.Where(s => s.IsPlaced).ToList();

            var bonusPoints = 0;
            if (placedPieces.Count == HandController.HandSlots.Length)
            {
                bonusPoints += Constants.BonusPointsAllPiecesUsed;
            }

            var words = GetNewWords(placedPieces);
            var basePoints = words.Sum(w => w.Points);
            var multiplier = 1;
            /*foreach (var piece in placedPieces.Where(p => p.CurrentBoardSlot.Color == Constants.Colors.TileColors.x2))
            {
                multiplier *= 2;
            }
            foreach (var piece in placedPieces.Where(p => p.CurrentBoardSlot.Color == Constants.Colors.TileColors.x3))
            {
                multiplier *= 3;
            }*/
            return basePoints * multiplier + bonusPoints;
        }

        public static List<Word> GetNewWords(List<HandSlot> placedPieces = null)
        {
            placedPieces = placedPieces ?? HandController.HandSlots.Where(s => s.IsPlaced).ToList();

            var words = new List<Word>();
            if (placedPieces.Any() == false) return words;

            // Check if pieces are in the same horizontal line
            bool allInTheSameRow = !placedPieces.Select(p => p.CurrentBoardSlot.Position.Row)
                                                .Distinct()
                                                .Skip(1)
                                                .Any();
            // Check if pieces are in the same vertical line
            bool allInTheSameColumn = !placedPieces.Select(p => p.CurrentBoardSlot.Position.Column)
                                                   .Distinct()
                                                   .Skip(1)
                                                   .Any();

            if (allInTheSameRow) // Horizontal
            {
                var row = placedPieces[0].CurrentBoardSlot.Position.Row;
                var startColumn = placedPieces.Select(p => p.CurrentBoardSlot.Position.Column).OrderBy(c => c).First();

                words.Add(GetWordFromRow(row, startColumn));

                foreach (var piece in placedPieces)
                {
                    words.Add(GetWordFromColumn(piece.CurrentBoardSlot.Position.Column, row));
                }
            }
            else if (allInTheSameColumn) // Vertical
            {
                var column = placedPieces[0].CurrentBoardSlot.Position.Column;
                var startRow = placedPieces.Select(p => p.CurrentBoardSlot.Position.Row).OrderBy(r => r).First();

                words.Add(GetWordFromColumn(column, startRow));

                foreach (var piece in placedPieces)
                {
                    words.Add(GetWordFromRow(piece.CurrentBoardSlot.Position.Row, column));
                }
            }

            var result = words.Where(w => w.Text.Length >= Constants.MinimumWordLength).Validate().ToList();
            if (placedPieces.Count > 0 && result.Count == 0) Game.Current.TurnErrors.Add(Constants.InvalidTurnReason.NoWords);
            return result;
        }

        private static IEnumerable<Word> Validate(this IEnumerable<Word> words)
        {
            foreach (var word in words)
            {
                word.IsValid = true;
                if (word.Text.Length < Constants.MinimumWordLength)
                {
                    Game.Current.TurnErrors.Add(Constants.InvalidTurnReason.InvalidWord);
                    word.IsValid = false;
                    continue;
                }
                if (WordController.WordExists(word.Text) == false)
                {
                    Game.Current.TurnErrors.Add(Constants.InvalidTurnReason.InvalidWord);
                    word.IsValid = false;
                    continue;
                }
            }
            return words;
        }

        public static bool WordsAreValid(List<Word> words)
        {
            if (words == null || words.Count == 0 || words.Any(w => w.IsValid == false))
                return false;

            return true;
        }

        private static Word GetWordFromRow(int row, int startColumn)
        {
            List<BoardSlot> letterPositions = new List<BoardSlot>();

            var column = startColumn;
            while (column - 1 >= 0)
            {
                var boardSlot = Board.Board.Slots[row, column - 1];
                if (boardSlot.IsFilled || boardSlot.IsPending)
                {
                    column--;
                }
                else
                {
                    break;
                }
            }

            var horizontalWordSB = new StringBuilder();
            int points = 0;
            BoardSlot currentBoardSlot = Board.Board.Slots[row, column];
            do
            {
                horizontalWordSB.Append(currentBoardSlot.CurrentLetter);
                points += currentBoardSlot.Points;
                letterPositions.Add(currentBoardSlot);

                column++;
                if (column == Board.Board.Columns)
                    break;

                currentBoardSlot = Board.Board.Slots[row, column];
            }
            while (currentBoardSlot.IsFilled || currentBoardSlot.IsPending);

            return new Word
            {
                Text = horizontalWordSB.ToString().ToUpperInvariant(),
                Points = points,
                LetterPositions = letterPositions
            };
        }

        private static Word GetWordFromColumn(int column, int startRow)
        {
            List<BoardSlot> letterPositions = new List<BoardSlot>();

            var row = startRow;
            while (row - 1 >= 0)
            {
                var boardSlot = Board.Board.Slots[row - 1, column];
                if (boardSlot.IsFilled || boardSlot.IsPending)
                {
                    row--;
                }
                else
                {
                    break;
                }
            }

            var verticalWordSB = new StringBuilder();
            int points = 0;
            BoardSlot currentBoardSlot = Board.Board.Slots[row, column];
            do
            {
                verticalWordSB.Append(currentBoardSlot.CurrentLetter);
                points += currentBoardSlot.Points;
                letterPositions.Add(currentBoardSlot);

                row++;
                if (row == Board.Board.Rows)
                    break;

                currentBoardSlot = Board.Board.Slots[row, column];
            }
            while (currentBoardSlot.IsFilled || currentBoardSlot.IsPending);

            return new Word
            {
                Text = verticalWordSB.ToString().ToUpperInvariant(),
                Points = points,
                LetterPositions = letterPositions
            };
        }

        private static bool HorizontalWordIsWhole(List<HandSlot> placedPieces)
        {
            var row = placedPieces[0].CurrentBoardSlot.Position.Row;
            var columns = placedPieces.Select(p => p.CurrentBoardSlot.Position.Column).OrderBy(c => c);
            var startColumn = columns.First();
            var endColumn = columns.Last();
            for (int col = startColumn + 1; col < endColumn; col++)
            {
                var slot = Board.Board.Slots[row, col];
                if (slot.IsFilled == false && slot.IsPending == false)
                {
                    // There is a gap - word is invalid
                    return false;
                }
            }

            return true;
        }

        private static bool VerticalWordIsWhole(List<HandSlot> placedPieces)
        {
            var column = placedPieces[0].CurrentBoardSlot.Position.Column;
            var rows = placedPieces.Select(p => p.CurrentBoardSlot.Position.Row).OrderBy(r => r);
            var startRow = rows.First();
            var endRow = rows.Last();
            for (int row = startRow + 1; row < endRow; row++)
            {
                var slot = Board.Board.Slots[row, column];
                if (slot.IsFilled == false && slot.IsPending == false)
                {
                    // There is a gap - word is invalid
                    return false;
                }
            }

            return true;
        }

        private static bool WordIsAttachedToFilledSlot(List<HandSlot> placedPieces)
        {
            foreach (var placedPiece in placedPieces)
            {
                if (placedPiece.CurrentBoardSlot == Board.Board.CenterSlot) return true;

                var position = placedPiece.CurrentBoardSlot.Position;
                if ((position.Row - 1).Between(0, Board.Board.Rows) &&
                    position.Column.Between(0, Board.Board.Columns) &&
                    Board.Board.Slots[position.Row - 1, position.Column].IsFilled) return true;

                if ((position.Row + 1).Between(0, Board.Board.Rows) &&
                    position.Column.Between(0, Board.Board.Columns) &&
                    Board.Board.Slots[position.Row + 1, position.Column].IsFilled) return true;

                if (position.Row.Between(0, Board.Board.Rows) &&
                    (position.Column - 1).Between(0, Board.Board.Columns) &&
                    Board.Board.Slots[position.Row, position.Column - 1].IsFilled) return true;

                if (position.Row.Between(0, Board.Board.Rows) &&
                    (position.Column + 1).Between(0, Board.Board.Columns) &&
                    Board.Board.Slots[position.Row, position.Column + 1].IsFilled) return true;
            }

            return false;
        }

        public static void EndGame(Constants.GameEndReason reason)
        {
            SidebarController.ToggleMenu(true);

            if (reason != Constants.GameEndReason.Forced)
            {
                var players = Game.Current.Players.OrderByDescending(p => p.Score).ToList();

                using var dbConnection = new SqliteConnection("Data Source=" + Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Game.db"));
                dbConnection.Open();

                var command = dbConnection.CreateCommand();
                command.CommandText = "CREATE TABLE IF NOT EXISTS Scores(Player TEXT NOT NULL, Score INTEGER NOT NULL, Timestamp TEXT NOT NULL)";
                command.ExecuteNonQuery();

                command.CommandText = "BEGIN";
                command.ExecuteNonQuery();

                foreach (var player in players)
                {
                    var insertCommand = dbConnection.CreateCommand();
                    insertCommand.CommandText = "INSERT INTO Scores(Player, Score, Timestamp) VALUES(@Player, @Score, @Timestamp)";
                    insertCommand.Parameters.AddWithValue("@Player", player.Name);
                    insertCommand.Parameters.AddWithValue("@Score", player.Score);
                    insertCommand.Parameters.AddWithValue("@Timestamp", DateTime.Now.ToString(Constants.DatabaseDateTimeFormat));
                    insertCommand.ExecuteNonQuery();
                }

                command.CommandText = "END";
                command.ExecuteNonQuery();
                dbConnection.Close();

                SidebarController.RenderHighscores();

                var winners = players.Where(p => p.Score == players[0].Score).ToList();

                var winText = winners.Count switch
                {
                    1 => $"{winners[0].Name} печели!",
                    2 => $"Равенство между {winners[0].Name} и {winners[1].Name}!",
                    _ => $"Равенство: {string.Join(", ", winners.Select(w => w.Name))}!"
                };

                string message = $"{reason.GetDescription()}\n" +
                                 $"{winText}";
                MessageBox.Show(message, "Край на играта");
            }
        }
    }
}
