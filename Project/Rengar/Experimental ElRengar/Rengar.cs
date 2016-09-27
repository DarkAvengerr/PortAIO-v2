using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElRengar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using Color = System.Drawing.Color;
    public enum Spells
    {
        Q,

        W,

        E,

        R
    }

    public class Rengar : Standards
    {
        #region Properties

        private static IEnumerable<AIHeroClient> Enemies => HeroManager.Enemies;

        #endregion

        #region Public Methods and Operators


        public static void OnLoad()
        {
            try
            {
                if (!Player.IsChampion("Rengar"))
                {
                    return;
                }

                Ignite = Player.GetSpellSlot("summonerdot");
                Chat.Print(
                    "[00:01] <font color='#CC0000'>HEEEEEEY!</font> Use ElUtilitySuite for optimal results! xo jQuery!!");

                spells[Spells.E].SetSkillshot(0.25f, 70f, 1500f, true, SkillshotType.SkillshotLine);

                MenuInit.Initialize();
                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                CustomEvents.Unit.OnDash += OnDash;
                Drawing.OnEndScene += OnDrawEndScene;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
                Orbwalking.AfterAttack += AfterAttack;
                Orbwalking.BeforeAttack += BeforeAttack;
                AttackableUnit.OnDamage += AttackableUnit_OnDamage;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Methods

        private static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var targ = args.Target as AIHeroClient;
            if (!args.Unit.IsMe || targ == null)
            {
                return;
            }

            if (!spells[Spells.Q].IsReady() || !spells[Spells.Q].IsReady() || !Orbwalker.ActiveMode.Equals(Orbwalking.OrbwalkingMode.Combo))
            {
                return;
            }

            if (targ.Distance(Player) <= Orbwalking.GetRealAutoAttackRange(Player) - 10)
            {
                if (IsActive("Combo.Use.items"))
                {
                    ActiveModes.CastItems(targ);
                }
                spells[Spells.Q].Cast();
            }
        }


        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var targ = target as Obj_AI_Base;
            if (!unit.IsMe || targ == null)
            {
                return;
            }

            Orbwalker.SetOrbwalkingPoint(Vector3.Zero);
            var mode = Orbwalker.ActiveMode;
            if (mode.Equals(Orbwalking.OrbwalkingMode.None) || mode.Equals(Orbwalking.OrbwalkingMode.LastHit) || mode.Equals(Orbwalking.OrbwalkingMode.LaneClear))
            {
                return;
            }

            if (spells[Spells.Q].IsReady() && spells[Spells.Q].Cast())
            {
                return;
            }

            if (IsActive("Combo.Use.items"))
            {
                ActiveModes.CastItems(targ);
            }
        }

        private static void AttackableUnit_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            try
            {
                if (!IsActive("Heal.AutoHeal"))
                {
                    return;
                }

                var obj = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>((uint)args.Source.NetworkId);
                var source = ObjectManager.GetUnitByNetworkId<GameObject>((uint)args.Source.NetworkId);

                if (obj.Type != GameObjectType.AIHeroClient || source.Type != GameObjectType.AIHeroClient)
                {
                    return;
                }

                var hero = (AIHeroClient)obj;

                if (RengarR || !spells[Spells.W].IsReady()
                    || Player.Buffs.Any(
                        b =>
                        b.Name.ToLower().Contains("recall") || b.Name.ToLower().Contains("teleport") || Ferocity <= 4))
                {
                    return;
                }

                if (((int)(args.Damage / hero.Health) > MenuInit.Menu.Item("Heal.Damage").GetValue<Slider>().Value)
                    || (hero.HealthPercent < MenuInit.Menu.Item("Heal.HP").GetValue<Slider>().Value))
                {
                    spells[Spells.W].Cast();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private static void KillstealHandler()
        {
            if (!IsActive("Killsteal.On") || Player.IsRecalling() || RengarR)
            {
                return;
            }

            var target = Enemies.FirstOrDefault(x => x.IsValidTarget(spells[Spells.E].Range));
            if (target == null)
            {
                return;
            }

            if (!RengarR)
            {
                if (spells[Spells.W].GetDamage(target) > target.Health && target.IsValidTarget(spells[Spells.W].Range))
                {
                    spells[Spells.W].Cast();
                }

                if (spells[Spells.E].GetDamage(target) > target.Health && target.IsValidTarget(spells[Spells.E].Range))
                {
                    var prediction = spells[Spells.E].GetPrediction(target);
                    if (prediction.Hitchance >= HitChance.VeryHigh)
                    {
                        spells[Spells.E].Cast(prediction.CastPosition);
                    }
                }
            }
        }

        private static void OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            var target = TargetSelector.GetTarget(1500f, TargetSelector.DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }


            var mode = Orbwalker.ActiveMode;
            if (!mode.Equals(Orbwalking.OrbwalkingMode.Combo) || !mode.Equals(Orbwalking.OrbwalkingMode.Mixed))
            {
                return;
            }

            Orbwalker.SetOrbwalkingPoint(Vector3.Zero);
            if (Ferocity == 5)
            {
                switch (IsListActive("Combo.Prio").SelectedIndex)
                {
                    case 0:
                        if (spells[Spells.E].IsReady())
                        {
                            var targetE = TargetSelector.GetTarget(
                                spells[Spells.E].Range,
                                TargetSelector.DamageType.Physical);

                            if (targetE.IsValidTarget())
                            {
                                var pred = spells[Spells.E].GetPrediction(targetE);
                                if (pred.Hitchance >= HitChance.High)
                                {
                                    LeagueSharp.Common.Utility.DelayAction.Add(300, () => spells[Spells.E].Cast(target));
                                }
                            }
                        }
                        break;
                    case 2:
                        if (spells[Spells.Q].IsReady()
                            && Player.CountEnemiesInRange(Player.AttackRange + Player.BoundingRadius + 100) != 0)
                        {
                            spells[Spells.Q].Cast();
                        }
                        break;
                }
            }
            else
            {
                if (IsListActive("Combo.Prio").SelectedIndex != 0)
                {
                    if (spells[Spells.E].IsReady())
                    {
                        var targetE = TargetSelector.GetTarget(
                            spells[Spells.E].Range,
                            TargetSelector.DamageType.Physical);
                        if (targetE.IsValidTarget(spells[Spells.E].Range))
                        {
                            var pred = spells[Spells.E].GetPrediction(targetE);
                            if (pred.Hitchance >= HitChance.VeryHigh)
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(300, () => spells[Spells.E].Cast(target));
                            }
                        }
                    }
                }
            }

            switch (IsListActive("Combo.Prio").SelectedIndex)
            {
                case 0:
                    if (spells[Spells.E].IsReady() && target.IsValidTarget(spells[Spells.E].Range))
                    {
                        var pred = spells[Spells.E].GetPrediction(target);
                        LeagueSharp.Common.Utility.DelayAction.Add(300, () => spells[Spells.E].Cast(pred.CastPosition));
                    }
                    break;

                case 2:
                    if (IsActive("Beta.Cast.Q1") && RengarR)
                    {
                        spells[Spells.Q].Cast();
                    }
                    break;
            }
        }

        private static void OnDraw(EventArgs args)
        {
            try
            {
                var drawW = MenuInit.Menu.Item("Misc.Drawings.W").GetValue<Circle>();
                var drawE = MenuInit.Menu.Item("Misc.Drawings.E").GetValue<Circle>();
                var drawExclamation = MenuInit.Menu.Item("Misc.Drawings.Exclamation").GetValue<Circle>();
                var drawSearchRange = MenuInit.Menu.Item("Beta.Search.Range").GetValue<Circle>();
                var searchrange = MenuInit.Menu.Item("Beta.searchrange").GetValue<Slider>().Value;
                var drawsearchrangeQ = MenuInit.Menu.Item("Beta.Search.QCastRange").GetValue<Circle>();
                var searchrangeQCastRange = MenuInit.Menu.Item("Beta.searchrange.Q").GetValue<Slider>().Value;

                if (IsActive("Misc.Drawings.Off"))
                {
                    return;
                }

                if (IsActive("Beta.Cast.Q1"))
                {
                    if (drawSearchRange.Active && spells[Spells.R].Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, searchrange, Color.Orange);
                    }

                    if (drawsearchrangeQ.Active && spells[Spells.R].Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, searchrangeQCastRange, Color.Orange);
                    }
                }

                if (RengarR && drawExclamation.Active)
                {
                    if (spells[Spells.R].Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, 1450f, Color.DeepSkyBlue);
                    }
                }

                if (drawW.Active)
                {
                    if (spells[Spells.W].Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.W].Range, Color.Purple);
                    }
                }

                if (drawE.Active)
                {
                    if (spells[Spells.E].Level > 0)
                    {
                        Render.Circle.DrawCircle(ObjectManager.Player.Position, spells[Spells.E].Range, Color.White);
                    }
                }

                if (IsActive("Misc.Drawings.Prioritized"))
                {
                    switch (IsListActive("Combo.Prio").SelectedIndex)
                    {
                        case 0:
                            Drawing.DrawText(
                                Drawing.Width * 0.70f,
                                Drawing.Height * 0.95f,
                                Color.Yellow,
                                "Prioritized spell: E");
                            break;
                        case 1:
                            Drawing.DrawText(
                                Drawing.Width * 0.70f,
                                Drawing.Height * 0.95f,
                                Color.White,
                                "Prioritized spell: W");
                            break;
                        case 2:
                            Drawing.DrawText(
                                Drawing.Width * 0.70f,
                                Drawing.Height * 0.95f,
                                Color.White,
                                "Prioritized spell: Q");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnDrawEndScene(EventArgs args)
        {
            try
            {
                if (IsActive("Misc.Drawings.Minimap") && spells[Spells.R].Level > 0)
                {
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, spells[Spells.R].Range, Color.White, 1, 23, true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.SData.Name.Equals("rengarr", StringComparison.InvariantCultureIgnoreCase))
            {
                if (ActiveModes.Youmuu.IsOwned() && ActiveModes.Youmuu.IsReady())
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(2500, () => ActiveModes.Youmuu.Cast());
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    ActiveModes.Combo();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    ActiveModes.Laneclear();
                    ActiveModes.Jungleclear();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    ActiveModes.Harass();
                    break;
            }

            SwitchCombo();
            KillstealHandler();

            // E on Immobile targets
            if (IsActive("Misc.Root") && spells[Spells.E].IsReady())
            {
                if (!RengarR)
                {
                    var target = HeroManager.Enemies.FirstOrDefault(h => h.IsValidTarget(spells[Spells.E].Range));
                    if (target != null)
                    {
                        if (Ferocity == 5)
                        {
                            spells[Spells.E].CastIfHitchanceEquals(target, HitChance.Immobile);
                        }
                    }
                }
            }

            if (IsActive("Beta.Cast.Q1") && IsListActive("Combo.Prio").SelectedIndex == 2)
            {
                if (Ferocity != 5)
                {
                    return;
                }

                var searchrange = MenuInit.Menu.Item("Beta.searchrange").GetValue<Slider>().Value;
                var target = HeroManager.Enemies.FirstOrDefault(h => h.IsValidTarget(searchrange, false));
                if (!target.IsValidTarget())
                {
                    return;
                }

                // Check if Rengar is in ultimate
                if (RengarR)
                {
                    // Check if the player distance <= than the set search range
                    if (Player.Distance(target) <= MenuInit.Menu.Item("Beta.searchrange.Q").GetValue<Slider>().Value)
                    {
                        // Cast Q with the set delay
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            MenuInit.Menu.Item("Beta.Cast.Q1.Delay").GetValue<Slider>().Value,
                            () => spells[Spells.Q].Cast());
                    }
                }
            }

            spells[Spells.R].Range = 1000 + spells[Spells.R].Level * 1000;
        }

        private static void SwitchCombo()
        {
            try
            {
                var switchTime = Utils.GameTimeTickCount - LastSwitch;
                if (MenuInit.Menu.Item("Combo.Switch").GetValue<KeyBind>().Active && switchTime >= 350)
                {
                    switch (IsListActive("Combo.Prio").SelectedIndex)
                    {
                        case 0:
                            MenuInit.Menu.Item("Combo.Prio").SetValue(new StringList(new[] { "E", "W", "Q" }, 2));
                            LastSwitch = Utils.GameTimeTickCount;
                            break;
                        case 1:
                            MenuInit.Menu.Item("Combo.Prio").SetValue(new StringList(new[] { "E", "W", "Q" }, 0));
                            LastSwitch = Utils.GameTimeTickCount;
                            break;

                        default:
                            MenuInit.Menu.Item("Combo.Prio").SetValue(new StringList(new[] { "E", "W", "Q" }, 0));
                            LastSwitch = Utils.GameTimeTickCount;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
    }
}