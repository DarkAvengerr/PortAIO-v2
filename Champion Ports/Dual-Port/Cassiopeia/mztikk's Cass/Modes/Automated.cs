using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksCassiopeia.Modes
{
    using System.Linq;

    using mztikksCassiopeia.Extensions;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikksCassiopeia.Config;

    internal static class Automated
    {
        #region Public Methods and Operators

        public static void EKillSteal()
        {
            if (Spells.E.IsReady())
            {
                var entKs =
                    HeroManager.Enemies.FirstOrDefault(
                        h => h.IsValidTarget(Spells.E.Range) && h.Health < Spells.GetEDamage(h));
                if (entKs != null)
                {
                    Spells.E.Cast(entKs);
                }
            }
        }

        public static void TearStack()
        {
            if (!Spells.Q.IsReady() || ObjectManager.Player.IsRecalling())
            {
                return;
            }

            var enemyTarget = TargetSelector.GetTarget(Spells.Q.Range + 200, TargetSelector.DamageType.Magical);
            if (enemyTarget != null)
            {
                Spells.Q.Cast(enemyTarget);
            }
            else
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range);
                var qFarmLoc = MinionManager.GetBestCircularFarmLocation(
                    minions.Select(x => x.Position.To2D()).ToList(), 
                    Spells.Q.Width, 
                    Spells.Q.Range);
                if (qFarmLoc.MinionsHit >= 1)
                {
                    Spells.Q.Cast(qFarmLoc.Position.To3D());
                }
                else
                {
                    var rndPlayerPos = ObjectManager.Player.Position;
                    rndPlayerPos.X += Mainframe.RDelay.Next(-200, 200);
                    rndPlayerPos.Y += Mainframe.RDelay.Next(-200, 200);
                    LeagueSharp.Common.Utility.DelayAction.Add(Mainframe.RDelay.Next(30, 70), () => Spells.Q.Cast(rndPlayerPos));
                }
            }
        }

        #endregion

        #region Methods

        internal static void AutoClearE()
        {
            if (!Spells.E.IsReady() || ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            var minionToE =
                MinionManager.GetMinions(
                    ObjectManager.Player.Position, 
                    Spells.E.Range, 
                    MinionTypes.All, 
                    MinionTeam.Enemy)
                    .FirstOrDefault(
                        x =>
                        x.IsValidTarget(Spells.E.Range) && Spells.GetEDamage(x) >= x.Health
                        && x.HasBuffOfType(BuffType.Poison));
            if (minionToE != null)
            {
                Spells.E.Cast(minionToE);
            }
        }

        internal static void AutoE()
        {
            if (!Spells.E.IsReady() || ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            if (Config.IsChecked("dontAutoHarassInBush"))
            {
                var bush =
                    ObjectManager.Get<GrassObject>()
                        .OrderBy(br => br.Position.Distance(ObjectManager.Player.Position))
                        .FirstOrDefault();
                if (bush != null && ObjectManager.Player.Position.Distance(bush.Position) <= bush.BoundingRadius)
                {
                    return;
                }
            }

            var target = TargetSelector.GetTarget(Spells.E.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget(Spells.E.Range))
            {
                return;
            }

            if (Config.IsChecked("dontAutoHarassTower") && target.Position.UnderEnemyTurret()
                && ObjectManager.Player.Position.UnderEnemyTurret())
            {
                return;
            }

            if (Spells.E.IsReady() && target.IsValidTarget(Spells.E.Range)
                && (!Config.IsChecked("autoHarassEonP") || target.HasBuffOfType(BuffType.Poison)))
            {
                if (Config.IsChecked("humanEInAutoHarass"))
                {
                    var delay = Computed.RandomDelay(Config.GetSliderValue("humanDelay"));
                    LeagueSharp.Common.Utility.DelayAction.Add(delay, () => Spells.E.Cast(target));
                }
                else
                {
                    Spells.E.Cast(target);
                }
            }
        }

        internal static void AutoQ()
        {
            if (!Spells.Q.IsReady() || ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            if (Config.IsChecked("dontAutoHarassInBush"))
            {
                var bush =
                    ObjectManager.Get<GrassObject>()
                        .OrderBy(br => br.Position.Distance(ObjectManager.Player.Position))
                        .FirstOrDefault();
                if (bush != null && ObjectManager.Player.Position.Distance(bush.Position) <= bush.BoundingRadius)
                {
                    return;
                }
            }

            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget(Spells.Q.Range))
            {
                return;
            }

            if (Config.IsChecked("dontAutoHarassTower") && target.Position.UnderEnemyTurret()
                && ObjectManager.Player.Position.UnderEnemyTurret())
            {
                return;
            }

            if (Spells.Q.IsReady() && !target.HasBuffOfType(BuffType.Poison))
            {
                var qPred = Spells.Q.GetPrediction(target);
                if (qPred.Hitchance >= HitChance.High)
                {
                    Spells.Q.Cast(qPred.CastPosition);
                }
            }
        }

        internal static void AutoW()
        {
            if (!Spells.W.IsReady() || ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            if (Config.IsChecked("dontAutoHarassInBush"))
            {
                var bush =
                    ObjectManager.Get<GrassObject>()
                        .OrderBy(br => br.Position.Distance(ObjectManager.Player.Position))
                        .FirstOrDefault();
                if (bush != null && ObjectManager.Player.Position.Distance(bush.Position) <= bush.BoundingRadius)
                {
                    return;
                }
            }

            var target = TargetSelector.GetTarget(Spells.W.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget(Spells.W.Range))
            {
                return;
            }

            if (Config.IsChecked("dontAutoHarassTower") && target.Position.UnderEnemyTurret()
                && ObjectManager.Player.Position.UnderEnemyTurret())
            {
                return;
            }

            if (Spells.W.IsReady() && target.IsValidTarget(Spells.W.Range))
            {
                var wPred = Spells.W.GetPrediction(target);
                if (wPred.CastPosition.Distance(ObjectManager.Player.Position) >= 550 && !wPred.CastPosition.IsWall()
                    && wPred.Hitchance >= HitChance.High)
                {
                    Spells.W.Cast(wPred.CastPosition);
                }
            }
        }

        #endregion
    }
}