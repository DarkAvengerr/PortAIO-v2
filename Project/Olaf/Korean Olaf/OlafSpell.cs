using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanOlaf
{
    using LeagueSharp;
    using LeagueSharp.Common;

    public class OlafSpell : Spell
    {
        public bool UseOnCombo { get; set; }

        public bool UseOnHarass { get; set; }

        public bool UseOnLastHit { get; set; }

        public bool UseOnLaneClear { get; set; }

        public OlafSpell(SpellSlot slot, float range, TargetSelector.DamageType damageType)
            : base(slot, range, damageType)
        {
            UseOnCombo = false;
            UseOnHarass = false;
            UseOnLastHit = false;
            UseOnLaneClear = false;
        }

        public OlafSpell(SpellSlot slot)
            : this(slot, 0F)
        {
            
        }

        public OlafSpell(SpellSlot slot, float range)
            : this(slot, range, TargetSelector.DamageType.Physical)
        {

        }
    }
}
