using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Arcane_Ryze.Modes
{
    class BeforeAA : Core
    {
        public static void OnAction(object sender, OrbwalkingActionArgs e)
        {
            if(Variables.Orbwalker.ActiveMode == OrbwalkingMode.Hybrid && e.Type == OrbwalkingType.BeforeAttack)
            {
                if(Spells.Q.IsReady() && Target.IsValidTarget() && !Target.IsZombie && PassiveStack < 4)
                {
                    Spells.Q.Cast(Target);  
                }
            }
        }
    }
}
