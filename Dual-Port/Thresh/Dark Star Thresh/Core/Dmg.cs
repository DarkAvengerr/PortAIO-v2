using System;
using LeagueSharp;
using LeagueSharp.Common;
using System.Linq;
using LeagueSharp.Common.Data;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Dark_Star_Thresh.Core
{
    class Dmg
    {
        public static float Damage(Obj_AI_Base target)
        {
            if (target == null || target.IsDead || target.IsInvulnerable) return 0;

            float dmg = 0;

            if (ObjectManager.Player.HasBuff("SummonerExhaust")) dmg = dmg * 0.6f;

            if (Spells.Q.IsReady()) dmg = dmg + Spells.Q.GetDamage(target);

            if (Spells.E.IsReady()) dmg = dmg + Spells.E.GetDamage(target);

            if (Spells.R.IsReady()) dmg = dmg + Spells.R.GetDamage(target);

            return dmg;
        }
       
        public static int StackDmg(Obj_AI_Base target)
        {
            if (target == null || target.IsDead || TalentReaper <= 1) return 0;

            int dmg = 0;

            if ( LeagueSharp.Common.Data.ItemData.Relic_Shield.GetItem().IsOwned())
            {
                dmg = 200 + ( 5 * ObjectManager.Player.Level);
            }

            if (LeagueSharp.Common.Data.ItemData.Targons_Brace.GetItem().IsOwned())
            {
                dmg =  210 + ( 10 * ObjectManager.Player.Level);
            }

            if (LeagueSharp.Common.Data.ItemData.Face_of_the_Mountain.GetItem().IsOwned() || LeagueSharp.Common.Data.ItemData.Eye_of_the_Equinox.GetItem().IsOwned())
            {
                dmg = 340 + ( 20 * ObjectManager.Player.Level);
            }

            return dmg;
        }

        public static int TalentReaper
        {
            get
            {
                var data = ObjectManager.Player.Buffs.FirstOrDefault(b => b.DisplayName == "TalentReaper");
                return data == null ? 0 : data.Count;
            }
        }
    }
}
