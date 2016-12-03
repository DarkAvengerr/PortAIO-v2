using System;
using LeagueSharp;
using LeagueSharp.Common;
using System.Collections.Generic;
using System.Linq;

using EloBuddy; 
using LeagueSharp.Common; 
namespace e.Motion_Katarina
{
    static class DaggerManager
    {
        private static List<Dagger> AllDaggers = new List<Dagger>();

        public static List<Dagger> GetDaggers()
        {
            return AllDaggers.Where(d => !d.IsDead()).ToList<Dagger>();
        }

        public static List<Dagger> GetDeadDaggers()
        {
            return AllDaggers.Where(d => d.IsDead()).ToList<Dagger>();
        }

        public static void startTracking()
        {
            Obj_AI_Base.OnCreate += OnCreate;
            Game.OnUpdate += CheckForUpdate;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            if(!Config.GetBoolValue("drawings.daggers"))
            {
                return;
            }
            foreach (Dagger dag in GetDaggers())
            {
                if (dag.IsActive())
                {
                    Render.Circle.DrawCircle(dag.GetPosition(), 140, System.Drawing.Color.Aqua);
                }
            }
        }


        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if((sender.Name == "Katarina_Base_E_Beam.troy" || sender.Name == "Katarina_Base_Q_Dagger_Land_Dirt.troy") && sender.Position.Distance(Logic.Player.Position) > 200)
            {
                //ObjectPooling
                //Saving some FPS - Probably not worth it tho
                foreach (Dagger d in GetDeadDaggers())
                {
                    if(d.IsDead())
                    {
                        d.Recreate(sender.Position);
                        return;
                    }
                }
                AllDaggers.Add(new Dagger(sender.Position));
                
            }
            if(sender.Name.Contains("PickUp"))
            {
                foreach (Dagger d in AllDaggers)
                {
                    if(d.GetPosition().Distance(sender.Position) < 200)
                    {
                        d.MarkDead();
                    }
                }
            }
        }

        private static void CheckForUpdate(EventArgs args)
        {
            AllDaggers.ForEach(d => d.CheckForUpdate());
        }        
    }
}
