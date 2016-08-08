using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Soraka_HealBot
{
    using System;
    using System.Linq;

    using LeagueSharp.Common;

    using Soraka_HealBot.Modes;

    internal static class Config
    {
        #region Properties

        internal static Menu AssistKs { get; private set; }

        internal static Menu AutoEMenu { get; private set; }

        internal static Menu AutoHarass { get; private set; }

        internal static Menu AutoRMenu { get; private set; }

        internal static Menu AutoWMenu { get; private set; }

        internal static Menu Combo { get; private set; }

        internal static Menu CustomPrio { get; private set; }

        internal static Menu Draw { get; private set; }

        internal static Menu Gapclose { get; private set; }

        internal static Menu Harass { get; private set; }

        internal static Menu HealPW { get; private set; }

        internal static Menu HealPWQ { get; private set; }

        internal static Menu Interrupter { get; private set; }

        internal static Menu LaneClear { get; private set; }

        internal static Menu OrbwalkMenu { get; private set; }

        internal static Menu PredictionMenu { get; private set; }

        internal static Menu Soraka { get; private set; }

        internal static Menu TargetSelectorMenu { get; private set; }

        internal static Menu WhiteListR { get; private set; }

        internal static Menu WhiteListW { get; private set; }

        internal static Menu WSettings { get; private set; }

        #endregion

        #region Methods

        internal static void BuildMenu()
        {
            Soraka = new Menu("Soraka HealBot", "healbot", true);

            TargetSelectorMenu = new Menu("Target Selector", "targetselector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            Soraka.AddSubMenu(TargetSelectorMenu);

            OrbwalkMenu = new Menu("Orbwalker", "orbwalker");
            Mainframe.Orbwalker = new Orbwalking.Orbwalker(OrbwalkMenu);
            Soraka.AddSubMenu(OrbwalkMenu);

            PredictionMenu = new Menu("Prediction", "prediction");
            PredictionMenu.AddItem(
                new MenuItem("pred.hitchance.q", "Q Prediction Hitchance").SetValue(
                    new StringList(
                        new[]
                            {
                                HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(), 
                                HitChance.VeryHigh.ToString()
                            }, 
                        2)));
            PredictionMenu.AddItem(
                new MenuItem("pred.hitchance.e", "E Prediction Hitchance").SetValue(
                    new StringList(
                        new[]
                            {
                                HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(), 
                                HitChance.VeryHigh.ToString()
                            }, 
                        2)));
            Soraka.AddSubMenu(PredictionMenu);

            AutoEMenu = new Menu("Auto E", "autoe");
            AutoEMenu.AddItem(new MenuItem("autoe.humanize", "Humanize").SetValue(true));
            AutoEMenu.AddItem(
                new MenuItem("autoe.lowerhuman", "Lower randomize Bound").SetValue(new Slider(90, 0, 250))).ValueChanged
                += OnLowerChange;
            AutoEMenu.AddItem(
                new MenuItem("autoe.upperhuman", "Upper randomize Bound").SetValue(new Slider(140, 10, 350)))
                .ValueChanged += OnUpperChange;
            foreach (var spl in
                NerfEverything.TargettedMoving.Where(
                    x =>
                    HeroManager.Enemies.Any(
                        y => y.ChampionName.Equals(x.ChampionName, StringComparison.CurrentCultureIgnoreCase))))
            {
                var hero =
                    HeroManager.Enemies.FirstOrDefault(
                        x => x.ChampionName.Equals(spl.ChampionName, StringComparison.CurrentCultureIgnoreCase));

                var spellSlot =
                    hero?.Spellbook.Spells.FirstOrDefault(
                        x => x.SData.Name.Equals(spl.SDataName, StringComparison.CurrentCultureIgnoreCase));
                if (spellSlot != null)
                {
                    AutoEMenu.AddItem(
                        new MenuItem(
                            "autoe." + spl.SDataName, 
                            $"{char.ToUpper(spl.ChampionName[0]) + spl.ChampionName.Substring(1)} ({spellSlot.Slot}) - {spl.SDataName}")
                            .SetValue(true));
                }
            }

            foreach (var spl in
                NerfEverything.NonMoving.Where(
                    x =>
                    HeroManager.Enemies.Any(
                        y => y.ChampionName.Equals(x.ChampionName, StringComparison.CurrentCultureIgnoreCase))))
            {
                var hero =
                    HeroManager.Enemies.FirstOrDefault(
                        x => x.ChampionName.Equals(spl.ChampionName, StringComparison.CurrentCultureIgnoreCase));

                var spellSlot =
                    hero?.Spellbook.Spells.FirstOrDefault(
                        x => x.SData.Name.Equals(spl.SDataName, StringComparison.CurrentCultureIgnoreCase));
                if (spellSlot != null)
                {
                    AutoEMenu.AddItem(
                        new MenuItem(
                            "autoe." + spl.SDataName, 
                            $"{char.ToUpper(spl.ChampionName[0]) + spl.ChampionName.Substring(1)} ({spellSlot.Slot}) - {spl.SDataName}")
                            .SetValue(true));
                }
            }

            foreach (var spl in
                NerfEverything.NonTargetMoving.Where(
                    x =>
                    HeroManager.Enemies.Any(
                        y => y.ChampionName.Equals(x.ChampionName, StringComparison.CurrentCultureIgnoreCase))))
            {
                var hero =
                    HeroManager.Enemies.FirstOrDefault(
                        x => x.ChampionName.Equals(spl.ChampionName, StringComparison.CurrentCultureIgnoreCase));

                var spellSlot =
                    hero?.Spellbook.Spells.FirstOrDefault(
                        x => x.SData.Name.Equals(spl.SDataName, StringComparison.CurrentCultureIgnoreCase));
                if (spellSlot != null)
                {
                    AutoEMenu.AddItem(
                        new MenuItem(
                            "autoe." + spl.SDataName, 
                            $"{char.ToUpper(spl.ChampionName[0]) + spl.ChampionName.Substring(1)} ({spellSlot.Slot}) - {spl.SDataName}")
                            .SetValue(true));
                }
            }

            /*foreach (var spl in NerfEverything.TargettedMoving)
            {
                AutoEMenu.AddItem(new MenuItem("autoe." + spl, "Auto E on: " + spl).SetValue(true));
            }

            foreach (var spl in NerfEverything.NonMoving)
            {
                AutoEMenu.AddItem(new MenuItem("autoe." + spl, "Auto E on: " + spl).SetValue(true));
            }

            foreach (var spl in NerfEverything.NonTargetMoving)
            {
                AutoEMenu.AddItem(new MenuItem("autoe." + spl, "Auto E on: " + spl).SetValue(true));
            }*/
            Soraka.AddSubMenu(AutoEMenu);

            Combo = new Menu("Combo", "combo");
            Combo.AddItem(new MenuItem("useQInCombo", "Use Q").SetValue(true));
            Combo.AddItem(new MenuItem("useEInCombo", "Use E").SetValue(true));
            Combo.AddItem(new MenuItem("eOnlyCC", "Use E only on immobile").SetValue(false));
            Combo.AddItem(new MenuItem("comboDisableAA", "Disable AA on heroes in combo mode").SetValue(false));
            Combo.AddItem(new MenuItem("bLvlDisableAA", "Disable AA after Level x").SetValue(false));
            Combo.AddItem(new MenuItem("lvlDisableAA", "Min Level to disable AA").SetValue(new Slider(8, 1, 18)));
            Soraka.AddSubMenu(Combo);

            Harass = new Menu("Harass", "harass");
            Harass.AddItem(new MenuItem("useQInHarass", "Use Q").SetValue(true));
            Harass.AddItem(new MenuItem("useEInHarass", "Use E").SetValue(false));
            Harass.AddItem(new MenuItem("disableAAH", "Disable AA on minions while Harass").SetValue(true));
            Harass.AddItem(
                new MenuItem("allyRangeH", "Allies in range x to disable AA on minions").SetValue(
                    new Slider(1400, 0, 5000)));
            Harass.AddItem(new MenuItem("eOnlyCCHarass", "Use E only on immobile").SetValue(true));
            Harass.AddItem(new MenuItem("manaHarass", "Min Mana % to Harass").SetValue(new Slider(40)));
            Soraka.AddSubMenu(Harass);

            AutoHarass = new Menu("Auto Harass", "autoharass");
            AutoHarass.AddItem(new MenuItem("autoQHarass", "Auto Q").SetValue(false));
            AutoHarass.AddItem(new MenuItem("autoEHarass", "Auto E").SetValue(false));
            AutoHarass.AddItem(new MenuItem("autoEHarassOnlyCC", "Use Auto E only on immobile").SetValue(true));
            AutoHarass.AddItem(new MenuItem("dontAutoHarassTower", "Dont Auto Harass under Tower").SetValue(true));
            AutoHarass.AddItem(new MenuItem("dontHarassInBush", "Dont Auto Harass when in Bush").SetValue(true));
            AutoHarass.AddItem(new MenuItem("manaAutoHarass", "Min Mana % to Auto Harass").SetValue(new Slider(60)));
            Soraka.AddSubMenu(AutoHarass);

            LaneClear = new Menu("LaneClear", "laneclear");
            LaneClear.AddItem(new MenuItem("useQInLC", "Use Q").SetValue(true));
            LaneClear.AddItem(new MenuItem("qTargets", "Min Targets to hit for Q").SetValue(new Slider(6, 1, 10)));
            LaneClear.AddItem(new MenuItem("manaLaneClear", "Min Mana % to LaneClear").SetValue(new Slider(60)));
            Soraka.AddSubMenu(LaneClear);

            var allAllies = HeroManager.Allies.Where(ally => !ally.IsMe).ToArray();
            AutoWMenu = new Menu("HealBot W", "healbotw");
            AutoWMenu.AddItem(new MenuItem("autoW", "Auto use W").SetValue(true));
            AutoWMenu.AddItem(
                new MenuItem("wHealMode", "Priority Mode").SetValue(
                    new StringList(
                        new[] { "Lowest Health", "Total AD", "Total AP", "AD+AP", "Closest", "Custom Priority" })));
            AutoWMenu.AddItem(new MenuItem("manaToW", "Min Mana % to Auto W").SetValue(new Slider(10)));
            AutoWMenu.AddItem(new MenuItem("playerHpToW", "Min Player HP % to Auto W").SetValue(new Slider(25, 6)));
            WhiteListW = new Menu("Whitelist", "whitelist");
            foreach (var ally in allAllies)
            {
                WhiteListW.AddItem(
                    new MenuItem("autoW_" + ally.ChampionName, "Auto Heal " + ally.ChampionName + " with W").SetValue(
                        true));
            }

            AutoWMenu.AddSubMenu(WhiteListW);
            WSettings = new Menu("Heal HP % Settings", "wsettings");
            HealPW = new Menu("HP % without Q Buff", "without");
            foreach (var ally in allAllies)
            {
                HealPW.AddItem(
                    new MenuItem("autoW_HP_" + ally.ChampionName, "Hp % to heal " + ally.ChampionName + " with W")
                        .SetValue(new Slider(50, 1)));
            }

            WSettings.AddSubMenu(HealPW);
            HealPWQ = new Menu("HP % with Q Buff", "with");
            foreach (var ally in allAllies)
            {
                HealPWQ.AddItem(
                    new MenuItem(
                        "autoWBuff_HP_" + ally.ChampionName, 
                        "Hp % to heal " + ally.ChampionName + " with W + Q Buff").SetValue(new Slider(75, 1)));
            }

            WSettings.AddSubMenu(HealPWQ);
            CustomPrio = new Menu("Custom Priority", "customprio");
            foreach (var ally in allAllies)
            {
                CustomPrio.AddItem(
                    new MenuItem("autoWPrio" + ally.ChampionName, "Custom Prio: " + ally.ChampionName).SetValue(
                        new Slider(1, 1, 5)));
            }

            WSettings.AddSubMenu(CustomPrio);
            AutoWMenu.AddSubMenu(WSettings);
            Soraka.AddSubMenu(AutoWMenu);

            AutoRMenu = new Menu("HealBot R", "healbotr");
            AutoRMenu.AddItem(new MenuItem("autoR", "Auto use R").SetValue(true));
            AutoRMenu.AddItem(new MenuItem("cancelBase", "Cancel Recall to Auto R").SetValue(true));
            AutoRMenu.AddItem(new MenuItem("autoRHP", "HP % to trigger R Logic").SetValue(new Slider(15, 1)));
            WhiteListR = new Menu("Whitelist", "whitelist");
            foreach (var ally in allAllies)
            {
                WhiteListR.AddItem(
                    new MenuItem("autoR_" + ally.ChampionName, "Auto Heal " + ally.ChampionName + " with R").SetValue(
                        true));
            }

            AutoRMenu.AddSubMenu(WhiteListR);
            Soraka.AddSubMenu(AutoRMenu);

            AssistKs = new Menu("AssistKS", "assistks");
            AssistKs.AddItem(new MenuItem("autoAssistKS", "Use R to Auto AssistKS").SetValue(false));
            AssistKs.AddItem(new MenuItem("assCancelBase", "Cancel Recall to AssistKS").SetValue(false));
            Soraka.AddSubMenu(AssistKs);

            Interrupter = new Menu("Interrupter", "interrupter");
            Interrupter.AddItem(new MenuItem("bInterrupt", "Interrupt spells with E").SetValue(true));
            Interrupter.AddItem(
                new MenuItem("dangerL", "Min DangerLevel to interrupt").SetValue(
                    new StringList(new[] { "Low", "Medium", "High" })));
            Soraka.AddSubMenu(Interrupter);

            Gapclose = new Menu("Anti Gapcloser", "antigapcloser");
            Gapclose.AddItem(new MenuItem("qGapclose", "Anti Gapclose with Q").SetValue(false));
            Gapclose.AddItem(new MenuItem("eGapclose", "Anti Gapclose with E").SetValue(false));
            Soraka.AddSubMenu(Gapclose);

            Draw = new Menu("Drawings", "drawings");
            Draw.AddItem(new MenuItem("wRangeDraw", "Draw W Range").SetValue(false));
            Draw.AddItem(new MenuItem("qRange", "Draw Q Range").SetValue(false));
            Draw.AddItem(new MenuItem("eRange", "Draw E Range").SetValue(false));
            Draw.AddItem(new MenuItem("onlyReady", "Only when Spells are ready").SetValue(true));
            Soraka.AddSubMenu(Draw);

            Soraka.AddToMainMenu();
        }

        internal static int GetSliderValue(string itemName)
        {
            return Soraka.Item(itemName).GetValue<Slider>().Value;
        }

        internal static int GetStringListValue(string itemName)
        {
            return Soraka.Item(itemName).GetValue<StringList>().SelectedIndex;
        }

        internal static bool IsChecked(string itemName)
        {
            return Soraka.Item(itemName).GetValue<bool>();
        }

        internal static bool IsKeyPressed(string itemName)
        {
            return Soraka.Item(itemName).GetValue<KeyBind>().Active;
        }

        private static void OnLowerChange(object sender, OnValueChangeEventArgs e)
        {
            if (e.GetNewValue<Slider>().Value >= GetSliderValue("autoe.upperhuman"))
            {
                e.Process = false;
            }
        }

        private static void OnUpperChange(object sender, OnValueChangeEventArgs e)
        {
            if (e.GetNewValue<Slider>().Value <= GetSliderValue("autoe.lowerhuman"))
            {
                e.Process = false;
            }
        }

        #endregion
    }
}