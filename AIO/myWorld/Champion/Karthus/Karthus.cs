using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using myWorld.Library.MenuWarpper;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld.Champion.Karthus
{
    class Karthus
    {
        static Menu menu;
        static Spell Q, W, E;
        static Program Main;

        public Karthus(Program program)
        {
            //Main = program;
            //menu = Main.GetMenu();

            //Q = new Spell(SpellSlot.Q, 875);
            //Q.SetSkillshot(1.1f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            //W = new Spell(SpellSlot.W, 1000);
            //W.SetSkillshot(0.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotLine);

            //Menu ComboMenu = new Menu("Combo", "Combo");
            //ComboMenu.AddBool("Config.Combo.UseQ", "Use Q");
            //ComboMenu.AddBool("Config.Combo.UseW", "Use W");
            //ComboMenu.AddBool("Config.Combo.UseE", "Use E");
            //menu.AddSubMenu(ComboMenu);

            //Menu HarassMenu = new Menu("Harass", "Harass");
            //HarassMenu.AddBool("Config.Harass.UseQ", "Use Q");
            //HarassMenu.AddBool("Config.Harass.UseW", "Use W");
            //menu.AddSubMenu(HarassMenu);

            //Menu FarmMenu = new Menu("Farm", "Farm");
            //FarmMenu.AddBool("Config.Farm.UseQ", "Use Q");
            //FarmMenu.AddBool("Config.Farm.UseE", "Use E");
            //FarmMenu.AddSlice("Config.Farm.mper");
            //menu.AddSubMenu(FarmMenu);

            //Menu LineMenu = new Menu("Line", "Line");
            //LineMenu.AddBool("Config.Line.UseQ", "Use Q");
            //LineMenu.AddBool("Config.Line.UseE", "Use E");
            //LineMenu.AddSlice("Config.Line.mer");
            //menu.AddSubMenu(LineMenu);

            //Menu JungleMenu = new Menu("Jungle", "Jungle");
            //JungleMenu.AddBool("Config.Jungle.UseQ", "Use Q");
            //JungleMenu.AddBool("Config.Jungle.UseE", "Use E");
            //JungleMenu.AddSlice("Config.Jungle.mper");
            //menu.AddSubMenu(JungleMenu);

            //Menu Misc = new Menu("Misc", "Misc");
            //Misc.AddBool("Config.Misc.AutoE", "Auto E off");
            //Misc.AddBool



        }
    }
}
