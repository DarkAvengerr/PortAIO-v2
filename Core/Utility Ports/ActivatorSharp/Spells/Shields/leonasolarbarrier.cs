using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Activator.Spells.Shields
{
    class leonasolarbarrier: CoreSpell
    {
        internal override string Name => "leonasolarbarrier";
        internal override string DisplayName => "Eclipse | W";
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.SelfMinMP, MenuType.SelfMuchHP };
        internal override int DefaultHP => 95;
        internal override int DefaultMP => 45;
        internal override int Priority => 3;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            if (Player.Mana / Player.MaxMana * 100 <
                Menu.Item("selfminmp" + Name + "pct").GetValue<Slider>().Value)
                return;

            foreach (var hero in Activator.Allies())
            {
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (ShouldUseOnMany(hero))
                        UseSpell();
                }
            }
        }
    }
}
