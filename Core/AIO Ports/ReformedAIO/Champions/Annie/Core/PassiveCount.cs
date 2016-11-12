using System.Linq;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Annie.Core
{
    using LeagueSharp;

    class PassiveCount
    {
        public int StunCount
        {
            get
            {
                return ObjectManager.Player.Buffs.Where(buff => buff.Name == "pyromania"
                || buff.Name == "pyromania_particle").Select(buff => buff.Name == "pyromania"
                ? buff.Count
                : 4).FirstOrDefault();
            }
        }
    }
}
