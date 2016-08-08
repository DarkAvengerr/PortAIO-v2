using System;
using System.Drawing;
using DZLib.Logging;
using iSeriesReborn.Utility;
using LeagueSharp;
using LeagueSharp.Common;
using Color = SharpDX.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.External.Activator.Items
{
    class _BOTRK : ISRItem
    {
        public void OnLoad()
        {
            
        }

        public void BuildMenu(Menu RootMenu)
        {
            var itemMenu = new Menu("Blade of the Ruined King","iseriesr.activator.offensive.botrk");
            {
                itemMenu.AddItem(new MenuItem("iseriesr.activator.offensive.botrk.enemy", "On TARGET health % >")).SetFontStyle(FontStyle.Bold, Color.Red).SetValue(new Slider(10));
                itemMenu.AddItem(new MenuItem("iseriesr.activator.offensive.botrk.my", "On MY health % <")).SetFontStyle(FontStyle.Bold, Color.Green).SetValue(new Slider(80));
                itemMenu.AddItem(new MenuItem("iseriesr.activator.offensive.botrk.combo", "Use In Combo")).SetValue(true);
                itemMenu.AddItem(new MenuItem("iseriesr.activator.offensive.botrk.mixed", "Use In Harass")).SetValue(false);
                itemMenu.AddItem(new MenuItem("iseriesr.activator.offensive.botrk.always", "Use Always")).SetValue(false);
                RootMenu.AddSubMenu(itemMenu);
            }
        }

        public ISRItemType GetItemType()
        {
            return ISRItemType.Offensive;
        }

        public bool ShouldRun()
        {
            return LeagueSharp.Common.Items.HasItem(3153) && LeagueSharp.Common.Items.CanUseItem(3153);
        }

        public void Run()
        {
            var currentMenuItem =
                Variables.Menu.Item(
                    $"iseriesr.activator.offensive.botrk.{Variables.Orbwalker.ActiveMode.ToString().ToLower()}");
            var currentValue = currentMenuItem?.GetValue<bool>() ?? false;

            if (currentValue || MenuExtensions.GetItemValue<bool>("iseriesr.activator.offensive.botrk.always"))
            {
                var target = Variables.Orbwalker.GetTarget();

                if (target.LSIsValidTarget(Orbwalking.GetRealAutoAttackRange(null)) && (target is AIHeroClient))
                {
                    var tg = target as AIHeroClient;
                    if (ObjectManager.Player.HealthPercent <=
                        MenuExtensions.GetItemValue<Slider>("iseriesr.activator.offensive.botrk.my").Value &&
                        (tg.Health / tg.MaxHealth) * 100 >=
                        MenuExtensions.GetItemValue<Slider>("iseriesr.activator.offensive.botrk.enemy").Value)
                    {
                        LeagueSharp.Common.Items.UseItem(3153, tg);
                    }
                }
            }
        }
    }
}
