using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace kSlovnik.Resources
{
    public static class WordController
    {
        public const string WordsFilePath = @"Resources\Words";
        public static List<string> AllWords = new List<string>();
        public static Dictionary<int, List<string>> WordsByLength = new Dictionary<int, List<string>>();

        public static void LoadWords()
        {
            AllWords.Clear();
            //Words.AddRange(File.ReadAllText(Path.Combine(WordsFilePath, @"BaseWords.txt")).Split());
            //Words.AddRange(File.ReadAllText(Path.Combine(WordsFilePath, @"ExtraWords.txt")).Split());
            AllWords.AddRange(File.ReadAllText(Path.Combine(WordsFilePath, @"DefaultForm.txt")).Split(','));
            AllWords.AddRange(File.ReadAllText(Path.Combine(WordsFilePath, @"DerivativeForm.txt")).Split(','));

            // Filter words
            AllWords = AllWords.Select(w => w.ToUpperInvariant().Trim()) // All upper case with no whitespace on either side
                         .Where(w => w.Length >= Constants.MinimumWordLength && // Not shorter than the minimum range
                                     w.Length <= Math.Max(Board.Board.Rows, Board.Board.Columns) && // Not longer than what the board can hold
                                     w.All(c => c >= 'А' && c <= 'Я')) // Only Cyrillic letters
                         .Distinct() // Unique words
                         .ToList();

            for (int i = Constants.MinimumWordLength; i <= Math.Max(Board.Board.Rows, Board.Board.Columns); i++)
            {
                WordsByLength.Add(i, new List<string>());
                WordsByLength[i].AddRange(AllWords.Where(w => w.Length == i));
            }
        }

        public static bool WordExists(string word)
        {
            // TODO: Implement proper search
            return AllWords.Contains(word.ToUpperInvariant());
        }

        public static void OpenDictionary()
        {
            // TODO: Create dict window with ability to add
        }
    }
}
