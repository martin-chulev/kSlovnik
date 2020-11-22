using kSlovnik.Game;
using kSlovnik.Generic;
using kSlovnik.Player;
using kSlovnik.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kSlovnik.AI
{
    public class AI
    {
        public Difficulty Difficulty { get; set; }

        public AI(Difficulty difficulty = Difficulty.Medium)
        {
            this.Difficulty = difficulty;
        }

        public async void PlayTurn()
        {
            //MessageBox.Show("Computer turn");
            var availableWords = GetAvailableWords();

            availableWords = Difficulty switch
            {
                Difficulty.Easy => availableWords.OrderBy(w => w.Text.Length).ToList(),
                Difficulty.Medium => availableWords.Shuffle(),
                Difficulty.Hard => availableWords.OrderByDescending(w => w.Text.Length).ToList(),
                Difficulty.Best => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };

            FindPlayableWord(availableWords);
            await GameController.EndTurn();
        }

        private List<PotentialWord> GetAvailableWords()
        {
            var sw = new Stopwatch();
            sw.Start();
            var result = new List<PotentialWord>();

            // Get hand
            var hand = HandController.HandSlots.Where(s => s.IsFilled).Select(s => s.Letter).ToList();
            var handLetters = hand.Where(s => s != '~').Select(l => l.ToString().ToUpperInvariant()[0]).ToList();
            var handGreyCount = hand.Count - handLetters.Count;

            // if center piece not placed:
            // find a word from hand only


            // Get board
            var boardRows = new List<string>();
            for (int row = 0; row < Board.Board.Rows; row++)
            {
                string rowStr = string.Empty;
                for (int column = 0; column < Board.Board.Columns; column++)
                {
                    rowStr += Board.Board.Slots[row, column].CurrentLetter;
                }
                boardRows.Add(rowStr);
            }

            var boardColumns = new List<string>();
            for (int column = 0; column < Board.Board.Columns; column++)
            {
                string columnStr = string.Empty;
                for (int row = 0; row < Board.Board.Rows; row++)
                {
                    columnStr += Board.Board.Slots[row, column].CurrentLetter;
                }
                boardColumns.Add(columnStr);
            }

            // Get row patterns
            for (int row = 0; row < boardRows.Count; row++)
            {
                var boardRow = boardRows[row];

                var options = new List<PotentialWord>();
                for (int i = 0; i < boardRow.Length; i++)
                {
                    if (i > 0 && boardRow[i - 1] != '\0') // If there is a letter on the left
                        continue;

                    for (int j = i; j < boardRow.Length; j++)
                    {
                        if (j < (boardRow.Length - 1) && boardRow[j + 1] != '\0') // If there is a letter on the right
                            continue;

                        var option = boardRow.Substring(i, j - i + 1);

                        if (option.Length > 1 && option.Any(c => c == '\0') && option.Any(c => c != '\0') && option.Count(c => c == '\0') <= hand.Count) {
                            options.Add(new PotentialWord(option.ToUpperInvariant(), row, i, isVertical: false));
                        }
                    }
                }

                var letterPattern = handGreyCount > 0 ? "." : $"[{string.Join(null, handLetters)}]";
                foreach (var potentialWord in options)
                {
                    var option = potentialWord.Text;
                    var pattern = $"^{option.Replace("\0", letterPattern).ToUpperInvariant()}$";
                    var matches = WordController.WordsByLength[option.Length].Where(word => Regex.IsMatch(word, pattern));
                    result.AddRange(GetFilteredMatches(potentialWord, matches, handLetters, handGreyCount));
                }
            }

            // Get column patterns
            for (int col = 0; col < boardColumns.Count; col++)
            {
                var boardColumn = boardColumns[col];

                var options = new List<PotentialWord>();
                for (int i = 0; i < boardColumn.Length; i++)
                {
                    if (i > 0 && boardColumn[i - 1] != '\0') // If there is a letter above
                        continue;

                    for (int j = i; j < boardColumn.Length; j++)
                    {
                        if (j < (boardColumn.Length - 1) && boardColumn[j + 1] != '\0') // If there is a letter below
                            continue;

                        var option = boardColumn.Substring(i, j - i + 1);

                        if (option.Length > 1 && option.Any(c => c == '\0') && option.Any(c => c != '\0') && option.Count(c => c == '\0') <= hand.Count)
                        {
                            options.Add(new PotentialWord(option.ToUpperInvariant(), i, col, isVertical: true));
                        }
                    }
                }

                var letterPattern = handGreyCount > 0 ? "." : $"[{string.Join(null, handLetters)}]";
                foreach (var potentialWord in options)
                {
                    var option = potentialWord.Text;
                    var pattern = $"^{option.Replace("\0", letterPattern).ToUpperInvariant()}$";
                    var matches = WordController.WordsByLength[option.Length].Where(word => Regex.IsMatch(word, pattern));
                    result.AddRange(GetFilteredMatches(potentialWord, matches, handLetters, handGreyCount));
                }
            }

            // if center piece placed:
            // get each row and column from board "  б а          "
            // split into different options (with at least 1 filled and at least 1 empty)
            // regex match dictionary for each pattern
            // filter results by subtracting letters from hand and checking if <= number of grey pieces in hand
            // remember start index of pattern on board in case a word is chosen for it

            /*var patternBuilder = new StringBuilder();
            patternBuilder.Append("^");
            patternBuilder.Append("")
            patternBuilder.Append("$")

            string regexPattern = @"^(?!.*o.*o)(?!.*a.*a)(?!.*e.*e.*e)(?!.*s.*s)d[oaes]{2}r[oaes]{0,3}$";
            Regex regex = new Regex(regexPattern);*/
            //WordController.Words.Where(w => )
            sw.Stop();
            //MessageBox.Show($"Found {result.Count} words\nin {sw.Elapsed}");
            return result;
        }

        private bool FindPlayableWord(List<PotentialWord> availableWords)
        {
            for (int i = 0; i < availableWords.Count; i++)
            {
                PlacePieces(availableWords[i], false);

                if (GameController.WordsAreValid(GameController.GetNewWords()) == true)
                {
                    HandController.ReturnAllToHand(changeVisualPosition: false);
                    PlacePieces(availableWords[i], true);
                    return true;
                }
                else
                {
                    HandController.ReturnAllToHand(changeVisualPosition: false);
                }
            }

            return false;
        }

        private void PlacePieces(PotentialWord potentialWord, bool changeVisualPosition)
        {
            var word = potentialWord.Text.ToLowerInvariant();
            for (int j = 0; j < word.Length; j++)
            {
                if (potentialWord.IsVertical)
                {
                    if (Board.Board.Slots[potentialWord.StartRow + j, potentialWord.StartColumn].IsFilled == true)
                        continue;
                }
                else
                {
                    if (Board.Board.Slots[potentialWord.StartRow, potentialWord.StartColumn + j].IsFilled == true)
                        continue;
                }

                var handSlot = HandController.HandSlots.FirstOrDefault(s => s.IsPlaced == false && s.Letter == word[j]);
                if (handSlot == null)
                {
                    handSlot = HandController.HandSlots.FirstOrDefault(s => s.IsPlaced == false && s.Letter == '~');
                    handSlot.SetPiece(word[j], false);
                }

                if (potentialWord.IsVertical)
                {
                    handSlot.PlaceOnBoard(potentialWord.StartRow + j, potentialWord.StartColumn, changeVisualPosition);
                }
                else
                {
                    handSlot.PlaceOnBoard(potentialWord.StartRow, potentialWord.StartColumn + j, changeVisualPosition);
                }

                if (changeVisualPosition)
                {
                    handSlot.Invoke((MethodInvoker)delegate { handSlot.Refresh(); });
                    SoundController.Click.Play();
                    Thread.Sleep(250);
                }
            }
        }

        private IEnumerable<PotentialWord> GetFilteredMatches(PotentialWord potentialWord, IEnumerable<string> matches, List<char> handLetters, int handGreyCount)
        {
            return matches.Where(w =>
            {
                var remainingGreys = handGreyCount;
                var remainingHandLetters = new List<char>(handLetters);
                for (int i = 0; i < w.Length; i++)
                {
                    if (w[i] == potentialWord.Text[i]) continue; // If letter is on board

                    if (remainingHandLetters.Contains(w[i])) // If letter is in hand (non-grey)
                    {
                        remainingHandLetters.Remove(w[i]);
                        continue;
                    }

                    if (remainingGreys > 0) // If there is a grey in hand to substitute letter
                    {
                        remainingGreys--;
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }).Select(m => new PotentialWord(m, potentialWord.StartRow, potentialWord.StartColumn, potentialWord.IsVertical));
        }
    }

    public enum Difficulty
    {
        [Description("Избира къси думи")]
        Easy,

        [Description("Избира среднодълги думи")]
        Medium,

        [Description("Избира дълги думи")]
        Hard,

        [Description("Избира думите, които носят най-много точки")]
        Best
    }
}
