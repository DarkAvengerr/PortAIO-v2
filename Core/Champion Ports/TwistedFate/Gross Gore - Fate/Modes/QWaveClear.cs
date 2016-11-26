#region Use
using System;
using System.Windows.Input;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common; 
#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate.Modes
{
    using Config = GrossGoreTwistedFate.Config;

    internal static class QWaveClear
    {
        #region Methods

        internal static void Execute()
        {
            var qMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;

            if (Config.UseClearQ)
            {
                if(Spells._q.IsReadyPerfectly())
                {
                    if(ObjectManager.Player.Mana >= qMana)
                    {
                        var minions = SebbyLib.Cache.GetMinions(ObjectManager.Player.ServerPosition, Spells._q.Range);

                        if (minions.Any() && minions.Count > 2)
                        {
                            var minionPos = minions.Select(x => x.Position.To2D()).ToList();

                            var farm = MinionManager.GetBestLineFarmLocation(minionPos, Spells._q.Width, Spells._q.Range);

                            if (farm.MinionsHit >= Config.ClearQCount)
                            {
                                Spells._q.Cast(farm.Position);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}