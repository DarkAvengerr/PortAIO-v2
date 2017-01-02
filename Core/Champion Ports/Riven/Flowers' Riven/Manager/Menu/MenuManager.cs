using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Riven_Reborn.Manager.Menu
{
    using SharpDX;
    using LeagueSharp.Common;
    using FlowersRivenCommon;

    internal class MenuManager : Logic
    {
        private static readonly Color menuColor = new Color(3, 253, 241);

        internal static void Init()
        {
            Menu =
                new Menu("Flowers' Riven Reborn", "Flowers' Riven Reborn", true).SetFontStyle(
                    System.Drawing.FontStyle.Regular, menuColor);

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
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q| Gapcloser", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboQW", "Use Q1 + W", true).SetValue(false));
                comboMenu.AddItem(new MenuItem("ComboEW", "Use E + W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("R1Combo", "Use R1", true).SetValue(new KeyBind('G', KeyBindType.Toggle, true))).Permashow();
                comboMenu.AddItem(
                    new MenuItem("R2Mode", "Use R2 Mode: ", true).SetValue(
                        new StringList(new[] { "Only Killable", "myLogic", "First Cast", "Off" }, 1)));
                comboMenu.AddItem(new MenuItem("ComboIgnite", "Use Ignite", true).SetValue(true));
            }

            var burstMenu = Menu.AddSubMenu(new Menu("Burst", "Burst"));
            {
                burstMenu.AddItem(new MenuItem("BurstFlash", "Use Flash", true).SetValue(true));
                burstMenu.AddItem(new MenuItem("BurstIgnite", "Use Ignite", true).SetValue(true));
                burstMenu.AddItem(new MenuItem("Note...", "Note: ", true));
                burstMenu.AddItem(new MenuItem("target...", "Left Cilck the Target", true));
                burstMenu.AddItem(new MenuItem("range...", "And Target in Burst Range", true));
                burstMenu.AddItem(new MenuItem("press...", "And then Press the Burst Key", true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassMode", "Harass Mode", true).SetValue(new StringList(new[] { "Smart", "Burst" })));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearWLogic", "Use W| Smart", true).SetValue(false));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                }
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealW", "Use W", true).SetValue(true));
                killStealMenu.AddItem(
                    new MenuItem("KillStealE", "Use E", true).SetValue(true).SetTooltip("E Gapcloser and R2 Kill"));
                killStealMenu.AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var qMenu = miscMenu.AddSubMenu(new Menu("Q Setting", "Q Setting"));
                {
                    var qDelayMenu = qMenu.AddSubMenu(new Menu("Delay Settings", "Delay Settings"));
                    {
                        qDelayMenu.AddItem(new MenuItem("Q1Delay", "Q1 Delay: ", true).SetValue(new Slider(280, 200, 350)));
                        qDelayMenu.AddItem(new MenuItem("Q2Delay", "Q2 Delay: ", true).SetValue(new Slider(280, 200, 350)));
                        qDelayMenu.AddItem(new MenuItem("Q3Delay", "Q3 Delay: ", true).SetValue(new Slider(380, 300, 450)));
                        qDelayMenu.AddItem(new MenuItem("AutoSetDelay", "Inlcude the Ping?", true).SetValue(false)).ValueChanged +=
                            DelayChanged;
                        qDelayMenu.AddItem(new MenuItem("MinDelay", "Set my Min QA Delay?", true).SetValue(false)).ValueChanged +=
                            myDelayChanged;
                    }

                    qMenu.AddItem(new MenuItem("KeepQALive", "Keep Q alive", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("Dance", "Dance Emote in QA", true).SetValue(false));
                    qMenu.AddItem(
                        new MenuItem("QMode", "Q Mode : ", true).SetValue(
                            new StringList(new[] { "Target Position", "Mouse", "Max Q To Target", "Max Q To Mouse" })));
                }

                var wMenu = miscMenu.AddSubMenu(new Menu("W Setting", "W Setting"));
                {
                    wMenu.AddItem(new MenuItem("AntiGapCloserW", "AntiGapCloser", true).SetValue(true));
                    wMenu.AddItem(new MenuItem("InterruptTargetW", "Interrupt Danger Spell", true).SetValue(true));
                }

                var eMenu = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    foreach (var target in HeroManager.Enemies)
                    {
                        if (target.ChampionName == "Darius")
                        {
                            var spellMenu = eMenu.AddSubMenu(new Menu("Darius", "Darius Spell"));
                            {
                                spellMenu.AddItem(new MenuItem("EDodgeDariusR", "Shield R", true).SetValue(true));
                            }
                        }

                        if (target.ChampionName == "Garen")
                        {
                            var spellMenu = eMenu.AddSubMenu(new Menu("Garen", "Garen Spell"));
                            {
                                spellMenu.AddItem(new MenuItem("EDodgeGarenQ", "Shield Q", true).SetValue(true));
                                spellMenu.AddItem(new MenuItem("EDodgeGarenR", "Shield R", true).SetValue(true));
                            }
                        }

                        if (target.ChampionName == "Irelia")
                        {
                            var spellMenu = eMenu.AddSubMenu(new Menu("Irelia", "Irelia Spell"));
                            {
                                spellMenu.AddItem(new MenuItem("EDodgeIreliaE", "Shield E", true).SetValue(true));
                            }
                        }

                        if (target.ChampionName == "LeeSin")
                        {
                            var spellMenu = eMenu.AddSubMenu(new Menu("LeeSin", "LeeSin Spell"));
                            {
                                spellMenu.AddItem(new MenuItem("EDodgeLeeSinR", "Shield R", true).SetValue(true));
                            }
                        }

                        if (target.ChampionName == "Olaf")
                        {
                            var spellMenu = eMenu.AddSubMenu(new Menu("Olaf", "Olaf Spell"));
                            {
                                spellMenu.AddItem(new MenuItem("EDodgeOlafE", "Shield E", true).SetValue(true));
                            }
                        }

                        if (target.ChampionName == "Pantheon")
                        {
                            var spellMenu = eMenu.AddSubMenu(new Menu("Pantheon", "Pantheon Spell"));
                            {
                                spellMenu.AddItem(new MenuItem("EDodgePantheonW", "Shield W", true).SetValue(true));
                            }
                        }

                        if (target.ChampionName == "Renekton")
                        {
                            var spellMenu = eMenu.AddSubMenu(new Menu("Renekton", "RenektonW Spell"));
                            {
                                spellMenu.AddItem(new MenuItem("EDodgeRenektonW", "Shield W", true).SetValue(true));
                            }
                        }

                        if (target.ChampionName == "Rengar")
                        {
                            var spellMenu = eMenu.AddSubMenu(new Menu("Rengar", "Rengar Spell"));
                            {
                                spellMenu.AddItem(new MenuItem("EDodgeRengarQ", "Shield Q", true).SetValue(true));
                            }
                        }

                        if (target.ChampionName == "Veigar")
                        {
                            var spellMenu = eMenu.AddSubMenu(new Menu("Veigar", "Veigar Spell"));
                            {
                                spellMenu.AddItem(new MenuItem("EDodgeVeigarR", "Shield R", true).SetValue(true));
                            }
                        }

                        if (target.ChampionName == "Volibear")
                        {
                            var spellMenu = eMenu.AddSubMenu(new Menu("Volibear", "Volibear Spell"));
                            {
                                spellMenu.AddItem(new MenuItem("EDodgeVolibearW", "Shield W", true).SetValue(true));
                            }
                        }

                        if (target.ChampionName == "XenZhao")
                        {
                            var spellMenu = eMenu.AddSubMenu(new Menu("XenZhao", "XenZhao Spell"));
                            {
                                spellMenu.AddItem(new MenuItem("EDodgeXenZhaoQ3", "Shield Q3", true).SetValue(true));
                            }
                        }

                        if (target.ChampionName == "TwistedFate")
                        {
                            var spellMenu = eMenu.AddSubMenu(new Menu("Twisted Fate", "TwistedFate Spell"));
                            {
                                spellMenu.AddItem(new MenuItem("EDodgeTwistedFateW", "Shield W", true).SetValue(true));
                            }
                        }
                    }

                    eMenu.AddItem(new MenuItem("EShielddogde", "Use E Shield Spell", true).SetValue(true));
                }

                var skinMenu = miscMenu.AddSubMenu(new Menu("SkinChance", "SkinChance"));
                {
                    SkinManager.AddToMenu(skinMenu, 7);
                }
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawBurst", "Draw Burst Range", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("DrawRStatus", "Draw R Status", true).SetValue(true));
                //DamageIndicator.AddToMenu(drawMenu, DamageCalculate.GetComboDamage);
            }

            Menu.AddItem(new MenuItem("asdvre1w56", "  "));
            Menu.AddItem(new MenuItem("Credit", "Credit : NightMoon")).SetFontStyle(
                    System.Drawing.FontStyle.Regular, menuColor);
            Menu.AddItem(new MenuItem("Version", "Version : 2.0.0.0")).SetFontStyle(
                    System.Drawing.FontStyle.Regular, menuColor);

            Menu.AddToMainMenu();

            if (!Menu.Item("AutoSetDelay", true).GetValue<bool>())
            {
                Menu.Item("Q1Delay", true).SetValue(new Slider(280, 200, 350));
                Menu.Item("Q2Delay", true).SetValue(new Slider(280, 200, 350));
                Menu.Item("Q3Delay", true).SetValue(new Slider(380, 300, 450));
            }

            if (Menu.Item("AutoSetDelay", true).GetValue<bool>())
            {
                Menu.Item("MinDelay", true).SetValue(false);
            }
        }

        private static void DelayChanged(object obj, OnValueChangeEventArgs Args)
        {
            if (!Args.GetNewValue<bool>())
            {
                Menu.Item("Q1Delay", true).SetValue(new Slider(280, 200, 350));
                Menu.Item("Q2Delay", true).SetValue(new Slider(280, 200, 350));
                Menu.Item("Q3Delay", true).SetValue(new Slider(380, 300, 450));
            }
        }

        private static void myDelayChanged(object obj, OnValueChangeEventArgs Args)
        {
            if (Args.GetNewValue<bool>())
            {
                Menu.Item("Q1Delay", true).SetValue(new Slider(250, 200, 350));
                Menu.Item("Q2Delay", true).SetValue(new Slider(250, 200, 350));
                Menu.Item("Q3Delay", true).SetValue(new Slider(350, 300, 450));
            }
        }
    }
}
