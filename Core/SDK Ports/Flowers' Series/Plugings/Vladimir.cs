using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Plugings
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using SharpDX;
    using System;
    using System.Linq;
    using static Common.Manager;

    public class Vladimir
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static HpBarDraw HpBarDraw = new HpBarDraw();

        private static Menu Menu => Program.Menu;
        private static AIHeroClient Me => Program.Me;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 600f);
            W = new Spell(SpellSlot.W, 350f);
            E = new Spell(SpellSlot.E, 610f);
            R = new Spell(SpellSlot.R, 625f);

            Q.SetTargetted(0.25f, float.MaxValue);
            R.SetSkillshot(0.25f, 175, 700, false, SkillshotType.SkillshotCircle);

            var ComboMenu = Menu.Add(new Menu("Vladimir_Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q", true));
                ComboMenu.Add(new MenuBool("W", "Use W", false));
                ComboMenu.Add(new MenuBool("WE", "Use W | Only E Is Charging!", true));
                ComboMenu.Add(new MenuBool("E", "Use E", true));
                ComboMenu.Add(new MenuSlider("ECharging", "Use E | Charging Max Range", 550, 300, 600));
                ComboMenu.Add(new MenuBool("R", "Use R", true));
                ComboMenu.Add(new MenuSlider("RCount", "Use R | Counts Enemies >=", 2, 1, 5));
                ComboMenu.Add(new MenuBool("Ignite", "Use Ignite", true));
                ComboMenu.Add(new MenuBool("Attack", "Disable AutoAttack In Combo Mode", false));
            }

            var HarassMenu = Menu.Add(new Menu("Vladimir_Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuKeyBind("Auto", "Auto Q Harass", System.Windows.Forms.Keys.G, KeyBindType.Toggle));
            }

            var LaneClearMenu = Menu.Add(new Menu("Vladimir_LaneClear", "Lane Clear"));
            {
                LaneClearMenu.Add(new MenuBool("Q", "Use Q", true));
                LaneClearMenu.Add(new MenuBool("QFrenzy", "Use Frenzy Q", true));
                LaneClearMenu.Add(new MenuBool("QLh", "Use Q | Only LastHit", true));
                LaneClearMenu.Add(new MenuSliderButton("E", "Use E | Min Hit Count >= ", 3, 1, 5, true));
            }

            var JungleClearMenu = Menu.Add(new Menu("Vladimir_JungleClear", "Jungle Clear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuBool("E", "Use E", true));
            }

            var LastHitMenu = Menu.Add(new Menu("Vladimir_LastHit", "Last Hit"));
            {
                LastHitMenu.Add(new MenuBool("Q", "Use Q", true));
                LastHitMenu.Add(new MenuBool("QFrenzy", "Use Frenzy Q", true));
                LastHitMenu.Add(new MenuBool("QHarass", "Use Q | Work in Harass Mode", true));
            }

            var KillStealMenu = Menu.Add(new Menu("Vladimir_KillSteal", "Kill Steal"));
            {
                KillStealMenu.Add(new MenuBool("Q", "Use Q", true));
                KillStealMenu.Add(new MenuBool("E", "Use E", true));
                KillStealMenu.Add(new MenuBool("R", "Use R", true));
                KillStealMenu.Add(new MenuSeparator("RList", "R List"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => KillStealMenu.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, AutoEnableList.Contains(i.ChampionName))));
                }
            }

            var FleeMenu = Menu.Add(new Menu("Vladimir_Flee", "Flee"));
            {
                FleeMenu.Add(new MenuBool("Q", "Use Q", false));
                FleeMenu.Add(new MenuBool("W", "Use W", true));
                FleeMenu.Add(new MenuKeyBind("Key", "Key", System.Windows.Forms.Keys.Z, KeyBindType.Press));
            }

            var MiscMenu = Menu.Add(new Menu("Vladimir_Misc", "Misc"));
            {
                MiscMenu.Add(new MenuSliderButton("WGap", "Use W Anti GapCloset", 40, 0, 100, true));
            }

            var DrawMenu = Menu.Add(new Menu("Vladimir_Draw", "Draw"));
            {
                DrawMenu.Add(new MenuBool("Q", "Q Range"));
                DrawMenu.Add(new MenuBool("E", "E Range"));
                DrawMenu.Add(new MenuBool("R", "R Range"));
                DrawMenu.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
                DrawMenu.Add(new MenuBool("Auto", "Draw Auto Q Status", true));
            }

            WriteConsole(GameObjects.Player.ChampionName + " Inject!");

            Variables.Orbwalker.OnAction += OnAction;
            Events.OnGapCloser += OnGapCloser;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnAction(object sender, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.BeforeAttack && InCombo && Menu["Vladimir_Combo"]["Attack"])
            {
                Args.Process = false;
            }
        }

        private static void OnGapCloser(object obj, Events.GapCloserEventArgs Args)
        {
            if (Menu["Vladimir_Misc"]["WGap"].GetValue<MenuSliderButton>().BValue && W.IsReady() && Args.IsDirectedToPlayer)
            {
                if (Args.Sender.DistanceToPlayer() <= 250 || Args.End.DistanceToPlayer() <= 200)
                {
                    if (Me.HealthPercent <= Menu["Vladimir_Misc"]["WGap"].GetValue<MenuSliderButton>().SValue)
                        W.Cast();
                }
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Me.HasBuff("VladimirE") || Me.HasBuff("VladimirSanguinePool"))
            {
                Variables.Orbwalker.Move(Game.CursorPos);
                Variables.Orbwalker.AttackState = false;
            }
            else
            {
                Variables.Orbwalker.AttackState = true;
            }

            if (InCombo)
            {
                Combo();
            }

            if (InHarass)
            {
                Harass();
            }

            if (InClear)
            {
                Lane();
                Jungle();
            }

            if (InLastHit || (InHarass && Menu["Vladimir_LastHit"]["QHarass"]))
            {
                LastHit();
            }

            if (Menu["Vladimir_Harass"]["Auto"].GetValue<MenuKeyBind>().Active)
            {
                AutoHarass();
            }

            if (Menu["Vladimir_Flee"]["Key"].GetValue<MenuKeyBind>().Active)
            {
                Flee();
            }

            KillSteal();
        }

        private static void Combo()
        {
            var target = GetTarget(R.Range, DamageType.Magical);

            if (CheckTarget(target))
            {
                if (Menu["Vladimir_Combo"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.CanCast(target) && !E.IsCharging)
                {
                    Q.Cast(target);
                }

                if (Menu["Vladimir_Combo"]["E"] && target.IsValidTarget(E.Range) && E.IsReady())
                {
                    if (E.IsCharging && target.DistanceToPlayer() >= Menu["Vladimir_Combo"]["ECharging"].GetValue<MenuSlider>().Value)
                    {
                        Me.Spellbook.CastSpell(E.Slot, false);
                    }
                    else if (!Me.HasBuff("VladimirE"))
                    {
                        E.StartCharging();
                    }
                }

                if (Menu["Vladimir_Combo"]["W"] && W.IsReady() && target.IsValidTarget(E.Range))
                {
                    if (Menu["Vladimir_Combo"]["WE"] && Me.HasBuff("VladimirE"))
                    {
                        W.Cast();
                    }
                    else if (!Menu["Vladimir_Combo"]["WE"] && target.IsValidTarget(W.Range))
                    {
                        W.Cast();
                    }
                }

                if (Menu["Vladimir_Combo"]["R"] && R.IsReady() && target.IsValidTarget(R.Range) && !E.IsCharging)
                {
                    if (target.Health <= GetDamage(target) + Me.GetAutoAttackDamage(target))
                    {
                        R.Cast(target);
                    }

                    if (R.GetPrediction(target).CastPosition.CountEnemyHeroesInRange(375f) >= Menu["Vladimir_Combo"]["RCount"].GetValue<MenuSlider>().Value)
                    {
                        R.Cast(R.GetPrediction(target).CastPosition);
                    }
                }
            }
        }

        private static void Harass()
        {
            var target = GetTarget(Q);

            if (CheckTarget(target))
            {
                if (Menu["Vladimir_Harass"]["Q"] && Q.IsReady() && Q.CanCast(target))
                {
                    Q.Cast(target);
                }
            }
        }

        private static void Lane()
        {
            var Minions = GetMinions(Me.Position, Q.Range);

            if (Minions.Count() > 0)
            {
                if (Menu["Vladimir_LaneClear"]["Q"] && Q.IsReady() && !E.IsCharging)
                {
                    if (!Menu["Vladimir_LaneClear"]["QFrenzy"] && Me.HasBuff("vladimirqfrenzy"))
                    {
                        return;
                    }

                    if (!Menu["Vladimir_LaneClear"]["QLh"])
                    {
                        Q.Cast(Minions.FirstOrDefault());
                    }
                    else if (Menu["Vladimir_LaneClear"]["QLh"])
                    {
                        var min = Minions.FirstOrDefault(x => x.Health < Q.GetDamage(x) && x.Health > Me.GetAutoAttackDamage(x));

                        if (min != null)
                        {
                            Q.Cast(min);
                        }
                    }
                }

                if (Menu["Vladimir_LaneClear"]["E"].GetValue<MenuSliderButton>().BValue && E.IsReady())
                {
                    if (Minions.Count() >= Menu["Vladimir_LaneClear"]["E"].GetValue<MenuSliderButton>().SValue)
                    {
                        if (!Me.HasBuff("VladimirE"))
                        {
                            E.StartCharging();
                        }
                    }
                }
            }
        }

        private static void Jungle()
        {
            var Mobs = GetMobs(Me.Position, Q.Range, true);

            if (Mobs.Count() > 0)
            {
                if (Menu["Vladimir_JungleClear"]["Q"] && Q.IsReady() && !E.IsCharging)
                {
                    Q.Cast(Mobs.FirstOrDefault());
                }

                if (Menu["Vladimir_JungleClear"]["E"] && E.IsReady())
                {
                    if (!Me.HasBuff("VladimirE"))
                    {
                        E.StartCharging();
                    }
                }
            }
        }

        private static void LastHit()
        {
            var Minions = GetMinions(Me.Position, Q.Range);

            if (Minions.Count() > 0)
            {
                if (Menu["Vladimir_LastHit"]["Q"] && Q.IsReady())
                {
                    var min = Minions.FirstOrDefault(x => x.Health < Q.GetDamage(x) && x.Health > Me.GetAutoAttackDamage(x));
                    var target = Variables.TargetSelector.GetTarget(Q);

                    if (InHarass)
                    {
                        if (target != null && target.IsHPBarRendered && Me.HasBuff("vladimirqfrenzy") && Menu["Vladimir_Harass"]["Q"])
                        {
                            Q.Cast(target);
                        }
                        else if (target == null && min != null)
                        {
                            if (!Menu["Vladimir_LastHit"]["QFrenzy"] && Me.HasBuff("vladimirqfrenzy"))
                            {
                                return;
                            }

                            Q.Cast(min);
                        }
                    }
                    else
                    {
                        if (min != null)
                        {
                            if (!Menu["Vladimir_LastHit"]["QFrenzy"] && Me.HasBuff("vladimirqfrenzy"))
                            {
                                return;
                            }

                            Q.Cast(min);
                        }
                    }
                }
            }
        }

        private static void AutoHarass()
        {
            var target = GetTarget(Q);

            if (CheckTarget(target))
            {
                if (Q.IsReady() && Q.CanCast(target))
                {
                    Q.Cast(target);
                }
            }
        }

        private static void Flee()
        {
            Variables.Orbwalker.Move(Game.CursorPos);

            var Qtarget = GetTarget(Q);
            var Wtarget = GetTarget(W.Range, DamageType.Magical);

            if (Menu["Vladimir_Flee"]["W"] && CheckTarget(Wtarget) && Wtarget.IsValidTarget(W.Range * 0.8f) && W.IsReady())
            {
                W.Cast();
            }

            if (Menu["Vladimir_Flee"]["Q"] && CheckTarget(Qtarget) && Qtarget.IsValidTarget(Q.Range) && Q.IsReady())
            {
                if (Me.HealthPercent < 60 && Qtarget.DistanceToPlayer() > Qtarget.GetRealAutoAttackRange())
                    Q.Cast(Qtarget);
            }
        }

        private static void KillSteal()
        {
            if (Menu["Vladimir_KillSteal"]["Q"] && Q.IsReady() && !E.IsCharging)
            {
                var qt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)).FirstOrDefault();

                if (CheckTarget(qt))
                {
                    Q.Cast(qt);
                    return;
                }
            }

            if (Menu["Vladimir_KillSteal"]["E"] && E.IsReady() && !E.IsCharging)
            {
                var et = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(E.Range) && x.Health < E.GetDamage(x)).FirstOrDefault();

                if (CheckTarget(et))
                {
                    E.Cast(et);
                    return;
                }
            }

            if (Menu["Vladimir_KillSteal"]["R"] && R.IsReady())
            {
                var rt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range) && x.Health < R.GetDamage(x) && Menu["Vladimir_KillSteal"][x.ChampionName.ToLower()]).FirstOrDefault();

                if (CheckTarget(rt))
                {
                    R.Cast(rt);
                    return;
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu["Vladimir_Draw"]["Q"] && Q.IsReady())
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.AliceBlue, 2);

            if (Menu["Vladimir_Draw"]["E"] && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.LightSeaGreen, 2);

            if (Menu["Vladimir_Draw"]["R"] && R.IsReady())
                Render.Circle.DrawCircle(Me.Position, R.Range, System.Drawing.Color.OrangeRed, 2);

            if (Menu["Vladimir_Draw"]["DrawDamage"])
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = target;
                    HpBarDraw.DrawDmg((float)GetDamage(target), new ColorBGRA(255, 204, 0, 170));
                }
            }

            if (Menu["Vladimir_Draw"]["Auto"])
            {
                var text = "";

                if (Menu["Harass"]["Auto"].GetValue<MenuKeyBind>().Active)
                {
                    text = "On";
                }
                else
                {
                    text = "Off";
                }

                Drawing.DrawText(Me.HPBarPosition.X + 30, Me.HPBarPosition.Y - 40, System.Drawing.Color.Red, "Auto Q (" + Menu["Harass"]["Auto"].GetValue<MenuKeyBind>().Key + "): ");
                Drawing.DrawText(Me.HPBarPosition.X + 115, Me.HPBarPosition.Y - 40, System.Drawing.Color.Yellow, text);
            }
        }
    }
}
