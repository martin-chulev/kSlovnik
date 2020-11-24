using kSlovnik.Piece;
using System;
using System.Collections.Generic;
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
                        Directory.Delete(autosavePath, true); // Delete existing autosaves
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
                /*Program.MainView.Invoke((MethodInvoker)delegate
                {
                    Util.CaptureScreenshot(Program.MainView, gameFileNameNoExt + ".png");
                });*/
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
                    Game.Current = JsonSerializer.Deserialize<Game>(File.ReadAllText(id));
                    return true;
                }

                if (id == null) id = "autosave";

                // Get most recent save
                var autosavePaths = Directory.GetFiles(Constants.SavesFolder).Where(p => Path.GetFileName(p).StartsWith(id)).ToList();
                if (autosavePaths.Any())
                {
                    var gameFilePath = autosavePaths.OrderByDescending(p => new FileInfo(p).LastWriteTime).First();
                    Game.Current = JsonSerializer.Deserialize<Game>(File.ReadAllText(gameFilePath));
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
    }
}
