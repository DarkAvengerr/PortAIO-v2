using EloBuddy; 
using LeagueSharp.Common; 
namespace AlqoholicLissandra.Spells
{
    #region Using Directives

    using System.Linq;

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
        internal override float Delay => 0.25f;

        /// <summary>
        ///     Spell Range
        /// </summary>
        internal override float Range => 715f;

        /// <summary>
        ///     Skillshot Type
        /// </summary>
        internal override SkillshotType SkillshotType => SkillshotType.SkillshotLine;

        /// <summary>
        /// </summary>
        internal override float Speed => 2200f;

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
        internal override float Width => 75f;

        #endregion

        #region Methods

        /// <summary>
        ///     Gets New Range and Width
        /// </summary>
        /// <param name="collision"></param>
        internal void CheckCollision(bool collision)
        {
            if (collision)
            {
                this.Range = 825f;
                this.Width = 90f;
            }
            else
            {
                this.Range = 715f;
                this.Width = 75f;
            }
        }

        internal override void Combo()
        {
            var target = TargetSelector.GetTarget(this.Range, this.DamageType);

            var collisionObjects = this.SpellObject.GetPrediction(target).CollisionObjects.ToList();

            var hitchance = HitChance.Medium;

            switch (
                AlqoholicLissandra.Menu.AlqoholicMenu.MainMenu.Item("qhitchance").GetValue<StringList>().SelectedIndex)
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

            this.CheckCollision(collisionObjects.Any());

            if (this.SpellObject.GetPrediction(target).Hitchance >= hitchance)
            {
                Spells.Q.SpellObject.Cast(target);
            }
        }

        /// <summary>
        ///     Spell Farm Logic
        /// </summary>
        internal override void Farm()
        {
            var minion = MinionManager.GetMinions(ObjectManager.Player.Position, this.Range);

            var minions = this.SpellObject.GetLineFarmLocation(minion, this.Width);

            if (minions.MinionsHit >= 3)
            {
                Spells.Q.SpellObject.Cast(minions.Position);
            }
        }

        /// <summary>
        ///     Spell Harass Logic
        /// </summary>
        internal override void Harass()
        {
            this.Combo();

            //TODO: ADD EXTENDED Q HARASS ON MINION
        }

        /// <summary>
        ///     Last Hit Logic
        /// </summary>
        internal override void LastHit()
        {
            var minion = MinionManager.GetMinions(this.Range).FirstOrDefault(x => Spells.Q.SpellObject.IsKillable(x));

            if (minion != null)
            {
                Spells.Q.SpellObject.Cast(minion);
            }
        }

        #endregion
    }
}