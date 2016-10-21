using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry.Core.Activator
{
    internal class Youmuu
    {
        internal static ItemData.Item Youmuus => ItemData.Youmuus_Ghostblade;
        public Youmuu()
        {
            Console.WriteLine("HikiCarry: Youmuu Initalized");
            Orbwalking.AfterAttack += YoumuuAfterAttack;
        }

        private void YoumuuAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Initializer.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
                Initializer.Activator.Item("youmuu").GetValue<bool>() && Youmuus.GetItem().IsOwned() &&
                Youmuus.GetItem().IsReady())
            {
                Youmuus.GetItem().Cast();
            }
        }
    }
}
