using kSlovnik.Board;
using kSlovnik.Generic;
using kSlovnik.Piece;
using kSlovnik.Player;
using kSlovnik.Resources;
using kSlovnik.Sidebar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kSlovnik.Game
{
    public static class GameController
    {
        public static void NewGame()
        {
            Game.Current = new Game();

            Game.Current.Players.Add(new Player.Player("Easy", ImageController.LetterImagesActive['а'], new AI.AI(AI.Difficulty.Easy)));
            Game.Current.Players.Add(new Player.Player("Medium", ImageController.LetterImagesActive['б'], new AI.AI(AI.Difficulty.Medium)));
            Game.Current.Players.Add(new Player.Player("Hard", ImageController.LetterImagesActive['в'], new AI.AI(AI.Difficulty.Hard)));
            Game.Current.CurrentPlayerIndex = 0;
            SidebarController.RenderTurnPlayerLabel();

            DeckController.LoadDeck();
            HandController.LoadHand(Game.Current.CurrentPlayer);
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
                    return false;

                Game.Current.TurnsWithoutPlacement = 0;
                Game.Current.CurrentPlayer.TurnsPlayed++;
                Game.Current.CurrentPlayer.Score += CalculatePoints(placedPieces);
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

        private static void StartNextTurn()
        {
            Game.Current.CurrentPlayerIndex++;
            if (Game.Current.CurrentPlayerIndex >= Game.Current.Players.Count)
            {
                Game.Current.CurrentPlayerIndex = 0;
            }

            SidebarController.RenderTurnPlayerLabel();
            SidebarController.RenderScoreboard();
            HandController.LoadHand(Game.Current.CurrentPlayer);
            if (Game.Current.CurrentPlayer.IsAI)
            {
                Game.Current.CurrentPlayer.AI.PlayTurn();
            }
        }

        public static bool PiecePlacementIsValid(List<HandSlot> placedPieces = null)
        {
            placedPieces = placedPieces ?? HandController.HandSlots.Where(s => s.IsPlaced).ToList();

            // If no pieces were placed - nothing to check
            if (placedPieces.Any() == false) return true;

            // Check if there is a piece in the center slot
            var centerSlot = Board.Board.CenterSlot;
            if (centerSlot.IsPending == false && centerSlot.IsFilled == false)
                return false;

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
                return false;

            // Check if the pieces are in the same horizontal word
            if (allInTheSameRow)
                if (HorizontalWordIsWhole(placedPieces) == false)
                    return false;

            // Check if the pieces are in the same vertical word
            if (allInTheSameColumn)
                if (VerticalWordIsWhole(placedPieces) == false)
                    return false;

            // Check if any of the pieces is next to an already filled slot or in the center
            if (WordIsAttachedToFilledSlot(placedPieces) == false)
                return false;

            return true;
        }

        public static int CalculatePoints(List<HandSlot> placedPieces = null)
        {
            placedPieces = placedPieces ?? HandController.HandSlots.Where(s => s.IsPlaced).ToList();

            var bonusPoints = 0;
            if (placedPieces.Count == HandController.HandSlots.Length)
            {
                bonusPoints += Constants.BonusPointsAllPiecesUsed;
            }

            var words = GetNewWords(placedPieces);
            var basePoints = words.Sum(w => w.Points);
            var multiplier = 1;
            foreach (var piece in placedPieces.Where(p => p.CurrentBoardSlot.Color == Constants.Colors.TileColors.x2))
            {
                multiplier *= 2;
            }
            foreach (var piece in placedPieces.Where(p => p.CurrentBoardSlot.Color == Constants.Colors.TileColors.x3))
            {
                multiplier *= 3;
            }
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

            return words.Where(w => w.Text.Length >= Constants.MinimumWordLength).Validate().ToList();
        }

        private static IEnumerable<Word> Validate(this IEnumerable<Word> words)
        {
            foreach (var word in words)
            {
                word.IsValid = true;
                if (word.Text.Length < Constants.MinimumWordLength)
                {
                    word.IsValid = false;
                    continue;
                }
                if (WordController.WordExists(word.Text) == false)
                {
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

                column++;
                if (column == Board.Board.Columns)
                    break;

                currentBoardSlot = Board.Board.Slots[row, column];
            }
            while (currentBoardSlot.IsFilled || currentBoardSlot.IsPending);

            return new Word
            {
                Text = horizontalWordSB.ToString().ToUpperInvariant(),
                Points = points
            };
        }

        private static Word GetWordFromColumn(int column, int startRow)
        {
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

                row++;
                if (row == Board.Board.Rows)
                    break;

                currentBoardSlot = Board.Board.Slots[row, column];
            }
            while (currentBoardSlot.IsFilled || currentBoardSlot.IsPending);

            return new Word
            {
                Text = verticalWordSB.ToString().ToUpperInvariant(),
                Points = points
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
            MessageBox.Show("Game over");
        }
    }
}
