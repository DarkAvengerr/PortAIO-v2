using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Utility
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Orbwalking = Orbwalking;

    internal class Items : Program
    {
        private static readonly LeagueSharp.Common.Items.Item BlutTrinket = new LeagueSharp.Common.Items.Item(3363);
        private new static readonly Menu Menu = Utilitymenu;

        public static void Init()
        {
            var ItemsMenu = Menu.AddSubMenu(new Menu("Items", "Items"));
            {
                ItemsMenu.AddItem(new MenuItem("Youmuus", "Use Youmuu's Ghostblade", true).SetValue(true));
                ItemsMenu.AddItem(new MenuItem("Cutlass", "Use Bilgewater Cutlass", true).SetValue(true));
                ItemsMenu.AddItem(new MenuItem("Botrk", "Use Blade of the Ruined King", true).SetValue(true));
                ItemsMenu.AddItem(new MenuItem("BlueTrinket", "Auto Buy Blue Trinket", true).SetValue(true));
            }

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var target = TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(Me),
                    TargetSelector.DamageType.Physical);

                if (target != null && target.IsHPBarRendered)
                {
                    if (Menu.Item("Youmuus", true).GetValue<bool>() && LeagueSharp.Common.Items.HasItem(3142) &&
                        target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me) + 150))
                    {
                        LeagueSharp.Common.Items.UseItem(3142);
                    }

                    if (Menu.Item("Cutlass", true).GetValue<bool>() && LeagueSharp.Common.Items.HasItem(3144) &&
                        target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)) && target.HealthPercent < 80)
                    {
                        LeagueSharp.Common.Items.UseItem(3144, target);
                    }

                    if (Menu.Item("Botrk", true).GetValue<bool>() && LeagueSharp.Common.Items.HasItem(3153) &&
                        target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Me)) &&
                        (target.HealthPercent < 80 || Me.HealthPercent < 80))
                    {
                        LeagueSharp.Common.Items.UseItem(3153, target);
                    }
                }
            }

            if (!Menu.Item("BlueTrinket", true).GetValue<bool>())
            {
                return;
            }

            if (Me.Level >= 9 && Me.InFountain() && !BlutTrinket.IsOwned())
            {
                Shop.BuyItem(ItemId.Farsight_Alteration);
            }
        }
    }
}
