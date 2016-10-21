using System;
using System.Drawing;

using DZLib.MenuExtensions;

using LeagueSharp.Common;

using Color = SharpDX.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iLucian.MenuHelper
{

    using LeagueSharp;

    class MenuGenerator
    {
        public static void Generate()
        {
            Variables.Menu = new Menu("iLucian", "com.ilucian", true).SetFontStyle(FontStyle.Bold, Color.DeepPink);
            var rootMenu = Variables.Menu;

            var owMenu = new Menu(":: iLucian - Orbwalker", "com.ilucian.orbwalker");
            {
                Variables.Orbwalker = new Orbwalking.Orbwalker(owMenu);
                rootMenu.AddSubMenu(owMenu);
            }

            var comboOptions =
                new Menu(":: iLucian - Combo Options", "com.ilucian.combo").SetFontStyle(FontStyle.Regular, Color.Aqua);
            {
                comboOptions.AddBool("com.ilucian.combo.q", "Use Q", true);
                comboOptions.AddBool("com.ilucian.combo.qExtended", "Use Extended Q", true);
                comboOptions.AddBool("com.ilucian.combo.w", "Use W", true);
                comboOptions.AddBool("com.ilucian.combo.e", "Use E", true);
                comboOptions.AddBool("com.ilucian.combo.startE", "Start Combo With E", true);
                comboOptions.AddKeybind(
                    "com.ilucian.combo.forceR", 
                    "Semi Ult Key", 
                    new Tuple<uint, KeyBindType>("T".ToCharArray()[0], KeyBindType.Press));
                comboOptions.AddSlider("com.ilucian.combo.eRange", "E Dash Range", 65, 50, 475);
                comboOptions.AddStringList(
                    "com.ilucian.combo.eMode", 
                    "E Mode", 
                    new[] { "Kite", "Side", "Cursor", "Enemy", "Fast Mode", "VHR Logic - Smart E" }, 
                    5);
                rootMenu.AddSubMenu(comboOptions);
            }

            var harassOptions = new Menu(":: iLucian - Harass Options", "com.ilucian.harass");
            {
                var autoHarassMenu = new Menu("Auto Harass", "com.ilucian.harass.auto");
                {
                    autoHarassMenu.AddKeybind(
                        "com.ilucian.harass.auto.autoharass", 
                        "Enabled", 
                        new Tuple<uint, KeyBindType>("Z".ToCharArray()[0], KeyBindType.Toggle))
                        .Permashow(true, "iLucian | Auto Harass", Color.Aqua);
                    autoHarassMenu.AddBool("com.ilucian.harass.auto.q", "Use Q", true);
                    autoHarassMenu.AddBool("com.ilucian.harass.auto.qExtended", "Use Extended Q", true);
                    autoHarassMenu.AddSlider(
                        "com.ilucian.harass.auto.autoharass.mana", 
                        "Min Mana % for auto harasss", 
                        70, 
                        0, 
                        100);
                }

                harassOptions.AddBool("com.ilucian.harass.q", "Use Q", true);
                harassOptions.AddBool("com.ilucian.harass.qExtended", "Use Extended Q", true);
                var harassWhitelist = new Menu("Extended Q Whitelist", "com.ilucian.harass.whitelist");
                {
                    foreach (var hero in HeroManager.Enemies)
                    {
                        harassWhitelist.AddBool(
                            "com.ilucian.harass.whitelist." + hero.ChampionName.ToLower(), 
                            "Don't Q: " + hero.ChampionName);
                    }
                }

                harassOptions.AddBool("com.ilucian.harass.w", "Use W", true);
                harassOptions.AddSlider("com.ilucian.harass.mana", "Min Mana % for harass", 70, 0, 100);
                harassOptions.AddSubMenu(harassWhitelist);
                harassOptions.AddSubMenu(autoHarassMenu);
                rootMenu.AddSubMenu(harassOptions);
            }

            var laneclearOptions = new Menu(":: iLucian - Laneclear Options", "com.ilucian.laneclear");
            {
                laneclearOptions.AddBool("com.ilucian.laneclear.q", "Use Q", true);
                laneclearOptions.AddSlider("com.ilucian.laneclear.qMinions", "Cast Q on x minions", 3, 1, 10);
                laneclearOptions.AddSlider("com.ilucian.laneclear.mana", "Min Mana % for laneclear", 70, 0, 100);
                rootMenu.AddSubMenu(laneclearOptions);
            }

            var jungleclearOptions = new Menu(":: iLucian - Jungleclear Options", "com.ilucian.jungleclear");
            {
                jungleclearOptions.AddBool("com.ilucian.jungleclear.q", "Use Q", true);
                jungleclearOptions.AddBool("com.ilucian.jungleclear.w", "Use W", true);
                jungleclearOptions.AddBool("com.ilucian.jungleclear.e", "Use E", true);
                jungleclearOptions.AddSlider("com.ilucian.jungleclear.mana", "Min Mana % for jungleclear", 70, 0, 100);
                rootMenu.AddSubMenu(jungleclearOptions);
            }

            var gapcloserMenu = new Menu(":: iLucian - Gapcloser Options", "com.ilucian.antigap");
            {
                CustomizableAntiGapcloser.AddToMenu(gapcloserMenu);
                rootMenu.AddSubMenu(gapcloserMenu);
            }

            var miscOptions = new Menu(":: iLucian - Misc Options", "com.ilucian.misc");
            {
                miscOptions.AddBool("com.ilucian.misc.antiVayne", "Anti Vayne Condemn", true);
                miscOptions.AddBool("com.ilucian.misc.antiMelee", "Anti Melee (E)", true);
                miscOptions.AddBool("com.ilucian.misc.usePrediction", "Use W Pred", true);
                miscOptions.AddBool("com.ilucian.misc.forcePassive", "Force Passive Target", true);
                miscOptions.AddBool("com.ilucian.misc.gapcloser", "Use E For Gapcloser", true);
                miscOptions.AddBool("com.ilucian.misc.eqKs", "EQ - Killsteal", true);
                miscOptions.AddBool("com.ilucian.misc.useChampions", "Use EQ on Champions", true);
                miscOptions.AddBool("com.ilucian.misc.extendChamps", "Use Ext Q on Champions", true);
                miscOptions.AddBool("com.ilucian.misc.drawQ", "Draw Ext Q Range", true);
                var dmgAfterComboItem =
                    new MenuItem("com.ilucian.misc.drawDamage", "Draw Damage After Combo").SetValue(true);
                {
                    LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = iLucian.GetComboDamage;
                    LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
                    dmgAfterComboItem.ValueChanged +=
                        delegate(object sender, OnValueChangeEventArgs eventArgs)
                            {
                                LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                            };
                    miscOptions.AddItem(dmgAfterComboItem);
                }

                rootMenu.AddSubMenu(miscOptions);
            }

            rootMenu.AddToMainMenu();
        }
    }
}