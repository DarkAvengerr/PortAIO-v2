using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanAnnie.Common
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class CommonSpell : Spell
    {
        #region Constructors and Destructors

        public CommonSpell(
            SpellSlot slot,
            float range = 0,
            TargetSelector.DamageType damageType = TargetSelector.DamageType.Physical)
            : base(slot, range, damageType)
        {
        }

        #endregion

        #region Public Properties

        public float LastTimeUsed
        {
            get
            {
                return Instance.CooldownExpires - Instance.Cooldown;
            }
        }

        public float UsedforXSecAgo
        {
            get
            {
                return Game.Time - LastTimeUsed;
            }
        }

        public bool UseOnCombo
        {
            get
            {
                return UseOnComboMenu != null && UseOnComboMenu.GetValue<bool>();
            }
        }

        public MenuItem UseOnComboMenu { get; set; }

        public bool UseOnHaras
        {
            get
            {
                return UseOnHarasMenu != null && UseOnHarasMenu.GetValue<bool>();
            }
        }

        public MenuItem UseOnHarasMenu { get; set; }

        public bool UseOnLaneClear
        {
            get
            {
                return UseOnComboMenu != null && UseOnLaneClearMenu.GetValue<bool>();
            }
        }

        public MenuItem UseOnLaneClearMenu { get; set; }

        #endregion

        #region Public Methods and Operators

        public bool CanCast()
        {
            return Instance.ToggleState <= 1;
        }

        #endregion
    }
}