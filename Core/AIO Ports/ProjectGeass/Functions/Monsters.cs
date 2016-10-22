using System.Collections.Generic;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Functions
{

    public static class Monsters
    {
        #region Public Structs

        public struct MonsterBar
        {
            #region Public Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="MonsterBar" /> struct.
            /// </summary>
            /// <param name="barWidth">
            ///     Width of the bar.
            /// </param>
            /// <param name="xoffset">
            ///     The xoffset.
            /// </param>
            /// <param name="yOffsetBegin">
            ///     The y offset begin.
            /// </param>
            /// <param name="yOffsetEnd">
            ///     The y offset end.
            /// </param>
            public MonsterBar(int barWidth, int xoffset, int yOffsetBegin, int yOffsetEnd)
            {
                BarWidth=barWidth;
                XOffset=xoffset;
                YOffsetBegin=yOffsetBegin;
                YOffsetEnd=yOffsetEnd;
            }

            #endregion Public Constructors

            #region Public Properties

            public int BarWidth{get;}
            public int XOffset{get;}
            public int YOffsetBegin{get;}
            public int YOffsetEnd{get;}

            #endregion Public Properties
        }

        #endregion Public Structs

        #region Public Fields

        /// <summary>
        ///     The big monsters
        /// </summary>
        public static string[] BigMonsters={"SRU_Red", "SRU_Blue", "SRU_Dragon", "SRU_Baron", "SRU_Crab", "SRU_Krug", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak"};

        /// <summary>
        ///     The monster bar dictionary
        /// </summary>
        public static Dictionary<string, MonsterBar> MonsterBarDictionary=new Dictionary<string, MonsterBar>
        {
            {"SRU_Red", new MonsterBar(145, 3, 18, 10)}, {"SRU_Blue", new MonsterBar(145, 3, 18, 10)}, {"SRU_Dragon", new MonsterBar(145, 3, 18, 10)}, {"SRU_Baron", new MonsterBar(194, -22, 13, 16)}, {"SRU_Crab", new MonsterBar(61, 45, 34, 3)},
            {"SRU_Krug", new MonsterBar(81, 58, 18, 4)}, {"SRU_Gromp", new MonsterBar(87, 62, 18, 4)}, {"SRU_Murkwolf", new MonsterBar(75, 54, 19, 4)}, {"SRU_Razorbeak", new MonsterBar(75, 54, 18, 4)}
        };

        /// <summary>
        ///     The monster locations
        /// </summary>
        public static Dictionary<string, Vector2> MonsterLocations=new Dictionary<string, Vector2>
        {
            {"Neutral.Dragon", SummonersRift.River.Dragon}, {"Neutral.Baron", SummonersRift.River.Baron}, {"Chaos.Red", new Vector2(7016.869f, 10775.55f)}, {"Chaos.Blue", new Vector2(10931.73f, 6990.844f)},
            {"Order.Red", new Vector2(7862.244f, 4111.187f)}, {"Order.Blue", new Vector2(3871.489f, 7901.054f)}
        };

        #endregion Public Fields
    }

}