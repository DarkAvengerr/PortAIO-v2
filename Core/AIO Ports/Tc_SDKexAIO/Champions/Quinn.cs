using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Champions
{
    using LeagueSharp;
    using LeagueSharp.SDK;

    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.Data.Enumerations;

    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    using System;
    using System.Linq;

    using Common;
    using Config;
    using Core;

    internal static class Quinn
    {
        private static Spell Q, W, E, R;
        private static Menu Menu => PlaySharp.ChampionMenu;
        private static AIHeroClient Player => PlaySharp.Player;
        private static HpBarDraw HpBarDraw = new HpBarDraw();
        private static string StartR = "QuinnR";

        internal static void Init()
        {
            Q = new Spell(SpellSlot.Q, 1000).SetSkillshot(0.25f, 90f, 1500, true, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 2100);
            E = new Spell(SpellSlot.E, 700).SetTargetted(0.25f, 2000f);
            R = new Spell(SpellSlot.R, 550);


            var QMenu = Menu.Add(new Menu("Q", "Q.Set"));
            {
                QMenu.Add(new MenuBool("ComboQ", "Combo Q", true));
                QMenu.Add(new MenuBool("HarassQ", "Harass Q", true));
                QMenu.Add(new MenuSlider("HarassMana", "Harass Min Mana >= %", 50));
                QMenu.Add(new MenuBool("LaneClearQ", "LaneClear Q", true));
                QMenu.Add(new MenuSlider("LaneMana", "LaneClear Min Mana >= %", 50));
                QMenu.Add(new MenuBool("JungleQ", "Jungle Q", true));
                QMenu.Add(new MenuSlider("JungleMana", "Jungle Min Mana >= %", 50));
                QMenu.Add(new MenuBool("AutoHarass", "Auto Harass", true));
                QMenu.Add(new MenuSlider("AutoHarassMana", "AutoHarass Min Mana >= %", 50));
                QMenu.Add(new MenuBool("KillSteal", "KillSteal Q", true));
            }

            var WMenu = Menu.Add(new Menu("W", "W.Set"));
            {
                WMenu.Add(new MenuBool("ComboW", "Combo W", true));
            }

            var EMenu = Menu.Add(new Menu("E", "E.Set"));
            {
                EMenu.Add(new MenuBool("ComboE", "Combo E", true));
                EMenu.Add(new MenuBool("HarassE", "Harass E", true));
                EMenu.Add(new MenuSlider("HarassMana", "Harass Min Mana >= %", 50));
                EMenu.Add(new MenuBool("JungleE", "Jungle E", true));
                EMenu.Add(new MenuSlider("JungleMana", "Jungle Min Mana >= %", 50));
                EMenu.Add(new MenuBool("Gapcloser", "Ahti GapCloser", true));
                EMenu.Add(new MenuBool("Anti", "Anti Kahazi and Rengar !", true));
                EMenu.Add(new MenuBool("Interrupt", "Interrupt W", true));
            }

            var RMenu = Menu.Add(new Menu("R", "R.Set"));
            {
                RMenu.Add(new MenuBool("AutoR", "Auto R !", true));
            }

            var Draw = Menu.Add(new Menu("Draw", "Draw"));
            {
                Draw.Add(new MenuBool("Q", "Q Range"));
                Draw.Add(new MenuBool("W", "W Range"));
                Draw.Add(new MenuBool("E", "E Range"));
                Draw.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
            }

            PlaySharp.Write(GameObjects.Player.ChampionName + "OK! :)");

            Game.OnUpdate += OnUpdate;
            Events.OnInterruptableTarget += OnInterruptableTarget;
            Events.OnGapCloser += OnGapCloser;
            GameObject.OnCreate += OnCreate;
            Variables.Orbwalker.OnAction += OnAction;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            AutoLogic();

            KillStealLogic();

            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    ComboLogic();
                    break;
                case OrbwalkingMode.Hybrid:
                    HarassLogic();
                    break;
                case OrbwalkingMode.LaneClear:
                    LaneClearLogic();
                    break;
            }
        }

        private static void AutoLogic()
        {
            if (Menu["Q"]["AutoHarass"] && Player.ManaPercent > Menu["Q"]["AutoHarassMana"].GetValue<MenuSlider>().Value && Variables.Orbwalker.ActiveMode != OrbwalkingMode.Combo && Q.IsReady())
            {
                var enemy = Manager.GetTarget(Q.Range, Q.DamageType);
                if (enemy.IsValidTarget(Q.Range))
                {
                    Q.Cast(enemy);
                }
            }

            if (Menu["R"]["AutoR"] && R.IsReady())
            {
                if (Player.InFountain() && !Player.IsRecalling() && R.Instance.Name == StartR)
                {
                    R.Cast();
                }
            }
        }

        private static void KillStealLogic()
        {
            if (Menu["Q"]["KillSteal"] && Q.IsReady())
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy =>
                    !enemy.IsZombie && enemy.IsValidTarget(Q.Range) && Q.GetDamage(enemy) > enemy.Health && !enemy.IsDead))
                {
                    Q.Cast(enemy);
                }
            }
        }

        private static void ComboLogic()
        {
            if (Menu["Q"]["ComboQ"] && Q.IsReady())
            {
                var enemy = Manager.GetTarget(Q.Range, Q.DamageType);

                if (enemy.IsValidTarget(Q.Range))
                {
                    Q.Cast(enemy);
                }
            }

            if (Menu["E"]["ComboE"] && E.IsReady())
            {
                var enemy = Manager.GetTarget(E.Range, E.DamageType);

                if (enemy.IsValidTarget(E.Range))
                {
                    if (enemy.HasBuff("QuinnW"))
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, enemy);
                        return;
                    }
                    else if (!enemy.HasBuff("QuinnW"))
                    {
                        E.CastOnUnit(enemy);
                    }
                }
            }

            if (Menu["W"]["ComboW"] && W.IsReady())
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValidTarget(W.Range)))
                {
                    if (NavMesh.IsWallOfGrass(enemy.ServerPosition, 1))
                    {
                        if (Player.ServerPosition.Distance(enemy.ServerPosition) < 650)
                        {
                            W.Cast();
                        }
                    }
                }
            }
        }

        private static void HarassLogic()
        {
            var e = Manager.GetTarget(Q.Range, Q.DamageType);

            if (Menu["Q"]["HarassQ"] && Player.ManaPercent > Menu["Q"]["HarassMana"].GetValue<MenuSlider>().Value && Q.IsReady())
            {
                if (e.IsValidTarget(Q.Range))
                {
                    Q.Cast(e);
                }
            }

            if (Menu["E"]["HarassE"] && Player.ManaPercent > Menu["E"]["HarassMana"].GetValue<MenuSlider>().Value && E.IsReady())
            {
                if (e.IsValidTarget(E.Range))
                {
                    if (e.HasBuff("QuinnW"))
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, e);
                        return;
                    }
                    else if (!e.HasBuff("QuinnW"))
                    {
                        E.CastOnUnit(e);
                    }
                }
            }
        }

        private static void LaneClearLogic()
        {
            if (Player.ManaPercent > Menu["Q"]["LaneMana"].GetValue<MenuSlider>().Value)
            {
                var minion = Manager.GetMinions(Player.ServerPosition, 700);

                if (Menu["Q"]["LaneClearQ"] && Q.IsReady())
                {
                    if (minion.FirstOrDefault() != null)
                    {
                        if (minion.Count >= 2)
                        {
                            if (Q.IsInRange(minion.FirstOrDefault()))
                            {
                                Q.Cast(minion.FirstOrDefault());
                            }
                        }
                    }
                }
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            var khazix = GameObjects.EnemyHeroes.Find(h => h.ChampionName.Equals("Khazix"));
            var rengar = GameObjects.EnemyHeroes.Find(h => h.ChampionName.Equals("Rengar"));

            if (Menu["E"]["Anti"] && E.IsReady())
            {
                if (khazix != null || rengar != null)
                {
                    if (sender.Name == ("Rengar_LeapSound.troy") && sender.Position.Distance(Player.Position) < E.Range)
                        E.CastOnUnit(rengar);

                    if (sender.Name == ("Khazix_Base_E_Tar.troy") && sender.Position.Distance(Player.Position) < E.Range)
                        E.CastOnUnit(khazix);
                }
            }
        }

        private static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (args.IsDirectedToPlayer && E.IsReady() && Menu["E"]["Gapcloser"])
            {
                if (args.Sender.IsValidTarget(E.Range))
                {
                    E.CastOnUnit(args.Sender);
                }
            }
        }

        private static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel >= DangerLevel.High && Menu["E"]["Interrupt"])
            {
                if (args.Sender.IsValidTarget(E.Range))
                {
                    E.CastOnUnit(args.Sender);
                }
            }
        }

        private static void OnAction(object sender, OrbwalkingActionArgs args)
        {
            if (args.Type == OrbwalkingType.AfterAttack)
            {
                if (Manager.Combo)
                {
                    var e = Manager.GetTarget(E);

                    if (Menu["E"]["ComboE"] && E.IsReady())
                    {
                        if (!e.HasBuff("QuinnW"))
                        {
                            if (e.IsValidTarget(E.Range))
                            {
                                E.CastOnUnit(e);
                            }
                        }
                    }
                }

                if (Manager.LaneClear)
                {
                    var mobs = Manager.GetMobs(Player.Position, E.Range, true);

                    if (mobs.Count > 0)
                    {
                        var mob = mobs[0];

                        if (mob.HasBuff("QuinnW"))
                            return;

                        if (Q.IsReady() && Menu["Q"]["JungleQ"] && Player.ManaPercent > Menu["Q"]["JungleMana"].GetValue<MenuSlider>().Value)
                        {
                            Q.Cast(mob.ServerPosition);
                        }

                        if (E.IsReady() && Menu["E"]["JungleE"] && Player.ManaPercent > Menu["E"]["JungleMana"].GetValue<MenuSlider>().Value)
                        {
                            E.CastOnUnit(mob);
                        }
                    }
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Menu["Draw"]["Q"].GetValue<MenuBool>() && Q.IsReady())
                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.BlueViolet);

            if (Menu["Draw"]["W"].GetValue<MenuBool>() && W.IsReady())
                Render.Circle.DrawCircle(Player.Position, W.Range, System.Drawing.Color.BlueViolet);

            if (Menu["Draw"]["E"].GetValue<MenuBool>() && E.IsReady())
                Render.Circle.DrawCircle(Player.Position, E.Range, System.Drawing.Color.BlueViolet);

            if (Menu["Draw"]["DrawDamage"].GetValue<MenuBool>())
            {
                foreach (var e in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && e.IsValid && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = e;
                    HpBarDraw.DrawDmg(GetDamage(e), new SharpDX.ColorBGRA(255, 204, 0, 170));
                }
            }
        }

        private static float GetDamage(AIHeroClient target)
        {
            float Damage = 0f;

            // rengenrate
            Damage -= target.HPRegenRate;

            if (Q.IsReady())
            {
                Damage += Q.GetDamage(target);
            }

            if (W.IsReady())
            {
                Damage += W.GetDamage(target);
            }

            if (E.IsReady())
            {
                Damage += E.GetDamage(target);
            }

            if (R.IsReady())
            {
                Damage += R.GetDamage(target);
            }

            if (target.ChampionName == "Moredkaiser")
                Damage -= target.Mana;

            // exhaust
            if (Player.HasBuff("SummonerExhaust"))
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