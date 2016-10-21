using System;
using SAutoCarry.Champions;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace SAutoCarry
{
    class Program
    {
        public static SCommon.PluginBase.Champion Champion; 

        public static void Game_OnGameLoad()
        {
            switch(ObjectManager.Player.ChampionName)
            {
                case "Vayne":
                    Champion = new Vayne();
                    break;

                case "Azir":
                    Champion = new Azir();
                    break;

                case "Rengar":
                    Champion = new Rengar();
                    break;

                case "Lucian":
                    Champion = new Lucian();
                    break;

                case "Riven":
                    Champion = new Riven();
                    break;

                case "Veigar":
                    Champion = new Veigar();
                    break;

                case "Pantheon":
                    Champion = new SAutoCarry.Champions.Pantheon();
                    break;

                case "Shyvana":
                    Champion = new Shyvana();
                    break;

                case "TwistedFate":
                    Champion = new SAutoCarry.Champions.TwistedFate();
                    break;

                case "Viktor":
                    Champion = new SAutoCarry.Champions.Viktor();
                    break;

                case "Twitch":
                    Champion = new Twitch();
                    break;

                case "Jax":
                    Champion = new Jax();
                    break;

                case "MasterYi":
                    Champion = new MasterYi();
                    break;

                case "Orianna":
                    Champion = new SAutoCarry.Champions.Orianna();
                    break;

                case "Blitzcrank":
                    Champion = new SAutoCarry.Champions.Blitzcrank();
                    break;

                case "Corki":
                    Champion = new Corki();
                    break;

                case "DrMundo":
                    Champion = new DrMundo();
                    break;

                case "Darius":
                    Champion = new Darius();
                    break;

                case "MissFortune":
                    Champion = new MissFortune();
                    break;

                case "Cassiopeia":
                    Champion = new Cassiopeia();
                    break;

                case "Jhin":
                    Champion = new Jhin();
                    break;
            }

            if (!Game.Version.StartsWith("6.8"))
                Chat.Print("Wrong game version");
        }
    }
}
