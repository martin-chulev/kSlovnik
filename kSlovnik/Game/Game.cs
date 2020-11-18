using System;
using System.Collections.Generic;
using System.Text;

namespace kSlovnik.Game
{
    public class Game
    {
        public static Game Current;

        public List<Player.Player> Players = new List<Player.Player>();

        public int CurrentPlayerIndex = 0;

        public Player.Player CurrentPlayer { get => Players[CurrentPlayerIndex]; }

        public int TurnScore = 0;

        public int TurnsWithoutPlacement = 0;
    }
}
