using System.Linq;
using hJhin.Extensions;
using LeagueSharp;
using LeagueSharp.SDK;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace hJhin.Modes
{
    static class Ultimate
    {
        public static void Execute()
        {
            if (Config.Menu["ultimate.settings"]["combo.r"])
            {
                if (ObjectManager.Player.IsActive(Spells.R))
                {
                    if (Config.Menu["ultimate.settings"]["auto.shoot.bullets"])
                    {
                        var enemies = GameObjects.EnemyHeroes.Where(
                                                    x =>
                                                        x.IsValidTarget(Spells.R.Range) &&
                                                        Config.Menu["ultimate.settings"]["combo.r." + x.ChampionName]
                                                        && Spells.R.GetPrediction(x).Hitchance >= Provider.HikiChance())
                                                    .MinOrDefault(x => x.Health);

                        var pred = Spells.R.GetPrediction(enemies);
                        if (enemies != null && pred.Hitchance >= Provider.HikiChance())
                        {
                            Spells.R.Cast(pred.CastPosition);
                            return;
                        }
                    }
                }
                else
                {
                    if (Spells.R.IsReady() && Config.SemiManualUlt.Active)
                    {
                        var enemies = GameObjects.EnemyHeroes.Where(
                                                    x =>
                                                        x.IsValidTarget(Spells.R.Range) &&
                                                        Config.Menu["ultimate.settings"]["combo.r." + x.ChampionName]
                                                        && Spells.R.GetPrediction(x).Hitchance >= Provider.HikiChance())
                                                    .MinOrDefault(x => x.Health);

                        var pred = Spells.R.GetPrediction(enemies);
                        if (enemies != null && pred.Hitchance >= Provider.HikiChance())
                        {
                            Spells.R.Cast(pred.CastPosition);
                            return;
                        }
                    }
                }
            }
        }
    }
}
