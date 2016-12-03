using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Azir_Free_elo_Machine
{
    class GapcloserList
    {
        private List<GapcloserObj> gapcloserObjl;
     
        public GapcloserList()
        { 
            this.load();
        }

        public void load()
        {
            gapcloserObjl=new List<GapcloserObj>();
            gapcloserObjl.Add(new GapcloserObj("AkaliShadowDance",SpellSlot.R,"Akali"));
            gapcloserObjl.Add(new GapcloserObj("Headbutt", SpellSlot.E, "Alistar"));
            gapcloserObjl.Add(new GapcloserObj("DianaTeleport", SpellSlot.E, "Diana"));//
            gapcloserObjl.Add(new GapcloserObj("IreliaGatotsu", SpellSlot.Q, "Irelia"));
            gapcloserObjl.Add(new GapcloserObj("JaxLeapStrike", SpellSlot.Q, "Jax"));
            gapcloserObjl.Add(new GapcloserObj("JayceToTheSkies", SpellSlot.Q, "Jayce"));
            gapcloserObjl.Add(new GapcloserObj("MaokaiUnstableGrowth", SpellSlot.Q, "Maokai"));
            gapcloserObjl.Add(new GapcloserObj("MonkeyKingNimbus", SpellSlot.Q, "Wukong")); //
            gapcloserObjl.Add(new GapcloserObj("Pantheon_LeapBash", SpellSlot.W, "Pantheon"));
            gapcloserObjl.Add(new GapcloserObj("PoppyHeroicCharge", SpellSlot.E, "Poppy"));
            gapcloserObjl.Add(new GapcloserObj("QuinnE", SpellSlot.E, "Quinn"));
            gapcloserObjl.Add(new GapcloserObj("XenZhaoSweep", SpellSlot.E, "XenZhaoSweep")); //
            gapcloserObjl.Add(new GapcloserObj("blindmonkqtwo", SpellSlot.Q, "Lee Sin"));
            gapcloserObjl.Add(new GapcloserObj("FizzPiercingStrike", SpellSlot.Q, "Fizz"));
            gapcloserObjl.Add(new GapcloserObj("RengarLeap", SpellSlot.Unknown, "`Rengar"));
      
        }

        public GapcloserObj FindGapCloserBy(string champName, SpellSlot spellSlot)
        {
           return gapcloserObjl.FirstOrDefault(x => x.getChampName == champName && x.getSpellSlot == spellSlot);
            
        }
    }
}
