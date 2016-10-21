using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Spells.Shields
{
    class luxprismaticwave : CoreSpell
    {
        internal override string Name => "luxprismaticwave";
        internal override string DisplayName => "Prismatic Barrier | W";
        internal override float Range => 1075f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP, MenuType.SelfMinMP };
        internal override int DefaultHP => 95;
        internal override int DefaultMP => 55;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.Distance(Player.ServerPosition) <= Range)
                {
                    if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                        Menu.Item("selfmuchhp" + Name + "pct").GetValue<Slider>().Value)
                            UseSpellTowards(Prediction.GetPrediction(hero.Player, 0.25f).UnitPosition);

                    if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                        Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0 || hero.MinionDamage > hero.Player.Health)
                            UseSpellTowards(Prediction.GetPrediction(hero.Player, 0.25f).UnitPosition);
                    }
                }
            }
        }
    }
}
