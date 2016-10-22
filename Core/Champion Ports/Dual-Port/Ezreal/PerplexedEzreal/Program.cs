using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PerplexedEzreal
{
    class Program
    {
        static AIHeroClient Player = ObjectManager.Player;
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Ezreal")
                return;
            SpellManager.Initialize();
            Config.Initialize();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Config.GapcloseE)
                return;
            var awayPosition = gapcloser.End.Extend(ObjectManager.Player.ServerPosition, ObjectManager.Player.Distance(gapcloser.End) + SpellManager.E.Range);
            SpellManager.E.Cast(awayPosition);
        }

        static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if ((Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit || Config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear) && Config.LastHitQ)
            {
                foreach (
                    var minionDie in
                        MinionManager.GetMinions(SpellManager.Q.Range)
                            .Where(
                                minion =>
                                    target.NetworkId != minion.NetworkId && minion.IsEnemy &&
                                    HealthPrediction.GetHealthPrediction(minion,
                                        (int) ((Player.AttackDelay*1000)*2.65f + Game.Ping/2), 0) <= 0 &&
                                    SpellManager.Q.GetDamage(minion) >= minion.Health && SpellManager.Q.IsReady()))
                    SpellManager.CastSpell(SpellManager.Q, minionDie, HitChance.High);
            }
        }

        static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling() || args == null)
                return;

            if (Config.UltLowest.Active)
                UltLowestTarget();

            AutoHarass();
            Killsteal();

            switch (Config.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
            }
        }

        static void Combo()
        {
            if (SpellManager.Q.IsReady() && Config.ComboQ)
            {
                var t = TargetSelector.GetTarget(SpellManager.Q.Range, TargetSelector.DamageType.Physical);
                if (t != null)
                    SpellManager.CastSpell(SpellManager.Q, t, HitChance.High);
            }

            if (SpellManager.W.IsReady() && Config.ComboW)
            {
                var t = TargetSelector.GetTarget(SpellManager.W.Range, TargetSelector.DamageType.Magical);
                if (t != null)
                    SpellManager.CastSpell(SpellManager.W, t, HitChance.High);
            }

            if (SpellManager.R.IsReady() && Config.ComboR)
            {
                foreach (var t in HeroManager.Enemies.Where(hero => hero.IsValidTarget(Config.UltMaxRange)))
                {
                    SpellManager.R.CastIfWillHit(t, Config.ComboRHit);
                    return;
                }
            }

            if (SpellManager.R.IsReady() && Config.ComboR)
            {
                var t = TargetSelector.GetTarget(Config.UltMaxRange, TargetSelector.DamageType.Physical);
                if (t != null)
                {
                    if (Player.Distance(t) > Config.UltMinRange && t.Health < DamageCalc.GetRDamage(t))
                        SpellManager.CastSpell(SpellManager.R, t, HitChance.High);
                }
            }
        }

        static void Harass()
        {
            if (Player.ManaPercent < Config.HarassMana)
                return;

            if (SpellManager.Q.IsReady() && Config.HarassQ)
            {
                var t = TargetSelector.GetTarget(SpellManager.Q.Range, TargetSelector.DamageType.Physical);
                if (t != null)
                    SpellManager.CastSpell(SpellManager.Q, t, HitChance.High);
            }

            if (SpellManager.W.IsReady() && Config.HarassW)
            {
                var t = TargetSelector.GetTarget(SpellManager.W.Range, TargetSelector.DamageType.Magical);
                if (t != null)
                    SpellManager.CastSpell(SpellManager.W, t, HitChance.High);
            }
        }

        static void AutoHarass()
        {
            if (!Config.ToggleAuto.Active)
                return;
            if (SpellManager.Q.IsReady() && Config.AutoQ)
            {
                var t =
                    HeroManager.Enemies.Where(hero => hero.IsValidTarget(SpellManager.Q.Range))
                        .FirstOrDefault(hero => Config.ShouldAuto(hero.ChampionName));
                if (t != null)
                    SpellManager.CastSpell(SpellManager.Q, t, HitChance.High);
            }
            if (SpellManager.W.IsReady() && Config.AutoW)
            {
                var t =
                    HeroManager.Enemies.Where(hero => hero.IsValidTarget(SpellManager.W.Range))
                        .FirstOrDefault(hero => Config.ShouldAuto(hero.ChampionName));
                if (t != null)
                    SpellManager.CastSpell(SpellManager.W, t, HitChance.High);
            }
        }

        static void LastHit()
        {
            if (SpellManager.Q.IsReady() && Config.LastHitQ)
            {
                var minion =
                    MinionManager.GetMinions(SpellManager.Q.Range, MinionTypes.All, MinionTeam.Enemy)
                        .FirstOrDefault(
                            min =>
                                min.IsValidTarget(SpellManager.Q.Range) &&
                                Player.GetSpellDamage(min, SpellSlot.Q) >= min.Health);
                if (minion != null)
                    SpellManager.CastSpell(SpellManager.Q, minion, HitChance.High);
            }
        }

        static void LaneClear()
        {
            if (Player.ManaPercent < Config.ClearMana)
                return;

            if (SpellManager.Q.IsReady() && Config.LaneClearQ)
            {
                var minion =
                    MinionManager.GetMinions(SpellManager.Q.Range, MinionTypes.All, MinionTeam.Enemy,
                        MinionOrderTypes.MaxHealth)
                        .OrderBy(min => min.Distance(Player))
                        .FirstOrDefault(min => min.IsValidTarget(SpellManager.Q.Range));
                if (minion != null)
                    SpellManager.CastSpell(SpellManager.Q, minion, HitChance.High);
            }
        }

        static void UltLowestTarget()
        {
            if (SpellManager.R.IsReady())
            {
                var t =
                    HeroManager.Enemies.Where(hero => hero.IsValidTarget(Config.UltMaxRange))
                        .OrderBy(hero => hero.Health)
                        .FirstOrDefault();
                if (t != null)
                    SpellManager.CastSpell(SpellManager.R, t, HitChance.High);
            }
        }

        static void Killsteal()
        {
            if (SpellManager.R.IsReady() && Config.Killsteal)
            {
                var t =
                    HeroManager.Enemies.FirstOrDefault(
                        hero =>
                            hero.IsValidTarget(Config.UltMaxRange) && Player.Distance(hero) > Config.UltMinRange &&
                            hero.Health < DamageCalc.GetRDamage(hero));
                if (t != null)
                    SpellManager.CastSpell(SpellManager.R, t, HitChance.High);
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (Config.DrawQ)
                Render.Circle.DrawCircle(Player.Position, SpellManager.Q.Range, Config.Settings.Item("drawQ").GetValue<Circle>().Color);
            if (Config.DrawW)
                Render.Circle.DrawCircle(Player.Position, SpellManager.W.Range, Config.Settings.Item("drawW").GetValue<Circle>().Color);
            if (Config.DrawMinR)
                Render.Circle.DrawCircle(Player.Position, Config.UltMinRange, Config.Settings.Item("drawMinR").GetValue<Circle>().Color);
            if (Config.DrawMaxR)
                Render.Circle.DrawCircle(Player.Position, Config.UltMaxRange, Config.Settings.Item("drawMaxR").GetValue<Circle>().Color);
        }
    }
}
