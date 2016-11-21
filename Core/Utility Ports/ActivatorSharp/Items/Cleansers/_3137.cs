using System;
using System.Linq;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Activator.Items.Cleansers
{
    class _3137 : CoreItem
    {
        internal override int Id => 3137;
        internal override string Name => "Dervish";
        internal override string DisplayName => "Dervish Blade";
        internal override int Priority => 6;
        internal override int Duration => 250;
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.Cleanse, MenuType.ActiveCheck };
        internal override MapType[] Maps => new[] { MapType.CrystalScar, MapType.TwistedTreeline };
        internal override int DefaultHP => 5;
        internal override int DefaultMP => 0;

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

                    Helpers.CheckDervish(hero.Player);

                    if (hero.DervishBuffCount >= Menu.Item("use" + Name + "number").GetValue<Slider>().Value &&
                        hero.DervishHighestBuffTime >= Menu.Item("use" + Name + "time").GetValue<Slider>().Value)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(Menu.Item("use" + Name + "delay").GetValue<Slider>().Value, () =>
                        {
                            UseItem(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                            hero.DervishBuffCount = 0;
                            hero.DervishHighestBuffTime = 0;
                        });                       
                    }
                }
            }
        }
    }
}
