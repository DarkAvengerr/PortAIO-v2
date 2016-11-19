using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.Logic
{
    using System.Collections.Generic;

    using SharpDX;

    internal sealed class FleeLogic
    {
        // Draws position where maxrange jumps are possible (drag, baron, wolves)
        public readonly Dictionary<string, Vector3> JumpPos = new Dictionary<string, Vector3> {
                                                                      {
                                                                          "mid_Dragon",
                                                                          new Vector3(
                                                                          9122f,
                                                                          4058f,
                                                                          53.95995f)
                                                                      },
                                                                      {
                                                                          "left_dragon",
                                                                          new Vector3(
                                                                          9088f,
                                                                          4544f,
                                                                          52.24316f)
                                                                      },
                                                                      {
                                                                          "baron",
                                                                          new Vector3(
                                                                          5774f,
                                                                          10706f,
                                                                          55.77578F)
                                                                      },
                                                                      //not pre 20
                                                                      {
                                                                          "red_wolves",
                                                                          new Vector3(
                                                                          11772f,
                                                                          8856f,
                                                                          50.30728f)
                                                                      },
                                                                      {
                                                                          "blue_wolves",
                                                                          new Vector3(
                                                                          3046f,
                                                                          6132f,
                                                                          57.04655f)
                                                                      }
                                                                  };

        public readonly List<Vector3> JunglePos = new List<Vector3> {
                                                          new Vector3(6271.479f, 12181.25f, 56.47668f),
                                                          new Vector3(6971.269f, 10839.12f, 55.2f),
                                                          new Vector3(8006.336f, 9517.511f, 52.31763f),
                                                          new Vector3(10995.34f, 8408.401f, 61.61731f),
                                                          new Vector3(10895.08f, 7045.215f, 51.72278f),
                                                          new Vector3(12665.45f, 6466.962f, 51.70544f),


                                                          new Vector3(5048f, 10460f, -71.2406f),
                                                          new Vector3(39000.529f, 7901.832f, 51.84973f),
                                                          new Vector3(2106.111f, 8388.643f, 51.77686f),
                                                          new Vector3(3753.737f, 6454.71f, 52.46301f),
                                                          new Vector3(6776.247f, 5542.872f, 55.27625f),
                                                          new Vector3(7811.688f, 4152.602f, 53.79456f),
                                                          new Vector3(8528.921f, 2822.875f, 50.92188f),


                                                          new Vector3(9802f, 4366f, -71.2406f),
                                                          new Vector3(3926f, 7918f, 51.74162f)
                                                      };
    }
}
