using System;
using System.Linq;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Activator.Spells.Evaders
{
    class blackshield : CoreSpell
    {
        internal override string Name => "blackshield";
        internal override string DisplayName => "Black Shield | E";
        internal override float Range => 750f;
        internal override MenuType[] Category => new[] { MenuType.Zhonyas, MenuType.SelfMinMP, MenuType.SelfMuchHP };
        internal override int DefaultHP => 0;
        internal override int DefaultMP => 0;
        internal override int Priority => 5;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            if (Player.Mana/Player.MaxMana * 100 <
                Menu.Item("selfminmp" + Name + "pct").GetValue<Slider>().Value)
                return;

            foreach (var hero in Activator.Allies())
            {
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.Distance(Player.ServerPosition) > Range)
                    continue;

                if (Menu.Item("use" + Name + "norm").GetValue<bool>())
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                        UseSpellOn(hero.Player);

                if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                        UseSpellOn(hero.Player);

                if (ShouldUseOnMany(hero))
                    UseSpellOn(hero.Player);
            }
        }
    }
}
