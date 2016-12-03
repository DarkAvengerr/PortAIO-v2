using EloBuddy; 
using LeagueSharp.Common; 
namespace myRyze.Manager.Events.Games.Mode
{
    using myCommon;
    using System.Linq;
    using LeagueSharp.Common;

    internal class KillSteal : Logic
    {
        internal static void Init()
        {
            if (Menu.Item("KillStealQ", true).GetValue<bool>() && Q.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x =>
                            x.Check(Q.Range) && x.Health < Q.GetDamage(x) &&
                            Q.GetPrediction(x).Hitchance >= HitChance.VeryHigh))
                {
                    Q.Cast(Q.GetPrediction(target).CastPosition, true);
                }
            }

            if (Menu.Item("KillStealW", true).GetValue<bool>() && W.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x =>
                            x.Check(W.Range) && x.Health < W.GetDamage(x)))
                {
                    W.CastOnUnit(target, true);
                }
            }

            if (Menu.Item("KillStealE", true).GetValue<bool>() && E.IsReady())
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x =>
                            x.Check(E.Range) && x.Health < E.GetDamage(x)))
                {
                    E.CastOnUnit(target, true);
                }
            }
        }
    }
}
