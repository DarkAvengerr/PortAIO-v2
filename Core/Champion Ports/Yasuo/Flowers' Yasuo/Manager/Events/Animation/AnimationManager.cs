using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo.Manager.Events
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class AnimationManager : Logic
    {
        internal static void Init(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs Args)
        {
            if (sender.IsMe)
            {
                if (Args.Animation == "Spell3")
                {
                    lastECast = Utils.TickCount;
                    isDashing = true;
                }
            }
        }
    }
}