#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Security.AccessControl;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace EloFactory_Riven
{
    internal class Program
    {
        public const string ChampionName = "Riven";

        public static Orbwalking.Orbwalker Orbwalker;

        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;
        public static Spell Qfarm;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite = new Spell(SpellSlot.Unknown, 600);
        public static SpellSlot FlashSlot;

        public static Items.Item HealthPotion = new Items.Item(2003, 0);
        public static Items.Item ManaPotion = new Items.Item(2004, 0);
        public static Items.Item CrystallineFlask = new Items.Item(2041, 0);
        public static Items.Item BiscuitofRejuvenation = new Items.Item(2010, 0);

        public static Menu Config;

        private static AIHeroClient Player;

        public static Items.Item YoumuusGhostblade = new Items.Item(3142, 0);
        public static Items.Item BilgewaterCutlass = new Items.Item(3144, 450);
        public static Items.Item BladeoftheRuinedKing = new Items.Item(3153, 450);
        public static Items.Item Tiamat = new Items.Item(3077, 400);
        public static Items.Item RavenousHydra = new Items.Item(3074, 400);
        public static Items.Item Entropy = new Items.Item(3184, 0);

        private static int lastCastQ;
        private static int lastCastW;
        private static int lastCastE;
        private static int lastCastAA;
        private static int lastCastFlashCombo;

        private static bool CanCastQ;
        private static bool CanCastW;
        private static bool CanCastE;
        private static bool CanMove;
        private static bool CanUseAA;

        private static bool CastedQ;
        private static bool CastedW;
        private static bool CastedE;
        private static bool CastedR2;
        private static bool UsedAA;

        public static float getQRange()
        {
            if (Player.HasBuff("RivenFengShuiEngine") && Player.GetBuffCount("RivenTriCleaveBuff") == 2)
            {
                return 650;
            }
            else return 260;
        }

        public static float ComboRange()
        {
            if (Player.CountEnemiesInRange(1300) > 1)
            {
                float range = 0f;

                if (Q.IsReady())
                {
                    range += Q.Range;
                }
                if (E.IsReady())
                {
                    range += E.Range;
                }
                if (W.IsReady())
                {
                    range += W.Range;
                }
                range += Player.AttackRange;
                return range;
            }

            else return 1200;
        }

        public static float LaneClearQRange()
        {
            if (Q.IsReady() && Config.Item("Riven.UseQLaneClear").GetValue<bool>() && Player.GetBuffCount("RivenTriCleaveBuff") != 2)
            {
                return 180;
            }
            else return 0;
        }

        public static float LaneClearERange()
        {
            if (E.IsReady() && Config.Item("Riven.UseELaneClear").GetValue<bool>())
            {
                return 270;
            }
            else return 0;
        }

        public static float LaneClearExtendedRange()
        {
            if (Config.Item("Riven.LockMovementLaneClear").GetValue<bool>())
            {
                return 300;
            }
            else return 0;
        }

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;

            if (Player.ChampionName != ChampionName) return;

            Q = new Spell(SpellSlot.Q, getQRange());
            Qfarm = new Spell(SpellSlot.Q, getQRange());
            W = new Spell(SpellSlot.W, 250f);
            E = new Spell(SpellSlot.E, 270f);
            R = new Spell(SpellSlot.R, 1100f);

            Q.SetSkillshot(0.25f, 100f, 2200f, false, SkillshotType.SkillshotCircle);
            Qfarm.SetSkillshot(0.25f, 100f, 2200f, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 225f, 1600f, false, SkillshotType.SkillshotCone);

            var ignite = Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "summonerdot");
            if (ignite != null)
                Ignite.Slot = ignite.Slot;

            FlashSlot = Player.GetSpellSlot("summonerflash");

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            #region Menu
            Config = new Menu(ChampionName + " By LuNi", ChampionName + " By LuNi", true);

            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddSubMenu(new Menu("KS Mode", "KS Mode"));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Riven.UseIgniteKS", "KS With Ignite").SetValue(true));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Riven.UseQKS", "KS With Q").SetValue(true));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Riven.Use3rdQKS", "KS With 3rd Q").SetValue(true));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Riven.Use3rdQKSCount", "Maximum Enemy Around To KS With 3rd Q").SetValue(new Slider(1, 1, 5)));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Riven.UseWKS", "KS With W").SetValue(true));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Riven.UseWKSCount", "Maximum Enemy Around To KS With W").SetValue(new Slider(1, 1, 5)));
            Config.SubMenu("Combo").AddSubMenu(new Menu("Items Activator", "Items Activator"));
            Config.SubMenu("Combo").SubMenu("Items Activator").AddSubMenu(new Menu("Use Youmuu's Ghostblade", "Use Youmuu's Ghostblade"));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Youmuu's Ghostblade").AddItem(new MenuItem("Riven.useYoumuusGhostblade", "Use Youmuu's Ghostblade").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Youmuu's Ghostblade").AddItem(new MenuItem("Riven.MinimumHPtoYoumuusGhostblade", "Minimum Health Percent To Force Youmuu's Ghostblade").SetValue(new Slider(75, 0, 100)));
            Config.SubMenu("Combo").SubMenu("Items Activator").AddSubMenu(new Menu("Use Tiamat", "Use Tiamat"));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Tiamat").AddItem(new MenuItem("Riven.useTiamat", "Use Tiamat").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Items Activator").AddSubMenu(new Menu("Use Ravenous Hydra", "Use Ravenous Hydra"));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Ravenous Hydra").AddItem(new MenuItem("Riven.useRavenousHydra", "Use Ravenous Hydra").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Items Activator").AddSubMenu(new Menu("Use Bilgewater Cutlass", "Use Bilgewater Cutlass"));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Bilgewater Cutlass").AddItem(new MenuItem("Riven.useBilgewaterCutlass", "Use Bilgewater Cutlass").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Bilgewater Cutlass").AddItem(new MenuItem("Riven.MinimumHPtoBilgewaterCutlass", "Minimum Health Percent To Force Bilgewater Cutlass").SetValue(new Slider(75, 0, 100)));
            Config.SubMenu("Combo").SubMenu("Items Activator").AddSubMenu(new Menu("Use Blade of the Ruined King", "Use Blade of the Ruined King"));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Blade of the Ruined King").AddItem(new MenuItem("Riven.useBladeoftheRuinedKing", "Use Blade of the Ruined King").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Blade of the Ruined King").AddItem(new MenuItem("Riven.MinimumHPtoBladeoftheRuinedKing", "Minimum Health Percent To Force Blade of the Ruined King").SetValue(new Slider(75, 0, 100)));
            Config.SubMenu("Combo").SubMenu("Items Activator").AddSubMenu(new Menu("Use Entropy", "Use Entropy"));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Entropy").AddItem(new MenuItem("Riven.useEntropy", "Use Entropy").SetValue(true));
            Config.SubMenu("Combo").AddSubMenu(new Menu("Q In Combo", "Q In Combo"));
            Config.SubMenu("Combo").SubMenu("Q In Combo").AddItem(new MenuItem("Riven.UseQCombo", "Use Q In Combo").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Q In Combo").AddSubMenu(new Menu("Q Advanced Settings", "Q Advanced Settings"));
            Config.SubMenu("Combo").SubMenu("Q In Combo").SubMenu("Q Advanced Settings").AddItem(new MenuItem("Riven.QDelay", "Q Delay (ms)")).SetValue(new Slider(0, 0, 300));
            Config.SubMenu("Combo").SubMenu("Q In Combo").SubMenu("Q Advanced Settings").AddItem(new MenuItem("Riven.KeepQUp", "Keep Q Up").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Q In Combo").SubMenu("Q Advanced Settings").AddItem(new MenuItem("Riven.GapCloseQ", "Gapclose with Q")).SetValue(true);
            Config.SubMenu("Combo").SubMenu("Q In Combo").SubMenu("Q Advanced Settings").AddItem(new MenuItem("Riven.GapCloseQCount", "Maximum Q Usage To GapClose Target")).SetValue(new Slider(2, 1, 3));
            Config.SubMenu("Combo").SubMenu("Q In Combo").SubMenu("Q Advanced Settings").AddItem(new MenuItem("Riven.GapCloserDelay", "Gapclose Q Delay (ms)")).SetValue(new Slider(0, 0, 200));
            Config.SubMenu("Combo").AddSubMenu(new Menu("W In Combo", "W In Combo"));
            Config.SubMenu("Combo").SubMenu("W In Combo").AddItem(new MenuItem("Riven.UseWCombo", "Use W In Combo").SetValue(true));
            Config.SubMenu("Combo").SubMenu("W In Combo").AddSubMenu(new Menu("Auto W Settings", "Auto W Settings"));
            Config.SubMenu("Combo").SubMenu("W In Combo").SubMenu("Auto W Settings").AddSubMenu(new Menu("AutoW Settings Early Game", "AutoW Settings Early Game"));
            Config.SubMenu("Combo").SubMenu("W In Combo").SubMenu("Auto W Settings").SubMenu("AutoW Settings Early Game").AddItem(new MenuItem("Riven.AutoWEarly", "Auto W Early Game").SetValue(true));
            Config.SubMenu("Combo").SubMenu("W In Combo").SubMenu("Auto W Settings").SubMenu("AutoW Settings Early Game").AddItem(new MenuItem("Riven.AutoWCountEarly", "Minimum Enemy Hit To Auto W Early Game").SetValue(new Slider(1, 1, 5)));
            Config.SubMenu("Combo").SubMenu("W In Combo").SubMenu("Auto W Settings").SubMenu("AutoW Settings Early Game").AddItem(new MenuItem("Riven.AutoWHPEarly", "Minimum Hp Percent To Auto W Early Game").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("Combo").SubMenu("W In Combo").SubMenu("Auto W Settings").AddSubMenu(new Menu("AutoW Settings Late Game", "AutoW Settings Late Game"));
            Config.SubMenu("Combo").SubMenu("W In Combo").SubMenu("Auto W Settings").SubMenu("AutoW Settings Late Game").AddItem(new MenuItem("Riven.AutoWLate", "Auto W Late Game").SetValue(true));
            Config.SubMenu("Combo").SubMenu("W In Combo").SubMenu("Auto W Settings").SubMenu("AutoW Settings Late Game").AddItem(new MenuItem("Riven.AutoWCountLate", "Minimum Enemy Hit To Auto W Late Game").SetValue(new Slider(3, 1, 5)));
            Config.SubMenu("Combo").SubMenu("W In Combo").SubMenu("Auto W Settings").SubMenu("AutoW Settings Late Game").AddItem(new MenuItem("Riven.AutoWHPLate", "Minimum Hp Percent To Auto W Late Game").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("Combo").SubMenu("W In Combo").SubMenu("Auto W Settings").AddSubMenu(new Menu("AutoW Deffensive Settings", "AutoW Deffensive Settings"));
            Config.SubMenu("Combo").SubMenu("W In Combo").SubMenu("Auto W Settings").SubMenu("AutoW Deffensive Settings").AddItem(new MenuItem("Riven.AutoWDeffensive", "Auto Deffensive W").SetValue(true));
            Config.SubMenu("Combo").SubMenu("W In Combo").SubMenu("Auto W Settings").SubMenu("AutoW Deffensive Settings").AddItem(new MenuItem("Riven.AutoWCountDeffensive", "Minimum Enemy Hit To Auto Deffensive W").SetValue(new Slider(1, 1, 5)));
            Config.SubMenu("Combo").SubMenu("W In Combo").SubMenu("Auto W Settings").SubMenu("AutoW Deffensive Settings").AddItem(new MenuItem("Riven.AutoWHPDeffensive", "Minimum Hp Percent To Auto Deffensive W").SetValue(new Slider(30, 0, 100)));
            Config.SubMenu("Combo").AddSubMenu(new Menu("E In Combo", "E In Combo"));
            Config.SubMenu("Combo").SubMenu("E In Combo").AddItem(new MenuItem("Riven.UseECombo", "Use E In Combo").SetValue(true));
            Config.SubMenu("Combo").SubMenu("E In Combo").AddSubMenu(new Menu("E Advanced Settings", "E Advanced Settings"));
            Config.SubMenu("Combo").SubMenu("E In Combo").SubMenu("E Advanced Settings").AddItem(new MenuItem("Riven.MiniHpForAutoE", "Minimum Hp To use E To Protect Yourself").SetValue(new Slider(40, 0, 100)));
            Config.SubMenu("Combo").AddSubMenu(new Menu("R In Combo", "R In Combo"));
            Config.SubMenu("Combo").SubMenu("R In Combo").AddSubMenu(new Menu("R1 In Combo", "R1 In Combo"));
            Config.SubMenu("Combo").SubMenu("R In Combo").SubMenu("R1 In Combo").AddItem(new MenuItem("Riven.UseRCombo", "Use R1 in Combo")).SetValue(true);
            Config.SubMenu("Combo").SubMenu("R In Combo").SubMenu("R1 In Combo").AddItem(new MenuItem("Riven.R1Mode", "R1 Mode Early Game").SetValue(new StringList(new[] { "Agressive R", "Save R" })));
            Config.SubMenu("Combo").SubMenu("R In Combo").SubMenu("R1 In Combo").AddItem(new MenuItem("Riven.R1ModeHidden", "R1 HiddenMode 1VS1 (R Only When Needed Never Over R)")).SetValue(false);
            Config.SubMenu("Combo").SubMenu("R In Combo").AddSubMenu(new Menu("R2 In Combo", "R2 In Combo"));
            Config.SubMenu("Combo").SubMenu("R In Combo").SubMenu("R2 In Combo").AddItem(new MenuItem("Riven.UseR2Combo", "Use R2 in Combo")).SetValue(true);
            Config.SubMenu("Combo").SubMenu("R In Combo").SubMenu("R2 In Combo").AddItem(new MenuItem("Riven.R2Mode", "R2 Mode")).SetValue(new StringList(new[] { "Kill Only", "Kill Or Maximum Damage" }, 1));
            Config.SubMenu("Combo").SubMenu("R In Combo").SubMenu("R2 In Combo").AddItem(new MenuItem("Riven.MiniCountR2", "Minimum Enemy Hit For Auto R2")).SetValue(new Slider(3, 2, 5));
            Config.SubMenu("Combo").AddSubMenu(new Menu("Flash In Combo", "Flash In Combo"));
            Config.SubMenu("Combo").SubMenu("Flash In Combo").AddSubMenu(new Menu("Flash 1v1", "Flash 1v1"));
            Config.SubMenu("Combo").SubMenu("Flash In Combo").SubMenu("Flash 1v1").AddItem(new MenuItem("Riven.UseFlashCombo1v1", "Use Flash In Combo 1V1").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Flash In Combo").AddSubMenu(new Menu("Flash TeamFight", "Flash TeamFight"));
            Config.SubMenu("Combo").SubMenu("Flash In Combo").SubMenu("Flash TeamFight").AddItem(new MenuItem("Riven.UseFlashComboTF", "Use Flash In Combo In TeamFight").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Flash In Combo").SubMenu("Flash TeamFight").AddItem(new MenuItem("Riven.EnemyCountFlashComboTF", "Minimum Enemy To Flash In Combo In TeamFight")).SetValue(new Slider(2, 1, 5));

            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddSubMenu(new Menu("Harass When Enemy Initiate Or Poke On Me", "Harass When Enemy Initiate Or Poke On Me"));
            Config.SubMenu("Harass").SubMenu("Harass When Enemy Initiate Or Poke On Me").AddItem(new MenuItem("Riven.HarassOnInitiate", "Harass When Enemy Initiate Or Poke On Me").SetValue(true));
            Config.SubMenu("Harass").SubMenu("Harass When Enemy Initiate Or Poke On Me").AddSubMenu(new Menu("Advanced Settings", "Advanced Settings"));
            Config.SubMenu("Harass").SubMenu("Harass When Enemy Initiate Or Poke On Me").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.QHarassOnInitiate", "Use Q in Harass When Enemy Initiate Or Poke On Me").SetValue(true));
            Config.SubMenu("Harass").SubMenu("Harass When Enemy Initiate Or Poke On Me").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.WHarassOnInitiate", "Use W in Harass When Enemy Initiate Or Poke On Me").SetValue(true));
            Config.SubMenu("Harass").SubMenu("Harass When Enemy Initiate Or Poke On Me").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.EHarassOnInitiate", "Use E in Harass When Enemy Initiate Or Poke On Me").SetValue(true));
            Config.SubMenu("Harass").SubMenu("Harass When Enemy Initiate Or Poke On Me").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.RHarassOnInitiate", "Use R in Harass When Enemy Initiate Or Poke On Me").SetValue(true));
            Config.SubMenu("Harass").SubMenu("Harass When Enemy Initiate Or Poke On Me").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.ItemsHarassOnInitiate", "Use Items in Harass When Enemy Initiate Or Poke On Me").SetValue(true));
            Config.SubMenu("Harass").AddSubMenu(new Menu("Harass When Enemy Farm", "Harass When Enemy Farm"));
            Config.SubMenu("Harass").SubMenu("Harass When Enemy Farm").AddItem(new MenuItem("Riven.HarassOnEnemyFarm", "Harass When Enemy Farm").SetValue(false));
            Config.SubMenu("Harass").SubMenu("Harass When Enemy Farm").AddSubMenu(new Menu("Advanced Settings", "Advanced Settings"));
            Config.SubMenu("Harass").SubMenu("Harass When Enemy Farm").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.QHarassOnEnemyFarm", "Use Q in Harass When Enemy Farm").SetValue(true));
            Config.SubMenu("Harass").SubMenu("Harass When Enemy Farm").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.WHarassOnEnemyFarm", "Use W in Harass When Enemy Farm").SetValue(true));
            Config.SubMenu("Harass").SubMenu("Harass When Enemy Farm").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.EHarassOnEnemyFarm", "Use E in Harass When Enemy Farm").SetValue(true));
            Config.SubMenu("Harass").SubMenu("Harass When Enemy Farm").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.RHarassOnEnemyFarm", "Use R in Harass When Enemy Farm").SetValue(true));
            Config.SubMenu("Harass").SubMenu("Harass When Enemy Farm").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.ItemsHarassOnEnemyFarm", "Use Items in Harass When Enemy Farm").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("Riven.UseEOnSpell", "Use Deffensive E On Enemy Spell").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("Riven.UseEOnAA", "Use Deffensive E On Enemy AA").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("Riven.MiniHpForHarass", "Minimum Health Percent To Use Harass")).SetValue(new Slider(25, 0, 100));
            Config.SubMenu("Harass").AddItem(new MenuItem("Riven.HarassActive", "Harass!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Flee", "Flee"));
            Config.SubMenu("Flee").AddItem(new MenuItem("Riven.UseQFleeMode", "Use Q In Flee Mode").SetValue(true));
            Config.SubMenu("Flee").AddItem(new MenuItem("Riven.UseWFleeMode", "Use W In Flee Mode").SetValue(true));
            Config.SubMenu("Flee").AddItem(new MenuItem("Riven.UseWFleeModeCount", "Minimum Enemy Around To Use W In Flee Mode")).SetValue(new Slider(1, 1, 5));
            Config.SubMenu("Flee").AddItem(new MenuItem("Riven.UseEFleeMode", "Use E In Flee Mode").SetValue(true));
            Config.SubMenu("Flee").AddItem(new MenuItem("Riven.FleeMode", "Flee Mode E Priority").SetValue(new StringList(new[] { "E 1st", "E After 1st Q", "E After 2nd Q" }, 2)));
            Config.SubMenu("Flee").AddItem(new MenuItem("Riven.UseYoumuuFleeMode", "Use Youmuu In Flee Mode").SetValue(true));
            Config.SubMenu("Flee").AddItem(new MenuItem("Riven.UseYoumuuFleeModeEnemyCount", "Minimum Enemy Around To Use Youmuu's Ghostblade In Flee Mode")).SetValue(new Slider(1, 0, 5));
            Config.SubMenu("Flee").AddItem(new MenuItem("Riven.FleeActive", "Flee!").SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Riven.UseQLaneClear", "Use Q in LaneClear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Riven.QLaneClearCount1", "Minimum Minion To Use Q In LaneClear").SetValue(new Slider(2, 1, 6)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Riven.UseWLaneClear", "Use W in LaneClear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Riven.WLaneClearCount1", "Minimum Minion To Use W In LaneClear").SetValue(new Slider(3, 1, 6)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Riven.UseELaneClear", "Use E in LaneClear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Riven.UseTiamatHydraLaneClear", "Use Tiamat / Hydra In LaneClear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Riven.TiamatHydraLaneClearCount1", "Minimum Minion To Use Tiamat / Hydra In LaneClear").SetValue(new Slider(2, 1, 6)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Riven.LockMovementLaneClear", "Lock Movement On WayPoint In LaneClear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Riven.SafeLaneClear", "Dont Use Spell In LaneClear If Enemy in Dangerous Range").SetValue(true));

            Config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Riven.UseQJungleClear", "Use Q In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Riven.UseWJungleClear", "Use W In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Riven.UseEJungleClear", "Use E In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Riven.UseTiamatHydraJungleClear", "Use Tiamat / Hydra In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Riven.SafeJungleClear", "Dont Use Spell In JungleClear If Enemy in Dangerous Range").SetValue(true));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddSubMenu(new Menu("Skin Changer", "Skin Changer"));
            Config.SubMenu("Misc").SubMenu("Skin Changer").AddItem(new MenuItem("Riven.SkinChanger", "Use Skin Changer").SetValue(false));
            Config.SubMenu("Misc").SubMenu("Skin Changer").AddItem(new MenuItem("Riven.SkinChangerName", "Skin choice").SetValue(new StringList(new[] { "Classic", "Redeemed", "Crimson Elite", "Battle Bunny", "Championship", "Dragonblade" })));
            Config.SubMenu("Misc").AddSubMenu(new Menu("Interrupt Settings", "Interrupt Settings"));
            Config.SubMenu("Misc").SubMenu("Interrupt Settings").AddItem(new MenuItem("Riven.WInterrupt", "Interrupt Spells With W").SetValue(true));
            Config.SubMenu("Misc").SubMenu("Interrupt Settings").AddItem(new MenuItem("Riven.3rdQInterrupt", "Interrupt Spells With 3rd Q").SetValue(true));
            Config.SubMenu("Misc").SubMenu("Interrupt Settings").AddItem(new MenuItem("Riven.EInterrupt", "Use GapClose E To Interrupt Spells").SetValue(true));
            Config.SubMenu("Misc").SubMenu("Interrupt Settings").AddItem(new MenuItem("Riven.LockMovementInterrupt", "Lock Movement On Enemy To Interrupt").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Riven.AutoWEGC", "Auto W On Gapclosers").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Riven.AutoPotion", "Use Auto Potion").SetValue(true));
            Config.SubMenu("Misc").AddSubMenu(new Menu("Auto Level Spell", "Auto Level Spell"));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").AddItem(new MenuItem("Riven.AutoLevelSpell", "Auto Level Spell").SetValue(false));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").AddSubMenu(new Menu("Advanced Settings", "Advanced Settings"));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV1", "Spell choice Lv. 1").SetValue(new StringList(new[] { "Q", "W", "E", "No Spell" }, 3)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV2", "Spell choice Lv. 2").SetValue(new StringList(new[] { "Q", "W", "E", "No Spell" }, 3)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV3", "Spell choice Lv. 3").SetValue(new StringList(new[] { "Q", "W", "E", "No Spell" }, 3)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV4", "Spell choice Lv. 4").SetValue(new StringList(new[] { "Q", "W", "E", "No Spell" }, 0)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV5", "Spell choice Lv. 5").SetValue(new StringList(new[] { "Q", "W", "E", "No Spell" }, 0)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV6", "Spell choice Lv. 6").SetValue(new StringList(new[] { "R", "No Spell" }, 0)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV7", "Spell choice Lv. 7").SetValue(new StringList(new[] { "Q", "W", "E", "No Spell" }, 0)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV8", "Spell choice Lv. 8").SetValue(new StringList(new[] { "Q", "W", "E", "No Spell" }, 2)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV9", "Spell choice Lv. 9").SetValue(new StringList(new[] { "Q", "W", "E", "No Spell" }, 0)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV10", "Spell choice Lv. 10").SetValue(new StringList(new[] { "Q", "W", "E", "No Spell" }, 2)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV11", "Spell choice Lv. 11").SetValue(new StringList(new[] { "R", "No Spell" }, 0)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV12", "Spell choice Lv. 12").SetValue(new StringList(new[] { "Q", "W", "E", "No Spell" }, 2)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV13", "Spell choice Lv. 13").SetValue(new StringList(new[] { "Q", "W", "E", "No Spell" }, 2)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV14", "Spell choice Lv. 14").SetValue(new StringList(new[] { "Q", "W", "E", "No Spell" }, 1)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV15", "Spell choice Lv. 15").SetValue(new StringList(new[] { "Q", "W", "E", "No Spell" }, 1)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV16", "Spell choice Lv. 16").SetValue(new StringList(new[] { "R", "No Spell" }, 0)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV17", "Spell choice Lv. 17").SetValue(new StringList(new[] { "Q", "W", "E", "No Spell" }, 1)));
            Config.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Riven.SpellLV18", "Spell choice Lv. 18").SetValue(new StringList(new[] { "Q", "W", "E", "No Spell" }, 1)));
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q range").SetValue(new Circle(true, Color.Indigo)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("WRange", "W range").SetValue(new Circle(true, Color.Green)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("ERange", "E range").SetValue(new Circle(true, Color.Green)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RRange", "R range").SetValue(new Circle(true, Color.Gold)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawOrbwalkTarget", "Draw Orbwalk target").SetValue(true));

            Config.AddToMainMenu();
            #endregion

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnNewPath += Obj_AI_Base_OnNewPath;
            CustomEvents.Unit.OnDash += Unit_OnDash;

        }

        private static readonly string[] Monsters =
        {
            "SRU_Razorbeak", "SRU_Krug", "Sru_Crab", "SRU_Baron", "SRU_Dragon",
            "SRU_Blue", "SRU_Red", "SRU_Murkwolf", "SRU_Gromp", "TT_NGolem5",
            "TT_NGolem2", "TT_NWolf6", "TT_NWolf3","TT_NWraith1", "TT_Spider"
        };

        #region ToogleOrder Game_OnUpdate
        public static void Game_OnGameUpdate(EventArgs args)
        {

            if (Config.Item("Riven.SkinChanger").GetValue<bool>())
            {
                //Player.SetSkin(Player.BaseSkinName, Config.Item("Riven.SkinChangerName").GetValue<StringList>().SelectedIndex);
            }

            if (Config.Item("Riven.AutoLevelSpell").GetValue<bool>()) LevelUpSpells();

            if (Player.IsDead) return;

            if (Player.GetBuffCount("Recall") == 1) return;

            if (Config.Item("Riven.KeepQUp").GetValue<bool>())
            {
                if (Player.GetBuffCount("RivenTriCleaveBuff") >= 1)
                {
                    if (Utils.GameTimeTickCount - lastCastQ >= 3650)
                    {
                        Q.Cast(Game.CursorPos);
                    }
                }
            }

            if (Config.Item("Riven.FleeActive").GetValue<KeyBind>().Active)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                FleeLogic();
            }

            Orbwalker.SetAttack(CanMove);
            Orbwalker.SetMovement(CanMove);

            SpellStatus();
            PotionManager();
            KillSteal();

            if (W.IsReady())
            {

                if (Config.Item("Riven.AutoWDeffensive").GetValue<bool>())
                {
                    if (Player.CountEnemiesInRange(W.Range) >= Config.Item("Riven.AutoWCountDeffensive").GetValue<Slider>().Value && Player.HealthPercent <= Config.Item("Riven.AutoWHPDeffensive").GetValue<Slider>().Value)
                    {
                        var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                        if (Player.CountEnemiesInRange(W.Range) == 1 && !target.HasBuff("Black Shield") && !target.HasBuff("Spell Shield") && !target.HasBuff("BansheesVeil"))
                        {
                            W.Cast(true);
                        }
                        else
                            W.Cast(true);
                    }
                }

                if (Game.Time < 1300f && Config.Item("Riven.AutoWEarly").GetValue<bool>())
                {
                    if (Player.CountEnemiesInRange(W.Range) >= Config.Item("Riven.AutoWCountEarly").GetValue<Slider>().Value && Player.HealthPercent <= Config.Item("Riven.AutoWHPEarly").GetValue<Slider>().Value)
                    {
                        var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                        if (Player.CountEnemiesInRange(W.Range) == 1 && !target.HasBuff("Black Shield") && !target.HasBuff("Spell Shield") && !target.HasBuff("BansheesVeil"))
                        {
                            W.Cast(true);
                        }
                        else
                            W.Cast(true);
                    }
                }

                if (Game.Time >= 1300f && Config.Item("Riven.AutoWLate").GetValue<bool>())
                {
                    if (Player.CountEnemiesInRange(W.Range) >= Config.Item("Riven.AutoWCountLate").GetValue<Slider>().Value && Player.HealthPercent <= Config.Item("Riven.AutoWHPLate").GetValue<Slider>().Value)
                    {
                        var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                        if (Player.CountEnemiesInRange(W.Range) == 1 && !target.HasBuff("Black Shield") && !target.HasBuff("Spell Shield") && !target.HasBuff("BansheesVeil"))
                        {
                            W.Cast(true);
                        }
                        else
                            W.Cast(true);
                    }
                }

            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }

            if (Config.Item("Riven.HarassActive").GetValue<KeyBind>().Active)
            {
                Harass();
            }

        }
        #endregion

        #region OnProcessSpellCast
        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            var targetCancel = TargetSelector.GetTarget(500, TargetSelector.DamageType.Physical);
            var target = TargetSelector.GetTarget(ComboRange(), TargetSelector.DamageType.Physical);
            

            #region Interrupt
            double ShouldUseOn = ShouldUse(args.SData.Name);
            if (unit.Team != ObjectManager.Player.Team && ShouldUseOn >= 0f && unit.IsValidTarget(Q.Range + E.Range))
            {
                if (Config.Item("Riven.3rdQInterrupt").GetValue<bool>() && Q.IsReady() && Player.Distance(unit) <= E.Range + Q.Range && Player.GetBuffCount("RivenTriCleaveBuff") == 2)
                {
                    if (Player.Distance(unit) > Q.Range && Config.Item("Riven.EInterrupt").GetValue<bool>())
                    {
                        E.Cast(unit.ServerPosition, true);
                        if (Config.Item("Riven.LockMovementInterrupt").GetValue<bool>())
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, unit.ServerPosition);
                        }
                        return;
                    }

                    if (Player.Distance(unit) <= Q.Range)
                    {
                        Q.Cast(true);
                        if (Config.Item("Riven.LockMovementInterrupt").GetValue<bool>())
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, unit.ServerPosition);
                        }
                        return;
                    }
                }

                if (Config.Item("Riven.WInterrupt").GetValue<bool>() && W.IsReady() && Player.Distance(unit) <= E.Range + W.Range)
                {
                    if (Player.Distance(unit) > W.Range && Config.Item("Riven.EInterrupt").GetValue<bool>())
                    {
                        E.Cast(unit.ServerPosition, true);
                        if (Config.Item("Riven.LockMovementInterrupt").GetValue<bool>())
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, unit.ServerPosition);
                        }
                        return;
                    }

                    if (Player.Distance(unit) <= W.Range)
                    {
                        if (!target.HasBuff("Black Shield") && !target.HasBuff("Spell Shield") && !target.HasBuff("BansheesVeil"))
                        {
                            W.Cast(true);
                        }
                        if (Config.Item("Riven.LockMovementInterrupt").GetValue<bool>())
                        {
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, unit.ServerPosition);
                        }
                        return;
                    }
                }
            }
            #endregion

            #region Combo
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Player.HealthPercent <= Config.Item("Riven.MiniHpForAutoE").GetValue<Slider>().Value && (unit.IsValid<AIHeroClient>() || unit.IsValid<Obj_AI_Turret>()) && unit.IsEnemy && args.Target.IsMe)
                {
                    if (Config.Item("Riven.UseECombo").GetValue<bool>())
                    {
                        if ((!Config.Item("Riven.GapCloseQ").GetValue<bool>() && target.Distance(Player) < E.Range + Q.Range) || (Config.Item("Riven.GapCloseQ").GetValue<bool>() && target.Distance(Player) < E.Range + Q.Range + Player.AttackRange))
                        {
                            E.Cast(target.ServerPosition);
                            if (R.IsReady() && !Player.HasBuff("RivenWindScarReady"))
                            {
                                RLogic();
                            }
                            ItemsActivator();
                        }
                    }
                }

            }
            #endregion

            #region Harass

            if (Config.Item("Riven.HarassActive").GetValue<KeyBind>().Active)
            {
                var EscapeEEndPos = Player.ServerPosition + (Player.ServerPosition - unit.ServerPosition).Normalized() * 300;
                if (unit.IsValid<AIHeroClient>() && unit.IsEnemy && Player.CountEnemiesInRange(1300) == 1)
                {
                    if (args.Target.IsMe)
                    {
                        if (Config.Item("Riven.HarassOnInitiate").GetValue<bool>() && (Player.Distance(unit) < E.Range + W.Range && Player.HealthPercent > Config.Item("Riven.MiniHpForHarass").GetValue<Slider>().Value))
                        {

                            if (E.IsReady() && CanCastE && Config.Item("Riven.EHarassOnInitiate").GetValue<bool>() && Player.Distance(unit) > Player.AttackRange)
                            {
                                E.Cast(unit.ServerPosition);
                                if (R.IsReady() && !Player.HasBuff("RivenWindScarReady") && Config.Item("Riven.RHarassOnInitiate").GetValue<bool>())
                                {
                                    RLogic();
                                }
                                if (Config.Item("Riven.ItemsHarassOnInitiate").GetValue<bool>())
                                {
                                    ItemsActivator();
                                }
                            }
                        }
                        else if (Config.Item("Riven.UseEOnAA").GetValue<bool>() && args.SData.IsAutoAttack())
                        {
                            E.Cast(EscapeEEndPos);
                        }

                        if (Config.Item("Riven.UseEOnSpell").GetValue<bool>() && !args.SData.IsAutoAttack())
                        {
                            E.Cast(EscapeEEndPos);
                        }

                    }
                    else if (Config.Item("Riven.HarassOnEnemyFarm").GetValue<bool>() && (Player.Distance(unit) < E.Range + W.Range && Player.HealthPercent > Config.Item("Riven.MiniHpForHarass").GetValue<Slider>().Value))
                    {

                        if (E.IsReady() && CanCastE && Config.Item("Riven.EHarassOnEnemyFarm").GetValue<bool>() && Player.Distance(unit) > Player.AttackRange)
                        {
                            E.Cast(unit.ServerPosition);
                            if (R.IsReady() && !Player.HasBuff("RivenWindScarReady") && Config.Item("Riven.RHarassOnEnemyFarm").GetValue<bool>())
                            {
                                RLogic();
                            }
                            if (Config.Item("Riven.ItemsHarassOnEnemyFarm").GetValue<bool>())
                            {
                                ItemsActivator();
                            }
                        }
                    }
                }
            }
            #endregion

            #region JungleClear
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {

                var useW = Config.Item("Riven.UseWJungleClear").GetValue<bool>();
                var useE = Config.Item("Riven.UseEJungleClear").GetValue<bool>();

                var MinionN = MinionManager.GetMinions(600, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

                if (!MinionN.IsValidTarget() || MinionN == null)
                {
                    var useQLC = Config.Item("Riven.UseQLaneClear").GetValue<bool>();
                    var useWLC = Config.Item("Riven.UseWLaneClear").GetValue<bool>();
                    var useELC = Config.Item("Riven.UseELaneClear").GetValue<bool>();
                    var useItemsLC = Config.Item("Riven.UseTiamatHydraLaneClear").GetValue<bool>();

                    var CountQ = Config.Item("Riven.QLaneClearCount").GetValue<Slider>().Value;
                    var CountW = Config.Item("Riven.WLaneClearCount").GetValue<Slider>().Value;
                    var CountItems = Config.Item("Riven.TiamatHydraLaneClearCount").GetValue<Slider>().Value;

                    if (Config.Item("Riven.SafeLaneClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;

                    if (CanUseAA && CanMove)
                    {
                        var MinionE = MinionManager.GetMinions(Player.AttackRange + Player.Distance(Player.BBox.Minimum) + 50, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health).FirstOrDefault();
                        if (!(CastedQ || CastedW || CastedE || UsedAA))
                        {
                            CanCastQ = false;
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, MinionE);
                        }
                    }

                    if (useItemsLC)
                    {
                        var allMinionsEItems = MinionManager.GetMinions(Player.Position, E.Range + RavenousHydra.Range, MinionTypes.All, MinionTeam.Enemy);
                        var allMinionsItems = MinionManager.GetMinions(Player.Position, RavenousHydra.Range, MinionTypes.All, MinionTeam.Enemy);

                        if (allMinionsItems.Any() && UsedAA)
                        {
                            var farmAll = W.GetCircularFarmLocation(allMinionsItems, RavenousHydra.Range);

                            if (farmAll.MinionsHit >= CountItems)
                            {
                                if (Tiamat.IsReady())
                                {
                                    Tiamat.Cast();
                                }

                                if (RavenousHydra.IsReady())
                                {
                                    RavenousHydra.Cast();
                                }
                            }

                        }
                        else if (allMinionsEItems.Any() && useELC && E.IsReady())
                        {
                            var farmAll = W.GetCircularFarmLocation(allMinionsEItems, RavenousHydra.Range);

                            if (farmAll.MinionsHit >= CountItems && farmAll.Position.Distance(Player) > Player.AttackRange)
                            {
                                E.Cast(farmAll.Position, true);
                            }
                        }
                    }

                    if (useQLC && Q.IsReady())
                    {
                        var allMinionsEQ = MinionManager.GetMinions(Player.Position, Qfarm.Range + E.Range, MinionTypes.All, MinionTeam.Enemy);
                        var allMinionsQ = MinionManager.GetMinions(Player.Position, Qfarm.Range, MinionTypes.All, MinionTeam.Enemy);

                        if (allMinionsQ.Any() && CanCastQ)
                        {
                            var farmAll = Qfarm.GetLineFarmLocation(allMinionsQ, Qfarm.Width);
                            if (farmAll.MinionsHit >= CountQ)
                            {
                                Q.Cast(farmAll.Position, true);
                            }
                        }
                        else if (allMinionsEQ.Any() && useELC && E.IsReady())
                        {
                            var farmAll = Qfarm.GetLineFarmLocation(allMinionsEQ, Qfarm.Width);
                            if (farmAll.MinionsHit >= CountQ && farmAll.Position.Distance(Player) > Player.AttackRange)
                            {
                                E.Cast(farmAll.Position, true);
                            }
                        }
                    }

                    if (useWLC && W.IsReady())
                    {
                        var allMinionsEW = MinionManager.GetMinions(Player.Position, W.Range + E.Range, MinionTypes.All, MinionTeam.Enemy);
                        var allMinionsW = MinionManager.GetMinions(Player.Position, W.Range, MinionTypes.All, MinionTeam.Enemy);
                        if (allMinionsW.Any() && CanCastW)
                        {
                            var farmAll = W.GetCircularFarmLocation(allMinionsW, W.Range);
                            if (farmAll.MinionsHit >= CountW)
                            {
                                W.Cast(farmAll.Position, true);
                            }
                        }
                        else if (allMinionsEW.Any() && useELC && E.IsReady())
                        {
                            var farmAll = W.GetCircularFarmLocation(allMinionsEW, W.Range);
                            if (farmAll.MinionsHit >= CountW && farmAll.Position.Distance(Player) > Player.AttackRange)
                            {
                                E.Cast(farmAll.Position, true);
                            }
                        }
                    }
                }

                if (MinionN.IsValidTarget())
                {
                    if (Config.Item("Riven.SafeJungleClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;


                    if (Monsters.Contains(unit.BaseSkinName) && unit.IsEnemy && args.Target.IsMe)
                    {
                        if (useW && W.IsReady())
                        {
                            if (Player.Distance(unit) <= W.Range)
                            {
                                W.Cast(true);
                                return;
                            }

                        }

                        if (useE && E.IsReady())
                        {
                            E.Cast(unit.ServerPosition, true);
                            return;
                        }
                    }

                    JungleClear();
                }
            }
            #endregion

            #region SpellCheckBase

            if (!unit.IsMe)
                return;

            switch (args.SData.Name)
            {
                case "RivenTriCleave":
                    CastedQ = true;
                    CanMove = false;
                    lastCastQ = Utils.GameTimeTickCount;
                    CanCastQ = false;

                    if (targetCancel.IsValid && targetCancel.Distance(Player.ServerPosition) <= Q.Range)
                    {
                        if (Player.Distance(targetCancel) < Player.AttackRange / 2)
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add(100 + 100 - Game.Ping / 2,
                                () => EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Player.ServerPosition));
                        }
                        else

                            LeagueSharp.Common.Utility.DelayAction.Add(100 + 100 - Game.Ping / 2,
                                () => EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, targetCancel.ServerPosition));
                    }

                    break;
                case "RivenMartyr":
                    CastedW = true;
                    CanCastW = false;
                    lastCastW = Utils.GameTimeTickCount;

                    break;
                case "RivenFeint":
                    CastedE = true;
                    CanCastE = false;
                    lastCastE = Utils.GameTimeTickCount;

                    break;
                case "rivenizunablade":
                    CastedR2 = true;

                    if (Q.IsReady() && target.IsValidTarget())
                    {
                        Q.Cast(target.ServerPosition, true);
                    }

                    break;
            }

            if (args.SData.Name.Contains("Attack"))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if (!Config.Item("Riven.UseWCombo").GetValue<bool>() || !Config.Item("Riven.UseECombo").GetValue<bool>())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(50 + (int)(Player.AttackDelay * 100) + Game.Ping / 2 + Config.Item("Riven.QDelay").GetValue<Slider>().Value, delegate
                        {
                            ItemsActivator();
                        });
                    }
                }
            }

            if (!CastedQ && args.SData.Name.Contains("Attack"))
            {
                UsedAA = true;
                CanUseAA = false;
                CanCastQ = false;
                CanCastW = false;
                CanCastE = false;
                lastCastAA = Utils.GameTimeTickCount;
            }
            #endregion

        }
        #endregion

        #region AntiGapCloser
        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Config.Item("Riven.AutoWEGC").GetValue<bool>() && W.IsReady() && gapcloser.Sender.IsValidTarget(W.Range))
            {
                if (!gapcloser.Sender.HasBuff("Black Shield") && !gapcloser.Sender.HasBuff("Spell Shield") && !gapcloser.Sender.HasBuff("BansheesVeil"))
                {
                    W.Cast(true);
                }
            }
        }
        #endregion

        #region On Dash
        static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var useQ = Config.Item("Riven.UseQCombo").GetValue<bool>();
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (!sender.IsEnemy) return;

            if (sender.NetworkId == target.NetworkId)
            {

                if (useQ && Q.IsReady() && Player.GetBuffCount("RivenTriCleaveBuff") == 2 && args.EndPos.Distance(Player) <= Q.Range)
                {

                    var delay = (int)(args.EndTick - Game.Time - Q.Delay - 0.1f);
                    if (delay > 0)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(delay * 1000, () => Q.Cast(args.EndPos));
                    }
                    else
                    {
                        Q.Cast(args.EndPos);
                    }
                }
            }
        }
        #endregion

        #region Combo
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(ComboRange(), TargetSelector.DamageType.Physical);

            if (CanUseAA && CanMove)
            {
                if (!(CastedQ || CastedW || CastedE || UsedAA))
                {
                    if (target.Distance(Player.ServerPosition) <= Player.AttackRange + Player.Distance(Player.BBox.Minimum) + 50)
                    {
                        CanCastQ = false;
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                }
            }

            if (W.IsReady() && (CanCastW  || Utils.GameTimeTickCount - lastCastFlashCombo < 1000) && Config.Item("Riven.UseWCombo").GetValue<bool>() && target.Distance(Player.ServerPosition) <= W.Range + 25)
            {
                if (Config.Item("Riven.UseWCombo").GetValue<bool>() && !target.HasBuff("Black Shield") && !target.HasBuff("Spell Shield") && !target.HasBuff("BansheesVeil"))
                {
                    ItemsActivator();
                    W.Cast(true);
                    if (R.IsReady() && !Player.HasBuff("RivenWindScarReady"))
                    {
                        RLogic();
                    }
                }
            }

            AutoItemsActivator();

            if (Config.Item("Riven.UseFlashCombo1v1").GetValue<bool>())
            {
                FlashComboLogic1v1();
            }

            if (Config.Item("Riven.UseFlashComboTF").GetValue<bool>())
            {
                FlashComboLogicTF();
            }
            
            if (E.IsReady() && CanCastE && Config.Item("Riven.UseECombo").GetValue<bool>() && target.Distance(Player.ServerPosition) > Player.AttackRange)
            {
                if (Config.Item("Riven.UseECombo").GetValue<bool>() && target.Distance(Player) > Q.Range)
                {
                    if ((!Config.Item("Riven.GapCloseQ").GetValue<bool>() && target.Distance(Player) < E.Range + Q.Range) || (Config.Item("Riven.GapCloseQ").GetValue<bool>() && target.Distance(Player) < E.Range + Q.Range + Player.AttackRange))
                    {
                        E.Cast(target.ServerPosition);
                        if (R.IsReady() && !Player.HasBuff("RivenWindScarReady"))
                        {
                            RLogic();
                        }
                        ItemsActivator();
                    }
                }
            }

            if (Q.IsReady() && target.Distance(Player.ServerPosition) <= Q.Range)
            {

                if (CanCastQ)
                {
                    ItemsActivator();
                    Q.Cast(target.ServerPosition);
                    if (R.IsReady() && !Player.HasBuff("RivenWindScarReady"))
                    {
                        RLogic();
                    }
                }
            }

            if (target.Distance(Player.ServerPosition) > Q.Range)
            {
                if (Config.Item("Riven.GapCloseQ").GetValue<bool>())
                {
                    QGapCloseLogic();
                }
            }
        }
        #endregion

        #region Harass
        private static void Harass()
        {
            var targetHarass = TargetSelector.GetTarget(Q.Range + Player.AttackRange, TargetSelector.DamageType.Physical);
            if (Config.Item("Riven.HarassActive").GetValue<KeyBind>().Active)
            {

                if (Player.CountEnemiesInRange(1300) == 1)
                {
                    if (Config.Item("Riven.HarassOnInitiate").GetValue<bool>() && (Player.Distance(targetHarass) < Q.Range + Player.AttackRange && Player.HealthPercent > Config.Item("Riven.MiniHpForHarass").GetValue<Slider>().Value))
                    {

                        if (CanUseAA && CanMove)
                        {
                            if (!(CastedQ || CastedW || CastedE || UsedAA))
                            {
                                if (targetHarass.Distance(Player.ServerPosition) <= Player.AttackRange + Player.Distance(Player.BBox.Minimum) + 50)
                                {
                                    CanCastQ = false;
                                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetHarass);
                                }
                            }
                        }

                        if (Config.Item("Riven.ItemsHarassOnInitiate").GetValue<bool>())
                        {
                            AutoItemsActivator();
                        }

                        if (W.IsReady() && CanCastW && Config.Item("Riven.WHarassOnInitiate").GetValue<bool>() && targetHarass.Distance(Player.ServerPosition) <= W.Range)
                        {
                            if (!targetHarass.HasBuff("Black Shield") && !targetHarass.HasBuff("Spell Shield") && !targetHarass.HasBuff("BansheesVeil"))
                            {
                                if (Config.Item("Riven.ItemsHarassOnInitiate").GetValue<bool>())
                                {
                                    ItemsActivator();
                                }
                                W.Cast(true);
                                if (R.IsReady() && !Player.HasBuff("RivenWindScarReady") && Config.Item("Riven.RHarassOnInitiate").GetValue<bool>())
                                {
                                    RLogic();
                                }
                            }
                        }

                        if (Q.IsReady() && targetHarass.Distance(Player.ServerPosition) <= Q.Range && Config.Item("Riven.QHarassOnInitiate").GetValue<bool>())
                        {

                            if (CanCastQ)
                            {
                                if (Config.Item("Riven.ItemsHarassOnInitiate").GetValue<bool>())
                                {
                                    ItemsActivator();
                                }
                                Q.Cast(targetHarass.ServerPosition);
                                if (R.IsReady() && !Player.HasBuff("RivenWindScarReady") && Config.Item("Riven.RHarassOnInitiate").GetValue<bool>())
                                {
                                    RLogic();
                                }
                            }
                        }

                        if (targetHarass.Distance(Player.ServerPosition) > Q.Range && Config.Item("Riven.QHarassOnInitiate").GetValue<bool>())
                        {
                            if (!UsedAA)
                            {
                                if (Q.IsReady() && Utils.GameTimeTickCount - lastCastE >= 700)
                                {
                                    if (R.IsReady() && !Player.HasBuff("RivenWindScarReady") && Config.Item("Riven.RHarassOnInitiate").GetValue<bool>())
                                    {
                                        RLogic();
                                    }

                                    QGapCloseLogic();
                                }
                            }
                        }
                    }

                    else if (Config.Item("Riven.HarassOnEnemyFarm").GetValue<bool>() && (Player.Distance(targetHarass) < Q.Range + Player.AttackRange && Player.HealthPercent > Config.Item("Riven.MiniHpForHarass").GetValue<Slider>().Value))
                    {

                        if (CanUseAA && CanMove)
                        {
                            if (!(CastedQ || CastedW || CastedE || UsedAA))
                            {
                                if (targetHarass.Distance(Player.ServerPosition) <= Player.AttackRange + Player.Distance(Player.BBox.Minimum) + 50)
                                {
                                    CanCastQ = false;
                                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, targetHarass);
                                }
                            }
                        }

                        if (Config.Item("Riven.ItemsHarassOnEnemyFarm").GetValue<bool>())
                        {
                            AutoItemsActivator();
                        }

                        if (W.IsReady() && CanCastW && Config.Item("Riven.WHarassOnEnemyFarm").GetValue<bool>() && targetHarass.Distance(Player.ServerPosition) <= W.Range)
                        {
                            if (!targetHarass.HasBuff("Black Shield") && !targetHarass.HasBuff("Spell Shield") && !targetHarass.HasBuff("BansheesVeil"))
                            {
                                if (Config.Item("Riven.ItemsHarassOnEnemyFarm").GetValue<bool>())
                                {
                                    ItemsActivator();
                                }
                                W.Cast(true);
                                if (R.IsReady() && !Player.HasBuff("RivenWindScarReady") && Config.Item("Riven.RHarassOnEnemyFarm").GetValue<bool>())
                                {
                                    RLogic();
                                }
                            }
                        }

                        if (Q.IsReady() && targetHarass.Distance(Player.ServerPosition) <= Q.Range && Config.Item("Riven.QHarassOnEnemyFarm").GetValue<bool>())
                        {

                            if (CanCastQ)
                            {
                                if (Config.Item("Riven.ItemsHarassOnEnemyFarm").GetValue<bool>())
                                {
                                    ItemsActivator();
                                }
                                Q.Cast(targetHarass.ServerPosition);
                                if (R.IsReady() && !Player.HasBuff("RivenWindScarReady") && Config.Item("Riven.RHarassOnEnemyFarm").GetValue<bool>())
                                {
                                    RLogic();
                                }
                            }
                        }

                        if (targetHarass.Distance(Player.ServerPosition) > Q.Range && Config.Item("Riven.QHarassOnEnemyFarm").GetValue<bool>())
                        {
                            if (!UsedAA)
                            {
                                if (Q.IsReady() && Utils.GameTimeTickCount - lastCastE >= 700)
                                {
                                    if (R.IsReady() && !Player.HasBuff("RivenWindScarReady") && Config.Item("Riven.RHarassOnEnemyFarm").GetValue<bool>())
                                    {
                                        RLogic();
                                    }

                                    QGapCloseLogic();
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region JungleClear
        public static void JungleClear()
        {
            var useQ = Config.Item("Riven.UseQJungleClear").GetValue<bool>();
            var useItems = Config.Item("Riven.UseTiamatHydraJungleClear").GetValue<bool>();

            var MinionN = MinionManager.GetMinions(600, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (Config.Item("Riven.SafeJungleClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;

            if (CanUseAA && CanMove)
            {
                if (!(CastedQ || CastedW || CastedE || UsedAA))
                {
                    if (MinionN.Distance(Player.ServerPosition) <= Player.AttackRange + Player.Distance(Player.BBox.Minimum) + 50)
                    {
                        CanCastQ = false;
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, MinionN);
                    }
                }
            }

            if (useQ && Q.IsReady() && MinionN.Distance(Player.ServerPosition) <= Q.Range)
            {

                if (CanCastQ)
                {
                    var allMonsterQ = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                    var farmAll = Q.GetCircularFarmLocation(allMonsterQ, Q.Width);
                    if (farmAll.MinionsHit >= 1)
                    {
                        Q.Cast(farmAll.Position, true);
                    }
                }
            }

            if (!CanUseAA && useItems)
            {
                if (Tiamat.IsReady() && MinionN.Distance(Player.ServerPosition) <= Tiamat.Range)
                {
                    Tiamat.Cast();
                }

                if (RavenousHydra.IsReady() && MinionN.Distance(Player.ServerPosition) <= RavenousHydra.Range)
                {
                    RavenousHydra.Cast();
                }
            }



        }
        #endregion

        #region Interupt Spell List
        public static double ShouldUse(string SpellName)
        {
            if (SpellName == "KatarinaR")
                return 0;
            if (SpellName == "AlZaharNetherGrasp")
                return 0;
            if (SpellName == "LuxMaliceCannon")
                return 0;
            if (SpellName == "MissFortuneBulletTime")
                return 0;
            if (SpellName == "CaitlynPiltoverPeacemaker")
                return 0;
            if (SpellName == "EzrealTrueshotBarrage")
                return 0;
            if (SpellName == "InfiniteDuress")
                return 0;
            if (SpellName == "VelkozR")
                return 0;
            if (SpellName == "XerathLocusOfPower2")
                return 0;
            if (SpellName == "Drain")
                return 0;
            if (SpellName == "Crowstorm")
                return 0;
            if (SpellName == "ReapTheWhirlwind")
                return 0;
            if (SpellName == "FallenOne")
                return 0;
            if (SpellName == "JudicatorIntervention")
                return 0;
            if (SpellName == "KennenShurikenStorm")
                return 0;
            if (SpellName == "LucianR")
                return 0;
            if (SpellName == "SoulShackles")
                return 0;
            if (SpellName == "NamiQ")
                return 0;
            if (SpellName == "AbsoluteZero")
                return 0;
            if (SpellName == "Pantheon_GrandSkyfall_Jump")
                return 0;
            if (SpellName == "RivenMartyr")
                return 0;
            if (SpellName == "RivenTriCleave_03")
                return 0;
            if (SpellName == "RunePrison")
                return 0;
            if (SpellName == "SkarnerImpale")
                return 0;
            if (SpellName == "UndyingRage")
                return 0;
            if (SpellName == "VarusQ")
                return 0;
            if (SpellName == "MonkeyKingSpinToWin")
                return 0;
            if (SpellName == "YasuoRKnockUpComboW")
                return 0;
            if (SpellName == "ZacE")
                return 0;
            if (SpellName == "ZacR")
                return 0;
            if (SpellName == "UrgotSwap2")
                return 0;
            return -1;
        }
        # endregion

        #region PotionManager
        public static void PotionManager()
        {
            if (Player.Level == 1 && Player.CountEnemiesInRange(1000) == 1 && Player.Health >= Player.MaxHealth * 0.35) return;
            if (Player.Level == 1 && Player.CountEnemiesInRange(1000) == 2 && Player.Health >= Player.MaxHealth * 0.50) return;

            if (Config.Item("Riven.AutoPotion").GetValue<bool>() && !Player.InFountain() && !Player.IsRecalling() && !Player.IsDead)
            {
                #region BiscuitofRejuvenation
                if (BiscuitofRejuvenation.IsReady() && !Player.HasBuff("ItemMiniRegenPotion") && !Player.HasBuff("ItemCrystalFlask"))
                {

                    if (Player.MaxHealth > Player.Health + 170 && Player.MaxMana > Player.Mana + 10 && Player.CountEnemiesInRange(1000) > 0 &&
                        Player.Health < Player.MaxHealth * 0.75)
                    {
                        BiscuitofRejuvenation.Cast();
                    }

                    else if (Player.MaxHealth > Player.Health + 170 && Player.MaxMana > Player.Mana + 10 && Player.CountEnemiesInRange(1000) == 0 &&
                        Player.Health < Player.MaxHealth * 0.6)
                    {
                        BiscuitofRejuvenation.Cast();
                    }

                }
                #endregion

                #region HealthPotion
                else if (HealthPotion.IsReady() && !Player.HasBuff("RegenerationPotion") && !Player.HasBuff("ItemCrystalFlask"))
                {

                    if (Player.MaxHealth > Player.Health + 150 && Player.CountEnemiesInRange(1000) > 0 &&
                        Player.Health < Player.MaxHealth * 0.75)
                    {
                        HealthPotion.Cast();
                    }

                    else if (Player.MaxHealth > Player.Health + 150 && Player.CountEnemiesInRange(1000) == 0 &&
                        Player.Health < Player.MaxHealth * 0.6)
                    {
                        HealthPotion.Cast();
                    }

                }
                #endregion

                #region CrystallineFlask
                else if (CrystallineFlask.IsReady() && !Player.HasBuff("ItemCrystalFlask") && !Player.HasBuff("RegenerationPotion") && !Player.HasBuff("FlaskOfCrystalWater") && !Player.HasBuff("ItemMiniRegenPotion"))
                {

                    if (Player.MaxHealth > Player.Health + 120 && Player.MaxMana > Player.Mana + 60 && Player.CountEnemiesInRange(1000) > 0 &&
                        (Player.Health < Player.MaxHealth * 0.85 || Player.Mana < Player.MaxMana * 0.65))
                    {
                        CrystallineFlask.Cast();
                    }

                    else if (Player.MaxHealth > Player.Health + 120 && Player.MaxMana > Player.Mana + 60 && Player.CountEnemiesInRange(1000) == 0 &&
                        (Player.Health < Player.MaxHealth * 0.7 || Player.Mana < Player.MaxMana * 0.5))
                    {
                        CrystallineFlask.Cast();
                    }

                }
                #endregion

                #region ManaPotion
                else if (ManaPotion.IsReady() && !Player.HasBuff("FlaskOfCrystalWater") && !Player.HasBuff("ItemCrystalFlask"))
                {

                    if (Player.MaxMana > Player.Mana + 100 && Player.CountEnemiesInRange(1000) > 0 &&
                        Player.Mana < Player.MaxMana * 0.7)
                    {
                        ManaPotion.Cast();
                    }

                    else if (Player.MaxMana > Player.Mana + 100 && Player.CountEnemiesInRange(1000) == 0 &&
                        Player.Mana < Player.MaxMana * 0.4)
                    {
                        ManaPotion.Cast();
                    }

                }
                #endregion
            }
        }
        #endregion

        #region DrawingRange
        public static void Drawing_OnDraw(EventArgs args)
        {

            foreach (var spell in SpellList)
            {
                var menuItem = Config.Item(spell.Slot + "Range").GetValue<Circle>();
                if (menuItem.Active && (spell.Slot != SpellSlot.R || R.Level > 0))
                    Render.Circle.DrawCircle(Player.Position, spell.Range, menuItem.Color);
            }

            if (Config.Item("DrawOrbwalkTarget").GetValue<bool>())
            {
                var orbT = Orbwalker.GetTarget();
                if (orbT.IsValidTarget())
                    Render.Circle.DrawCircle(orbT.Position, 100, System.Drawing.Color.Pink);
            }

        }
        #endregion

        #region Up Spell
        private static void LevelUpSpells()
        {
            int qL = Player.Spellbook.GetSpell(SpellSlot.Q).Level;
            int wL = Player.Spellbook.GetSpell(SpellSlot.W).Level;
            int eL = Player.Spellbook.GetSpell(SpellSlot.E).Level;
            int rL = Player.Spellbook.GetSpell(SpellSlot.R).Level;

            if (qL + wL + eL + rL < ObjectManager.Player.Level)
            {
                if (qL + wL + eL + rL == 0)
                {
                    switch (Config.Item("Riven.SpellLV1").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 1)
                {
                    switch (Config.Item("Riven.SpellLV2").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 2)
                {
                    switch (Config.Item("Riven.SpellLV3").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 3)
                {
                    switch (Config.Item("Riven.SpellLV4").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 4)
                {
                    switch (Config.Item("Riven.SpellLV5").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 5)
                {
                    switch (Config.Item("Riven.SpellLV6").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 6)
                {
                    switch (Config.Item("Riven.SpellLV7").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 7)
                {
                    switch (Config.Item("Riven.SpellLV8").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 8)
                {
                    switch (Config.Item("Riven.SpellLV9").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 9)
                {
                    switch (Config.Item("Riven.SpellLV10").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 10)
                {
                    switch (Config.Item("Riven.SpellLV11").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 11)
                {
                    switch (Config.Item("Riven.SpellLV12").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 12)
                {
                    switch (Config.Item("Riven.SpellLV13").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 13)
                {
                    switch (Config.Item("Riven.SpellLV14").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 14)
                {
                    switch (Config.Item("Riven.SpellLV15").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 15)
                {
                    switch (Config.Item("Riven.SpellLV16").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 16)
                {
                    switch (Config.Item("Riven.SpellLV17").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 17)
                {
                    switch (Config.Item("Riven.SpellLV18").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                    }
                    return;
                }
            }
            

            
        }
        #endregion

        #region PassiveDamageCheck
        public static float PassiveDamageCheck()
        {
            double damage = 0;
            if (Player.Level >= 1 && Player.Level < 3)
            {
                damage += (float)0.2;
            }

            if (Player.Level >= 3 && Player.Level < 6)
            {
                damage += (float)0.25;
            }

            if (Player.Level >= 6 && Player.Level < 9)
            {
                damage += (float)0.3;
            }

            if (Player.Level >= 9 && Player.Level < 12)
            {
                damage += (float)0.35;
            }

            if (Player.Level >= 12 && Player.Level < 15)
            {
                damage += (float)0.4;
            }

            if (Player.Level >= 15 && Player.Level < 18)
            {
                damage += (float)0.45;
            }

            if (Player.Level == 18)
            {
                damage += (float)0.5;
            }

            return (float)damage;
        }
        #endregion

        #region PlayerDamage
        public static float getComboDamage(AIHeroClient target)
        {
            float damage = 0f;
            if (Config.Item("Riven.UseQCombo").GetValue<bool>())
            {
                if (Q.IsReady())
                {
                    if (Player.GetBuffCount("RivenTriCleaveBuff") == 0)
                    {
                        damage += Q.GetDamage(target) * 3f;
                        damage += (Player.TotalAttackDamage * PassiveDamageCheck() + (float)Player.GetAutoAttackDamage(target)) * 3f;
                    }
                    if (Player.GetBuffCount("RivenTriCleaveBuff") == 1)
                    {
                        damage += Q.GetDamage(target) * 2f;
                        damage += (Player.TotalAttackDamage * PassiveDamageCheck() + (float)Player.GetAutoAttackDamage(target)) * 2f;
                    }
                    if (Player.GetBuffCount("RivenTriCleaveBuff") == 2)
                    {
                        damage += Q.GetDamage(target) * 1f;
                        damage += (Player.TotalAttackDamage * PassiveDamageCheck() + (float)Player.GetAutoAttackDamage(target));
                    }
                }
            }
            if (Config.Item("Riven.UseWCombo").GetValue<bool>())
            {
                if (W.IsReady())
                {
                    damage += W.GetDamage(target);
                    damage += (float)Player.GetAutoAttackDamage(target);
                    damage += (Player.TotalAttackDamage * PassiveDamageCheck() + (float)Player.GetAutoAttackDamage(target));
                }
            }
            if (Config.Item("Riven.UseECombo").GetValue<bool>())
            {
                if (E.IsReady())
                {
                    damage += (Player.TotalAttackDamage * PassiveDamageCheck() + (float)Player.GetAutoAttackDamage(target));
                }
            }
            if (Config.Item("Riven.UseRCombo").GetValue<bool>())
            {
                if (R.IsReady() || Player.HasBuff("RivenWindScarReady"))
                {
                    damage += R.GetDamage(target);
                }
            }

            if (Ignite.Slot != SpellSlot.Unknown)
            {
                damage += (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            }

            return damage;
        }


        public static float getFlashComboDamage(AIHeroClient target)
        {
            float damage = 0f;
            if (Config.Item("Riven.UseQCombo").GetValue<bool>())
            {
                if (Q.IsReady())
                {
                    damage += Q.GetDamage(target);
                    damage += (Player.TotalAttackDamage * PassiveDamageCheck() + (float)Player.GetAutoAttackDamage(target));
                }
            }
            if (Config.Item("Riven.UseWCombo").GetValue<bool>())
            {
                if (W.IsReady())
                {
                    damage += W.GetDamage(target);
                    damage += (Player.TotalAttackDamage * PassiveDamageCheck() + (float)Player.GetAutoAttackDamage(target));
                }
            }
            if (Config.Item("Riven.UseRCombo").GetValue<bool>())
            {
                if (R.IsReady() || Player.HasBuff("RivenWindScarReady"))
                {
                    damage += R.GetDamage(target);
                    damage += (Player.TotalAttackDamage * PassiveDamageCheck() + (float)Player.GetAutoAttackDamage(target));
                }
            }

            if (Ignite.Slot != SpellSlot.Unknown)
            {
                damage += (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            }

            return damage;
        }

        public static float getComboDamageNoUlt(AIHeroClient target)
        {
            float damage = 0f;
            if (Config.Item("Riven.UseQCombo").GetValue<bool>())
            {
                if (Q.IsReady())
                {
                    if (Player.GetBuffCount("RivenTriCleaveBuff") == 0)
                    {
                        damage += Q.GetDamage(target) * 3f;
                        damage += (Player.TotalAttackDamage * PassiveDamageCheck() + (float)Player.GetAutoAttackDamage(target)) * 3f;
                    }
                    if (Player.GetBuffCount("RivenTriCleaveBuff") == 1)
                    {
                        damage += Q.GetDamage(target) * 2f;
                        damage += (Player.TotalAttackDamage * PassiveDamageCheck() + (float)Player.GetAutoAttackDamage(target)) * 2f;
                    }
                    if (Player.GetBuffCount("RivenTriCleaveBuff") == 2)
                    {
                        damage += Q.GetDamage(target) * 1f;
                        damage += (Player.TotalAttackDamage * PassiveDamageCheck() + (float)Player.GetAutoAttackDamage(target));
                    }
                }
            }
            if (Config.Item("Riven.UseWCombo").GetValue<bool>())
            {
                if (W.IsReady())
                {
                    damage += W.GetDamage(target);
                    damage += (float)Player.GetAutoAttackDamage(target);
                    damage += (Player.TotalAttackDamage * PassiveDamageCheck() + (float)Player.GetAutoAttackDamage(target));
                }
            }
            if (Config.Item("Riven.UseECombo").GetValue<bool>())
            {
                if (E.IsReady())
                {
                    damage += (Player.TotalAttackDamage * PassiveDamageCheck() + (float)Player.GetAutoAttackDamage(target));
                }
            }

            if (Ignite.Slot != SpellSlot.Unknown)
            {
                damage += (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            }

            return damage;
        }


        public static float getMiniComboDamage(AIHeroClient target)
        {
            float damage = 0f;
            if (Config.Item("Riven.UseQCombo").GetValue<bool>())
            {
                if (Q.IsReady())
                {
                    damage += Q.GetDamage(target) * 1f;
                    damage += (Player.TotalAttackDamage * PassiveDamageCheck() + (float)Player.GetAutoAttackDamage(target));
                }
            }
            if (Config.Item("Riven.UseWCombo").GetValue<bool>())
            {
                if (W.IsReady())
                {
                    damage += W.GetDamage(target);
                    damage += (float)Player.GetAutoAttackDamage(target);
                    damage += (Player.TotalAttackDamage * PassiveDamageCheck() + (float)Player.GetAutoAttackDamage(target));
                }
            }

            if (Ignite.Slot != SpellSlot.Unknown)
            {
                damage += (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            }

            return damage;
        }
        #endregion

        #region Items

        #region ItemsActivator
        private static void ItemsActivator()
        {
            if (Config.Item("Riven.useTiamat").GetValue<bool>() && Tiamat.IsReady() && Player.CountEnemiesInRange(Tiamat.Range) > 0)
            {
                Tiamat.Cast();
            }

            if (Config.Item("Riven.useRavenousHydra").GetValue<bool>() && RavenousHydra.IsReady() && Player.CountEnemiesInRange(RavenousHydra.Range) > 0)
            {
                RavenousHydra.Cast();
            }

            if (Config.Item("Riven.useEntropy").GetValue<bool>() && Entropy.IsReady() && Player.CountEnemiesInRange(Player.AttackRange) > 0)
            {
                Entropy.Cast();
            }
        }
        #endregion

        #region AutoItemsActivator
        private static void AutoItemsActivator()
        {
            var target = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical);

            if (Config.Item("Riven.useYoumuusGhostblade").GetValue<bool>() && YoumuusGhostblade.IsReady())
            {
                if (target.IsValidTarget(1000) && (Player.Distance(target) > Player.AttackRange || Player.Distance(target) < 300))
                {
                    YoumuusGhostblade.Cast();
                }

                else if (target.IsValidTarget(1000) && Player.HealthPercent <= Config.Item("Riven.MinimumHPtoYoumuusGhostblade").GetValue<Slider>().Value)
                {
                    YoumuusGhostblade.Cast();
                }

            }

            if (Config.Item("Riven.useBilgewaterCutlass").GetValue<bool>() && BilgewaterCutlass.IsReady())
            {
                if (target.IsValidTarget(BilgewaterCutlass.Range) && ((target.IsFacing(Player)) || (!target.IsFacing(Player) && Player.Distance(target) > 400)))
                {
                    BilgewaterCutlass.Cast(target);
                }

                else if (target.IsValidTarget(BilgewaterCutlass.Range) && Player.HealthPercent <= Config.Item("Riven.MinimumHPtoBilgewaterCutlass").GetValue<Slider>().Value)
                {
                    BilgewaterCutlass.Cast(target);
                }

            }

            if (Config.Item("Riven.useBladeoftheRuinedKing").GetValue<bool>() && BladeoftheRuinedKing.IsReady())
            {
                if (target.IsValidTarget(BladeoftheRuinedKing.Range) && ((target.IsFacing(Player)) || (!target.IsFacing(Player) && Player.Distance(target) > 400)))
                {
                    BladeoftheRuinedKing.Cast(target);
                    return;
                }

                else if (target.IsValidTarget(BladeoftheRuinedKing.Range) && Player.HealthPercent <= Config.Item("Riven.MinimumHPtoBladeoftheRuinedKing").GetValue<Slider>().Value)
                {
                    BladeoftheRuinedKing.Cast(target);
                    return;
                }

            }
        }
        #endregion

        #endregion

        #region SpellStatus
        private static void SpellStatus()
        {

            if (CastedQ)
            {
                if (Utils.GameTimeTickCount - lastCastQ >= (int)(Player.AttackCastDelay * 1000) + 310)
                {
                    CastedQ = false;
                    CanMove = true;
                    CanUseAA = true;
                }
            }

            if (!CanCastQ && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var target = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical);
                if (target.Distance(Player) > target.AttackRange && Player.GetBuffCount("RivenTriCleaveBuff") == 2 && !E.IsReady() && Player.Distance(target) < Q.Range && getComboDamage(target) > target.Health)
                {
                    CanCastQ = true;
                }
            }

            if (CastedW)
            {
                if (Utils.GameTimeTickCount - lastCastW >= 270)
                {
                    CastedW = false;
                    CanMove = true;
                    CanUseAA = true;
                }
            }

            if (CastedE)
            {
                if (Utils.GameTimeTickCount - lastCastE >= 300)
                {
                    CastedE = false;
                    CanMove = true;
                }
            }

            if (UsedAA)
            {
                if (Utils.GameTimeTickCount - lastCastAA >= (Player.AttackDelay * 100) + 100 - Game.Ping + Config.Item("Riven.QDelay").GetValue<Slider>().Value)
                {
                    UsedAA = false;
                    CanMove = true;
                    CanCastQ = true;
                    CanCastE = true;
                    CanCastW = true;
                }
            }

            if (!CanCastW && W.IsReady())
            {
                if (!(UsedAA || CastedQ || CastedE))
                {
                    CanCastW = true;
                }
            }

            if (!CanCastE && E.IsReady())
            {
                if (!(UsedAA || CastedQ || CastedW))
                {
                    CanCastE = true;
                }
            }

            if (!CanUseAA)
            {
                if (!(CastedQ || CastedW || CastedE || CastedR2))
                {
                    if (Utils.GameTimeTickCount - lastCastAA >= 1000)
                    {
                        CanUseAA = true;
                    }
                }
            }

            if (!CanMove)
            {
                if (!(CastedQ || CastedW || CastedE || CastedR2))
                {
                    if (Utils.GameTimeTickCount - lastCastAA >= 1100)
                    {
                        CanMove = true;
                    }
                }
            }
        }

        #endregion

        #region OnNewPath
        private static void Obj_AI_Base_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.IsMe && !args.IsDash)
            {
                if (CastedQ)
                {
                    CastedQ = false;
                    CanUseAA = true;
                    CanMove = true;
                }
            }
        }
        #endregion

        #region QGapCloseLogic
        public static void QGapCloseLogic()
        {
            var target = TargetSelector.GetTarget(ComboRange(), TargetSelector.DamageType.Physical);

            if (target.Distance(Player.ServerPosition) > Player.AttackRange + Player.Distance(Player.BBox.Minimum) + 100)
            {
                if (getComboDamage(target) > target.Health)
                {
                    if (!E.IsReady() && Utils.GameTimeTickCount - lastCastQ >= Config.Item("Riven.GapCloserDelay").GetValue<Slider>().Value * 10 && !UsedAA)
                    {
                        if (Q.IsReady() && Utils.GameTimeTickCount - lastCastE >= 700)
                        {
                            if (R.IsReady() && !Player.HasBuff("RivenWindScarReady"))
                            {
                                RLogic();
                            }

                            Q.Cast(target.ServerPosition);
                        }
                    }
                }
            }
           
        }
        #endregion

        #region RLogic
        public static void RLogic()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (Player.CountEnemiesInRange(1300) > 1)
            {
                R.Cast();
            }

            if (Player.CountEnemiesInRange(1300) == 1)
            {
                if (Player.CountAlliesInRange(1300) >= 1 + 1)
                {
                    if (getComboDamageNoUlt(target) > target.Health)
                    {
                        if (Player.HealthPercent < 50)
                        {
                            if (getMiniComboDamage(target) < target.Health)
                            {
                                R.Cast();
                            }
                        }
                    }
                }
                if (Player.CountAlliesInRange(1300) == 0 + 1 && (!Config.Item("Riven.R1ModeHidden").GetValue<bool>() || getComboDamage(target) > target.Health))
                {
                    switch (Config.Item("Riven.R1Mode").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                if (Game.Time < 1300f)
                                {
                                    R.Cast();
                                }
                                if (Game.Time >= 1300f)
                                {
                                    if (getMiniComboDamage(target) < target.Health)
                                    {
                                        R.Cast();
                                    }
                                }
                                break;
                            }

                        case 1:
                            {
                                if (getMiniComboDamage(target) < target.Health)
                                {
                                    R.Cast();
                                }
                                break;
                            }
                    }

                }
            }
        }
        #endregion

        #region FleeLogic
        public static void FleeLogic()
        {
            
            if (Config.Item("Riven.UseWFleeMode").GetValue<bool>())
            {
                if (Player.CountEnemiesInRange(W.Range) >= Config.Item("Riven.UseWFleeModeCount").GetValue<Slider>().Value)
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                    if (Player.CountEnemiesInRange(W.Range) == 1 && !target.HasBuff("Black Shield") && !target.HasBuff("Spell Shield") && !target.HasBuff("BansheesVeil"))
                    {
                        W.Cast(true);
                    }
                    else
                        W.Cast(true);
                }
            }

            if (Config.Item("Riven.UseYoumuuFleeMode").GetValue<bool>() && Player.CountEnemiesInRange(1300) >= Config.Item("Riven.UseYoumuuFleeModeEnemyCount").GetValue<Slider>().Value)
            {
                if (YoumuusGhostblade.IsReady())
                {
                    YoumuusGhostblade.Cast();
                }
            }

            if (Config.Item("Riven.UseEFleeMode").GetValue<bool>())
            {
                if (Config.Item("Riven.UseQFleeMode").GetValue<bool>() && Q.IsReady())
                {

                    switch (Config.Item("Riven.FleeMode").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {
                                E.Cast(Game.CursorPos, true);
                                break;
                            }

                        case 1:
                            {
                                if (Player.GetBuffCount("RivenTriCleaveBuff") == 1)
                                {
                                    E.Cast(Game.CursorPos, true);
                                }
                                break;
                            }

                        case 2:
                            {
                                if (Player.GetBuffCount("RivenTriCleaveBuff") == 2)
                                {
                                    E.Cast(Game.CursorPos, true);
                                }
                                break;
                            }
                    }

                }
                else
                    E.Cast(Game.CursorPos, true);
            }

            if (Config.Item("Riven.UseQFleeMode").GetValue<bool>())
            {
                    Q.Cast(Game.CursorPos, true);
            }


        }
        #endregion

        #region KillSteal
        public static void KillSteal()
        {
            var target = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical);

            if (Config.Item("Riven.R2Mode").GetValue<StringList>().SelectedIndex == 1 && target.IsValidTarget(R.Range) && Player.HasBuff("RivenWindScarReady") && !target.HasBuff("SionPassiveZombie") && !target.HasBuff("Udying Rage") && !target.HasBuff("JudicatorIntervention") && !target.HasBuff("Spell Shield") && !target.HasBuff("BansheesVeil"))
            {
                R.CastIfWillHit(target, Config.Item("Riven.MiniCountR2").GetValue<Slider>().Value);

                if ((R.GetDamage(target) / target.MaxHealth * 100) > target.Health / target.MaxHealth * 50)
                {
                    R.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                }

                if (Q.IsReady() && target.Health <= (float)(R.GetDamage(target) + Player.GetAutoAttackDamage(target) * 2 + Q.GetDamage(target)) * 1.2)
                {
                    if (target.Distance(Player.ServerPosition) <= Player.AttackRange + 180)
                    {
                        R.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                }
            }

            var useIgniteKS = Config.Item("Riven.UseIgniteKS").GetValue<bool>();
            var useQKS = Config.Item("Riven.UseQKS").GetValue<bool>();
            var use3rdQKS = Config.Item("Riven.Use3rdQKS").GetValue<bool>();
            var useWKS = Config.Item("Riven.UseWKS").GetValue<bool>();

            var use3rdQKSCount = Config.Item("Riven.Use3rdQKSCount").GetValue<Slider>().Value;
            var useWKSCount = Config.Item("Riven.UseWKSCount").GetValue<Slider>().Value;

            foreach (var targets in ObjectManager.Get<AIHeroClient>().Where(t => !t.IsMe && t.Team != ObjectManager.Player.Team))
            {
                if (!target.HasBuff("SionPassiveZombie") && !target.HasBuff("Udying Rage") && !target.HasBuff("JudicatorIntervention"))
                {
                    if (useIgniteKS && Ignite.Slot != SpellSlot.Unknown && target.Health < Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) <= Ignite.Range && !target.IsDead && target.IsValidTarget())
                    {
                        Ignite.Cast(target, true);
                        return;
                    }

                    if (Player.HasBuff("RivenWindScarReady") && R.GetDamage(targets) > targets.Health && targets.Distance(Player) < R.Range && !target.HasBuff("BansheesVeil"))
                    {
                        R.CastIfHitchanceEquals(targets, HitChance.VeryHigh, true);
                    }


                    if (useQKS && Q.IsReady() && target.Health < Q.GetDamage(target) && Player.Distance(target) <= Q.Range && !target.IsDead && target.IsValidTarget())
                    {
                        if (Player.GetBuffCount("RivenTriCleaveBuff") != 2 || (use3rdQKS && Player.CountEnemiesInRange(1300) <= use3rdQKSCount))
                        {
                            Q.Cast(target.ServerPosition, true);
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                            return;
                        }
                    }

                    if (useWKS && W.IsReady() && target.Health < W.GetDamage(target) && Player.Distance(target) <= W.Range && !target.IsDead && target.IsValidTarget() && Player.CountEnemiesInRange(1300) <= useWKSCount && !target.HasBuff("BansheesVeil"))
                    {
                        W.Cast(true);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        return;
                    }
                }

            }
        }
        #endregion

        #region FlashComboLogic1v1
        public static void FlashComboLogic1v1()
        {
            var target = TargetSelector.GetTarget(E.Range + 425 + W.Range, TargetSelector.DamageType.Physical);


            if (Player.CountEnemiesInRange(1300) == 1)
            {
                if (target.IsValidTarget() && Player.Distance(target) > E.Range + Q.Range && getFlashComboDamage(target) > target.Health)
                {
                    if (FlashSlot.IsReady() && E.IsReady() && W.IsReady())
                    {
                        lastCastFlashCombo = Utils.GameTimeTickCount;
                        E.Cast(target.ServerPosition, true);
                        LeagueSharp.Common.Utility.DelayAction.Add(10 + Game.Ping / 2, () => R.Cast(target.ServerPosition));
                        LeagueSharp.Common.Utility.DelayAction.Add(250 + Game.Ping / 2, () => Player.Spellbook.CastSpell(FlashSlot, target.ServerPosition));
                    }
                }
            }
        }
        #endregion

        #region FlashComboLogicTF
        public static void FlashComboLogicTF()
        {
            var target = TargetSelector.GetTarget(E.Range + 425 + W.Range, TargetSelector.DamageType.Physical);


            if (Player.CountEnemiesInRange(1300) > 1)
            {
                if (target.IsValidTarget() && Player.Distance(target) > E.Range + Q.Range && getFlashComboDamage(target) > target.Health && target.CountAlliesInRange(W.Range - 10) >= Config.Item("Riven.EnemyCountFlashComboTF").GetValue<Slider>().Value - 1)
                {
                    if (FlashSlot.IsReady() && E.IsReady() && W.IsReady())
                    {
                        lastCastFlashCombo = Utils.GameTimeTickCount;
                        E.Cast(target.ServerPosition, true);
                        LeagueSharp.Common.Utility.DelayAction.Add(10 + Game.Ping / 2, () => R.Cast(target.ServerPosition));
                        LeagueSharp.Common.Utility.DelayAction.Add(250 + Game.Ping / 2, () => Player.Spellbook.CastSpell(FlashSlot, target.ServerPosition));
                    }
                }
            }
        }
        #endregion
    }

}
