using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Activator.Spells.Heals
{
    class sorakaw : CoreSpell
    {
        internal override string Name => "sorakaw";
        internal override string DisplayName => "Astral Infusion | W";
        internal override float Range => 550f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfMinHP };
        internal override int DefaultHP => 90;
        internal override int DefaultMP => 55;
        internal override int Priority => 4;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            if (Player.Health/Player.MaxHealth * 100 <
                Menu.Item("selfminhp" + Name + "pct").GetValue<Slider>().Value)
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.IsMe)
                    continue;

                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.Distance(Player.ServerPosition) <= Range)
                {
                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                        UseSpellOn(hero.Player);
                }
            }
        }
    }
}
