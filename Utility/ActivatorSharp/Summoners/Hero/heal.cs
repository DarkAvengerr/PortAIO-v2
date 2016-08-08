using System;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Summoners
{
    internal class heal : CoreSum
    {
        internal override string Name => "summonerheal";
        internal override string DisplayName => "Heal";
        internal override string[] ExtraNames => new[] { "" };
        internal override float Range => 850f;
        internal override int Duration => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.LSDistance(Player.ServerPosition) <= Range)
                {
                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0 && !hero.Player.LSIsRecalling() && !hero.Player.LSInFountain())
                            UseSpell(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);

                        if (hero.TowerDamage > 0 && Menu.Item("use" + Name + "tower").GetValue<bool>())
                            UseSpell(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                    }

                    if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                        Menu.Item("selfmuchhp" + Name + "pct").GetValue<Slider>().Value)
                    {
                        if (hero.Player.MaxHealth - hero.Player.Health > 75 + 15 * Math.Min(Activator.Player.Level, 18))
                        {
                            if (!hero.Player.LSIsRecalling() && !hero.Player.LSInFountain())
                                UseSpell(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                        }
                    }
                }
            }
        }
    }
}
