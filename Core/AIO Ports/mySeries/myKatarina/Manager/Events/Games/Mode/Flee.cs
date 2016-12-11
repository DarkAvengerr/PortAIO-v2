using EloBuddy; 
using LeagueSharp.Common; 
namespace myKatarina.Manager.Events.Games
{
    using LeagueSharp.Common;
    using myCommon;
    using LeagueSharp;
    using Spells;

    internal class Flee : Logic
    {
        internal static void Init()
        {
            if (Menu.GetBool("FleeW") && W.IsReady())
            {
                W.Cast();
            }

            if (Menu.GetBool("FleeE") && E.IsReady())
            {
                var nearest = SpellManager.badaoFleeLogic.MinOrDefault(x => x.Position.Distance(Game.CursorPos));

                if (nearest != null && nearest.Position.DistanceToMouse() < Me.DistanceToMouse() &&
                    nearest.Position.DistanceToPlayer() > 300)
                {
                    var pos = nearest.Position.To2D().Extend(Game.CursorPos.To2D(), 150);
                    E.Cast(pos);
                }
            }
        }
    }
}