#region Use
using LeagueSharp;
using LeagueSharp.Common;
using System.Linq; 
#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate.Modes
{
    using Config = GrossGoreTwistedFate.Config;

    internal static class ComboMode
    {
        #region Methods

        internal static void Execute()
        {
            var wMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;

            if (ObjectManager.Player.Mana >= wMana)
            {
                var entKs = HeroManager.Enemies.FirstOrDefault(
                x => !x.IsDead && x.IsValidTarget()
                && (ObjectManager.Player.Distance(x) < Orbwalking.GetAttackRange(ObjectManager.Player) + 200)
                && x.Health < ObjectManager.Player.GetSpellDamage(x, SpellSlot.W));

                if (entKs != null)
                {
                    if (Spells._w.IsReadyPerfectly())
                    {
                        if (CardSelector.Status == SelectStatus.Ready)
                        {
                            CardSelector.StartSelecting(Cards.First);
                        }
                        else if (CardSelector.Status == SelectStatus.Selecting)
                        {
                            CardSelector.JumpToCard(Cards.First);
                        }
                    }
                }else
                {
                    if(Config.UseGoldCombo)
                    {
                        if (Spells._w.IsReadyPerfectly())
                        {
                            if (CardSelector.Status == SelectStatus.Ready)
                            {
                                CardSelector.StartSelecting(Cards.Yellow);
                            }
                        }

                        else if (CardSelector.Status == SelectStatus.Selecting)
                        {
                            CardSelector.JumpToCard(Cards.Yellow);
                        }
                    }
                }
            }
        }

        #endregion
    }
}