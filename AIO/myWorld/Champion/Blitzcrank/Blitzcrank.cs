using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using myWorld.Library.MenuWarpper;
using myWorld.Library.SimplePrediction;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld.Champion.Blitzcrank
{
    class Blitzcrank
    {
        static Spell Q;
        static Menu menu;
        public Blitzcrank()
        {
            menu = Program.MainMenu;

            List<string> hitChances = new List<string>();
            foreach (HitChance value in Enum.GetValues(typeof(HitChance)))
            {
                hitChances.Add(value.ToString());
            }

            Q = new Spell(SpellSlot.Q, 1050);
            Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);

            Menu Config = new Menu("BlitzCrank", "BlitzCrank");

            Menu HitChanceMenu = new Menu("HitChance", "HitChance");

            Menu ComboHitChaceMenu = new Menu("Combo", "Combo");
            ComboHitChaceMenu.AddList("HitChance.Combo.Q", "Q HitChance", new StringList(hitChances.ToArray(), 5));
            HitChanceMenu.AddSubMenu(ComboHitChaceMenu);

            Menu HarassHitChaceMenu = new Menu("Harass", "Harass");
            HarassHitChaceMenu.AddList("HitChance.Harass.Q", "Q HitChance", new StringList(hitChances.ToArray(), 4));
            HitChanceMenu.AddSubMenu(HarassHitChaceMenu);

            Config.AddSubMenu(HitChanceMenu);
        }
    }
}
