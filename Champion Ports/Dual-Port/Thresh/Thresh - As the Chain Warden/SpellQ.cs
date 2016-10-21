using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ThreshAsurvil
{

	public enum QState {
		ThreshQ,
		threshqleap,
		Cooldown
	}

	public static class SpellQ {

		public static bool CastQ1(AIHeroClient target) {
			var Config = Thresh.Config;
			var Q = Thresh.Q;
			var hitChangceIndex = Config.Item("命中率").GetValue<StringList>().SelectedIndex;

			if (Config.Item("预判模式").GetValue<StringList>().SelectedIndex == 0)
			{
				var hitChangceList = new[] { HitChance.VeryHigh, HitChance.High, HitChance.Medium };
				return Q.CastIfHitchanceEquals(target, hitChangceList[hitChangceIndex]);
			}
			else if (Config.Item("预判模式").GetValue<StringList>().SelectedIndex == 1)
			{
				var hitChangceList = new[] { SebbyLib.Prediction.HitChance.VeryHigh, SebbyLib.Prediction.HitChance.High, SebbyLib.Prediction.HitChance.Medium };
				return Q.CastOKTW(target, hitChangceList[hitChangceIndex]);
			}
			else if(Config.Item("预判模式").GetValue<StringList>().SelectedIndex == 2)
			{
				var hitChangceList = new[] { HitChance.VeryHigh, HitChance.High, HitChance.Medium };
				Q.SPredictionCast(target, hitChangceList[hitChangceIndex]);
			}
			return false;
		}

		public static bool CastOKTW(this Spell spell, AIHeroClient target, SebbyLib.Prediction.HitChance hitChance) {
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

			if (spell.Speed != float.MaxValue && SebbyLib.OktwCommon.CollisionYasuo(HeroManager.Player.ServerPosition, poutput2.CastPosition))
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

		private static bool IsAllyDashWithW() {
			//threesisters
			return HeroManager.Allies.Any(a => a.HasBuff("InitializeShieldMarker") && a.GetBuff("InitializeShieldMarker").Caster.IsMe);
		}

		public static bool CastQ2() {
			if (Thresh.QTarget is AIHeroClient && (Thresh.QTarget.GetPassiveTime("ThreshQ") < 0.3 || IsAllyDashWithW()))
			{
				return Thresh.Q.Cast();
			}
			return false;
		}

		public static QState GetState() {
			if (!Thresh.Q.IsReady())
			{
				return QState.Cooldown;
			}
			else
			{
				if (Thresh.Q.Instance.Name == "ThreshQ")
				{
					return QState.ThreshQ;
				}
				else
				{
					return QState.threshqleap;
				}
			}
		}
	}
}
