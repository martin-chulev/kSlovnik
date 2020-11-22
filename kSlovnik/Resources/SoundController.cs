using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace kSlovnik.Resources
{
    public static class SoundController
    {
        public const string SoundsFolderPath = @"Resources\Sounds";

        public static GameSoundPlayer Click;

        public static void LoadSounds()
        {
            Click = new GameSoundPlayer(Path.Combine(SoundsFolderPath, @"click.wav"));
        }

        public class GameSoundPlayer
        {
            private System.Media.SoundPlayer sound;

            public GameSoundPlayer(string soundPath)
            {
                this.sound = new System.Media.SoundPlayer(soundPath);
            }

            public void Play()
            {
                if (Constants.UserSettings.SoundsOn)
                {
                    try
                    {
                        sound.Play();
                    }
                    catch
                    {
                        // TODO: Log
                    }
                }
            }
        }
    }
}
