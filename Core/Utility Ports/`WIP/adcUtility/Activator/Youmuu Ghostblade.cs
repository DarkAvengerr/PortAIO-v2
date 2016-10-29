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
    public static class Ghostblade
    {
        private static Obj_AI_Base adCarry = null;
        public static bool hikiGhostBlade { get; set; }

        public static Obj_AI_Base adcGhostblade
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
        static Ghostblade()
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

            var usebilge = Program.Config.Item("gBlade").GetValue<bool>();
            if (Items.CanUseItem(3142))
            {
                Items.UseItem(3142);
            }
        }

    }
}