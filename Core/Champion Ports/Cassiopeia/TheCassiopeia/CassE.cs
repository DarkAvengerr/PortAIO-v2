using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using TheCassiopeia.Commons;
using TheCassiopeia.Commons.ComboSystem;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace TheCassiopeia
{
    class CassE : Skill
    {
        public bool Farm;
        public int SkillDelay;
        private int _killedWithE;
        public int SkillDelayRnd;
        private int _currentRandomDelay;
        private Random _random = new Random();

        public CassE(SpellSlot slot)
            : base(slot)
        {
            Range = Instance.SData.CastRange + 50;
            SetTargetted(0.2f, float.MaxValue);
            UseManaManager = true;

        }

        public override void Initialize(ComboProvider combo)
        {
            _currentRandomDelay = _random.Next(0, SkillDelayRnd);
            Orbwalking.BeforeAttack += args =>
            {
                if (combo.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit || combo.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    if (this.IsReady() && ManaManager.CanUseMana(Orbwalking.OrbwalkingMode.LastHit) || _killedWithE == args.Target.NetworkId)
                        args.Process = false;
                }
            };
            base.Initialize(combo);
        }


        public override void Lasthit()
        {
            if (!Farm) return;
            var killableMinion = MinionManager.GetMinions(Range, MinionTypes.All, MinionTeam.NotAlly).FirstOrDefault(minion =>
            {
                var hpred = HealthPrediction.GetHealthPrediction(minion, (int)(Delay * 1000f));
                if (hpred <= 0) return false;
                return GetDamage(minion) > hpred || minion.Team == GameObjectTeam.Neutral;
            });

            if (killableMinion == null) return;

            Cast(killableMinion);
            _killedWithE = killableMinion.NetworkId;
        }

        public override void Execute(AIHeroClient target)
        {
            if (SkillDelay == 0 && SkillDelayRnd <= 1 || Instance.CooldownExpires + SkillDelay / 1000f + _currentRandomDelay / 1000f < Game.Time)
            {
                if (Cast(target) == CastStates.SuccessfullyCasted)
                    _currentRandomDelay = _random.Next(0, SkillDelayRnd);
            }
        }

        public override void Harass(AIHeroClient target)
        {
            if (ManaManager.CanUseMana(Orbwalking.OrbwalkingMode.Mixed))
                base.Harass(target);
        }

        public override void LaneClear()
        {
            if (!ManaManager.CanUseMana(Orbwalking.OrbwalkingMode.LaneClear)) return;

            var clearMinion = MinionManager.GetMinions(Range, MinionTypes.All, MinionTeam.NotAlly).FirstOrDefault();

            if (clearMinion != null)
                Cast(clearMinion);
        }

        public override float GetDamage(AIHeroClient enemy)
        {
            return base.GetDamage(enemy) * 2;
        }

        public override int GetPriority()
        {
            return 1;
        }

        public override void Draw()
        {
            if (Provider.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LastHit) return;
            var killableMinion = MinionManager.GetMinions(Range, MinionTypes.All, MinionTeam.NotAlly).FirstOrDefault(minion =>
            {
                var hpred = HealthPrediction.GetHealthPrediction(minion, (int)(Delay * 1000f));
                if (hpred <= 0) return false;
                return GetDamage(minion) > hpred || minion.Team == GameObjectTeam.Neutral;
            });
            if (killableMinion != null)
                Render.Circle.DrawCircle(killableMinion.Position, 75, Color.Aquamarine);
            base.Draw();
        }
    }
}
