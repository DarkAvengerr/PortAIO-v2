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
namespace LeeSin_EloClimber
{
    internal class Smite
    {
        public static List<Obj_AI_Base> jungleMinion;
        public static Dictionary<string, bool> smiteList;
        public static Obj_AI_Base smite_target;

        internal static void Load()
        {
            if (LeeSin.SmiteSpell == null)
                return;

            // Init Variable
            smiteList = new Dictionary<string,bool>();

            // Callback
            Game.OnUpdate += Update;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            if(MenuManager.myMenu.Item("smite.State").GetValue<Boolean>() && smite_target.IsValidTarget(500, false, LeeSin.myHero.Position))
            {
                Vector2 textPosition = smite_target.HPBarPosition;
                if (MenuManager.myMenu.Item("smite.use").GetValue<KeyBind>().Active)
                    Drawing.DrawText(textPosition.X + 40, textPosition.Y - 15, Color.Green, "Auto Smite On");
                else
                    Drawing.DrawText(textPosition.X + 40, textPosition.Y - 15, Color.Red, "Auto Smite Off");
            }
        }

        private static void Update(EventArgs args)
        {
            if (!LeeSin.SmiteSpell.IsReady())
                return;

            jungleMinion = MinionManager.GetMinions(500, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (jungleMinion.Count() != 0 && jungleMinion[0].IsValidTarget(500, true, LeeSin.myHero.Position))
            {
                var target = jungleMinion[0];
                string Key = target.Name;

                if (target.Name.Contains("Dragon"))
                    Key = "Dragon";

                smiteList.Clear();
                smiteList.Add("SRU_RiftHerald17.1.1", MenuManager.myMenu.Item("smite.Herald").GetValue<Boolean>());
                smiteList.Add("SRU_Red4.1.1", MenuManager.myMenu.Item("smite.Red").GetValue<Boolean>());
                smiteList.Add("SRU_Red10.1.1", MenuManager.myMenu.Item("smite.Red").GetValue<Boolean>());
                smiteList.Add("SRU_Blue1.1.1", MenuManager.myMenu.Item("smite.Blue").GetValue<Boolean>());
                smiteList.Add("SRU_Blue7.1.1", MenuManager.myMenu.Item("smite.Blue").GetValue<Boolean>());
                smiteList.Add("Dragon", MenuManager.myMenu.Item("smite.Drake").GetValue<Boolean>());
                smiteList.Add("SRU_Baron12.1.1", MenuManager.myMenu.Item("smite.Nashor").GetValue<Boolean>());

                try
                {
                    if (smiteList[Key])
                    {
                        smite_target = target;
                        Cast_Smite(target);
                    }
                }
                catch { };
            }
        }

        private static int smiteDmg()
        {
            List<int> Basic = new List<int> { 390, 410, 430, 450, 480, 510, 540, 570, 600, 640, 680, 720, 760, 800, 850, 900, 950, 1000 };
            return Basic[LeeSin.myHero.Level - 1];
        }
        
        private static void Cast_Smite(Obj_AI_Base target)
        {
            if (!MenuManager.myMenu.Item("smite.use").GetValue<KeyBind>().Active)
                return;

            int smite_dmg = smiteDmg();
            double q1_dmg = 0;
            double q2_dmg = 0;
            double q_dmg = 0;

            if (LeeSin.Q.IsReady())
            {
                q1_dmg = LeeSin.GetDamage_Q1(target);
                q2_dmg = LeeSin.GetDamage_Q2(target, (q1_dmg + smite_dmg));
                q_dmg = q1_dmg + q2_dmg;
            }


            if (MenuManager.myMenu.Item("smite.useQ").GetValue<Boolean>() && LeeSin.Q.IsReady() && !LeeSin.IsSecondCast(LeeSin.Q))
            {
                if (q_dmg + smite_dmg > target.Health)
                {
                    LeeSin.Q.Cast(target.Position);
                }
            }
            else if (MenuManager.myMenu.Item("smite.useQ").GetValue<Boolean>() && LeeSin.Q.IsReady() && LeeSin.IsSecondCast(LeeSin.Q))
            {
                if (q2_dmg + smite_dmg > target.Health)
                {
                    LeeSin.SmiteSpell.Cast(target);
                    LeeSin.Q.Cast();
                }
            }
            else if (smite_dmg > target.Health)
            {
                LeeSin.SmiteSpell.Cast(target);
            }
        }
    }
}
