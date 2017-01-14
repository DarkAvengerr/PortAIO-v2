using EloBuddy; 
using LeagueSharp.Common; 
namespace MoonRiven
{
    using System.Drawing;
    using LeagueSharp.Common;

    internal class MenuInit : Logic
    {
        internal static void Init()
        {
            Menu = new Menu("MoonRiven", "MoonRiven", true).SetFontStyle(FontStyle.Regular, menuColor);

            var targetMenu = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            {
                TargetSelector.AddToMenu(targetMenu);
            }

            var orbMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            {
                Orbwalker = new Orbwalking.Orbwalker(orbMenu);
            }

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQGap", "Use Q Gapcloser", true).SetValue(false));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboWLogic", "Use W Logic", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboEGap", "Use E Gapcloser", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR1", "Use R1", true).SetValue(new KeyBind('G', KeyBindType.Toggle, true)));
                comboMenu.AddItem(
                    new MenuItem("ComboR2", "Use R2 Mode: ", true).SetValue(
                        new StringList(new[] {"my Logic", "Only KillSteal", "First Cast", "Off"})));
                comboMenu.AddItem(new MenuItem("ComboItem", "Use Timat", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboYoumuu", "Use Youmuu", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboDot", "Use Ignite", true).SetValue(true));
            }

            var burstMenu = Menu.AddSubMenu(new Menu("Burst", "Burst"));
            {
                burstMenu.AddItem(new MenuItem("BurstFlash", "Use Flash", true).SetValue(true));
                burstMenu.AddItem(new MenuItem("BurstDot", "Use Ignite", true).SetValue(true));
                burstMenu.AddItem(
                    new MenuItem("BurstMode", "Burst Mode: ", true).SetValue(
                        new StringList(new[] {"Shy Mode", "EQ Flash Mode"})));
                burstMenu.AddItem(
                    new MenuItem("BurstSwitch", "Switch Burst Mode Key", true).SetValue(
                        new KeyBind('H',KeyBindType.Press))).ValueChanged += SwitchBurstMode;
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassMode", "Harass Mode: ", true).SetValue(new StringList(new[] { "Smart", "Normal" })));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearQSmart", "Use Q Smart Farm", true).SetValue(true)); 
                    laneClearMenu.AddItem(new MenuItem("LaneClearQT", "Use Q Reset Attack Turret", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearWCount", "Use W| Min hit Count >= x", true).SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(new MenuItem("LaneClearItem", "Use Item", true).SetValue(true));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearItem", "Use Item", true).SetValue(true));
                }
            }

            var fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                fleeMenu.AddItem(new MenuItem("FleeQ", "Use Q", true).SetValue(true));
                fleeMenu.AddItem(new MenuItem("FleeW", "Use W", true).SetValue(true));
                fleeMenu.AddItem(new MenuItem("FleeE", "Use E", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var qMenu = miscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
                {
                    qMenu.AddItem(new MenuItem("KeepQ", "Keep Q alive", true).SetValue(true));
                    qMenu.AddItem(
                        new MenuItem("QMode", "Q Mode: ", true).SetValue(new StringList(new[] {"To target", "To mouse"})));
                }

                var wMenu = miscMenu.AddSubMenu(new Menu("W Settings", "W Settings"));
                {
                    wMenu.AddItem(new MenuItem("AntiGapcloserW", "Anti Gapcloser", true).SetValue(true));
                    wMenu.AddItem(new MenuItem("InterruptW", "Interrupt Danger Spell", true).SetValue(true));
                }

                var eMenu = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    eMenu.AddItem(new MenuItem("DodgeE", "dodge", true).SetValue(true));
                }
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawRStatus", "Draw R Status", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("DrawBurst", "Draw Burst Status", true).SetValue(true));
                DamageIndicator.AddToMenu(drawMenu);
            }


            Menu.AddItem(new MenuItem("asdqwe123asd", " ", true));
            Menu.AddItem(new MenuItem("Credits", "Credit: NightMoon", true).SetFontStyle(FontStyle.Regular, menuColor));
            Menu.AddToMainMenu();
        }

        private static void SwitchBurstMode(object obj, OnValueChangeEventArgs Args)
        {
            if (Args.GetNewValue<KeyBind>().Active)
            {
                switch (Menu.Item("BurstMode", true).GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        Menu.Item("BurstMode", true).SetValue(new StringList(new[] { "Shy Mode", "EQ Flash Mode" }, 1));
                        break;
                    case 1:
                        Menu.Item("BurstMode", true).SetValue(new StringList(new[] { "Shy Mode", "EQ Flash Mode" }));
                        break;
                }
            }
        }

        internal static bool ComboQGap => Menu.Item("ComboQGap", true).GetValue<bool>();
        internal static bool ComboW => Menu.Item("ComboW", true).GetValue<bool>();
        internal static bool ComboWLogic => Menu.Item("ComboWLogic", true).GetValue<bool>();
        internal static bool ComboE => Menu.Item("ComboE", true).GetValue<bool>();
        internal static bool ComboEGap => Menu.Item("ComboEGap", true).GetValue<bool>();
        internal static bool ComboR => Menu.Item("ComboR1", true).GetValue<KeyBind>().Active;
        internal static int ComboR2 => Menu.Item("ComboR2", true).GetValue<StringList>().SelectedIndex;
        internal static bool ComboItem => Menu.Item("ComboItem", true).GetValue<bool>();
        internal static bool ComboYoumuu => Menu.Item("ComboYoumuu", true).GetValue<bool>();
        internal static bool ComboDot => Menu.Item("ComboDot", true).GetValue<bool>();
        internal static bool BurstFlash => Menu.Item("BurstFlash", true).GetValue<bool>();
        internal static bool BurstDot => Menu.Item("BurstDot", true).GetValue<bool>();
        internal static int BurstMode => Menu.Item("BurstMode", true).GetValue<StringList>().SelectedIndex;
        internal static bool HarassQ => Menu.Item("HarassQ", true).GetValue<bool>();
        internal static bool HarassW => Menu.Item("HarassW", true).GetValue<bool>();
        internal static bool HarassE => Menu.Item("HarassE", true).GetValue<bool>();
        internal static int HarassMode => Menu.Item("HarassMode", true).GetValue<StringList>().SelectedIndex;
        internal static bool LaneClearQ => Menu.Item("LaneClearQ", true).GetValue<bool>();
        internal static bool LaneClearQSmart => Menu.Item("LaneClearQSmart", true).GetValue<bool>();
        internal static bool LaneClearQT => Menu.Item("LaneClearQT", true).GetValue<bool>();
        internal static bool LaneClearItem => Menu.Item("LaneClearItem", true).GetValue<bool>();
        internal static int LaneClearWCount => Menu.Item("LaneClearWCount", true).GetValue<Slider>().Value;
        internal static bool LaneClearW => Menu.Item("LaneClearW", true).GetValue<bool>();
        internal static bool JungleClearQ => Menu.Item("JungleClearQ", true).GetValue<bool>();
        internal static bool JungleClearW => Menu.Item("JungleClearW", true).GetValue<bool>();
        internal static bool JungleClearE => Menu.Item("JungleClearE", true).GetValue<bool>();
        internal static bool JungleClearItem => Menu.Item("JungleClearItem", true).GetValue<bool>();
        internal static bool FleeQ => Menu.Item("FleeQ", true).GetValue<bool>();
        internal static bool FleeW => Menu.Item("FleeW", true).GetValue<bool>();
        internal static bool FleeE => Menu.Item("FleeE", true).GetValue<bool>();
        internal static bool KeepQ => Menu.Item("KeepQ", true).GetValue<bool>();
        internal static int QMode => Menu.Item("QMode", true).GetValue<StringList>().SelectedIndex;
        internal static bool AntiGapcloserW => Menu.Item("AntiGapcloserW", true).GetValue<bool>();
        internal static bool InterruptW => Menu.Item("InterruptW", true).GetValue<bool>();
        internal static bool DodgeE => Menu.Item("DodgeE", true).GetValue<bool>();
        internal static bool DrawW => Menu.Item("DrawW", true).GetValue<bool>();
        internal static bool DrawE => Menu.Item("DrawE", true).GetValue<bool>();
        internal static bool DrawRStatus => Menu.Item("DrawRStatus", true).GetValue<bool>();
        internal static bool DrawBurst => Menu.Item("DrawBurst", true).GetValue<bool>();
    }
}