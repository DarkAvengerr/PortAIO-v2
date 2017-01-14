using EloBuddy; 
using LeagueSharp.Common; 
namespace MoonRiven
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    internal class Logic
    {
        internal static Orbwalking.Orbwalker Orbwalker { get; set; }

        internal static Spell Q { get; set; }
        internal static Spell W { get; set; }
        internal static Spell E { get; set; }
        internal static Spell R { get; set; }
        internal static Spell Ignite { get; set; }
        internal static Spell Flash { get; set; }

        internal static SpellSlot IgniteSlot { get; set; } = SpellSlot.Unknown;
        internal static SpellSlot FlashSlot { get; set; } = SpellSlot.Unknown;

        internal static Menu Menu { get; set; }

        internal static AIHeroClient myTarget { get; set; }
        internal static AIHeroClient Me => ObjectManager.Player;

        internal static bool isRActive => ObjectManager.Player.GetSpell(SpellSlot.R).Name == "RivenIzunaBlade";

        internal static int qStack { get; set; }
        internal static int lastQTime { get; set; }

        internal static readonly Color menuColor = new Color(3, 253, 241);
    }
}
