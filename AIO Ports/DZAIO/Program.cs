using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Core;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn
{
    class Program
    {
        public static void Main()
        {
            Variables.BootstrapInstance = new Bootstrap();
            Variables.BootstrapInstance.Initialize();
        }
    }
}
