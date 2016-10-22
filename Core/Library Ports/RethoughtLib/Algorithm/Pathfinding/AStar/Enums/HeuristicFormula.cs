using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.Algorithm.Pathfinding.AStar.Enums
{
    public enum HeuristicFormula
    {
        Manhattan = 1,

        MaxDxdy = 2,

        DiagonalShortCut = 3,

        Euclidean = 4,

        EuclideanNoSqr = 5,

        Custom1 = 6
    }
}
