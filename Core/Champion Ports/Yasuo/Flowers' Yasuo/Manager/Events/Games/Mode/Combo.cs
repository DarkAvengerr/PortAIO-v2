using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo.Manager.Events.Games.Mode
{
    using System.Linq;
    using Spells;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Orbwalking = Orbwalking;
    using static Common.Common;

    internal class Combo : Logic
    {
        internal static void Init()
        {
            var target = TargetSelector.GetTarget(1200f, TargetSelector.DamageType.Physical);

            if (target == null)
            {
                return;
            }

            if (target.DistanceToPlayer() > R.Range)
            {
                return;
            }

            if (Menu.Item("ComboIgnite", true).GetValue<bool>() && Ignite != SpellSlot.Unknown && Ignite.IsReady()
                && target.IsValidTarget(600f)
                && (target.Health <= Me.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)
                    || target.HealthPercent <= 25))
            {
                Me.Spellbook.CastSpell(Ignite, target);
            }

            if (Menu.Item("ComboItems", true).GetValue<bool>())
            {
                SpellManager.UseItems(target, true);
            }

            if (Menu.Item("ComboR", true).GetValue<KeyBind>().Active && R.IsReady())
            {
                var KnockedUpEnemies =
                    HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range))
                        .Where(x => x.HasBuffOfType(BuffType.Knockup) || x.HasBuffOfType(BuffType.Knockback))
                        .Where(CanCastDelayR);

                if (KnockedUpEnemies.Count() >= Menu.Item("ComboRCount", true).GetValue<Slider>().Value)
                {
                    R.Cast();
                }

                foreach (var rTarget in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range))
                    .Where(x => x.HasBuffOfType(BuffType.Knockup) || x.HasBuffOfType(BuffType.Knockback))
                    .Where(CanCastDelayR))
                {
                    if (Menu.Item("R" + rTarget.ChampionName.ToLower(), true).GetValue<bool>() &&
                        rTarget.HealthPercent <= Menu.Item("ComboRHp", true).GetValue<Slider>().Value)
                    {
                        R.Cast();
                    }

                    if (Menu.Item("ComboRAlly", true).GetValue<bool>() &&
                        HeroManager.Allies.Any(x => !x.IsDead && !x.IsZombie && x.Distance(rTarget) <= 600) &&
                        rTarget.Health >= Menu.Item("ComboRHp", true).GetValue<Slider>().Value)
                    {
                        R.Cast();
                    }
                }
            }

            if (Menu.Item("ComboE", true).GetValue<bool>() && E.IsReady())
            {
                var dmg = (float)(SpellManager.GetQDmg(target) * 2 + SpellManager.GetEDmg(target)) +
                          Me.GetAutoAttackDamage(target) * 2 +
                          (R.IsReady() ? R.GetDamage(target) : (float)SpellManager.GetQDmg(target));

                if (target.DistanceToPlayer() >= Orbwalking.GetRealAutoAttackRange(Me) + 65 &&
                    dmg >= target.Health && SpellManager.CanCastE(target) &&
                    (Menu.Item("ComboETurret", true).GetValue<bool>() || !UnderTower(PosAfterE(target))))
                {
                    E.CastOnUnit(target, true);
                }
            }

            if (Menu.Item("ComboEGapcloser", true).GetValue<bool>() && E.IsReady() &&
                target.DistanceToPlayer() >= Menu.Item("ComboEGap", true).GetValue<Slider>().Value)
            {
                if (Menu.Item("ComboEMode", true).GetValue<StringList>().SelectedIndex == 0)
                {
                    SpellManager.EGapTarget(target, Menu.Item("ComboETurret", true).GetValue<bool>(),
                        Menu.Item("ComboEGap", true).GetValue<Slider>().Value, false);
                }
                else
                {
                    SpellManager.EGapMouse(target, Menu.Item("ComboETurret", true).GetValue<bool>(),
                        Menu.Item("ComboEGap", true).GetValue<Slider>().Value, false);
                }
            }

            if (Menu.Item("ComboQ", true).GetValue<bool>() && Me.Spellbook.GetSpell(SpellSlot.Q).IsReady() && !isDashing)
            {
                if (SpellManager.HaveQ3)
                {
                    if (target.IsValidTarget(Q3.Range))
                    {
                        SpellManager.CastQ3();
                    }
                }
                else
                {
                    if (target.IsValidTarget(Q.Range))
                    {
                        Q.Cast(target, true);
                    }
                }
            }

            if (IsDashing)
            {
                if (Menu.Item("ComboEQ", true).GetValue<bool>() && Q.IsReady() && !SpellManager.HaveQ3 &&
                    target.Distance(lastEPos) <= 220)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(10, () => Q.Cast(Me.Position, true));
                }

                if (Menu.Item("ComboEQ3", true).GetValue<bool>() && Q3.IsReady() && SpellManager.HaveQ3 &&
                    target.Distance(lastEPos) <= 220)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(10, () => Q3.Cast(Me.Position, true));
                }

                if (Menu.Item("ComboQStack", true).GetValue<StringList>().SelectedIndex != 3 && Q.IsReady() && !SpellManager.HaveQ3)
                {
                    switch (Menu.Item("ComboQStack", true).GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            if (MinionManager.GetMinions(lastEPos, 220, MinionTypes.All, MinionTeam.NotAlly).Count > 0 ||
                                HeroManager.Enemies.Count(x => x.IsValidTarget(220, true, lastEPos)) > 0)
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(10, () => Q.Cast(Me.Position, true));
                            }
                            break;
                        case 1:
                            if (HeroManager.Enemies.Count(x => x.IsValidTarget(220, true, lastEPos)) > 0)
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(10, () => Q.Cast(Me.Position, true));
                            }
                            break;
                        case 2:
                            if (MinionManager.GetMinions(lastEPos, 220, MinionTypes.All, MinionTeam.NotAlly).Count > 0)
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(10, () => Q.Cast(Me.Position, true));
                            }
                            break;
                    }
                }
            }
        }
    }
}
