using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace kSlovnik.Game
{
    [Serializable]
    public class Game
    {
        public static Game Current;

        public List<Player.Player> Players = new List<Player.Player>();

        public int CurrentPlayerIndex = 0;

        public Player.Player CurrentPlayer { get => Players[CurrentPlayerIndex]; }

        public int TurnScore = 0;

        public int TurnsWithoutPlacement = 0;

        public static bool Save(string id)
        {
            throw new NotImplementedException();
            try
            {
                var settingsFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), $"{id}.game");

                if (File.Exists(settingsFilePath))
                    File.Delete(settingsFilePath);

                File.WriteAllText(settingsFilePath, JsonSerializer.Serialize(Game.Current, options: new JsonSerializerOptions { WriteIndented = true })); ;
                return true;
            }
            catch(Exception e)
            {
                // TODO: Log and message
                return false;
            }
        }

        public static bool Load(string id)
        {
            try
            {
                throw new NotImplementedException();
                var settingsFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), $"{id}.game");

                if (File.Exists(settingsFilePath))
                {
                    Game.Current = JsonSerializer.Deserialize<Game>(File.ReadAllText(settingsFilePath));
                    return true;
                }
                else
                {
                    MessageBox.Show("Играта не е намерена");
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
