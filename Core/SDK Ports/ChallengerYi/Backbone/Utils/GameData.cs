using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Data.DataTypes;
using LeagueSharp.Data.Enumerations;
using LeagueSharp.SDK;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace ChallengerYi.Backbone.Utils
{
    internal class GameData
    {
        internal static List<ChampionData> Champions;

        internal static void Init()
        {
            Champions = new List<ChampionData>();
            foreach (var champion in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsEnemy))
            {
                var name = champion.CharData.BaseSkinName;
                var championSpells = new List<SpellDatabaseEntry>();
                championSpells =
                    LeagueSharp.SDK.SpellDatabase.Spells.Where(entry => entry.ChampionName == name).ToList();
                var cc =
                    championSpells.Where(
                        entry => entry != null && entry.AppliedBuffsOnEnemies != null &&
                                 entry.AppliedBuffsOnEnemies.Any(
                                     buffType =>
                                         buffType == BuffType.Stun || buffType == BuffType.Knockback ||
                                         buffType == BuffType.Knockup || buffType == BuffType.Polymorph ||
                                         buffType == BuffType.Taunt || buffType == BuffType.Blind))
                        .Select(entry => entry.Slot)
                        .ToList();
                var gapclosers =
                    championSpells.Where(
                        entry =>
                            entry != null && entry.SpellTags != null &&
                            entry.SpellTags.Any(tag => tag == SpellTags.Dash || tag == SpellTags.Blink))
                        .Select(entry => entry.Slot)
                        .ToList();
                /*Let them burn the flash its worth lol
                      var flash =
                        champion.Spellbook.Spells.FirstOrDefault(
                            spell =>
                                spell.Name.ToLower().Contains("flash") || spell.SData.Name.ToLower().Contains("flash"));
                    if (flash != null && (flash.Slot == SpellSlot.Summoner1 || flash.Slot == SpellSlot.Summoner2))
                    {
                        gapclosers.Add(flash.Slot);
                    }*/
                Champions.Add(new ChampionData(name, cc, gapclosers));
            }
        }
    }
}