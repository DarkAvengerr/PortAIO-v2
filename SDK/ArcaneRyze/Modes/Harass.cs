#region

using LeagueSharp.SDK;
using static Arcane_Ryze.Core;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Arcane_Ryze.Modes
{
    internal class Harass
    {
        public static void HarassLogic()
        {
            if(Target.IsValidTarget() && Target != null)
            {
                if(PassiveStack < 4)
                {
                    if(Spells.E.IsReady())
                    {
                        Spells.E.Cast(Target);
                    }
                    else if(Spells.W.IsReady())
                    {
                        Spells.W.Cast(Target);
                    }
                }
            }
        }
    }
}
