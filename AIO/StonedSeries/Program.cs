using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy;
using LeagueSharp.Common;
namespace StonedSeriesAIO
{
    internal class Program
    {
        public static void Main()
        {
            string ChampionSwitch = ObjectManager.Player.ChampionName.ToLowerInvariant();

            Chat.Print("TheKushStyle is @ the PortAIO discord, huge thanks to him for this. Awesome dude! - Berb");

            switch (ChampionSwitch)
            {
                case "akali":
                    new Akali();
                    Chat.Print("<font color='#FF00BF'>Stoned Series {0} Loaded By</font> <font color='#FF0000'>The</font><font color='#FFFF00'>Kush</font><font color='#40FF00'>Style</font>", ChampionSwitch);
                    break;

                case "amumu":
                    new Amumu();
                    Chat.Print("<font color='#FF00BF'>Stoned Series {0} Loaded By</font> <font color='#FF0000'>The</font><font color='#FFFF00'>Kush</font><font color='#40FF00'>Style</font>", ChampionSwitch);
                    break;

                case "drmundo":
                    new DrMundo();
                    Chat.Print("<font color='#FF00BF'>Stoned Series {0} Loaded By</font> <font color='#FF0000'>The</font><font color='#FFFF00'>Kush</font><font color='#40FF00'>Style</font>", ChampionSwitch);
                    break;

                case "jarvaniv":
                    new JarvanIV();
                    Chat.Print("<font color='#FF00BF'>Stoned Series {0} Loaded By</font> <font color='#FF0000'>The</font><font color='#FFFF00'>Kush</font><font color='#40FF00'>Style</font>", ChampionSwitch);
                    break;

                case "ryze":
                    new Ryze();
                    Chat.Print("<font color='#FF00BF'>Stoned Series {0} Loaded By</font> <font color='#FF0000'>The</font><font color='#FFFF00'>Kush</font><font color='#40FF00'>Style</font>", ChampionSwitch);
                    break;

                case "volibear":
                    new Volibear();
                    Chat.Print("<font color='#FF00BF'>Stoned Series {0} Loaded By</font> <font color='#FF0000'>The</font><font color='#FFFF00'>Kush</font><font color='#40FF00'>Style</font>", ChampionSwitch);
                    break;

                default:
                    Chat.Print("{0} not supported in Stoned Series", ChampionSwitch);
                    break;
            }
        }
    }
}
