using System.Linq;
using iDzed.Activator.Spells;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iDZed.Activator.Spells
{
    class Ignite : ISummonerSpell
    {
        public void OnLoad() { }
        public string GetDisplayName()
        {
            return "Ignite";
        }

        public void AddToMenu(Menu menu)
        {
        }

        public bool RunCondition()
        {
            if (GetSummonerSpell().IsReady() &&
                HeroManager.Enemies.FirstOrDefault(
                    x => x.IsValidTarget(GetSummonerSpell().Range) && x.HasBuff("zedulttargetmark")) != null)
            {
                return true;
            }

            return GetSummonerSpell().IsReady() &&
                   MenuHelper.IsMenuEnabled("com.idz.zed.activator.summonerspells." + GetName() + ".enabled") &&
                   ObjectManager.Player.GetEnemiesInRange(GetSummonerSpell().Range)
                       .Any(
                           h =>
                               h.Health + 20 <
                               ObjectManager.Player.GetSummonerSpellDamage(h, Damage.SummonerSpell.Ignite) &&
                               h.IsValidTarget(GetSummonerSpell().Range) && h.CountAlliesInRange(550f) < 3 && !((h.Health + 20 > Zed._spells[SpellSlot.Q].GetDamage(h) + Zed._spells[SpellSlot.E].GetDamage(h) + ObjectManager.Player.GetAutoAttackDamage(h))));
        }

        public void Execute()
        {
            AIHeroClient target = ObjectManager.Player.GetEnemiesInRange(GetSummonerSpell().Range).Find(h => h.Health + 20 < ObjectManager.Player.GetSummonerSpellDamage(h, Damage.SummonerSpell.Ignite) || h.HasBuff("zedulttargetmark"));
            if (target.IsValidTarget(GetSummonerSpell().Range))
            {
                GetSummonerSpell().Cast(target);
            }
        }

        public SummonerSpell GetSummonerSpell()
        {
            return SummonerSpells.Ignite;
        }

        public string GetName()
        {
            return GetSummonerSpell().Names.First().ToLowerInvariant();
        }
    }
}
