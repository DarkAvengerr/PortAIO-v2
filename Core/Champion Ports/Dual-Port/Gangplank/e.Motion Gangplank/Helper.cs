using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using SharpDX;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace e.Motion_Gangplank
{
    public static class Helper
    {
        private const int QDELAY = 300;
        public static Vector2 PredPos;
        public static int GetQTime(Vector3 position)
        {
            return (int)(Program.Player.Distance(position) / 2.6f + QDELAY + Game.Ping/2);
        }

        public static bool GetPredPos(AIHeroClient enemy, bool additionalReactionTime = false, bool additionalBarrelTime = false)
        {
            PredPos = SPrediction.Prediction.GetFastUnitPosition(enemy, Config.Menu.Item("misc.enemyReactionTime").GetValue<Slider>().Value);
            float reactionDistance = Config.Menu.Item("misc.enemyReactionTime").GetValue<Slider>().Value  +  (additionalReactionTime? Config.Menu.Item("misc.additionalReactionTime").GetValue<Slider>().Value : 0) * enemy.MoveSpeed*0.001f;
            if (PredPos.Distance(enemy) > reactionDistance)
            {
                PredPos = enemy.Position.Extend(PredPos.To3D(), reactionDistance).To2D();
            }
            return true;
        }
        public static bool CannotEscape(this Vector3 kegPosition, Vector3 distCalcPosition, AIHeroClient enemy, bool additionalReactionTime = false, bool additionalBarrelTime = false)
        {
            GetPredPos(enemy, additionalReactionTime);
            if (enemy.Position.Distance(kegPosition) <= 325 && PredPos.Distance(kegPosition) < 325 - enemy.MoveSpeed*(GetQTime(kegPosition)+(additionalBarrelTime ? 400 : 0) - (additionalReactionTime ? Config.Item("misc.additionalReactionTime").GetValue<Slider>().Value : 0) - Config.Item("misc.enemyReactionTime").GetValue<Slider>().Value) * 0.00095f)
            {
                //Chat.Print("Distance:" + PredPos.Distance(kegPosition));
                //Chat.Print("Max Distance:" + (400 - enemy.MoveSpeed * (GetQTime(kegPosition) + (additionalBarrelTime ? 400 : 0) - (additionalReactionTime ? Config.Item("misc.additionalReactionTime").GetValue<Slider>().Value : 0) - Config.Item("misc.reactionTime").GetValue<Slider>().Value) * 0.00095f));
                return true;
            }
            return false;
        }

        public static Vector3 ExtendToMaxRange(this Vector3 startPosition, Vector3 endPosition, float maxrange)
        {
            return startPosition.Extend(endPosition, Math.Min(startPosition.Distance(endPosition), maxrange));
        }
    }
}
