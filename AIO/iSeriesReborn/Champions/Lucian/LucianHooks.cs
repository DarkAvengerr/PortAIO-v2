using iSeriesReborn.Champions.Lucian.Skills;
using iSeriesReborn.Utility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Lucian
{
    class LucianHooks
    {
        public static bool HasPassive = false;

        internal static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (Orbwalking.IsAutoAttack(args.SData.Name) && args.SData.Name != "LucianPassiveAttack")
                {
                    HasPassive = false;

                    switch (Variables.Orbwalker.ActiveMode)
                    {
                        case Orbwalking.OrbwalkingMode.Combo:
                            Combo.ExecuteComboLogic(args);
                            break;
                        case Orbwalking.OrbwalkingMode.Mixed:
                            Combo.ExecuteComboLogic(args);
                            break;

                        case Orbwalking.OrbwalkingMode.LaneClear:

                            break;
                    }
                }
            }
        }

        internal static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            switch (args.Slot)
            {
                case SpellSlot.E:
                case SpellSlot.Q:
                case SpellSlot.W:
                    HasPassive = true;
                    break;
            }
        }

        internal static void OnAfterAA(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //HasPassive = false;
        }
    }
}
