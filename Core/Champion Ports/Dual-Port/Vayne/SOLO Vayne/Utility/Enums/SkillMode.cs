using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne.Utility.Enums
{
    enum SkillMode
    {
        /// <summary>
        /// The Skill logic is called every tick
        /// </summary>
        OnUpdate,

        /// <summary>
        /// The Skill Logic is called after an AA
        /// </summary>
        OnAfterAA
    }
}
