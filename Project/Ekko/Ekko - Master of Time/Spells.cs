using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Ekko_master_of_time
{
    internal class Spells
    {
        private Spell _q, _w, _e, _r;
        public Spell Q { get { return _q; } }
        public Spell W { get { return _w; } }
        public Spell E { get { return _e; } }
        public Spell R { get { return _r; } }
        public Spell Ignite = new Spell(SpellSlot.Unknown, 600);
        public Spells()
        {

            _q = new Spell(SpellSlot.Q, 750f);
            _w = new Spell(SpellSlot.W, 1620f);
            _e = new Spell(SpellSlot.E, 400f);
            _r = new Spell(SpellSlot.R, 400f);

            Q.SetSkillshot(0.25f, 60f, 2200f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.5f, 500f, 1000f, false, SkillshotType.SkillshotCircle);

            var ignite = HeroManager.Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "summonerdot");
            if (ignite != null)
                Ignite.Slot = ignite.Slot;
        }
    }
}