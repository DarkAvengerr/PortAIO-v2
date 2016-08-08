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
    public static class Hydra
    {
        private static Obj_AI_Base rekSai = null;
        
        public static bool hikiHydra { get; set; }
        public const string championName = "RekSai";

        public static Obj_AI_Base reksaiHydra
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
        static Hydra()
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
            if (Program.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                thCombo();
            }
        }
        private static void thCombo()
        {
            var useTiamat = Program.Config.Item("use.tiamat").GetValue<bool>();
            var useHydra = Program.Config.Item("use.hydra").GetValue<bool>();
            
            if (useTiamat && Items.HasItem(3077) && Items.CanUseItem(3077))
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && !x.IsZombie && x.LSIsValidTarget(250f)))
                {
                    Items.UseItem(3077);
                }
            }
            if (useHydra && Items.HasItem(3074) && Items.CanUseItem(3074))
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && !x.IsZombie && x.LSIsValidTarget(250f)))
                {
                    Items.UseItem(3074);
                }
            }
        }
    }
}