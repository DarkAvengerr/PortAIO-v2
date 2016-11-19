using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.Core
{
    using System.Linq;

    using LeagueSharp;

    internal sealed class GnarState
    {
        public bool Mini => ObjectManager.Player.CharData.BaseSkinName == "Gnar";
        
        public bool Mega => ObjectManager.Player.CharData.BaseSkinName == "gnarbig";

        public bool TransForming => ObjectManager.Player.Buffs.Any(x => x.DisplayName.Contains("gnartransformsoon"))
            || (Mini && ObjectManager.Player.ManaPercent >= 95)
            || (Mega && ObjectManager.Player.ManaPercent <= 10);   
    }
}
