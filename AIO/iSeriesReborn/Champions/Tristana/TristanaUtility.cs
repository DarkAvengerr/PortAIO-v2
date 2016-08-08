using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iSeriesReborn.Utility;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Tristana
{
    class TristanaUtility
    {
        public static float GetERRange()
        {
            return 550 + (7 * (ObjectManager.Player.Level - 1));
        }

        public static BuffInstance GetEBuff(Obj_AI_Base target)
        {
            return target.Buffs.FirstOrDefault(x => x.DisplayName == "TristanaECharge" || x.Name == "TristanaECharge");
        }

        public static bool HasE(Obj_AI_Base target)
        {
            return GetEBuff(target) != null;
        }

        public static float GetRDamage(Obj_AI_Base target)
        {
            return (Variables.spells[SpellSlot.E].GetDamage(target) * ((0.30f * GetEBuff(target).Count) + 1f) + Variables.spells[SpellSlot.R].GetDamage(target));
        }
    }
}
