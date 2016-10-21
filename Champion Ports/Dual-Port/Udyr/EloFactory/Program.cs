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

using EloBuddy; namespace EloFactory_Udyr
{
    internal class Program
    {
        public const string ChampionName = "Udyr";

        public static Orbwalking.Orbwalker Orbwalker;

        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;

        public static float QMANA;
        public static float WMANA;
        public static float EMANA;
        public static float RMANA;

        public static Items.Item HealthPotion = new Items.Item(2003, 0);
        public static Items.Item ManaPotion = new Items.Item(2004, 0);
        public static Items.Item CrystallineFlask = new Items.Item(2041, 0);
        public static Items.Item BiscuitofRejuvenation = new Items.Item(2010, 0);

        public static Items.Item TearoftheGoddess = new Items.Item(3070, 0);
        public static Items.Item TearoftheGoddessCrystalScar = new Items.Item(3073, 0);
        public static Items.Item ArchangelsStaff = new Items.Item(3003, 0);
        public static Items.Item ArchangelsStaffCrystalScar = new Items.Item(3007, 0);
        public static Items.Item Manamune = new Items.Item(3004, 0);
        public static Items.Item ManamuneCrystalScar = new Items.Item(3008, 0);
        public static int Muramana = 3042;

        public static Menu Config;

        private static AIHeroClient Player;

        public static int[] abilitySequence;
        public static int[] abilitySequence2;
        public static int qOff = 0, wOff = 0, eOff = 0, rOff = 0;

        public static void Game_OnGameLoad()
        {
            Player = ObjectManager.Player;

            if (Player.ChampionName != ChampionName) return;

            Q = new Spell(SpellSlot.Q, Player.AttackRange * 2);
            W = new Spell(SpellSlot.W, Player.AttackRange * 2);
            E = new Spell(SpellSlot.E, Player.AttackRange * 2);
            R = new Spell(SpellSlot.R, Player.AttackRange * 2);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            abilitySequence = new int[] { 4, 1, 3, 4, 4, 2, 4, 3, 4, 3, 3, 3, 1, 1, 1, 1, 2, 2 };
            abilitySequence2 = new int[] { 1, 2, 3, 1, 1, 2, 1, 2, 1, 2, 2, 3, 3, 4, 3, 3, 4, 4 };

            Config = new Menu(ChampionName + " By LuNi", ChampionName + " By LuNi", true);

            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("Udyr.UseQCombo", "Use Q In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Udyr.UseWCombo", "Use W In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Udyr.UseECombo", "Use E In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Udyr.UseRCombo", "Use R In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Udyr.AutoMuramana", "Auto Muramana Usage").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Udyr.AutoMuramanaMiniMana", "Minimum Mana To Use Auto Muramana").SetValue(new Slider(10, 0, 100)));
            Config.SubMenu("Combo").AddItem(new MenuItem("Udyr.ComboSettings", "Playing Style").SetValue(new StringList(new[] { "TigerCombo", "PhoenixCombo" }, 1)));

            Config.AddSubMenu(new Menu("LastHit", "LastHit"));
            Config.SubMenu("LastHit").AddItem(new MenuItem("Udyr.UseQLastHit", "Active Q Stance to LastHit").SetValue(true));
            Config.SubMenu("LastHit").AddItem(new MenuItem("Udyr.QMiniManaLastHit", "Minimum Mana To Active Q Stance In LastHit").SetValue(new Slider(35, 0, 100)));
            Config.SubMenu("LastHit").AddItem(new MenuItem("Udyr.UseWLastHit", "Active W Stance To Restore Health In LastHit").SetValue(true));
            Config.SubMenu("LastHit").AddItem(new MenuItem("Udyr.WMiniManaLastHit", "Minimum Mana To Active W Stance In LastHit").SetValue(new Slider(35, 0, 100)));
            Config.SubMenu("LastHit").AddItem(new MenuItem("Udyr.WMiniHpLastHit", "Minimum Hp % To Active W Stance In LastHit").SetValue(new Slider(35, 0, 100)));

            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Udyr.UseQLaneClear", "Use Q in LaneClear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Udyr.QMiniManaLaneClear", "Minimum Mana To Use Q In LaneClear").SetValue(new Slider(35, 0, 100)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Udyr.UseWLaneClear", "Use W in LaneClear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Udyr.WMiniManaLaneClear", "Minimum Mana To Use W In LaneClear").SetValue(new Slider(35, 0, 100)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Udyr.WMiniHpLaneClear", "Minimum Hp % To Active W Stance In In LaneClear").SetValue(new Slider(30, 0, 100)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Udyr.UseRLaneClear", "Use R in LaneClear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Udyr.RMiniManaLaneClear", "Minimum Mana To Use R In LaneClear").SetValue(new Slider(35, 0, 100)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Udyr.RLaneClearCount", "Minimum Minion To Use R In LaneClear").SetValue(new Slider(3, 1, 6)));

            Config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Udyr.UseQJungleClear", "Use Q In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Udyr.QMiniManaJungleClear", "Minimum Mana To Use Q In JungleClear").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Udyr.UseWJungleClear", "Use W In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Udyr.WMiniManaJungleClear", "Minimum Mana To Use W In JungleClear").SetValue(new Slider(40, 0, 100)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Udyr.WMiniHpJungleClear", "Minimum Health % To Use W In JungleClear").SetValue(new Slider(30, 0, 100)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Udyr.UseEJungleClear", "Use E In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Udyr.EMiniManaJungleClear", "Minimum Mana To Use E In JungleClear").SetValue(new Slider(35, 0, 100)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Udyr.UseRJungleClear", "Use R In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Udyr.RMiniManaJungleClear", "Minimum Mana To Use R In JungleClear").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Udyr.SafeJungleClear", "Dont Use Spell In Jungle Clear If Enemy in Dangerous Range").SetValue(true));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("Udyr.AutoEMovement", "Auto Use E When Moving").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("Udyr.AutoEMovementMana", "Minimum Mana % to Auto E  When Moving").SetValue(new Slider(70, 0, 100)));
            Config.SubMenu("Misc").AddItem(new MenuItem("Udyr.EInterrupt", "Interrupt Spells With E").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Udyr.AutoEEGC", "Auto E On Gapclosers").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Udyr.AutoWEGC", "Auto W On Gapclosers").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Udyr.AutoWWhenEnemyCast", "Auto W On Enemy Spell or AA Cast On Me").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Udyr.AutoWWhenEnemyCastMiniHp", "Minimum Hp % to Auto W On Enemy Spell or AA Cast On Me").SetValue(new Slider(80, 0, 100)));
            Config.SubMenu("Misc").AddItem(new MenuItem("Udyr.AutoPotion", "Use Auto Potion").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Udyr.StackPassiveInFountain", "Use Spell to Stack Passive in Fountain").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Udyr.StackPassiveInFountainKeepBuffUp", "Keep 3rd Buff Up When Stacking Passive in Fountain").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Udyr.StackPassiveInFountainMinMana", "Minimum Mana % to Stack Passive in Fountain").SetValue(new Slider(90, 0, 100)));
            Config.SubMenu("Misc").AddItem(new MenuItem("Udyr.AutoLevelSpell", "Auto Level Spell").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Udyr.AutoLevelSpellStyleSettings", "Auto Level Spell For ").SetValue(new StringList(new[] { "TigerCombo", "PhoenixCombo" }, 1)));

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("GapCloseRange", "Draw E GapClose Range").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RRange", "R range").SetValue(new Circle(true, Color.Gold)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawOrbwalkTarget", "Draw Orbwalk target").SetValue(true));

            Config.AddToMainMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;


        }

        #region ToogleOrder Game_OnUpdate
        public static void Game_OnGameUpdate(EventArgs args)
        {

            if (Config.Item("Udyr.AutoLevelSpell").GetValue<bool>()) LevelUpSpells();

            if (Player.IsDead) return;

            if (Player.GetBuffCount("Recall") == 1) return;

            ManaManager();
            PotionManager();

            if (!Player.InFountain() && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LastHit && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed &&
                Config.Item("Udyr.AutoEMovement").GetValue<bool>() && Player.ManaPercent >= Config.Item("Udyr.AutoEMovementMana").GetValue<Slider>().Value && E.IsReady() && Player.CountEnemiesInRange(700) == 0)
            {
                E.Cast();
            }

            if (Config.Item("Udyr.StackPassiveInFountain").GetValue<bool>() && ObjectManager.Player.InFountain() && Player.ManaPercent >= Config.Item("Udyr.StackPassiveInFountainMinMana").GetValue<Slider>().Value)
            {

                var buffEndTime = Config.Item("Udyr.StackPassiveInFountainKeepBuffUp").GetValue<bool>() ? float.MaxValue : GetPassiveBuffTimer(Player);

                if (Q.IsReady() && (MonkeyAgility != 3 || buffEndTime > Game.Time + (buffEndTime / 2)))
                {
                    Q.Cast();
                    return;
                }

                if (W.IsReady() && !Q.IsReady() && (MonkeyAgility != 3 || buffEndTime > Game.Time + (buffEndTime / 2)))
                {
                    W.Cast();
                    return;
                }

                if (R.IsReady() && !Q.IsReady() && !W.IsReady() && (MonkeyAgility != 3 || buffEndTime > Game.Time + (buffEndTime / 2)))
                {
                    R.Cast();
                    return;
                }

            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear();
                JungleClear();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                LastHit();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                LastHit();
            }

        }
        #endregion

        #region Interrupt OnProcessSpellCast
        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {

            double InterruptOn = SpellToInterrupt(args.SData.Name);
            if (unit.Team != ObjectManager.Player.Team && InterruptOn >= 0f && unit.IsValidTarget(800))
            {

                if (Config.Item("Udyr.EInterrupt").GetValue<bool>() && E.IsReady() && Player.Mana > QMANA && Player.Distance(unit) <= 800)
                {
                    if (!isInEStance)
                    {
                        E.Cast();
                    }
                    else
                        return;

                }

            }

            if (Config.Item("Udyr.AutoWWhenEnemyCast").GetValue<bool>() && (unit.IsValid<AIHeroClient>() && !unit.IsValid<Obj_AI_Turret>()) && unit.IsEnemy && args.Target.IsMe && W.IsReady() && Player.Distance(unit) < 1000)
            {
                if (Player.HealthPercent < Config.Item("Udyr.AutoWWhenEnemyCastMiniHp").GetValue<Slider>().Value && !isInEStance)
                {
                    W.Cast();
                }

            }

        }
        #endregion

        #region AntiGapCloser
        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {

            if (Config.Item("Udyr.AutoEEGC").GetValue<bool>() && E.IsReady() && Player.Mana > EMANA && Player.Distance(gapcloser.Sender) <= E.Range && !isInEStance)
            {
                E.Cast(gapcloser.Sender, true);
            }

            else if (Config.Item("Udyr.AutoWEGC").GetValue<bool>() && W.IsReady() && !isInEStance && Player.Mana > WMANA && Player.Distance(gapcloser.Sender) < 600)
            {
                W.Cast();
            }

        }
        #endregion

        #region Combo
        public static void Combo()
        {

            var useQ = Program.Config.Item("Udyr.UseQCombo").GetValue<bool>();
            var useE = Program.Config.Item("Udyr.UseECombo").GetValue<bool>();
            var useR = Program.Config.Item("Udyr.UseRCombo").GetValue<bool>();

            var target = TargetSelector.GetTarget(Player.AttackRange + 800, TargetSelector.DamageType.Physical);
            if (target.IsValidTarget())
            {

                if (Config.Item("Udyr.ComboSettings").GetValue<StringList>().SelectedIndex == 1)
                {

                    #region Sort E combo mode
                    if (useE && E.IsReady() && Player.Mana > EMANA)
                    {

                        if (Player.Distance(target) > E.Range)
                        {
                            E.Cast();
                        }

                        else if (Player.Distance(target) <= E.Range && !target.HasBuff("udyrbearstuncheck"))
                        {
                            E.Cast();
                        }

                    }
                    #endregion

                    #region Sort R combo mode
                    if (useR && R.IsReady() && Player.Mana >= RMANA && !HavePhoenixAoe && Player.Distance(target) <= R.Range && target.HasBuff("udyrbearstuncheck"))
                    {
                        R.Cast();
                    }
                    #endregion

                    #region Sort Q combo mode
                    if (useQ && Q.IsReady() && Player.Mana >= QMANA && Player.Distance(target) <= Q.Range && target.HasBuff("udyrbearstuncheck"))
                    {
                        Q.Cast();
                    }
                    #endregion

                }

                if (Config.Item("Udyr.ComboSettings").GetValue<StringList>().SelectedIndex != 1)
                {

                    #region Sort E combo mode
                    if (useE && E.IsReady() && Player.Mana > EMANA)
                    {
                        if (Player.Distance(target) > E.Range)
                        {
                            E.Cast();
                        }

                        else if (Player.Distance(target) <= E.Range && !target.HasBuff("udyrbearstuncheck"))
                        {
                            E.Cast();

                        }

                    }
                    #endregion

                    #region Sort Q combo mode
                    if (useQ && Q.IsReady() && Player.Mana >= QMANA && Player.Distance(target) <= Q.Range && target.HasBuff("udyrbearstuncheck"))
                    {
                        Q.Cast();
                    }
                    #endregion

                    #region Sort R combo mode
                    if (useR && R.IsReady() && Player.Mana >= RMANA && !HavePhoenixAoe && Player.Distance(target) <= R.Range && target.HasBuff("udyrbearstuncheck"))
                    {
                        R.Cast();
                    }
                    #endregion

                }

            }

        }
        #endregion

        #region LastHit
        public static void LastHit()
        {

            var useQ = Program.Config.Item("Udyr.UseQLastHit").GetValue<bool>();
            var useW = Program.Config.Item("Udyr.UseWLastHit").GetValue<bool>();

            var QMinMana = Config.Item("Udyr.QMiniManaLastHit").GetValue<Slider>().Value;
            var WMinMana = Config.Item("Udyr.WMiniManaLastHit").GetValue<Slider>().Value;

            var WMinHp = Config.Item("Udyr.WMiniHpLastHit").GetValue<Slider>().Value;

            var MinionQ = MinionManager.GetMinions(Player.AttackRange, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (useQ && !isInQStance && Q.IsReady() && MinionQ.Health < Q.GetDamage(MinionQ) * 0.9 && Player.ManaPercent >= QMinMana && (Player.HealthPercent > WMinHp || !useW))
            {
                Q.Cast();
            }

            else if (useW && !isInWStance && W.IsReady() && Player.ManaPercent >= WMinMana && Player.HealthPercent <= WMinHp)
            {
                W.Cast();
            }



        }
        #endregion

        #region LaneClear
        public static void LaneClear()
        {

            var useQ = Program.Config.Item("Udyr.UseQLaneClear").GetValue<bool>();
            var useW = Program.Config.Item("Udyr.UseWLaneClear").GetValue<bool>();
            var useR = Program.Config.Item("Udyr.UseRLaneClear").GetValue<bool>();

            var QMinMana = Config.Item("Udyr.QMiniManaLaneClear").GetValue<Slider>().Value;
            var WMinMana = Config.Item("Udyr.WMiniManaLaneClear").GetValue<Slider>().Value;
            var RMinMana = Config.Item("Udyr.RMiniManaLaneClear").GetValue<Slider>().Value;

            var WMinHp = Config.Item("Udyr.WMiniHpLaneClear").GetValue<Slider>().Value;
            var RCount = Config.Item("Udyr.RLaneClearCount").GetValue<Slider>().Value;

            var allMinionsQ = MinionManager.GetMinions(Q.Range, MinionTypes.All);

            if (useR && !isInRStance && R.IsReady() && Player.ManaPercent >= RMinMana && (Player.HealthPercent > WMinHp || !useW))
            {

                var Rfarm = R.GetCircularFarmLocation(allMinionsQ, R.Range);
                if (Rfarm.MinionsHit >= RCount)
                    R.Cast();

            }

            if (useQ && !isInQStance && Q.IsReady() && Player.ManaPercent >= QMinMana && (Player.HealthPercent > WMinHp || !useW))
            {
                Q.Cast();
            }

            else if (useW && !isInWStance && W.IsReady() && Player.ManaPercent >= WMinMana && Player.HealthPercent <= WMinHp)
            {
                W.Cast();
            }

        }
        #endregion

        #region JungleClear
        public static void JungleClear()
        {

            var useQ = Program.Config.Item("Udyr.UseQJungleClear").GetValue<bool>();
            var useW = Program.Config.Item("Udyr.UseWJungleClear").GetValue<bool>();
            var useE = Program.Config.Item("Udyr.UseWJungleClear").GetValue<bool>();
            var useR = Program.Config.Item("Udyr.UseWJungleClear").GetValue<bool>();

            var QMinMana = Config.Item("Udyr.QMiniManaJungleClear").GetValue<Slider>().Value;
            var WMinMana = Config.Item("Udyr.WMiniManaJungleClear").GetValue<Slider>().Value;
            var EMinMana = Config.Item("Udyr.EMiniManaJungleClear").GetValue<Slider>().Value;
            var RMinMana = Config.Item("Udyr.RMiniManaJungleClear").GetValue<Slider>().Value;

            var WMinHp = Config.Item("Udyr.WMiniHpJungleClear").GetValue<Slider>().Value;

            var allMinionsR = MinionManager.GetMinions(Player.AttackRange * 2, MinionTypes.All, MinionTeam.Neutral);
            var MinionN = MinionManager.GetMinions(Player.AttackRange * 2, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (Config.Item("Udyr.SafeJungleClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;

            if (!MinionN.IsValidTarget() || MinionN == null)
            {
                LaneClear();
                return;
            }

            if (Config.Item("Udyr.ComboSettings").GetValue<StringList>().SelectedIndex == 1)
            {

                if (useE && E.IsReady() && Player.Distance(MinionN) < Player.AttackRange && Player.Mana > EMANA && Player.ManaPercent >= EMinMana && !MinionN.HasBuff("udyrbearstuncheck"))
                {
                    E.Cast(MinionN, true);
                }

                else if (useW && W.IsReady() && Player.Distance(MinionN) < Player.AttackRange && Player.Mana > WMANA && Player.ManaPercent >= WMinMana && Player.HealthPercent <= WMinHp)
                {
                    W.Cast(MinionN, true);
                }

                else if (useR && R.IsReady() && Player.Distance(MinionN) < Player.AttackRange && Player.Mana > RMANA && Player.ManaPercent >= RMinMana)
                {
                    R.Cast(MinionN, true);
                }

                else if (useQ && Q.IsReady() && Player.Distance(MinionN) < Player.AttackRange && Player.Mana > QMANA && Player.ManaPercent >= QMinMana)
                {
                    Q.Cast(MinionN, true);
                }

            }

            if (Config.Item("Udyr.ComboSettings").GetValue<StringList>().SelectedIndex != 1)
            {

                if (useE && E.IsReady() && Player.Distance(MinionN) < Player.AttackRange && Player.Mana > EMANA && Player.ManaPercent >= EMinMana && !MinionN.HasBuff("udyrbearstuncheck"))
                {
                    E.Cast(MinionN, true);
                }

                else if (useW && W.IsReady() && Player.Distance(MinionN) < Player.AttackRange && Player.Mana > WMANA && Player.ManaPercent >= WMinMana && Player.HealthPercent <= WMinHp)
                {
                    W.Cast(MinionN, true);
                }

                else if (useQ && Q.IsReady() && Player.Distance(MinionN) < Player.AttackRange && Player.Mana > WMANA && Player.ManaPercent >= QMinMana)
                {
                    Q.Cast(MinionN, true);
                }

                else if (useR && R.IsReady() && Player.Distance(MinionN) < Player.AttackRange && Player.Mana > RMANA && Player.ManaPercent >= RMinMana)
                {
                    R.Cast(MinionN, true);
                }

            }



        }
        #endregion

        #region Check Stances

        #region Check Q Stance
        private static bool isInQStance
        {
            get { return ObjectManager.Player.HasBuff("UdyrTigerStance"); }
        }
        #endregion

        #region Check W Stance
        private static bool isInWStance
        {
            get { return ObjectManager.Player.HasBuff("UdyrTurtleStance"); }
        }
        #endregion

        #region Check E Stance
        private static bool isInEStance
        {
            get { return ObjectManager.Player.HasBuff("UdyrBearStance"); }
        }
        #endregion

        #region Check R Stance
        private static bool isInRStance
        {
            get { return ObjectManager.Player.HasBuff("UdyrPhoenixStance"); }
        }
        #endregion

        #endregion

        #region Check Phoenix Buff Activation
        private static bool HavePhoenixAoe
        {
            get { return ObjectManager.Player.HasBuff("UdyrPhoenixActivation"); }
        }
        #endregion

        #region PassiveBuffCount Monkey Agility
        private static int MonkeyAgility
        {
            get
            {
                var data = ObjectManager.Player.Buffs.FirstOrDefault(b => b.DisplayName == "UdyrMonkeyAgilityBuff");
                return data != null ? data.Count : 0;
            }
        }
        #endregion

        #region BeforeAA
        static void Orbwalking_BeforeAttack(LeagueSharp.Common.Orbwalking.BeforeAttackEventArgs args)
        {

            if (Config.Item("Udyr.AutoMuramana").GetValue<bool>())
            {
                int Muramanaitem = Items.HasItem(Muramana) ? 3042 : 3043;
                if (args.Target.IsValid<AIHeroClient>() && args.Target.IsEnemy && Items.HasItem(Muramanaitem) && Items.CanUseItem(Muramanaitem) && Player.ManaPercent > Config.Item("Udyr.AutoMuramanaMiniMana").GetValue<Slider>().Value)
                {
                    if (!ObjectManager.Player.HasBuff("Muramana"))
                        Items.UseItem(Muramanaitem);
                }
                else if (ObjectManager.Player.HasBuff("Muramana") && Items.HasItem(Muramanaitem) && Items.CanUseItem(Muramanaitem))
                    Items.UseItem(Muramanaitem);
            }

        }
        #endregion

        #region PlayerDamage
        public static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (R.IsReady())
            {
                damage += (float)Damage.GetSpellDamage(Player, hero, SpellSlot.R);
            }
            if (Q.IsReady())
            {
                damage += Damage.GetSpellDamage(Player, hero, SpellSlot.Q);
            }
            if (W.IsReady())
            {
                damage += (float)Damage.GetSpellDamage(Player, hero, SpellSlot.W);
            }
            if (Player.Spellbook.CanUseSpell(Player.GetSpellSlot("summonerdot")) == SpellState.Ready)
            {
                damage += (float)Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            return (float)damage;
        }
        #endregion

        #region Interrupt Spell List
        public static double SpellToInterrupt(string SpellName)
        {
            if (SpellName == "KatarinaR")
                return 0;
            if (SpellName == "AlZaharNetherGrasp")
                return 0;
            if (SpellName == "GalioIdolOfDurand")
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

        #region ManaManager
        public static void ManaManager()
        {

            QMANA = Q.Instance.SData.Mana;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;
            RMANA = R.Instance.SData.Mana;

            if (ObjectManager.Player.Health < ObjectManager.Player.MaxHealth * 0.2)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
            }
        }
        #endregion

        #region PotionManager
        public static void PotionManager()
        {
            if (Player.Level == 1 && Player.CountEnemiesInRange(1000) == 1 && Player.Health >= Player.MaxHealth * 0.35) return;
            if (Player.Level == 1 && Player.CountEnemiesInRange(1000) == 2 && Player.Health >= Player.MaxHealth * 0.50) return;

            if (Config.Item("Udyr.AutoPotion").GetValue<bool>() && !Player.InFountain() && !Player.IsRecalling() && !Player.IsDead)
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

            if (Config.Item("GapCloseRange").GetValue<bool>())
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, Player.AttackRange + 800, (Player.CountEnemiesInRange(Player.AttackRange + 800) > 0 && isInEStance) ? Color.OrangeRed : Color.Green, 10, 10);

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
            int qL = Player.Spellbook.GetSpell(SpellSlot.Q).Level + qOff;
            int wL = Player.Spellbook.GetSpell(SpellSlot.W).Level + wOff;
            int eL = Player.Spellbook.GetSpell(SpellSlot.E).Level + eOff;
            int rL = Player.Spellbook.GetSpell(SpellSlot.R).Level + rOff;
            if (qL + wL + eL + rL < ObjectManager.Player.Level)
            {
                int[] level = new int[] { 0, 0, 0, 0 };
                for (int i = 0; i < ObjectManager.Player.Level; i++)
                {

                    if (Config.Item("Udyr.AutoLevelSpellStyleSettings").GetValue<StringList>().SelectedIndex == 1)
                    {
                        level[abilitySequence[i] - 1] = level[abilitySequence[i] - 1] + 1;
                    }

                    if (Config.Item("Udyr.AutoLevelSpellStyleSettings").GetValue<StringList>().SelectedIndex != 1)
                    {
                        level[abilitySequence2[i] - 1] = level[abilitySequence2[i] - 1] + 1;
                    }

                }
                if (qL < level[0]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (wL < level[1]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (eL < level[2]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (rL < level[3]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);

            }
        }
        #endregion

        #region CheckBuffTimer
        private static float GetPassiveBuffTimer(AIHeroClient Player)
        {
            var buffEndTimer = Player.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => buff.DisplayName == "UdyrMonkeyAgilityBuff")
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault();
            return buffEndTimer;
        }
        #endregion

    }

}
