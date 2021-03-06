using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Activator.Spells.Slows
{
    class highlander : CoreSpell
    {
        internal override string Name => "highlander";
        internal override string DisplayName => "Highlander | R";
        internal override MenuType[] Category => new[] { MenuType.SlowRemoval, MenuType.ActiveCheck };
        internal override int Priority => 2;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            if (Player.HasBuffOfType(BuffType.Slow) && Menu.Item("use" + Name + "sr").GetValue<bool>())
            {
                if (!Parent.Item(Parent.Name + "useon" + Player.NetworkId).GetValue<bool>())
                    return;

                UseSpell(Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex == 1);
            }
        }
    }
}
