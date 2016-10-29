using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Library.Spell_Information
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal sealed class SpellInformation
    {
        public float TravelTime(Obj_AI_Base target, float delay, float speed)
        {
            return ObjectManager.Player.Distance(target.Position) / (speed + delay);
        }


    }
}
