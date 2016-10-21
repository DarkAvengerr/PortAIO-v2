using Geass_Tristana.Misc;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Linq;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Geass_Tristana.Events
{
    internal partial class OrbwalkerEvents : Core
    {
        public const string MenuItemBase = ".Orbwalker.";
        public const string MenuNameBase = ".Orbwalker Menu";

        public const string ManaMenuItemBase = ".ManaManager.";
        public const string ManaMenuNameBase = ".ManaManager Menu";
        private readonly Misc.Damage _damageLib = new Misc.Damage();

        private void OrbwalkModeHandler()
        {
           //  Logger.WriteLog($"Orbwalker mode {CommonOrbwalker.ActiveMode}");
            switch (CommonOrbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Mixed();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
            }
        }

        public void OnUpdate(EventArgs args)
        {
            if (!DelayHandler.CheckOrbwalk()) return;
            DelayHandler.UseOrbwalk();

            if (CommonOrbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && SMenu.Item(MenuNameBase + "Clear.Boolean.FocusETarget").GetValue<bool>())
            {
                foreach (
                    var minon in
                        MinionManager.GetMinions(Champion.Player.Position, Champion.GetSpellQ.Range,
                            MinionTypes.All, MinionTeam.NotAlly).Where(charge => charge.HasBuff("TristanaECharge") && charge.IsValidTarget(Champion.GetSpellQ.Range)))
                {
                     Logger.WriteLog($"Orbwalker Force Target {minon.Name}");
                    CommonOrbwalker.ForceTarget(minon);
                    return;
                }
            }
            else if (CommonOrbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed
               && SMenu.Item(MenuNameBase + "Mixed.Boolean.FocusETarget").GetValue<bool>() ||
               CommonOrbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
               SMenu.Item(MenuNameBase + "Combo.Boolean.FocusETarget").GetValue<bool>())
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(target => target.HasBuff("TristanaECharge") && target.IsValidTarget(Champion.GetSpellQ.Range)))
                {
                     Logger.WriteLog($"Orbwalker Force Target {enemy.Name}");
                    CommonOrbwalker.ForceTarget(enemy);
                    return;
                }
            }

            OrbwalkModeHandler();
        }
    }
}