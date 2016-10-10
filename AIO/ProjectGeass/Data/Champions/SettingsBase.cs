using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Data.Champions
{

    internal class SettingsBase
    {
        #region Public Fields

        public const short E=W+1;
        public const short Q=1;
        public const short R=E+1;
        public const short W=Q+1;
        public static string[] ManaAbilities={$"Q", $"W", $"E", $"R"};
        public static string[] ManaModes={"Combo", "Mixed", "Clear"};

        #endregion Public Fields
    }

}