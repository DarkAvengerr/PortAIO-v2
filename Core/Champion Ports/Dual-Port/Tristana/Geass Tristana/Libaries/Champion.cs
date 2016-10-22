using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Geass_Tristana.Libaries
{
    internal class Champion : GeassLib.Interfaces.Core.Champion
    {
        public Champion(float qRange, float wRange, float eRange, float rRange)
        {
            GetSpellQ = new Spell(SpellSlot.Q, qRange);
            GetSpellW = new Spell(SpellSlot.W, wRange);
            GetSpellE = new Spell(SpellSlot.E, eRange);
            GetSpellR = new Spell(SpellSlot.R, rRange);

            GetSpellW.SetSkillshot(0.35f, 250f, 1400f, false, SkillshotType.SkillshotCircle);
        }

        public Spell GetSpellE { get; set; }

        public Spell GetSpellQ { get; set; }

        public Spell GetSpellR { get; set; }

        public Spell GetSpellW { get; set; }

        public int GetManaPercent => (int)(Player.Mana / Player.MaxMana * 100);
        public int HealthPercent => (int)(Player.Health / Player.MaxHealth * 100);

        public void UpdateChampionRange(int level)
        {
            GetSpellQ.Range = 550 + (9 * (level - 1));
            GetSpellE.Range = 625 + (9 * (level - 1));
            GetSpellR.Range = 517 + (9 * (level - 1));
        }

        public AIHeroClient Player { get; set; }
    }
}