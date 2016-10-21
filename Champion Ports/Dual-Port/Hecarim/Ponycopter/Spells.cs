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
 namespace Ponycopter {
    class Spells {
        private static Spell spellQ, spellE, spellW, spellR;

        public static Spell Q {
            get { return spellQ; }
            set { spellQ = value; }
        }

        public static Spell E {
            get { return spellE; }
            set { spellE = value; }
        }

        public static Spell W {
            get { return spellW; }
            set { spellW = value; }
        }

        public static Spell R {
            get { return spellR; }
            set { spellR = value; }
        }
    }
}
