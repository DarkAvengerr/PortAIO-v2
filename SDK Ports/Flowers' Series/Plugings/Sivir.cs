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

    public static class Sivir
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
            Q = new Spell(SpellSlot.Q, 1200f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 90f, 1350f, false, SkillshotType.SkillshotLine);

            var ComboMenu = Menu.Add(new Menu("Sivir_Combo", "Combo"));
            {
                ComboMenu.Add(new MenuBool("Q", "Use Q", true));
                ComboMenu.Add(new MenuBool("W", "Use W", true));
                ComboMenu.Add(new MenuBool("R", "Use R", true));
                ComboMenu.Add(new MenuSlider("RMin", "When Enemies Counts >=", 2, 1, 5));
            }

            var HarassMenu = Menu.Add(new Menu("Sivir_Harass", "Harass"));
            {
                HarassMenu.Add(new MenuBool("Q", "Use Q", true));
                HarassMenu.Add(new MenuSlider("QMama", "Min Harass Q Mana Percent <= %", 50));
            }

            var LaneClearMenu = Menu.Add(new Menu("Sivir_LaneClear", "LaneClear"));
            {
                LaneClearMenu.Add(new MenuBool("Q", "Use Q", true));
                LaneClearMenu.Add(new MenuSlider("QMin", "Q Min Hit >=", 3, 1, 5));
                LaneClearMenu.Add(new MenuSlider("QMama", "Q Min Mana", 50));
                LaneClearMenu.Add(new MenuBool("W", "Use W", true));
                LaneClearMenu.Add(new MenuBool("WTurret", "W Turret", true));
                LaneClearMenu.Add(new MenuSlider("WMin", "W Counts Enemy <=", 1, 1, 5));
                LaneClearMenu.Add(new MenuSlider("WMama", "W Min Mana", 50));
            }

            var JungleClearMenu = Menu.Add(new Menu("Sivir_JungleClear", "JungleClear"));
            {
                JungleClearMenu.Add(new MenuBool("Q", "Use Q", true));
                JungleClearMenu.Add(new MenuSlider("QMama", "Q Min Mana", 30));
                JungleClearMenu.Add(new MenuBool("W", "Use W", true));
                JungleClearMenu.Add(new MenuSlider("WMama", "W Min Mana", 30));
            }

            var AutoMenu = Menu.Add(new Menu("Sivir_Auto", "Auto"));
            {
                AutoMenu.Add(new MenuBool("Q", "Auto Q (Can't Move!)", true));
                AutoMenu.Add(new MenuBool("KillSteal", "Auto Q (KillSteal)", true));
                AutoMenu.Add(new MenuBool("R", "Auto R", true));
                AutoMenu.Add(new MenuSlider("RMin", "Auto R | When Counts Enemy >=", 3, 1, 5));
            }

            var EMenu = Menu.Add(new Menu("Sivir_E", "E Settings"));
            {
                EMenu.Add(new MenuBool("Enable", "Enable", true));
                EMenu.Add(new MenuSlider("MeHp", "When Player Hp <= %", 80));
            }

            var Draw = Menu.Add(new Menu("Sivir_Draw", "Draw"));
            {
                Draw.Add(new MenuBool("Q", "Draw Q Range"));
                Draw.Add(new MenuBool("Damage", "Draw Combo Damage", true));
            }

            WriteConsole(GameObjects.Player.ChampionName + " Inject!");

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Variables.Orbwalker.OnAction += OnAction;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu["Sivir_E"]["Enable"] && Me.HealthPercent <= Menu["Sivir_E"]["MeHp"].GetValue<MenuSlider>().Value)
            {
                if (sender != null && sender.IsEnemy && sender is AIHeroClient)
                {
                    var e = (AIHeroClient)sender;

                    if (Args.Target != null)
                    {
                        if (Args.Target.IsMe)
                        {
                            if (CanE(e, Args))
                            {
                                DelayAction.Add(120, () => E.Cast());
                            }
                        }
                    }

                }
            }
        }

        private static bool CanE(AIHeroClient e, GameObjectProcessSpellCastEventArgs args)
        {
            if (AutoAttack.IsAutoAttack(args.SData.Name))
            {
                if (e.ChampionName == "TwistedFate")
                {
                    if (args.SData.Name == "GoldCardLock" || args.SData.Name == "RedCardLock" || args.SData.Name == "BlueCardLock")
                    {
                        return true;
                    }
                }
                else if (e.ChampionName == "Leona")
                {
                    if (args.SData.Name == "LeonaQ")
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (!AutoAttack.IsAutoAttack(args.SData.Name))
            {
                if (args.SData.Name.ToLower().Contains("summoner"))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private static void OnAction(object obj, OrbwalkingActionArgs Args)
        {
            if (Args.Type == OrbwalkingType.AfterAttack)
            {
                if (Args.Target is AIHeroClient && InCombo && Menu["Sivir_Combo"]["W"] && W.IsReady())
                {
                    var WTarget = Args.Target as AIHeroClient;

                    if (WTarget.IsValidTarget(Me.GetRealAutoAttackRange() + 180) && Me.CanAttack)
                    {
                        W.Cast();
                        Variables.Orbwalker.ResetSwingTimer();
                    }
                }


                if (Args.Target is Obj_AI_Minion && InClear)
                {
                    var WTarget = Args.Target as Obj_AI_Minion;

                    if (Menu["Sivir_LaneClear"]["W"] && Me.ManaPercent >= Menu["Sivir_LaneClear"]["WMama"].GetValue<MenuSlider>().Value &&
                        Me.CountEnemyHeroesInRange(800) <= Menu["Sivir_LaneClear"]["WMin"].GetValue<MenuSlider>().Value)
                    {
                        var Minions = GetMinions(Me.Position, GetAttackRange(Me) + 180);

                        if (W.IsReady() && Minions.Count() >= 2)
                        {
                            W.Cast();
                            Variables.Orbwalker.ResetSwingTimer();
                        }
                    }

                    if (Menu["Sivir_JungleClear"]["W"] && Me.ManaPercent >= Menu["Sivir_JungleClear"]["WMama"].GetValue<MenuSlider>().Value &&
                        WTarget.IsValidTarget(Me.GetRealAutoAttackRange()))
                    {
                        var Mobs = GetMobs(Me.Position, GetAttackRange(Me));

                        if (W.IsReady() && Mobs.Count() > 0)
                        {
                            W.Cast();
                            Variables.Orbwalker.ResetSwingTimer();
                        }
                    }
                }

                if (Args.Target is Obj_AI_Turret || Args.Target.Type == GameObjectType.obj_AI_Turret)
                {
                    if (Menu["Sivir_LaneClear"]["WTurret"])
                    {
                        if (W.IsReady() && Me.CountEnemyHeroesInRange(1000) == 0)
                        {
                            W.Cast();
                            Variables.Orbwalker.ResetSwingTimer();
                        }
                    }
                }
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
                return;

            if (InCombo)
            {
                var e = GetTarget(Q.Range);

                if (CheckTarget(e))
                {
                    if (Menu["Sivir_Combo"]["Q"] && Q.IsReady())
                    {
                        var QPred = Q.GetPrediction(e, true);

                        if (e.IsValidTarget(Q.Range) && !Me.IsDashing() &&
                            QPred.Hitchance >= HitChance.VeryHigh)
                        {
                            Q.Cast(QPred.CastPosition);
                        }
                    }

                    if (Menu["Sivir_Combo"]["R"] && Me.CountEnemyHeroesInRange(850) >= Menu["Sivir_Combo"]["RMin"].GetValue<MenuSlider>().Value && 
                        ((e.Health <= Me.GetAutoAttackDamage(e) * 3 && !Q.IsReady()) || (e.Health <= Me.GetAutoAttackDamage(e) * 3 + Q.GetDamage(e))))
                    {
                        R.Cast();
                    }
                }
            }

            if (InHarass)
            {
                if (Menu["Sivir_Harass"]["Q"] && Q.IsReady() && Me.ManaPercent >= Menu["Sivir_Harass"]["QMama"].GetValue<MenuSlider>().Value)
                {
                    var QTarget = GetTarget(Q.Range);
                    var QPred = Q.GetPrediction(QTarget, true);

                    if (QTarget.IsValidTarget(Q.Range) && !Me.IsDashing() && QPred.Hitchance >= HitChance.VeryHigh)
                    {
                        Q.Cast(QPred.CastPosition);
                    }
                }
            }

            if (InClear)
            {
                if (Menu["Sivir_LaneClear"]["Q"] && Me.ManaPercent >= Menu["Sivir_LaneClear"]["QMama"].GetValue<MenuSlider>().Value)
                {
                    var FL = Q.GetLineFarmLocation(GetMinions(Me.Position, Q.Range), Q.Width);

                    if (FL.MinionsHit >= Menu["Sivir_LaneClear"]["QMin"].GetValue<MenuSlider>().Value)
                    {
                        Q.Cast(FL.Position);
                    }
                }

                if (Menu["Sivir_JungleClear"]["Q"] && Me.ManaPercent >= Menu["Sivir_JungleClear"]["QMama"].GetValue<MenuSlider>().Value)
                {
                    var mobs = GetMobs(Me.Position, Q.Range);

                    if (mobs.Count() > 0)
                    {
                        foreach (var mob in mobs)
                        {
                            if (Q.IsReady())
                                Q.Cast(mob);
                        }
                    }
                }
            }

            if (Menu["Sivir_Auto"]["Q"] && Q.IsReady())
            {
                foreach (var e in GetEnemies(Q.Range))
                {
                    if (!CanMove(e) && e.Health <= Me.GetSpellDamage(e, SpellSlot.Q))
                    {
                        Q.Cast(e);
                    }
                }
            }

            if (Menu["Sivir_Auto"]["KillSteal"] && Q.IsReady())
            {
                foreach (var e in GetEnemies(Q.Range))
                {
                    if (e.Health <= Me.GetSpellDamage(e, SpellSlot.Q))
                    {
                        Q.Cast(e);
                    }
                }
            }

            if (R.IsReady())
            {
                if (Menu["Sivir_Auto"]["R"] && Me.CountEnemyHeroesInRange(850) >= Menu["Sivir_Auto"]["RMin"].GetValue<MenuSlider>().Value)
                {
                    R.Cast();
                }
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (Me.IsDead)
                return;

            if (Menu["Sivir_Draw"]["Q"] && Q.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.AliceBlue);
            }

            if (Menu["Sivir_Draw"]["Damage"])
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget() && !x.IsDead && !x.IsZombie))
                {
                    if (target != null)
                    {
                        HpBarDraw.Unit = target;

                        HpBarDraw.DrawDmg((float)GetDamage(target), new SharpDX.ColorBGRA(255, 200, 0, 170));
                    }
                }
            }
        }
    }
}
