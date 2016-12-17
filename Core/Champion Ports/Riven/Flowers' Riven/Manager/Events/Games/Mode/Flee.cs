using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Events.Games.Mode
{
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using myCommon;

    internal class Flee : Logic
    {
        internal static void Init()
        {
            if (
                HeroManager.Enemies.Any(
                    x => x.DistanceToPlayer() <= W.Range && !x.HasBuffOfType(BuffType.SpellShield)) && W.IsReady())
            {
                W.Cast(true);
            }

            if (E.IsReady() && !Me.IsDashing() 
                && ((!Q.IsReady() && qStack == 0) || (Q.IsReady() && qStack == 2)) && Me.CanMoveMent())
            {
                E.Cast(Me.Position.Extend(Game.CursorPos, E.Range), true);
            }

            if (Q.IsReady() && !Me.IsDashing() && Me.CanMoveMent())
            {
                Q.Cast(Me.Position.Extend(Game.CursorPos, 350f), true);
            }
        }
    }
}
