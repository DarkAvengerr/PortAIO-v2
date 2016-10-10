using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Functions.Calculations
{

    public static class Damage
    {
        #region Public Methods

        public static float CalcRealDamage(Obj_AI_Base target, float fakeDamage)
        {
            if (CheckNoDamageBuffs((AIHeroClient)target))
                return 0f;

            var defuffer=1f;

            if (target.HasBuff("FerociousHowl")||target.HasBuff("GarenW"))
                defuffer*=.7f;

            if (target.HasBuff("Medidate"))
                defuffer*=.5f-target.Spellbook.GetSpell(SpellSlot.E).Level*.05f;

            if (target.HasBuff("gragaswself"))
                defuffer*=.9f-target.Spellbook.GetSpell(SpellSlot.W).Level*.02f;

            if (target.Name.Contains("Baron")&&StaticObjects.Player.HasBuff("barontarget"))
                defuffer*=0.5f;

            if (StaticObjects.Player.HasBuff("summonerexhaust"))
                defuffer*=.4f;

            if (!target.IsChampion())
                return fakeDamage*defuffer;

            var healthDebuffer=0f;
            var hero=(AIHeroClient)target;

            if ((hero.ChampionName=="Blitzcrank")&&!target.HasBuff("BlitzcrankManaBarrierCD")&&!target.HasBuff("ManaBarrier"))
                healthDebuffer+=target.Mana/2;

            return fakeDamage*defuffer-(healthDebuffer+GetShield(target)+target.FlatHPRegenMod+10);
        }

        public static bool CheckNoDamageBuffs(AIHeroClient target)
        {
            foreach (var b in target.Buffs.Where(b => b.IsValidBuff()))
                switch (b.DisplayName)
                {
                    case "Chrono Shift":
                        return true;

                    case "JudicatorIntervention":
                        return true;

                    case "Undying Rage":
                        if (target.ChampionName=="Tryndamere")
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

            return target.HasBuffOfType(BuffType.Invulnerability)||target.HasBuffOfType(BuffType.SpellImmunity);
            // || target.HasBuffOfType(BuffType.SpellShield));
        }

        public static float GetShield(Obj_AI_Base target) {return ShieldBuffNames.Any(target.HasBuff)? target.AllShield : 0;}

        #endregion Public Methods

        #region Private Fields

        private const string ShieldNames=
            "blindmonkwoneshield,evelynnrshield,EyeOfTheStorm,ItemSeraphsEmbrace,JarvanIVGoeldenAegis,KarmaSolKimShield,lulufarieshield,luxprismaticwaveshieldself,manabarrier,mordekaiserironman,nautiluspiercinggazeshield,orianaredactshield,rumbleshieldbuff,Shenstandunitedshield,SkarnerExoskeleton,summonerbarrier,tahmkencheshield,udyrturtleactivation,UrgotTerrorCapacitorActive2,ViktorPowerTransfer,dianashield,malphiteshieldeffect,RivenFeint,ShenStandUnited,sionwshieldstacks,vipassivebuff";

        private static readonly string[] ShieldBuffNames=ShieldNames.Split(',');

        #endregion Private Fields
    }

}