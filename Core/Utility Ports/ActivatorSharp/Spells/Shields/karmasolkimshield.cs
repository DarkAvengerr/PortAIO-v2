using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Activator.Spells.Shields
{
    class karmasolkimshield : CoreSpell
    {
        internal override string Name => "karmasolkimshield";
        internal override string DisplayName => "Inspire | E";
        internal override float Range => 800f;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfMinMP, MenuType.SelfMuchHP };
        internal override int DefaultHP => 95;
        internal override int DefaultMP => 55;
        internal override int Priority => 3;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            if (Player.Mana/Player.MaxMana * 100 <
                Menu.Item("selfminmp" + Name + "pct").GetValue<Slider>().Value)
                return;

            foreach (var hero in Activator.Allies())
            {
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue; 

                if (hero.Player.Distance(Player.ServerPosition) <= Range)
                {
                    if (hero.Player.Health/hero.Player.MaxHealth * 100 <=
                        Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0 || hero.MinionDamage > hero.Player.Health)
                            UseSpellOn(hero.Player);
                    }

                    if (ShouldUseOnMany(hero))
                        UseSpellOn(hero.Player);
                }
            }     
        }
    }
}
