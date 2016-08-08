using System;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Summoners
{
    internal class mana : CoreSum
    {
        internal override string Name => "summonermana";
        internal override string DisplayName => "Clarity";
        internal override string[] ExtraNames => new[] { "" };
        internal override float Range => 600f;
        internal override int Duration => 1000;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.MaxMana <= 200 || hero.Player.LSDistance(Player.ServerPosition) > Range)
                    continue;

                if (hero.Player.Mana/hero.Player.MaxMana*100 <= Menu.Item("selflowmp" + Name + "pct").GetValue<Slider>().Value)
                {
                    if (!hero.Player.LSIsRecalling() && !hero.Player.LSInFountain())
                        UseSpell();
                }
            }
        }
    }
}
