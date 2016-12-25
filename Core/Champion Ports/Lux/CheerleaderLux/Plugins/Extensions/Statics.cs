using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace CheerleaderLux.Extensions
{
    public class Statics
    {
        public static Menu Config;
        public static readonly List<Obj_AI_Base> Attackers = new List<Obj_AI_Base>();
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q1;
        public static Spell E1;
        public static Spell W1;
        public static Spell R1;
        public static GameObject Lux_E;
        public static SpellSlot Ignite;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public static readonly AIHeroClient Player = ObjectManager.Player;
    }
}
