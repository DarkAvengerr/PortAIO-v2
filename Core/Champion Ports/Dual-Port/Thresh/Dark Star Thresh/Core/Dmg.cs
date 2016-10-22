using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Dark_Star_Thresh.Core
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class Dmg
    {
        public float Damage(Obj_AI_Base target)
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

            var dmg = 0;

            if ( ItemData.Relic_Shield.GetItem().IsOwned())
            {
                dmg = 200 + 5 * ObjectManager.Player.Level;
            }

            if (ItemData.Targons_Brace.GetItem().IsOwned())
            {
                dmg =  210 + 10 * ObjectManager.Player.Level;
            }

            if (ItemData.Face_of_the_Mountain.GetItem().IsOwned() || ItemData.Eye_of_the_Equinox.GetItem().IsOwned())
            {
                dmg = 340 + 20 * ObjectManager.Player.Level;
            }

            return dmg;
        }

        public static int TalentReaper
        {
            get
            {
                var data = ObjectManager.Player.Buffs.FirstOrDefault(b => b.DisplayName == "TalentReaper");
                return data?.Count ?? 0;
            }
        }
    }
}
