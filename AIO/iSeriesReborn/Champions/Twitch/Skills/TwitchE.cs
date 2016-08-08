using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZLib.Logging;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.MenuUtility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Twitch.Skills
{
    internal class TwitchE
    {
        public static void ExecuteLogic()
        {
            if (Variables.spells[SpellSlot.E].IsEnabledAndReady())
            {

                var eHero =
                    HeroManager.Enemies.FirstOrDefault(
                        m => m.LSIsValidTarget(Variables.spells[SpellSlot.E].Range) && m.LSHasBuff("twitchdeadlyvenom"));
                if (eHero != null)
                {
                    var buffCount = eHero.GetBuff("twitchdeadlyvenom").Count;
                    var distance = ObjectManager.Player.LSDistance(eHero);
                    if (buffCount == 6)
                    {
                        Variables.spells[SpellSlot.E].Cast();
                    }
                    else if (distance > Variables.spells[SpellSlot.E].Range * 0.85f && buffCount >= 4)
                    {
                        //The Target is about to leave range and has at least 4 stacks on them.
                        Variables.spells[SpellSlot.E].Cast();
                    }
                }
            }
        }

        public static float GetDamage(AIHeroClient hero)
        {
            var baseDamage = Variables.spells[SpellSlot.E].GetDamage(hero);

            if (ObjectManager.Player.LSHasBuff("summonerexhaust"))
            {
                baseDamage *= 0.4f;
            }

            if (hero.LSHasBuff("FerociousHowl"))
            {
                baseDamage *= 0.35f;
            }

            return baseDamage;
        }
    }
}
