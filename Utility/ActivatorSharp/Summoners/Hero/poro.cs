using System;
using System.Linq;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Summoners
{
    class poro : CoreSum
    {
        internal override string Name => "summonerporothrow";
        internal override string DisplayName => "Poro Toss";
        internal override float Range => 1500f;
        internal override int Duration => 100;

        private static Spell mark;

        public poro()
        {
            mark = new Spell(Player.LSGetSpellSlot(Name), Range);
            mark.SetSkillshot(0f, 60f, 1500f, true, SkillshotType.SkillshotLine);
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !mark.LSIsReady())
                return;

            if (Player.GetSpell(mark.Slot).Name.ToLower() != Name)
                return;

            foreach (var tar in Activator.Heroes.Where(hero => hero.Player.LSIsValidTarget(Range)))
            {
                if (Parent.Item(Parent.Name + "useon" + tar.Player.NetworkId).GetValue<bool>())
                    mark.CastIfHitchanceEquals(tar.Player, HitChance.Medium);
            }
        }
    }
}
