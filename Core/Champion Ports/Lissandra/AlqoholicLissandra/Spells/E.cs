using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicLissandra.Spells
{
    #region Using Directives

    using AlqoholicLissandra.Menu;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class E : SpellBase
    {
        #region Fields

        internal string EBuffName = "LissandraE";

        #endregion

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
        internal override float Delay => 0.25f;

        /// <summary>
        ///     Spell Range
        /// </summary>
        internal override float Range => 1050f;

        /// <summary>
        ///     Skillshot Type
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.SkillshotLine;

        /// <summary>
        /// </summary>
        internal override float Speed => 850f;

        /// <summary>
        ///     Gets or sets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.E;

        /// <summary>
        ///     Gets or sets a value indicating whether the spell is targeted.
        /// </summary>
        internal override bool Targeted => false;

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        internal override float Width => 125f;

        #endregion

        #region Methods

        internal override void Combo()
        {
            var smartLogic = AlqoholicMenu.MainMenu.Item("comboesmartlogic").GetValue<bool>();

            var target = TargetSelector.GetTarget(this.Range, this.DamageType);

            if (!smartLogic && !ObjectManager.Player.HasBuff(this.EBuffName))
            {
                var hitchance = HitChance.Medium;

                switch (AlqoholicMenu.MainMenu.Item("ehitchance").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        hitchance = HitChance.Low;
                        break;
                    case 1:
                        hitchance = HitChance.Medium;
                        break;
                    case 2:
                        hitchance = HitChance.High;
                        break;
                    case 3:
                        hitchance = HitChance.VeryHigh;
                        break;
                }

                if (ObjectManager.Player.HasBuff(this.EBuffName)
                    || (Spells.E.SpellObject.GetPrediction(target).Hitchance < hitchance))
                {
                    return;
                }
                if (AlqoholicMenu.MainMenu.Item("comboe2use").GetValue<bool>())
                {
                    Spells.E.SpellObject.Cast(target);
                    LeagueSharp.Common.Utility.DelayAction.Add(1500, () => Spells.E.SpellObject.Cast());
                }
                else
                {
                    Spells.E.SpellObject.Cast(target);
                }
            }
            else
            {
                /* SMART LOGIC V2 */
                var eTargets = Spells.E.SpellObject.GetPrediction(target, true, Spells.E.Range + (Spells.W.Range / 2));
                if (eTargets.AoeTargetsHitCount < 2 || !Spells.W.SpellObject.IsReady())
                {
                    return;
                }
                Spells.E.SpellObject.Cast(eTargets.CastPosition);
                LeagueSharp.Common.Utility.DelayAction.Add(1500, () => Spells.E.SpellObject.Cast());
                var wTargets = Spells.W.SpellObject.GetPrediction(target, true);
                if (wTargets.AoeTargetsHitCount > 1)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(100, () => Spells.W.SpellObject.Cast());
                }

                /* SMART LOGIC */
                //var smartRange = Spells.E.Range + (Spells.W.Range / 2);
                //var smartTargets = HeroManager.Enemies.OrderBy(x => x.Distance(ObjectManager.Player.Position) < smartRange).ToList();

                //var castPosition = new Vector3(0, 0, 0);
                //castPosition = smartTargets.Aggregate(castPosition, (current, x) => current + x.Position);
                //castPosition = castPosition / smartTargets.Count;

                //Spells.E.SpellObject.Cast(castPosition);
            }
        }

        /// <summary>
        ///     Escape Logic
        /// </summary>
        internal void Escape()
        {
            Spells.E.SpellObject.Cast(Game.CursorPos);
            LeagueSharp.Common.Utility.DelayAction.Add(1500, () => Spells.E.SpellObject.Cast());
        }

        /// <summary>
        ///     Spell Farm Logic
        /// </summary>
        internal override void Farm()
        {
        }

        /// <summary>
        ///     Spell Harass Logic
        /// </summary>
        internal override void Harass()
        {
        }

        #endregion
    }
}