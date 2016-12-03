using EloBuddy; 
using LeagueSharp.Common; 
namespace myViktor.Manager.Events.Games.Mode
{
    using myCommon;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class LastHit : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("LastHitMana")))
            {
                if (Menu.GetBool("LastHitQ") && Q.IsReady())
                {
                    var qMinions =
                        MinionManager.GetMinions(Me.Position, Q.Range)
                            .FirstOrDefault(
                                x => x.Health > Me.GetAutoAttackDamage(x) && x.Health < DamageCalculate.GetQDamage(x));

                    if (qMinions != null && qMinions.IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(qMinions, true);
                    }
                }
            }
        }
    }
}
