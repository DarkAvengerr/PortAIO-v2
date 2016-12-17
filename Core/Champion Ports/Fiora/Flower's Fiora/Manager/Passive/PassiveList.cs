using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Fiora.Manager.Passive
{
    using LeagueSharp;

    internal class PassiveList
    {
        public PassiveType PassiveType;
        public Obj_GeneralParticleEmitter Passive;
        public PassiveDirection Direction;

        public PassiveList(Obj_GeneralParticleEmitter pass, PassiveType type, PassiveDirection dircetion)
        {
            Passive = pass;
            PassiveType = type;
            Direction = dircetion;
        }
    }
}