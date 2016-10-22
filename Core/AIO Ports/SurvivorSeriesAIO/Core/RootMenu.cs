// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootMenu.cs" company="SurvivorSeriesAIO">
//     Copyright (c) SurvivorSeriesAIO. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using Color = SharpDX.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Core
{
    public class RootMenu : IRootMenu
    {
        public RootMenu(string name)
        {
            Root = MenuFactory.Create(name).SetFontStyle(FontStyle.Bold, Color.DeepPink);
            TargetSelector = MenuFactory.CreateMenu(Root, "Target Selector")
                .SetFontStyle(FontStyle.Bold, Color.Chartreuse);
            Orbwalking = MenuFactory.CreateMenu(Root, "Orbwalking").SetFontStyle(FontStyle.Bold, Color.Chartreuse);

            AutoLeveler = MenuFactory.CreateMenu(Root, "[SS] AutoLeveler")
                .SetFontStyle(FontStyle.Bold, Color.Chartreuse);

            Activator = MenuFactory.CreateMenu(Root, "[SS] Activator")
                .SetFontStyle(FontStyle.Bold, Color.Chartreuse);

            AutoLevelerInit(MenuItemFactory.Create(AutoLeveler));
            ActivatorInit(MenuItemFactory.Create(Activator));

            Prediction = MenuFactory.CreateMenu(Root, "Prediction").SetFontStyle(FontStyle.Bold, Color.Chartreuse);

            PredictionInit(MenuItemFactory.Create(Prediction));

            Champion =
                MenuFactory.CreateMenu(Root, $"Champion: {ObjectManager.Player.ChampionName}")
                    .SetFontStyle(FontStyle.Bold, Color.Chartreuse);

            SettingsMisc = MenuFactory.CreateMenu(Root, "[SS] Settings")
                .SetFontStyle(FontStyle.Bold, Color.DeepPink);

            SettingsMiscInit(MenuItemFactory.Create(SettingsMisc));
            // Note to myself: add some more menu's or item's BUT ONLY if they are champ independent
            // Note to myself: otherwise put them in Activator / Champion class
            Root.AddToMainMenu();
        }

        public Menu Prediction { get; }
        public Menu SettingsMisc { get; }
        public MenuItem Credits { get; private set; }
        public MenuItem Credits2 { get; private set; }
        public MenuItem Credits1 { get; private set; }
        public MenuItem Credits3 { get; private set; }
        public MenuItem SPreditctionLoaded { get; private set; }

        public Menu Orbwalking { get; }

        public Menu Root { get; }

        public Menu TargetSelector { get; }

        public Menu Champion { get; }

        public MenuItem SelectedHitChance { get; set; }

        public MenuItem SelectedPrediction { get; set; }
        public MenuItem PlugAutoLeveler { get; set; }
        public MenuItem PlugActivator { get; set; }
        public Menu AutoLeveler { get; }
        public Menu Activator { get; }

        private void PredictionInit(MenuItemFactory factory)
        {
            SelectedPrediction =
                factory.WithName("Prediction")
                    .WithValue(new StringList(new[] {"Common", "OKTW/Sebby", "SPrediction", "ExoryPrediction (Soon)"}, 1))
                    .Build();

            SelectedPrediction.ValueChanged += (sender, eventArgs) =>
            {
                if (eventArgs.GetNewValue<StringList>().SelectedIndex == 2)
                {
                    SPrediction.Prediction.Initialize(Prediction);
                    SPreditctionLoaded = factory.WithName("SPrediction Loaded!").Build();
                }
            };

            SelectedHitChance =
                factory.WithName("HitChance")
                    .WithValue(new StringList(new[] {"Medium", "High", "Very High"}, 1))
                    .Build();
        }

        private void SettingsMiscInit(MenuItemFactory factory)
        {
            Credits =
                factory.WithName("                                             .:Credits:.")
                    .Build();

            Credits1 =
                factory.WithName("Developer: SupportExTraGoZ")
                    .WithFont(FontStyle.Bold, Color.DeepPink)
                    .Build();

            Credits2 =
                factory.WithName("Designer: gimleey")
                    .WithFont(FontStyle.Bold, Color.DeepPink)
                    .Build();

            Credits3 =
                factory.WithName("Really Good Mentor: You know who you're I thank you alot <3")
                    .WithFont(FontStyle.Bold)
                    .Build();
        }

        private void AutoLevelerInit(MenuItemFactory factory)
        {
            PlugAutoLeveler =
                factory.WithName("[AutoLeveler] Enabled?")
                    .WithValue(true)
                    .Build();
        }

        private void ActivatorInit(MenuItemFactory factory)
        {
            PlugActivator =
                factory.WithName("[Activator] Enabled?")
                    .WithValue(true)
                    .Build();
        }
    }
}