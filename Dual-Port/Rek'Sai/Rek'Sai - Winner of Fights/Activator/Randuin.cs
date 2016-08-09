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
    public static class Randuin
    {
        private static Obj_AI_Base rekSai = null;
        
        public static bool hikiRanduin { get; set; }
        public const string championName = "RekSai";

        public static Obj_AI_Base reksaiRanduin
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
        static Randuin()
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
            var useRanduin = Program.Config.Item("use.randuin").GetValue<bool>();
            var randuinCount = Program.Config.Item("randuin.count").GetValue<Slider>().Value;
            if (useRanduin && Items.HasItem(3143) && Items.CanUseItem(3143) && ObjectManager.Player.CountEnemiesInRange(490f) >= randuinCount)
            {
                Items.UseItem(3143);
            }
        }
        
    }
}