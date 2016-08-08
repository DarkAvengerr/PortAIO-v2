using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Reflection;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PerplexedViktor
{
    class Program
    {
        static AIHeroClient Player = ObjectManager.Player;
        static System.Version Version = Assembly.GetExecutingAssembly().GetName().Version;

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Viktor")
                return;

            SpellManager.Initialize();
            Config.Initialize();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.LSIsRecalling() || args == null)
                return;

            AutoHarass();
            Killsteal();
            AutoFollow();
            AutoW();

            switch (Config.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
            }
        }

        static void Combo()
        {
            if(SpellManager.IgniteSlot != SpellSlot.Unknown && Player.GetSpell(SpellManager.IgniteSlot).LSIsReady())
            {
                var enemy = TargetSelector.GetTarget(600, TargetSelector.DamageType.True);
                if (enemy != null)
                    if (DamageCalc.GetTotalDamage(enemy) > enemy.Health)
                        Player.Spellbook.CastSpell(SpellManager.IgniteSlot, enemy);
            }
            if (SpellManager.Q.LSIsReady() && Config.ComboQ)
            {
                var enemy = TargetSelector.GetTarget(SpellManager.Q.Range, TargetSelector.DamageType.Magical);
                if (enemy != null)
                    if (Orbwalking.InAutoAttackRange(enemy))
                    {
                        SpellManager.CastSpell(SpellManager.Q, enemy);
                        return;
                    }
            }

            if(SpellManager.W.LSIsReady() && Config.ComboW)
            {
                var enemy = TargetSelector.GetTarget(SpellManager.W.Range, TargetSelector.DamageType.Magical);
                if (enemy != null)
                {
                    SpellManager.CastSpell(SpellManager.W, enemy, HitChance.VeryHigh);
                    return;
                }
            }

            if (SpellManager.E.LSIsReady() && Config.ComboE)
            {
                var enemy = TargetSelector.GetTarget(SpellManager.E.Range + SpellManager.E2.Range, TargetSelector.DamageType.Magical);
                if (enemy != null)
                    CastE(enemy);
            }

            if (SpellManager.R.LSIsReady() && Config.ComboR && !SpellManager.UltHasBeenCasted)
            {
                var enemy = TargetSelector.GetTarget(SpellManager.R.Range, TargetSelector.DamageType.Magical);
                if(enemy != null)
                {
                    if (DamageCalc.GetTotalDamage(enemy) > enemy.Health)
                        LeagueSharp.Common.Utility.DelayAction.Add(200, () => SpellManager.CastSpell(SpellManager.R, enemy));
                }
                var enemies = HeroManager.Enemies.Where(x => x.LSIsValidTarget(SpellManager.R.Range));
                foreach(var t in enemies)
                {
                    if (t.LSGetEnemiesInRange(SpellManager.R.Width).Count >= Config.ComboRHit)
                        SpellManager.CastSpell(SpellManager.R, t);
                }
            }
        }

        static void CastE(Obj_AI_Base enemy)
        {
            if (Player.ServerPosition.LSDistance(enemy.ServerPosition) < SpellManager.E2.Range)
            {
                SpellManager.E.UpdateSourcePosition(enemy.ServerPosition, enemy.ServerPosition);
                var prediction = SpellManager.E.GetPrediction(enemy, true);
                if (prediction.Hitchance >= HitChance.High)
                    SpellManager.CastSpell(SpellManager.E, enemy.ServerPosition, prediction.CastPosition);
            }
            else if (Player.ServerPosition.LSDistance(enemy.ServerPosition) < SpellManager.E.Range + SpellManager.E2.Range)
            {
                var castStartPos = Player.ServerPosition.LSExtend(enemy.ServerPosition, SpellManager.E2.Range);
                SpellManager.E.UpdateSourcePosition(castStartPos, castStartPos);
                var prediction = SpellManager.E.GetPrediction(enemy, true);
                if (prediction.Hitchance >= HitChance.High)
                    SpellManager.CastSpell(SpellManager.E, castStartPos, prediction.CastPosition);
            }
        }

        static void Harass()
        {
            if (Player.ManaPercent < Config.HarassMana)
                return;

            if (SpellManager.Q.LSIsReady() && Config.HarassQ)
            {
                var enemy = TargetSelector.GetTarget(SpellManager.Q.Range, TargetSelector.DamageType.Magical);
                if (enemy != null)
                    if (Orbwalking.InAutoAttackRange(enemy))
                    {
                        SpellManager.CastSpell(SpellManager.Q, enemy);
                        return;
                    }
            }

            if (SpellManager.E.LSIsReady() && Config.HarassE)
            {
                var enemy = TargetSelector.GetTarget(SpellManager.E.Range + SpellManager.E2.Range, TargetSelector.DamageType.Magical);
                if (enemy != null)
                    CastE(enemy);
            }
        }

        static void LaneClear()
        {
            if (Player.ManaPercent < Config.ClearMana)
                return;

            if (SpellManager.Q.LSIsReady() && Config.ClearQ)
            {
                var minion = MinionManager.GetMinions(SpellManager.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
                if (minion != null)
                    SpellManager.CastSpell(SpellManager.Q, minion);
            }

            if (SpellManager.E.LSIsReady() && Config.ClearE)
            {
                //Jungle
                var jungleCreep = MinionManager.GetMinions(SpellManager.E.Range + SpellManager.E2.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
                if (jungleCreep != null)
                {
                    if (jungleCreep.LSDistance(Player) > SpellManager.E2.Range)
                        SpellManager.CastSpell(SpellManager.E, Player.ServerPosition.LSExtend(jungleCreep.ServerPosition, SpellManager.E2.Range), jungleCreep.ServerPosition);
                    else
                        SpellManager.CastSpell(SpellManager.E, jungleCreep.ServerPosition.LSExtend(Player.ServerPosition, 50), jungleCreep.ServerPosition);
                }
                else //Lane
                {
                    foreach (var minion in MinionManager.GetMinions(Player.ServerPosition, SpellManager.E2.Range))
                    {
                        var location = MinionManager.GetBestLineFarmLocation(MinionManager.GetMinions(minion.ServerPosition, SpellManager.E.Range).Select(x => x.ServerPosition.LSTo2D()).ToList(), SpellManager.E.Width, SpellManager.E.Range);
                        if (location.MinionsHit >= Config.ClearEHit)
                            SpellManager.CastSpell(SpellManager.E, minion.ServerPosition, location.Position.To3D());
                    }
                }
            }
        }

        static void AutoHarass()
        {
            if (!Config.ToggleAuto.Active)
                return;

            if (SpellManager.E.LSIsReady() && Config.AutoE)
            {
                var enemy = TargetSelector.GetTarget(SpellManager.E.Range + SpellManager.E2.Range, TargetSelector.DamageType.Magical);
                if(enemy != null)
                {
                    if (Config.ShouldAuto(enemy.ChampionName) && (!enemy.LSUnderTurret(true) && !Player.LSUnderTurret(true) && Config.AutoTurret))
                        CastE(enemy);
                }
            }
        }

        static void Killsteal()
        {
            if (SpellManager.E.LSIsReady() && Config.KillstealE)
            {
                var enemy = HeroManager.Enemies.FirstOrDefault(x => x.LSIsValidTarget(SpellManager.E.Range + SpellManager.E2.Range) && x.Health < Player.LSGetSpellDamage(x, SpellSlot.E));
                if (enemy != null)
                    CastE(enemy);
            }
        }

        static void AutoFollow()
        {
            if (SpellManager.UltHasBeenCasted && Config.AutoFollow)
            {
                var t = HeroManager.Enemies.Where(x => x.LSIsValidTarget(SpellManager.R.Range) && DamageCalc.RemainingUltCanKill(x)).FirstOrDefault();
                if (t != null) //Killable first
                    LeagueSharp.Common.Utility.DelayAction.Add(100, () => SpellManager.CastSpell(SpellManager.R, t));
                else //Then target
                {
                    t = TargetSelector.GetTarget(SpellManager.R.Range, TargetSelector.DamageType.Magical);
                    if (t != null)
                        LeagueSharp.Common.Utility.DelayAction.Add(100, () => SpellManager.CastSpell(SpellManager.R, t));
                }
            }
        }

        static void AutoW()
        {
            if (SpellManager.W.LSIsReady() && Config.WCC)
            {
                var enemies = HeroManager.Enemies.Where(x => x.LSIsValidTarget(SpellManager.W.Range));
                foreach (var enemy in enemies)
                {
                    if (enemy.HasBuffOfType(BuffType.Charm) || enemy.HasBuffOfType(BuffType.Fear) || enemy.HasBuffOfType(BuffType.Knockup) || enemy.HasBuffOfType(BuffType.Slow)
                        || enemy.HasBuffOfType(BuffType.Snare) || enemy.HasBuffOfType(BuffType.Stun) || enemy.HasBuffOfType(BuffType.Suppression) || enemy.HasBuffOfType(BuffType.Taunt)
                        || enemy.IsStunned || enemy.IsRooted || enemy.LSIsRecalling())
                    {
                        SpellManager.CastSpell(SpellManager.W, enemy);
                        return;
                    }
                }
            }
        }
        static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (args.SData.Name == "ViktorChaosStorm")
            {
                SpellManager.UltCastedTime = Game.Time;
                Console.WriteLine("Casted Viktor ult!");
            }
        }


        static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                if (SpellManager.W.LSIsReady() && Config.InterruptW)
                {
                    if (Game.Time + SpellManager.W.Delay < args.EndTime)
                    {
                        SpellManager.CastSpell(SpellManager.W, sender);
                        return;
                    }
                }

                if (SpellManager.R.LSIsReady() && Config.InterruptR)
                {
                    if (Game.Time + SpellManager.R.Delay < args.EndTime)
                    {
                        SpellManager.CastSpell(SpellManager.R, sender);
                        return;
                    }
                }
            }
        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (SpellManager.W.LSIsReady() && Config.GapcloseW)
            {
                if (Player.LSDistance(gapcloser.Sender) < SpellManager.W.Range)
                    SpellManager.CastSpell(SpellManager.W, gapcloser.End);
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (Config.DrawQ)
                Render.Circle.DrawCircle(Player.Position, SpellManager.Q.Range, Config.Settings.Item("drawQ").GetValue<Circle>().Color);
            if (Config.DrawW)
                Render.Circle.DrawCircle(Player.Position, SpellManager.W.Range, Config.Settings.Item("drawW").GetValue<Circle>().Color);
            if (Config.DrawW)
                Render.Circle.DrawCircle(Player.Position, SpellManager.E.Range + SpellManager.E2.Range, Config.Settings.Item("drawE").GetValue<Circle>().Color);
            if (Config.DrawR)
                Render.Circle.DrawCircle(Player.Position, SpellManager.R.Range, Config.Settings.Item("drawR").GetValue<Circle>().Color);
        }


    }
}
