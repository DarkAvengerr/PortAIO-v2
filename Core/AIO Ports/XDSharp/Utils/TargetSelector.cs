using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace XDSharp.Utils
{
    class TargetSelector
    {
        public static List<AIHeroClient> Targets = new List<AIHeroClient>();
        public static TargetingMode TMode = TargetingMode.FastKill;

        private static string[] p1 = new string[] { "Alistar", "Amumu", "Bard", "Blitzcrank", "Braum", "Cho'Gath", "Dr. Mundo", "Garen", "Gnar",
                "Hecarim", "Janna", "Jarvan IV", "Leona", "Lulu", "Malphite", "Nami", "Nasus", "Nautilus", "Nunu",
                "Olaf", "Rammus", "Renekton", "Sejuani", "Shen", "Shyvana", "Singed", "Sion", "Skarner", "Sona",
                "Soraka", "Taric", "Thresh", "Volibear", "Warwick", "MonkeyKing", "Yorick", "Zac", "Zyra" };

        private static string[] p2 = new string[] { "Aatrox", "Darius", "Elise", "Evelynn", "Galio", "Gangplank", "Gragas", "Irelia", "Jax",
                "Lee Sin", "Maokai", "Morgana", "Nocturne", "Pantheon", "Poppy", "Rengar", "Rumble", "Ryze", "Swain",
                "Trundle", "Tryndamere", "Udyr", "Urgot", "Vi", "XinZhao" };

        private static string[] p3 = new string[] { "Akali", "Diana", "Fiddlesticks", "Fiora", "Fizz", "Heimerdinger", "Jayce", "Kassadin",
                "Kayle", "Kha'Zix", "Lissandra", "Mordekaiser", "Nidalee", "Riven", "Shaco", "Vladimir", "Yasuo",
                "Zilean" };

        private static string[] p4 = new string[] { "Ahri", "Anivia", "Annie", "Ashe", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
                "Ezreal", "Graves", "Jinx", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "LeBlanc", "Lucian",
                "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon", "Teemo",
                "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath", "Zed",
                "Ziggs" };

        public enum TargetingMode
        {
            FastKill = 1,
            AutoPriority = 0
        }

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }


#region Targetlist
        private static double FindPrioForTarget(AIHeroClient enemy, TargetingMode TMode)
        {
            switch (TMode)
            {
                case TargetingMode.AutoPriority:
                    {
                        if (p1.Contains(enemy.BaseSkinName))
                        {
                            return 4;
                        }
                        else if (p2.Contains(enemy.BaseSkinName))
                        {
                            return 3;
                        }
                        else if (p3.Contains(enemy.BaseSkinName))
                        {
                            return 2;
                        }
                        else if (p4.Contains(enemy.BaseSkinName))
                        {
                            return 1;
                        }
                        else
                        {
                            return 5;
                        }
                    }
                case TargetingMode.FastKill:
                    {
                        if (enemy.IsValid && enemy != null && enemy.IsVisible && !enemy.IsDead)
                            return (enemy.Health / Player.GetSpellDamage(enemy, XDSharp.Champions.Main.TSSpell()));
                        else
                            return 1000000;
                    }
                default:
                    return 0;
            }
        }

        public static void Targetlist(TargetingMode TMode)
        {
            int i1, i2;
            AIHeroClient Buf;
            {
                if (Targets.Count != ObjectManager.Get<AIHeroClient>().Where(hero => hero.Team != ObjectManager.Player.Team).Count())
                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(hero => hero.Team != ObjectManager.Player.Team))
                    {
                        Targets.Add(enemy);
                    }


                for (i1 = 0; i1 < Targets.Count; i1++)
                    for (i2 = 0; i2 < Targets.Count; i2++)
                        if (FindPrioForTarget(Targets[i1], TMode) > FindPrioForTarget(Targets[i2], TMode))
                        {
                            Buf = Targets[i2];
                            Targets[i1] = Targets[i1];
                            Targets[i2] = Buf;
                        }
                        else if (FindPrioForTarget(Targets[i1], TMode) < FindPrioForTarget(Targets[i2], TMode))
                        {
                            Buf = Targets[i1];
                            Targets[i1] = Targets[i2];
                            Targets[i2] = Buf;
                        }
            }
        }
#endregion

    }
}
