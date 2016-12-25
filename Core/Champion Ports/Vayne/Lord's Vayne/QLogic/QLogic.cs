using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Lord_s_Vayne.QLogic
{
    class QLogic
    {
        public static
         Vector2 SideQ(Vector2 point1, Vector2 point2, double angle)
        {
            angle *= Math.PI / 180.0;
            var temp = Vector2.Subtract(point2, point1);
            var result = new Vector2(0);
            var x = (float)(temp.X * Math.Cos(angle) - temp.Y * Math.Sin(angle)) / 4;
            var y = (float)(temp.X * Math.Sin(angle) + temp.Y * Math.Cos(angle)) / 4;
            var sol = Vector2.Add(result, point1);
            result.X = x;
            result.Y = y;
            result = sol;
            return result;
        }
      
        public static
            Vector2 SafeQ(Vector2 point1, Vector2 point2, double angle)
        {
            angle *= Math.PI / -50.0;
            var temp = Vector2.Subtract(point2, point1);
            var result = new Vector2(0);
            var x = (float)(temp.X * Math.Cos(angle) - temp.Y * Math.Sin(angle)) / 4;
            var y = (float)(temp.X * Math.Sin(angle) + temp.Y * Math.Cos(angle)) / 4;
            var sol = Vector2.Add(result, point1);
            result.X = x;
            result.Y = y;
            result = sol;
            return result;
        }

        public static
            Vector2 AggresiveQ(Vector2 point1, Vector2 point2, double angle)    
        {
            angle *= Math.PI / 300;
            var temp = Vector2.Subtract(point2, point1);
            var result = new Vector2(0);
            var x = (float)(temp.X * Math.Cos(angle) - temp.Y * Math.Sin(angle)) / 4;
            var y = (float)(temp.X * Math.Sin(angle) + temp.Y * Math.Cos(angle)) / 4;
            var sol = Vector2.Add(result, point1);
            result.X = x;
            result.Y = y;
            result = sol;
            return result;
        }

    }
}

