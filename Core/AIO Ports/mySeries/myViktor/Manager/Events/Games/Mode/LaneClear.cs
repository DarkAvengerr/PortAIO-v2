using EloBuddy; 
using LeagueSharp.Common; 
namespace myViktor.Manager.Events.Games.Mode
{
    using Spells;
    using myCommon;
    using System.Linq;
    using LeagueSharp.Common;

    internal class LaneClear : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("LaneClearMana")) && ManaManager.SpellFarm)
            {
                if (Menu.GetBool("LaneClearQ") && Q.IsReady())
                {
                    var qMinion =
                        MinionManager.GetMinions(Me.Position, Q.Range)
                            .FirstOrDefault(
                                x => x.Health < DamageCalculate.GetQDamage(x) && x.Health > Me.GetAutoAttackDamage(x));

                    if (qMinion != null && qMinion.IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(qMinion, true);
                    }
                }

                if (Menu.GetBool("LaneClearE") && E.IsReady())
                {
                    var eMinions = MinionManager.GetMinions(Me.Position, E.Range + EWidth);

                    if (eMinions.Any())
                    {
                        var eMinion = eMinions.FirstOrDefault(x => x.IsValidTarget(E.Range));
                        var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

                        if (target != null && target.IsValidTarget(E.Range))
                        {
                            var exEMinions = MinionManager.GetMinions(target.Position, E.Range);
                            var eFarm =
                                SpellManager.GetBestLineFarmLocation(
                                    exEMinions.Select(x => x.Position.To2D()).ToList(), target.Position, E.Width, EWidth);

                            if (eFarm.MinionsHit+1 >= Menu.GetSlider("LaneClearECount"))
                            {
                                SpellManager.FixECast(target.Position, eFarm.Position.To3D());
                            }
                        }
                        else if (eMinion != null)
                        {
                            var exEMinions = MinionManager.GetMinions(eMinion.Position, E.Range);
                            var eFarm =
                                SpellManager.GetBestLineFarmLocation(
                                    exEMinions.Select(x => x.Position.To2D()).ToList(), eMinion.Position, E.Width, EWidth);

                            if (eFarm.MinionsHit >= Menu.GetSlider("LaneClearECount"))
                            {
                                SpellManager.FixECast(eMinion.Position, eFarm.Position.To3D());
                            }
                        }
                    }
                }
            }
        }
    }
}
