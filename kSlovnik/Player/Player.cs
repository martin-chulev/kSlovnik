using System;
using System.Collections.Generic;
using System.Text;

namespace kSlovnik.Player
{
    public class Player
    {
        public string Name { get; set; }

        public Hand Hand { get; set; }

        public int Score { get; set; }

        public bool IsAI { get; set; }

        public Player(string name = null, bool isAI = false)
        {
            this.Name = name ?? "Играч " + (Game.Game.Current.Players.Count + 1);
            this.Hand = new Hand();
        }
    }
}
