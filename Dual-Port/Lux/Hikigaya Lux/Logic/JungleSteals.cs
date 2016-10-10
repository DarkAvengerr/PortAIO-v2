using System.Linq;
using Hikigaya_Lux.Core;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Hikigaya_Lux.Logic
{
    static class JungleSteals
    {
        private static int JungSkill()
        {
            return LuxMenu.Config.Item("jungle.steal.skill").GetValue<StringList>().SelectedIndex == 0 ? 1 : 2;
        }

        public static string JungKey()
        {
            return LuxMenu.Config.Item("jungle.steal.skill").GetValue<StringList>().SelectedIndex == 0 ? "Q" : "E";
        }

        private static readonly string[] JungleMinions = new string[] { "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "SRU_Red", "SRU_Krug", "SRU_Dragon","SRU_Baron", "Sru_Crab"};

        private static bool IsSteableMonster(string name)
        {
            return JungleMinions.Contains(name);
        }

        private static void JungleStealWithQ()
        {
            var minions = MinionManager.GetMinions(Spells.Q.Range, MinionTypes.All, MinionTeam.Neutral);
            foreach (var minion in minions.Where(minion => IsSteableMonster(minion.Name) && minion.Health <= Spells.Q.GetDamage(minion)))
            {
                Spells.Q.Cast(minion);
                break;
            }
        }

        private static void JungleStealWithE()
        {
            var minions = MinionManager.GetMinions(Spells.E.Range, MinionTypes.All, MinionTeam.Neutral);
            foreach (var minion in minions.Where(minion => IsSteableMonster(minion.Name) && minion.Health <= Spells.E.GetDamage(minion)))
            {
                Spells.E.Cast(minion);
                if (Helper.LuxE == null || Helper.EInsCheck() != 2) continue;
                Spells.E.Cast();
                break;
            }
        }

        public static void JungleStealz()
        {
            if (JungSkill() == 0)
            {
                JungleStealWithQ();
            }
            else
            {
                JungleStealWithE();
            }
        }

    }
}
