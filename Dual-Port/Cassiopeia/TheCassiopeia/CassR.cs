using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using TheCassiopeia.Commons;
using TheCassiopeia.Commons.ComboSystem;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheCassiopeia
{
    class CassR : Skill
    {
        public bool UltOnKillable;
        public int MinTargetsFacing;
        public int MinTargetsNotFacing;
        public int GapcloserUltHp;
        public int MinHealth;
        public int PanicModeHealth;
        public MenuItem BurstMode;
        public bool MinEnemiesOnlyInCombo;

        public CassR(SpellSlot slot)
            : base(slot)
        {
            Range = 825f;
        }


        public override void Initialize(ComboProvider combo)
        {
            SetSkillshot(0.4f, (float)(80 * Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);
            base.Initialize(combo);
        }

        public override void Update(Orbwalking.OrbwalkingMode mode, ComboProvider combo, AIHeroClient target)
        {
            if (!MinEnemiesOnlyInCombo && CanBeCast())
            {
                var pred = GetPrediction(target);
                if (pred.Hitchance < HitChance.Low) return;

                var targets = HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(Range) && WillHit(enemy.Position, pred.CastPosition));
                var looking = targets.Count(trgt => trgt.IsFacingMe());
                if (looking >= MinTargetsFacing || targets.Count() >= MinTargetsNotFacing)
                    Cast(pred.CastPosition);

            }

            base.Update(mode, combo, target);
        }

        private List<Vector2> Grouped(Vector3[] pos)
        {
            var posReal = pos.Select(item => item.To2D()).ToArray();
            var items = new List<Vector2>();
            var player2D = ObjectManager.Player.Position.To2D();
            var biggestItems = items;
            for (var i = 0; i < posReal.Length; i++)
            {
                items = new List<Vector2>();
                var current = posReal[i];
                var currentP = current - player2D;
                items.Add(current);

                var angle = 0f;
                for (var j = 0; j < posReal.Length; j++)
                {
                    if (i == j) continue;

                    var cAngle = currentP.AngleBetweenEx(posReal[j] - player2D);
                    if (cAngle > 0 && cAngle < 72)
                    {
                        items.Add(posReal[j]);
                        if (cAngle > angle)
                        {
                            angle = cAngle;
                        }
                    }
                }

                if (items.Count > biggestItems.Count)
                {
                    biggestItems = items;
                }
            }

            return biggestItems;
        }

        public Tuple<Vector3, int> GetBestPosition(IEnumerable<Obj_AI_Base> targets)
        {
            var preds = Grouped(targets.Where(enemy => enemy.IsValidTarget(Range + enemy.MoveSpeed * Delay)).Select(item => CassW.GetMovementPrediction(item, Delay)).Where(pred =>
            {
                var dst = pred.Distance(ObjectManager.Player.Position, true);
                return dst < Range * Range;
            }).ToArray());

            if (preds.Count > 0)
            {
                var final = new Vector3();
                for (var i = 0; i < preds.Count; i++)
                {
                    final.X += preds[i].X;
                    final.Y += preds[i].Y;
                }
                final.X /= preds.Count;
                final.Y /= preds.Count;
                return new Tuple<Vector3, int>(final, preds.Count);
            }
            return null;
        }

        public override void Execute(AIHeroClient target)
        {
            //Console.WriteLine(Delay);
            var bestPosFacing = GetBestPosition(HeroManager.Enemies.Where(item => item.IsFacingMe()));

            if (MinTargetsFacing <= bestPosFacing?.Item2)
            {
                Cast(bestPosFacing.Item1);
                return;
            }

            var bestPos = GetBestPosition(HeroManager.Enemies);
            if (bestPos?.Item2 >= MinTargetsNotFacing)
            {
                Cast(bestPos.Item1);
                return;
            }


            if (UltOnKillable && Provider.GetComboDamage(target) > target.Health && target.IsFacingMe() && target.HealthPercent > MinHealth && target.IsValidTarget() || PanicModeHealth > ObjectManager.Player.HealthPercent || BurstMode.IsActive())
            {
                var targetPos = CassW.GetMovementPrediction(target);
                if (targetPos.Distance(ObjectManager.Player.Position, true) < Range * Range)
                {
                    Cast(targetPos);
                }
            }
        }

        public override void Gapcloser(ComboProvider combo, ActiveGapcloser gapcloser)
        {
            if (ObjectManager.Player.HealthPercent < GapcloserUltHp && gapcloser.Sender.IsValidTarget(Range))
            {
                var pred = GetPrediction(gapcloser.Sender);
                if (pred.Hitchance < HitChance.Low) return;

                Cast(pred.CastPosition);
            }
        }

        public override int GetPriority()
        {
            return 4;
        }
    }
}
