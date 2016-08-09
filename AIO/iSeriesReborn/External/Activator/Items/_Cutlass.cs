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
    class _Cutlass : ISRItem
    {
        public void OnLoad()
        {

        }

        public void BuildMenu(Menu RootMenu)
        {
            var itemMenu = new Menu("Bilgewater Cutlass", "iseriesr.activator.offensive.cutlass");
            {
                itemMenu.AddItem(new MenuItem("iseriesr.activator.offensive.cutlass.enemy", "On TARGET health % >")).SetFontStyle(FontStyle.Bold, Color.Red).SetValue(new Slider(10));
                itemMenu.AddItem(new MenuItem("iseriesr.activator.offensive.cutlass.my", "On MY health % <")).SetFontStyle(FontStyle.Bold, Color.Green).SetValue(new Slider(80));
                itemMenu.AddItem(new MenuItem("iseriesr.activator.offensive.cutlass.combo", "Use In Combo")).SetValue(true);
                itemMenu.AddItem(new MenuItem("iseriesr.activator.offensive.cutlass.mixed", "Use In Harass")).SetValue(false);
                itemMenu.AddItem(new MenuItem("iseriesr.activator.offensive.cutlass.always", "Use Always")).SetValue(false);
                RootMenu.AddSubMenu(itemMenu);
            }
        }

        public ISRItemType GetItemType()
        {
            return ISRItemType.Offensive;
        }

        public bool ShouldRun()
        {
            return LeagueSharp.Common.Items.HasItem(3144) && LeagueSharp.Common.Items.CanUseItem(3144);
        }

        public void Run()
        {
            var currentMenuItem =
                Variables.Menu.Item(
                    $"iseriesr.activator.offensive.cutlass.{Variables.Orbwalker.ActiveMode.ToString().ToLower()}");
            var currentValue = currentMenuItem?.GetValue<bool>() ?? false;


            if (currentValue || MenuExtensions.GetItemValue<bool>("iseriesr.activator.offensive.cutlass.always"))
            {
                var target = TargetSelector.GetTarget(450f, TargetSelector.DamageType.True);
                if (target.IsValidTarget())
                {
                    if (ObjectManager.Player.HealthPercent <=
                        MenuExtensions.GetItemValue<Slider>("iseriesr.activator.offensive.cutlass.my").Value &&
                        target.HealthPercent >=
                        MenuExtensions.GetItemValue<Slider>("iseriesr.activator.offensive.cutlass.enemy").Value)
                    {
                        LeagueSharp.Common.Items.UseItem(3144, target);
                    }
                }
            }
        }
    }
}
