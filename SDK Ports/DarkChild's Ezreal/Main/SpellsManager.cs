using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DarkEzreal.Main
{
    using System.Collections.Generic;
    using System.Linq;

    using DarkEzreal.Common;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    internal static class SpellsManager
    {
        public static Spell Q, W, E, R;

        public static List<Spell> Spells = new List<Spell>();

        public static void Init()
        {
            Q = new Spell(SpellSlot.Q, 1150);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 475);
            R = new Spell(SpellSlot.R, 3000);

            Q.SetSkillshot(250, 60, 2000, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(250, 80, 1500, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(250, 200, int.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(1000, 160, 2000, false, SkillshotType.SkillshotLine);

            Spells.Add(Q);
            Spells.Add(W);
            Spells.Add(E);
            Spells.Add(R);
        }
    }
}
