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
    public static class Bilgewater_Cutlass
    {
        private static Obj_AI_Base adCarry = null;
        public static bool hikiBilgewater { get; set; }

        public static Obj_AI_Base adcBilgewater
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
        static Bilgewater_Cutlass()
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
            var usebilge = Program.Config.Item("useBilge").GetValue<bool>();
            var theirhpbilge = Program.Config.Item("theirhpbilge").GetValue<Slider>().Value;
            var myhpbilge = Program.Config.Item("myhpbilge").GetValue<Slider>().Value;

            if (usebilge && ((tar.Health / tar.MaxHealth) < theirhpbilge) && ((ObjectManager.Player.Health / ObjectManager.Player.MaxHealth) < myhpbilge))
            {
                if (Items.CanUseItem(3153))
                {
                    Items.UseItem(3153, tar);
                }
            }
        }

    }
}