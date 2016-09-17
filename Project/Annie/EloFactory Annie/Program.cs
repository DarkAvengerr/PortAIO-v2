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
 namespace EloFactory_Annie
{
    internal class Program
    {
        public const string ChampionName = "Annie";

        public static Orbwalking.Orbwalker Orbwalker;

        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite = new Spell(SpellSlot.Unknown, 600);
        public static SpellSlot FlashSlot;

        public static float QMANA;
        public static float WMANA;
        public static float EMANA;
        public static float RMANA;

        public static bool StunUp = false;

        public static Items.Item HealthPotion = new Items.Item(2003, 0);
        public static Items.Item ManaPotion = new Items.Item(2004, 0);
        public static Items.Item CrystallineFlask = new Items.Item(2041, 0);
        public static Items.Item BiscuitofRejuvenation = new Items.Item(2010, 0);

        public static Menu Config;

        private static AIHeroClient Player;

        public static int[] abilitySequence;
        public static int qOff = 0, wOff = 0, eOff = 0, rOff = 0;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;

            if (Player.ChampionName != ChampionName) return;

            Q = new Spell(SpellSlot.Q, 625f);
            W = new Spell(SpellSlot.W, 600f);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 625f);

            Q.SetTargetted(0.25f, 1400f);
            W.SetSkillshot(0.50f, 250f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.20f, 250f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            var ignite = Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "summonerdot");
            if (ignite != null)
                Ignite.Slot = ignite.Slot;

            FlashSlot = Player.GetSpellSlot("summonerflash");

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            abilitySequence = new int[] { 2, 1, 1, 2, 3, 4, 1, 1, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };

            Config = new Menu(ChampionName + " By LuNi", ChampionName + " By LuNi", true);

            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddSubMenu(new Menu("KS Mode", "KS Mode"));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Annie.UseStunKS", "KS With Stun Up").SetValue(true));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Annie.UseStunKS1v1", "KS With Stun Up Only When No Other Enemy Close To Me").SetValue(false));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Annie.UseIgniteKS", "KS With Ignite").SetValue(true));           
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Annie.UseQKS", "KS With Q").SetValue(true));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Annie.UseWKS", "KS With W").SetValue(true));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Annie.UseRKSEarlyGame", "KS With R Early Game If Enemy Close To His Tower").SetValue(true));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Annie.UseRKS", "KS With R Always").SetValue(false));
            Config.SubMenu("Combo").AddItem(new MenuItem("Annie.UseQCombo", "Use Q In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Annie.UseWCombo", "Use W In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Annie.UseECombo", "Use E In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Annie.UseRCombo", "Use R In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Annie.AutoE", "Use Auto E To Stack Passive").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Annie.AA", "AA Usage In Combo (Mode)").SetValue(new StringList(new[] { "Inteligent AA", "No AA" })));

            Config.AddSubMenu(new Menu("Flash R", "Flash R"));
            Config.SubMenu("Flash R").AddItem(new MenuItem("Annie.FlashR", "Flash R Key ! (Need Stun Up)").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Flash R").AddItem(new MenuItem("Annie.MoveOnCursorWhenFlashRKey", "Move On Cursor When Flash R Key Press").SetValue(true));
            Config.SubMenu("Flash R").AddItem(new MenuItem("Annie.FlashREnemiesHitCount", "Minimum Enemies Hit To Flash R").SetValue(new Slider(3, 1, 5)));

            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("Annie.UseQHarass", "Use Q In Harass").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("Annie.QMiniManaHarass", "Minimum Mana Percent To Use Q In Harass").SetValue(new Slider(20, 0, 100)));
            Config.SubMenu("Harass").AddItem(new MenuItem("Annie.UseWHarass", "Use W In Harass").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("Annie.WMiniManaHarass", "Minimum Mana Percent To Use W In Harass").SetValue(new Slider(70, 0, 100)));
            Config.SubMenu("Harass").AddItem(new MenuItem("Annie.HarassActive", "Harass!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Harass").AddItem(new MenuItem("Annie.HarassActiveT", "Harass (toggle)!").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));

            Config.AddSubMenu(new Menu("LastHit", "LastHit"));
            Config.SubMenu("LastHit").AddItem(new MenuItem("Annie.UseQLastHit", "Use Q In LastHit").SetValue(true));
            Config.SubMenu("LastHit").AddItem(new MenuItem("Annie.UseQLastHitStun", "Never Use Q In LastHit When PassiveStun Up").SetValue(true));

            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Annie.UseQLaneClear", "Use Q in LaneClear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Annie.UseQLaneClearStun", "Never Use Q In LaneClear When PassiveStun Up").SetValue(false));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Annie.UseWLaneClear", "Use W in LaneClear").SetValue(false));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Annie.WMiniManaLaneClear", "Minimum Mana Percent To Use W In LaneClear").SetValue(new Slider(70, 0, 100)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Annie.WLaneClearCount", "Minimum Minion To Use W In LaneClear").SetValue(new Slider(4, 1, 6)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Annie.UseWLaneClearStun", "Never Use W In LaneClear When PassiveStun Up").SetValue(false));

            Config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Annie.UseQJungleClear", "Use Q In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Annie.QMiniManaJungleClear", "Minimum Mana Percent To Use Q In JungleClear").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Annie.UseWJungleClear", "Use W In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Annie.WMiniManaJungleClear", "Minimum Mana Percent To Use W In JungleClear").SetValue(new Slider(0, 0, 100)));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddSubMenu(new Menu("Skin Changer", "Skin Changer"));
            Config.SubMenu("Misc").SubMenu("Skin Changer").AddItem(new MenuItem("Annie.SkinChanger", "Use Skin Changer").SetValue(false));
            Config.SubMenu("Misc").SubMenu("Skin Changer").AddItem(new MenuItem("Annie.SkinChangerName", "Skin choice").SetValue(new StringList(new[] { "Classic", "Goth", "Red Riding", "Annie in Wonderland", "Prom Queen", "Frostfire", "Reverse", "FrankenTibbers", "Panda", "Sweetheart" })));
            Config.SubMenu("Misc").AddItem(new MenuItem("Annie.QInterrupt", "Interrupt Spells When PassiveStun Up With Q").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Annie.WInterrupt", "Interrupt Spells When PassiveStun Up With W").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Annie.RInterrupt", "Interrupt Spells When PassiveStun Up With R").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("Annie.AutoQEGC", "Auto Q When StunPassive Up On Gapclosers").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("Annie.AutoWEGC", "Auto W When StunPassive Up On Gapclosers").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("Annie.AutoEEGC", "Auto E On Gapclosers").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("Annie.AutoPotion", "Use Auto Potion").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Annie.AutoLevelSpell", "Auto Level Spell").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Annie.StackPassiveInFountain", "Use Spell W To Stack Passive In Fountain").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Annie.WMiniManaStackPassiveInFountain", "Minimum Mana Percent To Stack Passive In Fountain").SetValue(new Slider(90, 0, 100)));
            

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q range").SetValue(new Circle(true, Color.Indigo)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("WRange", "W range").SetValue(new Circle(true, Color.Green)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RRange", "R range").SetValue(new Circle(true, Color.Gold)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawOrbwalkTarget", "Draw Orbwalk target").SetValue(true));

            Config.AddToMainMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;


        }

        #region ToogleOrder Game_OnUpdate
        public static void Game_OnGameUpdate(EventArgs args)
        {
            //Player.SetSkin(Player.BaseSkinName, Config.Item("Annie.SkinChanger").GetValue<bool>() ? Config.Item("Annie.SkinChangerName").GetValue<StringList>().SelectedIndex : Player.SkinId);

            if (Config.Item("Annie.AutoLevelSpell").GetValue<bool>()) LevelUpSpells();

            if (Player.IsDead) return;

            if (Player.GetBuffCount("Recall") == 1) return;

            ManaManager();
            PotionManager();

            StunUp = GetPassiveCount();

            KillSteal();

            if (Config.Item("Annie.AutoE").GetValue<bool>() && E.IsReady() && !StunUp && ObjectManager.Player.Mana > RMANA + EMANA + QMANA + WMANA && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
            {
                E.Cast();
            }


            if (Config.Item("Annie.StackPassiveInFountain").GetValue<bool>() && W.IsReady() && ObjectManager.Player.InFountain() && !StunUp && Player.ManaPercent >= Config.Item("Annie.WMiniManaStackPassiveInFountain").GetValue<Slider>().Value)
                W.Cast(ObjectManager.Player, true, true);


            if (Config.Item("Annie.FlashR").GetValue<KeyBind>().Active)
            {
                if (Config.Item("Annie.MoveOnCursorWhenFlashRKey").GetValue<bool>())
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    FlashRLogic();
                }

                if (!Config.Item("Annie.MoveOnCursorWhenFlashRKey").GetValue<bool>())
                {
                    FlashRLogic();
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Orbwalking.Attack = true;
                LaneClear();
                JungleClear();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                Orbwalking.Attack = true;
                LastHit();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Orbwalking.Attack = true;
                LastHit();
            }

            if (Config.Item("Annie.HarassActive").GetValue<KeyBind>().Active || Config.Item("Annie.HarassActiveT").GetValue<KeyBind>().Active)
            {
                Orbwalking.Attack = true;
                Harass();
            }

        }
        #endregion

        #region Interrupt OnProcessSpellCast
        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            double InterruptOn = SpellToInterrupt(args.SData.Name);
            if (unit.Team != ObjectManager.Player.Team && InterruptOn >= 0f && unit.IsValidTarget(Q.Range))
            {

                if (Config.Item("Annie.QInterrupt").GetValue<bool>() && Q.IsReady() && Player.Mana > QMANA && Player.Distance(unit) <= Q.Range && StunUp)
                {
                    Q.Cast(unit, true);
                }

                if (Config.Item("Annie.WInterrupt").GetValue<bool>() && W.IsReady() && Player.Mana > WMANA + QMANA && Player.Distance(unit) <= W.Range - 20 && StunUp)
                {
                    W.Cast(unit, true, true);
                }

                if (Config.Item("Annie.RInterrupt").GetValue<bool>() && R.IsReady() && Player.Mana > WMANA + QMANA + RMANA && Player.Distance(unit) <= R.Range - 20 && StunUp)
                {
                    R.Cast(unit, true, true);
                }

            }
        }
        #endregion

        #region AntiGapCloser
        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {

            if (Config.Item("Annie.AutoEEGC").GetValue<bool>() && E.IsReady() && Player.Mana > EMANA + QMANA && Player.Distance(gapcloser.Sender) <= Q.Range && !StunUp)
            {
                E.Cast();
            }

            if (Config.Item("Annie.AutoQEGC").GetValue<bool>() && Q.IsReady() && Player.Mana > QMANA && Player.Distance(gapcloser.Sender) <= Q.Range && StunUp)
            {
                Q.Cast(gapcloser.Sender, true);
            }

            if (Config.Item("Annie.AutoWEGC").GetValue<bool>() && W.IsReady() && Player.Mana > WMANA + QMANA && Player.Distance(gapcloser.Sender) < W.Range - 20 && StunUp)
            {
                W.Cast(gapcloser.Sender, true, true);
            }

        }
        #endregion

        #region Combo
        public static void Combo()
        {

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                switch (Config.Item("Annie.AA").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        {
                            var t = TargetSelector.GetTarget(ObjectManager.Player.AttackRange, TargetSelector.DamageType.Magical);
                            if (t.IsValidTarget() && ObjectManager.Player.GetAutoAttackDamage(t) > t.Health || (Player.Distance(t) <= (Q.Range * 0.7) && !Q.IsReady() && !W.IsReady()) || (Player.Mana < QMANA || Player.Mana < WMANA || Player.Mana < EMANA))
                                Orbwalking.Attack = true;
                            else
                                Orbwalking.Attack = false;
                            break;
                        }
                    case 2:
                        {
                            var t = TargetSelector.GetTarget(ObjectManager.Player.AttackRange, TargetSelector.DamageType.Magical);
                            if (t.IsValidTarget() && ObjectManager.Player.GetAutoAttackDamage(t) > t.Health || (Player.Mana < QMANA || Player.Mana < WMANA || Player.Mana < EMANA))
                                Orbwalking.Attack = true;
                            else
                                Orbwalking.Attack = false;
                            break;
                        }

                }

            }

            var useQ = Config.Item("Annie.UseQCombo").GetValue<bool>();
            var useW = Config.Item("Annie.UseWCombo").GetValue<bool>();
            var useE = Config.Item("Annie.UseECombo").GetValue<bool>();
            var useR = Config.Item("Annie.UseRCombo").GetValue<bool>();

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target.IsValidTarget())
            {
                #region Sort E combo mode
                if (useE && E.IsReady() && !Config.Item("Annie.AutoE").GetValue<bool>() && !StunUp && Player.Mana > EMANA)
                {
                    E.Cast();
                }
                #endregion

                #region Sort R combo mode
                if (useR && R.IsReady() && Player.Mana >= RMANA && !Tibers)
                {

                    RLogic();

                }
                #endregion

                #region Sort W combo mode
                if (useW && W.IsReady() && Player.Mana >= WMANA)
                {

                    if (StunUp && target.CountEnemiesInRange(250) > 1)
                    {
                        W.Cast(target, true, true);
                    }

                    else if (!Q.IsReady())
                    {
                        W.Cast(target, true, true);
                    }

                    else if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Taunt))
                    {
                        W.Cast(target, true, true);
                    }

                }
                #endregion

                #region Sort Q combo mode
                if (useQ && Q.IsReady() && Player.Mana >= QMANA)
                {

                    if (StunUp && target.CountEnemiesInRange(400) > 1 && (W.IsReady() || R.IsReady()))
                    {
                        return;
                    }
                    else
                        Q.Cast(target, true);

                }
                #endregion
            }

        }
        #endregion

        #region Harass
        public static void Harass()
        {

            var targetH = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            var useQ = Config.Item("Annie.UseQHarass").GetValue<bool>();
            var useW = Config.Item("Annie.UseWHarass").GetValue<bool>();


            if (useW && W.IsReady() && ObjectManager.Player.Distance(targetH) < W.Range - 20 && Player.ManaPercent >= Config.Item("Annie.WMiniManaHarass").GetValue<Slider>().Value)
            {
                W.Cast(targetH, true, true);
            }

            if (useQ && Q.IsReady() && ObjectManager.Player.Distance(targetH) <= Q.Range && Player.ManaPercent >= Config.Item("Annie.QMiniManaHarass").GetValue<Slider>().Value)
            {
                Q.Cast(targetH, true);
            }


        }
        #endregion

        #region LastHit
        public static void LastHit()
        {


            var useQ = Config.Item("Annie.UseQLastHit").GetValue<bool>();

            var allMinionsQ = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy);

            foreach (var minion in allMinionsQ)
            {
                if (useQ && Q.IsReady() && minion.Health > ObjectManager.Player.GetAutoAttackDamage(minion) && minion.Health < Q.GetDamage(minion) * 0.9)
                {
                    if (Config.Item("Annie.UseQLastHitStun").GetValue<bool>() && StunUp)
                    {
                        return;
                    }
                    else
                    {
                        Q.Cast(minion);
                    }

                }
            }

        }
        #endregion

        #region LaneClear
        public static void LaneClear()
        {

            var useQ = Config.Item("Annie.UseQLaneClear").GetValue<bool>();
            var useW = Config.Item("Annie.UseWLaneClear").GetValue<bool>();

            var allMinionsQ = MinionManager.GetMinions(Q.Range, MinionTypes.All);

            foreach (var minion in allMinionsQ)
            {
                if (useQ && Q.IsReady() && Player.Mana > QMANA && minion.Health > ObjectManager.Player.GetAutoAttackDamage(minion) && minion.Health < Q.GetDamage(minion) * 0.9)
                {
                    if (Config.Item("Annie.UseQLaneClearStun").GetValue<bool>() && StunUp)
                    {
                        return;
                    }
                    else
                    {
                        Q.Cast(minion);
                    }

                }
            }

            if (useW && W.IsReady() && Player.Mana > WMANA && Player.ManaPercent >= Config.Item("Annie.WMiniManaLaneClear").GetValue<Slider>().Value)
            {
                if (Config.Item("Annie.UseWLaneClearStun").GetValue<bool>() && StunUp)
                {
                    return;
                }
                else
                {
                    var Wfarm = W.GetCircularFarmLocation(allMinionsQ, W.Width);
                    if (Wfarm.MinionsHit >= Config.Item("Annie.WLaneClearCount").GetValue<Slider>().Value && W.IsReady())
                        W.Cast(Wfarm.Position);
                }

            }

        }
        #endregion

        #region JungleClear
        public static void JungleClear()
        {

            var useQ = Config.Item("Annie.UseQJungleClear").GetValue<bool>();
            var useW = Config.Item("Annie.UseWJungleClear").GetValue<bool>();

            var MinionN = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (!MinionN.IsValidTarget() || MinionN == null)
            {
                LaneClear();
                return;
            }


            if (Q.IsReady() && Player.Mana > QMANA && useQ)
            {
                if (MinionN.Distance(Player) < Q.Range && Player.ManaPercent >= Config.Item("Annie.QMiniManaJungleClear").GetValue<Slider>().Value)
                {
                    Q.Cast(MinionN);
                }

            }

            if (W.IsReady() && Player.Mana > WMANA && useW)
            {
                if (MinionN.Distance(Player) < W.Range && Player.ManaPercent >= Config.Item("Annie.WMiniManaJungleClear").GetValue<Slider>().Value)
                {
                    W.Cast(MinionN);
                }

            }



        }
        #endregion

        #region Check Tibers
        private static bool Tibers
        {
            get { return ObjectManager.Player.HasBuff("infernalguardiantimer"); }
        }
        #endregion

        #region GetAnniePassive
        public static bool GetPassiveCount()
        {
            var buffs = Player.Buffs.Where(buff => (buff.Name.ToLower() == "pyromania" || buff.Name.ToLower() == "pyromania_particle"));
            if (buffs.Any())
            {
                var buff = buffs.First();
                if (buff.Name.ToLower() == "pyromania_particle")
                    return true;
                else
                    return false;
            }
            return false;
        }
        #endregion

        #region KillSteal
        public static void KillSteal()
        {
            var UseStunKS = Config.Item("Annie.UseStunKS").GetValue<bool>();
            var UseStunKS1v1 = Config.Item("Annie.UseStunKS1v1").GetValue<bool>();
            var UseIgniteKS = Config.Item("Annie.UseIgniteKS").GetValue<bool>();
            var UseQKS = Config.Item("Annie.UseQKS").GetValue<bool>();
            var UseWKS = Config.Item("Annie.UseWKS").GetValue<bool>();
            var UseRKS = Config.Item("Annie.UseRKS").GetValue<bool>();

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(target => !target.IsMe && target.Team != ObjectManager.Player.Team))
            {

                if (!target.HasBuff("SionPassiveZombie") && !target.HasBuffOfType(BuffType.Invulnerability) && !target.HasBuffOfType(BuffType.SpellImmunity))
                {

                    if (UseStunKS)
                    {
                        if (!UseStunKS1v1 || (UseStunKS1v1 && Player.CountEnemiesInRange(1300) == 1))
                        {
                            if (UseQKS && Q.IsReady() && Player.Mana >= QMANA && target.Health < Q.GetDamage(target) && Player.Distance(target) <= Q.Range && !target.IsDead && target.IsValidTarget())
                            {
                                Q.CastOnUnit(target, true);
                                return;
                            }
                            if (UseWKS && W.IsReady() && Player.Mana >= WMANA && target.Health < W.GetDamage(target) && Player.Distance(target) < W.Range && !target.IsDead && target.IsValidTarget())
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            if (UseWKS && UseQKS && W.IsReady() && Q.IsReady() && Player.Mana >= WMANA + QMANA && target.Health < W.GetDamage(target) + Q.GetDamage(target) && Player.Distance(target) < W.Range && !target.IsDead && target.IsValidTarget())
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            if (UseIgniteKS && Ignite.Slot != SpellSlot.Unknown && target.Health < Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) <= Ignite.Range && !target.IsDead && target.IsValidTarget())
                            {
                                Ignite.Cast(target, true);
                                return;
                            }
                            if (UseQKS && UseIgniteKS && Q.IsReady() && Ignite.Slot != SpellSlot.Unknown && Player.Mana >= QMANA && target.Health < Q.GetDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) < Ignite.Range && !target.IsDead && target.IsValidTarget())
                            {
                                Q.CastOnUnit(target, true);
                                return;
                            }
                            if (UseWKS && UseIgniteKS && W.IsReady() && Ignite.Slot != SpellSlot.Unknown && Player.Mana >= WMANA && target.Health < W.GetDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) < Ignite.Range && !target.IsDead && target.IsValidTarget())
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            if (UseWKS && UseQKS && UseIgniteKS && W.IsReady() && Q.IsReady() && Ignite.Slot != SpellSlot.Unknown && Player.Mana >= WMANA + QMANA && target.Health < W.GetDamage(target) + Q.GetDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) < W.Range && !target.IsDead && target.IsValidTarget())
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            if (UseRKS && R.IsReady() && Player.Mana >= RMANA && target.Health < R.GetDamage(target) && Player.Distance(target) < R.Range && !target.IsDead && target.IsValidTarget())
                            {
                                R.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            if (UseRKS && UseQKS && R.IsReady() && Q.IsReady() && Player.Mana >= RMANA + QMANA && target.Health < R.GetDamage(target) + Q.GetDamage(target) && Player.Distance(target) < R.Range && !target.IsDead && target.IsValidTarget())
                            {
                                R.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            if (UseRKS && UseWKS && R.IsReady() && W.IsReady() && Player.Mana >= RMANA + WMANA && target.Health < R.GetDamage(target) + W.GetDamage(target) && Player.Distance(target) < W.Range && !target.IsDead && target.IsValidTarget())
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            if (UseRKS && UseWKS && UseQKS && R.IsReady() && W.IsReady() && Q.IsReady() && Player.Mana >= RMANA + WMANA + QMANA && target.Health < R.GetDamage(target) + W.GetDamage(target) + Q.GetDamage(target) && Player.Distance(target) < W.Range && !target.IsDead && target.IsValidTarget())
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            if (UseRKS && UseWKS && UseQKS && UseIgniteKS && R.IsReady() && W.IsReady() && Q.IsReady() && Ignite.Slot != SpellSlot.Unknown && Player.Mana >= RMANA + WMANA + QMANA && target.Health < R.GetDamage(target) + W.GetDamage(target) + Q.GetDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) < Ignite.Range && !target.IsDead && target.IsValidTarget())
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                        }
                    }

                    if ((!UseStunKS && !StunUp))
                    {
                        if (UseQKS && Q.IsReady() && Player.Mana >= QMANA && target.Health < Q.GetDamage(target) && Player.Distance(target) <= Q.Range && !target.IsDead && target.IsValidTarget())
                        {
                            Q.CastOnUnit(target, true);
                            return;
                        }
                        if (UseWKS && W.IsReady() && Player.Mana >= WMANA && target.Health < W.GetDamage(target) && Player.Distance(target) < W.Range && !target.IsDead && target.IsValidTarget())
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        if (UseWKS && UseQKS && W.IsReady() && Q.IsReady() && Player.Mana >= WMANA + QMANA && target.Health < W.GetDamage(target) + Q.GetDamage(target) && Player.Distance(target) < W.Range && !target.IsDead && target.IsValidTarget())
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        if (UseIgniteKS && Ignite.Slot != SpellSlot.Unknown && target.Health < Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) <= Ignite.Range && !target.IsDead && target.IsValidTarget())
                        {
                            Ignite.Cast(target, true);
                            return;
                        }
                        if (UseQKS && UseIgniteKS && Q.IsReady() && Ignite.Slot != SpellSlot.Unknown && Player.Mana >= QMANA && target.Health < Q.GetDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) < Ignite.Range && !target.IsDead && target.IsValidTarget())
                        {
                            Q.CastOnUnit(target, true);
                            return;
                        }
                        if (UseWKS && UseIgniteKS && W.IsReady() && Ignite.Slot != SpellSlot.Unknown && Player.Mana >= WMANA && target.Health < W.GetDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) < Ignite.Range && !target.IsDead && target.IsValidTarget())
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        if (UseWKS && UseQKS && UseIgniteKS && W.IsReady() && Q.IsReady() && Ignite.Slot != SpellSlot.Unknown && Player.Mana >= WMANA + QMANA && target.Health < W.GetDamage(target) + Q.GetDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) < W.Range && !target.IsDead && target.IsValidTarget())
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        if (UseRKS && R.IsReady() && Player.Mana >= RMANA && target.Health < R.GetDamage(target) && Player.Distance(target) < R.Range && !target.IsDead && target.IsValidTarget())
                        {
                            R.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        if (UseRKS && UseQKS && R.IsReady() && Q.IsReady() && Player.Mana >= RMANA + QMANA && target.Health < R.GetDamage(target) + Q.GetDamage(target) && Player.Distance(target) < R.Range && !target.IsDead && target.IsValidTarget())
                        {
                            R.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        if (UseRKS && UseWKS && R.IsReady() && W.IsReady() && Player.Mana >= RMANA + WMANA && target.Health < R.GetDamage(target) + W.GetDamage(target) && Player.Distance(target) < W.Range && !target.IsDead && target.IsValidTarget())
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        if (UseRKS && UseWKS && UseQKS && R.IsReady() && W.IsReady() && Q.IsReady() && Player.Mana >= RMANA + WMANA + QMANA && target.Health < R.GetDamage(target) + W.GetDamage(target) + Q.GetDamage(target) && Player.Distance(target) < W.Range && !target.IsDead && target.IsValidTarget())
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        if (UseRKS && UseWKS && UseQKS && UseIgniteKS && R.IsReady() && W.IsReady() && Q.IsReady() && Ignite.Slot != SpellSlot.Unknown && Player.Mana >= RMANA + WMANA + QMANA && target.Health < R.GetDamage(target) + W.GetDamage(target) + Q.GetDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) < Ignite.Range && !target.IsDead && target.IsValidTarget())
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                    }
                }

            }
        }
        #endregion

        #region PlayerDamage
        public static float getComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Player.Mana >= RMANA)
            {
                damage += (float)Damage.GetSpellDamage(Player, hero, SpellSlot.R);
            }
            if (Player.Mana >= (QMANA * 3) + RMANA)
            {
                damage += Damage.GetSpellDamage(Player, hero, SpellSlot.Q) * 3;
            }
            if (Player.Mana >= (QMANA * 3) + RMANA + WMANA)
            {
                damage += (float)Damage.GetSpellDamage(Player, hero, SpellSlot.W);
            }
            if (Player.Spellbook.CanUseSpell(Player.GetSpellSlot("summonerdot")) == SpellState.Ready)
            {
                damage += (float)Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            return (float)damage;
        }


        public static float getComboDamageNoUlt(AIHeroClient hero)
        {
            double damage = 0;
            if (Player.Mana >= (QMANA * 2))
            {
                damage += Damage.GetSpellDamage(Player, hero, SpellSlot.Q) * 2.5;
            }
            if (Player.Mana >= (QMANA * 2) + WMANA)
            {
                damage += (float)Damage.GetSpellDamage(Player, hero, SpellSlot.W);
            }
            if (Player.Spellbook.CanUseSpell(Player.GetSpellSlot("summonerdot")) == SpellState.Ready)
            {
                damage += (float)Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            return (float)damage;
        }

        public static float getMiniComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Player.Mana >= QMANA)
            {
                damage += Damage.GetSpellDamage(Player, hero, SpellSlot.Q) * 1.5;
            }
            if (Player.Mana >= QMANA + WMANA)
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

            if (Config.Item("Annie.AutoPotion").GetValue<bool>() && !Player.InFountain() && !Player.IsRecalling() && !Player.IsDead)
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
            int qL = Player.Spellbook.GetSpell(SpellSlot.Q).Level + qOff;
            int wL = Player.Spellbook.GetSpell(SpellSlot.W).Level + wOff;
            int eL = Player.Spellbook.GetSpell(SpellSlot.E).Level + eOff;
            int rL = Player.Spellbook.GetSpell(SpellSlot.R).Level + rOff;
            if (qL + wL + eL + rL < ObjectManager.Player.Level)
            {
                int[] level = new int[] { 0, 0, 0, 0 };
                for (int i = 0; i < ObjectManager.Player.Level; i++)
                {
                    level[abilitySequence[i] - 1] = level[abilitySequence[i] - 1] + 1;
                }
                if (qL < level[0]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (wL < level[1]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (eL < level[2]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (rL < level[3]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);

            }
        }
        #endregion

        #region RLogic
        public static void RLogic()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            if (Player.CountEnemiesInRange(1300) > 1)
            {
                foreach (PredictionOutput RPrediction in ObjectManager.Get<AIHeroClient>()
                .Where(x => x.IsValidTarget(R.Range)).Select(x => R.GetPrediction(x, true)).Where(pred => pred.Hitchance >= HitChance.High && pred.AoeTargetsHitCount >= 2))
                {
                    if (StunUp)
                    {
                        R.Cast(RPrediction.CastPosition);
                        return;
                    }

                    if (Player.HealthPercent <= target.HealthPercent)
                    {
                        if (Player.HealthPercent < 50)
                        {
                            R.Cast(RPrediction.CastPosition);
                            return;
                        }
                        return;
                    }
                    return;

                }
                if (target.CountAlliesInRange(R.Width) == 0)
                {
                    if (Player.HealthPercent <= target.HealthPercent)
                    {
                        if (Player.HealthPercent < 50)
                        {
                            R.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                            return;
                        }
                        return;
                    }
                    return;
                }
                return;
            }

            if (Player.CountEnemiesInRange(1300) == 1)
            {
                if (Player.CountAlliesInRange(1300) >= 1 + 1)
                {
                    if (Player.HealthPercent <= target.HealthPercent)
                    {
                        if (Player.HealthPercent < 50)
                        {
                                R.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                                return;
                        }
                        return;
                    }
                    return;
                }

                if (Game.Time < 1300f)
                {
                    if (Player.CountAlliesInRange(1300) == 0 + 1)
                    {
                        if (getComboDamage(target) > target.Health)
                        {
                            if (CanEscapeWithFlash(target.ServerPosition.To2D()))
                            {
                                if (StunUp || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup))
                                {
                                    R.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                                    return;
                                }

                                if ((Player.Distance(target) > Q.Range || target.Health > Q.GetDamage(target) || !Q.IsReady()) && (Player.Distance(target) > W.Range || target.Health > W.GetDamage(target) || !W.IsReady()) && target.Health > Player.BaseAttackDamage && target.Health < R.GetDamage(target) && Config.Item("Annie.UseRKSEarlyGame").GetValue<bool>())
                                {
                                    R.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                                    return;
                                }
                                return;
                            }
                            if (getMiniComboDamage(target) < target.Health)
                            {
                                if (StunUp || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup))
                                {
                                    R.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                                }
                                return;
                            }

                            if (Player.HealthPercent <= target.HealthPercent)
                            {
                                if (Player.HealthPercent < 50)
                                {
                                    if (getMiniComboDamage(target) < target.Health)
                                    {
                                        R.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                                        return;
                                    }
                                    return;
                                }
                                return;
                            }
                            return;
                        }
                        return;
                    }
                    return;
                }

                if (Game.Time >= 1300f)
                {
                    if (Player.CountAlliesInRange(1300) == 0 + 1)
                    {
                        if (getComboDamage(target) > target.Health)
                        {
                            if (CanEscapeWithFlash(target.ServerPosition.To2D()))
                            {
                                if (StunUp || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup))
                                {
                                    if (getMiniComboDamage(target) < target.Health)
                                    {
                                        R.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                                        return;
                                    }
                                    return;
                                }
                                return;
                            }
                            if (getComboDamageNoUlt(target) < target.Health)
                            {
                                if (StunUp || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup))
                                {
                                    R.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                                }
                                return;
                            }

                            if (Player.HealthPercent <= target.HealthPercent)
                            {
                                if (Player.HealthPercent < 50)
                                {
                                    if (getMiniComboDamage(target) < target.Health)
                                    {
                                        R.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                                        return;
                                    }
                                    return;
                                }
                                return;
                            }
                            return;
                        }
                        return;
                    }
                    return;
                }
                return;
            }
            return;
        }
        #endregion

        #region FlashRLogic
        public static void FlashRLogic()
        {
            if (R.IsReady() && FlashSlot.IsReady() && Player.Mana >= RMANA && StunUp)
            {
                foreach (PredictionOutput RPrediction in ObjectManager.Get<AIHeroClient>()
                .Where(x => x.IsValidTarget(R.Range + 425)).Select(x => R.GetPrediction(x, true)).Where(pred => pred.Hitchance >= HitChance.High && pred.AoeTargetsHitCount >= Config.Item("Annie.FlashREnemiesHitCount").GetValue<Slider>().Value))
                {
                    Player.Spellbook.CastSpell(FlashSlot, RPrediction.CastPosition);
                    LeagueSharp.Common.Utility.DelayAction.Add(10, () => R.Cast(RPrediction.CastPosition));
                }
            }
        }
        #endregion

        public static bool CanEscapeWithFlash(Vector2 pos)
        {
            foreach (Obj_AI_Turret turret in ObjectManager.Get<Obj_AI_Turret>().Where(turret => turret.IsEnemy && turret.Health > 0))
            {
                if (pos.Distance(turret.Position.To2D()) < (1800 + Player.BoundingRadius))
                    return true;
            }
            return false;
        }
    }
}
