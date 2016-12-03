using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Activator.Spells.Shields
{
    class bruame : CoreSpell
    {
        internal override string Name => "braume";
        internal override string DisplayName => "Unbreakable | E";
        internal override float Range => 1000;
        internal override MenuType[] Category => new[] { MenuType.Zhonyas, MenuType.SelfMuchHP };
        internal override int DefaultHP => 95;
        internal override int DefaultMP => 55;
        internal override int Priority => 3;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {                    
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.Distance(Player.ServerPosition) <= hero.Player.BoundingRadius)
                {
                    if (Menu.Item("use" + Name + "norm").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                            if (hero.Attacker != null)
                                UseSpellTo(hero.Attacker.ServerPosition);

                    if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                            if (hero.Attacker != null)
                                UseSpellTo(hero.Attacker.ServerPosition);

                    if (ShouldUseOnMany(hero) && hero.Attacker != null)
                        UseSpellTo(hero.Attacker.ServerPosition);
                }
            }
        }
    }
}
