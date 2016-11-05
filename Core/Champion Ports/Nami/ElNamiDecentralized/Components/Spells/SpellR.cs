namespace ElNamiDecentralized.Components.Spells
{
    using System;
    using System.Linq;

    using ElNamiDecentralized.Enumerations;
    using ElNamiDecentralized.Utils;

    using EloBuddy;
    using LeagueSharp.Common;

    using SharpDX;

    /// <summary>
    ///     The spell W.
    /// </summary>
    internal class SpellR : ISpell
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the damage type.
        /// </summary>
        internal override TargetSelector.DamageType DamageType => TargetSelector.DamageType.Magical;

        /// <summary>
        ///     Gets the delay.
        /// </summary>
        internal override float Delay => 500f;

        /// <summary>
        ///     Gets the range.
        /// </summary>
        internal override float Range => 2750f;

        /// <summary>
        ///     Gets the aoe.
        /// </summary>
        internal override bool Aoe => true;

        /// <summary>
        ///     Gets or sets the skillshot type.
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.SkillshotLine;

        /// <summary>
        ///     Gets the speed.
        /// </summary>
        internal override float Speed => 850f;

        /// <summary>
        ///     Gets the spell slot.
        /// </summary>
        internal override SpellSlot SpellSlot => SpellSlot.R;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        internal override float Width => 260f;

        #endregion

        #region Methods

        /// <summary>
        ///     The on combo callback.
        /// </summary>
        internal override void OnCombo()
        {
            try
            {
                if (this.SpellObject == null)
                {
                    return;
                }

                // For later
                /*
                var target = HeroManager.Enemies.OrderBy(x => x.Distance(ObjectManager.Player)).FirstOrDefault(x => Misc.SpellR.SpellObject.IsInRange(x));
                if (target != null)
                {
                    var rect = ObjectManager.Player.ServerPosition.To2D();
                    var rectEnd = this.SpellObject.GetPrediction(target).CastPosition.To2D();

                    var rectangle = new Geometry.Polygon.Rectangle(rect, rectEnd, this.Width);
                    Logging.AddEntry(LoggingEntryTrype.Info, "SpellR.cs can hit {0}", HeroManager.Enemies.Count(x => rectangle.IsInside(x)));

                    if (HeroManager.Enemies.Count(x => rectangle.IsInside(x)) >= MyMenu.RootMenu.Item("comborhit").GetValue<Slider>().Value)
                    {
                        this.SpellObject.Cast(target);
                    }
                }*/

                var target = Misc.GetTarget(this.Range, this.DamageType);
                if (target != null)
                {
                    this.SpellObject.CastIfWillHit(target, MyMenu.RootMenu.Item("comborhit").GetValue<Slider>().Value);
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryTrype.Error, "@SpellR.cs: Can not run OnCombo - {0}", e);
                throw;
            }
        }

        #endregion
    }
}
 