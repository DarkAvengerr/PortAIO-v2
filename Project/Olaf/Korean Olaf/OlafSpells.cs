using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanOlaf
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    class OlafSpells
    {
        public OlafSpell Q { get; set; }
        public OlafSpell W { get; set; }
        public OlafSpell E { get; set; }
        public OlafSpell R { get; set; }

        public OlafSpells()
        {
            float qRange = 1000F; 
            float qDelay = 0.25F; 
            float qWidth = 90F;
            float qSpeed = 1600F; 

            Console.WriteLine(ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.LineWidth);

            Q = new OlafSpell(SpellSlot.Q, qRange, TargetSelector.DamageType.Physical);
            Q.SetSkillshot(qDelay, qWidth, qSpeed, false, SkillshotType.SkillshotLine);

            W = new OlafSpell(SpellSlot.W);

            float eRange = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).SData.CastRange;
            E = new OlafSpell(SpellSlot.E, eRange, TargetSelector.DamageType.True);

            R = new OlafSpell(SpellSlot.R, 1500F);
        }
    }
}
