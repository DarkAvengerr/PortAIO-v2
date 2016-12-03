using EloBuddy; 
using LeagueSharp.Common; 
namespace myAnnie.Manager.Events.Games
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using Spells;
    using myCommon;

    internal class AutoFollow : Logic
    {
        internal static void Init()
        {
            if (Menu.GetBool("AutoFollow") && SpellManager.HaveBear)
            {
                var e = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Magical);

                if (e != null && e.IsValidTarget(2000) && !e.IsZombie)
                {
                    if (Game.Time > clickTime + 1.5)
                    {
                        R.Cast(e);
                        clickTime = Game.Time;
                    }
                }
                else if (e == null && Game.Time > clickTime + 1.5)
                {
                    R.Cast(Me.ServerPosition);
                    clickTime = Game.Time;
                }
            }
        }
    }
}