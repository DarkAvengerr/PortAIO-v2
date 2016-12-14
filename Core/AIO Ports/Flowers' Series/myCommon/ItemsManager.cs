using EloBuddy; 
using LeagueSharp.Common; 
namespace ADCCOMMON
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class ItemsManager
    {
        private static Menu itemMenu;

        public static void AddToMenu(Menu mainMenu)
        {
            itemMenu = mainMenu;

            itemMenu.AddItem(new MenuItem("Youmuus", "Use Youmuu's Ghostblade", true).SetValue(true));
            itemMenu.AddItem(new MenuItem("Cutlass", "Use Bilgewater Cutlass", true).SetValue(true));
            itemMenu.AddItem(new MenuItem("Botrk", "Use Blade of the Ruined King", true).SetValue(true));

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (Flowers_ADC_Series.Logic.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Physical);

                if (target != null && target.IsHPBarRendered)
                {
                    if (itemMenu.Item("Youmuus", true).GetValue<bool>() && Items.HasItem(3142) &&
                        target.IsValidTarget(600 + 150))
                    {
                        Items.UseItem(3142);
                    }

                    if (itemMenu.Item("Cutlass", true).GetValue<bool>() && Items.HasItem(3144) &&
                        target.IsValidTarget(600) && target.HealthPercent < 80)
                    {
                        Items.UseItem(3144, target);
                    }

                    if (itemMenu.Item("Botrk", true).GetValue<bool>() && Items.HasItem(3153) &&
                        target.IsValidTarget(600) &&
                        (target.HealthPercent < 80 || ObjectManager.Player.HealthPercent < 80))
                    {
                        Items.UseItem(3153, target);
                    }
                }
            }
        }
    }
}
