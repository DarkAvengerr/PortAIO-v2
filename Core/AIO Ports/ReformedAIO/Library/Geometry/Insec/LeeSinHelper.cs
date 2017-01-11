using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Library.Geometry.Insec
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    class LeeSinHelper
    {
        public AIHeroClient Ally(float Distance, Vector3 Position)
        {
            return HeroManager.Allies.Where(x => x.IsAlly && !x.IsMe)
                .FirstOrDefault(x => x.Distance(Position) < 225);
        }

        public Obj_AI_Turret AllyTurret(float Distance, Vector3 Position)
        {
            return ObjectManager.Get<Obj_AI_Turret>()
                   .FirstOrDefault(x => x.Distance(Position) < Distance && x.IsAlly && !x.IsDead);
        }

        public Obj_AI_Base Minions(float Distance, Vector3 Position)
        {
            return MinionManager.GetMinions(ObjectManager.Player.Position,
                Distance,
                MinionTypes.All,
                MinionTeam.Ally).FirstOrDefault(x => x.Distance(Position) < 225);
        }

        public Obj_AI_Base Ward(float Distance, Vector3 Position)
        {
            return ObjectManager.Get<Obj_AI_Base>()
                   .Where(x => x.IsAlly && x.Name.ToLower().Contains("ward"))
                   .OrderBy(x => x.Distance(Position) < Distance)
                   .FirstOrDefault(x => x.Distance(Position) < 225);
        }
    }
}
