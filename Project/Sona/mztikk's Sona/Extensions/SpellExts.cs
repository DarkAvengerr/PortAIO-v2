using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkSona.Extensions
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    public static class SpellExts
    {
        #region Public Methods and Operators

        public static bool CanCast(this Spell spell)
            => spell.Level > 0 && spell.IsReady() && spell.ManaCost < ObjectManager.Player.Mana;

        public static bool CanCast(this SpellDataInst spell)
            =>
                spell.IsReady()
                && ObjectManager.Player.Spellbook.Spells.FirstOrDefault(x => x.Name == spell.Name) != null;

        #endregion
    }
}