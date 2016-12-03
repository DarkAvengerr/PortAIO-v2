using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Activator.Spells.Health
{
    class threshw : CoreSpell
    {
        internal override string Name => "threshw";
        internal override string DisplayName => "Dark Passage | W";
        internal override float Range => 950f;
        internal override MenuType[] Category => new[] { MenuType.Zhonyas, MenuType.SelfCount, MenuType.SelfMuchHP };
        internal override int DefaultHP => 20;
        internal override int DefaultMP => 0;
        internal override int Priority => 6;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.IsMe)
                    continue;

                if (hero.Player.Distance(Player.ServerPosition) <= Range)
                {
                    if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                        continue;

                    if (!Player.HasBuffOfType(BuffType.Invulnerability))
                    {
                        if (Menu.Item("use" + Name + "norm").GetValue<bool>())
                            if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Danger))
                                if (hero.Attacker != null)
                                    UseSpellTo(Prediction.GetPrediction(hero.Player, 0.25f).UnitPosition);

                        if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                            if (hero.IncomeDamage > 0 && hero.HitTypes.Contains(HitType.Ultimate))
                                if (hero.Attacker != null)
                                    UseSpellTo(Prediction.GetPrediction(hero.Player, 0.25f).UnitPosition);

                        if (hero.Player.CountEnemiesInRange(300) >=
                            Menu.Item("selfcount" + Name).GetValue<Slider>().Value)
                                UseSpellTo(Prediction.GetPrediction(hero.Player, 0.25f).UnitPosition);

                        if (ShouldUseOnMany(hero))
                            UseSpellTo(Prediction.GetPrediction(hero.Player, 0.25f).UnitPosition);
                    }
                }
            }
        }
    }
}
