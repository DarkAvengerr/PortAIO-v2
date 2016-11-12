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
    public static class Heal
    {
        private static Obj_AI_Base adCarry = null;
        private static Obj_AI_Minion jungleMobs;
        public static bool hikiHeal { get; set; }

        public static Obj_AI_Base adcHeal
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
        static Heal()
        {
            Game.OnUpdate += Game_OnGameUpdate;
            adCarry = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.IsMe);
            jungleMobs = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100) 
                && x.CharData.BaseSkinName.Contains("Dragon")
                || x.CharData.BaseSkinName.Contains("Baron")
                || x.CharData.BaseSkinName.Contains("SRU_Blue")
                || x.CharData.BaseSkinName.Contains("SRU_Red") && x.IsHPBarRendered && !x.IsDead);
            if (adCarry != null)
            {
                Console.Write(adCarry.CharData.BaseSkinName);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            var useHeal = Program.Config.Item("use.heal").GetValue<bool>();
            var healMyhp = Program.Config.Item("heal.myhp").GetValue<Slider>().Value;

            if (useHeal && Program.Heal.IsReady() && ObjectManager.Player.Distance(jungleMobs.Position) >= 300)
            {
                if (ObjectManager.Player.HealthPercent < healMyhp)
                {
                    ObjectManager.Player.Spellbook.CastSpell(Program.Heal);
                }
            }
        }



    }
}