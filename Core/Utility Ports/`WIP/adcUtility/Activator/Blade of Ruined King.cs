using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace adcUtility.Activator
{
    public static class Blade_of_Ruined_King
    {
        private static Obj_AI_Base adCarry = null;
        public static bool hikiBOTRK { get; set; }

        public static Obj_AI_Base adcBOTRK
        {
            get
            {
                if (adCarry != null && adCarry.IsValid)
                {
                    return adCarry;
                }
                return null;
            }
        }
        static Blade_of_Ruined_King()
        {
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            adCarry = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.IsMe);
            if (adCarry != null)
            {
                Console.Write(adCarry.CharData.BaseSkinName);
            }
        }

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {

            var tar = (AIHeroClient)target;
            var useBtork = Program.Config.Item("useBOTRK").GetValue<bool>();
            var theirhp = Program.Config.Item("theirhp").GetValue<Slider>().Value;
            var myhp = Program.Config.Item("myhp").GetValue<Slider>().Value;

            if (useBtork && ((tar.Health / tar.MaxHealth) < theirhp) && ((ObjectManager.Player.Health / ObjectManager.Player.MaxHealth) < myhp))
            {
                if (Items.CanUseItem(3144))
                {
                    Items.UseItem(3144, tar);
                }
            }
        }

    }
}