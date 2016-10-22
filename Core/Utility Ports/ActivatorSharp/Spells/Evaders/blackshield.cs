using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Spells.Evaders
{
    class blackshield : CoreSpell
    {
        internal override string Name => "blackshield";
        internal override string DisplayName => "Black Shield | E";
        internal override float Range => 750f;
        internal override MenuType[] Category => new[] { MenuType.Zhonyas, MenuType.SpellShield, MenuType.SelfMinMP };
        internal override int DefaultHP => 0;
        internal override int DefaultMP => 0;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            if (Player.Mana/Player.MaxMana*100 <
                Menu.Item("selfminmp" + Name + "pct").GetValue<Slider>().Value)
                return;

            foreach (var hero in Activator.Allies())
            {
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.Distance(Player.ServerPosition) > Range)
                    continue;

                if (Menu.Item("ss" + Name + "all").GetValue<bool>())
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Spell))
                        UseSpellOn(hero.Player);                 

                if (Menu.Item("ss" + Name + "cc").GetValue<bool>())
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.CrowdControl))
                        UseSpellOn(hero.Player);

                if (Menu.Item("use" + Name + "norm").GetValue<bool>())
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                        UseSpellOn(hero.Player);

                if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                        UseSpellOn(hero.Player);
            }
        }
    }
}
