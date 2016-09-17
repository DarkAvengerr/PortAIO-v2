using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SorakaSharp.Source.Handler
{
    internal static class CCombo
    {
        //Load Spells from the SpellHandler
        private static Spell Q
        {
            get { return CSpell.Q; }
        }

        private static Spell E
        {
            get { return CSpell.E; }
        }

        private static bool ComboValid
        {
            get
            {
                return CConfig.ConfigMenu.Item("comboUseQ").GetValue<bool>() ||
                       CConfig.ConfigMenu.Item("comboUseE").GetValue<bool>();
            }
        }

        internal static void Combo()
        {
            // Validate spell usage
            if (!ComboValid)
                return;

            var target = TargetSelector.GetTarget(Q.IsReady() ? Q.Range : E.Range,
                TargetSelector.DamageType.Magical);

            var useQ = CConfig.ConfigMenu.Item("comboUseQ").GetValue<bool>();
            var useE = CConfig.ConfigMenu.Item("comboUseE").GetValue<bool>();

            if (target != null)
            {
                //Cast Q
                if (useQ && Q.CanCast(target))
                {
                    Q.Cast(target);

                }

                //Cast E
                if (useE && E.CanCast(target))
                {
                    E.Cast(target);
                }
            }
        }
    }
}
