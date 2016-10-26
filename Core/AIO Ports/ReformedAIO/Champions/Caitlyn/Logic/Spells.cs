namespace ReformedAIO.Champions.Caitlyn.Logic
{
    using System.Collections.Generic;

    using EloBuddy;
    using LeagueSharp.Common;

    internal class Spells
    {
        public static Dictionary<SpellSlot, Spell> Spell;

        public void OnLoad()
        {
            Spell = new Dictionary<SpellSlot, Spell>();

            var q = new Spell(SpellSlot.Q, 1250f);
            q.SetSkillshot(0.65f, 60f, 2200f, false, SkillshotType.SkillshotLine);

            var w = new Spell(SpellSlot.W, 800f);
            w.SetSkillshot(1.5f, 20f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            var e = new Spell(SpellSlot.E, 750f); // This is the real E range.
            e.SetSkillshot(0.30f, 70f, 2000f, true, SkillshotType.SkillshotLine);

            var r = new Spell(SpellSlot.R, 3000f);
            r.SetTargetted(0.7f, 200f);
            

            Spell.Add(q.Slot, q);
            Spell.Add(w.Slot, w);
            Spell.Add(e.Slot, e);
            Spell.Add(r.Slot, r);
        }
    }
}
