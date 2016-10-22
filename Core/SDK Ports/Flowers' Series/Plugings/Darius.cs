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
    using System;
    using System.Linq;
    using static Common.Manager;

    public static class Darius
    {
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static HpBarDraw DrawHpBar = new HpBarDraw();

        private static Menu Menu => Program.Menu;
        private static AIHeroClient Me => Program.Me;

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 425f);
            W = new Spell(SpellSlot.W, 170f);
            E = new Spell(SpellSlot.E, 540f);
            R = new Spell(SpellSlot.R, 460f);
            E.SetSkillshot(0.01f, 100f, float.MaxValue, false, SkillshotType.SkillshotCone);

            var ComboMenu = Menu.Add(new Menu("Darius_Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q", true));
                ComboMenu.Add(new MenuBool("W", "Use W", true));
                ComboMenu.Add(new MenuBool("E", "Combo!", true));
                ComboMenu.Add(new MenuBool("EUnderTower", "Dont Cast In Turret", true));
            }

            var HarassMenu = Menu.Add(new Menu("Darius_Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuSlider("Mana", "Min Harass Mana >= %", 60));
            }

            var LaneClearMenu = Menu.Add(new Menu("Darius_LaneClear", "LaneClear"));
            {
                LaneClearMenu.Add(new MenuBool("Q", "Use Q", true));
                LaneClearMenu.Add(new MenuSlider("QMin", "Min Hit >= ", 3, 1, 5));
                LaneClearMenu.Add(new MenuSlider("Mana", "Min LaneClear Mana >= %", 50));
            }

            var JungleClearMenu = Menu.Add(new Menu("Darius_JungleClear", "JungleClear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuBool("W", "Use W", true));
                JungleClearMenu.Add(new MenuSlider("Mana", "Min Jungle Mana >= %", 30));
            }

            var AutoMenu = Menu.Add(new Menu("Darius_Auto", "Auto"));
            {
                AutoMenu.Add(new MenuBool("W", "Auto W LastHit", true));
                AutoMenu.Add(new MenuKeyBind("E", "Auto E", System.Windows.Forms.Keys.T, KeyBindType.Toggle));
                AutoMenu.Add(new MenuBool("R", "Auto R", true));
            }

            var EMenu = Menu.Add(new Menu("Darius_EBlackList", "E BlackList"));
            {
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => EMenu.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName)));
                }
            }

            var RMenu = Menu.Add(new Menu("Darius_RMenu", "R Settings"));
            {
                RMenu.Add(new MenuSlider("Tolerance", "Damage Tolerance", 0, -100, 100));
            }

            var DrawMenu = Menu.Add(new Menu("Darius_Draw", "Draw"));
            {
                DrawMenu.Add(new MenuBool("Q", "Q Range"));
                DrawMenu.Add(new MenuBool("E", "E Range"));
                DrawMenu.Add(new MenuBool("R", "R Range"));
                DrawMenu.Add(new MenuBool("RD", "R Damage", true));
            }

            WriteConsole(GameObjects.Player.ChampionName + " Inject!");

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Variables.Orbwalker.OnAction += OnAction;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
                return;

            if (Me.HasBuff("dariusqcast"))
            {
                Variables.Orbwalker.Move(Game.CursorPos);
            }

            if (InCombo)
            {
                ComboLogic();
            }

            if (InHarass && Me.ManaPercent >= Menu["Darius_Harass"]["Mana"].GetValue<MenuSlider>().Value)
            {
                HarassLogic();
            }

            if (InClear && Q.IsReady())
            {
                LaneLogic();
                JungleLogic();
            }

            if (Menu["Darius_Auto"]["E"].GetValue<MenuKeyBind>().Active && E.IsReady())
            {
                AutoELogic();
            }
        }

        private static void ComboLogic()
        {
            var target = GetTarget(Q.Range);

            if (CheckTarget(target))
            {
                if (Menu["Darius_Combo"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range) && QOutSide(target))
                {
                    Q.Cast();
                }

                if (Menu["Darius_Combo"]["E"] && E.IsReady() &&
                    !Menu["Darius_EBlackList"][target.ChampionName.ToLower()] &&
                    target.IsValidTarget(E.Range) && !InAutoAttackRange(target) && !target.HasBuff("BlackShield"))
                {
                    if (Menu["Darius_Combo"]["EUnderTower"] && Me.IsUnderEnemyTurret())
                        return;

                    E.Cast(target);
                }
            }
        }

        private static void HarassLogic()
        {
            var target = GetTarget(Q.Range);

            if (CheckTarget(target))
            {
                if (Menu["Darius_Harass"]["Q"] && Q.IsReady() && target.IsValidTarget(Q.Range) && QOutSide(target))
                {
                    Q.Cast();
                }
            }
        }

        private static void LaneLogic()
        {
            var minions = GetMinions(Me.Position, Q.Range);

            if (minions.Count() >= Menu["Darius_QMenu"]["LaneMin"].GetValue<MenuSlider>().Value && 
                Menu["Darius_QMenu"]["LaneClear"] &&
                Me.ManaPercent >= Menu["Darius_QMenu"]["LaneMana"].GetValue<MenuSlider>().Value)
            {
                Q.Cast();
            }
        }

        private static void JungleLogic()
        {
            var mobs = GetMobs(Me.Position, GetAttackRange(Me));

            if (mobs.Count() > 0 && Menu["Darius_JungleClear"]["Q"] &&
                Me.ManaPercent >= Menu["Darius_JungleClear"]["Mana"].GetValue<MenuSlider>().Value 
                && mobs.FirstOrDefault().Health > Me.GetRealAutoAttackRange())
            {
                Q.Cast();
            }
        }

        private static void AutoELogic()
        {
            if (Me.IsUnderEnemyTurret())
            {
                return;
            }

            var target = GetTarget(E.Range);

            if (CheckTarget(target) && !Menu["Darius_EBlackList"][target.ChampionName.ToLower()] && 
                target.IsValidTarget(E.Range) && !InAutoAttackRange(target) && !target.HasBuff("BlackShield"))
            {
                E.Cast(target);
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (Me.IsDead)
                return;

            if (Menu["Darius_Draw"]["Q"])
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.BlueViolet);

            if (Menu["Darius_Draw"]["E"] && E.IsReady())
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.Blue);

            if (Menu["Darius_Draw"]["R"] && R.IsReady())
                Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.White);

            if (Menu["Darius_Draw"]["RD"])
            {
                foreach (var e in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = e;
                    HpBarDraw.DrawDmg(GetRDamage(e), new SharpDX.ColorBGRA(255, 204, 0, 170));
                }
            }
        }

        private static void OnAction(object obj, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.BeforeAttack)
            {
                if (Me.HasBuff("dariusqcast"))
                {
                    Args.Process = false;
                }

                if (InHarass || InClear || InLastHit)
                {
                    if (Menu["Darius_Auto"]["W"] && W.IsReady())
                    {
                        var minions = GetMinions(Me.Position, GetAttackRange(Me) + 180);

                        if (minions.Count() > 0)
                        {
                            foreach (var min in minions.Where(m => m.IsValidTarget() && m.Health <= W.GetDamage(m) && InAutoAttackRange(m)))
                            {
                                if (min != null)
                                {
                                    W.Cast();
                                    Variables.Orbwalker.ForceTarget = min;
                                }
                            }
                        }
                    }
                }
            }

            if (Args.Type == OrbwalkingType.AfterAttack)
            {
                if (InCombo && Menu["Darius_Combo"]["W"])
                {
                    if (Args.Target is AIHeroClient && W.IsReady())
                    {
                        var target = Args.Target as AIHeroClient;

                        if (InAutoAttackRange(target))
                        {
                            W.Cast();
                        }
                    }
                }

                if (InClear && Menu["Darius_JungleClear"]["W"] && Me.ManaPercent >= Menu["Darius_JungleClear"]["Mana"].GetValue<MenuSlider>().Value)
                {
                    if (Args.Target is Obj_AI_Minion && W.IsReady())
                    {
                        var mobs = GetMobs(Me.Position, GetAttackRange(Me));

                        if (W.IsReady() && mobs.Count() > 0)
                        {
                            W.Cast();
                        }
                    }
                }
            }
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu["Darius_Auto"]["R"] && R.IsReady())
            {
                var target = GetTarget(R.Range);

                if (CheckTarget(target) && !target.IsInvulnerable && target.IsValidTarget(R.Range) && target.Health <= GetRDamage(target) && !target.HasBuff("willrevive"))
                {
                    R.CastOnUnit(target);
                    return;
                }
            }

            if (InCombo)
            {
                if (Menu["Darius_Combo"]["W"] && Args.Target is AIHeroClient && W.IsReady())
                {
                    var target = Args.Target as AIHeroClient;

                    if (InAutoAttackRange(target) && Me.CanAttack && target.IsHPBarRendered)
                    {
                        W.Cast();
                    }
                }
            }
        }

        private static bool QOutSide(AIHeroClient target)
        {
            if (target != null && Q.IsInRange(target))
            {
                if (target.DistanceToPlayer() >= 250)
                {
                    return true;
                }

                if (!InAutoAttackRange(target))
                {
                    return true;
                }
            }

            return false;
        }

        private static int GetPassiveCount(AIHeroClient target)
        {
            return target.GetBuffCount("DariusHemo") > 0 ? target.GetBuffCount("DariusHemo") : 0;
        }

        private static float GetRDamage(AIHeroClient target)
        {
            var Damage = R.GetDamage(target);

            if (!R.IsReady())
            {
                Damage = 0f;
            }

            // rengenrate
            Damage -= target.HPRegenRate;

            // tolerance
            Damage += Menu["Darius_RMenu"]["Tolerance"].GetValue<MenuSlider>().Value;

            // passive
            if (target.HasBuff("DariusHemo"))
                Damage += Damage * GetPassiveCount(target) * 0.2f;

            if (target.ChampionName == "Moredkaiser")
                Damage -= target.Mana;

            // exhaust
            if (Me.HasBuff("SummonerExhaust"))
                Damage = Damage * 0.6f;

            // blitzcrank passive
            if (target.HasBuff("BlitzcrankManaBarrierCD") && target.HasBuff("ManaBarrier"))
                Damage -= target.Mana / 2f;

            // kindred r
            if (target.HasBuff("KindredRNoDeathBuff"))
                Damage = 0;

            // tryndamere r
            if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
                Damage = 0;

            // kayle r
            if (target.HasBuff("JudicatorIntervention"))
                Damage = 0;

            // zilean r
            if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
                Damage = 0;

            // fiora w
            if (target.HasBuff("FioraW"))
                Damage = 0;

            return Damage;
        }
    }
}
