#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Base/Helpers.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System;
using System.IO;
using System.Linq;
using Activator.Data;
using Activator.Handlers;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Activator.Base
{
    internal class Helpers
    {
        /// <summary>
        /// Returns if the matched hero is valid and in the current game.
        /// </summary>
        /// <param name="heroname">The heroname.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool IsEnemyInGame(string heroname)
        {
            return Activator.Heroes.Exists(x => x.Player.ChampionName == heroname && x.Player.IsEnemy);
        }

        /// <summary>
        /// Returns if the minion is an "Epic" minion (baron, dragon, etc)
        /// </summary>
        /// <param name="minion">The minion. </param>
        /// <returns></returns>
        public static bool IsEpicMinion(Obj_AI_Base minion)
        {
            var name = minion.Name;
            return minion is Obj_AI_Minion &&
                  (name.StartsWith("SRU_Baron") || name.StartsWith("SRU_Dragon") ||
                   name.StartsWith("SRU_RiftHerald") || name.StartsWith("TT_Spiderboss"));
        }

        public static bool IsCrab(Obj_AI_Base unit)
        {
            return unit.Name.StartsWith("Sru_Crab");
        }

        /// <summary>
        /// Returns if the minion is a "Large" minion (Red Buff, Blue Buff, etc)
        /// </summary>
        /// <param name="minion">The minion. </param>
        /// <param name="notMini">Check if is mini. </param>
        /// <returns></returns>
        public static bool IsLargeMinion(Obj_AI_Base minion, bool notMini = true)
        {
            var name = minion.Name;
            return minion is Obj_AI_Minion && (notMini && !minion.Name.Contains("Mini")) &&
                   (name.StartsWith("SRU_Blue") || name.StartsWith("SRU_Red") || name.StartsWith("TT_NWraith1.1") ||
                    name.StartsWith("TT_NWraith4.1") || name.StartsWith("TT_NGolem2.1") || name.StartsWith("TT_NGolem5.1") ||
                    name.StartsWith("TT_NWolf3.1") || name.StartsWith("TT_NWolf6.1"));
        }

        /// <summary>
        /// Returns if the minion is a "Small" minion (Razorbeak, Krug, etc)
        /// </summary>
        /// <param name="minion">The minion. </param>
        /// <param name="notMini">Check if is mini. </param>
        /// <returns></returns>
        public static bool IsSmallMinion(Obj_AI_Base minion, bool notMini = true)
        {
            var name = minion.Name;
            return minion is Obj_AI_Minion && (notMini && !minion.Name.Contains("Mini")) &&
                  (name.StartsWith("SRU_Murkwolf") || name.StartsWith("SRU_Razorbeak") ||
                   name.StartsWith("SRU_Gromp") || name.StartsWith("SRU_Krug"));  
        }

        /// <summary>
        /// Will try to Reset income damage if target is not valid.
        /// </summary>
        /// <param name="hero">The hero to reset damage. </param>
        public static void ResetIncomeDamage(Champion hero)
        {
            if (hero.Player.IsZombie || hero.Immunity || !hero.Player.IsValidTarget(float.MaxValue, false))
            {
                hero.Attacker = null;
                hero.BuffDamage = 0;
                hero.TroyDamage = 0;
                hero.AbilityDamage = 0;
                hero.MinionDamage = 0;
                hero.TowerDamage = 0;
                hero.HitTypes.Clear();
            }            
        }

        /// <summary>
        /// Returns the primary role of a hero.
        /// </summary>
        /// <param name="hero"></param>
        /// <returns></returns>
        public static PrimaryRole GetRole(AIHeroClient hero)
        {
            var assassins = new[] // heroes who may use sweepers
            {
                "Ahri", "Akali", "Annie", "Diana", "Ekko", "Elise", "Evelynn", "Fiddlesticks", "Fizz", "Gragas", "Kassadin",
                "Khazix", "Leblanc", "Lissandra", "Nidalee", "Nocturne", "Rengar", "Shaco",
                "Syndra", "Talon", "Zed", "Kindred", "Twistedfate"
            };

            var fighters = new[] // heroes who may not upgrade trinket
            {
                "Aatrox", "Darius", "DrMundo", "Fiora", "Gangplank", "Garen", "Gnar", "Hecarim",
                "Illaoi", "Irelia", "Jax", "Jayce", "Kayle", "Kennen", "LeeSin", "MasterYi", "Mordekaiser", "Nasus", "Olaf", "Pantheon",
                "RekSai", "Renekton", "Riven", "Rumble", "Shyvana", "Skarner", "Teemo", "Trundle", "Tryndamere", "Udyr", "Vi", "Vladimir",
                "Volibear", "Warwick", "Wukong", "XinZhao", "Yasuo", "Yorick", "Katarina"
            };

            var mages = new[] // mage heroes who may prefer farsight orb
            {
                "Anivia", "AurelionSol", "Azir", "Brand", "Cassiopeia", "Heimerdinger", "Karma",
                "Karthus", "Lux", "Malzahar", "Orianna", "Ryze", "Swain", 
                "Veigar", "Velkoz", "Viktor", "Xerath", "Ziggs", "Taliyah"
            };

            var supports = new[]
            {
                "Alistar", "Bard", "Blitzcrank", "Braum", "Janna", "Leona", "Lulu", "Morgana", "Nami", "Nunu",
                "Sona", "Soraka", "TahmKench", "Taric", "Thresh",
                "Zilean", "Zyra", "Ivern"
            };

            var tanks = new[]
            {
                "Amumu", "Chogath", "Galio", "JarvanIV", "Malphite", "Maokai", "Nautilus",
                "Poppy", "Rammus", "Sejuani", "Shen", "Singed", "Sion", "Zac"
            };

            var marksmen = new[] // heroes that will 100% buy farsight orb
            {
                "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jhin", "Jinx", "Kalista",
                "KogMaw", "Lucian", "MissFortune", "Quinn", "Sivir", "Tristana", "Twitch", "Urgot", "Varus",
                "Vayne"
            };

            if (assassins.Contains(hero.ChampionName))
            {
                return PrimaryRole.Assassin;
            }

            if (fighters.Contains(hero.ChampionName))
            {
                return PrimaryRole.Fighter;
            }

            if (mages.Contains(hero.ChampionName))
            {
                return PrimaryRole.Mage;
            }

            if (supports.Contains(hero.ChampionName))
            {
                return PrimaryRole.Support;
            }

            if (tanks.Contains(hero.ChampionName))
            {
                return PrimaryRole.Marksman;
            }

            if (marksmen.Contains(hero.ChampionName))
            {
                return PrimaryRole.Marksman;
            }

            return PrimaryRole.Unknown;
        }

        /// <summary>
        /// Properly updates the Cleanse buff count
        /// </summary>
        /// <param name="player"></param>
        internal static void CheckCleanse(AIHeroClient player)
        {
            foreach (var hero in Activator.Heroes.Where(x => x.Player.NetworkId == player.NetworkId))
            {
                hero.CleanseBuffCount = Buffs.GetAuras(hero.Player, "summonerboost").Count();

                if (hero.CleanseBuffCount > 0)
                {
                    foreach (var buff in Buffs.GetAuras(hero.Player, "summonerboost"))
                    {
                        var duration = (int) (buff.EndTime - buff.StartTime);
                        if (duration > hero.CleanseHighestBuffTime)
                        {
                            hero.CleanseHighestBuffTime = duration * 1000;
                        }
                    }

                    hero.LastDebuffTimestamp = Utils.GameTimeTickCount;
                }

                else
                {
                    if (hero.CleanseHighestBuffTime > 0)
                        hero.CleanseHighestBuffTime -= hero.CleanseHighestBuffTime;
                    else
                        hero.CleanseHighestBuffTime = 0;
                }
            }
        }

        /// <summary>
        /// Properly updates the Dervish buff count
        /// </summary>
        /// <param name="player"></param>
        internal static void CheckDervish(AIHeroClient player)
        {
            foreach (var hero in Activator.Heroes.Where(x => x.Player.NetworkId == player.NetworkId))
            {
                hero.DervishBuffCount = Buffs.GetAuras(hero.Player, "Dervish").Count();

                if (hero.DervishBuffCount > 0)
                {
                    foreach (var buff in Buffs.GetAuras(hero.Player, "Dervish"))
                    {
                        var duration = (int) (buff.EndTime - buff.StartTime);
                        if (duration > hero.DervishHighestBuffTime)
                        {
                            hero.DervishHighestBuffTime = duration * 1000;
                        }
                    }

                    hero.LastDebuffTimestamp = Utils.GameTimeTickCount;
                }

                else
                {
                    if (hero.DervishHighestBuffTime > 0)
                        hero.DervishHighestBuffTime -= hero.DervishHighestBuffTime;
                    else
                        hero.DervishHighestBuffTime = 0;
                }
            }
        }

        /// <summary>
        /// Properly updates the QSS buff count
        /// </summary>
        /// <param name="player"></param>
        internal static void CheckQSS(AIHeroClient player)
        {
            foreach (var hero in Activator.Heroes.Where(x => x.Player.NetworkId == player.NetworkId))
            {
                hero.QSSBuffCount = Buffs.GetAuras(hero.Player, "Quicksilver").Count();

                if (hero.QSSBuffCount > 0)
                {
                    foreach (var buff in Buffs.GetAuras(hero.Player, "Quicksilver"))
                    {
                        var duration = (int) (buff.EndTime - buff.StartTime);
                        if (duration > hero.QSSHighestBuffTime)
                        {
                            hero.QSSHighestBuffTime = duration * 1000;
                        }
                    }

                    hero.LastDebuffTimestamp = Utils.GameTimeTickCount;
                }

                else
                {
                    if (hero.QSSHighestBuffTime > 0)
                        hero.QSSHighestBuffTime -= hero.QSSHighestBuffTime;
                    else
                        hero.QSSHighestBuffTime = 0;
                }
            }
        }

        /// <summary>
        /// Properly updates the Mikaels buff count
        /// </summary>
        /// <param name="player"></param>
        internal static void CheckMikaels(AIHeroClient player)
        {
            foreach (var hero in Activator.Heroes.Where(x => x.Player.NetworkId == player.NetworkId))
            {
                hero.MikaelsBuffCount = Buffs.GetAuras(hero.Player, "Mikaels").Count();

                if (hero.MikaelsBuffCount > 0)
                {
                    foreach (var buff in Buffs.GetAuras(hero.Player, "Mikaels"))
                    {
                        var duration = (int) (buff.EndTime - buff.StartTime);
                        if (duration > hero.MikaelsHighestBuffTime)
                        {
                            hero.MikaelsHighestBuffTime = duration * 1000;
                        }
                    }

                    hero.LastDebuffTimestamp = Utils.GameTimeTickCount;
                }

                else
                {
                    if (hero.MikaelsHighestBuffTime > 0)
                        hero.MikaelsHighestBuffTime -= hero.MikaelsHighestBuffTime;
                    else
                        hero.MikaelsHighestBuffTime = 0;
                }

                foreach (var aura in Auradata.BuffList.Where(au => hero.Player.HasBuff(au.Name)))
                {
                    if (aura.DoT && hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Activator.Origin.Item("useMikaelsdot").GetValue<Slider>().Value)
                    {
                        hero.ForceQSS = true;
                        LeagueSharp.Common.Utility.DelayAction.Add(100, () => hero.ForceQSS = false);
                    }
                }
            }
        }

        /// <summary>
        /// Properly updates the Mercurial buff count
        /// </summary>
        /// <param name="player"></param>
        internal static void CheckMercurial(AIHeroClient player)
        {
            foreach (var hero in Activator.Heroes.Where(x => x.Player.NetworkId == player.NetworkId))
            {
                hero.MercurialBuffCount = Buffs.GetAuras(hero.Player, "Mercurial").Count();

                if (hero.MercurialBuffCount > 0)
                {
                    foreach (var buff in Buffs.GetAuras(hero.Player, "Mercurial"))
                    {
                        var duration = (int) (buff.EndTime - buff.StartTime);
                        if (duration > hero.MercurialHighestBuffTime)
                        {
                            hero.MercurialHighestBuffTime = duration * 1000;
                        }
                    }

                    hero.LastDebuffTimestamp = Utils.GameTimeTickCount;
                }

                else
                {
                    if (hero.MercurialHighestBuffTime > 0)
                        hero.MercurialHighestBuffTime -= hero.MercurialHighestBuffTime;
                    else
                        hero.MercurialHighestBuffTime = 0;
                }
            }
        }

        public static void CreateLogPath()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EloBuddy\\", "Cache", "activator");

            if (!Directory.Exists(path))
                 Directory.CreateDirectory(path);
        }


        public static void ExportSpellData(Gamedata data, string type = null)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EloBuddy\\", "Cache", "activator",
                $"activator_{data.ChampionName.ToLower()}.txt");

            var file = new StreamWriter(path, true);
            if (data.SDataName.Contains("attack"))
            {
                return;
            }

            file.WriteLine(@"#region Spelldata dumper © 2015 Kurisu Solutions");
            file.WriteLine(@"// Dumps spell data from the client into a text file.");
            file.WriteLine(@"// {0}", DateTime.Now.ToString("F"));
            file.WriteLine(@"#endregion");
            file.WriteLine(@"");
            file.WriteLine(@"Spells.Add(new Gamedata");
            file.WriteLine(@"{");
            file.WriteLine(@"    // TargetingType = ""{0}"",", type);
            file.WriteLine(@"    SDataName = ""{0}"",", data.SDataName.ToLower());
            file.WriteLine(@"    ChampionName = ""{0}"",", data.ChampionName.ToLower());
            file.WriteLine(@"    Slot = SpellSlot.{0},", data.Slot);
            file.WriteLine(@"    CastRange = ""{0}"",", data.CastRange);
            file.WriteLine(@"    Radius = ""{0}"",", data.Radius);
            file.WriteLine(@"    Delay = {0}f,", data.Delay);
            file.WriteLine(@"    HitTypes = new[] {{ }},");
            file.WriteLine(@"    FixedRange = true,");
            file.WriteLine(@"    MissileName = """",");
            file.WriteLine(@"    HitTypes = new HitType[] {{ }},");
            file.WriteLine(@"    MissileSpeed = {0},", data.MissileSpeed);
            file.WriteLine(@"});");
            file.WriteLine(@"");
            file.Close();
        }
    }
}
