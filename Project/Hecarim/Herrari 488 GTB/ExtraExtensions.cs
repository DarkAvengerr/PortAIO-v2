using System;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Herrari_488_GTB
{
    static class ExtraExtensions
    {
        internal static bool isReadyPerfectly(this Spell spell)
        {
            return spell != null && spell.Slot != SpellSlot.Unknown && spell.Instance.State != SpellState.Cooldown && spell.Instance.State != SpellState.Disabled && spell.Instance.State != SpellState.NoMana && spell.Instance.State != SpellState.NotLearned && spell.Instance.State != SpellState.Surpressed && spell.Instance.State != SpellState.Unknown && spell.Instance.State == SpellState.Ready;
        }

        internal static bool isKillableAndValidTarget(this AIHeroClient Target, double CalculatedDamage, TargetSelector.DamageType damageType, float distance = float.MaxValue)
        {
            if (Target == null || !Target.IsValidTarget(distance) || Target.Health <= 0 || Target.CharData.BaseSkinName == "gangplankbarrel")
                return false;

            if (Target.HasBuff("kindredrnodeathbuff"))
            {
                return false;
            }

            // Tryndamere's Undying Rage (R)
            if (Target.HasBuff("Undying Rage") && Target.Health <= Target.MaxHealth * 0.10f)
            {
                return false;
            }

            // Kayle's Intervention (R)
            if (Target.HasBuff("JudicatorIntervention"))
            {
                return false;
            }

            // Poppy's Diplomatic Immunity (R)
            if (Target.HasBuff("DiplomaticImmunity") && !ObjectManager.Player.HasBuff("poppyulttargetmark"))
            {
                //TODO: Get the actual target mark buff name
                return false;
            }

            // Banshee's Veil (PASSIVE)
            if (Target.HasBuff("BansheesVeil"))
            {
                // TODO: Get exact Banshee's Veil buff name.
                return false;
            }

            // Sivir's Spell Shield (E)
            if (Target.HasBuff("SivirShield"))
            {
                // TODO: Get exact Sivir's Spell Shield buff name
                return false;
            }

            // Nocturne's Shroud of Darkness (W)
            if (Target.HasBuff("ShroudofDarkness"))
            {
                // TODO: Get exact Nocturne's Shourd of Darkness buff name
                return false;
            }

            if (ObjectManager.Player.HasBuff("summonerexhaust"))
                CalculatedDamage *= 0.6;

            if (Target.ChampionName == "Blitzcrank")
                if (!Target.HasBuff("manabarriercooldown"))
                    if (Target.Health + Target.HPRegenRate + (damageType == TargetSelector.DamageType.Physical ? Target.AttackShield : Target.MagicShield) + (Target.Mana * 0.6) + Target.PARRegenRate < CalculatedDamage)
                        return true;

            if (Target.ChampionName == "Garen")
                if (Target.HasBuff("GarenW"))
                    CalculatedDamage *= 0.7;

            if (Target.HasBuff("FerociousHowl"))
                CalculatedDamage *= 0.3;

            return Target.Health + Target.HPRegenRate + (damageType == TargetSelector.DamageType.Physical ? Target.AttackShield : Target.MagicShield) < CalculatedDamage - 2;
        }

        internal static bool isKillableAndValidTarget(this Obj_AI_Minion Target, double CalculatedDamage, TargetSelector.DamageType damageType, float distance = float.MaxValue)
        {
            if (Target == null || !Target.IsValidTarget(distance) || Target.Health <= 0 || Target.HasBuffOfType(BuffType.SpellImmunity) || Target.HasBuffOfType(BuffType.SpellShield) || Target.CharData.BaseSkinName == "gangplankbarrel")
                return false;

            if (ObjectManager.Player.HasBuff("summonerexhaust"))
                CalculatedDamage *= 0.6;

            BuffInstance dragonSlayerBuff = ObjectManager.Player.GetBuff("s5test_dragonslayerbuff");
            if (dragonSlayerBuff != null)
            {
                if (dragonSlayerBuff.Count >= 4)
                    CalculatedDamage += dragonSlayerBuff.Count == 5 ? CalculatedDamage * 0.30 : CalculatedDamage * 0.15;

                if (Target.CharData.BaseSkinName.ToLowerInvariant().Contains("dragon"))
                    CalculatedDamage *= 1 - (dragonSlayerBuff.Count * 0.07);
            }

            if (Target.CharData.BaseSkinName.ToLowerInvariant().Contains("baron") && ObjectManager.Player.HasBuff("barontarget"))
                CalculatedDamage *= 0.5;

            return Target.Health + Target.HPRegenRate + (damageType == TargetSelector.DamageType.Physical ? Target.AttackShield : Target.MagicShield) < CalculatedDamage - 2;
        }

        internal static bool isKillableAndValidTarget(this Obj_AI_Base Target, double CalculatedDamage, TargetSelector.DamageType damageType, float distance = float.MaxValue)
        {
            if (Target == null || !Target.IsValidTarget(distance) || Target.Health <= 0 || Target.CharData.BaseSkinName == "gangplankbarrel")
                return false;

            if (Target.HasBuff("kindredrnodeathbuff"))
            {
                return false;
            }

            // Tryndamere's Undying Rage (R)
            if (Target.HasBuff("Undying Rage") && Target.Health <= Target.MaxHealth * 0.10f)
            {
                return false;
            }

            // Kayle's Intervention (R)
            if (Target.HasBuff("JudicatorIntervention"))
            {
                return false;
            }

            // Poppy's Diplomatic Immunity (R)
            if (Target.HasBuff("DiplomaticImmunity") && !ObjectManager.Player.HasBuff("poppyulttargetmark"))
            {
                //TODO: Get the actual target mark buff name
                return false;
            }

            // Banshee's Veil (PASSIVE)
            if (Target.HasBuff("BansheesVeil"))
            {
                // TODO: Get exact Banshee's Veil buff name.
                return false;
            }

            // Sivir's Spell Shield (E)
            if (Target.HasBuff("SivirShield"))
            {
                // TODO: Get exact Sivir's Spell Shield buff name
                return false;
            }

            // Nocturne's Shroud of Darkness (W)
            if (Target.HasBuff("ShroudofDarkness"))
            {
                // TODO: Get exact Nocturne's Shourd of Darkness buff name
                return false;
            }

            if (ObjectManager.Player.HasBuff("summonerexhaust"))
                CalculatedDamage *= 0.6;

            if (Target.CharData.BaseSkinName == "Blitzcrank")
                if (!Target.HasBuff("manabarriercooldown"))
                    if (Target.Health + Target.HPRegenRate + (damageType == TargetSelector.DamageType.Physical ? Target.AttackShield : Target.MagicShield) + (Target.Mana * 0.6) + Target.PARRegenRate < CalculatedDamage)
                        return true;

            if (Target.CharData.BaseSkinName == "Garen")
                if (Target.HasBuff("GarenW"))
                    CalculatedDamage *= 0.7;


            if (Target.HasBuff("FerociousHowl"))
                CalculatedDamage *= 0.3;

            BuffInstance dragonSlayerBuff = ObjectManager.Player.GetBuff("s5test_dragonslayerbuff");
            if (dragonSlayerBuff != null)
                if (Target.IsMinion)
                {
                    if (dragonSlayerBuff.Count >= 4)
                        CalculatedDamage += dragonSlayerBuff.Count == 5 ? CalculatedDamage * 0.30 : CalculatedDamage * 0.15;

                    if (Target.CharData.BaseSkinName.ToLowerInvariant().Contains("dragon"))
                        CalculatedDamage *= 1 - (dragonSlayerBuff.Count * 0.07);
                }

            if (Target.CharData.BaseSkinName.ToLowerInvariant().Contains("baron") && ObjectManager.Player.HasBuff("barontarget"))
                CalculatedDamage *= 0.5;

            return Target.Health + Target.HPRegenRate + (damageType == TargetSelector.DamageType.Physical ? Target.AttackShield : Target.MagicShield) < CalculatedDamage - 2;
        }

        internal static bool isManaPercentOkay(this AIHeroClient hero, int ManaPercent)
        {
            return hero.ManaPercent > ManaPercent;
        }

        internal static double isImmobileUntil(this AIHeroClient unit)
        {
            var result =
                unit.Buffs.Where(
                    buff =>
                        buff.IsActive && Game.Time <= buff.EndTime &&
                        (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun ||
                         buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare))
                    .Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return (result - Game.Time);
        }

        internal static bool isWillDieByTristanaE(this Obj_AI_Base target)
        {
            if (ObjectManager.Player.ChampionName == "Tristana")
                if (target.HasBuff("tristanaecharge"))
                    if (target.isKillableAndValidTarget((float)(Damage.GetSpellDamage(ObjectManager.Player, target, SpellSlot.E) * (target.GetBuffCount("tristanaecharge") * 0.30) + Damage.GetSpellDamage(ObjectManager.Player, target, SpellSlot.E)), TargetSelector.DamageType.Physical))
                        return true;
            return false;
        }

        internal static Spell.CastStates CastWithExtraTrapLogic(this Spell spell)
        {
            if (spell.isReadyPerfectly())
            {
                var Teleport = MinionManager.GetMinions(spell.Range).FirstOrDefault(x => x.HasBuff("teleport_target"));
                var Zhonya = HeroManager.Enemies.FirstOrDefault(x => ObjectManager.Player.Distance(x) <= spell.Range && x.HasBuff("zhonyasringshield"));

                if (Teleport != null)
                    return spell.Cast(Teleport);

                if (Zhonya != null)
                    return spell.Cast(Zhonya);

            }
            return Spell.CastStates.NotCasted;
        }

        internal static float GetRealAutoAttackRange(this AttackableUnit unit, AttackableUnit target, int AutoAttackRange)
        {
            float result = AutoAttackRange + unit.BoundingRadius;
            if (target.IsValidTarget())
                return result + target.BoundingRadius;
            return result;
        }

        internal static bool isJungleMob(this Obj_AI_Base mob)
        {
            return MinionManager.GetMinions(float.MaxValue, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Any(x => x.NetworkId == mob.NetworkId);
        }

        internal static bool isLaneMob(this Obj_AI_Base mob)
        {
            return MinionManager.GetMinions(float.MaxValue).Any(x => x.NetworkId == mob.NetworkId);
        }
    }
}
