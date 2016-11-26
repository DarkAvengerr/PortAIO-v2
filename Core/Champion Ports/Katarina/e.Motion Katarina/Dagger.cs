using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace e.Motion_Katarina
{
    public class Dagger
    {
        private static readonly int DELAY = 0;
        private static readonly int MAXACTIVETIME = 4000;
        private bool Destructable;
        private Vector3 Position;
        private int Time;
        
        public Dagger(Vector3 position)
        {
            Time = Utils.TickCount + DELAY;
            this.Position = position;
        }

        //Object Pooling Pseudo-Constructor
        public void Recreate(Vector3 position)
        {
            Destructable = false;
            Time = Utils.TickCount + DELAY;
            this.Position = position;
        }

        public Vector3 GetPosition()
        {
            return Position;
        }

        public bool IsDead()
        {
            return Destructable;
        }
        public void MarkDead()
        {
            Destructable = true;
        }

        public bool IsActive()
        {
            return Utils.TickCount >= Time;
        }

        public void CheckForUpdate()
        {
            if(Time + MAXACTIVETIME <= Utils.TickCount)
            {
                Destructable = true;
                return;
            }
        }
       
    }
}
