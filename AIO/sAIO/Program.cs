using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using sAIO.Champions;
using sAIO.Core;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace sAIO
{    
    public class Program
    {
        public static AIHeroClient player { get { return ObjectManager.Player; } }
        public static Orbwalking.Orbwalker orbwalker;
        public static Spell Q, Q2, W, W2, E, E2, R;
        public static Items.Item Tiamat, Hydra;
        public static Menu menu;
        public static SpellSlot Ignite;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            //string[] supportedChampions={"Blitzcank","Cassiopeia","Darius","Katarina",""}

            menu = new Menu("sAIO: " + player.ChampionName, player.ChampionName, true);
            Menu OrbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            orbwalker = new Orbwalking.Orbwalker(OrbwalkerMenu);
            menu.AddSubMenu(OrbwalkerMenu);
            Menu TargetSelectorMenu = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            menu.AddSubMenu(TargetSelectorMenu);

            switch(player.ChampionName)
            {
                case "Katarina":
                    new Katarina();
                    break;

                case "Renekton":
                    new Renekton();
                    break;

                case "Ryze":
                    new Ryze();
                    break;

                case "Cassiopeia":
                    new Cassiopeia();
                    break;

                case "Nasus":
                    new NasusA();
                    break;

                case "Darius":
                    new Darius();
                    break;

                case "Leona":
                    new Leona();
                    break;

                case "Talon":
                    new Talon();
                    break;

                case "Blitzcrank":
                    new BlitzcrankA();
                    break;

                case "Akali":
                    new AkaliA();
                    break;
            }

        }

    }
}
