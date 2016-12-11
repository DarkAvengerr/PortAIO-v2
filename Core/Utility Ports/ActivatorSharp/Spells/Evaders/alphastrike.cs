using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Activator.Spells.Evaders
{
    class alphastrike : CoreSpell
    {
        internal override string Name => "alphastrike";
        internal override string DisplayName => "Alpha Strike | Q";
        internal override float Range => 600f;
        internal override MenuType[] Category => new[] { MenuType.Zhonyas, MenuType.SelfMuchHP };
        internal override int Priority => 6;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (hero.Attacker == null)
                        continue;

                    if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                        continue;

                    if (hero.Attacker.Distance(hero.Player.ServerPosition) <= Range)
                    {
                        if (Menu.Item("ss" + Name + "all").GetValue<bool>())
                            if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Spell))
                                CastOnBestTarget((AIHeroClient) hero.Attacker, true);

                        if (Menu.Item("ss" + Name + "cc").GetValue<bool>())
                            if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.CrowdControl))
                                CastOnBestTarget((AIHeroClient) hero.Attacker, true);

                        if (Menu.Item("use" + Name + "norm").GetValue<bool>())
                            if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                                CastOnBestTarget((AIHeroClient) hero.Attacker, true);

                        if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                            if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                                CastOnBestTarget((AIHeroClient) hero.Attacker, true);

                        if (ShouldUseOnMany(hero))
                            CastOnBestTarget((AIHeroClient)hero.Attacker, true);
                    }
                }
            }
        }
    }
}
