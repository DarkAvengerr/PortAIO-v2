namespace KoreanCommon
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using EloBuddy;

    public class KoreanPrediction
    {
        private readonly List<PredictionItem> predictionItems;

        public KoreanPrediction(Spell spell, KoreanPredictionTypes type = KoreanPredictionTypes.Slow)
        {
            predictionItems = new List<PredictionItem>();
            foreach (AIHeroClient objAiHero in HeroManager.Enemies)
            {
                predictionItems.Add(new PredictionItem(objAiHero, spell, type));
            }
        }

        public Vector3 GetPrediction(AIHeroClient target)
        {
            return predictionItems.First(x => x.Target == target).GetPrediction();
        }

        public void Cast(AIHeroClient target)
        {
            PredictionItem predictionItem = predictionItems.First(x => x.Target == target);
            Vector3 castPosition = predictionItem.GetPrediction();

            if (predictionItem.PredictionSpell.IsReady() && predictionItem.PredictionSpell.IsInRange(castPosition)
                && !castPosition.IsWall())
            {
                predictionItem.PredictionSpell.Cast(castPosition);
            }
        }
    }
}