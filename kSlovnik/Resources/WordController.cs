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
        public static HashSet<string> AllWords = new HashSet<string>();
        public static Dictionary<int, HashSet<string>> WordsByLength = new Dictionary<int, HashSet<string>>();

        public static void LoadWords()
        {
            AllWords.Clear();
            //Words.AddRange(File.ReadAllText(Path.Combine(WordsFilePath, @"BaseWords.txt")).Split());
            //Words.AddRange(File.ReadAllText(Path.Combine(WordsFilePath, @"ExtraWords.txt")).Split());
            AllWords.UnionWith(File.ReadAllText(Path.Combine(WordsFilePath, @"DefaultForm.txt")).Split(','));
            AllWords.UnionWith(File.ReadAllText(Path.Combine(WordsFilePath, @"DerivativeForm.txt")).Split(','));

            // Filter words
            AllWords = AllWords.Select(w => w.ToUpperInvariant().Trim()) // All upper case with no whitespace on either side
                         .Where(w => w.Length >= Constants.MinimumWordLength && // Not shorter than the minimum range
                                     w.Length <= Math.Max(Board.Board.Rows, Board.Board.Columns) && // Not longer than what the board can hold
                                     w.All(c => c >= 'А' && c <= 'Я')) // Only Cyrillic letters
                         .Distinct() // Unique words
                         .ToHashSet();

            for (int i = Constants.MinimumWordLength; i <= Math.Max(Board.Board.Rows, Board.Board.Columns); i++)
            {
                WordsByLength.Add(i, new HashSet<string>());
                WordsByLength[i].UnionWith(AllWords.Where(w => w.Length == i));
            }
        }

        public static bool WordExists(string word)
        {
            return AllWords.Contains(word.ToUpperInvariant());
        }
    }
}
