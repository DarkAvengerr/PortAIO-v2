using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Spells.Evaders
{
    class maokaiunstablegrowth : CoreSpell
    {
        internal override string Name => "maokaiunstablegrowth";
        internal override string DisplayName => "Twisted Advance | W";
        internal override float Range => 525f;
        internal override MenuType[] Category => new[] { MenuType.SpellShield, MenuType.Zhonyas, MenuType.SelfMinMP };
        internal override int DefaultHP => 0;
        internal override int DefaultMP => 45;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            if (Player.Mana/Player.MaxMana*100 <
                Menu.Item("selfminmp" + Name + "pct").GetValue<Slider>().Value)
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Attacker == null || hero.Player.NetworkId != Player.NetworkId)
                    continue;

                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Attacker.LSDistance(hero.Player.ServerPosition) > Range)
                    continue;

                if (Menu.Item("ss" + Name + "all").GetValue<bool>())
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Spell))
                        CastOnBestTarget((AIHeroClient)hero.Attacker);

                if (Menu.Item("ss" + Name + "cc").GetValue<bool>())
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.CrowdControl))
                        CastOnBestTarget((AIHeroClient)hero.Attacker);

                if (Menu.Item("use" + Name + "norm").GetValue<bool>())
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                        CastOnBestTarget((AIHeroClient) hero.Attacker);

                if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                    if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                        CastOnBestTarget((AIHeroClient)hero.Attacker);     
     
            }
        }
    }
}
