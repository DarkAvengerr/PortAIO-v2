using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicKarthus.Spells
{
    #region Using Directives

    using System;
    using System.Linq;

    using AlqoholicKarthus.Menu;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class Q : SpellBase
    {
        #region Properties

        /// <summary>
        ///     Aoe boolean
        /// </summary>
        internal override bool Aoe => true;

        /// <summary>
        ///     Collision boolean
        /// </summary>
        internal override bool Collision => false;

        /// <summary>
        ///     TargetSelector Damagetype
        /// </summary>
        internal override TargetSelector.DamageType DamageType => TargetSelector.DamageType.Magical;

        /// <summary>
        ///     Spell Delay
        /// </summary>
        internal override float Delay => 1f;

        /// <summary>
        ///     Spell Range
        /// </summary>
        internal override float Range => 875f;

        /// <summary>
        ///     Skillshot Type
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.SkillshotCircle;

        /// <summary>
        /// </summary>
        internal override float Speed => float.MaxValue;

        /// <summary>
        ///     Gets or sets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.Q;

        /// <summary>
        ///     Gets or sets a value indicating whether the spell is targeted.
        /// </summary>
        internal override bool Targeted => false;

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        internal override float Width => 160f;

        internal float VariableWidth = 160f;

        #endregion

        #region Methods

        internal override void Combo(int predictionMode)
        {
            var target = TargetSelector.GetTarget(this.Range, this.DamageType);

            var commonPrediction = true;

            if (target == null)
            {
                return;
            }

            switch (predictionMode)
            {
                case 0:
                    commonPrediction = true;
                    break;
                case 1:
                    commonPrediction = false;
                    break;
            }

            if (commonPrediction)
            {
                var prediction = Spells.Q.SpellObject.GetPrediction(target);
                var hitChance = HitChance.Medium;

                switch (AlqoholicMenu.MainMenu.Item("combo.hitchance").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        hitChance = HitChance.Low;
                        break;
                    case 1:
                        hitChance = HitChance.Medium;
                        break;
                    case 2:
                        hitChance = HitChance.High;
                        break;
                    case 3:
                        hitChance = HitChance.VeryHigh;
                        break;
                    case 4:
                        hitChance = HitChance.Immobile;
                        break;
                }

                if (prediction.Hitchance < hitChance)
                {
                    return;
                }
                Spells.Q.Width = this.GetDynamicQWidth(target);
                Spells.Q.SpellObject.Cast(target);
            }
            else
            {
                const SebbyLib.Prediction.SkillshotType CoreType = SebbyLib.Prediction.SkillshotType.SkillshotCircle;

                var predInput = new SebbyLib.Prediction.PredictionInput
                {
                    Aoe = this.Aoe,
                    Collision = Spells.Q.Collision,
                    Speed = Spells.Q.Speed,
                    Delay = Spells.Q.Delay,
                    Range = Spells.Q.Range,
                    From = ObjectManager.Player.ServerPosition,
                    Radius = Spells.Q.Width,
                    Unit = target,
                    Type = CoreType
                };

                var poutput = SebbyLib.Prediction.Prediction.GetPrediction(predInput);

                switch (AlqoholicMenu.MainMenu.Item("combo.hitchance").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (poutput.Hitchance >= SebbyLib.Prediction.HitChance.Low)
                        {
                            Spells.Q.SpellObject.Cast(poutput.CastPosition);
                        }
                        break;
                    case 1:
                        if (poutput.Hitchance >= SebbyLib.Prediction.HitChance.Medium)
                        {
                            Spells.Q.SpellObject.Cast(poutput.CastPosition);
                        }
                        break;
                    case 2:
                        if (poutput.Hitchance >= SebbyLib.Prediction.HitChance.High)
                        {
                            Spells.Q.SpellObject.Cast(poutput.CastPosition);
                        }
                        break;
                    case 3:
                        if (poutput.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                        {
                            Spells.Q.SpellObject.Cast(poutput.CastPosition);
                        }
                        else if (predInput.Aoe && poutput.AoeTargetsHitCount > 1
                                 && poutput.Hitchance >= SebbyLib.Prediction.HitChance.High)
                        {
                            Spells.Q.SpellObject.Cast(poutput.CastPosition);
                        }
                        break;
                    case 4:
                        if (poutput.Hitchance == SebbyLib.Prediction.HitChance.Immobile)
                        {
                            Spells.Q.SpellObject.Cast(poutput.CastPosition);
                        }
                        break;
                }
            }
        }

        internal float GetDynamicQWidth(Obj_AI_Base target)
        {
            return Math.Max(30, (1f - (ObjectManager.Player.Distance(target) / Spells.Q.Range)) * this.VariableWidth);
        }

        /// <summary>
        ///     Spell Farm Logic
        /// </summary>
        internal override void Farm()
        {
            var minion = MinionManager.GetMinions(this.Range);

            var minions =
                Spells.Q.SpellObject.GetCircularFarmLocation(minion, this.VariableWidth);

            switch (AlqoholicMenu.MainMenu.Item("laneclear.mode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (minions.MinionsHit >= 1)
                    {
                        Spells.Q.SpellObject.Cast(minions.Position);
                    }
                    break;
                case 1:
                    if ((minions.Position.Distance(ObjectManager.Player) > ObjectManager.Player.AttackRange)
                        && (ObjectManager.Player.Distance(minions.Position) <= this.Range) && minions.MinionsHit >= 1)
                    {
                        Spells.Q.SpellObject.Cast(minions.Position);
                    }
                    break;
            }
        }

        /// <summary>
        ///     Spell Harass Logic
        /// </summary>
        internal override void Harass()
        {
            this.Combo(AlqoholicMenu.MainMenu.Item("prediction.mode").GetValue<StringList>().SelectedIndex);
        }

        internal override void LastHit()
        {
            var minion = MinionManager.GetMinions(this.Range).First(x => Spells.Q.SpellObject.IsKillable(x));

            if (minion != null)
            {
                Spells.Q.SpellObject.Cast(minion);
            }
        }

        #endregion
    }
}