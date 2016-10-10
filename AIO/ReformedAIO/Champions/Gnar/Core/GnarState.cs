using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.Core
{
    using System.Linq;

    internal sealed class GnarState
    {
        public bool Mini => Vars.Player.CharData.BaseSkinName == "Gnar";
        
        public bool Mega => Vars.Player.CharData.BaseSkinName == "gnarbig";

        public bool TransForming => Vars.Player.Buffs.Any(x => x.DisplayName.Contains("gnartransformsoon"))
            || (Mini && Vars.Player.ManaPercent >= 95)
            || (Mega && Vars.Player.ManaPercent <= 10);   
    }
}
