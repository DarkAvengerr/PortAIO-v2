using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Collision = LeagueSharp.Common.Collision;

using EloBuddy; 
using LeagueSharp.Common; 
namespace LeeSin_EloClimber
{
    internal class resultPred
    {
        public Vector3 predPos;
        public int Hitchance;

        // Default constructor:
        public resultPred()
        {
        }

        // Constructor:
        public resultPred(Vector3 pos, int hit)
        {
            this.predPos = pos;
            this.Hitchance = hit;
        }
    }

    internal class myPred
    {
        private static Dictionary<int, float> newPath = new Dictionary<int, float>();

        internal static void load()
        {
            Obj_AI_Base.OnNewPath += OnNewPath;
            foreach (var unit in HeroManager.Enemies)
            {
                newPath.Add(unit.NetworkId, Environment.TickCount);
            }
        }

        private static int GetNow()
        {
            return ((DateTime.Now.Minute * 60) * 1000) + (DateTime.Now.Second * 1000) + DateTime.Now.Millisecond;
        }

        internal static resultPred GetPrediction(AIHeroClient target, Spell isSpell)
        {
            resultPred result = new resultPred(new Vector3(), 0);

            if (target != null & target.IsValidTarget(LeeSin.Q.Range + LeeSin.W.Range))
            {

                Vector3[] path = target.Path;
                if (path.Count() == 1)
                {
                    if (path[0].Distance(LeeSin.myHero.ServerPosition) < isSpell.Range)
                        result.Hitchance = 8;

                    result.predPos = path[0];
                }
                else if (path.Count() >= 2)
                {
                    if (GetNow() - newPath[target.NetworkId] < 100 || GetNow() > 3000)
                    {
                        float timeToHit = (LeeSin.myHero.ServerPosition.Distance(target.ServerPosition) / isSpell.Speed) + (isSpell.Delay / 1000);
                        float DistanceRun = target.MoveSpeed * timeToHit;

                        Vector3 pos = path[1];
                        pos = target.ServerPosition.Extend(path[1], (DistanceRun - (isSpell.Width / 2) - target.BoundingRadius));

                        if (pos.Distance(LeeSin.myHero.ServerPosition) < isSpell.Range)
                        {
                            if (DistanceRun > 500)
                                result.Hitchance = 3;
                            else if (DistanceRun > 400)
                                result.Hitchance = 4;
                            else if (DistanceRun > 300)
                                result.Hitchance = 5;
                            else if (DistanceRun > 200)
                                result.Hitchance = 6;
                            else if (DistanceRun < 200)
                                result.Hitchance = 7;

                            result.predPos = pos;
                        }
                    }

                }

                PredictionInput predInput = new PredictionInput { From = LeeSin.myHero.ServerPosition, Radius = isSpell.Width, Range = isSpell.Range };
                predInput.CollisionObjects[0] = CollisionableObjects.Heroes;
                predInput.CollisionObjects[1] = CollisionableObjects.Minions;

                IEnumerable<Obj_AI_Base> rCol = Collision.GetCollision(new List<Vector3> {  result.predPos }, predInput).ToArray();
                IEnumerable<Obj_AI_Base> rObjCol = rCol as Obj_AI_Base[] ?? rCol.ToArray();

                if (rObjCol.Count() > 0)
                    result.Hitchance = 0;
            }
            return result;
        }

        private static void OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs Args)
        {
            if (sender.IsEnemy && sender.IsChampion())
            {
                newPath.Remove(sender.NetworkId);
                newPath.Add(sender.NetworkId, GetNow());
            }
        }
    }
}
