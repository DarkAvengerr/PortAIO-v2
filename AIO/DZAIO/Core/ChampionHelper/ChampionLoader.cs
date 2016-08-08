using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Core.ChampionHelper
{
    class ChampionLoader
    {
         private static Random random = new Random();

        public static void LoadChampion()
        {
            if (Variables.ChampList.ContainsKey(ObjectManager.Player.ChampionName))
            {
                Variables.CurrentChampion = Variables.ChampList[ObjectManager.Player.ChampionName].Invoke();
                Variables.CurrentChampion.OnLoad(Variables.AssemblyMenu);
                Variables.CurrentChampion.RegisterEvents();
                Chat.Print("<b><font color='#FF0000'>[DZAIO: Reborn] </font></b><font color='#FFFFFF'>Loaded</font> <b><font color='#FF0000'>{0}</font></b> plugin!", ObjectManager.Player.ChampionName);

                var champString = Variables.ChampList.Where(n => n.Key != ObjectManager.Player.ChampionName).Aggregate("<b>Also try it with: </b>", (current, champKvp) => current + ("<b><font color='" + GetRandomColor() + "'>" + champKvp.Key + "</font></b><font color='#FFFFFF'>, </font>"));

                Chat.Print(champString);
            }
        }

        public static string GetRandomColor()
        {
            var color = $"#{random.Next(0x1000000):X6}";

            return color;
        }
    }
}
