using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld.Library.STS
{
    public delegate int CompareDelegate(AIHeroClient i1, AIHeroClient i2);

    class STS_DefineModeType
    {
        public int id;
        public string name;
        public CompareDelegate sortfunc;

        public STS_DefineModeType(int id, string name, CompareDelegate sortfunc)
        {
            this.id = id;
            this.name = name;
            this.sortfunc = sortfunc;
        }
    }
}
