using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkPantheon
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class Computed
    {
        #region Methods

        internal static float GetComboDamage(AIHeroClient target)
        {
            var rtn = 0d;
            if (Spells.Q.IsReady())
            {
                rtn += ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q);
            }

            if (Spells.W.IsReady())
            {
                rtn += ObjectManager.Player.GetSpellDamage(target, SpellSlot.W);
            }

            if (Spells.E.IsReady())
            {
                rtn += ObjectManager.Player.GetSpellDamage(target, SpellSlot.E);
            }

            rtn += ObjectManager.Player.GetAutoAttackDamage(target, true);

            return (float)rtn;
        }

        #endregion
    }
}