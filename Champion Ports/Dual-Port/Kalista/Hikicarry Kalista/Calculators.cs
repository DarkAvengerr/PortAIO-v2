using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry_Kalista
{
    class Calculators
    {
        private static readonly float[] RRD = { 20, 30, 40, 50, 60 };
        private static readonly float[] RRDM = { 0.6f, 0.6f, 0.6f, 0.6f, 0.6f };
        private static readonly float[] RRPS = { 10, 14, 19, 25, 32 };
        private static readonly float[] RRPSM = { 0.2f, 0.225f, 0.25f, 0.275f, 0.3f };

        public static float CustomCalculator(Obj_AI_Base target, int customStacks = -1)
        {
            int buff = target.GetBuffCount("KalistaExpungeMarker");

            if (buff > 0 || customStacks > -1)
            {
                var tDamage = (RRD[Program.E.Level - 1] + RRDM[Program.E.Level - 1] * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod)) +
                       ((customStacks < 0 ? buff : customStacks) - 1) *
                       (RRPS[Program.E.Level - 1] + RRPSM[Program.E.Level - 1] * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod));

                return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, tDamage);
            }
            return 0;
        }
        public static float JungleCalculator(Obj_AI_Minion minion, int customStacks = -1)
        {
            int buff = minion.GetBuffCount("KalistaExpungeMarker");

            if (buff > 0 || customStacks > -1)
            {
                var tDamage = (RRD[Program.E.Level - 1] + RRDM[Program.E.Level - 1] * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod)) +
                       ((customStacks < 0 ? buff : customStacks) - 1) *
                       (RRPS[Program.E.Level - 1] + RRPSM[Program.E.Level - 1] * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod));

                return (float)ObjectManager.Player.CalcDamage(minion, Damage.DamageType.Physical, tDamage);
            }

            return 0;
        }
        public static float MinionCalculator (Obj_AI_Base minion, int customStacks = -1)
        {
            int buff = minion.GetBuffCount("KalistaExpungeMarker");

            if (buff > 0 || customStacks > -1)
            {
                var tDamage = (RRD[Program.E.Level - 1] + RRDM[Program.E.Level - 1] * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod)) +
                       ((customStacks < 0 ? buff : customStacks) - 1) *
                       (RRPS[Program.E.Level - 1] + RRPSM[Program.E.Level - 1] * (ObjectManager.Player.BaseAttackDamage + ObjectManager.Player.FlatPhysicalDamageMod));

                return (float)ObjectManager.Player.CalcDamage(minion, Damage.DamageType.Physical, tDamage);
            }

            return 0;
        }
        public static float ChampionTotalDamage(AIHeroClient target)
        {
            var damage = 0f;

            if (Program.E.IsReady())
            {
                switch (Program.Config.Item("calculator").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        damage += CustomCalculator(target);
                        break;
                    case 1:
                        damage += (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, Program.E.GetDamage(target));
                        break;
                }

            }
            return (float)damage;
        }
        public static float JungleTotalDamage(Obj_AI_Minion target)
        {
            var damage = 0f;

            if (Program.E.IsReady())
            {
                switch (Program.Config.Item("calculator").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        damage += JungleCalculator(target);
                        break;
                    case 1:
                        damage += (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, Program.E.GetDamage(target));
                        break;
                }
            }
            return (float)damage;
        }
        public static float MinionTotalDamage(Obj_AI_Minion target)
        {
            var damage = 0f;

            if (Program.E.IsReady())
            {
                switch (Program.Config.Item("calculator").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        damage += JungleCalculator(target);
                        break;
                    case 1:
                        damage += (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, Program.E.GetDamage(target));
                        break;
                }

            }
            return (float)damage;
        }
        public static int KillableSpearCount(AIHeroClient enemy)
        {
            float spearDamage = ChampionTotalDamage(enemy);
            float killableSpearCount = enemy.Health / spearDamage;
            int totalSpear = (int)Math.Ceiling(killableSpearCount) - 1;

            return totalSpear;
        }
    }
}
