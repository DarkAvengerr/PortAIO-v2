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
 namespace BadaoKingdom.BadaoChampion.BadaoJhin
{
    public static class BadaoJhinPassive
    {
        public static List<GameObject> JhinPassive /*= new List<GameObject>();*/
        {
            get
            {
                return jhinpassive.Where(x => x.IsValid && x.IsVisible).ToList();
            }
        }
        public static List<Obj_AI_Minion> JhinTrap /*= new List<Obj_AI_Minion>();*/
        {
            get
            {
                return  jhintrap.Where(x => x.IsValid && x.IsVisible).ToList();
            }
        }
        public static List<GameObject> jhinpassive = new List<GameObject>();
        public static List<Obj_AI_Minion> jhintrap = new List<Obj_AI_Minion>();
        public static void BadaoActiavte()
        {
            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            //foreach (var obj in JhinPassive)
            //{
            //    Render.Circle.DrawCircle(obj.Position, 100, Color.Green);
            //}
            //foreach (var obj in JhinTrap)
            //{
            //    Render.Circle.DrawCircle(obj.Position, 100, Color.Yellow);
            //}
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            //Chat.Print(sender.Name);
            if ((sender is Obj_AI_Minion) && sender.IsAlly && (sender as Obj_AI_Minion).CharData.BaseSkinName == "jhintrap")
            {
                jhintrap.Add(sender as Obj_AI_Minion);
            }
            else if (sender.Name == "Jhin_Base_E_passive_mark.troy")
            {
                //Chat.Print("all by myself");
                jhinpassive.Add(sender);
            }

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            jhintrap.RemoveAll(x => x == null || !(x as GameObject).IsValid);
            jhinpassive.RemoveAll(x => x == null || !x.IsValid);
        }
    }
}
