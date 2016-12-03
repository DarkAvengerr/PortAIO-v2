using EloBuddy; 
using LeagueSharp.Common; 
namespace myDiana.Manager.Events.Games.Mode
{
    using Spells;
    using System.Linq;
    using LeagueSharp;
    using myCommon;
    using LeagueSharp.Common;

    internal class JungleClear : Logic
    {
        internal static void Init()
        {
            if (ManaManager.HasEnoughMana(Menu.GetSlider("JungleClearmana")) && ManaManager.SpellFarm)
            {
                var mobs = MinionManager.GetMinions(Me.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);

                if (mobs.Count > 0)
                {
                    var bigmob = mobs.FirstOrDefault(x => !x.Name.ToLower().Contains("mini"));
                    var mob = mobs.FirstOrDefault();

                    if (Menu.GetBool("JungleClearQ") && Q.IsReady() && mob.IsValidTarget(Q.Range))
                    {
                        Q.Cast(mob, true);
                    }
               
                    if (Menu.GetBool("JungleClearW") && W.IsReady() && mob.IsValidTarget(W.Range))
                    {
                        W.Cast(true);
                    }

                    if (Menu.GetBool("JungleClearR") && R.IsReady() && bigmob.IsValidTarget(R.Range))
                    {
                        if (SpellManager.HaveQPassive(bigmob))
                        {
                            R.CastOnUnit(bigmob, true);
                        }
                        else if (bigmob != null && bigmob.Health < Me.GetSpellDamage(bigmob, SpellSlot.R) &&
                            bigmob.Health > Me.TotalAttackDamage * 2)
                        {
                            R.CastOnUnit(bigmob, true);
                        }
                    }
                }
            }
        }
    }
}
