using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Common.Evade
{
    using LeagueSharp;
    using LeagueSharp.SDK;

    static class Helpers
    {
        public static bool IsSpellShielded(this AIHeroClient unit)
        {
            if (GameObjects.Player.HasBuffOfType(BuffType.SpellShield))
            {
                return true;
            }

            if (GameObjects.Player.HasBuffOfType(BuffType.SpellImmunity))
            {
                return true;
            }

            //Sivir E
            if (unit.GetLastCastedSpell().Name == "SivirE" && (Variables.TickCount - unit.GetLastCastedSpell().EndTime) < 300)
            {
                return true;
            }

            //Morganas E
            if (unit.GetLastCastedSpell().Name == "BlackShield" && (Variables.TickCount - unit.GetLastCastedSpell().EndTime) < 300)
            {
                return true;
            }

            //Nocturnes E
            if (unit.GetLastCastedSpell().Name == "NocturneShit" && (Variables.TickCount - unit.GetLastCastedSpell().EndTime) < 300)
            {
                return true;
            }

            return false;
        }
    }
}
