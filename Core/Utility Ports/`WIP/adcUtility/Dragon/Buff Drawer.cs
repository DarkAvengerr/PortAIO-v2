using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace adcUtility.Dragon
{
    public static class Buff_Drawer
    {
        private static Obj_AI_Base adCarry = null;
        public static bool hikiBuffDrawer { get; set; }

        public static Obj_AI_Base buffDraw
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
        static Buff_Drawer()
        {

            Drawing.OnDraw += Drawing_OnDraw;
            adCarry = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.IsMe);
            if (adCarry != null)
            {
                Console.Write(adCarry.CharData.BaseSkinName);
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            var enemydragonCount = Program.Config.Item("enemy.dragon.buff.count").GetValue<Circle>();
            var enemydragonColor = Program.Config.Item("enemy.dragon.buff.count").GetValue<Circle>().Color;

            var allydragonCount = Program.Config.Item("ally.dragon.buff.count").GetValue<Circle>();
            var allydragonColor = Program.Config.Item("ally.dragon.buff.count").GetValue<Circle>().Color;  

            if (enemydragonCount.Active)
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy && x.HasBuff("s5test_dragonslayerbuff")))
                {
                    float enemyStackCount = enemy.GetBuffCount("s5test_dragonslayerbuff");
                    Drawing.DrawText(1180, 70, enemydragonColor, "Enemy Dragon Count: " + enemyStackCount); 
                }
            }
            if (allydragonCount.Active)
            {
                foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(x => x.IsMe && x.HasBuff("s5test_dragonslayerbuff")))
                {
                    float allyStackCount = ally.GetBuffCount("s5test_dragonslayerbuff");
                    Drawing.DrawText(1180, 85, allydragonColor, "Ally Dragon Count: " + allyStackCount);
                }
            }
        }

    }
}