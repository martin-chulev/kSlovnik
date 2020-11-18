using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace kSlovnik.Resources
{
    public static class SoundController
    {
        public const string SoundsFolderPath = @"Resources\Sounds";

        public static System.Media.SoundPlayer Click;

        public static void LoadSounds()
        {
            Click = new System.Media.SoundPlayer(Path.Combine(SoundsFolderPath, @"click.wav"));
        }
    }
}
