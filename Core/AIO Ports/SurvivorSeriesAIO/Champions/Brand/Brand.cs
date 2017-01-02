/**
 * 
 * 
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;
using SharpDX;
using SurvivorSeriesAIO.Core;
using SurvivorSeriesAIO.SurvivorMain;
using SurvivorSeriesAIO.Utility;
using Color = System.Drawing.Color;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Champions
{
    internal class Brand : ChampionBase
    {
        private float RManaC;
        public List<Spell> SpellList = new List<Spell>();
        //private AIHeroClient Player { get { return ObjectManager.Player; } }

        public Brand(IRootMenu menu, Orbwalking.Orbwalker Orbwalker)
            : base(menu, Orbwalker)
        {
            // manual override - default is spell values from LeagueSharp.Data
            Q.SetSkillshot(0.25f, 60f, 1600f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(1.15f, 230f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetTargetted(0.25f, 2000f);
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

            Config = new Configuration(menu.Champion);
            InitHpbarOverlay();
        }

        public Configuration Config { get; }

        private void InitHpbarOverlay()
        {
            //DrawDamage.DamageToUnit = CalculateDamage;
            //DrawDamage.Enabled = () => Config.DrawComboDamage.GetValue<bool>();
            //DrawDamage.Fill = () => Config.FillColor.GetValue<Circle>().Active;
            //DrawDamage.FillColor = () => Config.FillColor.GetValue<Circle>().Color;
        }

        private void OnDraw(EventArgs args)
        {
            foreach (var enemy in HeroManager.Enemies.Where(x => x.HasBuff("brandablazebomb") && x.IsValidTarget()))
                if (Config.DrawPassiveBombOnEnemy.GetValue<bool>())
                    Render.Circle.DrawCircle(enemy.Position, 420f, Color.Orange);
            if (Config.drawQ.GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Aqua);
            if (Config.drawW.GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.MediumPurple);
            if (Config.drawE.GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.LightPink);
            if (Config.drawR.GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.MediumVioletRed);
        }

        /// <summary>
        ///     BlockAA in Combo || Dont BlockAA in Combo
        /// </summary>
        private void AABlock()
        {
            Orbwalker.SetAttack(!Config.HarassBlockAA.GetValue<bool>());
            //SebbyLib.OktwCommon.blockAttack = Config.Item("CBlockAA").GetValue<bool>();
        }

        private void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
                return;
            /*if (LeagueSharp.Common.Config.Item("UseSkin").GetValue<bool>())
            {
                //Player.SetSkin(Player.CharData.BaseSkinName, LeagueSharp.Common.Config.Item("SkinID").GetValue<Slider>().Value);
            }*/
            // Checks
            RManaCost();
            //new Utility.Activator().SeraphUsage();
            //new Utility.Activator().ProHexGLPUsage();
            // Combo
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                Combo();
            //Lane
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear) // LaneClear mode broken? kappa
                LaneClear();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                AABlock();
                Harass();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
            {
                Orbwalker.SetMovement(true);
                Orbwalker.SetAttack(true);
            }
            if (Config.StunTargetKey.GetValue<KeyBind>().Active)
            {
                Orbwalking.MoveTo(Game.CursorPos);
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
                {
                    var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                    if (!t.IsValidTarget() || (t == null))
                        return;
                    // Wait till the enemy has BrandABlaze from the W or E casting and then cast Q.
                    if (t.HasBuff("brandablaze"))
                    {
                        if (Q.IsReady())
                            SpellCast.SebbySpellMain(Q, t);
                    }
                    else
                    {
                        if (Q.IsReady() && E.IsReady())
                            E.CastOnUnit(t);
                        else if (Q.IsReady() && W.IsReady())
                            SpellCast.SebbySpellMain(W, t);
                    }
                }
            }
        }

        /// <summary>
        ///     AntiGapCloser Event & Action
        /// </summary>
        /// <param name="gapcloser"></param>
        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Config.QGapC.GetValue<bool>() || (Player.Mana < Q.Instance.SData.Mana + E.Instance.SData.Mana))
                return;

            var t = gapcloser.Sender;

            if (t.IsValidTarget(E.Range) && (t.HasBuff("brandablaze") || E.IsReady()))
            {
                E.CastOnUnit(t);
                if (Q.IsReady())
                    Q.Cast(t);
            }
        }

        /// <summary>
        ///     Interrupter Event & Action
        /// </summary>
        /// <param name="t"></param>
        /// <param name="args"></param>
        private void Interrupter2_OnInterruptableTarget(AIHeroClient t, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Config.InterruptEQ.GetValue<bool>() || (Player.Mana < Q.Instance.SData.Mana + E.Instance.SData.Mana))
                return;

            if (t.IsValidTarget(E.Range) && (t.HasBuff("brandablaze") || E.IsReady()))
            {
                E.CastOnUnit(t);
                if (Q.IsReady())
                    Q.Cast(t);
            }
        }

        /// <summary>
        ///     Take in account the bonus damage of the player
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private float BonusDmg(AIHeroClient target)
        {
            return
                (float)
                Player.CalcDamage(target, Damage.DamageType.Magical, target.MaxHealth*0.08 - target.HPRegenRate*5);
        }

        private bool LogQUse(Obj_AI_Base m)
        {
            if (m.HasBuff("brandablaze"))
                return true;
            if ((E.Instance.CooldownExpires - Game.Time + 2 >= Q.Instance.Cooldown) &&
                (W.Instance.CooldownExpires - Game.Time + 2 >= Q.Instance.Cooldown))
                return true;
            return false;
        }

        private void QUsage()
        {
            var m = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (!m.IsValidTarget())
                return;
            // Q Improvement + KS Below
            if (OktwCommon.GetKsDamage(m, Q) + BonusDmg(m) + OktwCommon.GetEchoLudenDamage(m) > m.Health)
                SpellCast.SebbySpellMain(Q, m);
            if (Config.QAblazedEnemy.GetValue<bool>() && m.HasBuff("brandablaze"))
            {
                foreach (
                    var enemy in
                    HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && enemy.HasBuff("brandablaze")))
                    SpellCast.SebbySpellMain(Q, enemy);
            }
            else
            {
                if (m.HasBuff("brandablaze") && Config.QAblazedEnemy.GetValue<bool>())
                {
                    var spreadTarget = m;

                    foreach (
                        var enemy in
                        HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && enemy.HasBuff("brandablaze"))
                    )
                        m = enemy;

                    if ((spreadTarget == m) && !LogQUse(m))
                        return;
                }

                if ((Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) && !W.IsReady() &&
                    (Player.ManaPercent > RManaC + Q.ManaCost))
                {
                    var enemystunned =
                        HeroManager.Enemies.Find(
                            enemy =>
                                (enemy.IsValidTarget(Q.Range) && enemy.HasBuff("stun")) ||
                                (enemy.IsStunned && Config.PrioritizeStunned.GetValue<bool>()));
                    if (enemystunned != null)
                    {
                        Orbwalker.ForceTarget(enemystunned);
                        SpellCast.SebbySpellMain(Q, enemystunned);
                    }
                    else
                    {
                        Orbwalker.ForceTarget(m);
                        SpellCast.SebbySpellMain(Q, m);
                    }
                }
                if (Player.Mana > RManaC + Q.ManaCost)
                {
                    var enemystunned =
                        HeroManager.Enemies.Find(
                            enemy =>
                                (enemy.IsValidTarget(Q.Range) && enemy.HasBuff("stun")) ||
                                (enemy.IsStunned && Config.PrioritizeStunned.GetValue<bool>()));
                    if (enemystunned != null)
                    {
                        Orbwalker.ForceTarget(enemystunned);
                        SpellCast.SebbySpellMain(Q, enemystunned);
                    }
                    else
                    {
                        Orbwalker.ForceTarget(m);
                        SpellCast.SebbySpellMain(Q, m);
                    }
                }
            }
        }

        private void RManaCost()
        {
            if (!R.IsReady())
                RManaC = W.Instance.SData.Mana;
            else
                RManaC = R.Instance.SData.Mana;
        }

        private void WUsage()
        {
            // W Usage
            var t = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
            {
                var Qdamage = Q.GetDamage(t);
                var Wdamage = OktwCommon.GetKsDamage(t, W) + BonusDmg(t) + OktwCommon.GetEchoLudenDamage(t);
                if (Wdamage > t.Health)
                    SpellCast.SebbySpellMain(W, t);
                else if ((Wdamage + Qdamage > t.Health) && (Player.ManaPercent > Q.ManaCost + E.ManaCost))
                    SpellCast.SebbySpellMain(W, t);

                if (Player.Mana > RManaC + W.ManaCost)
                {
                    var enemystunned =
                        HeroManager.Enemies.Find(
                            enemy =>
                                (enemy.IsValidTarget(W.Range) && enemy.HasBuff("stun")) ||
                                (enemy.IsStunned && Config.PrioritizeStunned.GetValue<bool>()));
                    if (enemystunned != null)
                    {
                        Orbwalker.ForceTarget(enemystunned);
                        SpellCast.SebbySpellMain(W, enemystunned);
                    }
                    else
                    {
                        Orbwalker.ForceTarget(enemystunned);
                        SpellCast.SebbySpellMain(W, t);
                    }
                }
            }
        }

        private void EUsage()
        {
            // E Usage
            var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget())
                if ((Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) && (Player.Mana > RManaC + E.ManaCost))
                {
                    var enemystunned =
                        HeroManager.Enemies.Find(
                            enemy =>
                                (enemy.IsValidTarget(E.Range) && enemy.HasBuff("stun")) ||
                                (enemy.IsStunned && Config.PrioritizeStunned.GetValue<bool>()));
                    if (enemystunned != null)
                    {
                        Orbwalker.ForceTarget(enemystunned);
                        E.CastOnUnit(enemystunned);
                    }
                    else
                    {
                        Orbwalker.ForceTarget(t);
                        E.CastOnUnit(t);
                    }
                }
                else
                {
                    // If there's a chance to KS/Get :kappa: a kill.
                    var eDmg = OktwCommon.GetKsDamage(t, E) + BonusDmg(t) + OktwCommon.GetEchoLudenDamage(t);
                    var wDmg = W.GetDamage(t);
                    if (eDmg > t.Health)
                    {
                        E.CastOnUnit(t);
                    }
                    else if ((wDmg + eDmg > t.Health) && (Player.Mana > W.ManaCost + E.ManaCost))
                    {
                        E.CastOnUnit(t);
                        SpellCast.SebbySpellMain(W, t);
                    }
                }
        }

        private void RUsage()
        {
            // Massive Thanks to Sebby for the time saved instead of writing it from scratch, instead I 'stole' it from him :feelsbadman:
            var bounceRange = 430;
            var t2 = TargetSelector.GetTarget(R.Range + bounceRange, TargetSelector.DamageType.Magical);

            if (t2.IsValidTarget(R.Range) &&
                (t2.CountEnemiesInRange(bounceRange) >= Config.NearbyREnemies.GetValue<Slider>().Value) &&
                (Config.NearbyREnemies.GetValue<Slider>().Value > 0))
                R.Cast(t2);

            if (t2.IsValidTarget() && OktwCommon.ValidUlt(t2))
                if ((t2.CountAlliesInRange(550) == 0) || (Player.HealthPercent < 50) ||
                    (t2.CountEnemiesInRange(bounceRange) > 1))
                {
                    var prepos = R.GetPrediction(t2).CastPosition;
                    var dmgR = R.GetDamage(t2);

                    if (t2.Health < dmgR*3)
                    {
                        var totalDmg = dmgR;
                        var minionCount = CountMinionsInRange(bounceRange, prepos);

                        if (t2.IsValidTarget(R.Range))
                        {
                            if (prepos.CountEnemiesInRange(bounceRange) > 1)
                                if (minionCount > 2)
                                    totalDmg = dmgR*2;
                                else
                                    totalDmg = dmgR*3;
                            else if (minionCount > 0)
                                totalDmg = dmgR*2;

                            if (W.IsReady())
                                totalDmg += W.GetDamage(t2);

                            if (E.IsReady())
                                totalDmg += E.GetDamage(t2);

                            if (Q.IsReady())
                                totalDmg += Q.GetDamage(t2);

                            totalDmg += BonusDmg(t2);
                            totalDmg += OktwCommon.GetEchoLudenDamage(t2);

                            // Hex
                            if (Items.HasItem(3155, t2))
                                totalDmg = totalDmg - 250;

                            // MoM
                            if (Items.HasItem(3156, t2))
                                totalDmg = totalDmg - 300;

                            if ((totalDmg > t2.Health - OktwCommon.GetIncomingDamage(t2)) &&
                                (Player.GetAutoAttackDamage(t2)*2 < t2.Health))
                                R.CastOnUnit(t2);
                        }
                        else if (t2.Health - OktwCommon.GetIncomingDamage(t2) < dmgR*2 + BonusDmg(t2))
                        {
                            if (Player.CountEnemiesInRange(R.Range) > 0)
                            {
                                foreach (
                                    var t in
                                    HeroManager.Enemies.Where(
                                        enemy => enemy.IsValidTarget(R.Range) && (enemy.Distance(prepos) < bounceRange))
                                )
                                    R.CastOnUnit(t);
                            }
                            else
                            {
                                var minions = Cache.GetMinions(Player.Position, R.Range);
                                foreach (
                                    var minion in
                                    minions.Where(
                                        minion =>
                                                minion.IsValidTarget(R.Range) && (minion.Distance(prepos) < bounceRange))
                                )
                                    R.CastOnUnit(minion);
                            }
                        }
                    }
                }
        }

        /// <summary>
        ///     Get W Damage
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private double Wdmg(Obj_AI_Base target)
        {
            return target.MaxHealth*(4.5 + W.Level*1.5)*0.01;
        }

        /// <summary>
        ///     Might as well use on if minion.CountEnemiesInRange
        /// </summary>
        /// <param name="range"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private int CountMinionsInRange(float range, Vector3 pos)
        {
            var minions = Cache.GetMinions(pos, range);
            var count = 0;
            foreach (var minion in minions)
                count++;
            return count;
        }

        /// <summary>
        ///     Fire Combo
        /// </summary>
        private void Combo()
        {
            // If Mana is < ManaManager : return;
            // Combo
            WUsage();
            QUsage();
            EUsage();
            RUsage();
        }

        private void Harass()
        {
            // Harass
            var t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (!t.IsValidTarget())
                return;
            if (Player.ManaPercent > Config.HarassManaManager.GetValue<Slider>().Value)
                if (Config.HarassW.GetValue<bool>() && W.IsInRange(t))
                    SpellCast.SebbySpellMain(W, t);
            if (Player.ManaPercent > Config.HarassManaManager.GetValue<Slider>().Value)
                if (Config.HarassQ.GetValue<bool>() && Q.IsInRange(t))
                    SpellCast.SebbySpellMain(Q, t);
            if (Player.ManaPercent > Config.HarassManaManager.GetValue<Slider>().Value)
                if (Config.HarassE.GetValue<bool>() && E.IsInRange(t))
                    E.CastOnUnit(t);
        }

        private void LaneClear()
        {
            // LaneClear W
            if (Player.ManaPercent > Config.laneclearMinimumMana.GetValue<Slider>().Value)
                if (Config.laneclearW.GetValue<bool>() && W.IsReady())
                {
                    var allMinionsW = Cache.GetMinions(Player.ServerPosition, W.Range, MinionTeam.Enemy);
                    var farmPos = Q.GetCircularFarmLocation(allMinionsW, 250);
                    if (farmPos.MinionsHit > Config.LaneClearWMinions.GetValue<Slider>().Value)
                        W.Cast(farmPos.Position);
                }
            // LaneClear E
            if (Player.ManaPercent > Config.laneclearMinimumMana.GetValue<Slider>().Value)
            {
                var allMinions = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Enemy);
                if (Config.laneclearE.GetValue<bool>() && E.IsReady() && !W.IsReady())
                    foreach (var minion in allMinions)
                        if (minion.IsValidTarget() && minion.HasBuff("brandablaze"))
                            E.CastOnUnit(minion);
                        else
                            E.CastOnUnit(minion);
            }
        }


        /// <summary>
        ///     Perfection is a must
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        private float CalculateDamage(Obj_AI_Base enemy)
        {
            // Calculate Damage + Bleed ticking
            float damage = 0;
            var BrandPassive = enemy.MaxHealth*14/100;
            var PassiveDamage = enemy.MaxHealth*2/100;
            // bool HasABlazeBomb = enemy.HasBuff("brandablazebomb");
            damage += BrandPassive;
            if (enemy.HasBuff("brandablazebomb"))
                damage += PassiveDamage;
            double ultdamage = 0;

            if (Q.IsReady())
                damage += Q.GetDamage(enemy);

            if (W.IsReady())
                if (enemy.HasBuff("brandablaze") || enemy.HasBuff("ablaze"))
                {
                    var BonusWDmg = W.GetDamage(enemy) + W.GetDamage(enemy)*25/100;
                    damage += BonusWDmg;
                }
                else
                {
                    damage += W.GetDamage(enemy);
                }

            if (E.IsReady())
                damage += E.GetDamage(enemy);

            if (R.IsReady())
                ultdamage += Player.GetSpellDamage(enemy, SpellSlot.R);

            return damage;
        }

        public class Configuration
        {
            public Configuration(Menu root)
            {
                ComboMenu = MenuFactory.CreateMenu(root, "Combo");
                HarassMenu = MenuFactory.CreateMenu(root, "Harass");
                LaneClearMenu = MenuFactory.CreateMenu(root, "Lane Clear");
                MiscMenu = MenuFactory.CreateMenu(root, "Misc");
                DrawingMenu = MenuFactory.CreateMenu(root, "Drawing");

                Combos(MenuItemFactory.Create(ComboMenu));
                Harass(MenuItemFactory.Create(HarassMenu));
                LaneClear(MenuItemFactory.Create(LaneClearMenu));
                Misc(MenuItemFactory.Create(MiscMenu));
                Drawings(MenuItemFactory.Create(DrawingMenu));
            }

            public Menu ComboMenu { get; }
            public Menu HarassMenu { get; }
            public Menu LaneClearMenu { get; }
            public Menu LastHitMenu { get; private set; }
            public Menu MiscMenu { get; }
            public Menu UltimateMenu { get; private set; }
            public Menu DrawingMenu { get; }
            public MenuItem LaneClearWMinions { get; private set; }
            public MenuItem laneclearMinimumMana { get; private set; }
            public MenuItem StunTargetKey { get; private set; }
            public MenuItem FillColor { get; private set; }
            public MenuItem DrawComboDamage { get; private set; }
            public MenuItem DrawPassiveBombOnEnemy { get; private set; }
            public MenuItem drawE { get; private set; }
            public MenuItem drawW { get; private set; }
            public MenuItem drawQ { get; private set; }
            public MenuItem drawR { get; private set; }
            public MenuItem laneclearW { get; private set; }
            public MenuItem laneclearE { get; private set; }
            public MenuItem PrioritizeStunned { get; private set; }
            public MenuItem QAblazedEnemy { get; private set; }
            public MenuItem QGapC { get; private set; }
            public MenuItem InterruptEQ { get; private set; }
            public MenuItem QOnlyAblazed { get; private set; }
            public MenuItem NearbyREnemies { get; private set; }
            public MenuItem HarassManaManager { get; private set; }
            public MenuItem HarassBlockAA { get; private set; }
            public MenuItem HarassE { get; private set; }
            public MenuItem HarassW { get; private set; }
            public MenuItem HarassQ { get; private set; }
            public MenuItem ComboUseR { get; private set; }
            public MenuItem ComboUseE { get; private set; }
            public MenuItem ComboUseW { get; private set; }
            public MenuItem ComboUseQ { get; private set; }

            private void Combos(MenuItemFactory factory)
            {
                ComboUseQ = factory.WithName("Use Q").WithValue(true).Build();
                ComboUseW = factory.WithName("Use W").WithValue(true).Build();
                ComboUseE = factory.WithName("Use E").WithValue(true).Build();
                ComboUseR = factory.WithName("Use R").WithValue(true).Build();
            }

            private void Drawings(MenuItemFactory factory)
            {
                // Drawing Menu
                drawQ = factory.WithName("Draw Q Range").WithValue(false).Build();
                drawW = factory.WithName("Draw W Range").WithValue(false).Build();
                drawE = factory.WithName("Draw E Range").WithValue(false).Build();
                drawR = factory.WithName("Draw R Range").WithValue(true).Build();
                DrawPassiveBombOnEnemy = factory.WithName("Draw Passive Bomb on Enemy (Range)").WithValue(true).Build();
                DrawComboDamage = factory
                    .WithName("Draw Combo Damage")
                    .WithValue(true)
                    .Build();

                FillColor = factory
                    .WithName("Fill Color")
                    .WithValue(new Circle(true, Color.FromArgb(204, 255, 0, 1)))
                    .Build();
            }

            private void Harass(MenuItemFactory factory)
            {
                HarassQ = factory.WithName("Use Q").WithValue(true).Build();
                HarassW = factory.WithName("Use W").WithValue(false).Build();
                HarassE = factory.WithName("Use E").WithValue(false).Build();
                HarassBlockAA = factory.WithName("Block AA's in Harass Mode").WithValue(true).Build();
                HarassManaManager = factory
                    .WithName("Minimum Mana%")
                    .WithValue(new Slider(30))
                    .WithTooltip("Minimum Mana that you need to have to AutoHarass with Q/W/E.")
                    .Build();
            }

            private void LaneClear(MenuItemFactory factory)
            {
                // LaneClear Menu
                laneclearW = factory.WithName("Use W to LaneHit").WithValue(true).Build();
                laneclearE = factory.WithName("Use E to LaneHit").WithValue(true).Build();

                LaneClearWMinions = factory
                    .WithName("Minimum Enemies inside W")
                    .WithValue(new Slider(2, 1, 10))
                    .Build();

                laneclearMinimumMana = factory
                    .WithName("Minimum Mana%")
                    .WithValue(new Slider(30))
                    .WithTooltip("Minimum Mana that you need to have to LaneHit with Q/E.")
                    .Build();
            }

            private void Misc(MenuItemFactory factory)
            {
                // Misc Menu
                PrioritizeStunned = factory.WithName("Prioritize Stunned Targets?").WithValue(true).Build();

                StunTargetKey =
                    factory.WithName("Stun the target if possible while holding Key -> 'G'")
                        .WithValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Press))
                        .Build();

                QAblazedEnemy = factory.WithName("Auto Q if Target's [ABlazed]").WithValue(true).Build();
                QGapC = factory.WithName("Auto Stun GapClosers").WithValue(true).Build();
                InterruptEQ = factory.WithName("Auto E-Q to Interrupt").WithValue(true).Build();
                QOnlyAblazed = factory.WithName("Use Q Only if Enemy is Ablazed").WithValue(true).Build();
                NearbyREnemies =
                    factory.WithName("Use R in Combo if X Enemies are nearby 'X' ->").WithValue(true).Build();
            }
        }
    }
}