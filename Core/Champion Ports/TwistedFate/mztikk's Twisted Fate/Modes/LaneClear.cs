using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksTwistedFate.Modes
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Config = mztikksTwistedFate.Config;

    internal static class LaneClear
    {
        #region Methods

        internal static void Execute()
        {
            if (ObjectManager.Player.ManaPercent < Config.GetSliderValue("manaToLC"))
            {
                return;
            }

            if (Config.IsChecked("useWinLC") && Spells.W.IsReady())
            {
                var targetMinion = Mainframe.Orbwalker.GetTarget() as Obj_AI_Minion;
                if (targetMinion != null)
                {
                    switch (Config.GetStringListValue("wModeLC"))
                    {
                        case 0:
                            var combinedManaPercent = ObjectManager.Player.MaxMana
                                                      / (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana
                                                         + ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana);
                            if (ObjectManager.Player.ManaPercent
                                >= Math.Max(45, Config.GetSliderValue("manaToLC") + 10 + combinedManaPercent))
                            {
                                /*var targetAoE =
                                    ObjectManager.Get<Obj_AI_Minion>()
                                        .Where(x => x.IsEnemy)
                                        .Count(x => x.Distance(targetMinion) <= 250);*/
                                var targetAoE = MinionManager.GetMinions(targetMinion.Position, 250).Count;
                                if (targetAoE > 2)
                                {
                                    CardSelector.StartSelecting(Cards.Red);
                                }
                            }
                            else
                            {
                                CardSelector.StartSelecting(Cards.Blue);
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
            }

            if (Config.IsChecked("useQinLC") && Spells.Q.IsReady())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range).Where(x => x.Type == GameObjectType.obj_AI_Minion && x.Team != ObjectManager.Player.Team).ToList();
                if (!minions.Any() || minions.Count < 1)
                {
                    return;
                }

                var minionPos = minions.Select(x => x.Position.To2D()).ToList();
                var farm = MinionManager.GetBestLineFarmLocation(minionPos, Spells.Q.Width, Spells.Q.Range);
                if (farm.MinionsHit >= Config.GetSliderValue("qTargetsLC"))
                {
                    Spells.Q.Cast(farm.Position);
                }
            }
        }

        #endregion
    }
}