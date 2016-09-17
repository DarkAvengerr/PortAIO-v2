using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNLib {
	public static class DrawHelper {
		/// <summary>
		/// 显示技能范围
		/// </summary>
		/// <param name="spell">技能</param>
		/// <param name="show">线圈信息，开关，颜色等</param>
		public static void DrawRange(this Spell spell,Circle show,bool OnlyReady = true) {

			if ((spell.IsReady() && OnlyReady || !OnlyReady) && show.Active)
			{
				Render.Circle.DrawCircle(HeroManager.Player.Position,spell.Range,show.Color,2);
			}
		}
	}
}
