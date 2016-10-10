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
    class Botrk
    {
        internal static ItemData.Item Botrkk => ItemData.Blade_of_the_Ruined_King;

        public Botrk()
        {
            Console.WriteLine("HikiCarry: Botrk Initalized");
            Orbwalking.AfterAttack += BotrkAfterAttack;
        }

        private void BotrkAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (target != null && Initializer.Activator.Item("botrk").GetValue<bool>() && (target.Health / target.MaxHealth < Initializer.Activator.Item("botrk.enemy.hp").GetValue<Slider>().Value) &&
                (ObjectManager.Player.Health/ObjectManager.Player.MaxHealth < Initializer.Activator.Item("botrk.adc.hp").GetValue<Slider>().Value)
                && Botrkk.GetItem().IsReady() && Botrkk.GetItem().IsOwned() && Botrkk.GetItem().IsInRange((AIHeroClient)target))
            {
                Botrkk.GetItem().Cast((AIHeroClient)target);
            }
        }
    }
}
