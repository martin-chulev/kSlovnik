using kSlovnik.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

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

        public static void ProcessWordsFromFilesToDb()
        {
            LoadWordsFromFiles();
            SaveWordsInDb();
            LoadWordsFromDb();
        }

        public static void LoadWords()
        {
            LoadWordsFromDb();
        }

        public static void LoadWordsFromDb()
        {
            PendingWords.Clear();
            InvalidLengthWords.Clear();
            InvalidCharactersWords.Clear();
            InvalidMiscWords.Clear();
            ApprovedWords.Clear();
            WordsByLength.Clear();

            using var dbConnection = new SqliteConnection("Data Source=" + Path.Combine(Path.GetFullPath(ProcessedWordsFilePath), "Dictionary.db"));
            dbConnection.Open();
            var command = dbConnection.CreateCommand();

            command.CommandText = "CREATE TABLE IF NOT EXISTS Words (Prefix TEXT NOT NULL, Root TEXT NOT NULL, Suffix TEXT NOT NULL, Ending TEXT NOT NULL, DefiniteArticle TEXT NOT NULL, IsApproved INTEGER, " +
                                                       "PRIMARY KEY (Prefix, Root, Suffix, Ending, DefiniteArticle)) WITHOUT ROWID";
            command.ExecuteNonQuery();

            command.CommandText = "SELECT iif(Prefix IS NULL, \"\", Prefix), " +
                                         "iif(Root IS NULL, \"\", Root), " +
                                         "iif(Suffix IS NULL, \"\", Suffix), " +
                                         "iif(Ending IS NULL, \"\", Ending), " +
                                         "iif(DefiniteArticle IS NULL, \"\", DefiniteArticle), " +
                                         "IsApproved " +
                                         "FROM Words";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var word = new DictionaryWord();
                word.Prefixes = new List<string>(((string)reader[0]).Split('-', StringSplitOptions.RemoveEmptyEntries));
                word.Root = (string)reader[1];
                word.Suffixes = new List<string>(((string)reader[2]).Split('-', StringSplitOptions.RemoveEmptyEntries));
                word.Ending = (string)reader[3];
                word.DefiniteArticle = (string)reader[4];

                //MessageBox.Show(reader[5].ToString());
                if (reader[5] == DBNull.Value || (long?)reader[5] == null)
                {
                    PendingWords.Add(word);
                }
                else if ((long?)reader[5] == 1)
                {
                    ApprovedWords.Add(word);
                }
            }

            GroupWordsByLength();
        }

        public static void LoadWordsFromFiles()
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

            GroupWordsByLength();
        }

        public static void GroupWordsByLength()
        {
            WordsByLength.Clear();
            for (int i = Constants.MinimumWordLength; i <= Math.Max(Board.Board.Rows, Board.Board.Columns); i++)
            {
                WordsByLength.Add(i, new HashSet<string>());
                WordsByLength[i].UnionWith(ApprovedWords.Where(w => w.FullWord.Length == i).Select(w => w.FullWord));
                WordsByLength[i].UnionWith(PendingWords.Where(w => w.FullWord.Length == i).Select(w => w.FullWord));
            }
        }

        public static void GroupWordByLength(DictionaryWord word)
        {
            var fullWord = word.FullWord;
            if (fullWord.Length >= Constants.MinimumWordLength && fullWord.Length <= Math.Max(Board.Board.Rows, Board.Board.Columns))
            {
                WordsByLength[fullWord.Length].Add(fullWord);
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
            SaveWordsInDb();
        }

        public static void SaveWordsInDb()
        {
            using var dbConnection = new SqliteConnection("Data Source=" + Path.Combine(Path.GetFullPath(ProcessedWordsFilePath), "Dictionary.db"));
            dbConnection.Open();

            var command = dbConnection.CreateCommand();
            command.CommandText = "CREATE TABLE IF NOT EXISTS Words (Prefix TEXT NOT NULL, Root TEXT NOT NULL, Suffix TEXT NOT NULL, Ending TEXT NOT NULL, DefiniteArticle TEXT NOT NULL, IsApproved INTEGER, " +
                                                       "PRIMARY KEY (Prefix, Root, Suffix, Ending, DefiniteArticle)) WITHOUT ROWID";
            command.ExecuteNonQuery();

            command.CommandText = "BEGIN";
            command.ExecuteNonQuery();

            foreach (var word in PendingWords)
            {
                SaveWordInDb(dbConnection, word);
            }
            foreach (var word in ApprovedWords)
            {
                SaveWordInDb(dbConnection, word);
            }

            command.CommandText = "END";
            command.ExecuteNonQuery();
            dbConnection.Close();
        }

        public static void SaveWordInDb(DictionaryWord word)
        {
            using var dbConnection = new SqliteConnection("Data Source=" + Path.Combine(Path.GetFullPath(ProcessedWordsFilePath), "Dictionary.db"));
            dbConnection.Open();
            SaveWordInDb(dbConnection, word);
            dbConnection.Close();
        }

        public static void SaveWordInDb(SqliteConnection dbConnection, DictionaryWord word)
        {
            try
            {
                var command = dbConnection.CreateCommand();
                command.CommandText = "INSERT INTO Words(Prefix, Root, Suffix, Ending, DefiniteArticle, IsApproved) VALUES(@Prefix, @Root, @Suffix, @Ending, @DefiniteArticle, @IsApproved)";
                command.Parameters.AddWithValue("@Prefix", string.Join('-', word.Prefixes));
                command.Parameters.AddWithValue("@Root", word.Root);
                command.Parameters.AddWithValue("@Suffix", string.Join('-', word.Suffixes));
                command.Parameters.AddWithValue("@Ending", word.Ending);
                command.Parameters.AddWithValue("@DefiniteArticle", word.DefiniteArticle);

                if (word.IsApproved == null) command.Parameters.AddWithValue("@IsApproved", DBNull.Value);
                else if (word.IsApproved == false) command.Parameters.AddWithValue("@IsApproved", "0");
                else if (word.IsApproved == true) command.Parameters.AddWithValue("@IsApproved", "1");

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                if (e.Message.StartsWith("SQLite Error 19:"))
                {
                    MessageBox.Show($"Думата {word.FullWord} вече съществува.");
                }
                else
                {
                    MessageBox.Show($"Грешка при добавяне на думата {word.FullWord} - {e.Message} | {e.InnerException?.Message}");
                }
            }
        }

        public static void SaveWordsInFiles()
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
