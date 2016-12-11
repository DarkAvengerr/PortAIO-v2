using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace LeeSin_EloClimber
{
    internal class Program
    {
        public static void Main()
        {
            OnLoad(new EventArgs()); // Load Game
        }

        static void OnLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "LeeSin")
                return;

            LeeSin.Load();

            Chat.Print("<font color=\"#FF001E\">[Lee Sin] Elo Climber </font><font color=\"#FF980F\"> Loaded</font>");
        }
    }
}
