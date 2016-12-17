using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Spells
{
    using myCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class SpellManager : Logic
    {
        internal static void Init()
        {
            Q = new Spell(SpellSlot.Q, 325f);
            W = new Spell(SpellSlot.W, 260f);
            E = new Spell(SpellSlot.E, 312f);
            R = new Spell(SpellSlot.R, 900f);

            Q.SetSkillshot(0.25f, 100f, 2200f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 45f, 1600f, false, SkillshotType.SkillshotCone);

            Ignite = Me.GetSpellSlot("SummonerDot");
            Flash = Me.GetSpellSlot("SummonerFlash");
        }

        internal static void CastItem(bool tiamat = false, bool youmuu = false)
        {
            if (tiamat)
            {
                if (Items.HasItem(3077) && Items.CanUseItem(3077))
                {
                    Items.UseItem(3077);
                }

                if (Items.HasItem(3074) && Items.CanUseItem(3074))
                {
                    Items.UseItem(3074);
                }

                if (Items.HasItem(3053) && Items.CanUseItem(3053))
                {
                    Items.UseItem(3053);
                }
            }

            if (youmuu)
            {
                if (Items.HasItem(3142) && Items.CanUseItem(3142))
                {
                    Items.UseItem(3142);
                }
            }
        }

        internal static void CastQ(Obj_AI_Base target)
        {
            if (target != null && !target.IsDead)
            {
                switch (Menu.GetList("QMode"))
                {
                    case 0:
                        Q.Cast(target.Position, true);
                        break;
                    case 1:
                        Q.Cast(Game.CursorPos, true);
                        break;
                    case 2:
                        Q.Cast(Me.Position.Extend(target.Position, Q.Range), true);
                        break;
                    default:
                        Q.Cast(Me.Position.Extend(Game.CursorPos, Q.Range), true);
                        break;
                }
            }
        }

        internal static void ResetQA(int time)
        {
            if (Menu.GetBool("Dance"))
            {
                EloBuddy.Player.DoEmote(Emote.Dance);
            }
            LeagueSharp.Common.Utility.DelayAction.Add(time, () =>
            {
                EloBuddy.Player.DoEmote(Emote.Dance);
                Orbwalking.ResetAutoAttackTimer();
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Me.Position.Extend(Game.CursorPos, +10));
            });
        }

        internal static void R2Logic(AIHeroClient target)
        {
            if (target == null || R.Instance.Name == "RivenFengShuiEngine")
            {
                return;
            }

            if (target.Check(850))
            {
                switch (Menu.GetList("R2Mode"))
                {
                    case 0:
                        if (DamageCalculate.GetRDamage(target) > target.Health && target.DistanceToPlayer() < 600)
                        {
                            var pred = R.GetPrediction(target, true);

                            if (pred.Hitchance >= HitChance.VeryHigh)
                            {
                                R.Cast(pred.CastPosition, true);
                            }
                        }
                        break;
                    case 1:
                        if (target.HealthPercent < 20 ||
                            (target.Health > DamageCalculate.GetRDamage(target) + Me.GetAutoAttackDamage(target) * 2 &&
                             target.HealthPercent < 40) ||
                            (target.Health <= DamageCalculate.GetRDamage(target)) || 
                            (target.Health <= DamageCalculate.GetComboDamage(target)*1.3))
                        {
                            var pred = R.GetPrediction(target, true);

                            if (pred.Hitchance >= HitChance.VeryHigh)
                            {
                                R.Cast(pred.CastPosition, true);
                            }
                        }
                        break;
                    case 2:
                        if (target.DistanceToPlayer() < 600)
                        {
                            var pred = R.GetPrediction(target, true);

                            if (pred.Hitchance >= HitChance.VeryHigh)
                            {
                                R.Cast(pred.CastPosition, true);
                            }
                        }
                        break;
                }
            }
        }
    }
}
