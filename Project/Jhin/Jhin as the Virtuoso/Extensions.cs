using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using SebbyLib;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Jhin_As_The_Virtuoso {
	public static class Extensions {
		/// <summary>
		/// 转为对话框用
		/// </summary>
		/// <param name="form"></param>
		/// <returns></returns>
		public static string ToUTF8(this string form) {
			var bytes = Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(form));
			return Encoding.Default.GetString(bytes);
		}
		/// <summary>
		/// 转换为菜单用
		/// </summary>
		/// <param name="form"></param>
		/// <returns></returns>
		public static string ToGBK(this string form) {
			var bytes = Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.Default.GetBytes(form));
			return Encoding.Default.GetString(bytes);
		}

		public static string ToHtml(this string form, Color color, FontStlye fontStlye = FontStlye.Null) {
			string colorhx = "#" + color.ToArgb().ToString("X6");
			return form.ToHtml(colorhx, fontStlye);
		}

		public static string ToHtml(this string form, int size) {
			form = form.ToUTF8();
			form = $"<font size=\"{size}\">{form}</font>";
			return form;
		}

		public static string ToHtml(this string form, string color, FontStlye fontStlye = FontStlye.Null) {
			form = form.ToUTF8();
			form = string.Format("<font color=\"{0}\">{1}</font>", color, form);

			if (fontStlye != FontStlye.Null)
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
		
		public static bool HasWBuff(this AIHeroClient target) {
			return target.IsEnemy && !target.IsDead && target.HasBuff("jhinespotteddebuff");
		}

		public static bool InRCone(this Vector3 pos) {
			var range = Jhin.R.Range;
			var angle = 70f * (float)Math.PI / 180;
			var end2 = Jhin.REndPos.To2D() - Jhin.Player.Position.To2D();
			var edge1 = end2.Rotated(-angle / 2);
			var edge2 = edge1.Rotated(angle);

			var point = pos.To2D() - Jhin.Player.Position.To2D();
			if (point.Distance(new Vector2(), true) < range * range && edge1.CrossProduct(point) > 0 && point.CrossProduct(edge2) > 0)
			{
				return true;
			}
			return false;
		}

		public static bool InRCone(this AIHeroClient enemy) {
			return enemy.Position.InRCone();
		}
		/**
		public static bool InRCone(this AIHeroClient enemy) {
			var range = Jhin.R.Range;
			var angle = 70f * (float)Math.PI / 180;
			var end2 = Jhin.REndPos.To2D() - Jhin.Player.Position.To2D();
			var edge1 = end2.Rotated(-angle / 2);
			var edge2 = edge1.Rotated(angle);

			var point = enemy.Position.To2D() - Jhin.Player.Position.To2D();
			if (point.Distance(new Vector2(), true) < range * range && edge1.CrossProduct(point) > 0 && point.CrossProduct(edge2) > 0)
			{
				return true;
			}
			return false;
		}*/

		public static double GetDmg(this Spell spell, Obj_AI_Base target) {
			double damage = 0;
			switch (spell.Slot)
			{
				case SpellSlot.Q:
					damage = 35 + Jhin.Q.Level * 25 + 0.4 * Jhin.Player.FlatPhysicalDamageMod;
					break;
				case SpellSlot.W:
					damage = 55 + Jhin.W.Level * 35 + 0.7 * Jhin.Player.FlatPhysicalDamageMod;
					break;
				case SpellSlot.E:
					damage = Jhin.E.GetDamage(target);
					break;
				case SpellSlot.R:
					damage = (-25 + 75 * Jhin.R.Level + 0.2 * Jhin.Player.FlatPhysicalDamageMod) * (1 + (100 - target.HealthPercent) * 0.02);
					break;
			}

			damage = Jhin.Player.CalcDamage(target, Damage.DamageType.Physical, damage);
			return damage;
		}

		public static bool CastSpell(this Spell spell,Obj_AI_Base target) {
			SebbyLib.Prediction.SkillshotType CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
			bool aoe2 = false;

			if (spell.Type == SkillshotType.SkillshotCircle)
			{
				CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
				aoe2 = true;
			}

			if (spell.Width > 80 && !spell.Collision)
				aoe2 = true;

			var predInput2 = new SebbyLib.Prediction.PredictionInput
			{
				Aoe = aoe2,
				Collision = spell.Collision,
				Speed = spell.Speed,
				Delay = spell.Delay,
				Range = spell.Range,
				From = HeroManager.Player.ServerPosition,
				Radius = spell.Width,
				Unit = target,
				Type = CoreType2
			};
			var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

			if (spell.Speed != float.MaxValue && OktwCommon.CollisionYasuo(HeroManager.Player.ServerPosition, poutput2.CastPosition))
				return false;

			if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
			{
				return spell.Cast(poutput2.CastPosition);
			}
			else if (predInput2.Aoe && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
			{
				return spell.Cast(poutput2.CastPosition);
			}

			return false;
		}
		
	}
}
