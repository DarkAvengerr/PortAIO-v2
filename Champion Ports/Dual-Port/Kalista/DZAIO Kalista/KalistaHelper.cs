using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Core;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Kalista
{
    static class KalistaHelper
    {
        public static bool HasRend(this Obj_AI_Base target)
        {
            return target.GetRendBuff() != null;
        }

        public static BuffInstance GetRendBuff(this Obj_AI_Base target)
        {
            return target.Buffs.FirstOrDefault(b => b.Caster.IsMe && b.IsValidBuff() && b.DisplayName == "KalistaExpungeMarker");
        }

        public static bool RendAboutToRunOut(this Obj_AI_Base target)
        {
            return target.GetRendBuff().EndTime - Game.Time < 0.30f;
        }

        public static bool CanBeRendKilled(this Obj_AI_Base target)
        {
            var rendBuffer = 25f;

            return target.HasRend()
                    && target.IsValidTarget(Variables.CurrentChampion.GetSpells()[SpellSlot.E].Range) &&
                   (HealthPrediction.GetHealthPrediction(target, 280) > 0 &&
                    HealthPrediction.GetHealthPrediction(target, 280) + rendBuffer <= GetRendDamage(target)) &&
                   (IsNotInvulnerable(target));
        }

        public static float GetRendDamage(Obj_AI_Base target)
        {
            if (!IsNotInvulnerable(target))
            {
                return 0f;
            }

            var spells = Variables.CurrentChampion.GetSpells();
            var baseDamage = spells[SpellSlot.E].GetDamage(target);
            var reductionList = GetReductionList();

            foreach (var t in reductionList)
            {
                switch (t.Item3)
                {
                    case ReductionType.Player:
                        if (ObjectManager.Player.HasBuff(t.Item1))
                        {
                            baseDamage *= t.Item2;
                        }
                        break;
                    case ReductionType.Target:
                        if (target.HasBuff(t.Item1))
                        {
                            baseDamage *= t.Item2;
                        }
                        break;
                }
            }

            return baseDamage;
        }

        public static bool IsNotInvulnerable(Obj_AI_Base target)
        {
            var TSInvulnerable = TargetSelector.IsInvulnerable(target, TargetSelector.DamageType.Physical, false);
            var invulnerable = (target.IsInvulnerable);

            return !invulnerable && !TSInvulnerable;
        }

        public static List<Tuple<String, float, ReductionType>> GetReductionList()
        {
            var reductionList = new List<Tuple<String, float, ReductionType>>()
            {
                new Tuple<string, float, ReductionType>("summonerexhaust", 0.4f, ReductionType.Player),
                new Tuple<string, float, ReductionType>("FerociousHowl", 0.35f, ReductionType.Target)
            };

            return reductionList;
        }
    }

    enum ReductionType
    {
        Player, Target
    }
}
