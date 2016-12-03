using EloBuddy; 
using LeagueSharp.Common; 
namespace myAnnie.Manager.Events.Games
{
    using Spells;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using myCommon;
    

    internal class FlashR : Logic
    {
        internal static void Init()
        {
            if (Menu.GetKey("EnableFlashR"))
            {
                Orbwalking.MoveTo(Game.CursorPos);

                foreach (var target in HeroManager.Enemies.Where(em => em.Check(R.Range + 425f)))
                {
                    if (E.IsReady() && !SpellManager.HaveStun)
                    {
                        E.Cast();
                    }

                    var RPred = R.GetPrediction(target, true);
                    var RHitCount = R.GetPrediction(target, true).AoeTargetsHitCount;

                    if (RPred.Hitchance >= HitChance.High && SpellManager.HaveStun)
                    {
                        if (Me.CountAlliesInRange(1000) >= Menu.GetSlider("FlashRCountsAlliesinRange") &&
                            Me.CountEnemiesInRange(R.Range + 425) >= Menu.GetSlider("FlashRCountsEnemiesinRange") &&
                            RHitCount >= Menu.GetSlider("FlashRCountsEnemiesinRange"))
                        {
                            Me.Spellbook.CastSpell(Flash, R.GetPrediction(target, true).CastPosition);
                            if (!Flash.IsReady())
                            {
                                R.Cast(R.GetPrediction(target, true).CastPosition, true);
                            }
                        }

                        if (Menu.GetBool("FlashRCanKillEnemy") && Me.CountEnemiesInRange(R.Range + 425) == 1 && Q.IsReady() &&
                            Ignite.IsReady() && target.Health < DamageCalculate.GetComboDamage(target))
                        {
                            Me.Spellbook.CastSpell(Flash, R.GetPrediction(target, true).CastPosition);
                            if (!Flash.IsReady())
                            {
                                R.Cast(R.GetPrediction(target, true).CastPosition, true); Me.Spellbook.CastSpell(Ignite, target); Q.Cast(target, true);
                            }
                        }
                        if (Menu.GetBool("FlashRCanKillEnemy") && Me.CountEnemiesInRange(R.Range + 425) == 1 && Q.IsReady() &&
                            target.Health < DamageCalculate.GetComboDamage(target))
                        {
                            Me.Spellbook.CastSpell(Flash, R.GetPrediction(target, true).CastPosition);
                            if (!Flash.IsReady())
                            {
                                R.Cast(R.GetPrediction(target, true).CastPosition, true);
                                Q.Cast(target, true);
                            }
                        }
                    }
                }
            }
        }
    }
}