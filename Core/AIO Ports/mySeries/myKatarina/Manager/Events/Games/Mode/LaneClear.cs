using EloBuddy; 
using LeagueSharp.Common; 
namespace myKatarina.Manager.Events.Games.Mode
{
    using Spells;
    using myCommon;
    using System.Linq;
    using LeagueSharp.Common;

    internal class LaneClear : Logic
    {
        internal static void Init()
        {
            if (SpellManager.isCastingUlt || !ManaManager.SpellFarm)
            {
                return;
            }

            if (Menu.GetBool("LaneClearQ") && Q.IsReady())
            {
                if (Menu.GetBool("LaneClearQLH"))
                {
                    var qMinion =
                        MinionManager.GetMinions(Me.Position, Q.Range)
                            .FirstOrDefault(x => x.Health < DamageCalculate.GetQDamage(x));

                    if (qMinion != null && qMinion.IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(qMinion, true);
                    }
                }
                else
                {
                    var qMinions = MinionManager.GetMinions(Me.Position, Q.Range);

                    if (qMinions.Count >= 2)
                    {
                        Q.CastOnUnit(qMinions.FirstOrDefault(), true);
                    }
                }
            }

            if (Menu.GetBool("LaneClearW") && W.IsReady())
            {
                var wMinions = MinionManager.GetMinions(Me.Position, W.Range);

                if (wMinions.Count >= Menu.GetSlider("LaneClearWCount"))
                {
                    W.Cast();
                }
            }
        }
    }
}
