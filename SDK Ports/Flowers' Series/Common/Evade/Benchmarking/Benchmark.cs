using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Common.Evade.Benchmarking
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;
    using SharpDX;

    public static class Benchmark
    {
        private static Vector2 startPoint;
        private static Vector2 endPoint;

        public static void Initialize()
        {
            Game.OnWndProc += Game_OnWndProc;
        }

        static void SpawnLineSkillShot(Vector2 start, Vector2 end)
        {
            SkillshotDetector.TriggerOnDetectSkillshot(DetectionType.ProcessSpell, Common.Evade.SpellDatabase.GetByName("TestLineSkillShot"), Utils.TickCount, start, end, end, GameObjects.Player);

            DelayAction.Add(5000, () => SpawnLineSkillShot(start, end));
        }

        static void SpawnCircleSkillShot(Vector2 start, Vector2 end)
        {
            SkillshotDetector.TriggerOnDetectSkillshot(DetectionType.ProcessSpell, Common.Evade.SpellDatabase.GetByName("TestCircleSkillShot"), Utils.TickCount, start, end, end, GameObjects.Player);

            DelayAction.Add(5000, () => SpawnCircleSkillShot(start, end));
        }


        static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowsMessages.LBUTTONDOWN)
            {
                startPoint = Game.CursorPos.ToVector2();
            }

            if (args.Msg == (uint)WindowsMessages.LBUTTONUP)
            {
                endPoint = Game.CursorPos.ToVector2();
            }

            if (args.Msg == (uint)WindowsMessages.KEYUP && args.WParam == 'L') //line missile skillshot
            {
                SpawnLineSkillShot(startPoint, endPoint);
            }

            if (args.Msg == (uint)WindowsMessages.KEYUP && args.WParam == 'I') //circular skillshoot
            {
                SpawnCircleSkillShot(startPoint, endPoint);
            }
        }
    }
}
