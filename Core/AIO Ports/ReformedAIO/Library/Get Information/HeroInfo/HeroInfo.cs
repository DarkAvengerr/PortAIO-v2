using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Library.Get_Information.HeroInfo
{
    using System;
    using System.Linq;

    using LeagueSharp;

    internal class HeroInfo
    {
        public float GetBuffEndTime(Obj_AI_Base target, string buffname)
        {
            return Math.Max(0, target.GetBuff(buffname).EndTime) - Game.Time;
        }

        public bool Unkillable(AIHeroClient target)
        {
            if (target.Buffs.Any(b => b.IsValid 
            && (b.DisplayName == "UndyingRage" 
            || b.DisplayName == "ChronoShift"
            || b.DisplayName == "JudicatorIntervention" 
            || b.DisplayName == "kindredrnodeathbuff")))
            {
                return true;
            }

            return target.HasBuffOfType(BuffType.Invulnerability) || target.IsInvulnerable;
        }

        public bool HasSpellShield(AIHeroClient target)
        {
            if (target.Buffs.Any(b => b.IsValid 
            && (b.DisplayName == "bansheesveil" 
            || b.DisplayName == "SivirE"
            || b.DisplayName == "NocturneW")))
            {
                return true;
            }
         
            return target.HasBuffOfType(BuffType.SpellShield) || target.HasBuffOfType(BuffType.SpellImmunity);
        }

        public int GetStunDuration(AIHeroClient target)
        {
            return (int)(target.Buffs.Where(b => b.IsActive
            && Game.Time < b.EndTime
            && Immobilized(target))
            .Aggregate(0f, (current, buff) => Math.Max(current, buff.EndTime)) - Game.Time) * 1000;
        }

        public bool Immobilized(AIHeroClient target)
        {
            return target.HasBuffOfType(BuffType.Stun)
                   || target.HasBuffOfType(BuffType.Snare)
                   || target.HasBuffOfType(BuffType.Knockup)
                   || target.HasBuffOfType(BuffType.Knockback)
                   || target.HasBuffOfType(BuffType.Charm)
                   || target.HasBuffOfType(BuffType.Fear)
                   || target.HasBuffOfType(BuffType.Taunt)
                   || target.HasBuffOfType(BuffType.Suppression);
        }
    }
}
