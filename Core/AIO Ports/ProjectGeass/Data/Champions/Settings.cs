using _Project_Geass.Functions;
using _Project_Geass.Module.Champions.Heroes.Events;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Data.Champions
{

    internal class Settings : SettingsBase
    {
        #region Public Constructors

        public Settings()
        {
            switch (StaticObjects.Player.ChampionName)
            {
                case nameof(Tristana):
                    AbilitieSettings=new int[] {E, W, Q, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W};

                    ManaSettings=new[,] {{-1, -1, 35, 25}, {-1, -1, 35, -1}, {-1, -1, 50, -1}};
                    DrawingSettings=new[] {false, false, true, true};
                    // ReSharper disable once UnusedVariable
                    break;

                case nameof(Ezreal):
                    AbilitieSettings=new int[] {Q, E, W, Q, Q, R, Q, E, Q, E, R, E, E, W, W, R, W, W};

                    ManaSettings=new[,] {{20, 30, -1, 35}, {30, 40, -1, 15}, {50, -1, -1, -1}};
                    DrawingSettings=new[] {true, true, false, false};
                    // ReSharper disable once UnusedVariable
                    break;

                case nameof(Ashe):
                    AbilitieSettings=new int[] {W, Q, W, E, W, R, W, Q, W, Q, R, Q, Q, E, E, R, E, E};

                    ManaSettings=new[,] {{25, 30, -1, 30}, {25, 35, -1, 35}, {40, 65, -1, -1}};
                    DrawingSettings=new[] {false, true, false, true};
                    // ReSharper disable once UnusedVariable
                    break;

                case nameof(Kalista):
                    AbilitieSettings=new int[] {E, W, Q, E, E, R, E, Q, E, Q, R, Q, Q, W, W, R, W, W};

                    ManaSettings=new[,] {{40, -1, 25, 15}, {-1, -1, 45, -1}, {-1, -1, 60, -1}};
                    DrawingSettings=new[] {true, false, true, true};
                    // ReSharper disable once UnusedVariable
                    break;
            }
        }

        #endregion Public Constructors

        #region Public Properties

        public int[] AbilitieSettings{get;set;}
        public bool[] DrawingSettings{get;set;}
        public int[,] ManaSettings{get;set;}

        #endregion Public Properties
    }

}