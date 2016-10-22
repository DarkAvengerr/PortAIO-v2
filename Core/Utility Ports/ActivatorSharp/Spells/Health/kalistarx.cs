using System;
using System.Linq;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Spells.Health
{
    internal class kalistarx : CoreSpell
    {
        internal override string Name => "kalistarx";
        internal override string DisplayName => "Fate's Call | R";
        internal override float Range => 1200f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP };
        internal override int DefaultHP => 20;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            var cooptarget =
                ObjectManager.Get<AIHeroClient>()
                    .FirstOrDefault(hero => hero.HasBuff("kalistacoopstrikeally"));

            foreach (var hero in Activator.Allies())
            {
                if (cooptarget?.NetworkId == hero.Player.NetworkId)
                {
                    if (hero.Player.Distance(cooptarget.ServerPosition) <= Range)
                    {
                        if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                            continue;

                        if (!cooptarget.HasBuffOfType(BuffType.Invulnerability))
                        {
                            if (hero.Player.Health/hero.Player.MaxHealth*100 <=
                                Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                            {
                                if (hero.IncomeDamage > 0)
                                    UseSpell();
                            }
                        }
                    }
                }
            }
        }
    }
}
