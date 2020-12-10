using kSlovnik.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace kSlovnik.Resources
{
    public static class WordController
    {
        public const string BaseWordsFilePath = @"Resources\Words\Base\";
        public const string ProcessedWordsFilePath = @"Resources\Words\Processed\";

        public static readonly string BasePendingPath = Path.Combine(BaseWordsFilePath, @"Pending.txt");
        public static readonly string BaseInvalidLengthPath = Path.Combine(BaseWordsFilePath, @"InvalidLength.txt");
        public static readonly string BaseInvalidCharactersPath = Path.Combine(BaseWordsFilePath, @"InvalidCharacters.txt");
        public static readonly string BaseInvalidMiscPath = Path.Combine(BaseWordsFilePath, @"InvalidMisc.txt");
        public static readonly string BaseApprovedPath = Path.Combine(BaseWordsFilePath, @"Approved.txt");

        public static readonly string ProcessedPendingPath = Path.Combine(ProcessedWordsFilePath, @"Pending.dict");
        public static readonly string ProcessedInvalidLengthPath = Path.Combine(ProcessedWordsFilePath, @"InvalidLength.dict");
        public static readonly string ProcessedInvalidCharactersPath = Path.Combine(ProcessedWordsFilePath, @"InvalidCharacters.dict");
        public static readonly string ProcessedInvalidMiscPath = Path.Combine(ProcessedWordsFilePath, @"InvalidMisc.dict");
        public static readonly string ProcessedApprovedPath = Path.Combine(ProcessedWordsFilePath, @"Approved.dict");

        public static HashSet<DictionaryWord> PendingWords = new HashSet<DictionaryWord>();
        public static HashSet<DictionaryWord> InvalidLengthWords = new HashSet<DictionaryWord>();
        public static HashSet<DictionaryWord> InvalidCharactersWords = new HashSet<DictionaryWord>();
        public static HashSet<DictionaryWord> InvalidMiscWords = new HashSet<DictionaryWord>();
        public static HashSet<DictionaryWord> ApprovedWords = new HashSet<DictionaryWord>();
        public static Dictionary<int, HashSet<string>> WordsByLength = new Dictionary<int, HashSet<string>>();

        public static void LoadWords()
        {
            PendingWords.Clear();
            if (File.Exists(ProcessedPendingPath)) PendingWords.UnionWith(JsonSerializer.Deserialize<HashSet<DictionaryWord>>(File.ReadAllText(ProcessedPendingPath)));
            else if (File.Exists(BasePendingPath)) PendingWords.UnionWith(File.ReadAllText(BasePendingPath).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(w => new DictionaryWord { Root = w.Trim().ToUpperInvariant(), IsApproved = null }));

            InvalidLengthWords.Clear();
            if (File.Exists(ProcessedInvalidLengthPath)) InvalidLengthWords.UnionWith(JsonSerializer.Deserialize<HashSet<DictionaryWord>>(File.ReadAllText(ProcessedInvalidLengthPath)));
            else if (File.Exists(BaseInvalidLengthPath)) InvalidLengthWords.UnionWith(File.ReadAllText(BaseInvalidLengthPath).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(w => new DictionaryWord { Root = w.Trim().ToUpperInvariant(), IsApproved = false }));

            InvalidCharactersWords.Clear();
            if (File.Exists(ProcessedInvalidCharactersPath)) InvalidCharactersWords.UnionWith(JsonSerializer.Deserialize<HashSet<DictionaryWord>>(File.ReadAllText(ProcessedInvalidCharactersPath)));
            else if (File.Exists(BaseInvalidCharactersPath)) InvalidCharactersWords.UnionWith(File.ReadAllText(BaseInvalidCharactersPath).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(w => new DictionaryWord { Root = w.Trim().ToUpperInvariant(), IsApproved = false }));

            InvalidMiscWords.Clear();
            if (File.Exists(ProcessedInvalidMiscPath)) InvalidMiscWords.UnionWith(JsonSerializer.Deserialize<HashSet<DictionaryWord>>(File.ReadAllText(ProcessedInvalidMiscPath)));
            else if (File.Exists(BaseInvalidMiscPath)) InvalidMiscWords.UnionWith(File.ReadAllText(BaseInvalidMiscPath).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(w => new DictionaryWord { Root = w.Trim().ToUpperInvariant(), IsApproved = false }));

            ApprovedWords.Clear();
            if (File.Exists(ProcessedApprovedPath)) ApprovedWords.UnionWith(JsonSerializer.Deserialize<HashSet<DictionaryWord>>(File.ReadAllText(ProcessedApprovedPath)));
            else if (File.Exists(BaseApprovedPath)) ApprovedWords.UnionWith(File.ReadAllText(BaseApprovedPath).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(w => new DictionaryWord { Root = w.Trim().ToUpperInvariant(), IsApproved = true }));

            WordsByLength.Clear();
            for (int i = Constants.MinimumWordLength; i <= Math.Max(Board.Board.Rows, Board.Board.Columns); i++)
            {
                WordsByLength.Add(i, new HashSet<string>());
                WordsByLength[i].UnionWith(ApprovedWords.Where(w => w.FullWord.Length == i).Select(w => w.FullWord));
                WordsByLength[i].UnionWith(PendingWords.Where(w => w.FullWord.Length == i).Select(w => w.FullWord));
            }
        }

        public static void ProcessPendingWords()
        {
            LoadWords();

            var pendingInvalidLength = PendingWords.Where(word => word.FullWord.Length < Constants.MinimumWordLength || word.FullWord.Length > Math.Max(Board.Board.Rows, Board.Board.Columns));
            var pendingInvalidCharacters = PendingWords.Where(word => word.FullWord.Any(c => c < 'А' || c > 'Я'));

            InvalidLengthWords.UnionWith(pendingInvalidLength);
            PendingWords.ExceptWith(InvalidLengthWords);

            InvalidCharactersWords.UnionWith(pendingInvalidCharacters);
            PendingWords.ExceptWith(InvalidCharactersWords);

            if (!Directory.Exists(ProcessedWordsFilePath))
                Directory.CreateDirectory(ProcessedWordsFilePath);

            File.WriteAllText(ProcessedApprovedPath, JsonSerializer.Serialize(ApprovedWords, new JsonSerializerOptions { WriteIndented = true }));
            File.WriteAllText(ProcessedInvalidMiscPath, JsonSerializer.Serialize(InvalidMiscWords, new JsonSerializerOptions { WriteIndented = true }));

            File.WriteAllText(ProcessedInvalidLengthPath, JsonSerializer.Serialize(InvalidLengthWords, new JsonSerializerOptions { WriteIndented = true }));
            File.WriteAllText(ProcessedInvalidCharactersPath, JsonSerializer.Serialize(InvalidCharactersWords, new JsonSerializerOptions { WriteIndented = true }));
            File.WriteAllText(ProcessedPendingPath, JsonSerializer.Serialize(PendingWords, new JsonSerializerOptions { WriteIndented = true }));

            LoadWords();
        }

        public static void AddWord(DictionaryWord word)
        {
            PendingWords.Add(word);
        }

        public static void Approve(this DictionaryWord word)
        {
            RemoveWord(word);
            ApprovedWords.Add(word);
            word.IsApproved = true;
        }

        public static void Reject(this DictionaryWord word)
        {
            RemoveWord(word);
            InvalidMiscWords.Add(word);
            word.IsApproved = false;
        }

        public static void SetAsPending(this DictionaryWord word)
        {
            RemoveWord(word);
            PendingWords.Add(word);
            word.IsApproved = null;
        }

        private static bool RemoveWord(DictionaryWord word)
        {
            bool removed = PendingWords.Remove(word);
            removed |= InvalidLengthWords.Remove(word);
            removed |= InvalidCharactersWords.Remove(word);
            removed |= InvalidMiscWords.Remove(word);
            removed |= ApprovedWords.Remove(word);
            return removed;
        }

        public static void SaveWords()
        {
            File.WriteAllText(ProcessedPendingPath, JsonSerializer.Serialize(PendingWords, new JsonSerializerOptions { WriteIndented = true }));
            File.WriteAllText(ProcessedInvalidLengthPath, JsonSerializer.Serialize(InvalidLengthWords, new JsonSerializerOptions { WriteIndented = true }));
            File.WriteAllText(ProcessedInvalidCharactersPath, JsonSerializer.Serialize(InvalidCharactersWords, new JsonSerializerOptions { WriteIndented = true }));
            File.WriteAllText(ProcessedInvalidMiscPath, JsonSerializer.Serialize(InvalidMiscWords, new JsonSerializerOptions { WriteIndented = true }));
            File.WriteAllText(ProcessedApprovedPath, JsonSerializer.Serialize(ApprovedWords, new JsonSerializerOptions { WriteIndented = true }));
        }

        public static bool WordExists(string word)
        {
            word = word.ToUpperInvariant();
            return WordsByLength[word.Length].Any(w => w.Equals(word));
        }
    }
}
