using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoGraves
{
    public static class BadaoGraves
    {
        public static void BadaoActivate()
        {
            BadaoGravesConfig.BadaoActivate();
            BadaoGravesCombo.BadaoActivate();
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.E)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(150, () => Orbwalking.ResetAutoAttackTimer());
            }
        }
    }
}
