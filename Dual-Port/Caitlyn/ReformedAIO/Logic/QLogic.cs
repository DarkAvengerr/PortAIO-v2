using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Caitlyn.Logic
{
    using LeagueSharp;
    using LeagueSharp.SDK;

    internal class QLogic
    {
        public float QDelay(AIHeroClient target)
        {
            var time = target.Distance(Vars.Player)/Spells.Spell[SpellSlot.Q].Speed;

            return time + Spells.Spell[SpellSlot.Q].Delay;
        }

        public bool CanKillSteal(AIHeroClient target)
        {
            return QDelay(target) > target.MoveSpeed;
        }
    }
}
