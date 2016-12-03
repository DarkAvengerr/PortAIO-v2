using System;
using System.Linq;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Activator.Summoners
{
    class snowball : CoreSum
    {
        internal override sealed string Name => "summonersnowball";
        internal override string DisplayName => "Mark";
        internal override string[] ExtraNames => new[] { "" };
        internal override sealed float Range => 1500f;
        internal override int Duration => 100;
        internal override int Priority => 3;

        private static Spell _mark;

        public snowball()
        {
            _mark = new Spell(Player.GetSpellSlot(Name), Range);
            _mark.SetSkillshot(0f, 60f, 1500f, true, SkillshotType.SkillshotLine);
        }

        public override void AttachMenu(Menu menu)
        {
            Activator.UseEnemyMenu = true;
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !_mark.IsReady())
                return;

            if (Player.GetSpell(_mark.Slot).Name.ToLower() != Name)
                return;

            foreach (var tar in Activator.Heroes.Where(hero => hero.Player.IsValidTarget(Range)))
            {
                if (Parent.Item(Parent.Name + "useon" + tar.Player.NetworkId).GetValue<bool>())
                    _mark.CastIfHitchanceEquals(tar.Player, HitChance.Medium);
            }
        }
    }
}
