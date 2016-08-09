using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Spells.Shields
{
    class fioraw : CoreSpell
    {
        internal override string Name => "fioraw";
        internal override string DisplayName => "Riposte | W";
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

                if (hero.Player.Distance(Player.ServerPosition) <= hero.Player.BoundingRadius)
                {
                    if (hero.IncomeDamage >= hero.Player.Health && hero.Attacker != null)
                        UseSpellTowards(hero.Attacker.ServerPosition);

                    if (hero.IncomeDamage / hero.Player.MaxHealth * 100 >=
                        Menu.Item("selfmuchhp" + Name + "pct").GetValue<Slider>().Value && hero.Attacker != null)
                            UseSpellTowards(hero.Attacker.ServerPosition);

                    if (Menu.Item("ss" + Name + "all").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Spell) && hero.Attacker != null)
                            UseSpellTowards(hero.Attacker.ServerPosition);

                    if (Menu.Item("ss" + Name + "cc").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.CrowdControl) && hero.Attacker != null)
                            UseSpellTowards(hero.Attacker.ServerPosition);

                    if (Menu.Item("use" + Name + "norm").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger) && hero.Attacker != null)
                                UseSpellTowards(hero.Attacker.ServerPosition);

                    if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate) && hero.Attacker != null)
                                UseSpellTowards(hero.Attacker.ServerPosition);
                }
            }
        }
    }
}
