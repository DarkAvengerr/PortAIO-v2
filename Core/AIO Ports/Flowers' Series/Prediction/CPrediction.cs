using LeagueSharp;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Prediction
{
    internal static class CPrediction
    {
        internal struct Position
        {
            public readonly AIHeroClient Hero;
            public readonly Vector3 UnitPosition;

            public Position(AIHeroClient hero, Vector3 unitPosition)
            {
                Hero = hero;
                UnitPosition = unitPosition;
            }
        }
    }
}
