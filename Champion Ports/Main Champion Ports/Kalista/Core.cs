using System;
using LeagueSharp;
using LeagueSharp.Common;
using S_Plus_Class_Kalista.Handlers;
using S_Plus_Class_Kalista.Libaries;
using EloBuddy;

namespace S_Plus_Class_Kalista
{
    public class Core
    {
        //This Bufftype thing by Hoes
        public static readonly BuffType[] Bufftype =
        {
            BuffType.Snare,
            BuffType.Blind,
            BuffType.Charm,
            BuffType.Stun,
            BuffType.Fear,
            BuffType.Slow,
            BuffType.Taunt,
            BuffType.Suppression
        };

        private const string MenuName = "S+ Class Kalista";
        public static bool MenuLoaded = false;
        public static Menu SMenu { get; set; } = new Menu(MenuName, MenuName, true);
        public static AIHeroClient Player => ObjectManager.Player;
      
        public static Orbwalking.Orbwalker CommonOrbwalker { get; set; }
      //  public static LukeSkywalker.Orbwalker LukeOrbwalker { get; set; }
        public static AIHeroClient SoulBoundHero { get; set; }
       
        public class Time
        {
            private static readonly DateTime AssemblyLoadTime = DateTime.Now;
            public static float LastTick { get; set; } = TickCount;
            public static float TickCount => (int)DateTime.Now.Subtract(AssemblyLoadTime).TotalMilliseconds;
            public static bool CheckLast()
            {
                return TickCount - LastTick > (1000 + Game.Ping / 2);
            }
        }

        public class Champion
        {
            public static Spell Q { get; set; }
            public static Spell W { get; set; }
            public static Spell E { get; set; }
            public static Spell R { get; set; }
            public AIHeroClient SoulBound { get; set; }

            public static void Load()
            {
                //Loads range and shit
                Q = new Spell(SpellSlot.Q, 1150);
                W = new Spell(SpellSlot.W, 5200);
                E = new Spell(SpellSlot.E, 950);
                R = new Spell(SpellSlot.R, 1200);

                Q.SetSkillshot(0.25f, 40f, 1200f, true, SkillshotType.SkillshotLine);
                R.SetSkillshot(0.50f, 1500f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            }
        }

    }
}
