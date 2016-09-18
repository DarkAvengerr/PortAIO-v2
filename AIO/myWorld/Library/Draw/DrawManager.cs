using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using myWorld.Library.MenuWarpper;

using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;

using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld.Library.Draw
{
    class DrawManager
    {
        static List<_Circle> Circles = new List<_Circle>();
        public DrawManager()
        {
            //_Circle c = new _Circle();
        }

        public void AddCircle(Obj_AI_Base pos, float range, Color color, string paramTxt, Func<bool> condition)
        {
            Circles.Add(new _Circle(pos, range, color, paramTxt, condition));
        }

        public void AddToMenu(Menu menu)
        {
            Menu CircleMenu = new Menu("Draw Manager", "Drwa Manager");
            foreach(_Circle cc in Circles)
            {
                cc.AddToMenu(CircleMenu);
            }

            menu.AddSubMenu(CircleMenu);

            Drawing.OnDraw += Drawing_OnDraw;
        }

        void Drawing_OnDraw(EventArgs args)
        {
            //throw new NotImplementedException();
            foreach(_Circle cc in Circles)
            {
                cc.Draw();
            }
        }
    }

    class _Circle
    {
        public bool Enable = true;
        public Func<bool> condition;

        Menu CircleMenu;
        Color DrawColor;
        float Range;

        string ParamText;

        Obj_AI_Base Position;

        public _Circle(Obj_AI_Base position, float range, Color color, string paramTxt, Func<bool> condition)
        {
            this.Position = position;
            this.Range = range;
            this.DrawColor = color;
            this.ParamText = paramTxt;
            this.condition = condition;
        }

        public void AddToMenu(Menu menu)
        {
            CircleMenu = new Menu(ParamText, ParamText);
            CircleMenu.AddBool("Circle.Enable", "Enable");
            menu.AddSubMenu(CircleMenu);
        }

        public void Draw()
        {
            if(condition == null || !condition() || !CircleMenu.GetBool("Circle.Enable")) return;

            Drawing.DrawCircle(this.Position.Position, Range, DrawColor);
        }
    }
}
