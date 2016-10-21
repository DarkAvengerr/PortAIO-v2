using EloBuddy;
using LeagueSharp.SDK;
namespace Flowers_Akali
{
    using Flowers_Akali.Common;
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using System;
    using System.Linq;
    using static Flowers_Akali.Common.Manager;

    internal class Program
    {
        internal static AIHeroClient Me;
        internal static Menu Menu;
        private static Spell Q, W, E, R;
        private static HpBarDraw DrawHpBar = new HpBarDraw();

        public static void Main()
        {
            Bootstrap.Init();
            if (GameObjects.Player.ChampionName.ToLower() != "akali")
            {
                return;
            }

            Me = GameObjects.Player;

            LoadSpell();
            LoadMenu();
            LoadEvent();
        }

        private static void LoadSpell()
        {
            Q = new Spell(SpellSlot.Q, 600f);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 325f);
            R = new Spell(SpellSlot.R, 700f);

            Q.SetTargetted(0.45f, float.MaxValue);
            W.SetSkillshot(0.25f, 400f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetTargetted(0.20f, float.MaxValue);
        }

        private static void LoadMenu()
        {
            Menu = new Menu("Flowers'Akali", "Flowers'Akali", true).Attach();

            Utility.Tools.Inject();

            var ComboMenu = Menu.Add(new Menu("Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q", true));
                ComboMenu.Add(new MenuBool("W", "Use W", true));
                ComboMenu.Add(new MenuSlider("WCount", "Use W | Counts Enemies >= ", 2, 1, 5));
                ComboMenu.Add(new MenuSlider("WHp", "Use W | Player HealthPercent <= %", 40));
                ComboMenu.Add(new MenuBool("E", "Use E", true));
                ComboMenu.Add(new MenuBool("R", "Use R", true));
                ComboMenu.Add(new MenuSliderButton("RGap", "Use R Gapcloser | When R Buff Count >=", 2, 1, 3, true));
                ComboMenu.Add(new MenuKeyBind("RTower", "Use R | Under Enemy Turret", System.Windows.Forms.Keys.T, KeyBindType.Toggle)).Active = true;
                ComboMenu.Add(new MenuBool("Ignite", "Use Ignite", true));
            }

            var HarassMenu = Menu.Add(new Menu("Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuBool("QLastHit", "Use Q | LastHit", true));
                HarassMenu.Add(new MenuBool("E", "Use E", true));
                HarassMenu.Add(new MenuSlider("Mana", "Harass Mode | Min ManaPercent >= %", 60));
            }

            var LaneClearMenu = Menu.Add(new Menu("LaneClear", "Lane Clear"));
            {
                LaneClearMenu.Add(new MenuBool("Q", "Use Q", true));
                LaneClearMenu.Add(new MenuSliderButton("E", "Use E | Min Hit Count >= ", 3, 1, 5, true));
                LaneClearMenu.Add(new MenuSlider("Mana", "LaneClear Mode | Min ManaPercent >= %", 60));
            }

            var JungleClearMenu = Menu.Add(new Menu("JungleClear", "Jungle Clear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuBool("E", "Use E", true));
                JungleClearMenu.Add(new MenuSlider("Mana", "JungleClear Mode | Min ManaPercent >= %", 60));
            }

            var LastHitMenu = Menu.Add(new Menu("LastHit", "Last Hit"));
            {
                LastHitMenu.Add(new MenuBool("Q", "Use Q", true));
                LastHitMenu.Add(new MenuSlider("Mana", "LastHit Mode | Min ManaPercent >= %", 60));
            }

            var KillStealMenu = Menu.Add(new Menu("KillSteal", "Kill Steal"));
            {
                KillStealMenu.Add(new MenuBool("Q", "Use Q", true));
                KillStealMenu.Add(new MenuBool("E", "Use E", true));
                KillStealMenu.Add(new MenuBool("R", "Use R", true));
                KillStealMenu.Add(new MenuBool("RCount", "Use R | Only R Count >= 2", true));
                KillStealMenu.Add(new MenuSeparator("RList", "R List"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => KillStealMenu.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, AutoEnableList.Contains(i.ChampionName))));
                }
            }

            var FleeMenu = Menu.Add(new Menu("Flee", "Flee"));
            {
                FleeMenu.Add(new MenuBool("W", "W", true));
                FleeMenu.Add(new MenuSliderButton("R", "Use R | When R Buff Count >=", 2, 1, 3, true));
                FleeMenu.Add(new MenuKeyBind("Key", "Flee Key", System.Windows.Forms.Keys.Z, KeyBindType.Press));
            }

            var GapCloserMenu = Menu.Add(new Menu("GapCloser", "Anti GapCloser"));
            {
                GapCloserMenu.Add(new MenuBool("W", "W", true));
            }

            var DrawMenu = Menu.Add(new Menu("Draw", "Draw"));
            {
                DrawMenu.Add(new MenuBool("Q", "Q Range"));
                DrawMenu.Add(new MenuBool("E", "E Range"));
                DrawMenu.Add(new MenuBool("R", "R Range"));
                DrawMenu.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
                DrawMenu.Add(new MenuBool("DrawR", "Draw R Status", true));
            }
        }

        private static void LoadEvent()
        {
            Events.OnGapCloser += OnGapCloser;
            Variables.Orbwalker.OnAction += OnAction;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnGapCloser(object obj, Events.GapCloserEventArgs Args)
        {
            if (Args.IsDirectedToPlayer && Args.Sender.IsEnemy && Args.Sender is AIHeroClient)
            {
                var target = Args.Sender;

                if (target.IsValidTarget(250))
                {
                    W.Cast(Me.ServerPosition);
                }
            }
        }

        private static void OnAction(object obj, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.AfterAttack)
            {
                if (E.IsReady())
                {
                    if (InCombo)
                    {
                        var target = GetTarget(E);

                        if (CheckTarget(target))
                        {
                            if (Menu["Combo"]["E"] && target.IsValidTarget(E.Range))
                            {
                                E.Cast();
                                return;
                            }
                        }
                    }

                    if (InHarass)
                    {
                        var target = GetTarget(E);

                        if (CheckTarget(target))
                        {
                            if (Menu["Harass"]["E"] && target.IsValidTarget(E.Range))
                            {
                                E.Cast();
                                return;
                            }
                        }
                    }

                    if (InClear)
                    {
                        var mobs = GetMobs(Me.Position, E.Range);

                        if (mobs.Count() > 0)
                        {
                            if (Menu["JungleClear"]["E"] && mobs.FirstOrDefault().IsValidTarget(E.Range))
                            {
                                E.Cast();
                                return;
                            }
                        }
                    }
                }
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu["Flee"]["Key"].GetValue<MenuKeyBind>().Active)
            {
                Flee();
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

            LastHit();
            KillSteal();
        }

        private static void Flee()
        {
            Variables.Orbwalker.Move(Game.CursorPos);

            if (Menu["Flee"]["W"] && W.IsReady())
            {
                var target = GetTarget(W.Range);

                if (CheckTarget(target))
                {
                    if (target.IsValidTarget(W.Range))
                    {
                        if (Me.ServerPosition.Distance(Game.CursorPos) < 200)
                        {
                            W.Cast(Me.ServerPosition);
                            return;
                        }
                        else if (Me.ServerPosition.Distance(Game.CursorPos) >= 200 &&
                            Me.ServerPosition.Distance(Game.CursorPos) <= 400)
                        {
                            W.Cast(Game.CursorPos);
                            return;
                        }
                    }
                }
            }

            if (Menu["Flee"]["R"].GetValue<MenuSliderButton>().BValue && R.IsReady() && RCount >= Menu["Flee"]["R"].GetValue<MenuSliderButton>().SValue)
            {
                var minion = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValidTarget(R.Range) && !x.IsAlly && R.CanCast(x)).
                    OrderBy(x => x.Position.Distance(Game.CursorPos)).FirstOrDefault();

                if (minion != null)
                {
                    R.CastOnUnit(minion);
                    return;
                }
            }
        }

        private static void Combo()
        {
            var target = GetTarget(1400, DamageType.Magical);

            if (CheckTarget(target))
            {
                if (Menu["Combo"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.CastOnUnit(target);
                }

                if (Menu["Combo"]["E"] && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.Cast();
                }

                if (Menu["Combo"]["W"] && W.IsReady() &&
                    (Me.CountEnemyHeroesInRange(700) >= Menu["Combo"]["WCount"].GetValue<MenuSlider>().Value ||
                    Me.HealthPercent <= Menu["Combo"]["WHp"].GetValue<MenuSlider>().Value))
                {
                    W.Cast(Me.ServerPosition);
                }

                if (Menu["Combo"]["R"] && R.IsReady())
                {
                    if (!Menu["Combo"]["RTower"].GetValue<MenuKeyBind>().Active && target.IsUnderEnemyTurret())
                    {
                        return;
                    }

                    if (Menu["Combo"]["RGap"].GetValue<MenuSliderButton>().BValue && target.DistanceToPlayer() > R.Range &&
                        target.IsValidTarget(1400) && Menu["Combo"]["RGap"].GetValue<MenuSliderButton>().SValue >= RCount)
                    {
                        var minion = GetMinions(Me.Position, R.Range).FirstOrDefault(x => x.Distance(target) < R.Range && R.CanCast(x));

                        if (minion != null)
                        {
                            R.CastOnUnit(minion);
                            return;
                        }
                    }

                    if (target.IsValidTarget(R.Range) && !target.IsValidTarget(E.Range))//!InAutoAttackRange(target))
                    {
                        R.CastOnUnit(target);
                    }
                }
            }
        }

        private static void Harass()
        {
            if (Me.ManaPercent >= Menu["Harass"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var target = GetTarget(Q.Range, DamageType.Magical);

                if (CheckTarget(target))
                {
                    if (Menu["Harass"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(target);
                        return;
                    }

                    if (Menu["Harass"]["E"] && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        E.Cast();
                        return;
                    }
                }
            }
        }

        private static void Lane()
        {
            if (Me.ManaPercent >= Menu["LaneClear"]["Mana"].GetValue<MenuSlider>().Value)
            {
                if (GetMinions(Me.Position, E.Range).Count() > Menu["LaneClear"]["E"].GetValue<MenuSliderButton>().SValue &&
                    Menu["LaneClear"]["E"].GetValue<MenuSliderButton>().BValue && E.IsReady())
                {
                    E.Cast();
                    return;
                }

                if (GetMinions(Me.Position, Q.Range).Count() > 0 && Menu["LaneClear"]["Q"] && Q.IsReady())
                {
                    Q.Cast(GetMinions(Me.Position, Q.Range).FirstOrDefault());
                    return;
                }
            }
        }

        private static void Jungle()
        {
            if (Me.ManaPercent >= Menu["JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
            {
                var mobs = GetMobs(Me.Position, Q.Range);

                if (mobs.Count() > 0)
                {
                    var mob = mobs.FirstOrDefault();

                    if (Menu["JungleClear"]["E"] && E.IsReady() && mob.IsValidTarget(E.Range))
                    {
                        E.Cast();
                        return;
                    }

                    if (Menu["JungleClear"]["Q"] && Q.IsReady() && mob.IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(mob);
                        return;
                    }
                }
            }
        }

        private static void LastHit()
        {
            if ((InLastHit && Menu["LastHit"]["Q"] && Me.ManaPercent >= Menu["LastHit"]["Mana"].GetValue<MenuSlider>().Value) ||
                (InHarass && Menu["Harass"]["QLastHit"] && Me.ManaPercent >= Menu["Harass"]["Mana"].GetValue<MenuSlider>().Value))
            {
                var minions = GetMinions(Me.Position, Q.Range);

                foreach (var min in minions.Where(x => x.IsValidTarget(Q.Range) &&
                !InAutoAttackRange(x) && x.Health <= Q.GetDamage(x)))
                {
                    if (min != null && Q.IsReady())
                    {
                        Q.CastOnUnit(min);
                        return;
                    }
                }
            }
        }

        private static void KillSteal()
        {
            foreach (var target in GetEnemies(R.Range))
            {
                if (CheckTarget(target))
                {
                    if (Menu["KillSteal"]["Q"] && target.Health < Q.GetDamage(target) && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(target);
                        return;
                    }

                    if (Menu["KillSteal"]["E"] && target.Health < E.GetDamage(target) && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        E.Cast();
                        return;
                    }

                    if (Menu["KillSteal"]["R"] && target.Health < R.GetDamage(target) &&
                        Menu["KillSteal"][target.ChampionName.ToLower()] && R.IsReady() &&
                        target.IsValidTarget(R.Range))
                    {
                        if (Menu["KillSteal"]["RCount"] && RCount < 2)
                        {
                            return;
                        }

                        R.CastOnUnit(target);
                        return;
                    }
                }
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu["Draw"]["Q"].GetValue<MenuBool>() && Q.IsReady())
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.AliceBlue, 2);

            if (Menu["Draw"]["E"].GetValue<MenuBool>() && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.LightYellow, 2);

            if (Menu["Draw"]["R"].GetValue<MenuBool>() && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, R.Range, System.Drawing.Color.OrangeRed, 2);

            if (Menu["Draw"]["DrawDamage"].GetValue<MenuBool>())
            {
                foreach (var target in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = target;
                    HpBarDraw.DrawDmg((float)GetDamage(target), new SharpDX.ColorBGRA(255, 204, 0, 170));
                }
            }

            if (Menu["Draw"]["DrawR"])
            {
                var text = "";

                if (Menu["Combo"]["RTower"].GetValue<MenuKeyBind>().Active)
                {
                    text = "On";
                }
                else
                {
                    text = "Off";
                }

                Drawing.DrawText(Me.HPBarPosition.X + 30, Me.HPBarPosition.Y - 40, System.Drawing.Color.Red, "R Turret(" + Menu["Combo"]["RTower"].GetValue<MenuKeyBind>().Key + "): ");
                Drawing.DrawText(Me.HPBarPosition.X + 115, Me.HPBarPosition.Y - 40, System.Drawing.Color.Yellow, text);
            }
        }

        private static int RCount
        {
            get
            {
                if (Me.HasBuff("AkaliShadowDance"))
                {
                    return Me.GetBuffCount("AkaliShadowDance");
                }
                else
                    return 0;
            }
        }
    }
}
