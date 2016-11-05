using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Wukong.Extensions
{
    using LeagueSharp;
    using LeagueSharp.Common;

    public static class SpellExts
    {
        #region Public Methods and Operators

        public static bool CanCast(this Spell spell)
            => spell.Level > 0 && spell.IsReady() && spell.ManaCost < ObjectManager.Player.Mana;

        #endregion
    }
}