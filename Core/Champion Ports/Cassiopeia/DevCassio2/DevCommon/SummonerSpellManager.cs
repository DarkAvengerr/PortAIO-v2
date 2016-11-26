using EloBuddy; 
using LeagueSharp.Common; 
 namespace DevCommom2
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    public class SummonerSpellManager
    {
        public SpellSlot IgniteSlot;
        public SpellSlot FlashSlot;
        public SpellSlot BarrierSlot;
        public SpellSlot HealSlot;
        public SpellSlot ExhaustSlot;

        public SummonerSpellManager()
        {
            IgniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
            FlashSlot = ObjectManager.Player.GetSpellSlot("SummonerFlash");
            BarrierSlot = ObjectManager.Player.GetSpellSlot("SummonerBarrier");
            HealSlot = ObjectManager.Player.GetSpellSlot("SummonerHeal");
            ExhaustSlot = ObjectManager.Player.GetSpellSlot("SummonerExhaust");
        }

        public bool CastIgnite(AIHeroClient target)
        {
            return IsReadyIgnite() && ObjectManager.Player.Spellbook.CastSpell(IgniteSlot, target);
        }

        public bool CastFlash(Vector3 position)
        {
            return IsReadyFlash() && ObjectManager.Player.Spellbook.CastSpell(FlashSlot, position);
        }

        public bool CastBarrier()
        {
            return IsReadyBarrier() && ObjectManager.Player.Spellbook.CastSpell(BarrierSlot);
        }

        public bool CastHeal()
        {
            return IsReadyHeal() && ObjectManager.Player.Spellbook.CastSpell(HealSlot);
        }

        public bool CastExhaust(AIHeroClient target)
        {
            return IsReadyExhaust() && ObjectManager.Player.Spellbook.CastSpell(ExhaustSlot, target);
        }

        public bool IsReadyIgnite()
        {
            return IgniteSlot != SpellSlot.Unknown &&
                   ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready;
        }

        public bool IsReadyFlash()
        {
            return FlashSlot != SpellSlot.Unknown &&
                   ObjectManager.Player.Spellbook.CanUseSpell(FlashSlot) == SpellState.Ready;
        }

        public bool IsReadyBarrier()
        {
            return BarrierSlot != SpellSlot.Unknown &&
                   ObjectManager.Player.Spellbook.CanUseSpell(BarrierSlot) == SpellState.Ready;
        }

        public bool IsReadyHeal()
        {
            return HealSlot != SpellSlot.Unknown &&
                   ObjectManager.Player.Spellbook.CanUseSpell(HealSlot) == SpellState.Ready;
        }

        public bool IsReadyExhaust()
        {
            return ExhaustSlot != SpellSlot.Unknown &&
                   ObjectManager.Player.Spellbook.CanUseSpell(ExhaustSlot) == SpellState.Ready;
        }

        public bool CanKillIgnite(AIHeroClient target)
        {
            return IsReadyIgnite() &&
                   target.Health < ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        public double GetIgniteDamage(AIHeroClient target)
        {
            return IsReadyIgnite()
                ? ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)
                : 0;
        }
    }
}
