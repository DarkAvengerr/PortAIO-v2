using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Events.Games.Mode
{
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Burst : Logic
    {
        internal static void Init()
        {
            var ForcusTarget = TargetSelector.GetSelectedTarget();
            var target = ForcusTarget;

            if (target != null && !target.IsDead && target.IsValidTarget() && !target.IsZombie)
            {
                if (R.IsReady() && R.Instance.Name == "RivenFengShuiEngine")
                {
                    if (Q.IsReady() && E.IsReady() &&
                        W.IsReady() &&
                        target.Distance(Me.ServerPosition) < E.Range + Me.AttackRange + 100)
                    {
                        E.Cast(target.Position);
                    }

                    if (E.IsReady() &&
                        target.Distance(Me.ServerPosition) < Me.AttackRange + E.Range + 100)
                    {
                        R.Cast();
                        E.Cast(target.Position);
                    }
                }

                if (W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                }

                if ((qStack == 1 || qStack == 2 || target.HealthPercent < 50) && R.Instance.Name == "RivenIzunaBlade")
                {
                    R.Cast(target.ServerPosition);
                }

                if (Menu.GetBool("BurstIgnite") && Ignite != SpellSlot.Unknown && Ignite.IsReady())
                {
                    if (target.HealthPercent < 50)
                    {
                        Me.Spellbook.CastSpell(Ignite, target);
                    }
                }

                if (Menu.GetBool("BurstFlash") && Flash != SpellSlot.Unknown)
                {
                    if (Flash.IsReady() && R.IsReady() &&
                        R.Instance.Name == "RivenFengShuiEngine" && E.IsReady() &&
                        W.IsReady() && target.Distance(Me.ServerPosition) <= E.Range + 425f &&
                        target.Distance(Me.ServerPosition) >= E.Range + Me.AttackRange + 85)
                    {
                        R.Cast();
                        E.Cast(target.Position);
                        LeagueSharp.Common.Utility.DelayAction.Add(150,
                            () => { Me.Spellbook.CastSpell(Flash, target.Position); });
                    }
                }
            }
        }
    }
}
