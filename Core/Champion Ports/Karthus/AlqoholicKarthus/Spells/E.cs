using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicKarthus.Spells
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using AlqoholicKarthus.Menu;

    #endregion

    internal class E : SpellBase
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
        internal override float Delay => 950f;

        /// <summary>
        ///     Spell Range
        /// </summary>
        internal override float Range => 425f;

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
        internal override SpellSlot SpellSlot => SpellSlot.E;

        /// <summary>
        ///     Gets or sets a value indicating whether the spell is targeted.
        /// </summary>
        internal override bool Targeted => false;

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        internal override float Width => 425f;

        #endregion

        #region Methods

        internal override void Combo(int predictionMode)
        {
            var target = TargetSelector.GetTarget(this.Range, this.DamageType);

            if (ObjectManager.Player.HasBuff("KarthusDefile")
                && HeroManager.Enemies.Count(x => x.Distance(ObjectManager.Player.Position) > this.Range)
                == HeroManager.Enemies.Count)
            {
                this.SpellObject.Cast(); // E Off
            }

            if ((target.Distance(ObjectManager.Player.Position) < this.Range - 80)
                && !ObjectManager.Player.HasBuff("KarthusDefile"))
            {
                this.SpellObject.Cast(); // E On
            }
        }

        /// <summary>
        ///     Spell Farm Logic
        /// </summary>
        internal override void Farm()
        {
            var minions =
                this.SpellObject.GetCircularFarmLocation(
                    MinionManager.GetMinions(this.Range, MinionTypes.All, MinionTeam.NotAlly));

            if (minions.MinionsHit >= 3 && !ObjectManager.Player.HasBuff("KarthusDefiled"))
            {
                this.SpellObject.Cast(); // E On
            }
            else
            {
                this.SpellObject.Cast(); // E Off
            }
        }

        /// <summary>
        ///     Spell Harass Logic
        /// </summary>
        internal override void Harass()
        {
            this.Combo(AlqoholicMenu.MainMenu.Item("prediction.mode").GetValue<StringList>().SelectedIndex);
        }

        #endregion
    }
}