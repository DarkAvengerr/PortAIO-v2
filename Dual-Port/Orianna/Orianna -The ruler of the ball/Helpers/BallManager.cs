using System;
using System.Linq;
using LeagueSharp;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace OriannaTheruleroftheBall
{
    class BallManager
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        public enum Ballstatus
        {
            Me,
            Ally,
            Land
        }

        public class Ball
        {
            public static Vector3 Position { get; set; }
            public static AIHeroClient Hero { get; set; }
            public static Ballstatus Status { get; set; }
            public static bool IsMoving { get; set; }
        }

        public static void BallManagerInit()
        {
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnCreate += Obj_AI_Base_OnCreate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.HasBuff("OrianaGhostSelf"))
            {
                Ball.Position = Player.ServerPosition;
                Ball.Status = Ballstatus.Me;
                Ball.Hero = Player;
                Ball.IsMoving = false;
            }

            foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly && !x.IsMe))
            {
                if (hero.HasBuff("OrianaGhost"))
                {
                    Ball.Position = hero.ServerPosition;
                    Ball.Status = Ballstatus.Ally;
                    Ball.Hero = hero;
                    Ball.IsMoving = false;
                    break;
                }
            }
        }

        private static void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (sender.Name == "Orianna_Base_Q_yomu_ring_green.troy")
            {
                Ball.Position = sender.Position;
                Ball.Status = Ballstatus.Land;
                Ball.IsMoving = false;
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                switch (args.SData.Name)
                {
                    case "OrianaIzunaCommand":
                        Ball.IsMoving = true;
                        break;
                    case "OrianaRedactCommand":
                        Ball.IsMoving = true;
                        break;
                }
            }
        }
    }
}
