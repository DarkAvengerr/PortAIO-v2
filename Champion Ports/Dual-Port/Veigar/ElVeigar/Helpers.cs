using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElVeigar
{
    using System.Collections.Generic;

    internal class Helpers
    {
        #region Static Fields

        public static readonly List<AddInvulnerableList> IgnoreList = new List<AddInvulnerableList>
                                                                          {
                                                                              new AddInvulnerableList
                                                                                  {
                                                                                      MenuName = "Banshees Veil",
                                                                                      DisplayName = "BansheesVeil",
                                                                                      Name = "bansheesveil"
                                                                                  },
                                                                              new AddInvulnerableList
                                                                                  {
                                                                                      MenuName = "Nocturne Shield",
                                                                                      DisplayName =
                                                                                          "NocturneShroudofDarkness",
                                                                                      Name =
                                                                                          "NocturneShroudofDarknessShield"
                                                                                  },
                                                                              new AddInvulnerableList
                                                                                  {
                                                                                      MenuName = "Morgana Shield",
                                                                                      DisplayName = "Black Shield",
                                                                                      Name = "BlackShield"
                                                                                  },
                                                                              new AddInvulnerableList
                                                                                  {
                                                                                      MenuName = "Sivir Shield",
                                                                                      DisplayName = "SivirE",
                                                                                      Name = "Spell Shield"
                                                                                  },
                                                                              new AddInvulnerableList
                                                                                  {
                                                                                      MenuName = "Fizz E",
                                                                                      DisplayName =
                                                                                          "fizztrickslamsounddummy",
                                                                                      Name = "fizztrickslamsounddummy"
                                                                                  },
                                                                              new AddInvulnerableList
                                                                                  {
                                                                                      MenuName = "Vladimir W",
                                                                                      DisplayName = "VladimirSanguinePool",
                                                                                      Name = "VladimirSanguinePool"
                                                                                  }
                                                                          };

        #endregion

        internal class AddInvulnerableList
        {
            #region Public Properties

            public string DisplayName { set; get; }

            public string MenuName { set; get; }

            public string Name { set; get; }

            #endregion
        }
    }
}