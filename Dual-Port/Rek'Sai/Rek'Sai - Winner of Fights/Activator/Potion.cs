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
    public static class Potion
    {
        private static Obj_AI_Base rekSai = null;
        public static bool hikiPotion { get; set; }
        public const string championName = "RekSai";

        public static Obj_AI_Base reksaiPotion
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
        static Potion()
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
            var useHP = Program.Config.Item("useHealth").GetValue<bool>();
            var myhpforhppot = Program.Config.Item("myhp").GetValue<Slider>().Value;

            var useMana = Program.Config.Item("useMana").GetValue<bool>();
            var mymanaformanapot = Program.Config.Item("mymana").GetValue<Slider>().Value;

            if (useHP && Items.HasItem(2041) || Items.HasItem(2003) || Items.HasItem(2010))
            {
                if (ObjectManager.Player.HealthPercent <= myhpforhppot)
                {
                    if (Items.CanUseItem(2041))
                    {
                        Items.UseItem(2041);
                    }
                    if (Items.CanUseItem(2003))
                    {
                        Items.UseItem(2003);
                    }
                    if (Items.CanUseItem(2010))
                    {
                        Items.UseItem(2010);
                    }
                }
            }
            if (useMana && Items.HasItem(2004))
            {
                if (ObjectManager.Player.ManaPercent <= mymanaformanapot)
                {
                    if (Items.CanUseItem(2004))
                    {
                        Items.UseItem(2004);
                    }
                }
            }

        }



    }
}