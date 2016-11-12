using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Activator.Spells.Shields
{
    class fioraw : CoreSpell
    {
        internal override string Name => "fioraw";
        internal override string DisplayName => "Riposte | W";
        internal override float Range => 750f;
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
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger) && hero.Attacker != null)
                                UseSpellTo(hero.Attacker.ServerPosition);

                    if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate) && hero.Attacker != null)
                                UseSpellTo(hero.Attacker.ServerPosition);

                    if (ShouldUseOnMany(hero) && hero.Attacker != null)
                        UseSpellTo(hero.Attacker.ServerPosition);
                }
            }
        }
    }
}
