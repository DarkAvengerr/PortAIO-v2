#region copyright

// Copyright (c) KyonLeague 2016
// If you want to copy parts of the code, please inform the author and give appropiate credits
// File: Program.cs
// Author: KyonLeague
// Contact: "cryz3rx" on Skype 

#endregion

#region usage

using System;
using LeagueSharp.SDK;
using RAREZyra.ChampionModes;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace RAREZyra
{
    internal class Program
    {

        public static void Main()
        {
            Bootstrap.Init();
            Events_OnLoad();
        }

        private static void Events_OnLoad()
        {
            Utilities.Player = GameObjects.Player;
            Utilities.InitMenu();
            Utilities.UpdateCheck();

            if (Utilities.Player.CharData.BaseSkinName == "Zyra")
            {
                var champion = new Zyra();
                champion.Init();
                Utilities.PrintChat("Zyra Initialized.");
            }
            
        }
    }
}