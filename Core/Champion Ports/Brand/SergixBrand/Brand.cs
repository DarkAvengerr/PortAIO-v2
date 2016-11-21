using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace SergixBrand
{
    class BrandCore : Core
    {
        public BrandCore(string championName, string menuTittle) : base(championName, menuTittle)
        {
            Console.WriteLine("Brand Loaded");
        }
   
    }
}
