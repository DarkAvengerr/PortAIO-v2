using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate.Modes
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using System.Linq;

    using Config = GrossGoreTwistedFate.Config;

    internal static class ComboMode
    {
        #region Methods

        internal static void Execute()
        {
            var wMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;

            if (ObjectManager.Player.Mana >= wMana)
            {
                var entKs =
                    HeroManager.Enemies.FirstOrDefault(
                        h =>
                        !h.IsDead && h.IsValidTarget()
                        && (ObjectManager.Player.Distance(h) < Orbwalking.GetAttackRange(ObjectManager.Player) + 200)
                        && h.Health < ObjectManager.Player.GetSpellDamage(h, SpellSlot.W));

                if (Config.IsChecked("wKS") && entKs != null)
                {
                    if(Spells.W.IsReady() && CardSelector.Status == SelectStatus.Ready)
                    {
                        CardSelector.StartSelecting(Cards.First);

                    }else if(CardSelector.Status == SelectStatus.Selecting)
                    {
                        CardSelector.GoToKey(Cards.First);
                    }
                }else
                {
                    if (Config.IsChecked("wCGold"))
                    {
                        if (Spells.W.IsReady() && CardSelector.Status == SelectStatus.Ready)
                        {
                            CardSelector.StartSelecting(Cards.Yellow);

                        }
                        else if (CardSelector.Status == SelectStatus.Selecting)
                        {
                            CardSelector.GoToKey(Cards.Yellow);
                        }
                    }
                }
            }


        }

        #endregion
    }
}