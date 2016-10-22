using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Slutty_Lucian
{
    class Program
    {
        public static void Main()
        {
            try
            {
                Lucian.OnLoad();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An Error Has Occured {0} " + ex);
            }
        }
    }
}
