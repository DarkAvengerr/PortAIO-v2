using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace CNLib {
	public static class DamageIndicator {
		public delegate float DamageToUnitDelegate(AIHeroClient hero);

		private const int XOffset = 10;
		private const int YOffset = 20;
		private const int Width = 103;
		private const int Height = 8;

		public static Color Color { get; set; } = Color.Lime;
		public static Color FillColor { get; set; } = Color.Goldenrod;

		public static bool Fill { get; set; } = true;
		public static bool Enabled { get; set; } = true;

		public static bool HealthbarEnabled { get; set; }
		public static bool PercentEnabled { get; set; }

		private static DamageToUnitDelegate _damageToUnit;

		private static readonly Render.Text Text = new Render.Text(0, 0, "", 14, SharpDX.Color.Red, "monospace");

		private static Font font = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "微软雅黑", Height = 50 });

		static DamageIndicator() {
			//font = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "微软雅黑", Height = 25 });
		}

		public static DamageToUnitDelegate DamageToUnit
		{
			get { return _damageToUnit; }

			set
			{
				if (_damageToUnit == null)
				{
					Drawing.OnDraw += Drawing_OnDraw;
				}
				_damageToUnit = value;
			}
		}

		

		private static void Drawing_OnDraw(EventArgs args) {
			if (!Enabled || _damageToUnit == null)
			{
				return;
			}

			foreach (var unit in HeroManager.Enemies.Where(h => h.IsValid && h.IsHPBarRendered))
			{
				var barPos = unit.HPBarPosition;
				var damage = _damageToUnit(unit);

				if (damage <= 0)
				{
					continue;
				}

				#region 血条上显示
				if (HealthbarEnabled)
				{
					var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
					var yPos = barPos.Y + YOffset;
					var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
					var xPosCurrentHp = barPos.X + XOffset + Width * unit.Health / unit.MaxHealth;

					if (damage > unit.Health)
					{
						Text.X = (int)barPos.X + XOffset;
						Text.Y = (int)barPos.Y + YOffset - 13;
						Text.OnEndScene();
					}

					Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, Color);

					if (Fill)
					{
						float differenceInHp = xPosCurrentHp - xPosDamage;
						var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);

						for (int i = 0; i < differenceInHp; i++)
						{
							Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, FillColor);
						}
					}
				}
				#endregion

				#region 百分比显示
				if (PercentEnabled)
				{
					
					// Get damage in percent and draw next to the health bar
					var pos = unit.Position.To2D();
					pos = Drawing.WorldToScreen(unit.Position);
					var text = string.Concat(Math.Ceiling((damage / unit.TotalShieldHealth()) * 100), "%");

					font.DrawTextCentered(text, pos, FillColor);
				}
				#endregion
			}
		}

		public static void drawText(string msg, Vector3 Hero,Color color, int weight = 0) {
			var wts = Drawing.WorldToScreen(Hero);
			Drawing.DrawText(wts[0] - (msg.Length) * 5, wts[1] + weight, color, msg);
		}
	}
}
