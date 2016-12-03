using EloBuddy; 
using LeagueSharp.Common; 
namespace myAnnie.Manager.Events.Games.Mode
{
    using Spells;
    using System.Linq;
    using myCommon;
    using LeagueSharp.Common;

    internal class Harass : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                if (Menu.GetBool("HarassQLH") && Q.IsReady() && !SpellManager.HaveStun)
                {
                    var qMinion =
                        MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly)
                            .FirstOrDefault(x => x.Health < Q.GetDamage(x));

                    if (qMinion != null && qMinion.IsValidTarget(Q.Range) && Me.CountEnemiesInRange(Q.Range) == 0)
                    {
                        Q.CastOnUnit(qMinion, true);
                    }
                }

                if (Menu.GetBool("HarassQ") && Q.IsReady())
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                    if (target.Check(Q.Range))
                    {
                        Q.CastOnUnit(target, true);
                    }
                }

                if (Menu.GetBool("HarassW") && W.IsReady())
                {
                    var target = TargetSelector.GetTarget(W.Range - 50, TargetSelector.DamageType.Magical);

                    if (target.Check(W.Range))
                    {
                        W.Cast(target.Position, true);
                    }
                }
            }
        }
    }
}
