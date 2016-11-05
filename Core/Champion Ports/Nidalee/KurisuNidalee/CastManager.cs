using System;
using SPrediction;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using KL = KurisuNidalee.KurisuLib;
using KN = KurisuNidalee.KurisuNidalee;
using EloBuddy;

namespace KurisuNidalee
{
    internal class CastManager
    {
        // Human Q Logic
        internal static void CastJavelin(Obj_AI_Base target, string mode)
        {
            if (target == null)
            {
                return;
            }
            // if not harass mode ignore mana check
            if (!KL.CatForm() && KL.CanUse(KL.Spells["Javelin"], true, mode))
            {
                if (mode != "ha" || KL.Player.ManaPercent > 65)
                { 
                    if (target.IsValidTarget(KL.Spells["Javelin"].Range))
                    {
                        // try prediction on champion
                        if (target.IsChampion())
                        {
                            var qoutput = KL.Spells["Javelin"].GetPrediction(target);

                            if (KN.Root.Item("ndhqcheck").GetValue<bool>())
                            {
                                switch (KN.Root.Item("ppred").GetValue<StringList>().SelectedValue)
                                {
                                    case "OKTW":
                                        var pi = new SebbyLib.Prediction.PredictionInput
                                        {
                                            Aoe = false,
                                            Collision = true,
                                            Speed = 1300f,
                                            Delay = 0.25f,
                                            Range = 1500f,
                                            From = KN.Player.ServerPosition,
                                            Radius = 40f,
                                            Unit = target,
                                            Type = SebbyLib.Prediction.SkillshotType.SkillshotLine
                                        };

                                        var po = SebbyLib.Prediction.Prediction.GetPrediction(pi);
                                        if (po.Hitchance == (SebbyLib.Prediction.HitChance) (KN.Root.Item("ndhqch").GetValue<StringList>().SelectedIndex + 3))
                                        {
                                            KL.Spells["Javelin"].Cast(po.CastPosition);
                                        }

                                        break;
                                    
                                    case "SPrediction":
                                        var so = KL.Spells["Javelin"].GetSPrediction((AIHeroClient) target);
                                        if (so.HitChance == (HitChance) (KN.Root.Item("ndhqch").GetValue<StringList>().SelectedIndex + 3))
                                        {
                                            KL.Spells["Javelin"].Cast(so.CastPosition);
                                        }
                                        break;

                                    case "Common":
                                        var co = KL.Spells["Javelin"].GetPrediction(target);
                                        if (co.Hitchance == (HitChance) (KN.Root.Item("ndhqch").GetValue<StringList>().SelectedIndex + 3))
                                        {
                                            KL.Spells["Javelin"].Cast(co.CastPosition);
                                        }
                                        break;
                                }
                            }

                            if (qoutput.Hitchance == HitChance.Collision && KL.Smite.IsReady())
                            {
                                if (KN.Root.Item("qsmcol").GetValue<bool>() && target.Health <= KL.CatDamage(target) * 3)
                                {
                                    if (qoutput.CollisionObjects.All(i => i.NetworkId != KL.Player.NetworkId))
                                    {
                                        var obj = qoutput.CollisionObjects.Cast<Obj_AI_Minion>().ToList();
                                        if (obj.Count == 1)
                                        {
                                            if (obj.Any(
                                                i =>
                                                    i.Health <= KL.Player.GetSummonerSpellDamage(i, Damage.SummonerSpell.Smite) &&
                                                    KL.Player.Distance(i) < 500 && KL.Player.Spellbook.CastSpell(KL.Smite, obj.First())))
                                            {
                                                KL.Spells["Javelin"].Cast(qoutput.CastPosition);
                                                return;
                                            }
                                        }
                                    }
                                }
                            }


                            if (!KN.Root.Item("ndhqcheck").GetValue<bool>())
                                KL.Spells["Javelin"].Cast(target);
                        }
                        else
                        {
                            KL.Spells["Javelin"].Cast(target);
                        }
                    }
                }
            }
        }

        // Human W Logic
        internal static void CastBushwhack(Obj_AI_Base target, string mode)
        {
            if (target == null)
            {
                return;
            }
            // if not harass mode ignore mana check
            if (!KL.CatForm() && KL.CanUse(KL.Spells["Bushwhack"], true, mode))
            {
                if (KL.Player.ManaPercent <= 65 && target.IsHunted() && target.CanMove)
                {
                    return;
                }

                if (mode != "ha" || KL.Player.ManaPercent > 65)
                {
                    if (target.IsValidTarget(KL.Spells["Bushwhack"].Range))
                    {
                        // try bushwhack prediction
                        if (KN.Root.Item("ndhwforce").GetValue<StringList>().SelectedIndex == 0)
                        {
                            if (target.IsChampion())
                                KL.Spells["Bushwhack"].CastIfHitchanceEquals(target, HitChance.VeryHigh);
                            else
                                KL.Spells["Bushwhack"].Cast(target.ServerPosition);
                        }

                        // try bushwhack behind target
                        if (KN.Root.Item("ndhwforce").GetValue<StringList>().SelectedIndex == 1)
                        {
                            var unitpos = KL.Spells["Bushwhack"].GetPrediction(target).UnitPosition;
                            KL.Spells["Bushwhack"].Cast(unitpos.Extend(KL.Player.ServerPosition, -75f));
                        }
                    }
                }
            }
        }


        // Cougar Q Logic
        internal static void CastTakedown(Obj_AI_Base target, string mode)
        {
            if (target == null)
            {
                return;
            }
            if (KL.CatForm() && KL.CanUse(KL.Spells["Takedown"], false, mode))
            {
                if (target.IsValidTarget(KL.Player.AttackRange + KL.Spells["Takedown"].Range))
                {
                    KL.Spells["Takedown"].CastOnUnit(target);
                }
            }
        }

        // Cougar W Logic
        internal static void CastPounce(Obj_AI_Base target, string mode)
        {
            if (target == null)
            {
                return;
            }
            // check the actual spell timer and if we have it enabled in our menu
            if (!KL.CatForm() || !KL.CanUse(KL.Spells["Pounce"], false, mode)) 
                return;

            // check if target is hunted in 750 range
            if (!target.IsValidTarget(KL.Spells["ExPounce"].Range))
                return;

            if (target.IsHunted())
            {
                // get hitbox
                var radius = KL.Player.AttackRange + KL.Player.Distance(KL.Player.BBox.Minimum) + 1;

                // force pounce if menu item enabled
                if (target.IsHunted() && KN.Root.Item("ndcwhunt").GetValue<bool>() ||

                    // or of target is greater than my attack range
                    target.Distance(KL.Player.ServerPosition) > radius ||

                    // or is jungling or waveclearing (without farm distance check)
                    mode == "jg" || mode == "wc" && !KN.Root.Item("ndcwdistwc").GetValue<bool>() ||

                    // or combo mode and ignoring distance check
                    !target.IsHunted() && mode == "co" && !KN.Root.Item("ndcwdistco").GetValue<bool>())
                {
                    if (KN.Root.Item("kitejg").GetValue<bool>() && mode == "jg" &&
                        target.Distance(Game.CursorPos) > 600 && target.Distance(KL.Player.ServerPosition) <= 300)
                    {
                        KL.Spells["Pounce"].Cast(Game.CursorPos);
                        return;
                    }

                    KL.Spells["Pounce"].Cast(target.ServerPosition);
                }
            }

            // if target is not hunted
            else
            {
                // check if in the original pounce range
                if (target.Distance(KL.Player.ServerPosition) > KL.Spells["Pounce"].Range)
                    return;

                // get hitbox
                var radius = KL.Player.AttackRange + KL.Player.Distance(KL.Player.BBox.Minimum) + 1;

                // check minimum distance before pouncing
                if (target.Distance(KL.Player.ServerPosition) > radius || 

                    // or is jungling or waveclearing (without distance checking)
                    mode == "jg" ||  mode == "wc" && !KN.Root.Item("ndcwdistwc").GetValue<bool>() ||

                    // or combo mode with no distance checking
                    mode == "co" && !KN.Root.Item("ndcwdistco").GetValue<bool>())
                {
                    if (target.IsChampion())
                    {
                        if (KN.Root.Item("ndcwcheck").GetValue<bool>())
                        {
                            var voutout = KL.Spells["Pounce"].GetPrediction(target);
                            if (voutout.Hitchance >= (HitChance) KN.Root.Item("ndcwch").GetValue<StringList>().SelectedIndex + 3)
                            {
                                KL.Spells["Pounce"].Cast(voutout.CastPosition);
                            }
                        }
                        else
                            KL.Spells["Pounce"].Cast(target.ServerPosition);
                    }
                    else 
                    {
                        // check pouncing near enemies
                        if (mode == "wc" && KN.Root.Item("ndcwene").GetValue<bool>() &&
                            target.ServerPosition.CountEnemiesInRange(550) > 0)
                            return;

                        // check pouncing under turret
                        if (mode == "wc" && KN.Root.Item("ndcwtow").GetValue<bool>() &&
                            target.ServerPosition.UnderTurret(true))
                            return;

                        KL.Spells["Pounce"].Cast(target.ServerPosition);
                    }
                }
            }
        }


        // Cougar E Logic
        internal static void CastSwipe(Obj_AI_Base target, string mode)
        {
            if (target == null)
            {
                return;
            }
            if (KL.CatForm() && KL.CanUse(KL.Spells["Swipe"], false, mode))
            {
                if (target.IsValidTarget(KL.Spells["Swipe"].Range))
                {
                    if (target.IsChampion())
                    {
                        if (KN.Root.Item("ndcecheck").GetValue<bool>())
                        {
                            var voutout = KL.Spells["Swipe"].GetPrediction(target);
                            if (voutout.Hitchance >= (HitChance) KN.Root.Item("ndcech").GetValue<StringList>().SelectedIndex + 3)
                            {
                                KL.Spells["Swipe"].Cast(voutout.CastPosition);
                            }
                        }
                        else
                            KL.Spells["Swipe"].Cast(target.ServerPosition);
                    }
                    else
                    {
                        // try aoe swipe if menu item > 1
                        var minhit = KN.Root.Item("ndcenum").GetValue<Slider>().Value;
                        if (minhit > 1 && mode == "wc")
                            KL.CastSmartSwipe();

                        // or cast normal
                        else
                            KL.Spells["Swipe"].Cast(target.ServerPosition);
                    }
                }
            }

            // check valid target in range
        }


        internal static void SwitchForm(Obj_AI_Base target, string mode)
        {
            if (target == null)
            {
                return;
            }
            // catform -> human
            if (KL.CatForm() && KL.CanUse(KL.Spells["Aspect"], false, mode))
            {
                if (!target.IsValidTarget(KL.Spells["Javelin"].Range))
                    return;

                // get hitbox
                var radius = KL.Player.AttackRange + KL.Player.Distance(KL.Player.BBox.Minimum) + 1;

                // dont switch if have Q buff and near target
                if (KL.CanUse(KL.Spells["Takedown"], true, mode) && KL.Player.HasBuff("Takedown") &&
                    target.Distance(KL.Player.ServerPosition) <= KL.Spells["Takedown"].Range + 65f)
                {
                    return;
                }

                // change form if Q is ready and meets hitchance
                if (target.IsChampion())
                {
                    if (KL.SpellTimer["Javelin"].IsReady())
                    {
                        var poutput = KL.Spells["Javelin"].GetPrediction(target);
                        if (poutput.Hitchance >= HitChance.High)
                        {
                            KL.Spells["Aspect"].Cast();
                        }
                    }
                }
                else
                {
                    // change to human if out of pounce range and can die
                    if (!KL.SpellTimer["Pounce"].IsReady(3) && target.Distance(KL.Player.ServerPosition) <= 525)
                    {
                        if (target.Distance(KL.Player.ServerPosition) > radius)
                        {
                            if (KL.Player.GetAutoAttackDamage(target, true) * 3 >= target.Health)
                                KL.Spells["Aspect"].Cast();
                        }
                    }
                }

                // is jungling
                if (mode == "jg")
                {
                    if (KL.CanUse(KL.Spells["Bushwhack"], true, mode) ||
                        KL.CanUse(KL.Spells["Javelin"], true, mode))
                    {
                        if ((!KL.SpellTimer["Pounce"].IsReady(2) || !KL.CanUse(KL.Spells["Pounce"], false, mode)) &&
                            (!KL.SpellTimer["Swipe"].IsReady() || !KL.CanUse(KL.Spells["Swipe"], false, mode)) &&
                            (!KL.SpellTimer["Takedown"].IsReady() || !KL.CanUse(KL.Spells["Takedown"], false, mode)) ||

                            !(KL.Player.Distance(target.ServerPosition) <= 355) ||
                             !KN.Root.Item("jgaacount").GetValue<KeyBind>().Active)
                        {
                            if (KL.Spells["Javelin"].Cast(target) != Spell.CastStates.Collision &&
                                KL.SpellTimer["Javelin"].IsReady())
                            {
                                KL.Spells["Aspect"].Cast();
                            }
                            else if (!KL.CanUse(KL.Spells["Javelin"], true, mode))
                            {
                                KL.Spells["Aspect"].Cast();
                            }
                        }
                    }
                }
            }

            // human -> catform
            if (!KL.CatForm() && KL.CanUse(KL.Spells["Aspect"], true, mode))
            {
                switch (mode)
                {
                    case "jg":
                        if (KL.Counter < KN.Root.Item("aareq").GetValue<Slider>().Value &&
                            KN.Root.Item("jgaacount").GetValue<KeyBind>().Active)
                        {
                            return;
                        }
                        break;
                    case "gap":
                        if (target.IsValidTarget(375))
                        {
                            KL.Spells["Aspect"].Cast();
                            return;
                        }
                        break;
                    case "wc":
                        if (target.IsValidTarget(375) && target.IsMinion)
                        {
                            KL.Spells["Aspect"].Cast();
                            return;
                        }
                        break;
                }

                if (target.IsHunted())
                {
                    // force switch no swipe/takedown req
                    if (!KN.Root.Item("ndhrcreq").GetValue<bool>() && mode == "co" ||
                        !KN.Root.Item("ndhrjreq").GetValue<bool>() && mode == "jg")
                    {
                        KL.Spells["Aspect"].Cast();
                        return;
                    }

                    if (target.Distance(KL.Player) > KL.Spells["Takedown"].Range + 50 &&
                        !KL.CanUse(KL.Spells["Pounce"], false, mode))
                        return;

                    // or check if pounce timer is ready before switch
                    if (KL.Spells["Aspect"].IsReady() && target.IsValidTarget(KL.Spells["ExPounce"].Range))
                    {
                        // dont change form if swipe or takedown isn't ready
                        if ((KL.SpellTimer["Takedown"].IsReady() || KL.SpellTimer["Swipe"].IsReady()) &&
                             KL.SpellTimer["Pounce"].IsReady(1))
                             KL.Spells["Aspect"].Cast();
                    }
                }
                else
                {
                    // check if in pounce range
                    if (target.IsValidTarget(KL.Spells["Pounce"].Range + 55))
                    {
                        if (mode != "jg")
                        {
                            // switch to cougar if can kill target
                            if (KL.CatDamage(target) * 3 >= target.Health)
                            {
                                if (mode == "co" && target.IsValidTarget(KL.Spells["Pounce"].Range + 200))
                                {
                                    if (!KL.CanUse(KL.Spells["Javelin"], true, "co") ||
                                         KL.Spells["Javelin"].Cast(target) == Spell.CastStates.Collision)
                                    {
                                        KL.Spells["Aspect"].Cast();
                                    }
                                }
                            }

                            // switch if Q disabled in menu
                            if (!KL.CanUse(KL.Spells["Javelin"], true, mode) ||

                                // delay the cast .5 seconds
                                Utils.GameTimeTickCount - (int) (KL.TimeStamp["Javelin"] * 1000) +
                                ((6 + (6 * KL.PercentCooldownMod)) * 1000) >= 500 &&

                                // if Q is not ready in 2 seconds
                                !KL.SpellTimer["Javelin"].IsReady(2))
                            {
                                KL.Spells["Aspect"].Cast();
                            }
                        }
                        else
                        {
                            if (KL.Spells["Javelin"].Cast(target) == Spell.CastStates.Collision &&
                                KN.Root.Item("spcol").GetValue<bool>())
                            {
                                if (KL.Spells["Aspect"].IsReady())
                                    KL.Spells["Aspect"].Cast();
                            }

                            if ((!KL.SpellTimer["Bushwhack"].IsReady() || !KL.CanUse(KL.Spells["Bushwhack"], true, mode)) &&
                                (!KL.SpellTimer["Javelin"].IsReady(3) || !KL.CanUse(KL.Spells["Javelin"], true, mode)))
                            {
                                if (KL.Spells["Aspect"].IsReady())
                                    KL.Spells["Aspect"].Cast();
                            }
                        }
                    }
                    

                    if (KN.Target.IsValidTarget(KL.Spells["Javelin"].Range) && target.IsChampion())
                    {
                        if (KL.SpellTimer["Javelin"].IsReady())
                        {
                            // check if in pounce range.
                            if (target.Distance(KL.Player.ServerPosition) <= KL.Spells["Pounce"].Range + 100f)
                            {
                                // if we dont meet hitchance on Q target pounce nearest target
                                var poutput = KL.Spells["Javelin"].GetPrediction(KN.Target);
                                if (poutput.Hitchance < (HitChance) (KN.Root.Item("ndhqch").GetValue<StringList>().SelectedIndex + 3))
                                {
                                    if (KL.Spells["Aspect"].IsReady())
                                        KL.Spells["Aspect"].Cast();
                                }
                            }
                        }

                        if (KN.Target.IsHunted() && KN.Target.Distance(KL.Player.ServerPosition) > KL.Spells["ExPounce"].Range + 100)
                        {
                            if (target.Distance(KL.Player.ServerPosition) <= KL.Spells["Pounce"].Range + 25)
                            {
                                if (KL.Spells["Aspect"].IsReady())
                                    KL.Spells["Aspect"].Cast();
                            }
                        }

                        if (!KL.SpellTimer["Javelin"].IsReady())
                        {
                            if (target.Distance(KL.Player.ServerPosition) <= KL.Spells["Pounce"].Range + 125)
                            {
                                KL.Spells["Aspect"].Cast();
                            }
                        }
                    }
                }
            }
        }
    }
}
