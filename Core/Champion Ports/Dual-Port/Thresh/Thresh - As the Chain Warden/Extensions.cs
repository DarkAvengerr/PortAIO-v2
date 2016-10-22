using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ThreshAsTheChainWarden
{
	public static class Extensions {

		//转换为对话框用
		public static string ToUTF8(this string form) {
			var bytes = Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(form));
			return Encoding.Default.GetString(bytes);
		}
		//转换为菜单用
		public static string ToGBK(this string form) {
			var bytes = Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.Default.GetBytes(form));
			return Encoding.Default.GetString(bytes);
		}


		public static string ToHtml(this string form, Color color, FontStlye fontStlye = FontStlye.Null) {
			string colorhx = "#" + color.ToArgb().ToString("X6");
			return form.ToHtml(colorhx, fontStlye);
		}

		public static string ToHtml(this string form,string color, FontStlye fontStlye = FontStlye.Null) {
			form = form.ToUTF8();
			form = string.Format("<font color=\"{0}\">{1}</font>", color,form);

			if (fontStlye!=FontStlye.Null)
			{
				switch (fontStlye)
				{
					case FontStlye.Bold:
						form = string.Format("<b>{0}</b>", form);
						break;
					case FontStlye.Cite:
						form = string.Format("<i>{0}</i>", form);
						break;
				}
			}
			return form;
		}

		public static float GetPassiveTime(this Obj_AI_Base target, String buffName) {
			return
				target.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
					.Where(buff => buff.Name == buffName)
					.Select(buff => buff.EndTime)
					.FirstOrDefault() - Game.Time;
		}

		public static Obj_AI_Turret GetMostCloseTower(this Obj_AI_Base target) {
			Obj_AI_Turret tur = null;

			if (target.IsDead) return null;

			foreach (var turret in ObjectManager.Get<Obj_AI_Turret>().Where(t =>
				t.IsValid && !t.IsDead && t.Health > 1f && t.IsVisible && t.Distance(target) < 1000))
			{
				if (turret != null)
				{
					if (tur == null)
					{
						tur = turret;
					}
					else if (tur != null && tur.Distance(target) > turret.Distance(target))
					{

						tur = turret;
					}
				}
			}
			return tur;
		}



		public static bool IsInTurret(this Obj_AI_Base targetHero, Obj_AI_Turret targetTurret = null) {
			

			if (targetTurret == null)
			{
				targetTurret = targetHero.GetMostCloseTower();
			}
			if (targetTurret != null && targetHero.Distance(targetTurret) < 850)
			{
				return true;
			}
			return false;
		}

		public static bool CastToReverse(this Spell spell, Obj_AI_Base target) {
			var eCastPosition = spell.GetPrediction(target).CastPosition;
			var position = Thresh.Player.ServerPosition + Thresh.Player.ServerPosition - eCastPosition;
			return spell.Cast(position);
		}

		public static bool IsFleeing(this AIHeroClient hero, Obj_AI_Base target) {
			if (hero == null || target == null)
			{
				return false;
			}

			if (hero.Path.Count()>0 && target.Distance(hero.Position) < target.Distance(hero.Path.Last()))
			{
				return true;
			}
			return false;
		}

		public static bool IsHunting(this AIHeroClient hero, Obj_AI_Base target) {
			if (target == null)
			{
				return false;
			}
			if (target.Path.Count() > 0 && hero.Distance(target.Position) > hero.Distance(target.Path.Last()))
			{
				return true;
			}
			return false;
		}

		public static int CountEnemiesInRangeDeley(this AIHeroClient hero, float range, float delay) {
			int count = 0;
			foreach (var t in HeroManager.Enemies.Where(t => t.IsValidTarget()))
			{
				Vector3 prepos = Prediction.GetPrediction(t, delay).CastPosition;
				if (hero.Distance(prepos) < range)
					count++;
			}
			return count;
		}

	}
}
