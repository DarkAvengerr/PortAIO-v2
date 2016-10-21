using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using SneakySkarner.Logging;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SneakySkarner.Utility
{
    class DamageBuffer
    {
        private List<float> damageList { get; set; }
        private float maxPercentHealth { get; set; }
        private float bufferStartTime { get; set; }

        public DamageBuffer(float bufferCapPercent)
        {
            maxPercentHealth = bufferCapPercent;
            damageList = new List<float>();
            
        }

        public bool IsBufferOverload(float damage, float currentHealth, float maxHealth)
        {
            if (damageList.Count < 1)
            {
                bufferStartTime = Game.Time;
            }
            damageList.Add(damage);
            Logger.PrintDebug((Game.Time - bufferStartTime).ToString());
            if (!((Game.Time - bufferStartTime) > 0.35)) return false;
            if (TotalDamageInBuffer > (maxHealth*maxPercentHealth))
            {
                damageList.Clear();
                Logger.PrintDebug("DamageBuffer overload! Shielding...");
                return true;
            }
            damageList.Clear();
            return false;
        }

        public float TotalDamageInBuffer
        {
            get { return damageList.Aggregate<float, float>(0, (current, dmg) => current + dmg); }
        }
    }
}
