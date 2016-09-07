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
    using System;
    using System.Linq;
    using static Common.Manager;

    public class Ryze
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static HpBarDraw DrawHpBar = new HpBarDraw();
        private static bool CanShield = false;
        private static float SpellTime = 0;

        private static AIHeroClient Me => Program.Me;

        private static Menu Menu => Program.Menu;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 1000f);
            W = new Spell(SpellSlot.W, 600f);
            E = new Spell(SpellSlot.E, 600f);
            R = new Spell(SpellSlot.R, 1500f);

            Q.SetSkillshot(0.25f, 50f, 1700, true, SkillshotType.SkillshotLine);
            Q.DamageType = W.DamageType = E.DamageType = DamageType.Magical;

            var ComboMenu = Menu.Add(new Menu("Ryze_Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q", true));
                ComboMenu.Add(new MenuBool("QNCol", "Use Q|Smart No Collision", true));
                ComboMenu.Add(new MenuBool("W", "Use W", true));
                ComboMenu.Add(new MenuBool("E", "Use E", true));
                ComboMenu.Add(new MenuBool("Shield", "Active Shield", true));
                ComboMenu.Add(new MenuSlider("ShieldHp", "When Player HealthPercent <= %", 60));
                ComboMenu.Add(new MenuKeyBind("ShieldKey", "Or Active Key", System.Windows.Forms.Keys.T, KeyBindType.Toggle));
            }

            var HarassMenu = Menu.Add(new Menu("Ryze_Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuBool("W", "Use W", true));
                HarassMenu.Add(new MenuBool("E", "Use E", true));
                HarassMenu.Add(new MenuSlider("Mana", "Harass Mode | Min ManaPercent >= %", 60));
            }

            var LaneClearMenu = Menu.Add(new Menu("Ryze_LaneClear", "Lane Clear"));
            {
                LaneClearMenu.Add(new MenuBool("Q", "Use Q", true));
                LaneClearMenu.Add(new MenuBool("W", "Use W", true));
                LaneClearMenu.Add(new MenuSliderButton("E", "Use Q | Min Hit Count >= ", 3, 1, 5, true));
                LaneClearMenu.Add(new MenuSlider("Mana", "LaneClear Mode | Min ManaPercent >= %", 60));
            }

            var JungleClearMenu = Menu.Add(new Menu("Ryze_JungleClear", "Jungle Clear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuBool("W", "Use W", true));
                JungleClearMenu.Add(new MenuBool("E", "Use E", true));
                JungleClearMenu.Add(new MenuSlider("Mana", "JungleClear Mode | Min ManaPercent >= %", 60));
            }

            var LastHitMenu = Menu.Add(new Menu("Ryze_LastHit", "Last Hit"));
            {
                LastHitMenu.Add(new MenuBool("Q", "Use Q", true));
                LastHitMenu.Add(new MenuBool("W", "Use W"));
                LastHitMenu.Add(new MenuBool("E", "Use E"));
                LastHitMenu.Add(new MenuSlider("Mana", "LastHit Mode | Min ManaPercent >= %", 60));
            }

            var KillStealMenu = Menu.Add(new Menu("Ryze_KillSteal", "Kill Steal"));
            {
                KillStealMenu.Add(new MenuBool("Q", "Use Q", true));
                KillStealMenu.Add(new MenuBool("W", "Use W", true));
                KillStealMenu.Add(new MenuBool("E", "Use E", true));
            }

            var MiscMenu = Menu.Add(new Menu("Ryze_Misc", "Misc"));
            {
                MiscMenu.Add(new MenuBool("WGap", "Use W|Anti GapCloset", true));
                MiscMenu.Add(new MenuBool("WInt", "Use W|Interrupt Spell", true));
                MiscMenu.Add(new MenuKeyBind("Clear", "Use Spell In Clear Mode", System.Windows.Forms.Keys.G, KeyBindType.Toggle));
                MiscMenu.Add(new MenuBool("SmartDisbale", "Smart Disable Attack In Combo Mode", true));
                MiscMenu.Add(new MenuKeyBind("DisableAll", "Disable All Attack In Combo Mode", System.Windows.Forms.Keys.A, KeyBindType.Toggle));
            }

            var DrawMenu = Menu.Add(new Menu("Ryze_Draw", "Draw"));
            {
                DrawMenu.Add(new MenuBool("Q", "Q Range"));
                DrawMenu.Add(new MenuBool("W", "W Range"));
                DrawMenu.Add(new MenuBool("E", "E Range"));
                DrawMenu.Add(new MenuBool("R", "R Range"));
                DrawMenu.Add(new MenuBool("RMin", "R Range(MinMap)"));
                DrawMenu.Add(new MenuBool("Combo", "Combo Shield Status", true));
                DrawMenu.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
            }

            WriteConsole(GameObjects.Player.ChampionName + " Inject!");

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Variables.Orbwalker.OnAction += OnAction;
            Events.OnGapCloser += OnGapCloser;
            Events.OnInterruptableTarget += OnInterruptableTarget;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnUpdate(EventArgs Args)
        {
            R.Range = R.Level * 1500f;

            if (Me.IsDead)
            {
                return;
            }

            if (Menu["Ryze_Combo"]["Shield"] && 
                (Menu["Ryze_Combo"]["ShieldKey"].GetValue<MenuKeyBind>().Active || 
                Me.HealthPercent <= Menu["Ryze_Combo"]["ShieldHp"].GetValue<MenuSlider>().Value))
            {
                CanShield = true;
            }
            else
            {
                CanShield = false;
            }

            if (Menu["Ryze_Combo"]["QNCol"] && InCombo && CanShield)
            {
                Q.Collision = false;
            }
            else
            {
                Q.Collision = true;
            }

            if (InCombo)
            {
                Combo();
            }

            if (InHarass)
            {
                Harass();
            }

            if (InClear && Menu["Ryze_Misc"]["Clear"].GetValue<MenuKeyBind>().Active)
            {
                Lane();
                Jungle();
            }

            if (InLastHit)
            {
                LastHit();
            }

            KillSteal();
        }

        private static void Combo()
        {
            var target = GetTarget(Q.Range, DamageType.Magical);

            if (CheckTarget(target))
            {
                CastSpell(target, Menu["Ryze_Combo"]["Q"], Menu["Ryze_Combo"]["W"], Menu["Ryze_Combo"]["E"]);
            }
        }

        private static void Harass()
        {
            if (Me.ManaPercent >= Menu["Ryze_Harass"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var target = GetTarget(Q.Range, Q.DamageType);
                var minions = GetMinions(Me.Position, E.Range);
                var min = minions.Where(x => x.IsValidTarget(E.Range) && x.Position.Distance(target.Position) <= 300).FirstOrDefault();

                if (CheckTarget(target))
                {
                    if (Menu["Ryze_Harass"]["Q"] && target.IsValidTarget(Q.Range) && !Me.HasBuff("ryzeqiconfullcharge"))
                    {
                        if (min != null && min.Health < Q.GetDamage(min) && min.HasBuff("RyzeE"))
                        {
                            Q.Cast(min);
                        }
                        else
                        {
                            Q.Cast(target);
                        }
                    }

                    if (Menu["Ryze_Harass"]["E"])
                    {
                        if (min != null && min.Health < Q.GetDamage(min) + E.GetDamage(min))
                        {
                            E.CastOnUnit(min);
                        }
                        else if (target.IsValidTarget(E.Range))
                        {
                            E.CastOnUnit(target);
                        }
                    }
                }
            }
        }

        private static void Lane()
        {
            if (Me.ManaPercent >= Menu["Ryze_LaneClear"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var minions = GetMinions(Me.Position, Q.Range);

                if (minions.Count() > 0)
                {
                    if (Menu["Ryze_LaneClear"]["E"].GetValue<MenuSliderButton>().BValue && E.IsReady())
                    {
                        foreach (var minE in minions.Where(x => x.IsValidTarget(E.Range) && 
                        GetMinions(x.Position, 250).Count() >= Menu["Ryze_LaneClear"]["E"].GetValue<MenuSliderButton>().SValue))
                        {
                            if (minE != null)
                            {
                                E.CastOnUnit(minE);
                            }
                        }
                    }

                    foreach (var min in minions.Where(x => x.IsValidTarget(600) && x.HasBuff("RyzeE")))
                    {
                        if (min != null)
                        {
                            if (Menu["Ryze_LaneClear"]["W"] && W.IsReady() && min.Health < W.GetDamage(min))
                            {
                                W.CastOnUnit(min);
                            }
                            else if (Menu["Ryze_LaneClear"]["Q"] && Q.IsReady())
                            {
                                Q.Cast(min);
                            }
                        }
                    }

                    if (Menu["Ryze_LaneClear"]["Q"] && Q.IsReady() && !minions.FirstOrDefault().HasBuff("RyzeE"))
                    {
                        Q.Cast(minions.FirstOrDefault());
                    }
                }
            }
        }

        private static void Jungle()
        {
            if (Me.ManaPercent >= Menu["Ryze_JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var mobs = GetMobs(Me.Position, Q.Range);

                if (mobs.Count() > 0)
                {
                    var mob = mobs.FirstOrDefault();

                    if (Menu["Ryze_JungleClear"]["Q"] && Q.IsReady())
                    {
                        Q.Cast(mob);
                        return;
                    }

                    if (Menu["Ryze_JungleClear"]["E"] && E.IsReady())
                    {
                        E.CastOnUnit(mob);
                        return;
                    }

                    if (Menu["Ryze_JungleClear"]["W"] && W.IsReady())
                    {
                        W.CastOnUnit(mob);
                        return;
                    }
                }
            }
        }

        private static void LastHit()
        {
            if (Me.ManaPercent >= Menu["Ryze_LastHit"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var Minions = GetMinions(Me.Position, Q.Range);

                if (Minions.Count() > 0)
                {
                    var min = Minions.FirstOrDefault();

                    if (Menu["Ryze_LastHit"]["E"] && E.IsReady() && min.IsValidTarget(E.Range) && min.Health < E.GetDamage(min))
                    {
                        E.Cast(min);
                        return;
                    }

                    if (Menu["Ryze_LastHit"]["W"] && W.IsReady() && min.IsValidTarget(W.Range) && min.Health < W.GetDamage(min))
                    {
                        W.Cast(min);
                        return;
                    }

                    if (Menu["Ryze_LastHit"]["Q"] && Q.IsReady() && !Me.HasBuff("ryzeqiconfullcharge") && min.IsValidTarget(Q.Range) && min.Health < Q.GetDamage(min))
                    {
                        Q.Cast(min);
                        return;
                    }
                }
            }
        }

        private static void KillSteal()
        {
            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Q.Range) && !x.IsDead && !x.IsZombie && x.IsHPBarRendered))
            {
                if (CheckTarget(target))
                {
                    if (Menu["Ryze_KillSteal"]["Q"] && Q.IsReady() && target.Health < Q.GetDamage(target) && target.IsValidTarget(Q.Range))
                    {
                        Q.Cast(target);
                    }

                    if (Menu["Ryze_KillSteal"]["W"] && W.IsReady() && target.Health < W.GetDamage(target) && target.IsValidTarget(W.Range))
                    {
                        W.CastOnUnit(target);
                    }

                    if (Menu["Ryze_KillSteal"]["E"] && E.IsReady() && target.Health < E.GetDamage(target) && target.IsValidTarget(E.Range))
                    {
                        E.CastOnUnit(target);
                    }
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe)
            {
                if (Args.Slot == SpellSlot.Q || Args.Slot == SpellSlot.W || Args.Slot == SpellSlot.E)
                {
                    SpellTime = Game.Time;
                }
            }
        }

        private static void OnAction(object obj, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.BeforeAttack && InCombo)
            {
                if (Menu["Ryze_Misc"]["DisableAll"].GetValue<MenuKeyBind>().Active)
                {
                    Args.Process = false;
                }
                else if (Menu["Ryze_Misc"]["SmartDisbale"] && (W.IsReady() || E.IsReady()))
                {
                    Args.Process = false;
                }
                else
                {
                    Args.Process = true;
                }
            }
        }

        private static void OnGapCloser(object obj, Events.GapCloserEventArgs Args)
        {
            if (Menu["Ryze_Misc"]["WGap"] && W.IsReady() && Args.Sender.IsEnemy && 
                Args.Sender is AIHeroClient && Args.Sender.IsValidTarget(250))
            {
                W.CastOnUnit(Args.Sender);
            }
        }

        private static void OnInterruptableTarget(object obj, Events.InterruptableTargetEventArgs Args)
        {
            if (Menu["Ryze_Misc"]["WInt"] && W.IsReady() && Args.Sender.IsEnemy &&
                Args.Sender is AIHeroClient)
            {
                if (Args.DangerLevel >= DangerLevel.High && Args.Sender.IsValidTarget(W.Range))
                {
                    W.CastOnUnit(Args.Sender);
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu["Ryze_Draw"]["Q"] && Q.IsReady())
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.AliceBlue, 2);

            if (Menu["Ryze_Draw"]["W"] && (W.IsReady() || Me.HasBuff("IllaoiW")))
                Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.LightSeaGreen, 2);

            if (Menu["Ryze_Draw"]["E"] && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.LightYellow, 2);

            if (Menu["Ryze_Draw"]["R"] && R.IsReady())
                Render.Circle.DrawCircle(Me.Position, R.Range, System.Drawing.Color.OrangeRed, 2);

            if (Menu["Ryze_Draw"]["RMin"] && R.IsReady())
                DrawEndScene(R.Range);

            if (Menu["Ryze_Draw"]["Combo"])
            {
                var text = "";
                var x = Drawing.WorldToScreen(Me.Position).X;
                var y = Drawing.WorldToScreen(Me.Position).Y;

                if (CanShield)
                {
                    text = "On";
                }
                else if (!CanShield)
                {
                    text = "Off";
                }

                Drawing.DrawText(x - 60, y + 40, System.Drawing.Color.Red, "Combo Shield : ");
                Drawing.DrawText(x + 50, y + 40, System.Drawing.Color.Yellow, text);
            }

            if (Menu["Ryze_Draw"]["DrawDamage"])
            {
                foreach (var target in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = target;
                    HpBarDraw.DrawDmg((float)GetDamage(target), new SharpDX.ColorBGRA(255, 204, 0, 170));
                }
            }
        }

        private static void CastSpell(AIHeroClient target, bool useQ, bool useW, bool useE)
        {
            if (CheckTarget(target))
            {
                if (!CanShield && Game.Time - SpellTime > 0.3)
                {
                    if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.Cast(target);
                    }

                    if (useW && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        W.CastOnUnit(target);
                    }

                    if (useE && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        E.CastOnUnit(target);
                    }
                }
                else if (CanShield)
                {
                    if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.Cast(target);
                    }

                    if (useW && W.IsReady() && target.IsValidTarget(W.Range) && !Q.IsReady())
                    {
                        W.CastOnUnit(target);
                    }

                    if (useE && E.IsReady() && target.IsValidTarget(E.Range) && !Q.IsReady())
                    {
                        E.CastOnUnit(target);
                    }
                }
            }
        }
    }
}
