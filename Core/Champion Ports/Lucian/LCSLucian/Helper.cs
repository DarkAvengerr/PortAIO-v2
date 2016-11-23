using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace LCS_Lucian
{
    class Helper
    {

        public static bool LEnabled(string menuName)
        {
            return LucianMenu.Config.Item(menuName).GetValue<bool>();
        }

        public static int LSlider(string menuName)
        {
            return LucianMenu.Config.Item(menuName).GetValue<Slider>().Value;
        }

        public static void LucianAntiGapcloser(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs spell)
        {
            if (sender.IsEnemy && spell.End.Distance(ObjectManager.Player.Position) < LucianSpells.E.Range && !spell.SData.IsAutoAttack() && spell.Target.IsMe)
            {
                foreach (var gapclose in AntiGapcloseSpell.GapcloseableSpells.Where(x => spell.SData.Name == ((AIHeroClient)sender).GetSpell(x.Slot).Name).OrderByDescending(c => LSlider("gapclose.slider." + spell.SData.Name)))
                {
                    if (LEnabled("gapclose." + ((AIHeroClient)sender).ChampionName))
                    {
                        LucianSpells.E.Cast(ObjectManager.Player.Position.Extend(spell.End, -LucianSpells.W.Range));
                    }
                }
            }
        }


    }
}