using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Viktor.Extensions;
using SharpDX;
using Utilities = Viktor.Extensions.Utilities;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Viktor.Champions
{
    internal class Viktor : VSpells
    {
        public Viktor()
        {
            LoadViktor();
        }

        private static void LoadViktor()
        {
            Game.OnWndProc += OnWndProc;
            Menus.InitializeMenu();
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += DrawingOnDraw;
            AntiGapcloser.OnEnemyGapcloser += OnAntiGapCloser;
            Orbwalking.OnNonKillableMinion += Orbwalking_OnNonKillableMinion;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            Orbwalking.BeforeAttack += OrbwalkingOnBeforeAttack;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            switch (Menus.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    UberMode();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    OnHarass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    OnLaneClear();
                    OnJungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
            }

            if (spells[Spells.R].Instance.Name != "ViktorChaosStorm" && Utilities.IsEnabled("r.follow"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(spells[Spells.R].Range + 500)))
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(200, () => spells[Spells.R].Cast(enemy.ServerPosition));
                }
            }

            W_OnCCandImmobile();
        }

        private static void OnWndProc(WndEventArgs args)
        {
            if (args.Msg == 0x20a)
            {
                Menus.menuCfg.Item("e.clear").SetValue(!Menus.menuCfg.Item("e.clear").GetValue<bool>());
            }
        }

        private static void Orbwalking_OnNonKillableMinion(AttackableUnit minion)
        {
            CastQUnkillabeMinion();
        }

        private static void OrbwalkingOnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Utilities.IsEnabled("q.misc"))
            {
                if (args.Target.Type == GameObjectType.AIHeroClient && Utilities.Player.HasBuff("viktorpowertransferreturn"))
                {
                    args.Process = true;
                }
                else
                {
                    args.Process = false;
                }
            }
        }

        public static void OnCombo()
        {
            if (spells[Spells.Q].IsReady() && Utilities.IsEnabled("q.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(spells[Spells.Q].Range)))
                {
                    spells[Spells.Q].Cast(enemy);
                }
            }

            if (spells[Spells.W].IsReady() && Utilities.IsEnabled("w.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(spells[Spells.W].Range)))
                {
                    spells[Spells.W].PredictionCast(enemy, Utilities.TheHitChance("sel.hitchance.w"), "sel.predict.w", Menus.menuCfg);
                }
            }

            if (spells[Spells.E].IsReady() && Utilities.IsEnabled("e.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.IsValidTarget(ERange + spells[Spells.E].Range)))
                {
                    HitEnemy_E(enemy, Utilities.TheHitChance("sel.hitchance.e"));
                }
            }

            if (spells[Spells.R].IsReady() && Utilities.IsEnabled("r.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(spells[Spells.R].Range)
                     && spells[Spells.R].GetPrediction(x, true).Hitchance >= Utilities.TheHitChance("sel.hitchance.r")))
                {
                    if (enemy.Health < CalculateDamage(enemy) && enemy.HealthPercent > 10)
                    {
                        if (Utilities.IsEnabled("r.champ.whitelist" + enemy.ChampionName))
                        {
                            spells[Spells.R].Cast(enemy);
                        }
                    }
                    if (Utilities.Player.CountEnemiesInRange(spells[Spells.R].Range) >= Utilities.SliderValue("r.min.hit"))
                    {
                        if (Utilities.IsEnabled("r.champ.whitelist" + enemy.ChampionName))
                        {
                            spells[Spells.R].CastIfWillHit(enemy, Utilities.SliderValue("r.min.hit"));
                        }
                    }
                }
            }
        }

        public static void OnHarass()
        {
            if (spells[Spells.Q].IsReady() && Utilities.IsEnabled("q.harass"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Utilities.Player.AttackRange)))
                {
                    spells[Spells.Q].Cast(enemy);
                }
            }

            if (spells[Spells.W].IsReady() && Utilities.IsEnabled("w.harass"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(spells[Spells.W].Range)))
                {
                    spells[Spells.W].PredictionCast(enemy, Utilities.TheHitChance("sel.hitchance.w"), "sel.predict.w", Menus.menuCfg);
                }
            }

            if (spells[Spells.E].IsReady() && Utilities.IsEnabled("e.combo"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(ERange + spells[Spells.E].Range)))
                {
                    HitEnemy_E(enemy, Utilities.TheHitChance("sel.hitchance.e"));
                }
            }
        }

        public static void OnLaneClear()
        {
            if (spells[Spells.E].IsReady() && Utilities.IsEnabled("e.clear"))
            {
                PredictionCastEOnMinion();
            }
        }

        public static void OnJungleClear()
        {
            if (spells[Spells.Q].IsReady())
            {
                foreach (var minion in MinionManager.GetMinions(Utilities.Player.Position, Utilities.Player.AttackRange, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth))
                {
                    spells[Spells.Q].Cast(minion);
                }
            }
            if (spells[Spells.E].IsReady() && Utilities.IsEnabled("e.clear"))
            {
                PredictionCastEOnJungle();
            }
        }

        private static void OnAntiGapCloser(ActiveGapcloser gGapCloser)
        {
            switch (gGapCloser.Sender.ChampionName)
            {
                case "LeBlanc":
                    if (Utilities.IsEnabled("w.gapcloser")
                        && gGapCloser.Sender.IsEnemy
                        && gGapCloser.Sender.HasBuff("LeblancSlide")
                        && gGapCloser.Start.Distance(Utilities.Player.Position) < Orbwalking.GetRealAutoAttackRange(Utilities.Player)
                        && spells[Spells.W].IsReady())
                    {
                        spells[Spells.W].Cast(gGapCloser.Start);
                    }
                    break;
                default:
                    if (Utilities.IsEnabled("w.gapcloser")
                        && gGapCloser.Sender.IsEnemy
                        && gGapCloser.End.Distance(Utilities.Player.Position) < Orbwalking.GetRealAutoAttackRange(Utilities.Player)
                        && spells[Spells.W].IsReady())
                    {
                        spells[Spells.W].Cast(gGapCloser.End);
                    }
                    break;
            }
        }

        public static void W_OnCCandImmobile()
        {
            if (spells[Spells.W].IsReady() && Utilities.IsEnabled("w.misc"))
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(spells[Spells.W].Range) && x.IsEnemyImmobile()))
                {
                    spells[Spells.W].PredictionCast(enemy, Utilities.TheHitChance("sel.hitchance.w"), "sel.predict.w", Menus.menuCfg);
                }
            }
        }

        private static void CastQUnkillabeMinion()
        {
            if (!Utilities.IsEnabled("q.lasthit"))
                return;

            var MinionQ = MinionManager.GetMinions(spells[Spells.Q].Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);

            if (!Utilities.Player.CanAttack || Utilities.Player.Spellbook.IsAutoAttacking)
            {
                foreach (var minion in MinionQ)
                {
                    if (minion.Health <= spells[Spells.Q].GetDamage(minion) && spells[Spells.Q].IsReady())
                    {
                        spells[Spells.Q].Cast();
                        Utilities.Orbwalker.ForceTarget(minion);
                    }
                }
            }
        }

        private static void HitEnemy_E(Obj_AI_Base enemy, HitChance hHitChance)
        {

            if (Utilities.Player.ServerPosition.Distance(enemy.ServerPosition) < ERange)
            {
                spells[Spells.E].UpdateSourcePosition(enemy.ServerPosition, enemy.ServerPosition);
                var prediction = spells[Spells.E].GetPrediction(enemy, true);
                if (prediction.Hitchance >= hHitChance)
                {
                    spells[Spells.E].Cast(enemy.ServerPosition, prediction.CastPosition);
                }
            }
            else if (Utilities.Player.ServerPosition.Distance(enemy.ServerPosition) < spells[Spells.E].Range + ERange)
            {
                var castStartPos = Utilities.Player.ServerPosition.Extend(enemy.ServerPosition, ERange);
                spells[Spells.E].UpdateSourcePosition(castStartPos, castStartPos);
                var prediction = spells[Spells.E].GetPrediction(enemy, true);
                if (prediction.Hitchance >= hHitChance)
                {
                    spells[Spells.E].Cast(castStartPos, prediction.CastPosition);
                }
            }
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                if (Utilities.IsEnabled("r.interrupt") && sender.IsValidTarget(spells[Spells.R].Range) && spells[Spells.R].Instance.Name == "ViktorChaosStorm")
                {
                    spells[Spells.R].Cast(sender);
                }
            }
        }

        public static float CalculateDamage(Obj_AI_Base enemy)
        {
            var QAADamage = new double[] { 20, 40, 60, 80, 100 };
            double damage = 0;

            if (Utilities.IsEnabled("q.combo") && spells[Spells.Q].IsReady())
            {
                damage += spells[Spells.Q].GetDamage(enemy);
            }

            if (spells[Spells.Q].IsReady() || Utilities.Player.HasBuff("viktorpowertransferreturn") && Utilities.IsEnabled("q.combo"))
            {
                damage += (float)Utilities.Player.CalcDamage(enemy, Damage.DamageType.Magical,
                    QAADamage[Utilities.Player.Level >= 18 ? 18 - 1 : Utilities.Player.Level - 1] +
                    (Utilities.Player.TotalMagicalDamage * .5) /*+ Utilities.Player.TotalAttackDamage()*/);
            }

            if (spells[Spells.E].IsReady() && Utilities.IsEnabled("e.combo"))
            {
                if (Utilities.Player.HasBuff("viktoreaug") || Utilities.Player.HasBuff("viktorqeaug") || Utilities.Player.HasBuff("viktorqweaug"))
                {
                    damage += spells[Spells.E].GetDamage(enemy, 1);
                }
                else
                {
                    damage += spells[Spells.E].GetDamage(enemy, 0);
                }
            }

            if (spells[Spells.R].IsReady() && Utilities.IsEnabled("r.combo"))
            {
                damage += spells[Spells.R].GetDamage(enemy);
                damage += spells[Spells.R].GetDamage(enemy, 2);
            }

            // Ludens Echo
            if (Items.HasItem(3285))
                damage += Utilities.Player.CalcDamage(enemy, Damage.DamageType.Magical, 100 + Utilities.Player.FlatMagicDamageMod * 0.1);

            // Sheen
            if (Items.HasItem(3057))
                damage += Utilities.Player.CalcDamage(enemy, Damage.DamageType.Physical, 0.5 * Utilities.Player.BaseAttackDamage);

            //Lich Bane
            if (Items.HasItem(3100))
                damage += Utilities.Player.CalcDamage(enemy, Damage.DamageType.Magical, 0.5 * Utilities.Player.FlatMagicDamageMod + 0.75 * Utilities.Player.BaseAttackDamage);

            return (float)damage;
        }

        public static FarmLocation GetBestLaserFarmLocation(bool jungle)
        {
            var bestEndPos = new Vector2();
            var bestStartPos = new Vector2();
            var mMinionCount = 0;
            List<Obj_AI_Base> AllMinions;

            if (!jungle)
            {
                AllMinions = MinionManager.GetMinions(EMaxRange);
            }
            else
            {
                AllMinions = MinionManager.GetMinions(EMaxRange, MinionTypes.All, MinionTeam.Neutral);
            }

            var mMinionsList = (from minions in AllMinions select minions.Position.To2D()).ToList<Vector2>();
            var possiblePosition = new List<Vector2>();
            possiblePosition.AddRange(mMinionsList);
            var Max = possiblePosition.Count;

            for (var i = 0; i < Max; i++)
            {
                for (var j = 0; j < Max; j++)
                {
                    if (possiblePosition[j] != possiblePosition[i])
                    {
                        possiblePosition.Add((possiblePosition[j] + possiblePosition[i]) / 2);
                    }
                }

            }

            foreach (var StartPositionMinion in AllMinions.Where(m => Utilities.Player.Distance(m) < ERange))
            {
                var StartPos = StartPositionMinion.Position.To2D();

                foreach (var Pos in possiblePosition)
                {
                    if (Pos.Distance(StartPos, true) <= LenghtE * LenghtE)
                    {
                        var EndPos = StartPos + LenghtE * (Pos - StartPos).Normalized();
                        var count =
                            mMinionsList.Count(Pos2 => Pos2.Distance(StartPos, EndPos, true, true) <= 140 * 140);

                        if (count >= mMinionCount)
                        {
                            bestEndPos = EndPos;
                            mMinionCount = count;
                            bestStartPos = StartPos;
                        }
                    }
                }
            }

            if ((!jungle && Utilities.SliderValue("e.clear.minhit") < mMinionCount) || (jungle && mMinionCount > 0))
            {
                return new FarmLocation(bestStartPos, bestEndPos, mMinionCount);
            }
            else
            {
                return new FarmLocation(bestStartPos, bestEndPos, 0);
            }
        } // Algorithm by Trus. Congratz bro

        public struct FarmLocation
        {
            /// <summary>
            /// The minions hit
            /// </summary>
            public int MinionsHit;

            /// <summary>
            /// The start position
            /// </summary>
            public Vector2 Position1;


            /// <summary>
            /// The end position
            /// </summary>
            public Vector2 Position2;

            /// <summary>
            /// Initializes a new instance of the <see cref="FarmLocation"/> struct.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="minionsHit">The minions hit.</param>

            public FarmLocation(Vector2 startpos, Vector2 endpos, int minionsHit)
            {
                Position1 = startpos;
                Position2 = endpos;
                MinionsHit = minionsHit;
            }
        } // Algorithm by Trus. Congratz bro

        private static bool PredictionCastEOnMinion()
        {
            var farmLocation = GetBestLaserFarmLocation(false);
            if (farmLocation.MinionsHit > 0)
            {
                CastE(farmLocation.Position1, farmLocation.Position2);
                return true;
            }

            return false;
        } // Algorithm by Trus. Congratz bro

        private static bool PredictionCastEOnJungle()
        {
            var farmLocation = GetBestLaserFarmLocation(true);

            if (farmLocation.MinionsHit > 0)
            {
                CastE(farmLocation.Position1, farmLocation.Position2);
                return true;
            }

            return false;
        } // Algorithm by Trus. Congratz bro

        private static void CastE(Vector3 source, Vector3 destination)
        {
            spells[Spells.E].Cast(source, destination);
        } 

        private static void CastE(Vector2 source, Vector2 destination)
        {
            spells[Spells.E].Cast(source, destination);
        }

        private static void UberMode()
        {
            Orbwalking.MoveTo(Game.CursorPos);
            if (!spells[Spells.Q].IsReady() 
                || !(Utilities.Player.HasBuff("viktorqaug") 
                || Utilities.Player.HasBuff("viktorqeaug") 
                || Utilities.Player.HasBuff("viktorqwaug") 
                || Utilities.Player.HasBuff("viktorqweaug")))
                return;

            foreach (var enemy in HeroManager.Enemies.Where(x => !x.IsValidTarget(spells[Spells.Q].Range) && Utilities.IsEnabled("q.uber")))
            {
                var mMinion = MinionManager.GetMinions(spells[Spells.Q].Range, MinionTypes.All, MinionTeam.NotAlly).MinOrDefault(m => Utilities.Player.Distance(m));

                if (mMinion.IsValidTarget(spells[Spells.Q].Range))
                {
                    spells[Spells.Q].Cast(mMinion);
                }
            }
        }

        private static void DrawingOnDraw(EventArgs args)
        {
            if (Utilities.Player.IsDead)
            {
                return;
            }
            if (Utilities.IsEnabled("draw.q") && spells[Spells.Q].IsReady())
            {
                Render.Circle.DrawCircle(Utilities.Player.Position, spells[Spells.Q].Range, System.Drawing.Color.Crimson);
            }
            if (Utilities.IsEnabled("draw.w") && spells[Spells.W].IsReady())
            {
                Render.Circle.DrawCircle(Utilities.Player.Position, spells[Spells.W].Range, System.Drawing.Color.Aqua);
            }
            if (Utilities.IsEnabled("draw.e") && spells[Spells.E].IsReady())
            {
                Render.Circle.DrawCircle(new Vector3(Utilities.Player.Position.X, Utilities.Player.Position.Y, Utilities.Player.Position.Z),
                    spells[Spells.E].Range + ERange, System.Drawing.Color.White, 5);
            }
            if (Utilities.IsEnabled("draw.r") && spells[Spells.R].IsReady())
            {
                Render.Circle.DrawCircle(Utilities.Player.Position, spells[Spells.R].Range, System.Drawing.Color.Tomato);
            }
            if (Utilities.IsEnabled("e.clear"))
            {
                var drawPos = Drawing.WorldToScreen(Utilities.Player.Position);
                var textSize = Drawing.GetTextEntent(("Spell Farm: ON"), 15);
                Drawing.DrawText(drawPos.X - textSize.Width - 70f, drawPos.Y, System.Drawing.Color.Chartreuse,
                    "Spell Farm: ON");
            }
            else
            {
                var drawPos2 = Drawing.WorldToScreen(Utilities.Player.Position);
                var textSize2 = Drawing.GetTextEntent(("Spell Farm: OFF"), 15);
                Drawing.DrawText(drawPos2.X - textSize2.Width - 70f, drawPos2.Y, System.Drawing.Color.Red,
                    "Spell Farm: OFF");
            }
        }
    }
}
