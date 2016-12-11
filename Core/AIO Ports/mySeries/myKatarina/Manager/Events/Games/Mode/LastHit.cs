using EloBuddy; 
using LeagueSharp.Common; 
namespace myKatarina.Manager.Events.Games.Mode
{
    using Spells;
    using myCommon;
    using System.Linq;
    using LeagueSharp.Common;

    internal class LastHit : Logic
    {
        internal static void Init()
        {
            if (SpellManager.isCastingUlt)
            {
                return;
            }

            if (Menu.GetBool("LastHitQ") && Q.IsReady())
            {
                var qMinion =
                    MinionManager.GetMinions(Me.Position, Q.Range)
                        .FirstOrDefault(x => x.Health < DamageCalculate.GetQDamage(x));

                if (qMinion != null && qMinion.IsValidTarget(Q.Range))
                {
                    Q.CastOnUnit(qMinion, true);
                }
            }
        }
    }
}
