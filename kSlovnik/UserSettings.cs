using kSlovnik.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace kSlovnik
{
    public static class UserSettings
    {
        public static bool SoundsOn = true;
        public static string LastAutosave = null;
        public static List<Player.Player> Players = new List<Player.Player>();

        public static void Save()
        {
            var userSettings = Util.GetFieldValues(typeof(UserSettings));
            var settingsFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "settings.json");

            if (File.Exists(settingsFilePath))
                File.Delete(settingsFilePath);

            File.WriteAllText(settingsFilePath, JsonSerializer.Serialize(userSettings, options: new JsonSerializerOptions { WriteIndented = true })); ;
        }

        public static void Load()
        {
            var settingsFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "settings.json");

            if (File.Exists(settingsFilePath))
            {
                var userSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(settingsFilePath));
                Util.SetFieldValues(typeof(UserSettings), userSettings);
            }
            
            if (Players.Count == 0)
            {
                Players.Add(new Player.Player(null, ImageController.LetterImagesActive['а'], null));
                Players.Add(new Player.Player("Компютър", ImageController.LetterImagesActive['б'], new AI.AI(AI.Difficulty.Medium)));
                Save();
            }
        }
    }
}
