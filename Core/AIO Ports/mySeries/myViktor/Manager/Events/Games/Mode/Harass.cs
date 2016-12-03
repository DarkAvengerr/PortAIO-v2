using EloBuddy; 
using LeagueSharp.Common; 
namespace myViktor.Manager.Events.Games.Mode
{
    using Spells;
    using myCommon;
    using LeagueSharp.Common;

    internal class Harass : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("HarassMana")))
            {
                if (Menu.GetBool("HarassE") && E.IsReady())
                {
                    var target = TargetSelector.GetTarget(E.Range + EWidth, TargetSelector.DamageType.Magical);

                    if (target.Check(E.Range + EWidth))
                    {
                        SpellManager.CastE(target);
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
            }
        }
    }
}
