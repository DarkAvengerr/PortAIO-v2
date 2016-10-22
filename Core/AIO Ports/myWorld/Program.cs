using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using myWorld.Library.SimpleEvade;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld
{
    public enum CastMode
    {
        Combo,
        Harass,
        Killsteal,
        Farm,
    }
    class Program
    {
        public static Initialization MainEvade = new Initialization();

        public static Menu MainMenu { get; set; }

        public static Orbwalking.Orbwalker MainOrbwalker;

        public static bool SystemEvadeStat = false;
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            MainMenu = new Menu("myWord", "myWord", true);

            MainEvade.Init(MainMenu.SubMenu("Evade"));

            var General = new Menu("General", "General");

            var Orb = new Menu("Orbwalker", "Orbwalker");
            MainOrbwalker = new Orbwalking.Orbwalker(Orb);
            General.AddSubMenu(Orb);
            //var TS = new Menu("TargetSelector", "TargetSelector");
            //TargetSelector.AddToMenu(TS);
            //General.AddSubMenu(TS);

            ChampLoad();

            MainMenu.AddSubMenu(General);
            MainMenu.AddToMainMenu(); //?

            Game.OnUpdate += Game_OnUpdate;
        }

        static void ChampLoad()
        {
            switch(ObjectManager.Player.ChampionName)
            {
                case "Ezreal":
                    var Ezreal = new Champion.Ezreal.Ezreal();
                    break;
                case "Jinx":
                    var Jinx = new Champion.Jinx.Jinx();
                    break;
                case "Draven":
                    var Draven = new Champion.Draven.Draven();
                    break;
                default:
                    break;
            }
        }

        static void Game_OnUpdate(EventArgs args)
        {
            //throw new NotImplementedException();
            if (MainEvade.IsEvading())
            {
                if(!SystemEvadeStat)
                {
                    MainOrbwalker.SetMovement(false);
                    SystemEvadeStat = true;
                }
            }
            else
            {
                if(SystemEvadeStat)
                {
                    MainOrbwalker.SetMovement(true);
                    SystemEvadeStat = false;
                }
            }
        }

        static void Game_OnStart()
        {
            //throw new NotImplementedException();
            
        }

        public Program GetMain()
        {
            return this;
        }

        public Orbwalking.Orbwalker GetOrbwalker()
        {
            return MainOrbwalker;
        }
    }
}
