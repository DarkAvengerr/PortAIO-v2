using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanAkali
{
    using KoreanCommon;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Akali : CommonChampion
    {
        public Akali() 
            : base("Korean Akali")
        {
            KoreanAkali.Spells.Load(this);
            CustomMenu.Load(this);
            AkaliFuckingCore = new Core(this);
            ForceUltimate.ForceUltimate = AkaliFuckingCore.Ultimate;
            Draws = new Drawing(this);
        }

        public Core AkaliFuckingCore { get; set; }

        public Drawing Draws { get; set; }
    }

    
}
