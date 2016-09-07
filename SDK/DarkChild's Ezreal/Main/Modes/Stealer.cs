using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DarkEzreal.Main.Modes
{
    using System.Linq;

    using DarkEzreal.Common;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;

    internal class Stealer
    {
        private static float LastTry;

        public static void Execute()
        {
            foreach (var hero in GameObjects.EnemyHeroes)
            {
                if (hero == null || (Game.Time - LastTry) < 3)
                {
                    return;
                }

                var qpred = SpellsManager.Q.GetPrediction(hero);
                var wpred = SpellsManager.W.GetPrediction(hero);
                var epred = SpellsManager.E.GetPrediction(hero);
                var rpred = SpellsManager.R.GetPrediction(hero);
                if (hero.IsValidTarget(SpellsManager.Q.Range) && qpred.Hitchance > HitChance.Collision && hero.IsKillable(SpellsManager.Q) && Config.QMenu["Qks"].GetBool("Qks"))
                {
                    SpellsManager.Q.Cast(qpred.CastPosition);
                    LastTry = Game.Time;
                    return;
                }

                if (hero.IsValidTarget(SpellsManager.W.Range) && wpred.Hitchance > HitChance.Collision && hero.IsKillable(SpellsManager.W) && Config.WMenu["Wks"].GetBool("Wks"))
                {
                    SpellsManager.W.Cast(wpred.CastPosition);
                    LastTry = Game.Time;
                    return;
                }

                if (hero.IsValidTarget(SpellsManager.E.Range + 200) && hero.IsKillable(SpellsManager.E) && Config.EMenu["Eks"].GetBool("Eks"))
                {
                    SpellsManager.E.Cast(epred.CastPosition);
                    LastTry = Game.Time;
                    return;
                }

                if (hero.IsValidTarget(SpellsManager.R.Range) && rpred.Hitchance > HitChance.Collision && hero.IsKillable(SpellsManager.R) && Config.RMenu["Rks"].GetBool("Rks"))
                {
                    SpellsManager.R.Cast(rpred.CastPosition);
                    LastTry = Game.Time;
                    return;
                }
            }

            foreach (var mob in GameObjects.Jungle.Where(m => Config.JungleMobs.Contains(m.CharData.BaseSkinName) && Config.MiscMenu["steal"].GetBool(m.CharData.BaseSkinName)))
            {
                var qpred = SpellsManager.Q.GetPrediction(mob);
                var rpred = SpellsManager.R.GetPrediction(mob);
                if (mob.IsValidTarget(SpellsManager.Q.Range) && qpred.Hitchance > HitChance.Collision && mob.IsKillable(SpellsManager.Q) && Config.QMenu["Qks"].GetBool("Qjs"))
                {
                    SpellsManager.Q.Cast(qpred.CastPosition);
                    LastTry = Game.Time;
                    return;
                }

                if (mob.IsValidTarget(Config.RMenu["Rks"].GetSlider("range") == 0 ? int.MaxValue : Config.RMenu["Rks"].GetSlider("range")) && rpred.Hitchance > HitChance.Collision
                    && mob.IsKillable(SpellsManager.R) && Config.RMenu["Rks"].GetBool("Rjs"))
                {
                    SpellsManager.R.Cast(rpred.CastPosition);
                    LastTry = Game.Time;
                    return;
                }
            }
        }
    }
}
