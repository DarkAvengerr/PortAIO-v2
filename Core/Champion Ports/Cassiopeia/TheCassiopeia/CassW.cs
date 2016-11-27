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
using Prediction = TheCassiopeia.Commons.Prediction.Prediction;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace TheCassiopeia
{
    class CassW : Skill
    {
        private CassQ _q;
        private CassE _e;
        private CassR _r;
        public bool UseOnGapcloser;
        public float MinRange = 500f;
        public float MaxRange = 800f;
        public float MinRangeHigh = 500f;
        public float MaxRangeHigh = 800f;
        public int ClearMinHit;


        public CassW(SpellSlot slot)
            : base(slot)
        {
            MinRange -= Instance.SData.CastRadius;
            MaxRange += Instance.SData.CastRadius;
            SetSkillshot(0.85f, Instance.SData.CastRadius, Instance.SData.MissileSpeed, false, SkillshotType.SkillshotCircle);
            Range = MaxRange;
            HarassEnabled = false;
        }

        public override void Initialize(ComboProvider combo)
        {
            _q = combo.GetSkill<CassQ>();
            _r = combo.GetSkill<CassR>();
            _e = combo.GetSkill<CassE>();
            base.Initialize(combo);
        }

        public override void Execute(AIHeroClient target)
        {
            if (_e.CanBeCast() && _e.IsKillable(target))
                return;

            var bestPosition = GetBestPosition(HeroManager.Enemies);
            if (Math.Abs(bestPosition.Item1.X) > 1)
            {
                Cast(bestPosition.Item1);
            }
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
            var preds = Grouped(targets.Where(enemy => enemy.IsValidTarget(MaxRange + enemy.MoveSpeed * Delay)).Select(item => GetMovementPrediction(item, Delay)).Where(pred =>
              {
                  var dst = pred.Distance(ObjectManager.Player.Position, true);
                  if (MinComboHitchance > HitChance.Low)
                  {
                      if (dst < MinRangeHigh * MinRangeHigh) return false;
                      return dst < MaxRangeHigh * MaxRangeHigh;
                  }
                  if (dst < MinRange * MinRange) return false;
                  return dst < MaxRange * MaxRange;
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
            return new Tuple<Vector3, int>(Vector3.Zero, 0);
        }

        public static Vector3 GetMovementPrediction(Obj_AI_Base target, float time = 1f)
        {
            var input = new TheCassiopeia.Commons.Prediction.PredictionInput() { Unit = target, Delay = time };
            if (input.Unit.IsDashing())
            {
                return Prediction.GetDashingPrediction(input).UnitPosition;
            }

            //Unit is immobile.
            var remainingImmobileT = Prediction.UnitIsImmobileUntil(input.Unit);
            return remainingImmobileT >= 0d ? Prediction.GetImmobilePrediction(input, remainingImmobileT).UnitPosition : Prediction.GetStandardPrediction(input).UnitPosition;
        }

        public override void Gapcloser(ComboProvider combo, ActiveGapcloser gapcloser)
        {
            if (UseOnGapcloser && (!_r.CanBeCast() || _r.GapcloserUltHp < ObjectManager.Player.HealthPercent))
            {
                Cast(gapcloser.Sender);
            }
        }

        public override float GetDamage(AIHeroClient enemy)
        {
            return 0;
        }

        public override void LaneClear()
        {
            var minions = GetBestPosition(MinionManager.GetMinions(800, MinionTypes.All, MinionTeam.NotAlly));
            // var minions = Grouped(MinionManager.GetMinions(800, MinionTypes.All, MinionTeam.NotAlly).Where(min => min.Distance(ObjectManager.Player.Position, true) > MinRange * MinRange).Select(item => item.Position).ToArray());
            //   Console.WriteLine(minions.Item2);

            if (minions.Item2 > ClearMinHit)
            {
                Cast(minions.Item1);
            }
        }

        public override int GetPriority()
        {
            return 1;
        }
    }
}
