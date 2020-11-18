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
        public static List<string> Words = new List<string>();

        public static void LoadWords()
        {
            Words.Clear();
            //Words.AddRange(File.ReadAllText(Path.Combine(WordsFilePath, @"BaseWords.txt")).Split());
            //Words.AddRange(File.ReadAllText(Path.Combine(WordsFilePath, @"ExtraWords.txt")).Split());
            Words.AddRange(File.ReadAllText(Path.Combine(WordsFilePath, @"DefaultForm.txt")).Split(','));
            Words.AddRange(File.ReadAllText(Path.Combine(WordsFilePath, @"DerivativeForm.txt")).Split(','));

            // Filter words
            Words = Words.Select(w => w.ToUpperInvariant().Trim()) // All upper case with no whitespace on either side
                         .Where(w => w.Length >= Constants.MinimumWordLength && // Not shorter than the minimum range
                                     w.Length <= Math.Max(Board.Board.Rows, Board.Board.Columns) && // Not longer than what the board can hold
                                     w.All(c => c >= 'А' && c <= 'Я')) // Only Cyrillic letters
                         .Distinct() // Unique words
                         .ToList();
        }

        public static bool WordExists(string word)
        {
            // TODO: Implement proper search
            return Words.Contains(word.ToUpperInvariant());
        }

        public static void OpenDictionary()
        {
            // TODO: Create dict window with ability to add
        }
    }
}
