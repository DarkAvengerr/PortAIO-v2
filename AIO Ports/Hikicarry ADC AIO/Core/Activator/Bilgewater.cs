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
    class Bilgewater
    {
        internal static ItemData.Item Bilgewaterr => ItemData.Bilgewater_Cutlass;

        public Bilgewater()
        {
            Console.WriteLine("HikiCarry: Bilgewater Initalized");
            Orbwalking.AfterAttack += BilgewaterAfterAttack;
        }

        private void BilgewaterAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (target != null && Initializer.Activator.Item("bilgewater").GetValue<bool>() && (target.Health / target.MaxHealth < Initializer.Activator.Item("bilgewater.enemy.hp").GetValue<Slider>().Value) &&
                (ObjectManager.Player.Health / ObjectManager.Player.MaxHealth < Initializer.Activator.Item("bilgewater.adc.hp").GetValue<Slider>().Value)
                && Bilgewaterr.GetItem().IsReady() && Bilgewaterr.GetItem().IsOwned() && Bilgewaterr.GetItem().IsInRange((AIHeroClient)target))
            {
                Bilgewaterr.GetItem().Cast((AIHeroClient)target);
            }
        }
    }
}
