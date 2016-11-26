using EloBuddy; 
using LeagueSharp.Common; 
 namespace DevCommom2
{
    using LeagueSharp;
    using LeagueSharp.Common;

    public class IgniteManager
    {
        public bool HasIgnite;

        private readonly SpellDataInst IgniteSpell;

        public IgniteManager()
        {
            IgniteSpell = ObjectManager.Player.Spellbook.GetSpell(ObjectManager.Player.GetSpellSlot("SummonerDot"));

            if (IgniteSpell != null && IgniteSpell.Slot != SpellSlot.Unknown)
            {
                HasIgnite = true;
            }
        }

        public bool Cast(AIHeroClient enemy)
        {
            if (!enemy.IsValid || !enemy.IsVisible || !enemy.IsTargetable || enemy.IsDead)
            {
                return false;
            }

            if (HasIgnite && IsReady() && enemy.IsValidTarget(600))
            {
                return ObjectManager.Player.Spellbook.CastSpell(IgniteSpell.Slot, enemy);
            }

            return false;
        }

        public bool IsReady()
        {
            return HasIgnite && this.IgniteSpell.State == SpellState.Ready;
        }

        public bool CanKill(AIHeroClient enemy)
        {
            return HasIgnite && IsReady() && enemy.Health < ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
        }
    }
}
