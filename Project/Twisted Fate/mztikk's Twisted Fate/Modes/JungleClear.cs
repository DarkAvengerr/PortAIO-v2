using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksTwistedFate.Modes
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikksTwistedFate.Config;

    internal static class JungleClear
    {
        #region Methods

        internal static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < Config.GetSliderValue("manaToJC"))
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

            if (Config.IsChecked("useWinJC") && Spells.W.IsReady())
            {
                switch (Config.GetStringListValue("wModeJC"))
                {
                    case 0:
                        if (jungle.Any(x => x.Name.StartsWith("SRU_Baron") || x.Name.StartsWith("SRU_Dragon")))
                        {
                            CardSelector.StartSelecting(Cards.Blue);
                        }
                        else
                        {
                            var combinedManaPercent = ObjectManager.Player.MaxMana
                                                      / (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana
                                                         + ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana);
                            if (ObjectManager.Player.ManaPercent
                                >= Math.Max(45, Config.GetSliderValue("manaToJC") + 10 + combinedManaPercent))
                            {
                                var targetAoE = jungle.Count(x => x.Distance(jungle.FirstOrDefault()) <= 250);
                                if (targetAoE > 2)
                                {
                                    CardSelector.StartSelecting(Cards.Red);
                                }
                                else
                                {
                                    // ReSharper disable once PossibleNullReferenceException
                                    if (jungle.FirstOrDefault().HealthPercent >= 40
                                        && ObjectManager.Player.HealthPercent < 75)
                                    {
                                        CardSelector.StartSelecting(Cards.Yellow);
                                    }
                                    else
                                    {
                                        CardSelector.StartSelecting(Cards.Blue);
                                    }
                                }

                                // CardSelector.StartSelecting(targetAoE > 2 ? Cards.Red : Cards.Yellow);
                            }
                            else
                            {
                                CardSelector.StartSelecting(Cards.Blue);
                            }
                        }

                        break;
                    case 1:
                        CardSelector.StartSelecting(Cards.Yellow);
                        break;
                    case 2:
                        CardSelector.StartSelecting(Cards.Blue);
                        break;
                    case 3:
                        CardSelector.StartSelecting(Cards.Red);
                        break;
                }
            }

            if (Config.IsChecked("useQinJC") && Spells.Q.IsReady())
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