using LeagueSharp;
using LeagueSharp.Common;
using WarwickII.Champion;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace WarwickII.Common
{
    internal class CommonMath
    {
        public static float GetComboDamage(Obj_AI_Base t)
        {
            var fComboDamage = 0d;

            if (Champion.PlayerSpells.Q.IsReady())
            {
                fComboDamage += PlayerSpells.WarwickDamage.Q(PlayerSpells.WarwickDamage.QFor.Enemy);
            }

            if (Champion.PlayerSpells.W.IsReady())
            {
                fComboDamage += PlayerSpells.WarwickDamage.W;
            }

            if (Champion.PlayerSpells.R.IsReady())
            {
                fComboDamage += PlayerSpells.R.GetDamage(t);
            }

            if (CommonItems.Youmuu.IsReady())
            {
                fComboDamage += ObjectManager.Player.TotalAttackDamage * 3;
            }

            if (Common.CommonSummoner.IgniteSlot != SpellSlot.Unknown
                && ObjectManager.Player.Spellbook.CanUseSpell(Common.CommonSummoner.IgniteSlot) == SpellState.Ready)
            {
                fComboDamage += ObjectManager.Player.GetSummonerSpellDamage(t, Damage.SummonerSpell.Ignite);
            }

            if (LeagueSharp.Common.Items.CanUseItem(3128))
            {
                fComboDamage += ObjectManager.Player.GetItemDamage(t, Damage.DamageItems.Botrk);
            }

            return (float)fComboDamage;
        }

    }
}
