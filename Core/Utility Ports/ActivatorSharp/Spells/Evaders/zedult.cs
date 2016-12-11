using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Activator.Spells.Evaders
{
    class zedult : CoreSpell
    {
        internal override string Name => "ZedR";
        internal override string DisplayName => "Death Mark | R";
        internal override float Range => 625f;
        internal override MenuType[] Category => new[] { MenuType.Zhonyas };
        internal override int Priority => 7;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            if (Player.GetSpell(SpellSlot.R).Name != "ZedR")
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                { 
                    if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                        continue;

                    if (hero.Attacker == null)
                        continue;

                    if (hero.Attacker.Distance(hero.Player.ServerPosition) > Range)
                        continue;

                    if (Menu.Item("use" + Name + "norm").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                            CastOnBestTarget((AIHeroClient) hero.Attacker);

                    if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                        if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                            CastOnBestTarget((AIHeroClient) hero.Attacker);
                }
            }
        }
    }
}
