
using CjShuJinx.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CjShuJinx.Modes
{
    using System;
    using LeagueSharp.Common;

    internal class ModePerma
    {
        private static Spell Q => Champion.PlayerSpells.Q;
        private static Spell W => Champion.PlayerSpells.W;
        private static Spell E => Champion.PlayerSpells.E;
        private static Spell R => Champion.PlayerSpells.R;
        public static void Init()
        {
            // Game.OnUpdate += GameOnOnUpdate;
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (Modes.ModeConfig.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                if (Modes.ModeSettings.MenuSpellE.Item("Settings.E.Auto").GetValue<StringList>().SelectedIndex == 1)
                {
                    var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                    if (t.IsValidTarget() && t.CanStun())
                    {
                     
                    }
                }
            }
        }
    }
}
