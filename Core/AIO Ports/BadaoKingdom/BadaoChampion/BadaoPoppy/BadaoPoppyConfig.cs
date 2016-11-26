using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoPoppy
{
    public static class BadaoPoppyConfig
    {
        public static Menu config;
        public static void BadaoActivate()
        {
            // spells init
            BadaoMainVariables.Q = new Spell(SpellSlot.Q, 430);
            BadaoMainVariables.Q.SetSkillshot(0.5f, 100, float.MaxValue, false, SkillshotType.SkillshotLine);
            BadaoMainVariables.W = new Spell(SpellSlot.W, 375);
            BadaoMainVariables.E = new Spell(SpellSlot.E, 525);
            BadaoMainVariables.E.SetTargetted(0, float.MaxValue);
            BadaoMainVariables.R = new Spell(SpellSlot.R, 1200);
            BadaoMainVariables.R.SetCharged(495, 1200, 1.5f);

            // main menu
            config = new Menu("BadaoKingdom " + ObjectManager.Player.ChampionName, ObjectManager.Player.ChampionName, true);
            config.SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.YellowGreen);

            // orbwalker menu
            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            BadaoMainVariables.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            config.AddSubMenu(orbwalkerMenu);

            // TS
            Menu ts = config.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);

            // Combo
            Menu Combo = config.AddSubMenu(new Menu("Combo", "Combo"));
            BadaoPoppyVariables.ComboQ = Combo.AddItem(new MenuItem("ComboQ", "Q")).SetValue(true);
            BadaoPoppyVariables.ComboW = Combo.AddItem(new MenuItem("ComboW", "W to gapclose")).SetValue(true);
            BadaoPoppyVariables.ComboE = Combo.AddItem(new MenuItem("ComboE", "E to gapclose")).SetValue(true);
            foreach (var hero in HeroManager.Enemies)
            {
                Combo.AddItem(new MenuItem("ComboE" + hero.NetworkId, "E stun " + hero.ChampionName + " (" + hero.Name + ")")).SetValue(true);
            }
            BadaoPoppyVariables.ComboRKillable = Combo.AddItem(new MenuItem("ComboRKillable", "R Killable")).SetValue(true);

            // Harass
            Menu Harass = config.AddSubMenu(new Menu("Harass", "Harass"));
            BadaoPoppyVariables.HarassQ = Harass.AddItem(new MenuItem("HarassQ", "Q")).SetValue(true);

            //JungleClear
            Menu Jungle = config.AddSubMenu(new Menu("Jungle", "Jungle"));
            BadaoPoppyVariables.JungleQ = Jungle.AddItem(new MenuItem("JungleQ","Q")).SetValue(true);
            BadaoPoppyVariables.JungleE = Jungle.AddItem(new MenuItem("JungleE", "E")).SetValue(true);
            BadaoPoppyVariables.JungleMana = Jungle.AddItem(new MenuItem("JungleMana", "Mana Limit")).SetValue(new Slider(40, 0, 100));

            // Assassinate
            Menu Assassinate = config.AddSubMenu(new Menu("Assassinate", "Assassinate"));
            BadaoPoppyVariables.AssassinateKey = Assassinate.AddItem(new MenuItem("AssassinateKey", "Active")).SetValue(new KeyBind('T',KeyBindType.Press));
            Assassinate.AddItem(new MenuItem("AssasinateNote", "Select a target to use this mode"));

            // Auto
            Menu Auto = config.AddSubMenu(new Menu("Auto", "Auto"));
            foreach (var hero in HeroManager.Enemies)
            {
                Auto.AddItem(new MenuItem("AutoAntiDash" + hero.NetworkId, "W anti dash " + hero.ChampionName + " (" + hero.Name + ")")).SetValue(true);
            }
            BadaoPoppyVariables.AutoEInterrupt = Auto.AddItem(new MenuItem("AutoEInterrupt", "E interrupt")).SetValue(false);
            BadaoPoppyVariables.AutoRInterrupt = Auto.AddItem(new MenuItem("AutoRInterrupt", "R interrupt")).SetValue(false);
            BadaoPoppyVariables.AutoRKS = Auto.AddItem(new MenuItem("AutoRKS", "R KS")).SetValue(true);
            BadaoPoppyVariables.AutoR3Target = Auto.AddItem(new MenuItem("AutoR3Target", "R hits 3 target")).SetValue(true);

            // attach to mainmenu
            config.AddToMainMenu();
        }
    }
}
