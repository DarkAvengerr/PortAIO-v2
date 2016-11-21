using System;
using System.Linq;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Activator.Spells.Shields
{
    class shenw : CoreSpell
    {
        internal override string Name => "shenw";
        internal override string DisplayName => "Spirit Refuge | W";
        internal override float Range => 300;
        internal override MenuType[] Category => new[] { MenuType.SelfLowHP, MenuType.SelfMinMP  };
        internal override int DefaultHP => 95;
        internal override int DefaultMP => 45;
        internal override int Priority => 3;

        public override void OnTick(EventArgs args)
        {
            if (!Menu.Item("use" + Name).GetValue<bool>() || !IsReady())
                return;

            if (Player.Mana/Player.MaxMana * 100 <
                Menu.Item("selfminmp" + Name + "pct").GetValue<Slider>().Value)
                return;

            foreach (var hero in Activator.Allies())
            {
                if (!Parent.Item(Parent.Name + "useon" + hero.Player.NetworkId).GetValue<bool>())
                    continue;

                if (hero.Player.NetworkId == Player.NetworkId)
                {
                    if (hero.Player.Health / hero.Player.MaxHealth * 100 <=
                        Menu.Item("selflowhp" + Name + "pct").GetValue<Slider>().Value)
                    {
                        if (hero.IncomeDamage > 0 || hero.MinionDamage > hero.Player.Health)
                        {
                            var shenWObj =
                                ObjectManager.Get<Obj_AI_Minion>()
                                    .FirstOrDefault(x => x.Name.ToLower() == "shenspiritunit");

                            if (shenWObj != null && shenWObj.IsAlly && shenWObj.Distance(Player.ServerPosition) <= Range)
                            {
                                UseSpell();
                            }
                        }
                    }
                }
            }
        }
    }
}
