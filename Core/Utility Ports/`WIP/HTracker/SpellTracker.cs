using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Core;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HTrackerSDK
{
    class SpellTracker
    {
        private static readonly SpellSlot[] Spells =
        {
            SpellSlot.Q,
            SpellSlot.W,
            SpellSlot.E,
            SpellSlot.R
        };
        private static readonly SpellSlot[] Summoners =
        {
            SpellSlot.Summoner1,
            SpellSlot.Summoner2,
        };

        public static string[] SummonersNames =
        {
            "summonerbarrier", "summonerboost", "summonerdot", "summonerexhaust",
            "summonerflash", "summonerhaste", "summonerheal", "summonerodinGarrison",
            "summonerteleport", 
        };

        private static int _x, _y;

        private static int GetCooldownExpires(Obj_AI_Base hero, SpellSlot slot)
        {
            return (int)hero.Spellbook.GetSpell(slot).CooldownExpires;
        }

        public static string GetSummonerName(Obj_AI_Base hero, SpellSlot slot)
        {
            return hero.Spellbook.GetSpell(slot).SData.Name;
        }

        public static int GetSummonerExpires(Obj_AI_Base hero, SpellSlot slot)
        {
            return (int)hero.Spellbook.GetSpell(slot).CooldownExpires;
        }

        public static int GetSpellLevel(Obj_AI_Base hero, SpellSlot slot)
        {
            return hero.Spellbook.GetSpell(slot).Level;
        }

        public static string GetSName(Obj_AI_Base hero, SpellSlot slot)
        {
            switch (hero.Spellbook.GetSpell(slot).SData.Name)
            {
                case "summonerflash":
                    return "F:";
                case "summonerheal":
                    return "H:";
                case "summonerbarrier":
                    return "B: ";
                case "summonerexhaust":
                    return "E: ";
                case "summonerdot":
                    return "I: ";
                case "summonerteleport: ":
                    return "TP: ";
                case "s5_summonersmiteduel":
                    return "S: ";
                case "s5_summonersmiteplayerganker":
                    return "S: ";
                case "s5_summonersmitequick":
                    return "S: ";
                case "itemsmiteaoe":
                    return "S: ";
            }
            return "U: ";
        }

        public static void PlayerTracker()
        {
            foreach (var ally in GameObjects.AllyHeroes.Where(x => x.IsVisible && x.IsValid && !x.IsDead && x.IsMe))
            {
                for (var i = 0; i < Spells.Length; i++)
                {
                    _x = (int)ally.HPBarPosition.X + 30 * i;
                    _y = (int)ally.HPBarPosition.Y + 50;
                    Drawing.DrawText(_x + 36, _y - 25, Color.Gold, Spells[i].ToString());

                    var cooldown = Convert.ToInt32(GetCooldownExpires(ally, Spells[i]) - Game.Time - 1);
                    if (cooldown > 0)
                    {
                        Drawing.DrawText(_x + 36, _y - 10, Color.White, "" + cooldown);
                    }
                    else
                    {
                        Drawing.DrawText(_x + 36, _y - 10, Color.White, "0");
                    }
                }
                for (var i = 0; i < Summoners.Length; i++)
                {
                    _x = (int)ally.HPBarPosition.X + 80 * i;
                    _y = (int)ally.HPBarPosition.Y + 50;
                    var cooldown = Convert.ToInt32(GetCooldownExpires(ally, Summoners[i]) - Game.Time - 1);
                    Drawing.DrawText(_x + 23, _y - 60, Color.Gold, "" +GetSName(ally, Summoners[i]));
                    if (cooldown > 0)
                    {
                        Drawing.DrawText(_x + 40, _y - 60, Color.White, "" + cooldown);
                    }
                    else
                    {
                        Drawing.DrawText(_x + 40, _y - 60, Color.White, "0");
                    }
                }
            }
        }

        public static void EnemyTracker()
        {
            foreach (var ally in GameObjects.EnemyHeroes.Where(x => x.IsVisible && x.IsValid && !x.IsDead))
            {
                for (var i = 0; i < Spells.Length; i++)
                {
                    _x = (int)ally.HPBarPosition.X + 30 * i;
                    _y = (int)ally.HPBarPosition.Y + 50;
                    Drawing.DrawText(_x + 10, _y - 15, Color.Gold, ""+Spells[i]);
                    var cooldown = Convert.ToInt32(GetCooldownExpires(ally, Spells[i]) - Game.Time - 1);
                    if (cooldown > 0)
                    {
                        Drawing.DrawText(_x + 10, _y, Color.White, "" + cooldown);
                        
                    }
                    else
                    {
                        Drawing.DrawText(_x + 10, _y, Color.White, "0");
                    }
                }
                for (var i = 0; i < Summoners.Length; i++)
                {
                    _x = (int)ally.HPBarPosition.X + 80 * i;
                    _y = (int)ally.HPBarPosition.Y + 50;
                    var cooldown = Convert.ToInt32(GetCooldownExpires(ally, Summoners[i]) - Game.Time - 1);
                    Drawing.DrawText(_x + 23, _y - 50, Color.Gold, "" + GetSName(ally, Summoners[i]));
                    if (cooldown > 0)
                    {
                        Drawing.DrawText(_x + 40, _y + -50, Color.White, "" + cooldown);
                    }
                    else
                    {
                        Drawing.DrawText(_x + 40, _y + -50, Color.Gold, "0");
                    }
                }
            }
        }

        public static void AllyTracker()
        {
            foreach (var ally in GameObjects.AllyHeroes.Where(x => x.IsVisible && x.IsValid && !x.IsDead && !x.IsMe))
            {
                for (var i = 0; i < Spells.Length; i++)
                {
                    _x = (int)ally.HPBarPosition.X + 30 * i;
                    _y = (int)ally.HPBarPosition.Y + 50;
                    Drawing.DrawText(_x + 10, _y - 15, Color.Gold, ""+Spells[i]);

                    var cooldown = Convert.ToInt32(GetCooldownExpires(ally, Spells[i]) - Game.Time - 1);
                    if (cooldown > 0)
                    {
                        Drawing.DrawText(_x + 10, _y, Color.White, "" + cooldown);
                    }
                    else
                    {
                        Drawing.DrawText(_x + 10, _y, Color.White, "0");
                    }
                }
                for (var i = 0; i < Summoners.Length; i++)
                {
                    _x = (int)ally.HPBarPosition.X + 80 * i;
                    _y = (int)ally.HPBarPosition.Y + 50;
                    var cooldown = Convert.ToInt32(GetCooldownExpires(ally, Summoners[i]) - Game.Time - 1);
                    Drawing.DrawText(_x + 23, _y - 50, Color.Gold, "" + GetSName(ally, Summoners[i]));
                    if (cooldown > 0)
                    {
                        Drawing.DrawText(_x + 30, _y + -50, Color.White, "" + cooldown);
                    }
                    else
                    {
                        Drawing.DrawText(_x + 40, _y + -50, Color.White, "0");
                    }
                }
            }
        }
    }
}
