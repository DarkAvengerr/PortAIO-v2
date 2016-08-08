using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Summoners
{
    internal class boost : CoreSum
    {
        internal override string Name => "summonerboost";
        internal override string DisplayName => "Cleanse";
        internal override string[] ExtraNames => new[] { "" };
        internal override float Range => float.MaxValue;
        internal override int Duration => 3000;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            foreach (var hero in Activator.Allies())
            {
                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                        return;

                    if (hero.Player.LSDistance(Player.ServerPosition) > Range)
                        return;

                    Helpers.CheckCleanse(hero.Player);

                    if (hero.CleanseBuffCount >= Menu.Item("use" + Name + "number").GetValue<Slider>().Value &&
                        hero.CleanseHighestBuffTime >= Menu.Item("use" + Name + "time").GetValue<Slider>().Value)
                    {
                        //if (!Menu.Item("use" + Name + "od").GetValue<bool>())
                        //{
                            LeagueSharp.Common.Utility.DelayAction.Add(
                                Game.Ping + Menu.Item("use" + Name + "delay").GetValue<Slider>().Value, delegate
                                {
                                    UseSpell(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
                                    hero.CleanseBuffCount = 0;
                                    hero.CleanseHighestBuffTime = 0;
                                });
                        //}
                    }
                }
            }
        }
    }
}
