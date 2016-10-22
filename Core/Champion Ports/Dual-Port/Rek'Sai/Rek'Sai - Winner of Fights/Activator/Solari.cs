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
 namespace RekSai.Activator
{
    public static class Solari
    {
        private static Obj_AI_Base rekSai = null;
       
        public static bool hikiSolari { get; set; }
        public const string championName = "RekSai";

        public static Obj_AI_Base reksaiSolari
        {
            get
            {
                if (rekSai != null && rekSai.IsValid)
                {
                    return rekSai;
                }
                return null;
            }
        }
        static Solari()
        {
           
            Game.OnUpdate += Game_OnGameUpdate;
            rekSai = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.IsMe && x.CharData.BaseSkinName == championName);
            if (rekSai != null)
            {
                Console.Write(rekSai.CharData.BaseSkinName);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            var useSolari = Program.Config.Item("use.randuin").GetValue<bool>();
            var allyHP = Program.Config.Item("solari.ally.hp").GetValue<Slider>().Value;
            if (useSolari && Items.CanUseItem(3190) && Items.HasItem(3190))
            {
                foreach (var ally in HeroManager.Allies)
                {
                    if (!ally.IsMe && !ally.IsRecalling() && ally.HealthPercent <= allyHP && ObjectManager.Player.Distance(ally.ServerPosition) <= 590f)
                    {
                        Items.UseItem(3190);
                    }
                }
            }
        }

    }
}