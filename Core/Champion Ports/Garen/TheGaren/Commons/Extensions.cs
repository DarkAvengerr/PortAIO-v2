using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheKalista.Commons
{
    public static class Extensions
    {
        public static SpellState GetState(this SpellDataInst spellData)
        {
            switch ((int)spellData.State)
            {
                case 0:
                    return SpellState.Ready;
                case 2:
                    return SpellState.NotLearned;
                case 4:
                    return SpellState.Surpressed;
                case 5:
                    return SpellState.Cooldown;
                case 6:
                    return SpellState.NoMana;
                case 10:
                    return SpellState.Surpressed;
                default:
                    return SpellState.Unknown;
            }
        }

        //public static SpellState GetState(this Spell spellData)
        //{
        //    return spellData.Instance.GetState();
        //}

        public static T ToEnum<T>(this string str)
        {
            return (T)Enum.Parse(typeof(T), str);
        }

        public static bool HasSpellShield(this AIHeroClient entity)
        {
            return entity.HasBuff("bansheesveil") || entity.HasBuff("SivirE") || entity.HasBuff("NocturneW");
        }

        public static bool IsPoisoned(this Obj_AI_Base entity)
        {
            return entity.HasBuffOfType(BuffType.Poison);
        }

        public static float GetPoisonedTime(this Obj_AI_Base entity)
        {
            if (!entity.IsPoisoned()) return 0;
            return entity.Buffs.Where(buff => buff.Type == BuffType.Poison).Max(buff => buff.EndTime);
            //return entity.Buffs.Where(buff => buff.Type == BuffType.Poison).OrderByDescending(poison => poison.EndTime).First().EndTime - Game.Time;
        }

        public static float GetIgniteDamage(this AIHeroClient souce)
        {
            return 50 + souce.Level * 20;
        }

        public static float GetRemainingIgniteDamage(this Obj_AI_Base target)
        {
            var ignitebuff = target.GetBuff("summonerdot");
            if (ignitebuff == null) return 0;
            return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.True, ((int)(ignitebuff.EndTime - Game.Time) + 1) * GetIgniteDamage(ignitebuff.Caster as AIHeroClient) / 5);
        }

        public static bool IsFacing(this Obj_AI_Base source, Obj_AI_Base target, float angle = 90)
        {
            return source.Direction.To2D().Perpendicular().AngleBetween((target.Position - source.Position).To2D()) < angle;
        }

        public static bool IsFacingMe(this Obj_AI_Base source, float angle = 90)
        {
            return source.Direction.To2D().Perpendicular().AngleBetween((ObjectManager.Player.Position - source.Position).To2D()) < angle;
        }

        public static bool NearFountain(this AIHeroClient hero, float distance = 750)
        {
            var map = LeagueSharp.Common.Utility.Map.GetMap();
            if (map != null && map.Type == LeagueSharp.Common.Utility.Map.MapType.SummonersRift)
            {
                distance += 300;
            }
            return hero.IsVisible &&
                   ObjectManager.Get<Obj_SpawnPoint>()
                       .Any(sp => sp.Team == hero.Team && hero.Distance(sp.Position, true) < distance * distance);
        }

    }
}
