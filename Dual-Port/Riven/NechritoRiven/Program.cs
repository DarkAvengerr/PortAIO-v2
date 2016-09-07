#region

using System;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

using EloBuddy;
using LeagueSharp.Common;
namespace NechritoRiven
{
    public class Program
    {
        public static void OnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Riven")
            {
                Chat.Print("Could not load Riven");
                return;
            }
            Load.Load.LoadAssembly();
        }
    }
}