using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Fiora.Manager.Menu
{
    using System.Linq;
    using Evade;
    using LeagueSharp.Common;

    internal class MenuManager : Logic
    {
        internal static void Init()
        {
            Menu = new Menu("Flowers' Fiora", "Flowers_Fiora", true);

            var orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            {
                Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            }

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboR", "Use R", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRSolo", "Use R| Solo R", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboRTeam", "Use R| Team Fight", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboPassive", "Forcus Attack Passive", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboYoumuu", "Use Youmuu", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboTiamat", "Use Tiamat", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboHydra", "Use Hydra", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboIgnite", "Use Ignite", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassPassive", "Forcus Attack Passive", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassTiamat", "Use Tiamat", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassHydra", "Use Hydra", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var laneclearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                laneclearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                laneclearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                laneclearMenu.AddItem(
                    new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var jungleclearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                jungleclearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                jungleclearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
                jungleclearMenu.AddItem(
                    new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                fleeMenu.AddItem(new MenuItem("FleeQ", "Use Q", true).SetValue(true));
                fleeMenu.AddItem(new MenuItem("FleeKey", "Flee Key", true).SetValue(new KeyBind('Z', KeyBindType.Press)));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var qSettings = miscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
                {
                    qSettings.AddItem(new MenuItem("QUnder", "Dont Cast Q If UnderTurret", true).SetValue(true));
                    qSettings.AddItem(new MenuItem("KillStealQ", "Use Q KillSteal", true).SetValue(true));
                }

                var wSettings = miscMenu.AddSubMenu(new Menu("W Settings", "W Settings"));
                {
                    wSettings.AddItem(new MenuItem("KillStealW", "Use W KillSteal", true).SetValue(true));
                }

                var evadeSettings = miscMenu.AddSubMenu(new Menu("Evade Settings", "Evade Settings"));
                {
                    var evadespellSettings = evadeSettings.AddSubMenu(new Menu("Dodge Spells", "Dodge Spells"));
                    {
                        var evadeSpells = evadespellSettings.AddSubMenu(new Menu("Evade spells", "evadeSpells"));
                        {
                            foreach (var spell in EvadeSpellDatabase.Spells)
                            {
                                var subMenu = evadeSpells.AddSubMenu(new Menu("Fiora " + spell.Slot, spell.Name));
                                {
                                    subMenu.AddItem(
                                        new MenuItem("DangerLevel" + spell.Name, "Danger level", true).SetValue(
                                            new Slider(spell.DangerLevel, 5, 1)));

                                    subMenu.AddItem(new MenuItem("Tower" + spell.Name, "Under Tower", true).SetValue(false));

                                    subMenu.AddItem(new MenuItem("Enabled" + spell.Name, "Enabled", true).SetValue(true));
                                }
                            }
                        }

                        var skillShotMenu = evadespellSettings.AddSubMenu(new Menu("Skillshots", "Skillshots"));
                        {
                            foreach (
                                var hero in
                                HeroManager.Enemies.Where(
                                    i => SpellDatabase.Spells.Any(a => a.ChampionName == i.ChampionName)))
                            {
                                skillShotMenu.AddSubMenu(new Menu(hero.ChampionName, "Evade" + hero.ChampionName.ToLower()));
                            }

                            foreach (
                                var spell in
                                SpellDatabase.Spells.Where(
                                    i => HeroManager.Enemies.Any(a => a.ChampionName == i.ChampionName)))
                            {
                                var subMenu =
                                    skillShotMenu.SubMenu("Evade" + spell.ChampionName.ToLower())
                                        .AddSubMenu(new Menu(spell.SpellName + " " + spell.Slot,
                                            "EvadeSpell" + spell.MenuItemName));
                                {
                                    subMenu.AddItem(
                                        new MenuItem("DangerLevel" + spell.MenuItemName, "Danger Level", true).SetValue(
                                            new Slider(spell.DangerValue, 1, 5)));
                                    subMenu.AddItem(
                                        new MenuItem("Enabled" + spell.MenuItemName, "Enabled", true).SetValue(
                                            !spell.DisabledByDefault));
                                }
                            }
                        }
                    }

                    var wdodgeMenu = evadeSettings.AddSubMenu(new Menu("Others Spell", "Others Spell"));
                    {
                        WDodgeSpell.Init(wdodgeMenu);
                    }

                    var evadeMenu = evadeSettings.AddSubMenu(new Menu("Evade Target", "EvadeTarget"));
                    {
                        evadeMenu.AddItem(new MenuItem("EvadeTargetW", "Use W", true).SetValue(true));
                        evadeMenu.AddItem(new MenuItem("EvadeTargetTower", "-> Under Tower", true).SetValue(false));
                        evadeMenu.AddItem(new MenuItem("BAttack", "Basic Attack", true).SetValue(true));
                        evadeMenu.AddItem(new MenuItem("BAttackHpU", "-> If Hp <", true).SetValue(new Slider(35)));
                        evadeMenu.AddItem(new MenuItem("CAttack", "Crit Attack", true).SetValue(true));
                        evadeMenu.AddItem(new MenuItem("CAttackHpU", "-> If Hp <", true).SetValue(new Slider(40)));

                        foreach (var hero in
                            HeroManager.Enemies.Where(i => EvadeTargetManager.Spells.Any(a => a.ChampionName == i.ChampionName)))
                        {
                            evadeMenu.AddSubMenu(new Menu("-> " + hero.ChampionName, "ET_" + hero.ChampionName));
                        }

                        foreach (
                            var spell in
                            EvadeTargetManager.Spells.Where(
                                i => HeroManager.Enemies.Any(a => a.ChampionName == i.ChampionName)))
                        {
                            evadeMenu.SubMenu("ET_" + spell.ChampionName)
                                .AddItem(
                                    new MenuItem(spell.MissileName, spell.MissileName + " (" + spell.Slot + ")", true)
                                        .SetValue(false));
                        }
                    }
                }

                var skinMenu = miscMenu.AddSubMenu(new Menu("SkinChange", "SkinChange"));
                {
                    skinMenu.AddItem(new MenuItem("EnableSkin", "Enabled", true).SetValue(false)).ValueChanged += EnbaleSkin;
                    skinMenu.AddItem(
                        new MenuItem("SelectSkin", "Select Skin: ", true).SetValue(
                            new StringList(new[]
                            {
                                "Classic", "Royal Guard", "Nightraven", "Headmistress", "PROJECT: Fiora", "Pool Party"
                            })));
                }
            }

            var drawingMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawingMenu.AddItem(new MenuItem("DrawingQ", "Draw Q Range", true).SetValue(false));
                drawingMenu.AddItem(new MenuItem("DrawingW", "Draw W Range", true).SetValue(false));
                drawingMenu.AddItem(new MenuItem("DrawingR", "Draw R Range", true).SetValue(false));
                drawingMenu.AddItem(new MenuItem("DrawingDamage", "Draw Combo Damage", true).SetValue(true));
            }

            Menu.AddToMainMenu();
        }
    }
}