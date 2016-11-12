using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Champions
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    using System;
    using System.Linq;

    using Color = System.Drawing.Color;

    using Common;
    using static Common.Manager;

    internal static class Ezreal
    {
        private static Spell Q, W, E, R;
        private static Menu Menu => PlaySharp.ChampionMenu;
        private static AIHeroClient Player => PlaySharp.Player;

        private static HpBarDraw HpBarDraw = new HpBarDraw();

        internal static void Init()
        {
            Q = new Spell(SpellSlot.Q, 1150f).SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 950f).SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);
            E = new Spell(SpellSlot.E, 475f);
            R = new Spell(SpellSlot.R, 1000f).SetSkillshot(1.1f, 160f, 2000f, false, SkillshotType.SkillshotLine);


            var QMenu = Menu.Add(new Menu("Q", "Q.Set"));
            {
                QMenu.Add(new MenuBool("Combo", "Combo!", true));
                QMenu.Add(new MenuBool("Harass", "Harass!", true));
                QMenu.Add(new MenuSlider("HarassMana", "Harass Min Mana >= %", 50));
                QMenu.Add(new MenuBool("LaneClear", "LaneClear!", true));
                QMenu.Add(new MenuSlider("LaneMana", "LaneClear Min Mana >= %", 50));
                QMenu.Add(new MenuBool("Jungle", "Jungle!", true));
                QMenu.Add(new MenuSlider("JungleMana", "Jungle Min Mana >= %", 50));
                QMenu.Add(new MenuBool("LastHit", "LastHit!", true));
                QMenu.Add(new MenuSlider("LastHitMana", "LastHit Min Mana >= %", 50));
                QMenu.Add(new MenuBool("KillSteal", "KillSteal!", true));
                var QList = QMenu.Add(new Menu("QList", "Q List"));
                {
                    if (GameObjects.EnemyHeroes.Any())
                    {
                        GameObjects.EnemyHeroes.ForEach(i => QList.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, !AutoEnableList.Contains(i.ChampionName))));
                    }
                }
            }

            var WMenu = Menu.Add(new Menu("W", "W.Set"));
            {
                WMenu.Add(new MenuBool("Combo", "Combo!", true));
                WMenu.Add(new MenuBool("Harass", "Harass!", true));
                WMenu.Add(new MenuSlider("HarassMana", "Harass Min Mana >= %", 50));
                WMenu.Add(new MenuBool("LaneClear", "LaneClear! (Push Tower)", true));
                WMenu.Add(new MenuBool("KillSteal", "KillSteal!", true));
                var WList = WMenu.Add(new Menu("WList", "W List"));
                {
                    if (GameObjects.EnemyHeroes.Any())
                    {
                        GameObjects.EnemyHeroes.ForEach(i => WList.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, !AutoEnableList.Contains(i.ChampionName))));
                    }
                }
            }

            var EMenu = Menu.Add(new Menu("E", "E.Set"));
            {
                EMenu.Add(new MenuBool("Gapcloser", "GapCloser!", true));
                EMenu.Add(new MenuBool("AntiMelee", "Anti Melee!", true));
                EMenu.Add(new MenuBool("AutoEGrAD", "Auto E Gap AD", true));
            }

            var RMenu = Menu.Add(new Menu("R", "R.Set"));
            {
                RMenu.Add(new MenuBool("Auto", "Auto R!", true));
                RMenu.Add(new MenuBool("Safe", "Safe R!", true));
                RMenu.Add(new MenuSlider("RMin", "Min Hit Counts >= ", 2, 1, 5));
                RMenu.Add(new MenuKeyBind("RKey", "R Key", System.Windows.Forms.Keys.T, KeyBindType.Press));
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
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
            Variables.Orbwalker.OnAction += OnAction;
            Events.OnGapCloser += OnGapCloser;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnEndScene(EventArgs args)
        {
            if (!Menu["Draw"]["DrawDamage"].GetValue<MenuBool>())
                return;

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget(1500) && !e.IsDead && e.IsVisible))
            {
                HpBarDraw.Unit = enemy;
                HpBarDraw.DrawDmg(GetDamage(enemy), SharpDX.Color.CadetBlue);
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Menu["E"]["AntiMelee"].GetValue<MenuBool>() && E.IsReady())
            {
                if (sender != null && sender.IsEnemy && args.Target != null && args.Target.IsMe)
                {
                    if (sender.Type == Player.Type && sender.IsMelee)
                    {
                        E.Cast(Player.Position.Extend(sender.Position, -E.Range));
                    }
                }
            }
        }

        private static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (Menu["E"]["Gapcloser"].GetValue<MenuBool>() && E.IsReady())
            {
                if (args.IsDirectedToPlayer && args.End.DistanceToPlayer() <= 200)
                {
                    E.Cast(Player.Position.Extend(args.Sender.Position, -E.Range));
                }
            }
        }

        private static void OnAction(object sender, OrbwalkingActionArgs args)
        {
            if (args.Type == OrbwalkingType.AfterAttack)
            {
                if (args.Target is AIHeroClient)
                {
                    if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
                    {
                        if (Menu["Q"]["Combo"].GetValue<MenuBool>() && Q.IsReady())
                        {
                            var target = args.Target as AIHeroClient;

                            if (target.IsValidTarget(Q.Range))
                            {
                                Q.Cast(target);
                            }
                        }

                        if (Menu["W"]["Combo"].GetValue<MenuBool>() && W.IsReady())
                        {
                            var target = args.Target as AIHeroClient;

                            if (target.IsValidTarget(W.Range))
                            {
                                W.Cast(target);
                            }
                        }
                    }

                    if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid && Menu["Q"]["Harass"].GetValue<MenuBool>() && !Player.IsUnderEnemyTurret())
                    {
                        var target = args.Target as AIHeroClient;

                        if (target.IsValidTarget(Q.Range) && Q.IsReady() && Player.ManaPercent >= Menu["Q"]["HarassMana"].GetValue<MenuSlider>().Value)
                        {
                            Q.Cast(target);
                        }
                    }
                }
            }

            if (args.Type == OrbwalkingType.BeforeAttack)
            {
                if (Menu["W"]["LaneClear"].GetValue<MenuBool>() && W.IsReady() && Player.CountEnemyHeroesInRange(850) < 1)
                {
                    if (args.Target is Obj_AI_Turret)
                    {
                        var target = args.Target as Obj_AI_Turret;

                        if (W.IsReady() && Player.CountAllyHeroesInRange(W.Range) >= 1)
                        {
                            W.Cast(GameObjects.AllyHeroes.Find(x => W.IsInRange(x)));
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
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.BlueViolet);

            if (Menu["Draw"]["W"].GetValue<MenuBool>() && W.IsReady())
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.BlueViolet);

            if (Menu["Draw"]["E"].GetValue<MenuBool>() && E.IsReady())
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.BlueViolet);
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            ComboLogic(args);

            HarassLogic(args);

            LaneLogic(args);

            JungleLogic(args);

            LastHit(args);

            AutoRLogic(args);

            RKey(args);

            KillSteal(args);
        }

        private static void ComboLogic(EventArgs args)
        {
            if (Combo)
            {
                if (Menu["Q"]["Combo"].GetValue<MenuBool>() && Q.IsReady())
                {
                    var QTarget = GetTarget(Q);

                    if (QTarget != null && QTarget.IsValidTarget(Q.Range) && QTarget.IsHPBarRendered)
                    {
                        Q.Cast(QTarget);
                    }
                }

                if (Menu["W"]["Combo"].GetValue<MenuBool>() && W.IsReady())
                {
                    var WTarget = GetTarget(W);

                    if (WTarget != null && WTarget.IsValidTarget(W.Range) && WTarget.IsHPBarRendered)
                    {
                        W.Cast(WTarget);
                    }
                }

                if (Menu["R"]["Auto"].GetValue<MenuBool>() && W.IsReady())
                {
                    var RTarget = GetTarget(R.Range);

                    if (RTarget != null)
                    {
                        if (RTarget.Health + RTarget.HPRegenRate * 2 <= R.GetDamage(RTarget) && RTarget.IsValidTarget(1500))
                        {
                            R.Cast(RTarget);
                        }
                        else if (RTarget.IsValidTarget(Q.Range + E.Range - 200) && RTarget.Health + RTarget.MagicShield + RTarget.HPRegenRate * 2 <= R.GetDamage(RTarget) + Q.GetDamage(RTarget) + W.GetDamage(RTarget) && Q.IsReady() && W.IsReady() && RTarget.CountAllyHeroesInRange(Q.Range + E.Range - 200) <= 1)
                        {
                            R.Cast(RTarget);
                        }
                        else if (RTarget.IsValidTarget())
                        {
                            R.CastIfWillHit(RTarget, Menu["R"]["RMin"].GetValue<MenuSlider>().Value);
                        }
                    }
                }
            }
        }

        private static void HarassLogic(EventArgs args)
        {
            if (Harass)
            {
                if (Menu["Q"]["Harass"].GetValue<MenuBool>() && Q.IsReady())
                {
                    foreach (var e in GetEnemies(Q.Range))
                    {
                        if (Menu["Q"]["QList"][e.ChampionName.ToLower()].GetValue<MenuBool>() &&
                            !Player.IsUnderEnemyTurret() && Player.ManaPercent >= Menu["Q"]["HarassMana"].GetValue<MenuSlider>().Value)
                        {
                            Q.Cast(e);
                        }
                    }
                }

                if (Menu["W"]["Harass"].GetValue<MenuBool>() && W.IsReady())
                {
                    foreach (var e in GetEnemies(W.Range))
                    {
                        if (Menu["W"]["WList"][e.ChampionName.ToLower()].GetValue<MenuBool>() && !Player.IsUnderEnemyTurret()
                            && Player.ManaPercent >= Menu["W"]["HarassMana"].GetValue<MenuSlider>().Value)
                        {
                            W.Cast(e);
                        }
                    }
                }
            }
        }

        private static void LaneLogic(EventArgs args)
        {
            if (LaneClear)
            {
                if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.LaneClear && Player.ManaPercent >= Menu["Q"]["LaneMana"].GetValue<MenuSlider>().Value)
                {
                    if (Q.IsReady() && !Player.IsUnderEnemyTurret())
                    {
                        var Minions = GetMinions(Player.Position, Q.Range);

                        if (Minions.Count() > 0)
                        {
                            foreach (var min in Minions.Where(x => x.Health < Q.GetDamage(x)))
                            {
                                if (min != null)
                                {
                                    if (InAutoAttackRange(min) && min.Health > Player.GetAutoAttackDamage(min))
                                    {
                                        Q.Cast(min);
                                    }
                                    else if (!InAutoAttackRange(min) && min.IsValidTarget(Q.Range))
                                    {
                                        Q.Cast(min);
                                    }
                                }
                                else if (min == null)
                                {
                                    Q.Cast(Minions.FirstOrDefault());
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void JungleLogic(EventArgs args)
        {
            if (LaneClear)
            {
                if (Menu["Q"]["Jungle"].GetValue<MenuBool>() && Player.ManaPercent >= Menu["Q"]["JungleMana"].GetValue<MenuSlider>().Value)
                {
                    var Mobs = GetMobs(Player.Position, Q.Range);

                    if (Mobs.Count() > 0)
                    {
                        foreach (var mob in Mobs)
                        {
                            if (Q.IsReady())
                            {
                                Q.Cast(mob);
                            }
                        }
                    }
                }
            }
        }

        private static void AutoRLogic(EventArgs args)
        {
            if (R.IsReady() && Menu["R"]["Auto"].GetValue<MenuBool>())
            {
                if (Menu["R"]["Safe"].GetValue<MenuBool>() && Player.CountEnemyHeroesInRange(900) != 0)
                {
                    return;
                }

                foreach (var t in GetEnemies(R.Range))
                {
                    var RDMG = R.GetDamage(t);
                    var boolean = t.Health + t.MagicShield + t.HPRegenRate * 2 < RDMG;

                    if (boolean && t.HasBuff("recall"))
                        R.Cast(t);

                    if (!CanMove(t) && t.Health < RDMG + Q.GetDamage(t) * 3 && t.IsValidTarget(Q.Range + E.Range - 200))
                    {
                        if (t.CountAllyHeroesInRange(400) >= 1)
                        {
                            R.CastIfWillHit(t, Menu["R"]["RMin"].GetValue<MenuSlider>().Value);
                        }
                        else
                        {
                            R.Cast(t);
                        }
                    }
                }
            }
        }

        private static void RKey(EventArgs args)
        {
            if (Menu["R"]["RKey"].GetValue<MenuKeyBind>().Active && R.IsReady())
            {
                var t = GetTarget(2000, DamageType.Magical);

                if (t != null)
                {
                    if (t.Health + t.HPRegenRate * 2 < R.GetDamage(t))
                    {
                        R.Cast(t);
                    }
                    else
                    {
                        R.CastIfWillHit(t, Menu["R"]["RMin"].GetValue<MenuSlider>().Value);
                    }
                }
            }
        }

        private static void LastHit(EventArgs args)
        {
            if ((Harass || Hit) && !Player.IsUnderEnemyTurret())
            {
                var targets = GetMinions(Player.Position, Q.Range);

                foreach (var target in targets)
                {
                    if (target != null)
                    {
                        if (target.IsValidTarget(Q.Range) && !InAutoAttackRange(target) && Q.IsReady() && target.Health < Q.GetDamage(target) &&
                            Player.ManaPercent >= Menu["Q"]["LastHitMana"].GetValue<MenuSlider>().Value)
                        {
                            Q.Cast(target);
                        }
                    }
                }
            }
        }

        private static void KillSteal(EventArgs args)
        {
            foreach (var e in GetEnemies(Q.Range))
            {
                if (Q.GetDamage(e) > e.Health && Menu["Q"]["KillSteal"].GetValue<MenuBool>())
                {
                    if (e.IsValidTarget(Q.Range))
                        Q.Cast(e);
                }

                if (W.GetDamage(e) > e.Health && Menu["W"]["KillSteal"].GetValue<MenuBool>())
                {
                    if (e.IsValidTarget(W.Range))
                        W.Cast(e);
                }

                if (e.IsValidTarget(W.Range) && Menu["W"]["KillSteal"].GetValue<MenuBool>() && Menu["Q"]["KillSteal"].GetValue<MenuBool>())
                {
                    if (e.Health < Q.GetDamage(e) + W.GetDamage(e))
                    {
                        W.Cast(e);
                        Q.Cast(e);
                    }
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
