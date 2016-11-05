using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
//using SDK = LeagueSharp.SDK;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoTheLastMemebender
{
    internal class Config
    {
        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;

        public Config()
        {
            Menu = new Menu("Yasuo - The Last memebender", "YLM", true);

            //Sub Menus
            var orbwalkMenu        = new Menu("Orbwalker"                  , "ylm.orbwalk");
            var targetSelectorMenu = new Menu("Targetselector settings"    , "ylm.targetselector");
            var gapcloseMenu       = new Menu("Gapclose settings"          , "ylm.gapclose"); //done
            var windwallMenu       = new Menu("Windwall"                   , "ylm.windwall"); //No gun make this, use ezvade lol
            var windwallSpells     = new Menu("Spells"                     , "ylm.windwall.spells");
            var towerDiveMenu      = new Menu("Tower dive settings"        , "ylm.towerdive"); //done 
            var spellMenu          = new Menu("Spell Settings"             , "ylm.spell"); //done
            var spellEMenu         = new Menu("E - Sweeping Blade settings", "ylm.spellSetting.E"); //done
            var spellRMenu         = new Menu("R - Last Breath settings"   , "ylm.spellSetting.R"); //done
            var comboMenu          = new Menu("Combo"                      , "ylm.combo"); //done - Items - Summoners
            var killstealMenu      = new Menu("Killsteal"                  , "ylm.killsteal");
            var harassMenu         = new Menu("Harass"                     , "ylm.mixed"); //done
            var lasthitMenu        = new Menu("Lasthit"                    , "ylm.lasthit"); //done
            var laneclearMenu      = new Menu("Laneclear"                  , "ylm.laneclear"); //done
            var jungleclearMenu    = new Menu("Jungleclear"                , "ylm.jungleclear"); //done
            var drawMenu           = new Menu("Drawings"                   , "ylm.drawings");
            var antiMenu           = new Menu("Interrupt & Antigapclose"   , "ylm.anti"); //done
            //var predictionMenu     = new Menu("Prediction Settings"        , "ylm.prediction");

            //Target Selector
            TargetSelector.AddToMenu(targetSelectorMenu);

            //Orbwalker
            Orbwalker = new Orbwalking.Orbwalker(orbwalkMenu);
            //keybindings
            Orbwalker.RegisterCustomMode("ylm.orbwalker.escape", "Escape", 'Y');
            Menu.AddSubMenu(orbwalkMenu);

            #region GapClose


            gapcloseMenu.AddItem(new MenuItem("ylm.gapclose.on", "Use Gapclose").SetValue(true));
            gapcloseMenu.AddItem(new MenuItem("ylm.gapclose.hpcheck", "Check health before gapclosing").SetValue(true));
            gapcloseMenu.AddItem(new MenuItem("ylm.gapclose.hpcheck2", "Only gapclose if my health > %").SetValue(new Slider(15, 0, 100)));
            gapcloseMenu.AddItem(new MenuItem("ylm.gapclose.stackQ", "Stack Q while gapclosing").SetValue(false));
            gapcloseMenu.AddItem(new MenuItem("ylm.gapclose.seperator", ""));
            gapcloseMenu.AddItem(new MenuItem("ylm.gapclose.limit", "Set gapclose range").SetValue(new Slider(3200, 650, 3200)));
            gapcloseMenu.AddItem(new MenuItem("ylm.gapclose.draw", "Draw gapclose target").SetValue(true));
            Menu.AddSubMenu(gapcloseMenu);

            #endregion

            #region Tower Dive

            towerDiveMenu.AddItem(new MenuItem("ylm.towerdive.enabled", "Tower Dive").SetValue(false));
            towerDiveMenu.AddItem(new MenuItem("ylm.towerdive.minAllies", "Min number of allies to dive").SetValue(new Slider(3,0,5)));
            Menu.AddSubMenu(towerDiveMenu);

            #endregion

            #region Spell Settings

            spellEMenu.AddItem(
                new MenuItem("ylm.spelle.range", "Check distance of target and E endpos").SetValue(true));
            spellEMenu.AddItem(
                new MenuItem("ylm.spelle.rangeslider", "Maximum distance").SetValue(new Slider(200, 150, 400)));
            spellEMenu.AddItem(new MenuItem("ylm.spelle.seperator", ""));
            spellEMenu.AddItem(new MenuItem("ylm.spelle.info", "> This option will check if the distance"));
            spellEMenu.AddItem(new MenuItem("ylm.spelle.info2", "> between your target and the endposition of your E cast"));
            spellEMenu.AddItem(new MenuItem("ylm.spelle.info3", "> is greater then the distance set in the slider."));
            spellEMenu.AddItem(new MenuItem("ylm.spelle.info4", "> If yes the cast will get blocked!"));
            spellEMenu.AddItem(new MenuItem("ylm.spelle.info5", "> This prevents dashing too far away from your target!"));
            spellMenu.AddSubMenu(spellEMenu);

            spellRMenu.AddItem(new MenuItem("ylm.spellr.auto", "Use Auto Ultimate").SetValue(false));
            spellRMenu.AddItem(
                new MenuItem("ylm.spellr.targetnumber", "Number of Targets for Auto R").SetValue(new Slider(3, 1, 5))).SetTooltip("Auto R ignores settings below and only checks for X targets");
            /*spellRMenu.AddItem(
                new MenuItem("ylm.spellr.infoautor", "> Auto R ignores settings below and only checks for X targets"));*/
            spellRMenu.AddItem(new MenuItem("ylm.spellr.seperator", ""));
            spellRMenu.AddItem(new MenuItem("ylm.spellr.delay", "Delay the ultimate for more CC").SetValue(true));
            spellRMenu.AddItem(new MenuItem("ylm.spellr.useqult", "Use Q while ulting").SetValue(true));
            spellRMenu.AddItem(new MenuItem("ylm.spellr.towercheck", "Use Ultimate under towers").SetValue(true));
            spellRMenu.AddItem(new MenuItem("ylm.spellr.seperator2", ""));
            foreach (var enemy in HeroManager.Enemies)
            {
                spellRMenu.AddItem(
                    new MenuItem(string.Format("ylm.spellr.rtarget.{0}", enemy.ChampionName.ToLower()),
                                enemy.ChampionName).SetValue(
                        new StringList(
                            new[] { "Don't Use", "Use if killable", "Always use", "Use advanced checks for target" },2)));
            }
            spellRMenu.AddItem(new MenuItem("ylm.spellr.seperator3", ""));
            spellRMenu.AddItem(new MenuItem("ylm.spellr.infoadvanced", ">> Advanced settings"));
            spellRMenu.AddItem(new MenuItem("ylm.spellr.targethealth", "Check for target health").SetValue(true));
            spellRMenu.AddItem(
                new MenuItem("ylm.spellr.targethealthslider", "Only ult if target health below < %").SetValue(
                    new Slider(40, 0, 100)));
            spellRMenu.AddItem(new MenuItem("ylm.spellr.playerhealth", "Check for our health").SetValue(true));
            spellRMenu.AddItem(
                new MenuItem("ylm.spellr.playerhealthslider", "Only ult if player health bigger > %").SetValue(
                    new Slider(40, 0, 100)));

            spellMenu.AddSubMenu(spellRMenu);


            spellMenu.AddItem(new MenuItem("ylm.spell.seperator", ""));
            spellMenu.AddItem(new MenuItem("ylm.spell.autolevelon", "Auto Level Enable/Disable").SetValue(true));
            spellMenu.AddItem(
                new MenuItem("ylm.spell.autolevelskills", "Auto Level Skills").SetValue(
                    new StringList(
                        new[]
                            { "No Autolevel", "QEWQ - R>Q>E>W", "EQWQ - R>Q>E>W", "EQEWE - R>Q>E>W", "QEWE - R>E>Q>W" })));

            Menu.AddSubMenu(spellMenu);
            #endregion

            #region Combo

            comboMenu.AddItem(new MenuItem("ylm.combo.mode", "Choose Combo mode").SetValue(new StringList(new[] { "Prefer Q3-E", "Prefer E-Q3" })));
            comboMenu.AddItem(new MenuItem("ylm.combo.items", "Use items in Combo").SetValue(true));
            comboMenu.AddItem(new MenuItem("ylm.combo.seperator", ""));
            comboMenu.AddItem(new MenuItem("ylm.combo.useq", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("ylm.combo.useq3", "Use Q3").SetValue(true));
            comboMenu.AddItem(new MenuItem("ylm.combo.usee", "Use E").SetValue(true));
            comboMenu.AddItem(new MenuItem("ylm.combo.user", "Use R").SetValue(true));
            comboMenu.AddItem(new MenuItem("ylm.combo.useignite", "Use Ignite").SetValue(true));
            Menu.AddSubMenu(comboMenu);

            #endregion

            #region Harass

            harassMenu.AddItem(new MenuItem("ylm.mixed.mode", "Choose Harass mode").SetValue(new StringList(new[] { "Normal Harass", "Safe Harass" })));
            //harassMenu.AddItem(new MenuItem("ylm.mixed.lasthitnotarget", "Enable smart lasthit if no target").SetValue(true));
            //harassMenu.AddItem(new MenuItem("ylm.mixed.lasthittarget", "Enable smart lasthit if target").SetValue(true));
            harassMenu.AddItem(new MenuItem("ylm.mixed.lasthit", "Last hit").SetValue(true)).SetTooltip("Will use spellsettings from the lasthitmenu");
            harassMenu.AddItem(new MenuItem("ylm.mixed.seperator", ""));
            harassMenu.AddItem(new MenuItem("ylm.mixed.infoabilities", "Harass abilities"));
            harassMenu.AddItem(new MenuItem("ylm.mixed.useq", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("ylm.mixed.useq3", "Use Q3").SetValue(true));
            harassMenu.AddItem(new MenuItem("ylm.mixed.usee", "Use E").SetValue(false));
            Menu.AddSubMenu(harassMenu);

            #endregion

            #region Killsteal

            killstealMenu.AddItem(new MenuItem("ylm.killsteal.usesmartks", "Use Smart Killsteal").SetValue(true));
            killstealMenu.AddItem(new MenuItem("ylm.killsteal.seperator", ""));
            killstealMenu.AddItem(new MenuItem("ylm.killsteal.useq", "Use Q").SetValue(true));
            killstealMenu.AddItem(new MenuItem("ylm.killsteal.useq3", "Use Q3").SetValue(true));
            killstealMenu.AddItem(new MenuItem("ylm.killsteal.usee", "Use E").SetValue(true));
            killstealMenu.AddItem(new MenuItem("ylm.killsteal.user", "Use R").SetValue(true));
            Menu.AddSubMenu(killstealMenu);

            #endregion

            #region Lasthit

            lasthitMenu.AddItem(new MenuItem("ylm.lasthit.enabled", "Use skills to lasthit").SetValue(false));
            lasthitMenu.AddItem(new MenuItem("ylm.lasthit.useq", "Use Q").SetValue(true));
            lasthitMenu.AddItem(new MenuItem("ylm.lasthit.useq3", "Use Q3").SetValue(true));
            lasthitMenu.AddItem(new MenuItem("ylm.lasthit.usee", "Use E").SetValue(true));
            Menu.AddSubMenu(lasthitMenu);

            #endregion

            #region Laneclear
            laneclearMenu.AddItem(
                new MenuItem("ylm.laneclear.changeWithScroll", "Toggle LaneClear skills with scrolling wheel").SetValue(false));
            laneclearMenu.AddItem(new MenuItem("ylm.laneclear.enabled", "Use skills to laneclear").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("ylm.laneclear.modee", "Choose Laneclear mode for E").SetValue(new StringList(new[] { "Only lasthit with E", "Always use E" })));
            laneclearMenu.AddItem(new MenuItem("ylm.laneclear.modeq3", "Choose Laneclear mode for Q3").SetValue(new StringList(new[] { "Cast to best position", "Cast to X or more amount of units" })));
            laneclearMenu.AddItem(new MenuItem("ylm.laneclear.modeq3amount", "Min units to hit with Q3").SetValue(new Slider(3, 1, 8)));
            laneclearMenu.AddItem(new MenuItem("ylm.laneclear.items", "Use items for Laneclear").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("ylm.laneclear.seperator", ""));
            laneclearMenu.AddItem(new MenuItem("ylm.laneclear.useq", "Use Q").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("ylm.laneclear.useq3", "Use Q3").SetValue(true));
            laneclearMenu.AddItem(new MenuItem("ylm.laneclear.usee", "Use E").SetValue(true));
            Menu.AddSubMenu(laneclearMenu);

            #endregion

            #region Jungleclear

            jungleclearMenu.AddItem(new MenuItem("ylm.jungleclear.enabled", "Use skills to jungle clear").SetValue(true));
            jungleclearMenu.AddItem(new MenuItem("ylm.jungleclear.items", "Use items for Jungleclear").SetValue(true));
            jungleclearMenu.AddItem(new MenuItem("ylm.jungleclear.seperator", ""));
            jungleclearMenu.AddItem(new MenuItem("ylm.jungleclear.useq", "Use Q").SetValue(true));
            jungleclearMenu.AddItem(new MenuItem("ylm.jungleclear.useq3", "Use Q3").SetValue(true));
            jungleclearMenu.AddItem(new MenuItem("ylm.jungleclear.usee", "Use E").SetValue(true));
            Menu.AddSubMenu(jungleclearMenu);

            #endregion

            #region Drawings

            drawMenu.AddItem(new MenuItem("ylm.drawings.drawdamage", "Draw damage on Healthbar").SetValue(new Circle(true, Color.GreenYellow)));
            drawMenu.AddItem(
                new MenuItem("ylm.drawings.drawspellsready", "Draw spells only if not on cooldown").SetValue(true));
            drawMenu.AddItem(new MenuItem("ylm.drawings.drawq", "Draw Q").SetValue(new Circle(true, Color.DarkOrange)));
            drawMenu.AddItem(new MenuItem("ylm.drawings.draww", "Draw W").SetValue(new Circle(true, Color.DarkOrange)));
            drawMenu.AddItem(new MenuItem("ylm.drawings.drawe", "Draw E").SetValue(new Circle(true, Color.DarkOrange)));
            drawMenu.AddItem(new MenuItem("ylm.drawings.drawr", "Draw R").SetValue(new Circle(true, Color.DarkOrange)));
            Menu.AddSubMenu(drawMenu);

            #endregion

            #region Interrupt & Anti Gapclose

            var interrupt = new Menu("Anti-Gapcloser", "ylm.anti.interrupt");
            interrupt.AddItem(new MenuItem("ylm.anti.interrupt.useq3", "Use Q3").SetValue(true));
            interrupt.AddItem(new MenuItem("ylm.anti.interrupt.seperator", ""));
            foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(h=>h.IsEnemy))
            {
                interrupt.AddItem(new MenuItem(string.Format("ylm.anti.interrupt.{0}", hero.ChampionName), hero.ChampionName).SetValue(true));
            }

            var antiGapclose = new Menu("Anti-Gapcloser", "ylm.anti.gapclose");
            antiGapclose.AddItem(new MenuItem("ylm.anti.gapclose.useq3", "Use Q3").SetValue(true));
            antiGapclose.AddItem(new MenuItem("ylm.anti.gapclose.seperator", ""));
            CustomizableAntiGapcloser.AddToMenu(antiGapclose);

            antiMenu.AddSubMenu(interrupt);
            antiMenu.AddSubMenu(antiGapclose);
            Menu.AddSubMenu(antiMenu);

            #endregion

            //Windwall - ezVade op
            windwallMenu.AddItem(new MenuItem("ylm.windwall.enabled", "Block targeted skills").SetValue(true));
                foreach (var hero in ObjectManager.Get<AIHeroClient>().Where(h=>h.IsEnemy))
                {
                    var hero1 = hero;
                    foreach (var spell in Wrapper.Spells.Database.Spells.Where(s=>s.ChampionName == hero1.ChampionName))
                    {
                        windwallSpells.AddItem(new MenuItem(string.Format("ylm.windwall.spells.{0}", spell.SpellName),
                            string.Format("{0} - {1}", hero1.ChampionName, spell.Slot)).SetValue(true));
                        Console.WriteLine("Added {0} - {1} - {2}", hero1.ChampionName, spell.Slot, spell.SpellName);
                    }
                }
            windwallMenu.AddSubMenu(windwallSpells);

            windwallMenu.AddItem(new MenuItem("ylm.windwall.info", "Use EzVade :^ )"));

            Menu.AddSubMenu(windwallMenu);


            Menu.AddToMainMenu();
            

        }

        public static T Param<T>(string item)
        {
            return Menu.Item(item).GetValue<T>();
        }
    }
}