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
    using System.Collections.Generic;
    using System.Linq;
    using static Common.Manager;

    public class Karma
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static int SwitchETickCount = 0, SwitchRTickCount = 0;
        private static HpBarDraw HpBarDraw = new HpBarDraw();

        private static AIHeroClient Me => Program.Me;

        private static Menu Menu => Program.Menu;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 950f);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 800f);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 60f, 1700f, true, SkillshotType.SkillshotLine);
            W.SetTargetted(0.25f, 2200f);
            E.SetTargetted(0.25f, float.MaxValue);

            var ComboMenu = Menu.Add(new Menu("Karma_Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q", true));
                ComboMenu.Add(new MenuBool("W", "Use W", true));
                ComboMenu.Add(new MenuList<string>("E", "Use E | Mode", new[] { "Low Hp", "Gank", "Off" }));
                ComboMenu.Add(new MenuSlider("EHp", "Use E Low Hp Mode | Min HealthPercent >= %", 45));
                ComboMenu.Add(new MenuKeyBind("ESwitch", "Switch E Mode Key", System.Windows.Forms.Keys.G, KeyBindType.Press));
                ComboMenu.Add(new MenuList<string>("R", "Use R | Mode", new[] { "All", "Q", "W", "E", "Off" }));
                ComboMenu.Add(new MenuKeyBind("RSwitch", "Switch R Mode Key", System.Windows.Forms.Keys.H, KeyBindType.Press));
            }

            var HarassMenu = Menu.Add(new Menu("Karma_Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuBool("R", "Use R", true));
                HarassMenu.Add(new MenuSlider("Mana", "Harass Mode | Min ManaPercent >= %", 60));
                HarassMenu.Add(new MenuKeyBind("Auto", "Auto Q Harass", System.Windows.Forms.Keys.T, KeyBindType.Toggle));
                HarassMenu.Add(new MenuSlider("AutoMana", "Auto Harass Mode | Min ManaPercent >= %", 60));
            }

            var LaneClearMenu = Menu.Add(new Menu("Karma_LaneClear", "Lane Clear"));
            {
                LaneClearMenu.Add(new MenuSliderButton("Q", "Use Q | Min Hit Count >= ", 3, 1, 5, true));
                LaneClearMenu.Add(new MenuSlider("Mana", "LaneClear Mode | Min ManaPercent >= %", 60));
            }

            var JungleClearMenu = Menu.Add(new Menu("Karma_JungleClear", "Jungle Clear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuBool("W", "Use W", true));
                JungleClearMenu.Add(new MenuBool("E", "Use E", true));
                JungleClearMenu.Add(new MenuBool("R", "Use RQ", true));
                JungleClearMenu.Add(new MenuSlider("Mana", "JungleClear Mode | Min ManaPercent >= %", 50));
            }

            var KillStealMenu = Menu.Add(new Menu("Karma_KillSteal", "Kill Steal"));
            {
                KillStealMenu.Add(new MenuBool("Q", "Use Q", true));
                KillStealMenu.Add(new MenuBool("W", "Use W", true));
                KillStealMenu.Add(new MenuBool("R", "Use R", true));
                KillStealMenu.Add(new MenuSeparator("KillStealList", "KillSteal List"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => KillStealMenu.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, AutoEnableList.Contains(i.ChampionName))));
                }
            }

            var FleeMenu = Menu.Add(new Menu("Karma_Flee", "Flee"));
            {
                FleeMenu.Add(new MenuBool("Q", "Use Q", true));
                FleeMenu.Add(new MenuBool("W", "Use W", true));
                FleeMenu.Add(new MenuBool("E", "Use E", true));
                FleeMenu.Add(new MenuBool("R", "Use R", true));
                FleeMenu.Add(new MenuKeyBind("Key", "Key", System.Windows.Forms.Keys.Z, KeyBindType.Press));
            }

            var MiscMenu = Menu.Add(new Menu("Karma_Misc", "Misc"));
            {
                MiscMenu.Add(new MenuBool("QGap", "Use Q Anti GapCloset", true));
                MiscMenu.Add(new MenuBool("WGap", "Use W Anti GapCloset", true));
                MiscMenu.Add(new MenuBool("EGap", "Use E Anti GapCloset", true));
                MiscMenu.Add(new MenuBool("RGap", "Use R Interrupt Spell", true));
                MiscMenu.Add(new MenuBool("Support", "Support Mode!", false));
            }

            var DrawMenu = Menu.Add(new Menu("Karma_Draw", "Draw"));
            {
                DrawMenu.Add(new MenuBool("Q", "Q Range"));
                DrawMenu.Add(new MenuBool("W", "W Range"));
                DrawMenu.Add(new MenuBool("E", "E Range"));
                DrawMenu.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
                DrawMenu.Add(new MenuBool("Auto", "Draw Auto Q Status", true));
                DrawMenu.Add(new MenuBool("ComboE", "Draw Combo E Status", true));
                DrawMenu.Add(new MenuBool("ComboR", "Draw Combo R Status", true));
            }

            WriteConsole(GameObjects.Player.ChampionName + " Inject!");

            Events.OnGapCloser += OnGapCloser;
            Variables.Orbwalker.OnAction += OnAction;
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += OnUpdate;
        }


        private static void OnGapCloser(object obj, Events.GapCloserEventArgs Args)
        {
            if (Args.IsDirectedToPlayer)
            {
                var sender = Args.Sender as AIHeroClient;

                if (sender.IsEnemy && sender.IsValidTarget(300))
                {
                    if (Menu["Karma_Misc"]["RGap"] && R.IsReady())
                    {
                        R.Cast();
                    }
                    else if (Menu["Karma_Misc"]["WGap"] && W.IsReady())
                    {
                        W.Cast(sender);
                    }
                    else if (Menu["Karma_Misc"]["EGap"] && E.IsReady())
                    {
                        E.Cast(Me);
                    }
                    else if (Menu["Karma_Misc"]["QGap"] && Q.IsReady())
                    {
                        Q.Cast(sender);
                    }
                }
            }
        }

        private static void OnAction(object obj, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.BeforeAttack && InHarass && Menu["Karma_Misc"]["Support"] && Args.Target.Type == GameObjectType.obj_AI_Minion)
            {
                Args.Process = false;
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu["Karma_Draw"]["Q"] && Q.IsReady())
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.AliceBlue, 2);

            if (Menu["Karma_Draw"]["W"] && (W.IsReady() || Me.HasBuff("HecarimW")))
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.LightSeaGreen, 2);

            if (Menu["Karma_Draw"]["E"] && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.OrangeRed, 2);

            if (Menu["Karma_Draw"]["DrawDamage"])
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(e => e.IsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = target;
                    HpBarDraw.DrawDmg((float)GetDamage(target), new ColorBGRA(255, 204, 0, 170));
                }
            }

            if (Menu["Karma_Draw"]["Auto"])
            {
                var text = "";

                if (Menu["Karma_Harass"]["Auto"].GetValue<MenuKeyBind>().Active)
                {
                    text = "On";
                }
                else
                {
                    text = "Off";
                }

                Drawing.DrawText(Me.HPBarPosition.X + 30, Me.HPBarPosition.Y - 40, System.Drawing.Color.Red, "Auto Q (" + Menu["Karma_Harass"]["Auto"].GetValue<MenuKeyBind>().Key + "): ");
                Drawing.DrawText(Me.HPBarPosition.X + 115, Me.HPBarPosition.Y - 40, System.Drawing.Color.Yellow, text);
            }

            if (Menu["Karma_Draw"]["ComboE"])
            {
                var text = "";
                var x = Drawing.WorldToScreen(Me.Position).X;
                var y = Drawing.WorldToScreen(Me.Position).Y;

                if (Menu["Karma_Combo"]["E"].GetValue<MenuList>().Index == 0)
                    text = "Low Hp!";
                else if (Menu["Karma_Combo"]["E"].GetValue<MenuList>().Index == 1)
                    text = "Gank!";
                else
                    text = "Off!";

                Drawing.DrawText(x - 55, y + 40, System.Drawing.Color.Red, "Combo E (" + Menu["Karma_Combo"]["ESwitch"].GetValue<MenuKeyBind>().Key + "): ");
                Drawing.DrawText(x + 50, y + 40, System.Drawing.Color.Yellow, text);
            }

            if (Menu["Karma_Draw"]["ComboR"])
            {
                var text = "";
                var x = Drawing.WorldToScreen(Me.Position).X;
                var y = Drawing.WorldToScreen(Me.Position).Y;

                if (Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index == 0)
                    text = "All!";
                else if (Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index == 1)
                    text = "Q!";
                else if (Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index == 2)
                    text = "W!";
                else if (Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index == 3)
                    text = "E!";
                else if (Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index == 4)
                    text = "Off!";

                Drawing.DrawText(x - 55, y + 60, System.Drawing.Color.Red, "Combo R (" + Menu["Karma_Combo"]["RSwitch"].GetValue<MenuKeyBind>().Key + "): ");
                Drawing.DrawText(x + 50, y + 60, System.Drawing.Color.Yellow, text);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            SwitchE();

            SwitchR();

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

            if (Menu["Karma_Flee"]["Key"].GetValue<MenuKeyBind>().Active)
            {
                Flee();
            }

            if (Menu["Karma_Harass"]["Auto"].GetValue<MenuKeyBind>().Active)
            {
                AutoHarass();
            }

            KillSteal();
        }

        private static void SwitchE()
        {
            if (Menu["Karma_Combo"]["ESwitch"].GetValue<MenuKeyBind>().Active)
            {
                if (Variables.TickCount - SwitchETickCount > 500)
                {
                    SwitchETickCount = Variables.TickCount;

                    var EMode = Menu["Karma_Combo"]["E"].GetValue<MenuList>().Index;

                    switch (EMode)
                    {
                        case 0:
                            Menu["Karma_Combo"]["E"].GetValue<MenuList>().Index = 1;
                            break;
                        case 1:
                            Menu["Karma_Combo"]["E"].GetValue<MenuList>().Index = 2;
                            break;
                        case 2:
                            Menu["Karma_Combo"]["E"].GetValue<MenuList>().Index = 0;
                            break;
                        default:
                            break;
                    }

                }
            }
        }

        private static void SwitchR()
        {
            if (Menu["Karma_Combo"]["RSwitch"].GetValue<MenuKeyBind>().Active)
            {
                if (Variables.TickCount - SwitchRTickCount > 500)
                {
                    SwitchRTickCount = Variables.TickCount;

                    var EMode = Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index;

                    switch (EMode)
                    {
                        case 0:
                            Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index = 1;
                            break;
                        case 1:
                            Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index = 2;
                            break;
                        case 2:
                            Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index = 3;
                            break;
                        case 3:
                            Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index = 4;
                            break;
                        case 4:
                            Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index = 0;
                            break;
                        default:
                            break;
                    }

                }
            }
        }

        private static void Combo()
        {
            var target = GetTarget(1200, DamageType.Magical);

            if (CheckTarget(target))
            {
                if (Menu["Karma_Combo"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    var QPred = Q.GetPrediction(target, (Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index == 0 || Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index == 1));

                    if (QPred.Hitchance >= HitChance.VeryHigh)
                    {
                        if ((Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index == 0 || Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index == 1) && R.IsReady())
                        {
                            R.Cast();
                        }
                        else
                        {
                            Q.Cast(QPred.CastPosition);
                        }
                    }
                }

                if (Menu["Karma_Combo"]["W"] && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    if ((Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index == 0 || Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index == 2) && R.IsReady())
                    {
                        R.Cast();
                    }
                    else
                    {
                        W.Cast(target);
                    }
                }

                if (Menu["Karma_Combo"]["E"].GetValue<MenuList>().Index != 2 && E.IsReady())
                {
                    if (Menu["Karma_Combo"]["E"].GetValue<MenuList>().Index == 0)
                    {
                        if (Me.HealthPercent <= Menu["Karma_Combo"]["EHp"].GetValue<MenuSlider>().Value)
                        {
                            if ((Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index == 0 || Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index == 3) && R.IsReady())
                            {
                                R.Cast();
                            }
                            else
                            {
                                E.Cast(Me);
                            }
                        }
                    }
                    else if (Menu["Karma_Combo"]["E"].GetValue<MenuList>().Index == 1)
                    {
                        if (target.DistanceToPlayer() <= Q.Range + 100)
                        {
                            if ((Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index == 0 || Menu["Karma_Combo"]["R"].GetValue<MenuList>().Index == 3) && R.IsReady())
                            {
                                R.Cast();
                            }
                            else
                            {
                                E.Cast(Me);
                            }
                        }
                    }
                }
            }
        }

        private static void Harass()
        {
            if (Me.ManaPercent >= Menu["Karma_Harass"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var target = GetTarget(Q.Range, DamageType.Magical);

                if (CheckTarget(target))
                {
                    if (Menu["Karma_Harass"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.CanCast(target))
                    {
                        var QPred = Q.GetPrediction(target, Menu["Karma_Harass"]["R"]);

                        if (QPred.Hitchance >= HitChance.VeryHigh)
                        {
                            if (Menu["Karma_Harass"]["R"] && R.IsReady())
                            {
                                R.Cast();
                            }
                            else
                            {
                                Q.Cast(QPred.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        private static void Lane()
        {
            if (Me.ManaPercent >= Menu["Karma_LaneClear"]["Mana"].GetValue<MenuSlider>().Value)
            {
                if (Menu["Karma_LaneClear"]["Q"].GetValue<MenuSliderButton>().BValue && Q.IsReady())
                {
                    var Minions = GetMinions(Me.Position, Q.Range);

                    if (Minions.Count() > 0)
                    {
                        var QFarm = Q.GetLineFarmLocation(Minions, Q.Width);

                        if (QFarm.MinionsHit >= Menu["Karma_LaneClear"]["Q"].GetValue<MenuSliderButton>().SValue)
                        {
                            Q.Cast(QFarm.Position);
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
                var mob = Mobs.FirstOrDefault();

                if (Me.ManaPercent >= Menu["Karma_JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
                {
                    if (Menu["Karma_JungleClear"]["Q"] && Q.IsReady() && mob.IsValidTarget(Q.Range))
                    {
                        if (Menu["Karma_JungleClear"]["R"] && R.IsReady())
                        {
                            R.Cast();
                        }
                        else
                        {
                            Q.Cast(mob);
                        }
                    }

                    if (Menu["Karma_JungleClear"]["W"] && W.IsReady() && mob.IsValidTarget(W.Range))
                    {
                        W.Cast(mob);
                    }

                    if (Menu["Karma_JungleClear"]["E"] && W.IsReady() && mob.IsAttackingPlayer)
                    {
                        E.Cast(Me);
                    }
                }
            }
        }

        private static void Flee()
        {
            Variables.Orbwalker.Move(Game.CursorPos);

            if (Menu["Karma_Flee"]["E"] && E.IsReady())
            {
                if (Menu["Karma_Flee"]["R"] && R.IsReady())
                {
                    R.Cast();
                }
                else
                {
                    E.Cast(Me);
                }
            }

            var target = GetTarget(Q.Range, DamageType.Magical);

            if (CheckTarget(target))
            {
                if (Menu["Karma_Flee"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    var QPred = Q.GetPrediction(target, Menu["Karma_Flee"]["R"]);

                    if (QPred.Hitchance >= HitChance.VeryHigh)
                    {
                        if (Menu["Karma_Flee"]["R"] && R.IsReady())
                        {
                            R.Cast();
                        }
                        else
                        {
                            Q.Cast(QPred.CastPosition);
                        }
                    }
                }

                if (Menu["Karma_Flee"]["W"] && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    if (Menu["Karma_Flee"]["R"] && R.IsReady())
                    {
                        R.Cast();
                    }
                    else
                    {
                        W.Cast(target);
                    }
                }
            }
        }

        private static void AutoHarass()
        {
            if (!Me.IsRecalling() && !Me.IsUnderEnemyTurret() && !InCombo && !InHarass && Me.ManaPercent >= Menu["Karma_Harass"]["AutoMana"].GetValue<MenuSlider>().Value)
            {
                var target = GetTarget(Q.Range, DamageType.Physical);

                if (CheckTarget(target))
                {
                    if (Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        var QPred = Q.GetPrediction(target, Menu["Karma_Harass"]["R"]);

                        if (QPred.Hitchance >= HitChance.VeryHigh)
                        {
                            if (Menu["Karma_Harass"]["R"] && R.IsReady())
                            {
                                R.Cast();
                            }
                            else
                            {
                                Q.Cast(QPred.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        private static void KillSteal()
        {
            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Q.Range) && !x.IsZombie && x.IsHPBarRendered && Menu["Karma_KillSteal"][x.ChampionName.ToLower()]))
            {
                if (CheckTarget(target))
                {
                    if (Menu["Karma_KillSteal"]["Q"] && Q.IsReady())
                    {
                        var QPred = Q.GetPrediction(target, Menu["Karma_KillSteal"]["R"]);

                        if (QPred.Hitchance >= HitChance.VeryHigh)
                        {
                            if (Menu["Karma_KillSteal"]["R"] && R.IsReady())
                            {
                                R.Cast();
                            }
                            else
                            {
                                Q.Cast(QPred.CastPosition);
                            }
                        }
                    }

                    if (Menu["Karma_KillSteal"]["W"] && W.IsReady())
                    {
                        if (Menu["Karma_KillSteal"]["R"] && R.IsReady())
                        {
                            R.Cast();
                        }
                        else
                        {
                            W.Cast(target);
                        }
                    }
                }
            }
        }
    }
}
