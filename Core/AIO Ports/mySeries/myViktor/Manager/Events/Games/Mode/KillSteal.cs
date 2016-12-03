using EloBuddy; 
using LeagueSharp.Common; 
namespace myViktor.Manager.Events.Games.Mode
{
    using Spells;
    using myCommon;
    using System.Linq;
    using LeagueSharp.Common;

    internal class KillSteal : Logic
    {
        internal static void Init()
        {
            if (Menu.GetBool("KillStealQ") && Q.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(x => x.Check(Q.Range) && x.Health < DamageCalculate.GetQDamage(x)))
                {
                    if (target.IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(target, true);
                    }
                }
            }

            if (Menu.GetBool("KillStealE") && E.IsReady())
            {
                foreach (
                  var target in
                  HeroManager.Enemies.Where(x => x.Check(E.Range + EWidth) && x.Health < DamageCalculate.GetEDamage(x)))
                {
                    if (target.IsValidTarget(E.Range + EWidth - 100))
                    {
                        SpellManager.CastE(target);
                    }
                }
            }
        }
    }
}
