using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Data
{
    using System.Collections.Generic;

    class Debuffs
    {
        public static List<DebuffStruct> debuffs = new List<DebuffStruct>
        {
                                       
        };

        public enum PotionMode
        {
            Instant, Delayed
        }

        public struct DebuffStruct
        {
            public readonly string BuffName;

            public readonly int DamageValue;

            public readonly int Time;

            public DebuffStruct(string buffName, int time, int damageValue)
            {
                this.BuffName = buffName;
                this.Time = time;
                this.DamageValue = damageValue;
            }
        }
    }
}
