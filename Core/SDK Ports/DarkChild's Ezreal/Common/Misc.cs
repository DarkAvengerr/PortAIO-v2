using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DarkEzreal.Common
{
    using System;
    using System.Linq;

    using DarkEzreal.Main;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Polygons;
    using LeagueSharp.SDK.UI;

    using SharpDX;

    internal static class Misc
    {
        public static HitChance hitchance(this Spell spell, Menu menu)
        {
            switch (menu[spell.Slot + "hit"].GetListIndex("hit"))
            {
                case 0:
                    {
                        return HitChance.Low;
                    }

                case 1:
                    {
                        return HitChance.Medium;
                    }

                case 2:
                    {
                        return HitChance.High;
                    }

                case 3:
                    {
                        return HitChance.VeryHigh;
                    }
            }

            return HitChance.None;
        }

        public static bool ManaManager(this Spell spell, AMenuComponent menu)
        {
            return menu.GetSliderBool("mana") ? Config.Player.ManaPercent >= menu.GetSliderButton("mana") : true;
        }

        public static bool SafetyManager(this Obj_AI_Base target, AMenuComponent menu)
        {
            return menu.GetSliderBool("danger") ? menu.GetSliderButton("danger") >= target.CountEnemyHeroesInRange(1000) && !target.IsUnderEnemyTurret() : true;
        }

        public static bool SafetyManager(this Vector3 vector3, AMenuComponent menu)
        {
            return menu.GetSliderBool("danger") ? menu.GetSliderButton("danger") >= vector3.CountEnemyHeroesInRange(1000) && !vector3.IsUnderEnemyTurret() : true;
        }

        public static bool WontDie(this Obj_AI_Base target, Spell spell)
        {
            return Health.GetPrediction(target, spell.TravelTime(target)) > 0;
        }

        public static int TravelTime(this Spell spell, Obj_AI_Base target)
        {
            return (int)(((target.DistanceToPlayer() / spell.Speed) * 1000) + (Math.Abs(spell.Delay + Game.Ping) / 2));
        }

        public static bool IsKillable(this Obj_AI_Base target, Spell spell)
        {
            return spell.GetDamage(target) >= Health.GetPrediction(target, spell.TravelTime(target));
        }

        public static void DrawRect(this Spell spell, Vector3 vector3, System.Drawing.Color color)
        {
            var rect = new RectanglePoly(Config.Player.ServerPosition, Config.Player.ServerPosition.Extend(vector3, spell.Range), spell.Width);
            rect.Draw(color, 2);
        }

        public static int AoE(this Spell spell, Vector3 vector3)
        {
            var rect = new RectanglePoly(Config.Player.ServerPosition, Config.Player.ServerPosition.Extend(vector3, spell.Range), spell.Width);
            return GameObjects.AllyHeroes.Count(a => a.IsValidTarget() && rect.IsInside(spell.GetPrediction(a).CastPosition));
        }

        public static bool IsCC(this Obj_AI_Base target)
        {
            return target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Suppression) || target.HasBuffOfType(BuffType.Snare)
                   || target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Fear);
        }

        public static string[] Hooks = { "threshq", "rocketgrab2" };

        public static bool ComboMode
        {
            get
            {
                return Variables.Orbwalker.ActiveMode.HasFlag(OrbwalkingMode.Combo);
            }
        }

        public static bool HybridMode
        {
            get
            {
                return Variables.Orbwalker.ActiveMode.HasFlag(OrbwalkingMode.Hybrid);
            }
        }

        public static bool LastHitMode
        {
            get
            {
                return Variables.Orbwalker.ActiveMode.HasFlag(OrbwalkingMode.LastHit);
            }
        }

        public static bool LaneClearMode
        {
            get
            {
                return Variables.Orbwalker.ActiveMode.HasFlag(OrbwalkingMode.LaneClear);
            }
        }
    }
}
