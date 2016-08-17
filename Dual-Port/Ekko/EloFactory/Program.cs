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

using EloBuddy; namespace EloFactory_Ekko
{
    internal class Program
    {
        public const string ChampionName = "Ekko";

        public static Orbwalking.Orbwalker Orbwalker;

        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite = new Spell(SpellSlot.Unknown, 600);
        public static Obj_GeneralParticleEmitter EkkoUlt { get; set; }

        public static float QMANA;
        public static float WMANA;
        public static float EMANA;
        public static float RMANA;

        public static Items.Item HealthPotion = new Items.Item(2003, 0);
        public static Items.Item ManaPotion = new Items.Item(2004, 0);
        public static Items.Item CrystallineFlask = new Items.Item(2041, 0);
        public static Items.Item BiscuitofRejuvenation = new Items.Item(2010, 0);

        public static Items.Item WoogletsWitchcap = new Items.Item(3090, 0);
        public static Items.Item ZhonyasHourglass = new Items.Item(3157, 0);

        public static Menu Config;

        private static AIHeroClient Player;

        public static int[] abilitySequence;
        public static int qOff = 0, wOff = 0, eOff = 0, rOff = 0;
        public static void Game_OnGameLoad()
        {
            Player = ObjectManager.Player;

            if (Player.ChampionName != ChampionName) return;

            Q = new Spell(SpellSlot.Q, 750f);
            W = new Spell(SpellSlot.W, 1620f);
            E = new Spell(SpellSlot.E, 400f);
            R = new Spell(SpellSlot.R, 400f);

            Q.SetSkillshot(0.25f, 60f, 2200f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.5f, 500f, 1000f, false, SkillshotType.SkillshotCircle);

            var ignite = Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "summonerdot");
            if (ignite != null)
                Ignite.Slot = ignite.Slot;

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            abilitySequence = new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };

            EkkoUlt = ObjectManager.Get<Obj_GeneralParticleEmitter>().FirstOrDefault(x => x.Name.Equals("Ekko_Base_R_TrailEnd.troy"));

            Config = new Menu(ChampionName + " By LuNi", ChampionName + " By LuNi", true);

            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddSubMenu(new Menu("KS Mode", "KS Mode"));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Ekko.UseIgniteKS", "KS With Ignite").SetValue(true));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Ekko.UseQKS", "KS With Q").SetValue(true));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("Ekko.UseEKS", "KS With E").SetValue(true));
            Config.SubMenu("Combo").AddSubMenu(new Menu("Auto R", "Auto R"));
            Config.SubMenu("Combo").SubMenu("Auto R").AddItem(new MenuItem("Ekko.RAuto", "Use Auto R").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Auto R").AddSubMenu(new Menu("R Auto Settings", "R Auto Settings"));
            Config.SubMenu("Combo").SubMenu("Auto R").SubMenu("R Auto Settings").AddItem(new MenuItem("Ekko.UseBurstRComboAuto", "Use Burst Mode R In Auto R").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Auto R").SubMenu("R Auto Settings").AddItem(new MenuItem("Ekko.MinimumHPBurstRAuto", "Minimum HP Percent To Use Burst Mode R In Auto R").SetValue(new Slider(40, 0, 100)));
            Config.SubMenu("Combo").SubMenu("Auto R").SubMenu("R Auto Settings").AddItem(new MenuItem("Ekko.MinimumEnemiesBurstRAuto", "Minimum Enemies in R Range To Use Burst Mode R In Auto R").SetValue(new Slider(2, 1, 5)));
            Config.SubMenu("Combo").SubMenu("Auto R").SubMenu("R Auto Settings").AddItem(new MenuItem("Ekko.MinimumEnemiesDrangeRAuto", "Maximum Enemies in Dangerous Range Around R Swap Position (Enemies In R Range Included)").SetValue(new Slider(3, 1, 5)));
            Config.SubMenu("Combo").SubMenu("Auto R").SubMenu("R Auto Settings").AddItem(new MenuItem("Ekko.UseSafeRComboAuto", "Use Safe Mode R In Auto R").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Auto R").SubMenu("R Auto Settings").AddItem(new MenuItem("Ekko.MinimumHPSafeRAuto", "Minimum HP Percent To Use Safe Mode R In Auto R").SetValue(new Slider(20, 0, 100)));
            Config.SubMenu("Combo").AddItem(new MenuItem("Ekko.UseQCombo", "Use Q In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Ekko.UseWCombo", "Use W In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Ekko.UseECombo", "Use E In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Ekko.UseRCombo", "Use R In Combo").SetValue(true));
            Config.SubMenu("Combo").AddSubMenu(new Menu("Items Activator", "Items Activator"));
            Config.SubMenu("Combo").SubMenu("Items Activator").AddSubMenu(new Menu("Use Zhonya's Hourglass", "Use Zhonya's Hourglass"));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Zhonya's Hourglass").AddItem(new MenuItem("Ekko.useZhonyasHourglass", "Use Zhonya's Hourglass").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Zhonya's Hourglass").AddItem(new MenuItem("Ekko.MinimumHPtoZhonyasHourglass", "Minimum Health Percent To Use Zhonya's Hourglass").SetValue(new Slider(30, 0, 100)));
            Config.SubMenu("Combo").SubMenu("Items Activator").AddSubMenu(new Menu("Use Wooglet's Witchcap", "Use Wooglet's Witchcap"));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Wooglet's Witchcap").AddItem(new MenuItem("Ekko.useWoogletsWitchcap", "Use Wooglet's Witchcap").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Wooglet's Witchcap").AddItem(new MenuItem("Ekko.MinimumHPtoWoogletsWitchcap", "Minimum Health Percent To Use Wooglet's Witchcap").SetValue(new Slider(30, 0, 100)));
            Config.SubMenu("Combo").AddItem(new MenuItem("Ekko.AutoWOnStunTarget", "Auto Use W On Stunned Target").SetValue(true));

            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("Ekko.UseQHarass", "Use Q In Harass").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("Ekko.QMiniManaHarass", "Minimum Mana To Use Q In Harass").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("Harass").AddItem(new MenuItem("Ekko.QWhenEnemyCastHarass", "Use Q On Enemy AA/Spell In Harass").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("Ekko.UseEHarass", "Use E In Harass When 2 Stack On Target").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("Ekko.EMiniManaHarass", "Minimum Mana To Use E In Harass").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("Harass").AddItem(new MenuItem("Ekko.HarassActive", "Harass!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Harass").AddItem(new MenuItem("Ekko.HarassActiveT", "Harass (toggle)!").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));

            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Ekko.UseQLaneClear", "Use Q in LaneClear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Ekko.QMiniManaLaneClear", "Minimum Mana To Use Q In LaneClear").SetValue(new Slider(30, 0, 100)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Ekko.QLaneClearCount", "Minimum Minion To Use Q In LaneClear").SetValue(new Slider(3, 1, 6)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Ekko.UseWLaneClear", "Use W in LaneClear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Ekko.WMiniManaLaneClear", "Minimum Mana To Use W In LaneClear").SetValue(new Slider(70, 0, 100)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Ekko.WLaneClearCount", "Minimum Minion To Use W In LaneClear").SetValue(new Slider(4, 1, 6)));

            Config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Ekko.UseQJungleClear", "Use Q In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Ekko.QMiniManaJungleClear", "Minimum Mana To Use Q In JungleClear").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Ekko.UseWJungleClear", "Use W In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Ekko.WMiniManaJungleClear", "Minimum Mana To Use W In JungleClear").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Ekko.UseEJungleClear", "Use E In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Ekko.EMiniManaJungleClear", "Minimum Mana To Use E In JungleClear").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Ekko.SafeJungleClear", "Dont Use Spell In Jungle Clear If Enemy in Dangerous Range").SetValue(true));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddSubMenu(new Menu("Skin Changer", "Skin Changer"));
            Config.SubMenu("Misc").SubMenu("Skin Changer").AddItem(new MenuItem("Ekko.SkinChanger", "Use Skin Changer").SetValue(false));
            Config.SubMenu("Misc").SubMenu("Skin Changer").AddItem(new MenuItem("Ekko.SkinChangerName", "Skin choice").SetValue(new StringList(new[] { "Classic", "Sandstorm" })));
            Config.SubMenu("Misc").AddItem(new MenuItem("Ekko.WInterrupt", "Interrupt Spells With W").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Ekko.AutoQEGC", "Auto Q On Gapclosers").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Ekko.AutoWEGC", "Auto W On Gapclosers").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Ekko.AutoPotion", "Use Auto Potion").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Ekko.AutoLevelSpell", "Auto Level Spell").SetValue(true));

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q range").SetValue(new Circle(true, Color.Indigo)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("WRange", "W range").SetValue(new Circle(true, Color.Green)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("ERange", "E range").SetValue(new Circle(true, Color.Green)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RRange", "R range").SetValue(new Circle(true, Color.Gold)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawOrbwalkTarget", "Draw Orbwalk target").SetValue(true));

            Config.AddToMainMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            CustomEvents.Unit.OnDash += Unit_OnDash;

        }

        #region ToogleOrder Game_OnUpdate
        public static void Game_OnGameUpdate(EventArgs args)
        {
            //Player.SetSkin(Player.BaseSkinName, Config.Item("Ekko.SkinChanger").GetValue<bool>() ? Config.Item("Ekko.SkinChangerName").GetValue<StringList>().SelectedIndex : Player.SkinId);

            if (Config.Item("Ekko.AutoLevelSpell").GetValue<bool>()) LevelUpSpells();

            if (Player.IsDead) return;

            if (Player.GetBuffCount("Recall") == 1) return;

            ManaManager();
            PotionManager();

            KillSteal();

            #region Sort R Auto

            if (Config.Item("Ekko.RAuto").GetValue<bool>() && R.IsReady())
            {
                if (Config.Item("Ekko.UseBurstRComboAuto").GetValue<bool>() && Player.HealthPercent >= Config.Item("Ekko.MinimumHPBurstRAuto").GetValue<Slider>().Value)
                {
                    var EnemiesCNoDash = HeroManager.Enemies.Count(x => x.IsValid<AIHeroClient>() && x.IsValidTarget() && !x.IsDead && x.Distance(EkkoUlt.Position) < 400 && getComboDamageUlt(x) > x.Health);
                    var CountEnemiesIn800 = HeroManager.Enemies.Count(x => x.IsValid<AIHeroClient>() && x.IsValidTarget() && !x.IsDead && x.Distance(EkkoUlt.Position) < 800);
                    var CountAlliesIn1000 = HeroManager.Allies.Count(x => x.IsValid<AIHeroClient>() && x.IsValidTarget() && !x.IsDead && x.Distance(EkkoUlt.Position) < 1000);

                    var target = TargetSelector.GetTarget(850, TargetSelector.DamageType.Magical);
                    if (Player.CountEnemiesInRange(850) == 0 || getComboDamageNoUlt(target) < target.Health)
                    {
                        if (EnemiesCNoDash >= Config.Item("Ekko.MinimumEnemiesBurstRAuto").GetValue<Slider>().Value && CountEnemiesIn800 <= Config.Item("Ekko.MinimumEnemiesDrangeRAuto").GetValue<Slider>().Value)
                        {
                            R.Cast();
                        }
                    }
                }

            }
            #endregion

            if (Config.Item("Ekko.AutoWOnStunTarget").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                if (target.IsValidTarget() && target.HasBuffOfType(BuffType.Stun) && Player.Distance(target) < W.Range)
                {
                    W.CastIfHitchanceEquals(target, HitChance.High, true);
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                JungleClear();
                LaneClear();
            }

            if (Config.Item("Ekko.HarassActive").GetValue<KeyBind>().Active || Config.Item("Ekko.HarassActiveT").GetValue<KeyBind>().Active)
            {
                Harass();
            }

        }
        #endregion

        #region Interupt OnProcessSpellCast
        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {

            double ShouldUseOn = ShouldUse(args.SData.Name);
            if (unit.Team != ObjectManager.Player.Team && ShouldUseOn >= 0f && unit.IsValidTarget(Q.Range))
            {

                if (Config.Item("Ekko.WInterrupt").GetValue<bool>() && W.IsReady() && Player.Mana >= WMANA && Player.Distance(unit) <= W.Range)
                {
                    W.CastIfHitchanceEquals(unit, HitChance.High, true);
                }

            }

            if (Config.Item("Ekko.RAuto").GetValue<bool>() && R.IsReady())
            {
                if (Config.Item("Ekko.UseSafeRComboAuto").GetValue<bool>() && Player.HealthPercent <= Config.Item("Ekko.MinimumHPSafeRAuto").GetValue<Slider>().Value)
                {
                    if ((unit.IsValid<AIHeroClient>() || unit.IsValid<Obj_AI_Turret>()) && unit.IsEnemy && args.Target.IsMe)
                    {
                        if (Player.CountEnemiesInRange(1300) > 1)
                        {
                            if (Player.CountAlliesInRange(1300) >= 1 + 1)
                            {
                                R.Cast();
                                return;
                            }
                            if (Player.CountAlliesInRange(1300) == 0 + 1)
                            {
                                if (unit.GetSpellDamage(Player, args.SData.Name) >= Player.Health || (unit.GetAutoAttackDamage(Player) >= Player.Health && args.SData.IsAutoAttack()))
                                {
                                    R.Cast();
                                    return;
                                }
                            }
                        }
                        else if (unit.GetSpellDamage(Player, args.SData.Name) >= Player.Health || (unit.GetAutoAttackDamage(Player) >= Player.Health && args.SData.IsAutoAttack()))
                        {
                            R.Cast();
                            return;
                        }
                    }
                }

            }

            if ((unit.IsValid<AIHeroClient>() || unit.IsValid<Obj_AI_Turret>()) && unit.IsEnemy && args.Target.IsMe && Config.Item("Ekko.useZhonyasHourglass").GetValue<bool>() && ZhonyasHourglass.IsReady() && Player.HealthPercent <= Config.Item("Ekko.MinimumHPtoZhonyasHourglass").GetValue<Slider>().Value)
            {
                if (Player.CountEnemiesInRange(1300) > 1)
                {
                    if (Player.CountAlliesInRange(1300) >= 1 + 1)
                    {
                        ZhonyasHourglass.Cast();
                        return;
                    }
                    if (Player.CountAlliesInRange(1300) == 0 + 1)
                    {
                        if (unit.GetSpellDamage(Player, args.SData.Name) >= Player.Health || (unit.GetAutoAttackDamage(Player) >= Player.Health && args.SData.IsAutoAttack()))
                        {
                            ZhonyasHourglass.Cast();
                            return;
                        }
                    }
                }
                if (Player.CountEnemiesInRange(1300) == 1)
                {
                    if (unit.GetSpellDamage(Player, args.SData.Name) >= Player.Health || (unit.GetAutoAttackDamage(Player) >= Player.Health && args.SData.IsAutoAttack()))
                    {
                        ZhonyasHourglass.Cast();
                        return;
                    }
                }

            }

            if ((unit.IsValid<AIHeroClient>() || unit.IsValid<Obj_AI_Turret>()) && unit.IsEnemy && args.Target.IsMe && Config.Item("Ekko.useWoogletsWitchcap").GetValue<bool>() && WoogletsWitchcap.IsReady() && Player.HealthPercent <= Config.Item("Ekko.MinimumHPtoWoogletsWitchcap").GetValue<Slider>().Value)
            {
                if (Player.CountEnemiesInRange(1300) > 1)
                {
                    if (Player.CountAlliesInRange(1300) >= 1 + 1)
                    {
                        WoogletsWitchcap.Cast();
                        return;
                    }
                    if (Player.CountAlliesInRange(1300) == 0 + 1)
                    {
                        if (unit.GetSpellDamage(Player, args.SData.Name) >= Player.Health || (unit.GetAutoAttackDamage(Player) >= Player.Health && args.SData.IsAutoAttack()))
                        {
                            WoogletsWitchcap.Cast();
                            return;
                        }
                    }
                }
                if (Player.CountEnemiesInRange(1300) == 1)
                {
                    if (unit.GetSpellDamage(Player, args.SData.Name) >= Player.Health || (unit.GetAutoAttackDamage(Player) >= Player.Health && args.SData.IsAutoAttack()))
                    {
                        WoogletsWitchcap.Cast();
                        return;
                    }
                }

            }


            if (Config.Item("Ekko.HarassActive").GetValue<KeyBind>().Active || Config.Item("Ekko.HarassActiveT").GetValue<KeyBind>().Active)
            {
                if (Config.Item("Ekko.QWhenEnemyCastHarass").GetValue<bool>() && (unit.IsValid<AIHeroClient>() && !unit.IsValid<Obj_AI_Turret>()) && unit.IsEnemy && args.Target.IsMe && Q.IsReady() && Player.Distance(unit) <= Q.Range)
                {
                    Q.CastIfHitchanceEquals(unit, HitChance.High, true);
                }
                Harass();
            }



        }
        #endregion

        #region AntiGapCloser
        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {

            if (Config.Item("Ekko.AutoQEGC").GetValue<bool>() && Q.IsReady() && (Player.Mana >= EMANA + QMANA) && Player.Distance(gapcloser.Sender) < Q.Range)
            {
                Q.CastIfHitchanceEquals(gapcloser.Sender, HitChance.High, true);
            }

            if (Config.Item("Ekko.AutoWEGC").GetValue<bool>() && W.IsReady() && (Player.Mana >= EMANA + WMANA) && Player.Distance(gapcloser.Sender) < Q.Range)
            {
                W.Cast(Player.ServerPosition, true);
            }

        }
        #endregion

        #region On Dash
        static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var useQ = Config.Item("Ekko.UseQCombo").GetValue<bool>();
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (!sender.IsEnemy) return;

            if (sender.NetworkId == target.NetworkId)
            {

                if (useQ && Q.IsReady() && Player.Mana >= QMANA && args.EndPos.Distance(Player) <= Q.Range)
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

        #region GameObject

        public static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {

            var particle = sender as Obj_GeneralParticleEmitter;
            if (particle != null)
            {
                if (particle.Name.Equals("Ekko_Base_R_TrailEnd.troy"))
                {
                    EkkoUlt = particle;
                }
            }
        }

        public static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var particle = sender as Obj_GeneralParticleEmitter;
            if (particle != null)
            {
                if (particle.Name.Equals("Ekko_Base_R_TrailEnd.troy"))
                {
                    EkkoUlt = null;
                }
            }
        }

        #endregion

        #region Combo
        public static void Combo()
        {

            var useQ = Config.Item("Ekko.UseQCombo").GetValue<bool>();
            var useW = Config.Item("Ekko.UseWCombo").GetValue<bool>();
            var useE = Config.Item("Ekko.UseECombo").GetValue<bool>();
            var useR = Config.Item("Ekko.UseRCombo").GetValue<bool>();

            #region Sort R combo mode
            if (useR && R.IsReady())
            {
                RLogic();
            }
            #endregion

            var target = TargetSelector.GetTarget(Q.Range + R.Range, TargetSelector.DamageType.Magical);
            if (target.IsValidTarget())
            {

                #region Sort W combo mode
                if (useW && W.IsReady() && Player.Mana >= WMANA)
                {
                    WLogic();
                }
                #endregion

                #region Sort E combo mode
                if (useE && E.IsReady() && Player.Mana >= EMANA)
                {
                    ELogic();
                }
                #endregion

                #region Sort Q combo mode
                if (useQ && Q.IsReady() && Player.Mana >= QMANA)
                {
                    QLogic();
                }
                #endregion

            }

        }
        #endregion

        #region Harass
        public static void Harass()
        {

            var targetH = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            var useQ = Config.Item("Ekko.UseQHarass").GetValue<bool>();
            var useE = Config.Item("Ekko.UseQHarass").GetValue<bool>();
            var QMinMana = Config.Item("Ekko.QMiniManaHarass").GetValue<Slider>().Value;
            var EMinMana = Config.Item("Ekko.EMiniManaHarass").GetValue<Slider>().Value;

            if (useQ && Q.IsReady() && Player.Distance(targetH) <= Q.Range && Player.ManaPercent >= QMinMana)
            {
                Q.CastIfHitchanceEquals(targetH, HitChance.High, true);
            }

            if (useE && E.IsReady() && targetH.GetBuffCount("EkkoStacks") == 2 && Player.Distance(targetH) <= E.Range + 450 && Player.ManaPercent >= EMinMana)
            {
                E.Cast(targetH.ServerPosition, true);
            }

        }
        #endregion

        #region LaneClear
        public static void LaneClear()
        {

            var useQ = Config.Item("Ekko.UseQLaneClear").GetValue<bool>();
            var useW = Config.Item("Ekko.UseWLaneClear").GetValue<bool>();

            var QMinMana = Config.Item("Ekko.QMiniManaLaneClear").GetValue<Slider>().Value;
            var WMinMana = Config.Item("Ekko.WMiniManaLaneClear").GetValue<Slider>().Value;

            if (useQ && Q.IsReady() && Player.Mana >= QMANA)
            {
                var allMinionsQ = MinionManager.GetMinions(Player.Position, 1000, MinionTypes.All, MinionTeam.Enemy);

                if (allMinionsQ.Any())
                {
                    var farmAll = Q.GetCircularFarmLocation(allMinionsQ, 150);
                    if (farmAll.MinionsHit >= Config.Item("Ekko.QLaneClearCount").GetValue<Slider>().Value)
                    {
                        Q.Cast(farmAll.Position, true);
                    }
                }
            }

            if (useW && W.IsReady() && Player.Mana >= WMANA && Player.ManaPercent >= WMinMana)
            {
                var allMinionsW = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy);

                if (allMinionsW.Any())
                {
                    var farmAll = W.GetCircularFarmLocation(allMinionsW, 350);
                    if (farmAll.MinionsHit >= Config.Item("Ekko.WLaneClearCount").GetValue<Slider>().Value)
                    {
                        W.Cast(farmAll.Position, true);
                    }
                }
            }

        }
        #endregion

        #region JungleClear
        public static void JungleClear()
        {

            var useQ = Config.Item("Ekko.UseQJungleClear").GetValue<bool>();
            var useW = Config.Item("Ekko.UseWJungleClear").GetValue<bool>();
            var useE = Config.Item("Ekko.UseWJungleClear").GetValue<bool>();

            var QMinMana = Config.Item("Ekko.QMiniManaJungleClear").GetValue<Slider>().Value;
            var WMinMana = Config.Item("Ekko.WMiniManaJungleClear").GetValue<Slider>().Value;
            var EMinMana = Config.Item("Ekko.EMiniManaJungleClear").GetValue<Slider>().Value;

            var MinionN = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (!MinionN.IsValidTarget() || MinionN == null)
            {
                LaneClear();
                return;
            }

            if (Config.Item("Ekko.SafeJungleClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;

            if (useQ && Q.IsReady() && Player.Mana >= QMANA && Player.ManaPercent >= QMinMana)
            {
                var allMonsterQ = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                var farmAll = Q.GetLineFarmLocation(allMonsterQ, Q.Width);
                if (farmAll.MinionsHit >= 1)
                {
                    Q.Cast(farmAll.Position, true);
                }
            }

            if (useW && W.IsReady() && Player.Mana >= WMANA && Player.ManaPercent >= WMinMana)
            {
                var allMonsterW = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                var farmAll = W.GetCircularFarmLocation(allMonsterW, 350);
                if (farmAll.MinionsHit >= 1)
                {
                    W.Cast(farmAll.Position, true);
                }
            }


            if (useE && E.IsReady() && Player.Mana >= EMANA && Player.ManaPercent >= EMinMana)
            {
                var MinionE = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

                E.Cast(MinionE.ServerPosition, true);
            }

        }
        #endregion

        #region KillSteal
        public static void KillSteal()
        {
            var useIgniteKS = Config.Item("Ekko.UseIgniteKS").GetValue<bool>();
            var useQKS = Config.Item("Ekko.UseQKS").GetValue<bool>();
            var useEKS = Config.Item("Ekko.UseEKS").GetValue<bool>();

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(target => !target.IsMe && target.Team != ObjectManager.Player.Team))
            {

                if (useQKS && Q.IsReady() && Player.Mana >= QMANA && target.Health < Q.GetDamage(target) && Player.Distance(target) <= Q.Range && !target.IsDead && target.IsValidTarget())
                {
                    Q.CastIfHitchanceEquals(target, HitChance.High, true);
                    return;
                }

                if (useEKS && E.IsReady() && Player.Mana >= EMANA && target.Health < E.GetDamage(target) && Player.Distance(target) <= E.Range + 450 && !target.IsDead && target.IsValidTarget())
                {
                    E.Cast(target.ServerPosition, true);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    return;
                }

                if (useIgniteKS && Ignite.Slot != SpellSlot.Unknown && Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > target.Health && target.IsValidTarget(Ignite.Range))
                {
                    Ignite.Cast(target, true);
                }

                if (useQKS && useEKS && Q.IsReady() && E.IsReady() && Player.Mana >= QMANA + EMANA && target.Health < E.GetDamage(target) + Q.GetDamage(target) && Player.Distance(target) <= E.Range + 450 && !target.IsDead && target.IsValidTarget())
                {
                    E.Cast(target.ServerPosition, true);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    return;
                }

                if (useQKS && useIgniteKS && Ignite.Slot != SpellSlot.Unknown && Q.IsReady() && Player.Mana >= QMANA && target.Health < Q.GetDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) < 600 && !target.IsDead && target.IsValidTarget())
                {
                    Q.CastIfHitchanceEquals(target, HitChance.High, true);
                    return;
                }

                if (useEKS && useIgniteKS && Ignite.Slot != SpellSlot.Unknown && E.IsReady() && Player.Mana >= EMANA && target.Health < E.GetDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) <= E.Range + 450 && !target.IsDead && target.IsValidTarget())
                {
                    E.Cast(target.ServerPosition, true);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    return;
                }

                if (useQKS && useEKS && useIgniteKS && Ignite.Slot != SpellSlot.Unknown && Q.IsReady() && E.IsReady() && Player.Mana >= QMANA + EMANA && target.Health < Q.GetDamage(target) + E.GetDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) <= E.Range + 450 && !target.IsDead && target.IsValidTarget())
                {
                    E.Cast(target.ServerPosition, true);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    return;
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

            if (Config.Item("Ekko.AutoPotion").GetValue<bool>() && !Player.InFountain() && !Player.IsRecalling() && !Player.IsDead)
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
                if (menuItem.Active && spell.Slot != SpellSlot.R)
                {
                    Render.Circle.DrawCircle(Player.Position, spell.Range, menuItem.Color);
                }

                if (menuItem.Active && spell.Slot == SpellSlot.R && R.Level > 0)
                {
                    Render.Circle.DrawCircle(EkkoUlt.Position, spell.Range, menuItem.Color);
                }

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

        #region QLogic
        public static void QLogic()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (Player.CountEnemiesInRange(1300) > 1)
            {
                if (Player.CountAlliesInRange(1300) >= 1 + 1)
                {
                    if (target.CountAlliesInRange(Q.Width) >= 1)
                    {
                        if (target.GetBuffCount("EkkoStacks") == 2)
                        {
                            Q.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                            return;
                        }
                        if (target.GetBuffCount("EkkoStacks") < 2)
                        {
                            Q.Cast(target, true, true);
                            return;
                        }
                        return;
                    }
                    if (target.CountAlliesInRange(Q.Width) == 0)
                    {
                        Q.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                        return;
                    }
                    return;
                }
                if (Player.CountAlliesInRange(1300) == 0 + 1)
                {
                    if (target.CountAlliesInRange(Q.Width) >= 1)
                    {
                        Q.Cast(target, true, true);
                        return;
                    }
                    if (target.CountAlliesInRange(Q.Width) == 0)
                    {
                        Q.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                        return;
                    }
                    return;
                }
                return;
            }

            if (Player.CountEnemiesInRange(1300) == 1)
            {
                Q.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                return;
            }
            return;
        }
        #endregion

        #region WLogic
        public static void WLogic()
        {
            var target = TargetSelector.GetTarget(E.Range + 450 + 200, TargetSelector.DamageType.Magical);

            if (Player.CountEnemiesInRange(1300) > 1)
            {
                if (target.CountAlliesInRange(W.Width) >= 1)
                {
                    if (Player.HealthPercent <= target.HealthPercent)
                    {
                        if (Player.CountEnemiesInRange(360) >= 1)
                        {
                            if (target.Distance(Player) <= 360)
                            {
                                W.Cast(Player.ServerPosition, true);
                                return;
                            }
                            if (target.Distance(Player) > 360)
                            {
                                W.Cast(target, true, true);
                                return;
                            }
                            return;
                        }
                        if (Player.CountEnemiesInRange(360) == 0)
                        {
                            W.Cast(target, true, true);
                            return;
                        }
                        return;
                    }

                    if (Player.HealthPercent > target.HealthPercent)
                    {
                        if (Player.HealthPercent >= 50)
                        {
                            if (target.GetBuffCount("EkkoStacks") == 2)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                                return;
                            }
                            if (target.GetBuffCount("EkkoStacks") < 2)
                            {
                                W.Cast(target, true, true);
                                return;
                            }
                            return;
                        }
                        if (Player.HealthPercent < 50)
                        {
                            if (Player.CountEnemiesInRange(360) >= 1)
                            {
                                if (target.Distance(Player) <= 360)
                                {
                                    W.Cast(Player.ServerPosition, true);
                                    return;
                                }
                                if (target.Distance(Player) > 360)
                                {
                                    W.Cast(target, true, true);
                                    return;
                                }
                                return;
                            }
                            if (Player.CountEnemiesInRange(360) == 0)
                            {
                                W.Cast(target, true, true);
                                return;
                            }
                            return;
                        }
                        return;
                    }
                    return;
                }

                if (target.CountAlliesInRange(W.Width) == 0)
                {
                    if (Player.CountEnemiesInRange(360) >= 1)
                    {
                        if (target.Distance(Player) <= 360)
                        {
                            W.Cast(Player.ServerPosition, true);
                            return;
                        }
                        if (target.Distance(Player) > 360)
                        {
                            W.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                            return;
                        }
                        return;
                    }
                    if (Player.CountEnemiesInRange(360) == 0)
                    {
                        W.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                        return;
                    }
                    return;
                }
                return;
            }

            if (Player.CountEnemiesInRange(1300) == 1)
            {
                W.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                return;
            }
            return;
        }
        #endregion

        #region ELogic
        public static void ELogic()
        {
            var target = TargetSelector.GetTarget(E.Range + 425, TargetSelector.DamageType.Magical);

            if (Player.Distance(target) > 260)
            {
                if (Player.HealthPercent >= 50 || R.IsReady())
                {
                    if (target.GetBuffCount("EkkoStacks") == 2)
                    {
                        E.Cast(target.ServerPosition, true);
                        return;
                    }
                    if (target.GetBuffCount("EkkoStacks") < 2)
                    {
                        E.Cast(target.ServerPosition, true);
                        return;
                    }
                    return;
                }
                if (Player.HealthPercent < 50 && !R.IsReady())
                {
                    if (target.GetBuffCount("EkkoStacks") == 2)
                    {
                        E.Cast(target.ServerPosition, true);
                        return;
                    }
                    return;
                }
                return;
            }
            return;
        }
        #endregion

        #region RLogic
        public static void RLogic()
        {
            var EnemiesCDash = HeroManager.Enemies.Count(x => x.IsValid<AIHeroClient>() && x.IsValidTarget() && !x.IsDead && x.Distance(EkkoUlt.Position) > 385 && x.Distance(EkkoUlt.Position) < 800 && getComboDamageNoUlt(x) > x.Health);
            var EnemiesCNoDash = HeroManager.Enemies.Count(x => x.IsValid<AIHeroClient>() && x.IsValidTarget() && !x.IsDead && x.Distance(EkkoUlt.Position) < 385 && getComboDamageUlt(x) > x.Health);
            var CountEnemiesIn800 = HeroManager.Enemies.Count(x => x.IsValid<AIHeroClient>() && x.IsValidTarget() && !x.IsDead && x.Distance(EkkoUlt.Position) < 800);
            var CountAlliesIn1000 = HeroManager.Allies.Count(x => x.IsValid<AIHeroClient>() && x.IsValidTarget() && !x.IsDead && x.Distance(EkkoUlt.Position) < 1000);
            var CountEnemiesIn1100 = HeroManager.Enemies.Count(x => x.IsValid<AIHeroClient>() && x.IsValidTarget() && !x.IsDead && x.Distance(EkkoUlt.Position) < 1100);
            var CountAlliesIn1300 = HeroManager.Allies.Count(x => x.IsValid<AIHeroClient>() && x.IsValidTarget() && !x.IsDead && x.Distance(EkkoUlt.Position) < 1300);

            var target = TargetSelector.GetTarget(850, TargetSelector.DamageType.Magical);
            if (Player.CountEnemiesInRange(850) == 0 || getComboDamageNoUlt(target) < target.Health)
            {
                if (EnemiesCNoDash >= 1 && CountEnemiesIn800 <= 2)
                {
                    R.Cast();
                }
                if (EnemiesCNoDash >= 1 && CountEnemiesIn800 > 2 && CountAlliesIn1000 >= CountEnemiesIn800)
                {
                    R.Cast();
                }

                if (EnemiesCDash >= 1 && CountEnemiesIn1100 <= 2 && E.IsReady())
                {
                    R.Cast();
                }
                if (EnemiesCDash >= 1 && CountEnemiesIn1100 > 2 && CountAlliesIn1300 >= CountEnemiesIn1100 && E.IsReady())
                {
                    R.Cast();
                }
            }
            
        }
        #endregion

        #region Player Damage
        public static float getComboDamageNoUlt(AIHeroClient hero)
        {
            double damage = 0;
            if (E.IsReady())
            {
                damage += Damage.GetSpellDamage(Player, hero, SpellSlot.E);
            }
            if (EMANA + QMANA >= Player.Mana)
            {
                damage += Damage.GetSpellDamage(Player, hero, SpellSlot.Q) * 4;
            }
            if (W.IsReady())
            {
                damage += (float)Damage.GetSpellDamage(Player, hero, SpellSlot.Q);
            }
            if (Player.Spellbook.CanUseSpell(Player.GetSpellSlot("summonerdot")) == SpellState.Ready)
            {
                damage += (float)Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            return (float)damage;
        }

        public static float getComboDamageUlt(AIHeroClient hero)
        {
            double damage = 0;
            if (R.IsReady())
            {
                damage += Damage.GetSpellDamage(Player, hero, SpellSlot.R);
            }
            if (E.IsReady())
            {
                damage += Damage.GetSpellDamage(Player, hero, SpellSlot.E);
            }
            if (EMANA + QMANA >= Player.Mana)
            {
                damage += Damage.GetSpellDamage(Player, hero, SpellSlot.Q) * 4;
            }
            if (W.IsReady())
            {
                damage += (float)Damage.GetSpellDamage(Player, hero, SpellSlot.Q);
            }
            if (Player.Spellbook.CanUseSpell(Player.GetSpellSlot("summonerdot")) == SpellState.Ready)
            {
                damage += (float)Player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            }
            return (float)damage;
        }
        #endregion

    }

}
