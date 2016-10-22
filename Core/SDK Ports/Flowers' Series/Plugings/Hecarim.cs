using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Plugings
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using SharpDX;
    using System;
    using System.Linq;
    using static Common.Manager;

    public class Hecarim
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static HpBarDraw DrawHpBar = new HpBarDraw();

        private static AIHeroClient Me => Program.Me;

        private static Menu Menu => Program.Menu;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 350f);
            W = new Spell(SpellSlot.W, 525f);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 1230f);

            R.SetSkillshot(0.25f, 300f, 1200f, false, SkillshotType.SkillshotCircle);

            var ComboMenu = Menu.Add(new Menu("Hecarim_Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q", true));
                ComboMenu.Add(new MenuBool("W", "Use W", true));
                ComboMenu.Add(new MenuBool("E", "Use E", true));
                ComboMenu.Add(new MenuBool("R", "Use R", true));
                ComboMenu.Add(new MenuBool("RSolo", "Use R | 1v1 Mode", true));
                ComboMenu.Add(new MenuSlider("RCount", "Use R | Counts Enemies >=", 2, 1, 5));
                ComboMenu.Add(new MenuBool("Ignite", "Use Ignite", true));
                ComboMenu.Add(new MenuBool("Item", "Use Item", true));
            }

            var HarassMenu = Menu.Add(new Menu("Hecarim_Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuSlider("Mana", "Harass Mode | Min ManaPercent >= %", 60));
                HarassMenu.Add(new MenuKeyBind("Auto", "Auto Q Harass", System.Windows.Forms.Keys.G, KeyBindType.Toggle));
                HarassMenu.Add(new MenuSlider("AutoMana", "Auto Harass Mode | Min ManaPercent >= %", 60));
            }

            var LaneClearMenu = Menu.Add(new Menu("Hecarim_LaneClear", "Lane Clear"));
            {
                LaneClearMenu.Add(new MenuSliderButton("Q", "Use Q | Min Hit Count >= ", 3, 1, 5, true));
                LaneClearMenu.Add(new MenuSliderButton("W", "Use W | Min Hit Count >= ", 3, 1, 5, true));
                LaneClearMenu.Add(new MenuSlider("Mana", "LaneClear Mode | Min ManaPercent >= %", 60));
            }

            var JungleClearMenu = Menu.Add(new Menu("Hecarim_JungleClear", "Jungle Clear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuBool("W", "Use W", true));
                JungleClearMenu.Add(new MenuBool("Item", "Use Item", true));
                JungleClearMenu.Add(new MenuSlider("Mana", "JungleClear Mode | Min ManaPercent >= %", 20));
            }

            var KillStealMenu = Menu.Add(new Menu("Hecarim_KillSteal", "Kill Steal"));
            {
                KillStealMenu.Add(new MenuBool("Q", "Use Q", true));
                KillStealMenu.Add(new MenuBool("W", "Use W", true));
                KillStealMenu.Add(new MenuSliderButton("R", "Use R | If Target Distance >=", 600, 100, (int)R.Range, true));
                KillStealMenu.Add(new MenuSeparator("RList", "R List"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => KillStealMenu.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, AutoEnableList.Contains(i.ChampionName))));
                }
            }

            var FleeMenu = Menu.Add(new Menu("Hecarim_Flee", "Flee"));
            {
                FleeMenu.Add(new MenuBool("W", "Use W", true));
                FleeMenu.Add(new MenuBool("E", "Use E", true));
                FleeMenu.Add(new MenuKeyBind("Key", "Key", System.Windows.Forms.Keys.Z, KeyBindType.Press));
            }

            var MiscMenu = Menu.Add(new Menu("Hecarim_Misc", "Misc"));
            {
                MiscMenu.Add(new MenuBool("EGap", "Use E Anti GapCloset", true));
                MiscMenu.Add(new MenuBool("EInt", "Use E Interrupt Spell", true));
                MiscMenu.Add(new MenuBool("RInt", "Use R Interrupt Spell", true));
            }

            var DrawMenu = Menu.Add(new Menu("Hecarim_Draw", "Draw"));
            {
                DrawMenu.Add(new MenuBool("Q", "Q Range"));
                DrawMenu.Add(new MenuBool("W", "W Range"));
                DrawMenu.Add(new MenuBool("E", "E Range"));
                DrawMenu.Add(new MenuBool("R", "R Range"));
                DrawMenu.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
                DrawMenu.Add(new MenuBool("Auto", "Draw Auto Q Status", true));
            }

            WriteConsole(GameObjects.Player.ChampionName + " Inject!");

            Variables.Orbwalker.OnAction += OnAction;
            Events.OnInterruptableTarget += OnInterruptableTarget;
            Events.OnGapCloser += OnGapCloser;
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += OnUpdate;
        }


        private static void OnAction(object obj, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.AfterAttack)
            {
                if (InCombo)
                {
                    var target = GetTarget(R.Range, DamageType.Physical);

                    if (CheckTarget(target))
                    {
                        if (Menu["Hecarim_Combo"]["W"] && Q.IsReady() && target.IsValidTarget(W.Range))
                        {
                            W.Cast();
                        }
                    }
                }

                if (InHarass)
                {
                    var Mobs = GetMobs(Me.Position, W.Range);

                    if (Mobs.Count() > 0)
                    {
                        var mob = Mobs.FirstOrDefault();

                        if (Me.ManaPercent >= Menu["Hecarim_JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
                        {
                            if (Menu["Hecarim_JungleClear"]["W"] && W.IsReady() && mob.IsValidTarget(W.Range))
                            {
                                W.Cast();
                            }
                        }
                    }
                }
            }
        }

        private static void OnInterruptableTarget(object obj, Events.InterruptableTargetEventArgs Args)
        {
            if (Args.Sender.IsEnemy)
            {
                var sender = Args.Sender as AIHeroClient;

                if (Menu["Hecarim_Misc"]["EInt"] && Args.DangerLevel >= DangerLevel.Medium && sender.IsValidTarget(E.Range) && E.IsReady())
                {
                    E.Cast();
                    Variables.Orbwalker.ForceTarget = sender;
                    return;
                }

                if (Menu["Hecarim_Misc"]["RInt"] && Args.DangerLevel >= DangerLevel.High && sender.IsValidTarget(R.Range) && R.IsReady())
                {
                    R.Cast(sender);
                    return;
                }
            }
        }

        private static void OnGapCloser(object obj, Events.GapCloserEventArgs Args)
        {
            if (Menu["Hecarim_Misc"]["EGap"] && Args.IsDirectedToPlayer)
            {
                var sender = Args.Sender as AIHeroClient;

                if (sender.IsEnemy && (Args.End.DistanceToPlayer() <= 200 || sender.DistanceToPlayer() <= 250) && E.IsReady())
                {
                    E.Cast();
                    Variables.Orbwalker.ForceTarget = sender;
                    return;
                }
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu["Hecarim_Draw"]["Q"] && Q.IsReady())
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.AliceBlue, 2);

            if (Menu["Hecarim_Draw"]["W"] && (W.IsReady() || Me.HasBuff("HecarimW")))
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.LightSeaGreen, 2);

            if (Menu["Hecarim_Draw"]["R"] && R.IsReady())
                Render.Circle.DrawCircle(Me.Position, R.Range, System.Drawing.Color.OrangeRed, 2);

            if (Menu["Hecarim_Draw"]["DrawDamage"])
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = target;
                    HpBarDraw.DrawDmg((float)GetDamage(target), new ColorBGRA(255, 204, 0, 170));
                }
            }

            if (Menu["Hecarim_Draw"]["Auto"])
            {
                var text = "";

                if (Menu["Hecarim_Harass"]["Auto"].GetValue<MenuKeyBind>().Active)
                {
                    text = "On";
                }
                else
                {
                    text = "Off";
                }

                Drawing.DrawText(Me.HPBarPosition.X + 30, Me.HPBarPosition.Y - 40, System.Drawing.Color.Red, "Auto Q (" + Menu["Hecarim_Harass"]["Auto"].GetValue<MenuKeyBind>().Key + "): ");
                Drawing.DrawText(Me.HPBarPosition.X + 115, Me.HPBarPosition.Y - 40, System.Drawing.Color.Yellow, text);
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
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

            if (Menu["Hecarim_Flee"]["Key"].GetValue<MenuKeyBind>().Active)
            {
                Flee();
            }

            if (Menu["Hecarim_Harass"]["Auto"].GetValue<MenuKeyBind>().Active)
            {
                AutoHarass();
            }

            KillSteal();
        }

        private static void Flee()
        {
            Variables.Orbwalker.Move(Game.CursorPos);

            if (Menu["Hecarim_Flee"]["E"] && E.IsReady())
            {
                E.Cast();
            }

            if (Menu["Hecarim_Flee"]["W"] && W.IsReady())
            {
                var target = GetTarget(W.Range, DamageType.Physical);

                if (CheckTarget(target) && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }
            }
        }

        private static void Combo()
        {
            var target = GetTarget(R.Range, DamageType.Physical);

            if (CheckTarget(target))
            {
                if (Menu["Hecarim_Combo"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.CanCast(target))
                {
                    Q.Cast(target);
                }

                if (Menu["Hecarim_Combo"]["W"] && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }

                if (Menu["Hecarim_Combo"]["E"] && E.IsReady() && E.CanCast(target) && !InAutoAttackRange(target) && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }

                if (Menu["Hecarim_Combo"]["R"] && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    if (Menu["Hecarim_Combo"]["RSolo"] && target.Health <= GetDamage(target) + Me.GetAutoAttackDamage(target) * 3 && Me.CountEnemyHeroesInRange(R.Range) == 1)
                    {
                        R.Cast(target);
                    }

                    if (R.GetPrediction(target).CastPosition.CountEnemyHeroesInRange(250) >= Menu["Hecarim_Combo"]["RCount"].GetValue<MenuSlider>().Value)
                    {
                        R.Cast(R.GetPrediction(target).CastPosition);
                    }
                }
            }
        }

        private static void Harass()
        {
            if (Me.ManaPercent >= Menu["Hecarim_Harass"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var target = GetTarget(Q.Range, DamageType.Physical);

                if (CheckTarget(target))
                {
                    if (Menu["Hecarim_Harass"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.Cast();
                    }
                }
            }
        }

        private static void AutoHarass()
        {
            if (!Me.IsRecalling() && !Me.IsUnderEnemyTurret() &&
                !InCombo && 
                !InHarass && 
                Me.ManaPercent >= Menu["Hecarim_Harass"]["AutoMana"].GetValue<MenuSlider>().Value)
            {
                var target = GetTarget(Q.Range, DamageType.Physical);

                if (CheckTarget(target))
                {
                    if (Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.Cast();
                    }
                }
            }
        }

        private static void Lane()
        {
            var Minions = GetMinions(Me.Position, W.Range);

            if (Minions.Count() > 0)
            {
                if (Me.ManaPercent >= Menu["Hecarim_LaneClear"]["Mana"].GetValue<MenuSlider>().Value)
                {
                    if (Menu["Hecarim_LaneClear"]["Q"].GetValue<MenuSliderButton>().BValue && Q.IsReady())
                    {
                        if (Minions.Where(x => x.IsValidTarget(Q.Range)).Count() >= Menu["Hecarim_LaneClear"]["Q"].GetValue<MenuSliderButton>().SValue)
                        {
                            Q.Cast();
                        }
                    }

                    if (Menu["Hecarim_LaneClear"]["W"].GetValue<MenuSliderButton>().BValue && W.IsReady())
                    {
                        if (Minions.Where(x => x.IsValidTarget(W.Range)).Count() >= Menu["Hecarim_LaneClear"]["W"].GetValue<MenuSliderButton>().SValue)
                        {
                            W.Cast();
                        }
                    }
                }
            }
        }

        private static void Jungle()
        {
            var Mobs = GetMobs(Me.Position, W.Range, true);

            if (Mobs.Count() > 0)
            {
                var mob = Mobs.FirstOrDefault();

                if (Me.ManaPercent >= Menu["Hecarim_JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
                {
                    if (Menu["Hecarim_JungleClear"]["Q"] && Q.IsReady() && mob.IsValidTarget(Q.Range))
                    {
                        Q.Cast();
                    }

                    if (Menu["Hecarim_JungleClear"]["W"] && W.IsReady() && mob.IsValidTarget(W.Range))
                    {
                        W.Cast();
                    }
                }
            }
        }

        private static void KillSteal()
        {
            if (Menu["Hecarim_KillSteal"]["Q"] && Q.IsReady())
            {
                var qt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)).FirstOrDefault();

                if (CheckTarget(qt))
                {
                    Q.Cast();
                    return;
                }
            }

            if (Menu["Hecarim_KillSteal"]["W"] && W.IsReady())
            {
                var wt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(W.Range) && x.Health < W.GetDamage(x)).FirstOrDefault();

                if (CheckTarget(wt))
                {
                    W.Cast();
                    return;
                }
            }

            if (Menu["Hecarim_KillSteal"]["R"].GetValue<MenuSliderButton>().BValue && R.IsReady())
            {
                var rt = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range) && x.Health < R.GetDamage(x) &&
                Menu["Hecarim_KillSteal"][x.ChampionName.ToLower()] &&
                x.DistanceToPlayer() >= Menu["Hecarim_KillSteal"]["R"].GetValue<MenuSliderButton>().SValue).FirstOrDefault();

                if (CheckTarget(rt))
                {
                    R.Cast(rt);
                    return;
                }
            }
        }
    }
}
