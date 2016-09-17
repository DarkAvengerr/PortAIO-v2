using EloBuddy;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNLib {
	public static class GameExtensions {
		public static float TotalShieldHealth(this Obj_AI_Base target) {
			return target.Health + target.AllShield + target.AttackShield + target.MagicShield;
		}

		public static bool HasUndyingBuff(this AIHeroClient target) {
			// Various buffs
			if (target.Buffs.Any(
				b => b.IsValid &&
					 (b.DisplayName == "Chrono Shift" /* Zilean R */||
					  b.DisplayName == "JudicatorIntervention" /* Kayle R */||
					  b.DisplayName == "Undying Rage" /* Tryndamere R */)))
			{
				return true;
			}

			return target.IsInvulnerable;
		}

		public static bool IsInRange(this Obj_AI_Base pos, Obj_AI_Base targetpos, float range) {
			return pos.Distance(targetpos) < range;
		}
		public static bool IsInRange(this Vector2 pos, Obj_AI_Base targetpos, float range) {
			return pos.Distance(targetpos) < range;
		}
		public static bool IsInRange(this Vector2 pos, Vector2 targetpos, float range) {
			return pos.Distance(targetpos) < range;
		}

		public static bool HasSpellShield(this AIHeroClient target) {
			// Various spellshields
			return target.HasBuffOfType(BuffType.SpellShield) || target.HasBuffOfType(BuffType.SpellImmunity);
		}

		public static bool IsValidTarget(this AIHeroClient target) {
			if (!target.IsDead && target.IsValid && !target.IsZombie)
			{
				return true;
			}
			return false;
		}
	}
}
