using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate.Modes
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = GrossGoreTwistedFate.Config;

    internal static class JungleLogic
    {
        #region Methods

        internal static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < 10)
            {
                return;
            }

            var jungle =
                MinionManager.GetMinions(ObjectManager.Player.Position, ObjectManager.Player.AttackRange + 200, MinionTypes.All, MinionTeam.Neutral)
                    .Where(x => x.Team == GameObjectTeam.Neutral)
                    .OrderByDescending(x => x.MaxHealth);

            if (!jungle.Any() || jungle.FirstOrDefault() == null)
            {
                return;
            }

            if (Spells.W.IsReady())
            {
                if (jungle.Any(x => x.Name.StartsWith("SRU_Baron") || x.Name.StartsWith("SRU_Dragon")))
                {
                    CardSelector.StartSelecting(Cards.Blue);
                }
                else
                {
                    var combinedManaPercent = ObjectManager.Player.MaxMana
                                                / (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana
                                                    + ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana);

                    if (ObjectManager.Player.ManaPercent >= Math.Max(45, 20 + combinedManaPercent))
                    {
                        var targetAoE = jungle.Count(x => x.Distance(jungle.FirstOrDefault()) <= 250);

                        if (targetAoE > 2)
                        {
                            CardSelector.StartSelecting(Cards.Red);
                        }
                        else
                        {
                            if (jungle.FirstOrDefault().HealthPercent >= 40 && ObjectManager.Player.HealthPercent < 75)
                            {
                                CardSelector.StartSelecting(Cards.Yellow);
                            }
                            else
                            {
                                CardSelector.StartSelecting(Cards.Blue);
                            }
                        }
                    }
                    else
                    {
                        CardSelector.StartSelecting(Cards.Blue);
                    }
                }
                
            }

            if (ObjectManager.Player.ManaPercent >= 25 && Spells.Q.IsReady())
            {
                var target = jungle.FirstOrDefault(x => x.IsValidTarget(Spells.Q.Range));

                if (target != null)
                {
                    Spells.Q.Cast(target);
                }
            }
        }

        #endregion
    }
}