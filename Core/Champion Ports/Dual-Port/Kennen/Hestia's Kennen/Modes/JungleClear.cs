using System.Linq;
using Kennen.Core;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kennen.Modes
{
    internal class JungleClear
    {
        public static void ExecuteJungleClear()
        {
            var castQ = Configs.config.Item("useQj").GetValue<bool>() && Spells.Q.IsReady();
            var castW = Configs.config.Item("useWj").GetValue<bool>() && Spells.W.IsReady();

            if (!Orbwalking.CanMove(50))
                return;

            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var minionsW = minions.Where(Champion.Kennen.HasMark).Count();

            if (minions.Count > 0)
            {
                var minion = minions[0];

                if (castQ)
                {
                    Spells.Q.CastSpell(minion, "predMode", "hitchanceQ");
                }
            }

            if (minionsW > 0)
            {
                if (castW)
                {
                    Spells.W.Cast();
                }
            }
        }
    }
}
