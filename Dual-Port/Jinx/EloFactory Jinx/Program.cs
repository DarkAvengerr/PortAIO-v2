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
 namespace EloFactory_Jinx
{
    internal class Program
    {
        public const string ChampionName = "Jinx";

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

        public static Menu Config;

        private static AIHeroClient Player;

        public static int[] abilitySequence;
        public static int qOff = 0, wOff = 0, eOff = 0, rOff = 0;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            Player = ObjectManager.Player;

            if (Player.ChampionName != ChampionName) return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 1300f);
            E = new Spell(SpellSlot.E, 840f);
            R = new Spell(SpellSlot.R, 2200f);

            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.7f, 120f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.6f, 140f, 1500f, false, SkillshotType.SkillshotLine);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            abilitySequence = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };

            Config = new Menu(ChampionName + " By LuNi", ChampionName + " By LuNi", true);

            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("Jinx.UseQCombo", "Use Q In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Jinx.UseWCombo", "Use W In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Jinx.UseECombo", "Use E In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Jinx.UseRCombo", "Use R In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Jinx.AutoEWhenEnemyCastAAM", "Use Auto E When Melee Enemy Cast AA On Me").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Jinx.AutoEWhenEnemyCastAAR", "Use Auto E When Ranged Enemy Cast AA On Me").SetValue(false));

            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("Jinx.UseWHarass", "Use W In Harass").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("Jinx.WMiniManaHarass", "Minimum Mana To Use W In Harass").SetValue(new Slider(70, 0, 100)));
            Config.SubMenu("Harass").AddItem(new MenuItem("Jinx.HarassActive", "Harass!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Harass").AddItem(new MenuItem("Jinx.HarassActiveT", "Harass (toggle)!").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));

            Config.AddSubMenu(new Menu("LastHit", "LastHit"));
            Config.SubMenu("LastHit").AddItem(new MenuItem("Jinx.UseWLastHit", "Use W In LastHit").SetValue(false));
            Config.SubMenu("LastHit").AddItem(new MenuItem("Jinx.WMiniManaLastHit", "Minimum Mana To Use W In LastHit").SetValue(new Slider(65, 0, 100)));
            Config.SubMenu("LastHit").AddItem(new MenuItem("Jinx.SafeWLastHit", "Never Use W In LastHit When Enemy Close To Your Position").SetValue(true));

            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Jinx.UseWLaneClear", "Use W in LaneClear").SetValue(false));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Jinx.WMiniManaLaneClear", "Minimum Mana To Use W In LaneClear").SetValue(new Slider(70, 0, 100)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("Jinx.SafeWLaneClear", "Never Use W In LaneClear When Enemy Close To Your Position").SetValue(true));

            Config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Jinx.UseQJungleClear", "Use Q Canon To Increase Range In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Jinx.QMiniManaJungleClear", "Minimum Mana To Use Q Canon In JungleClear").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Jinx.UseWJungleClear", "Use W In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Jinx.WMiniManaJungleClear", "Minimum Mana To Use W In JungleClear").SetValue(new Slider(40, 0, 100)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("Jinx.SafeWJungleClear", "Never Use W In JungleClear When Enemy Close To Your Position").SetValue(true));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddSubMenu(new Menu("Skin Changer", "Skin Changer"));
            Config.SubMenu("Misc").SubMenu("Skin Changer").AddItem(new MenuItem("Jinx.SkinChanger", "Use Skin Changer").SetValue(false));
            Config.SubMenu("Misc").SubMenu("Skin Changer").AddItem(new MenuItem("Jinx.SkinChangerName", "Skin choice").SetValue(new StringList(new[] { "Classic", "Mafia", "Firecracker" })));
            Config.SubMenu("Misc").AddItem(new MenuItem("Jinx.EInterrupt", "Interrupt Spells With E").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Jinx.AutoEEGC", "Auto Self E On Gapclosers").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("Jinx.AutoPotion", "Use Auto Potion").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("Jinx.AutoLevelSpell", "Auto Level Spell").SetValue(true));

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q range").SetValue(new Circle(true, Color.Indigo)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("WRange", "W range").SetValue(new Circle(true, Color.Green)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("ERange", "E range").SetValue(new Circle(true, Color.Green)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("RRange", "R range").SetValue(new Circle(true, Color.Gold)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawOrbwalkTarget", "Draw Orbwalk target").SetValue(true));

            Config.AddToMainMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;


        }

        #region ToogleOrder Game_OnUpdate
        public static void Game_OnGameUpdate(EventArgs args)
        {
            //Player.SetSkin(Player.BaseSkinName, Config.Item("Jinx.SkinChanger").GetValue<bool>() ? Config.Item("Jinx.SkinChangerName").GetValue<StringList>().SelectedIndex : Player.SkinId);

            if (Config.Item("Jinx.AutoLevelSpell").GetValue<bool>()) LevelUpSpells();

            if (Player.IsDead) return;

            if (Player.GetBuffCount("Recall") == 1) return;

            ManaManager();
            PotionManager();

            KillSteal();

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                JungleClear();
                LaneClear();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                LastHit();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                LastHit();
            }

            if (Config.Item("Jinx.HarassActive").GetValue<KeyBind>().Active || Config.Item("Jinx.HarassActiveT").GetValue<KeyBind>().Active)
            {
                Harass();
            }

        }
        #endregion

        #region Interupt OnProcessSpellCast
        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {

            double ShouldUseOn = SpellToInterupt(args.SData.Name);
            if (unit.Team != ObjectManager.Player.Team && ShouldUseOn >= 0f && unit.IsValidTarget(E.Range))
            {

                if (Config.Item("Jinx.EInterrupt").GetValue<bool>() && E.IsReady() && Player.Mana > EMANA && Player.Distance(unit) <= E.Range)
                {
                    E.Cast(unit, true, true);
                }

            }

            if (Config.Item("Jinx.AutoEWhenEnemyCastAAM").GetValue<bool>() && (unit.IsValid<AIHeroClient>() && !unit.IsValid<Obj_AI_Turret>()) && unit.IsEnemy && args.Target.IsMe && args.SData.IsAutoAttack() && E.IsReady() && Player.Distance(unit) < 300)
            {
                E.Cast(Player.ServerPosition, true);
            }

            if (Config.Item("Jinx.AutoEWhenEnemyCastAAR").GetValue<bool>() && (unit.IsValid<AIHeroClient>() && !unit.IsValid<Obj_AI_Turret>()) && unit.IsEnemy && args.Target.IsMe && args.SData.IsAutoAttack() && E.IsReady() && Player.Distance(unit) > 300 && Player.Distance(unit) <= E.Range && Player.CountEnemiesInRange(300) == 0)
            {
                E.Cast(unit, true, true);
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Config.Item("Jinx.UseECombo").GetValue<bool>() && (unit.IsValid<AIHeroClient>() && !unit.IsValid<Obj_AI_Turret>()) && unit.IsEnemy && args.Target.IsMe && args.SData.IsAutoAttack() && E.IsReady() && Player.Distance(unit) < 300)
                {
                    E.Cast(Player.ServerPosition, true);
                }

                if (Config.Item("Jinx.UseECombo").GetValue<bool>() && (unit.IsValid<AIHeroClient>() && !unit.IsValid<Obj_AI_Turret>()) && unit.IsEnemy && args.Target.IsMe && args.SData.IsAutoAttack() && E.IsReady() && Player.Distance(unit) > 300 && Player.Distance(unit) <= E.Range && Player.CountEnemiesInRange(300) == 0)
                {
                    E.Cast(unit, true, true);
                }
            }
        }
        #endregion

        #region AntiGapCloser
        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {

            if (Config.Item("Jinx.AutoEEGC").GetValue<bool>() && E.IsReady() && Player.Mana > EMANA && Player.Distance(gapcloser.Sender) <= E.Range)
            {
                E.Cast(Player.ServerPosition, true);
            }

        }
        #endregion

        #region Combo
        public static void Combo()
        {

            var useQ = Program.Config.Item("Jinx.UseQCombo").GetValue<bool>();
            var useW = Program.Config.Item("Jinx.UseWCombo").GetValue<bool>();
            var useR = Program.Config.Item("Jinx.UseRCombo").GetValue<bool>();

            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (target.IsValidTarget())
            {

                #region Sort R combo mode
                if (useR && R.IsReady() && R.GetDamage(target) > target.Health)
                {
                    RUsage();
                }
                #endregion

                #region Sort W combo mode
                if (useW && W.IsReady() && Player.Mana >= WMANA + RMANA)
                {
                    WUsage();
                }
                #endregion

                #region Sort Q combo mode
                if (useQ && Q.IsReady())
                {
                    QNoCanonUP();
                    QCanonUP();
                }
                #endregion
            }

        }
        #endregion

        #region Harass
        public static void Harass()
        {
            var useW = Program.Config.Item("Jinx.UseWHarass").GetValue<bool>();

            var WMinMana = Config.Item("Jinx.WMiniManaHarass").GetValue<Slider>().Value;

            var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Physical);
            

            if (target.IsValidTarget() && useW && W.IsReady() && Player.ManaPercent >= WMinMana)
            {
                WUsage();
            }


        }
        #endregion

        #region LastHit
        public static void LastHit()
        {

            var useW = Program.Config.Item("Jinx.UseWLastHit").GetValue<bool>();

            var WMinMana = Config.Item("Jinx.WMiniManaLastHit").GetValue<Slider>().Value;

            var MinionW = MinionManager.GetMinions(1300, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).FirstOrDefault();

            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);


            if (useW && MinionW.IsValidTarget() && Player.ManaPercent >= WMinMana && MinionW.Health < W.GetDamage(MinionW))
            {
                if (Config.Item("Jinx.SafeWLastHit").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0)
                {
                    return;
                }
                else
                    W.CastIfHitchanceEquals(MinionW, HitChance.High, true);
            }

        }
        #endregion

        #region LaneClear
        public static void LaneClear()
        {

            var useW = Config.Item("Jinx.UseWLaneClear").GetValue<bool>();

            var WMinMana = Config.Item("Jinx.WMiniManaLaneClear").GetValue<Slider>().Value;

            var MinionW = MinionManager.GetMinions(1300, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).FirstOrDefault();


            if (useW && MinionW.IsValidTarget() && Player.ManaPercent >= WMinMana && MinionW.Health < W.GetDamage(MinionW))
            {
                if (Config.Item("Jinx.SafeWLaneClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0)
                {
                    return;
                }
                else
                    W.CastIfHitchanceEquals(MinionW, HitChance.High, true);
            }


        }
        #endregion

        #region JungleClear
        public static void JungleClear()
        {

            var useQ = Program.Config.Item("Jinx.UseQJungleClear").GetValue<bool>();
            var useW = Program.Config.Item("Jinx.UseWJungleClear").GetValue<bool>();

            var QMinMana = Config.Item("Jinx.QMiniManaJungleClear").GetValue<Slider>().Value;
            var WMinMana = Config.Item("Jinx.WMiniManaJungleClear").GetValue<Slider>().Value;

            var MinionQ = MinionManager.GetMinions(900, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();


            if (useQ && MinionQ.IsValidTarget() && MinionQ.Distance(Player) > 660 && Player.ManaPercent >= QMinMana && !CanonUp)
            {
                Q.Cast();
            }
            else if (useQ && MinionQ.IsValidTarget() && MinionQ.Distance(Player) < 660 && CanonUp)
            {
                Q.Cast();
            }

            if (useW && W.IsReady() && Player.Distance(MinionQ) <= 900 && Player.ManaPercent >= WMinMana)
            {
                if (Config.Item("Jinx.SafeWJungleClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0)
                {
                    return;
                }
                else
                    W.CastIfHitchanceEquals(MinionQ, HitChance.High, true);
            }

        }
        #endregion

        #region KillSteal
        public static void KillSteal()
        {

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(target => !target.IsMe && target.Team != ObjectManager.Player.Team))
            {
                if (target.IsValidTarget(W.Range) && W.IsReady() && W.GetDamage(target) > target.Health && Player.Mana > WMANA && Player.Distance(target) > 660 && Player.Distance(target) <= 1200)
                {
                    W.CastIfHitchanceEquals(target, HitChance.High, true);
                }

                if (target.IsValidTarget(R.Range) && R.IsReady() && R.GetDamage(target) > target.Health && Player.Mana > RMANA && Player.Distance(target) <= 2200)
                {
                    RUsage();
                }

                if (target.IsValidTarget(W.Range) && W.IsReady() && R.IsReady() && W.GetDamage(target) + R.GetDamage(target) > target.Health && Player.Mana > WMANA + RMANA && Player.Distance(target) > 660 && Player.Distance(target) <= 1200)
                {
                    W.CastIfHitchanceEquals(target, HitChance.High, true);
                }
            }
        }
        #endregion

        #region Interupt Spell List
        public static double SpellToInterupt(string SpellName)
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
            if (SpellName == "AbsoluteZero")
                return 0;
            if (SpellName == "Pantheon_GrandSkyfall_Jump")
                return 0;
            if (SpellName == "SkarnerImpale")
                return 0;
            if (SpellName == "YasuoRKnockUpComboW")
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

            if (Config.Item("Jinx.AutoPotion").GetValue<bool>() && !Player.InFountain() && !Player.IsRecalling() && !Player.IsDead)
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

        #region Check If CanonUp
        private static bool CanonUp
        {
            get { return Math.Abs(ObjectManager.Player.AttackRange - 525f) > float.Epsilon; }
        }
        #endregion

        public static void QNoCanonUP()
        {
            var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Physical);

            #region No Canon
            if (target.IsValidTarget() && !CanonUp)
            {
                if (Player.CountEnemiesInRange(1200) > 1)
                {
                    if (Player.CountAlliesInRange(1200) >= 1 + 1)
                    {
                        if (Player.HealthPercent <= target.HealthPercent)
                        {

                            if (Player.Distance(target) > 660 && Player.Distance(target) <= 790 - 65)
                            {
                                Q.Cast();
                                return;
                            }

                            if (Player.Distance(target) > 790 - 65 && Player.Distance(target) <= 900)
                            {
                                Q.Cast();
                                return;
                            }
                            return;
                        }

                        if (Player.HealthPercent > target.HealthPercent)
                        {
                            if (target.CountEnemiesInRange(50) >= 1)
                            {
                                if (Player.MoveSpeed <= target.MoveSpeed)
                                {

                                    if (Player.Distance(target) > 660 && Player.Distance(target) <= 790 - 65)
                                    {
                                        Q.Cast();
                                        return;
                                    }

                                    if (Player.Distance(target) > 790 - 65 && Player.Distance(target) <= 900)
                                    {
                                        Q.Cast();
                                        return;
                                    }
                                    return;
                                }

                                if (Player.MoveSpeed > target.MoveSpeed)
                                {
                                    if (Player.Distance(target) <= 660)
                                    {
                                        Q.Cast();
                                        return;
                                    }

                                    if (Player.Distance(target) > 660 && Player.Distance(target) <= 790 - 65)
                                    {
                                        Q.Cast();
                                        return;
                                    }

                                    if (Player.Distance(target) > 790 - 65 && Player.Distance(target) <= 900)
                                    {
                                        Q.Cast();
                                        return;
                                    }
                                    return;
                                }
                                return;
                            }

                            if (target.CountEnemiesInRange(50) == 0)
                            {
                                if (Player.MoveSpeed <= target.MoveSpeed)
                                {

                                    if (Player.Distance(target) > 660 && Player.Distance(target) <= 790 - 65)
                                    {
                                        Q.Cast();
                                        return;
                                    }

                                    if (Player.Distance(target) > 790 - 65 && Player.Distance(target) <= 900)
                                    {
                                        Q.Cast();
                                        return;
                                    }
                                    return;
                                }

                                if (Player.MoveSpeed > target.MoveSpeed)
                                {

                                    if (Player.Distance(target) > 790 - 65 && Player.Distance(target) <= 900)
                                    {
                                        Q.Cast();
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

                    if (Player.CountAlliesInRange(1200) == 0 + 1)
                    {
                        if (Player.HealthPercent <= target.HealthPercent)
                        {

                            if (Player.Distance(target) > 660 && Player.Distance(target) <= 790 - 65)
                            {
                                Q.Cast();
                                return;
                            }

                            if (Player.Distance(target) > 790 - 65 && Player.Distance(target) <= 900)
                            {
                                Q.Cast();
                                return;
                            }
                            return;
                        }

                        if (Player.HealthPercent > target.HealthPercent)
                        {
                            if (target.CountEnemiesInRange(50) >= 1)
                            {
                                if (Player.MoveSpeed <= target.MoveSpeed)
                                {

                                    if (Player.Distance(target) > 660 && Player.Distance(target) <= 790 - 65)
                                    {
                                        Q.Cast();
                                        return;
                                    }

                                    if (Player.Distance(target) > 790 - 65 && Player.Distance(target) <= 900)
                                    {
                                        Q.Cast();
                                        return;
                                    }
                                    return;
                                }

                                if (Player.MoveSpeed > target.MoveSpeed)
                                {
                                    if (Player.Distance(target) <= 660)
                                    {
                                        Q.Cast();
                                        return;
                                    }

                                    if (Player.Distance(target) > 660 && Player.Distance(target) <= 790 - 65)
                                    {
                                        Q.Cast();
                                        return;
                                    }

                                    if (Player.Distance(target) > 790 - 65 && Player.Distance(target) <= 900)
                                    {
                                        Q.Cast();
                                        return;
                                    }
                                    return;
                                }
                                return;
                            }

                            if (target.CountEnemiesInRange(50) == 0)
                            {
                                if (Player.MoveSpeed <= target.MoveSpeed)
                                {

                                    if (Player.Distance(target) > 660 && Player.Distance(target) <= 790 - 65)
                                    {
                                        Q.Cast();
                                        return;
                                    }

                                    if (Player.Distance(target) > 790 - 65 && Player.Distance(target) <= 900)
                                    {
                                        Q.Cast();
                                        return;
                                    }
                                    return;
                                }

                                if (Player.MoveSpeed > target.MoveSpeed)
                                {

                                    if (Player.Distance(target) > 790 - 65 && Player.Distance(target) <= 900)
                                    {
                                        Q.Cast();
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
                if (Player.CountEnemiesInRange(1200) == 1)
                {
                    if (Player.CountAlliesInRange(1200) >= 1 + 1)
                    {
                        if (Player.HealthPercent <= target.HealthPercent)
                        {
                            if (Player.Distance(target) > 660 && Player.Distance(target) <= 790 - 65)
                            {
                                Q.Cast();
                                return;
                            }

                            if (Player.Distance(target) > 790 - 65 && Player.Distance(target) <= 900)
                            {
                                Q.Cast();
                                return;
                            }
                            return;
                        }

                        if (Player.HealthPercent > target.HealthPercent)
                        {
                            if (Player.MoveSpeed <= target.MoveSpeed)
                            {

                                if (Player.Distance(target) > 660 && Player.Distance(target) <= 790 - 65)
                                {
                                    Q.Cast();
                                    return;
                                }

                                if (Player.Distance(target) > 790 - 65 && Player.Distance(target) <= 900)
                                {
                                    Q.Cast();
                                    return;
                                }
                                return;
                            }

                            if (Player.MoveSpeed > target.MoveSpeed)
                            {

                                if (Player.Distance(target) > 790 - 65 && Player.Distance(target) <= 900)
                                {
                                    Q.Cast();
                                    return;
                                }
                                return;
                            }
                            return;
                        }
                        return;
                    }
                    if (Player.CountAlliesInRange(1200) == 0 + 1)
                    {
                        if (Player.HealthPercent <= target.HealthPercent)
                        {

                            if (Player.Distance(target) > 660 && Player.Distance(target) <= 790 - 65)
                            {
                                Q.Cast();
                                return;
                            }

                            if (Player.Distance(target) > 790 - 65 && Player.Distance(target) <= 900)
                            {
                                Q.Cast();
                                return;
                            }
                            return;
                        }

                        if (Player.HealthPercent > target.HealthPercent)
                        {
                            if (Player.MoveSpeed <= target.MoveSpeed)
                            {
                                if (Player.Distance(target) > 660 && Player.Distance(target) <= 790 - 65)
                                {
                                    Q.Cast();
                                    return;
                                }

                                if (Player.Distance(target) > 790 - 65 && Player.Distance(target) <= 900)
                                {
                                    Q.Cast();
                                    return;
                                }
                                return;
                            }

                            if (Player.MoveSpeed > target.MoveSpeed)
                            {

                                if (Player.Distance(target) > 790 - 65 && Player.Distance(target) <= 900)
                                {
                                    Q.Cast();
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
        }

        public static void QCanonUP()
        {
            var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Physical);

            #region Canon
            if (target.IsValidTarget() && CanonUp)
            {
                if (Player.CountEnemiesInRange(1200) > 1)
                {
                    if (Player.CountAlliesInRange(1200) >= 1 + 1)
                    {
                        if (Player.HealthPercent <= target.HealthPercent)
                        {
                            if (Player.Distance(target) <= 660)
                            {
                                Q.Cast();
                                return;
                            }
                            return;
                        }

                        if (Player.HealthPercent > target.HealthPercent)
                        {
                            if (target.CountEnemiesInRange(50) >= 1)
                            {
                                if (Player.MoveSpeed <= target.MoveSpeed)
                                {
                                    if (Player.Distance(target) <= 660)
                                    {
                                        Q.Cast();
                                        return;
                                    }
                                    return;
                                }
                                return;
                            }

                            if (target.CountEnemiesInRange(50) == 0)
                            {
                                if (Player.MoveSpeed <= target.MoveSpeed)
                                {
                                    if (Player.Distance(target) <= 660)
                                    {
                                        Q.Cast();
                                        return;
                                    }
                                    return;
                                }

                                if (Player.MoveSpeed > target.MoveSpeed)
                                {
                                    if (Player.Distance(target) <= 660)
                                    {
                                        Q.Cast();
                                        return;
                                    }

                                    if (Player.Distance(target) > 660 && Player.Distance(target) <= 790 - 65)
                                    {
                                        Q.Cast();
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
                    if (Player.CountAlliesInRange(1200) == 0 + 1)
                    {
                        if (Player.HealthPercent <= target.HealthPercent)
                        {
                            if (Player.Distance(target) <= 660)
                            {
                                Q.Cast();
                                return;
                            }
                            return;
                        }

                        if (Player.HealthPercent > target.HealthPercent)
                        {
                            if (target.CountEnemiesInRange(50) >= 1)
                            {
                                if (Player.MoveSpeed <= target.MoveSpeed)
                                {
                                    if (Player.Distance(target) <= 660)
                                    {
                                        Q.Cast();
                                        return;
                                    }
                                    return;
                                }
                                return;
                            }

                            if (target.CountEnemiesInRange(50) == 0)
                            {
                                if (Player.MoveSpeed <= target.MoveSpeed)
                                {
                                    if (Player.Distance(target) <= 660)
                                    {
                                        Q.Cast();
                                        return;
                                    }
                                    return;
                                }

                                if (Player.MoveSpeed > target.MoveSpeed)
                                {
                                    if (Player.Distance(target) <= 660)
                                    {
                                        Q.Cast();
                                        return;
                                    }

                                    if (Player.Distance(target) > 660 && Player.Distance(target) <= 790 - 65)
                                    {
                                        Q.Cast();
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
                if (Player.CountEnemiesInRange(1200) == 1)
                {
                    if (Player.CountAlliesInRange(1200) >= 1 + 1)
                    {
                        if (Player.HealthPercent <= target.HealthPercent)
                        {
                            if (Player.Distance(target) <= 660)
                            {
                                Q.Cast();
                                return;
                            }
                            return;
                        }

                        if (Player.HealthPercent > target.HealthPercent)
                        {
                            if (Player.MoveSpeed <= target.MoveSpeed)
                            {
                                if (Player.Distance(target) <= 660)
                                {
                                    Q.Cast();
                                    return;
                                }
                                return;
                            }

                            if (Player.MoveSpeed > target.MoveSpeed)
                            {
                                if (Player.Distance(target) <= 660)
                                {
                                    Q.Cast();
                                    return;
                                }

                                if (Player.Distance(target) > 660 && Player.Distance(target) <= 790 - 65)
                                {
                                    Q.Cast();
                                    return;
                                }
                                return;
                            }
                            return;
                        }
                        return;
                    }
                    if (Player.CountAlliesInRange(1200) == 0 + 1)
                    {
                        if (Player.HealthPercent <= target.HealthPercent)
                        {
                            if (Player.Distance(target) <= 660)
                            {
                                Q.Cast();
                                return;
                            }
                            return;
                        }

                        if (Player.HealthPercent > target.HealthPercent)
                        {
                            if (Player.MoveSpeed <= target.MoveSpeed)
                            {
                                if (Player.Distance(target) <= 660)
                                {
                                    Q.Cast();
                                    return;
                                }
                                return;
                            }

                            if (Player.MoveSpeed > target.MoveSpeed)
                            {
                                if (Player.Distance(target) <= 660)
                                {
                                    Q.Cast();
                                    return;
                                }

                                if (Player.Distance(target) > 660 && Player.Distance(target) <= 790 - 65)
                                {
                                    Q.Cast();
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
        }

        #region W Usage
        public static void WUsage()
        {
            var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Physical);

            if (Player.CountEnemiesInRange(1300) > 1)
            {
                if (Player.CountAlliesInRange(1300) >= 1 + 1)
                {
                    if (Player.HealthPercent <= target.HealthPercent)
                    {
                        if (Player.Distance(target) <= 660)
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 660 && Player.Distance(target) <= 780)
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 780 && Player.Distance(target) <= 900)
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        return;
                    }

                    if (Player.HealthPercent > target.HealthPercent)
                    {
                        if (Player.MoveSpeed <= target.MoveSpeed)
                        {
                            if (Player.Distance(target) <= 660)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }

                            if (Player.Distance(target) > 660 && Player.Distance(target) <= 780)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }

                            if (Player.Distance(target) > 780 && Player.Distance(target) <= 900)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            return;
                        }

                        if (Player.MoveSpeed > target.MoveSpeed)
                        {
                            if (Player.Distance(target) > 660 && Player.Distance(target) <= 780)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }

                            if (Player.Distance(target) > 780 && Player.Distance(target) <= 900)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            return;
                        }
                        return;
                    }
                    return;
                }
                if (Player.CountAlliesInRange(1300) == 0 + 1)
                {
                    if (Player.HealthPercent <= target.HealthPercent)
                    {
                        if (Player.Distance(target) <= 660)
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 660 && Player.Distance(target) <= 780)
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 780 && Player.Distance(target) <= 900)
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        return;
                    }

                    if (Player.HealthPercent > target.HealthPercent)
                    {
                        if (Player.MoveSpeed <= target.MoveSpeed)
                        {
                            if (Player.Distance(target) <= 660)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }

                            if (Player.Distance(target) > 660 && Player.Distance(target) <= 780)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }

                            if (Player.Distance(target) > 780 && Player.Distance(target) <= 900)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            return;
                        }

                        if (Player.MoveSpeed > target.MoveSpeed)
                        {
                            if (Player.Distance(target) > 660 && Player.Distance(target) <= 780)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }

                            if (Player.Distance(target) > 780 && Player.Distance(target) <= 900)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
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
            if (Player.CountEnemiesInRange(1300) == 1)
            {
                if (Player.CountAlliesInRange(1300) >= 1 + 1)
                {
                    if (Player.HealthPercent <= target.HealthPercent)
                    {
                        if (Player.Distance(target) <= 660)
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 660 && Player.Distance(target) <= 780)
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 780 && Player.Distance(target) <= 900)
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        return;
                    }

                    if (Player.HealthPercent > target.HealthPercent)
                    {
                        if (Player.MoveSpeed <= target.MoveSpeed)
                        {
                            if (Player.Distance(target) <= 660)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }

                            if (Player.Distance(target) > 660 && Player.Distance(target) <= 780)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }

                            if (Player.Distance(target) > 780 && Player.Distance(target) <= 900)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            return;
                        }

                        if (Player.MoveSpeed > target.MoveSpeed)
                        {

                            if (Player.Distance(target) > 660 && Player.Distance(target) <= 780)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }

                            if (Player.Distance(target) > 780 && Player.Distance(target) <= 900)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            return;
                        }
                        return;
                    }
                    return;
                }
                if (Player.CountAlliesInRange(1300) == 0 + 1)
                {
                    if (Player.HealthPercent <= target.HealthPercent)
                    {
                        if (Player.Distance(target) <= 660)
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 660 && Player.Distance(target) <= 780)
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 780 && Player.Distance(target) <= 900)
                        {
                            W.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        return;
                    }

                    if (Player.HealthPercent > target.HealthPercent)
                    {
                        if (Player.MoveSpeed <= target.MoveSpeed)
                        {
                            if (Player.Distance(target) <= 660)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }

                            if (Player.Distance(target) > 660 && Player.Distance(target) <= 780)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }

                            if (Player.Distance(target) > 780 && Player.Distance(target) <= 900)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            return;
                        }

                        if (Player.MoveSpeed > target.MoveSpeed)
                        {

                            if (Player.Distance(target) > 660 && Player.Distance(target) <= 780)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }

                            if (Player.Distance(target) > 780 && Player.Distance(target) <= 900)
                            {
                                W.CastIfHitchanceEquals(target, HitChance.High, true);
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

        #region R Usage
        public static void RUsage()
        {
            var target = TargetSelector.GetTarget(2200, TargetSelector.DamageType.Physical);

            if (Player.CountEnemiesInRange(2200) > 1)
            {
                if (Player.CountAlliesInRange(2200) >= 1 + 1)
                {
                    if (Player.HealthPercent <= target.HealthPercent)
                    {
                        if (Player.Distance(target) <= 660)
                        {
                            R.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 660 && Player.Distance(target) <= 1430)
                        {
                            R.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 1430 && Player.Distance(target) <= 2200)
                        {
                            R.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        return;
                    }

                    if (Player.HealthPercent > target.HealthPercent)
                    {
                        if (Player.MoveSpeed <= target.MoveSpeed)
                        {

                            if (Player.Distance(target) > 660 && Player.Distance(target) <= 1430)
                            {
                                R.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }

                            if (Player.Distance(target) > 1430 && Player.Distance(target) <= 2200)
                            {
                                R.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            return;
                        }

                        if (Player.MoveSpeed > target.MoveSpeed)
                        {

                            if (Player.Distance(target) > 1430 && Player.Distance(target) <= 2200)
                            {
                                R.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            return;
                        }
                        return;
                    }
                    return;
                }
                if (Player.CountAlliesInRange(2200) == 0 + 1)
                {
                    if (Player.HealthPercent <= target.HealthPercent)
                    {
                        if (Player.Distance(target) <= 660)
                        {
                            R.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 660 && Player.Distance(target) <= 1430)
                        {
                            R.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 1430 && Player.Distance(target) <= 2200)
                        {
                            R.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        return;
                    }

                    if (Player.HealthPercent > target.HealthPercent)
                    {

                        if (Player.Distance(target) > 660 && Player.Distance(target) <= 1430)
                        {
                            R.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 1430 && Player.Distance(target) <= 2200)
                        {
                            R.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        return;
                    }
                    return;
                }
                return;
            }
            if (Player.CountEnemiesInRange(2200) == 1)
            {
                if (Player.CountAlliesInRange(2200) >= 1 + 1)
                {
                    if (Player.HealthPercent <= target.HealthPercent)
                    {
                        if (Player.Distance(target) <= 660)
                        {
                            R.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 660 && Player.Distance(target) <= 1430)
                        {
                            R.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 1430 && Player.Distance(target) <= 2200)
                        {
                            R.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        return;
                    }

                    if (Player.HealthPercent > target.HealthPercent)
                    {
                        if (Player.MoveSpeed <= target.MoveSpeed)
                        {

                            if (Player.Distance(target) > 660 && Player.Distance(target) <= 1430)
                            {
                                R.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }

                            if (Player.Distance(target) > 1430 && Player.Distance(target) <= 2200)
                            {
                                R.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            return;
                        }

                        if (Player.MoveSpeed > target.MoveSpeed)
                        {

                            if (Player.Distance(target) > 1430 && Player.Distance(target) <= 2200)
                            {
                                R.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            return;
                        }
                        return;
                    }
                    return;
                }
                if (Player.CountAlliesInRange(2200) == 0 + 1)
                {
                    if (Player.HealthPercent <= target.HealthPercent)
                    {
                        if (Player.Distance(target) <= 660)
                        {
                            R.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 660 && Player.Distance(target) <= 1430)
                        {
                            R.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }

                        if (Player.Distance(target) > 1430 && Player.Distance(target) <= 2200)
                        {
                            R.CastIfHitchanceEquals(target, HitChance.High, true);
                            return;
                        }
                        return;
                    }

                    if (Player.HealthPercent > target.HealthPercent)
                    {
                        if (Player.MoveSpeed <= target.MoveSpeed)
                        {

                            if (Player.Distance(target) > 660 && Player.Distance(target) <= 1430)
                            {
                                R.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }

                            if (Player.Distance(target) > 1430 && Player.Distance(target) <= 2200)
                            {
                                R.CastIfHitchanceEquals(target, HitChance.High, true);
                                return;
                            }
                            return;
                        }

                        if (Player.MoveSpeed > target.MoveSpeed)
                        {

                            if (Player.Distance(target) > 1430 && Player.Distance(target) <= 2200)
                            {
                                R.CastIfHitchanceEquals(target, HitChance.High, true);
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

        #region BeforeAA
        static void Orbwalking_BeforeAttack(LeagueSharp.Common.Orbwalking.BeforeAttackEventArgs args)
        {

            if ((Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed) && CanonUp && Q.IsReady())
            {
                Q.Cast();
            }

        }
        #endregion
    }

}
