namespace ReformedAIO.Champions.Caitlyn.Logic
{
    using EloBuddy;
    using EloBuddy.SDK;

    internal class QLogic
    {
        public float QDelay(AIHeroClient target)
        {
            var time = target.Distance(Vars.Player)/Spells.Spell[SpellSlot.Q].Speed;

            return time + Spells.Spell[SpellSlot.Q].Delay;
        }
    }
}
