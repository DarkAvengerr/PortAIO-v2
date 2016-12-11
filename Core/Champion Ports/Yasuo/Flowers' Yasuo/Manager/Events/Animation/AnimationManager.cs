using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo.Manager.Events
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using Orbwalking = Orbwalking;

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

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
                {
                    return;
                }

                if (Args.Animation == "1df607e5")
                {
                    Orbwalker.SetAttack(false);
                    LeagueSharp.Common.Utility.DelayAction.Add(300 + Game.Ping, () =>
                    {
                        Orbwalking.ResetAutoAttackTimer();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Me.Position.Extend(Game.CursorPos, +10));
                        Orbwalker.SetAttack(true);
                    });
                }
            }
        }
    }
}