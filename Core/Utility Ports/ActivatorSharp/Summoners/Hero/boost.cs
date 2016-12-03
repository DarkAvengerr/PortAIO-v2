using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Activator.Summoners
{
    internal class boost : CoreSum
    {
        internal override string Name => "summonerboost";
        internal override string DisplayName => "Cleanse";
        internal override string[] ExtraNames => new[] { "" };
        internal override float Range => float.MaxValue;
        internal override int Duration => 100;
        internal override int Priority => 6;

        public override void AttachMenu(Menu menu)
        {
            Activator.UseAllyMenu = true;
            var ccmenu = new Menu(DisplayName + " Buff Types", DisplayName.ToLower() + "cdeb");
            ccmenu.AddItem(new MenuItem(Name + "cexh", "Exhaust")).SetValue(false);
            ccmenu.AddItem(new MenuItem(Name + "csupp", "Supression")).SetValue(false);
            ccmenu.AddItem(new MenuItem(Name + "cstun", "Stuns")).SetValue(true);
            ccmenu.AddItem(new MenuItem(Name + "ccharm", "Charms")).SetValue(true);
            ccmenu.AddItem(new MenuItem(Name + "ctaunt", "Taunts")).SetValue(true);
            ccmenu.AddItem(new MenuItem(Name + "cflee", "Flee/Fear")).SetValue(true);
            ccmenu.AddItem(new MenuItem(Name + "csnare", "Snares")).SetValue(true);
            ccmenu.AddItem(new MenuItem(Name + "cpolymorph", "Polymorphs")).SetValue(true);
            ccmenu.AddItem(new MenuItem(Name + "csilence", "Silences")).SetValue(false);
            ccmenu.AddItem(new MenuItem(Name + "cblind", "Blinds")).SetValue(false);
            ccmenu.AddItem(new MenuItem(Name + "cslow", "Slows")).SetValue(false);
            ccmenu.AddItem(new MenuItem(Name + "cpoison", "Poisons")).SetValue(false);
            menu.AddSubMenu(ccmenu);

            menu.AddItem(new MenuItem("use" + Name + "number", "Min Buffs to Use")).SetValue(new Slider(1, 1, 5))
                .SetTooltip("Will Only " + DisplayName + " if Your Buff Count is >= Value");
            menu.AddItem(new MenuItem("use" + Name + "time", "Min Durration to Use"))
                .SetValue(new Slider(250, 250, 2000))
                .SetTooltip("Will not use unless the buff durration (stun, snare, etc) last at least this long (ms, 500 = 0.5 seconds)");
            menu.AddItem(new MenuItem("use" + Name + "od", "Use for Dangerous Only")).SetValue(false);
            menu.AddItem(new MenuItem("use" + Name + "delay", "Activation Delay (in ms)")).SetValue(new Slider(100, 0, 1000));
            menu.AddItem(new MenuItem("mode" + Name, "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }));
        }

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                        continue;

                    Helpers.CheckCleanse(hero.Player);

                    if (hero.CleanseBuffCount >= Menu.Item("use" + Name + "number").GetValue<Slider>().Value &&
                        hero.CleanseHighestBuffTime >= Menu.Item("use" + Name + "time").GetValue<Slider>().Value)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(Menu.Item("use" + Name + "delay").GetValue<Slider>().Value, delegate
                        {
                            UseSpell(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                            hero.CleanseBuffCount = 0;
                            hero.CleanseHighestBuffTime = 0;
                        });
                    }
                }
            }
        }
    }
}
