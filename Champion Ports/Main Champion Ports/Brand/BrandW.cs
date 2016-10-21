using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using TheBrand.Commons.ComboSystem;
using EloBuddy;

namespace TheBrand
{
    class BrandW : Skill
    {
        private BrandE _brandE;
        private BrandQ _brandQ;
        public bool DrawPredictedW, InterruptE, InterruptW;
        public int WaveclearTargets;
        public Color PredictedWColor;
        public bool TryAreaOfEffect;

        public BrandW(SpellSlot slot)
            : base(slot)
        {
            SetSkillshot(1.15f, 230f, int.MaxValue, false, SkillshotType.SkillshotCircle); // adjusted the range, for some reason the prediction was off, and missplaced it alot
            Range = 920;
        }

        public override void Initialize(ComboProvider combo)
        {
            _brandE = combo.GetSkill<BrandE>();
            _brandQ = combo.GetSkill<BrandQ>();
            Drawing.OnDraw += Draw;
            base.Initialize(combo);
        }

        private void Draw(EventArgs args)
        {
            if (!DrawPredictedW) return;
            try
            {
                var target = Provider.Target;
                if (Provider.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || target == null) return;
                var prediction = GetPrediction(target, true);
                if (prediction.CastPosition.Distance(ObjectManager.Player.Position) < 900)
                    Render.Circle.DrawCircle(prediction.CastPosition, 240f, PredictedWColor);
            }
            catch { }
        }

        public override void Execute(AIHeroClient target)
        {
            if (!Provider.ShouldBeDead(target))
            {
                Cast(target, aoe: TryAreaOfEffect);
            }
        }

        public override float GetDamage(AIHeroClient enemy)
        {
            var baseDamage = base.GetDamage(enemy);
            return enemy.HasBuff("brandablaze") || _brandE.CanBeCast() && enemy.Distance(ObjectManager.Player) < 650 ? baseDamage * 1.25f : baseDamage;
        }

        public override void LaneClear(ComboProvider combo, AIHeroClient target)
        {
            var locationM = GetCircularFarmLocation(MinionManager.GetMinions(900 + 120, MinionTypes.All, MinionTeam.NotAlly));
            if (locationM.MinionsHit >= WaveclearTargets)
                Cast(locationM.Position);
        }

        public override void Interruptable(ComboProvider combo, AIHeroClient sender, ComboProvider.InterruptableSpell interruptableSpell, float endTime)
        {
            var distance = sender.Distance(ObjectManager.Player);
            if (sender.HasBuff("brandablaze") || Provider.IsMarked(sender) || !_brandQ.CouldHit(sender) || !InterruptW) return;

            if (Cast(sender) == CastStates.SuccessfullyCasted)
                Provider.SetMarked(sender); //Todo: risky, keep an eye on this. If the W misses u r fucked 
        }

        public override int GetPriority()
        {
            return 2;
        }
    }
}
