using EloBuddy; 
using LeagueSharp.Common; 
namespace myViktor.Manager.Events
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using myCommon;
    using Spells;

    internal class SpellCastManager : Logic
    {
        internal static void Init(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Menu.GetBool("BrokenW") && sender.IsEnemy && W.IsReady() && sender.IsValidTarget(W.Range))
            {
                if (SpellManager.CastWTargetPos(Args.SData.Name))
                {
                    W.Cast(sender.ServerPosition, true);
                }

                if (SpellManager.CastWMePos(Args.SData.Name))
                {
                    if (Args.End.Distance(Me.Position) <= 100)
                    {
                        W.Cast(Me.Position, true);
                    }
                }
            }
        }
    }
}