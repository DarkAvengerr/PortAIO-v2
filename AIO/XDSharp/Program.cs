using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace XDSharp
{
    class Program
    {

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }


#region OnGameLoad
        private static void OnGameLoad(EventArgs args)
        {
            try
            {
                switch (Player.ChampionName)
                {/*
                    case "Cassiopeia":
                        new XDSharp.Champions.Cassiopeia.Main().OGLoad();
                        break;
                    case "LeeSin":
                        new XDSharp.Champions.LeeSin.Main().OGLoad();
                        break;
                    case "Blitzcrank":
                        new XDSharp.Champions.Blitzcrank.Main().OGLoad();
                        break;
                    case "Ekko":
                        new XDSharp.Champions.Ekko.Main().OGLoad();
                        break;*/
                    case "Karthus":
                        new XDSharp.Champions.Karthus.Main().OGLoad();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
#endregion
    }
}
