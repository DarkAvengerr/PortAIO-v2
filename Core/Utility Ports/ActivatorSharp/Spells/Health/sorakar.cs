using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Activator.Spells.Health
{
    class sorakar : CoreSpell
    {
        internal override string Name => "sorakar";
        internal override string DisplayName => "Wish | R [Global]";
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP };
        internal override int DefaultHP => 15;
        internal override int DefaultMP => 0;
        internal override int Priority => 6;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (!Player.HasBuffOfType(BuffType.Invulnerability))
                {
                    if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                        continue;

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
