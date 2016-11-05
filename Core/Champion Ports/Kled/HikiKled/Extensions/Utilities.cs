using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace Hiki.Kled.Extensions
{
    public static class Utilities
    {
        public static string[] HighChamps =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
                "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Leblanc",
                "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon",
                "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath",
                "Zed", "Ziggs","Kindred","Jhin"
            };

        public static string[] HitchanceNameArray = { "Low", "Medium", "High", "Very High", "Only Immobile" };
        public static HitChance[] HitchanceArray = { HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh, HitChance.Immobile };

        public static HitChance HikiChance(string menuName)
        {
            return HitchanceArray[Menus.Config.Item(menuName).GetValue<StringList>().SelectedIndex];
        }

        public static bool Enabled(string menuName)
        {
            return Menus.Config.Item(menuName).GetValue<bool>();
        }

        public static int Slider(string menuName)
        {
            return Menus.Config.Item(menuName).GetValue<Slider>().Value;
        }

        public static bool IsSkaarl()
        {
            return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name != "KledRiderQ"; ;
        }

        public static bool IsKled()
        {
            return ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name == "KledRiderQ";
        }

        public static void ECast(AIHeroClient enemy)
        {
            var range = Orbwalking.GetRealAutoAttackRange(enemy);
            var path = Geometry.CircleCircleIntersection(ObjectManager.Player.ServerPosition.To2D(),
                Prediction.GetPrediction(enemy, 0.25f).UnitPosition.To2D(), Spells.E.Range, range);

            if (path.Count() > 0)
            {
                var epos = path.MinOrDefault(x => x.Distance(Game.CursorPos));
                if (epos.To3D().UnderTurret(true) || epos.To3D().IsWall())
                {
                    return;
                }

                if (epos.To3D().CountEnemiesInRange(Spells.E.Range - 100) > 0)
                {
                    return;
                }
                Spells.E.Cast(epos);
            }
            if (path.Count() == 0)
            {
                var epos = ObjectManager.Player.ServerPosition.Extend(enemy.ServerPosition, -Spells.E.Range);
                if (epos.UnderTurret(true) || epos.IsWall())
                {
                    return;
                }

                // no intersection or target to close
                Spells.E.Cast(ObjectManager.Player.ServerPosition.Extend(enemy.ServerPosition, -Spells.E.Range));
            }
        }
    }
}
