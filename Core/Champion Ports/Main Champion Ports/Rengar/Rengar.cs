namespace ElRengarRevamped
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

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
                if (Player.ChampionName != "Rengar")
                {
                    return;
                }

                Youmuu = new Items.Item(3142, 0f);

                Ignite = Player.GetSpellSlot("summonerdot");
                Chat.Print(
                    "[00:01] <font color='#CC0000'>HEEEEEEY!</font> Use ElUtilitySuite for optimal results! xo jQuery!!");

                spells[Spells.E].SetSkillshot(0.25f, 70f, 1500f, true, SkillshotType.SkillshotLine);

                MenuInit.Initialize();
                Game.OnUpdate += OnUpdate;
                Drawing.OnDraw += OnDraw;
                CustomEvents.Unit.OnDash += OnDash;
                Drawing.OnEndScene += OnDrawEndScene;
                Obj_AI_Base.OnSpellCast += OnProcessSpellCast;
                Orbwalking.AfterAttack += AfterAttack;
                Orbwalking.BeforeAttack += BeforeAttack;
                //Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Methods

        public static void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe && args.Animation == "Spell5")
            {
                Console.WriteLine(args.Animation);
            }
        }


        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var enemy = target as Obj_AI_Base;
            if (!unit.IsMe || enemy == null || !(target is AIHeroClient))
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (Player.CountEnemiesInRange(Player.AttackRange + Player.BoundingRadius + 100) != 0)
                {
                    spells[Spells.Q].Cast();
                    ActiveModes.CastItems(enemy);
                }
            }
        }

        private static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!IsActive("Combo.Use.QQ"))
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && !HasPassive && spells[Spells.Q].IsReady()
                && !(IsListActive("Combo.Prio").SelectedIndex == 0
                     || IsListActive("Combo.Prio").SelectedIndex == 1 && Ferocity == 5))
            {
                if (Player.CountEnemiesInRange(Player.AttackRange + Player.BoundingRadius + 100) != 0)
                {
                    args.Process = false;
                    spells[Spells.Q].Cast();
                }
            }
        }

        private static void Heal()
        {
            if (RengarR || Player.IsRecalling() || Player.InFountain() || Ferocity != 5)
            {
                return;
            }

            if (Player.CountEnemiesInRange(1000) > 1 && spells[Spells.W].IsReady())
            {
                if (IsActive("Heal.AutoHeal")
                    && (Player.Health / Player.MaxHealth) * 100
                    <= MenuInit.Menu.Item("Heal.HP").GetValue<Slider>().Value)
                {
                    spells[Spells.W].Cast();
                }
            }
        }

        private static void KillstealHandler()
        {
            if (!IsActive("Killsteal.On") || Player.IsRecalling())
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

            if (!RengarR)
            {
                ActiveModes.CastItems(target);
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Orbwalking.Orbwalk(target ?? null, Game.CursorPos);
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
                if (Items.HasItem(3142) && Items.CanUseItem(3142))
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(2000, () => Items.UseItem(3142));
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
            Heal();
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