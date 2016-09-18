using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoElise
{
    public static class BadaoEliseHelper
    {
        public static bool CanUseQHuman()
        {
            return
                BadaoEliseSpellsManager.QHumanReady
                && (BadaoEliseSpellsManager.IsHuman || 
                BadaoMainVariables.R.IsReady())
               ;
        }
        public static bool CanUseWHuman()
        {
            return
                BadaoEliseSpellsManager.WHumanReady
                && (BadaoEliseSpellsManager.IsHuman ||
                BadaoMainVariables.R.IsReady())
               ;
        }
        public static bool CanUseEHuman()
        {
            return
                BadaoEliseSpellsManager.EHumanReady
                && (BadaoEliseSpellsManager.IsHuman ||
                BadaoMainVariables.R.IsReady())
               ;
        }
        public static bool CanUseQSpider()
        {
            return
                BadaoEliseSpellsManager.QSpiderReady
                && (!BadaoEliseSpellsManager.IsHuman ||
                BadaoMainVariables.R.IsReady())
               ;
        }
        public static bool CanUseWSpider()
        {
            return
                BadaoEliseSpellsManager.WSpiderReady
                && (!BadaoEliseSpellsManager.IsHuman ||
                BadaoMainVariables.R.IsReady())
               ;
        }
        public static bool CanUseESpider()
        {
            return
                BadaoEliseSpellsManager.ESpiderReady
                && (!BadaoEliseSpellsManager.IsHuman ||
                BadaoMainVariables.R.IsReady())
               ;
        }
        public static bool UseE1Combo()
        {
            return CanUseEHuman() && BadaoEliseVariables.ComboE.GetValue<bool>();
        }
        public static bool UseE2Combo()
        {
            return CanUseESpider() && BadaoEliseVariables.ComboE2.GetValue<bool>();
        }
        public static bool UseRCombo()
        {
            return BadaoMainVariables.R.IsReady() && BadaoEliseVariables.ComboR.GetValue<bool>();
        }
        public static bool UseQJungleClear()
        {
            return CanUseQHuman() && BadaoEliseVariables.JungleQ.GetValue<bool>()
                && ObjectManager.Player.Mana * 100f / ObjectManager.Player.MaxMana >= 
                BadaoEliseVariables.JungleMana.GetValue<Slider>().Value;
        }
        public static bool UseWJungleClear()
        {
            return CanUseWHuman() && BadaoEliseVariables.JungleW.GetValue<bool>()
                && ObjectManager.Player.Mana * 100f / ObjectManager.Player.MaxMana >=
                BadaoEliseVariables.JungleMana.GetValue<Slider>().Value;
        }
        public static bool UseRJungleClear()
        {
            return BadaoMainVariables.R.IsReady() && BadaoEliseVariables.JungleR.GetValue<bool>();
        }
        //public static bool UseRLaneClear()
        //{
        //    return BadaoMainVariables.R.IsReady() && BadaoEliseVariables.LaneClearR.GetValue<bool>();
        //}
    }

}
