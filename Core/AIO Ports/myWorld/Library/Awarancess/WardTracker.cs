using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld.Library.Awarancess
{
    class WardTracker
    {
        static Dictionary<string, Ward> Types = new Dictionary<string, Ward>();

        public WardTracker()
        {

            #region Types

            Types.Add("YellowTrinket", new Ward(Color.Yellow, 60, true));
            

            #endregion

            // TODO
        }
    }

    class Ward
    {
        public System.Drawing.Color color;
        public int duration;
        public bool isWard;
        public Ward(Color color, int duration, bool isWard)
        {
            this.color = color;
            this.duration = duration;
            this.isWard = isWard;
        }
    }
}
