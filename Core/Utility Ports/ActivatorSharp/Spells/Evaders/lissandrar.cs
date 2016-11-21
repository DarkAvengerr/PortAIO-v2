using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Activator.Spells.Evaders
{
    class lissandrar : CoreSpell
    {
        internal override string Name => "lissandrar";
        internal override string DisplayName => "Frozen Tomb | R";
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.Zhonyas, MenuType.SelfMuchHP };
        internal override int DefaultHP => 30;
        internal override int DefaultMP => 0;
        internal override int Priority => 7;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
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
}
