using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Activator.Spells.Evaders
{
    class fizzjump : CoreSpell
    {
        internal override string Name => "fizzjump";
        internal override string DisplayName => "Playful / Trickster | E";
        internal override float Range => 1000f;
        internal override MenuType[] Category => new[] { MenuType.Zhonyas, MenuType.SelfMinMP };
        internal override int DefaultHP => 30;
        internal override int DefaultMP => 45;
        internal override int Priority => 5;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            if (Player.Mana/Player.MaxMana*100 <
                Menu.Item("selfminmp" + Name + "pct").GetValue<Slider>().Value)
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                        continue;

                    if (Menu.Item("use" + Name + "norm").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                            UseSpellTo(Game.CursorPos);

                    if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                            UseSpellTo(Game.CursorPos);
                }
            }
        }
    }
}
