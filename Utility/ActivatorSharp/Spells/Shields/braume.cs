using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Spells.Shields
{
    class bruame : CoreSpell
    {
        internal override string Name => "braume";
        internal override string DisplayName => "Unbreakable | E";
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.SelfMuchHP, MenuType.Zhonyas, MenuType.SpellShield };
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

                if (hero.Player.LSDistance(Player.ServerPosition) <= hero.Player.BoundingRadius)
                {
                    if (hero.IncomeDamage/hero.Player.MaxHealth*100 >=
                        Menu.Item("selfmuchhp" + Name + "pct").GetValue<Slider>().Value)
                        if (hero.Attacker != null)
                            UseSpellTowards(hero.Attacker.ServerPosition);

                    if (Menu.Item("ss" + Name + "all").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Spell))
                            if (hero.Attacker != null)
                                UseSpellTowards(hero.Attacker.ServerPosition);

                    if (Menu.Item("use" + Name + "norm").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                            if (hero.Attacker != null)
                                UseSpellTowards(hero.Attacker.ServerPosition);

                    if (Menu.Item("ss" + Name + "cc").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.CrowdControl))
                            if (hero.Attacker != null)
                                UseSpellTowards(hero.Attacker.ServerPosition);

                    if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                            if (hero.Attacker != null)
                                UseSpellTowards(hero.Attacker.ServerPosition);
                }
            }
        }
    }
}
