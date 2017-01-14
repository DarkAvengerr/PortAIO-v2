using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicLissandra.Spells
{
    #region Using Directives

    using System.Linq;

    using AlqoholicLissandra.Menu;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    internal class R : SpellBase
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
        internal override float Delay => 0f;

        /// <summary>
        ///     Spell Range
        /// </summary>
        internal override float Range => 400f;

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
        internal override SpellSlot SpellSlot => SpellSlot.R;

        /// <summary>
        ///     Gets or sets a value indicating whether the spell is targeted.
        /// </summary>
        internal override bool Targeted => true;

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        internal override float Width => 400f;

        #endregion

        #region Methods

        /// <summary>
        ///     Spell Combo Logic
        /// </summary>
        internal override void Combo()
        {
            var enemies =
                HeroManager.Enemies.Where(x => x.Distance(ObjectManager.Player) <= this.Range)
                    .OrderBy(x => Spells.R.SpellObject.GetDamage(x)).ToList();

            //TODO: SELF ULT
            if (enemies.Count() > 2)
            {
                Spells.R.SpellObject.Cast(ObjectManager.Player);
            }

            foreach (var enemy in enemies.Where(enemy => AlqoholicMenu.MainMenu.Item("ult" + enemy.ChampionName).GetValue<bool>()).ToList())
            {
                Spells.R.SpellObject.Cast(enemy);
            }
        }

        #endregion
    }
}