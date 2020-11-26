using Ionic.Zip;
using kSlovnik.Piece;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace kSlovnik.Game
{
    [Serializable]
    public class Game
    {
        [JsonIgnore]
        public static Game Current;

        [JsonInclude]
        public string Id;

        [JsonInclude]
        public List<Player.Player> Players = new List<Player.Player>();

        [JsonInclude]
        public int CurrentPlayerIndex = 0;

        [JsonIgnore]
        public Player.Player CurrentPlayer { get => Players[CurrentPlayerIndex]; }

        [JsonInclude]
        public int TurnScore = 0;

        [JsonInclude]
        public int TurnsWithoutPlacement = 0;

        #region Properties for Save/Load
        [JsonInclude]
        public Queue<char> DeckPieces { get => Deck.Pieces; set => Deck.Pieces = value; }

        [JsonInclude]
        public Board.BoardSlotInfo[] BoardSlots
        {
            get
            {
                var boardFlattened = new Board.BoardSlotInfo[Board.Board.Slots.Length];
                foreach (var boardSlot in Board.Board.Slots)
                {
                    boardFlattened[(int)boardSlot.Tag] = Board.BoardSlotInfo.FromBoardSlot(boardSlot);
                }
                return boardFlattened;
            }
            set
            {
                foreach (var boardSlotInfo in value)
                {
                    var index = boardSlotInfo.Tag;
                    Board.BoardSlotInfo.ApplyBoardInfoToSlot(boardSlotInfo, Board.Board.Slots[index / Board.Board.Columns, index % Board.Board.Columns]);
                }
            }
        }
        #endregion

        public static bool Save(bool autosave = true)
        {
            try
            {
                if (Directory.Exists(Constants.SavesFolder) == false)
                {
                    var dirInfo = Directory.CreateDirectory(Constants.SavesFolder);
                    dirInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                }

                if (autosave)
                {
                    var autosavePath = Path.Combine(Constants.SavesFolder, "Autosave");
                    if (Directory.Exists(autosavePath))
                    {
                        // TODO: Delete autosaves for oldest game if more than 10 games autosaved
                        //Directory.Delete(autosavePath, true); // Delete existing autosaves
                    }
                    Directory.CreateDirectory(autosavePath);
                }

                var folderPath = autosave ? Path.Combine(Constants.SavesFolder, "Autosave") : Constants.SavesFolder;
                var turnCount = Game.Current.Players.Sum(p => p.TurnsPlayed);
                var gameFileNameNoExt = autosave ? $"autosave_{Game.Current.Id}_{turnCount}" : $"{Game.Current.Id}_{turnCount}";
                var gameFileName = $"{gameFileNameNoExt}.game";
                var gameFilePath = Path.Combine(folderPath, gameFileName);

                if (File.Exists(gameFilePath))
                    File.Delete(gameFilePath);

                File.WriteAllText(gameFilePath, JsonSerializer.Serialize(Game.Current, options: new JsonSerializerOptions { WriteIndented = true }));
                Program.MainView.Invoke((MethodInvoker)delegate
                {
                    var thumbnailPath = Path.Combine(folderPath, gameFileNameNoExt + ".png");
                    Util.CaptureScreenshot(Program.MainView, thumbnailPath);
                    Util.CreatePackagedFile(Path.Combine(folderPath, gameFileNameNoExt + ".save"), gameFilePath, thumbnailPath);
                });
                return true;
            }
            catch (Exception e)
            {
                // TODO: Log and message
                return false;
            }
        }

        public static bool Load(string id, bool idIsFullPath = false)
        {
            try
            {
                if (Directory.Exists(Constants.SavesFolder) == false)
                    return false;

                if (idIsFullPath)
                {
                    Game.Current = LoadSaveFileGame(id);
                    return true;
                }

                var folderPath = Constants.SavesFolder;
                if (id == null)
                {
                    id = "autosave";
                    folderPath = Path.Combine(folderPath, "Autosave");

                    if (Directory.Exists(folderPath) == false)
                        return false;
                }

                // Get most recent save
                var autosavePaths = Directory.GetFiles(folderPath).Where(p => Path.GetFileName(p).StartsWith(id) && Path.GetFileName(p).EndsWith(".save")).ToList();
                if (autosavePaths.Any())
                {
                    var gameFilePath = autosavePaths.OrderByDescending(p => new FileInfo(p).LastWriteTime).First();
                    Game.Current = LoadSaveFileGame(gameFilePath);
                    return true;
                }
                else
                {
                    // No saves found
                    return false;
                }
            }
            catch (Exception e)
            {
                // TODO: Log and message
                return false;
            }
        }

        public static Image LoadSaveFileThumbnail(string packagedFileName)
        {
            using (ZipFile zip = ZipFile.Read(packagedFileName))
            {
                var thumbnail = zip.Entries.FirstOrDefault(f => f.FileName.EndsWith(".png"));
                using (MemoryStream fs = new MemoryStream())
                {
                    thumbnail.Extract(fs);
                    fs.Seek(0, SeekOrigin.Begin);
                    return Image.FromStream(fs);
                }
            }
        }

        public static Game LoadSaveFileGame(string packagedFileName)
        {
            using (ZipFile zip = ZipFile.Read(packagedFileName))
            {
                var thumbnail = zip.Entries.FirstOrDefault(f => f.FileName.EndsWith(".png"));

                var game = zip.Entries.FirstOrDefault(f => f.FileName.EndsWith(".game"));
                using (MemoryStream fs = new MemoryStream())
                {
                    game.Extract(fs);
                    fs.Seek(0, SeekOrigin.Begin);
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        var gameJson = sr.ReadToEnd();
                        return JsonSerializer.Deserialize<Game>(gameJson);
                    }
                }
            }
        }
    }
}
