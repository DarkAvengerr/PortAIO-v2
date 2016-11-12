using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Activator.Spells.Health
{
    class kindredr : CoreSpell
    {
        internal override string Name => "kindredr";
        internal override string DisplayName => "Lamb's Respite | R";
        internal override float Range => 400f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP };
        internal override int DefaultHP => 20;
        internal override int DefaultMP => 0;
        internal override int Priority => 6;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                {
                    if (hero.Player.Distance(Player.ServerPosition) <= 500)
                    {
                        if (!hero.Player.HasBuffOfType(BuffType.Invulnerability))
                        {
                            if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                                Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                            {
                                if (hero.IncomeDamage > 0)
                                    UseSpellOn(Player);
                            }
                        }
                    }
                }
            }
        }
    }
}
