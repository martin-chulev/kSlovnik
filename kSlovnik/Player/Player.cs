using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace kSlovnik.Player
{
    public class Player
    {
        public string Name { get; set; }

        public Image Avatar { get; set; }

        public Hand Hand { get; set; }

        public int Score { get; set; }

        public int TurnsPlayed { get; set; }

        public AI.AI AI { get; set; }

        public bool IsAI { get => this.AI != null; }

        public bool IsInTurn { get => Game.Game.Current.CurrentPlayer == this; }

        public Player(string name = null, Image avatar = null, AI.AI ai = null)
        {
            this.Name = name ?? "Играч " + (Game.Game.Current.Players.Count + 1);
            this.Avatar = avatar;
            this.Hand = new Hand();
            this.Score = 0;
            this.TurnsPlayed = 0;
            this.AI = ai;
        }
    }
}
