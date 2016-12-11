using System;
using Activator.Base;
using LeagueSharp.Common;
using LeagueSharp;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Activator.Items.Defensives
{
    class _3107 : CoreItem 
    {
        internal override int Id => 3107;
        internal override int Priority => 5;
        internal override sealed string Name => "Redemption";
        internal override string DisplayName => "Redemption";
        internal override int Duration => 250;
        internal override sealed float Range => 5500;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfMuchHP };
        internal override MapType[] Maps => new[] { MapType.SummonersRift, MapType.HowlingAbyss };
        internal override int DefaultHP => 40;
        internal override int DefaultMP => 0;

        internal Spell Redemption;

        public _3107()
        {
            Redemption = new Spell(Player.GetSpellSlot("itemredemption"), Range);
            Redemption.SetSkillshot(2.5f, 550f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            EloBuddy.Player.OnSwapItem += (sender, args) =>
            {
                if (sender.IsMe)
                {
                    Redemption = new Spell(Player.GetSpellSlot("itemredemption"), Range);
                }
            };
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.Distance(Player.ServerPosition) <= Range)
                {
                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                    {
                        if (hero.TowerDamage > 0 || hero.IncomeDamage > 0 || hero.MinionDamage > hero.Player.Health)
                        {
                            if (!hero.Player.InFountain() && !hero.Player.IsRecalling())
                            {
                                if (Redemption.CastIfHitchanceEquals(hero.Player, HitChance.VeryHigh))
                                {
                                    if (!hero.Player.Position.IsOnScreen())
                                    {
                                        Chat.Print( "<b><font color=\"#FF3366\">Activator#</font></b> -" +
                                                        "<font color=\"#FFF280\"> Redemption </font> casted on " + hero.Player.ChampionName + "!");
                                    }
                                }
                            }
                        }
                    }

                    if (ShouldUseOnMany(hero))
                    {
                        if (!hero.Player.InFountain() && !hero.Player.IsRecalling())
                        {
                            if (Redemption.CastIfHitchanceEquals(hero.Player, HitChance.VeryHigh))
                            {
                                if (!hero.Player.Position.IsOnScreen())
                                {
                                    Chat.Print("<b><font color=\"#FF3366\">Activator#</font></b> -" +
                                                    "<font color=\"#FFF280\"> Redemption </font> casted on " + hero.Player.ChampionName + "!");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
