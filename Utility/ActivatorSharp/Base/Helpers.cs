#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Base/Helpers.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System.Linq;
using Activator.Data;
using Activator.Handlers;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Base
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
        public static void ResetIncomeDamage(AIHeroClient hero)
        {
            foreach (var unit in Activator.Heroes.Where(x => x.Player.NetworkId == hero.NetworkId))
            {
                if (unit.IncomeDamage != 0 && unit.IncomeDamage.ToString().Contains("E")) // Check Expo
                {
                    unit.Attacker = null;
                    unit.IncomeDamage = 0;
                    unit.HitTypes.Clear();
                }

                if (unit.Player.IsZombie || unit.Immunity || !unit.Player.LSIsValidTarget(float.MaxValue, false))
                {
                    unit.Attacker = null;
                    unit.IncomeDamage = 0;
                    unit.HitTypes.Clear();
                }
            }
        }

        /// <summary>
        /// Returns the primary role of a hero.
        /// </summary>
        /// <param name="hero"></param>
        /// <returns></returns>
        public static PrimaryRole GetRole(AIHeroClient hero)
        {
            var assassins = new[] // heroes who use sweepers
            {
                "Ahri", "Akali", "Annie", "Diana", "Ekko", "Elise", "Evelynn", "Fiddlesticks", "Fizz", "Gragas", "Kassadin", "Katarina",
                "Khazix", "Leblanc", "Lissandra", "MasterYi", "Nidalee", "Nocturne", "Rengar", "Shaco",
                "Syndra", "Talon", "Zed", "Kindred"
            };

            var fighters = new[] // heroes who may not upgrade trinket
            {
                "Aatrox", "Darius", "DrMundo", "Fiora", "Gangplank", "Garen", "Gnar", "Hecarim",
                "Illaoi", "Irelia", "Jax", "Jayce", "Kayle", "Kennen", "LeeSin", "Mordekaiser", "Nasus", "Olaf", "Pantheon",
                "RekSai", "Renekton", "Riven", "Rumble", "Shyvana", "Skarner", "Teemo", "Trundle", "Tryndamere", "Udyr", "Vi", "Vladimir",
                "Volibear", "Warwick", "Wukong", "XinZhao", "Yasuo", "Yorick"
            };

            var mages = new[] // mage heroes who may prefer farsight orb
            {
                "Anivia", "AurelionSol", "Azir", "Brand", "Cassiopeia", "Heimerdinger", "Karma",
                "Karthus", "Lux", "Malzahar", "Orianna", "Ryze", "Swain", "Twistedfate",
                "Veigar", "Velkoz", "Viktor", "Xerath", "Ziggs", "Taliyah"
            };

            var supports = new[]
            {
                "Alistar", "Bard", "Blitzcrank", "Braum", "Janna", "Leona", "Lulu", "Morgana", "Nami", "Nunu",
                "Sona", "Soraka", "TahmKench", "Taric", "Thresh",
                "Zilean", "Zyra"
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
                        hero.CleanseHighestBuffTime -= hero.QSSHighestBuffTime;
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

                foreach (var aura in Auradata.BuffList.Where(au => hero.Player.LSHasBuff(au.Name)))
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
    }
}
