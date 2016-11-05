using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RivenToTheChallenger
{
    class Config
    {
        //Flame about singleton inc
        private Menu menu;

        private static string scriptPrefix => "andre.rttc.";

        public Orbwalking.Orbwalker Orbwalker;

        public MenuItem this[string itemName] => menu.Item(scriptPrefix + itemName);

        public Config()
        {
            menu = new Menu("Riven To The Challenger", "andre.rttc", true);
            
            #region SubMenus declaration
            var orbwalkerMenu = new Menu("Orbwalker", "andre.rttc.orbwalker");
            var comboMenu =  new Menu("Combo Settings", "andre.rttc.combo");
            var utilityMenu =  new Menu("Utility Settings", "andre.rttc.utility");
            var fleeMenu =  new Menu("Flee Settings", "andre.rttc.flee");
            var jungleMenu =  new Menu("Jungle clear Settings", "andre.rttc.jungle");
            var eMenu =  new Menu("E Settings", "andre.rttc.eee");
            var drawMenu =  new Menu("Draw Settings", "andre.rttc.draw");
            var advancedMenu =  new Menu("Advanced", "andre.rttc.advanced");
            #endregion 

            #region SubMenus
            
            #region Orbwalker
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Orbwalker.RegisterCustomMode(orbwalkerMenu.Name + ".escape", "Escape", 'Y');
            menu.AddSubMenu(orbwalkerMenu);
            #endregion
            
            #region Combo Settings
            #region SpellUsage
            var comboSpellUsage = new Menu("Spell usage", comboMenu.Name + ".spells"); //combo.spells
            comboSpellUsage.AddItem(new MenuItem(comboSpellUsage.Name + ".useq", "Use [Q - Broken Wings]").SetValue(true));
            comboSpellUsage.AddItem(new MenuItem(comboSpellUsage.Name + ".usew", "Use [W - Ki Burst]").SetValue(true));
            comboSpellUsage.AddItem(new MenuItem(comboSpellUsage.Name + ".usehydra", "Use Ravenous Hydra").SetValue(true));
            comboSpellUsage.AddItem(new MenuItem(comboSpellUsage.Name + ".useygb", "Use Youmus Ghostblade").SetValue(true));
            comboMenu.AddSubMenu(comboSpellUsage);
            #endregion
            
            #region Initiator
            var initiatorMenu = new Menu("Initiator", comboMenu.Name + ".ew");//combo.ew
            initiatorMenu.AddItem(
                new MenuItem(initiatorMenu.Name + ".w", "After E -> W").SetValue(
                    new StringList(new[] { "Force Q", "Force AA", "Automatic" }, 2)));
            comboMenu.AddSubMenu(initiatorMenu);
            #endregion

            #region GapClose
            var comboGapClose = new Menu("GapClose", comboMenu.Name + ".gap"); //combo.gap
            comboGapClose.AddItem(
                new MenuItem(comboGapClose.Name + ".q", "Gapclose [Q - Broken Wings]").SetValue(
                    new StringList(new[] { "Disabled", "In Combo", "In Combo + Target is left-clicked" })));
            comboGapClose.AddItem(
                new MenuItem(comboGapClose.Name + ".w", "Gapclose [W - Ki Burst]").SetValue(
                    new StringList(new[] { "Disabled", "In Combo", "In Combo + Target is left-clicked" })));
            comboMenu.AddSubMenu(comboGapClose);
            #endregion

            #region R Settings
            var comboRMenu = new Menu("RSettings", comboMenu.Name + ".r"); //combo.r
            comboRMenu.AddItem(new MenuItem(comboRMenu.Name + ".r1", "Use R1").SetValue(true));
            comboRMenu.AddItem(new MenuItem(comboRMenu.Name + ".r2", "Use R2").SetValue(true));
            //comboRMenu.AddItem(new MenuItem("andre.rttc.combo.r.r2bindfr1", "Bind R2 Cast to Force R1").SetValue(true));
            comboRMenu.AddItem(
                new MenuItem("andre.rttc.combo.r.combomode", "Force R1 after next animation").SetValue(new KeyBind('N',
                    KeyBindType.Toggle, false)));
            comboMenu.AddSubMenu(comboRMenu);
            #endregion

            menu.AddSubMenu(comboMenu);
            #endregion
            
            #region Utility Menu
            #region KillSteal
            var killStealMenu = new Menu("killSteal", utilityMenu.Name + ".ks"); //utility.ks
            killStealMenu.AddItem(new MenuItem(killStealMenu.Name + ".useq", "Use Q").SetValue(true));
            killStealMenu.AddItem(new MenuItem(killStealMenu.Name + ".usew", "Use W").SetValue(true));
            killStealMenu.AddItem(new MenuItem(killStealMenu.Name + ".user", "Use R2").SetValue(true));
            killStealMenu.AddItem(new MenuItem(killStealMenu.Name + ".usei", "Use ignite").SetValue(true));

            utilityMenu.AddSubMenu(killStealMenu);
            #endregion

            #region Defensive
            var defensiveMenu = new Menu("Defensive", utilityMenu.Name + ".defensive"); //utility.defensive
            defensiveMenu.AddItem(new MenuItem(defensiveMenu.Name + ".w", "Use W").SetValue(true));
            utilityMenu.AddSubMenu(defensiveMenu);
            #endregion

            menu.AddSubMenu(utilityMenu);

            #endregion

            #region Flee Menu
            //utility
            fleeMenu.AddItem(new MenuItem(fleeMenu.Name + ".useq", "Use Q").SetValue(true));
            fleeMenu.AddItem(new MenuItem(fleeMenu.Name + ".usee", "Use E").SetValue(true));
            fleeMenu.AddItem(new MenuItem(fleeMenu.Name + ".key", "Can be set in orbwalker"));
            menu.AddSubMenu(fleeMenu);
            #endregion

            #region Jungle

            #endregion

            #region Harass

            #endregion

            #region Draw

            #endregion

            #region Advanced

            #endregion
            #endregion

            menu.AddToMainMenu();


        }
    }
}
