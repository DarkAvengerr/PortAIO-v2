using System;
using Activator.Base;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Activator.Summoners
{
    internal class barrier : CoreSum
    {
        internal override string Name => "summonerbarrier";
        internal override string DisplayName => "Barrier";
        internal override string[] ExtraNames => new[] { "" };
        internal override float Range => float.MaxValue;
        internal override int Duration => 100;
        internal override int Priority => 5;

        public override void AttachMenu(Menu menu)
        {
            Activator.UseAllyMenu = true;
            menu.AddItem(new MenuItem("selflowhp" + Name + "pct", "Use on Hero HP (%) <=")).SetValue(new Slider(25));
            menu.AddItem(new MenuItem("use" + Name + "ulti", "Use on Dangerous (Ultimates Only)")).SetValue(true);
            menu.AddItem(new MenuItem("f" + Name, "Always Use Barrier"))
    .SetValue(false).SetTooltip("Will force barrier ultimates ignoring HP% & income damage");
            menu.AddItem(new MenuItem("use" + Name + "tower", "Include Tower Damage")).SetValue(true);
            menu.AddItem(new MenuItem("mode" + Name, "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }, 1));
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId != Player.NetworkId)
                    continue;

                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                    Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                {
                    if (hero.IncomeDamage > 0 && !hero.Player.IsRecalling() && !hero.Player.InFountain())
                        UseSpell(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);

                    if (hero.TowerDamage > 0 && Menu.Item("use" + Name + "tower").GetValue<bool>())
                        UseSpell(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                }

                if (Menu.Item("use" + Name + "ulti").GetValue<bool>())
                {
                    if (hero.HitTypes.Contains(HitType.Ultimate))
                    {
                        if (Menu.Item("f" + Name).GetValue<bool>())
                            UseSpell();

                        else if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                                 Math.Min(100, Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value + 20))
                        {
                            UseSpell();
                        }

                        else if (hero.IncomeDamage >= hero.Player.Health)
                        {
                            UseSpell();
                        }
                    }
                }
            }
        }
    }
}
