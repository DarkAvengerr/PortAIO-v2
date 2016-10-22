using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Slutty_Thresh
{
    internal class Program
    {
        public static void Main()
        {
            try
            {
                SluttyThresh.OnLoad();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }
    }
}