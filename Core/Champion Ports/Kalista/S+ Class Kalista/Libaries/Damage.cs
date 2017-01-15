using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace S_Plus_Class_Kalista.Libaries
{
    class Damage
    {
        internal class DamageCalc : Core
        {
            #region Public Functions

            public static bool CheckNoDamageBuffs(AIHeroClient target)// From Asuna
            {
                foreach (var b in target.Buffs.Where(b => b.IsValidBuff()))
                {
                    switch (b.DisplayName)
                    {
                        case "Chrono Shift":
                            return true;
                        case "JudicatorIntervention":
                            return true;
                        case "Undying Rage":
                            if (target.ChampionName == "Tryndamere")
                                return true;
                            continue;

                            //Spell Shields
                        case "bansheesveil":
                            return true;
                        case "SivirE":
                            return true;
                        case "NocturneW":
                            return true;
                        case "kindredrnodeathbuff":
                            return true;
                    }
                }
                if (target.ChampionName == "Poppy" && HeroManager.Allies.Any(
                    o =>
                    {
                        return !o.IsMe
                               && o.Buffs.Any(
                                   b =>
                                       b.Caster.NetworkId == target.NetworkId && b.IsValidBuff()
                                       && b.DisplayName == "PoppyDITarget");
                    }))
                {
                    return true;
                }

                return (target.HasBuffOfType(BuffType.Invulnerability)
                        || target.HasBuffOfType(BuffType.SpellImmunity));
                // || target.HasBuffOfType(BuffType.SpellShield));
            }

            #endregion Public Functions

            #region Private Functions

            private const string ShieldNames ="blindmonkwoneshield,evelynnrshield,EyeOfTheStorm,ItemSeraphsEmbrace,JarvanIVGoeldenAegis,KarmaSolKimShield,lulufarieshield,luxprismaticwaveshieldself,manabarrier,mordekaiserironman,nautiluspiercinggazeshield,orianaredactshield,rumbleshieldbuff,Shenstandunitedshield,SkarnerExoskeleton,summonerbarrier,tahmkencheshield,udyrturtleactivation,UrgotTerrorCapacitorActive2,ViktorPowerTransfer,dianashield,malphiteshieldeffect,RivenFeint,ShenStandUnited,sionwshieldstacks,vipassivebuff";

            public static string[] ShieldBuffNames = ShieldNames.Split(',');

            public static int GetRendCount(Obj_AI_Base target)
            {
                return target.GetBuffCount("kalistaexpungemarker");
            }


            private static readonly float[] RendBase = new float[] { 20, 30, 40, 50, 60 };
            private const float RendBaseAdRate = .6f;
            private static readonly float[] RendStackBase = { 5, 9, 14, 20, 27 };
            private static readonly float[] RendStackAdRate = { 0.15f, 0.18f, 0.21f, 0.24f, 0.27f };

            //private static float GetRendDamage(Obj_AI_Base target)
            //{
            //    if (!Champion.E.IsReady()) return 0f;
            //    return (float)Player.CalcDamage(target, LeagueSharp.Common.Damage.DamageType.Physical, Champion.E.GetDamage(target) - 20);

            //}

            private static float GetRendDamage(Obj_AI_Base target)
            {
                //var damage = 0f;
                //if (GetRendCount(target) <= 0) return 0f;

                //var eLevel = Champion.E.Level - 1;
                //var baseAd = Player.BaseAttackDamage + Player.FlatPhysicalDamageMod;
                //damage = (RendBase[eLevel] + RendBaseAdRate * baseAd) + ((GetRendCount(target) - 1) * (RendStackBase[eLevel] + RendStackAdRate[eLevel] * baseAd));
                //return (float)Player.CalcDamage(target, LeagueSharp.Common.Damage.DamageType.Physical, damage - (target.FlatHPRegenMod / 2));

                if (!Champion.E.IsReady()) return 0f;

                return (float)Player.CalcDamage(target, LeagueSharp.Common.Damage.DamageType.Physical, Champion.E.GetDamage(target) - (target.FlatHPRegenMod / 2 + 15) );
            }

            public static float GetShield(Obj_AI_Base target)
            {              
                return ShieldBuffNames.Any(target.HasBuff) ? target.AllShield : 0;
            }

            public static float CalculateRendDamage(Obj_AI_Base target)
            {
                var defuffer = 1f;

                if (target.HasBuff("FerociousHowl") || target.HasBuff("GarenW"))
                    defuffer *= .7f;

                if (target.HasBuff("Medidate"))
                    defuffer *= .5f - target.Spellbook.GetSpell(SpellSlot.E).Level * .05f;

                if (target.HasBuff("gragaswself"))
                    defuffer *= .9f - target.Spellbook.GetSpell(SpellSlot.W).Level * .02f;

                if (target.Name.Contains("Baron") && Player.HasBuff("barontarget"))
                    defuffer *= 0.5f;

                if (target.Name.Contains("Dragon") && Player.HasBuff("s5test_dragonslayerbuff"))
                    defuffer *= (1 - (.07f * Player.GetBuffCount("s5test_dragonslayerbuff")));

                if (Player.HasBuff("summonerexhaust"))
                    defuffer *= .4f;


                if (!target.IsChampion()) return (GetRendDamage(target) * defuffer);

                var healthDebuffer = 0f;
                var hero = (AIHeroClient)target;

                if (hero.ChampionName == "Blitzcrank" && !target.HasBuff("BlitzcrankManaBarrierCD") && !target.HasBuff("ManaBarrier"))
                    healthDebuffer += target.Mana / 2;

                return (GetRendDamage(target) * defuffer) - (healthDebuffer + GetShield(target) + /*target.FlatHPRegenMod +*/ 15);
            }

            #endregion Private Functions
        }
    }
}
