using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.KogMaw
{
    class KogUtils
    {
        public static float GetAARange()
        {
            if (ObjectManager.Player.LSHasBuff("kogmawbioarcanebarrage"))
            {
                return GetWRange();
            }

            return Orbwalking.GetRealAutoAttackRange(null) + 65f;
        }

        public static float GetWRange()
        {
            return 110f + 20f * ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level;
        }

        public static BuffInstance GetRBuff()
        {
            return
                ObjectManager.Player.Buffs
                    .FirstOrDefault(m => m.DisplayName.ToLower() == "kogmawlivingartillery");
        }

        public static float GetRCount()
        {
            return GetRBuff() != null ? GetRBuff().Count : -1;
        }
    }
}
