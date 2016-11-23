using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Yasuo.Manager.Menu
{
    using Evade;
    using SharpDX;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Orbwalking = Orbwalking;

    internal class MenuManager : Logic
    {
        internal static void Init()
        {
            Menu = new Menu("Flowers' Yasuo", "Flowers' Yasuo", true);

            var orbMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            {
                Orbwalker = new Orbwalking.Orbwalker(orbMenu);
            }

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboQStack", "Use Q| Stack Q(When Dashing)", true).SetValue(
                        new StringList(new[] {"Both", "Only Heros", "Only Minion", "Off"}, 3)));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboETurret", "Use E Under turret", true).SetValue(false));
                comboMenu.AddItem(new MenuItem("ComboEGapcloser", "Use E Gapcloser", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboEMode", "Use E Gapcloser Mode: ", true).SetValue(
                        new StringList(new[] {"Target", "Mouse"})));
                comboMenu.AddItem(
                    new MenuItem("ComboEGap", "Use E GapCloser| Target Distance to Player >=x", true).SetValue(
                        new Slider(250, 0, 1300)));
                comboMenu.AddItem(new MenuItem("ComboEQ", "Use EQ", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboEQ3", "Use EQ3", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboR", "Use R", true).SetValue(new KeyBind('R', KeyBindType.Toggle, true)));
                comboMenu.AddItem(
                    new MenuItem("ComboRHp", "Use R|When target HealthPercent <= x%", true).SetValue(new Slider(50)));
                comboMenu.AddItem(
                    new MenuItem("ComboRCount", "Use R|When knockedUp enemy Count >= x", true).SetValue(
                        new Slider(2, 1, 5)));
                comboMenu.AddItem(
                    new MenuItem("ComboRAlly", "Use R| When Have Ally In Range", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboEQFlash", "Use EQ Flash?", true).SetValue(new KeyBind('E', KeyBindType.Toggle)));
                comboMenu.AddItem(
                    new MenuItem("ComboEQFlashSolo", "Use EQ Flash|Solo Mode", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboEQFlashTeam", "Use EQ Flash|Team Fight", true).SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("ComboEQFlashTeamCount", "Use EQ Flash|Hit Count >= x", true).SetValue(
                        new Slider(3, 1, 5)));
                comboMenu.AddItem(
                    new MenuItem("ComboEQFlashTeamAlly", "Use EQ Flash|Ally Count >= x", true).SetValue(
                        new Slider(2, 0, 5)));
                comboMenu.AddItem(new MenuItem("ComboIgnite", "Use Ignite", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboItems", "Use Items", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassQ3", "Use Q3", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(false));
                harassMenu.AddItem(new MenuItem("HarassTower", "Use E Under Tower", true).SetValue(true));
            }

            var laneClearMenu = Menu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                laneClearMenu.AddItem(new MenuItem("LaneClearQ3", "Use Q3", true).SetValue(true));
                laneClearMenu.AddItem(
                    new MenuItem("LaneClearQ3count", "Use Q3| Hit Minions >= x", true).SetValue(new Slider(3, 1, 5)));
                laneClearMenu.AddItem(new MenuItem("LaneClearE", "Use E", true).SetValue(true));
                laneClearMenu.AddItem(new MenuItem("LaneClearETurret", "Use E Under Turret", true).SetValue(false));
                laneClearMenu.AddItem(new MenuItem("LaneClearItems", "Use Items", true).SetValue(true));
            }

            var jungleClearMenu = Menu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                jungleClearMenu.AddItem(new MenuItem("JungleClearQ3", "Use Q3", true).SetValue(true));
                jungleClearMenu.AddItem(new MenuItem("JungleClearE", "Use E", true).SetValue(true));
            }

            var lastHitMenu = Menu.AddSubMenu(new Menu("LastHit", "LastHit"));
            {
                lastHitMenu.AddItem(new MenuItem("LastHitQ", "Use Q", true).SetValue(true));
                lastHitMenu.AddItem(new MenuItem("LastHitQ3", "Use Q3", true).SetValue(true));
                lastHitMenu.AddItem(new MenuItem("LastHitE", "Use E", true).SetValue(true));
                lastHitMenu.AddItem(new MenuItem("LastHitETurret", "Use E Under Turret", true).SetValue(false));
            }

            var fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            {
                fleeMenu.AddItem(new MenuItem("FleeQ", "Use Q", true).SetValue(true));
                fleeMenu.AddItem(new MenuItem("FleeQ3", "Use Q3", true).SetValue(true));
                fleeMenu.AddItem(new MenuItem("FleeE", "Use E", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var qMenu = miscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
                {
                    qMenu.AddItem(new MenuItem("KillStealQ", "Use Q KillSteal", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("KillStealQ3", "Use Q3 KillSteal", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("Q3Int", "Use Q3 Interrupter", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("Q3Anti", "Use Q3 AntiGapcloser", true).SetValue(true));
                    qMenu.AddItem(
                        new MenuItem("StackQ", "Stack Q", true).SetValue(new KeyBind('T', KeyBindType.Toggle, true)));
                    qMenu.AddItem(
                        new MenuItem("AutoQ", "Auto Q Harass Enemy", true).SetValue(new KeyBind('N', KeyBindType.Toggle, true)));
                    qMenu.AddItem(
                        new MenuItem("AutoQ3", "Auto Q3 Harass Enemy", true).SetValue(false));
                }

                var eMenu = miscMenu.AddSubMenu(new Menu("E Settings", "E Settings"));
                {
                    eMenu.AddItem(new MenuItem("KillStealE", "Use E KillSteal", true).SetValue(true));
                }

                var rMenu = miscMenu.AddSubMenu(new Menu("R Settings", "R Settings"));
                {
                    var rWhitelist = rMenu.AddSubMenu(new Menu("R Whitelist", "R Whitelist"));
                    {
                        foreach (var hero in HeroManager.Enemies)
                        {
                            rWhitelist.AddItem(
                                new MenuItem("R" + hero.ChampionName.ToLower(), hero.ChampionName, true).SetValue(true));
                        }
                    }

                    var autoR = rMenu.AddSubMenu(new Menu("Auto R", "Auto R"));
                    {
                        autoR.AddItem(new MenuItem("AutoR", "Auto R", true)).SetValue(true);
                        autoR.AddItem(
                            new MenuItem("AutoRCount", "Auto R|When knockedUp enemy Count >= x", true).SetValue(
                                new Slider(3, 1, 5)));
                        autoR.AddItem(
                            new MenuItem("AutoRRangeCount", "Auto R|When all Enemy Count >= x", true).SetValue(
                                new Slider(2, 1, 5)));
                        autoR.AddItem(
                            new MenuItem("AutoRMyHp", "Auto R|When Player HealthPercent >= x%", true).SetValue(
                                new Slider(50)));
                    }
                }

                var evadeSettings = miscMenu.AddSubMenu(new Menu("Evade Settings", "Evade Settings"));
                {
                    var evadespellSettings = evadeSettings.AddSubMenu(new Menu("Dodge Spells", "Dodge Spells"));
                    {
                        var evadeSpells = evadespellSettings.AddSubMenu(new Menu("Evade spells", "evadeSpells"));
                        {
                            foreach (var spell in EvadeSpellDatabase.Spells)
                            {
                                var subMenu = evadeSpells.AddSubMenu(new Menu("Yasuo " + spell.Slot, spell.Name));
                                {
                                    subMenu.AddItem(
                                        new MenuItem("DangerLevel" + spell.Name, "Danger level", true).SetValue(
                                            new Slider(spell.DangerLevel, 5, 1)));

                                    if (spell.Slot == SpellSlot.E)
                                    {
                                        subMenu.AddItem(new MenuItem("ETower", "Under Tower", true).SetValue(false));
                                    }

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

                    var evadeMenu = evadeSettings.AddSubMenu(new Menu("Evade Target", "EvadeTarget"));
                    {
                        evadeMenu.AddItem(new MenuItem("EvadeTargetW", "Use W", true).SetValue(true));
                        evadeMenu.AddItem(new MenuItem("EvadeTargetE", "Use E (To Dash Behind WindWall)", true).SetValue(true));
                        evadeMenu.AddItem(new MenuItem("EvadeTargetETower", "-> Under Tower", true).SetValue(false));
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
                                "Classic", "High Noon", "Project: Yasuo", "Blood Moon", "Others", "Others1", "Others2",
                                "Others3", "Others4"
                            })));
                }

                var autoWardMenu = miscMenu.AddSubMenu(new Menu("Auto Ward", "Auto Ward"));
                {
                    autoWardMenu.AddItem(new MenuItem("AutoWardEnable", "Enabled", true).SetValue(true));
                    autoWardMenu.AddItem(new MenuItem("OnlyCombo", "Only Combo Mode Active", true).SetValue(true));
                }

                miscMenu.AddItem(new MenuItem("EQFlash", "EQFlash", true).SetValue(new KeyBind('A', KeyBindType.Press)));

            }

            var drawMenu = Menu.AddSubMenu(new Menu("Draw", "Draw"));
            {
                drawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("DrawQ3", "Draw Q3 Range", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("DrawW", "Draw W Range", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("DrawSpots", "Draw WallJump Spots", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("DrawStackQ", "Draw Stack Q Status", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("DrawAutoQ", "Draw Auto Q Status", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("DrawComboEQStatus", "Draw Combo EQ Flash Status", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("DrawRStatus", "Draw Combo R Status", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("DrawStackQPerma", "Draw Stack Q PermaShow", true).SetValue(true))
                    .ValueChanged += StackQChanged;
                drawMenu.AddItem(new MenuItem("DrawAutoQPerma", "Draw Auto Q PermaShow", true).SetValue(true))
                    .ValueChanged += AutoQChanged;
                drawMenu.AddItem(new MenuItem("DrawComboEQPerma", "Draw Combo EQFlash PermaShow", true).SetValue(true))
                    .ValueChanged += ComboEQFlashChanged;
                drawMenu.AddItem(new MenuItem("DrawRStatusPerma", "Draw Combo R PermaShow", true).SetValue(true))
                    .ValueChanged += ComboRChanged;
            }

            Menu.AddItem(new MenuItem("asd ad asd ", " ", true));
            Menu.AddItem(new MenuItem("Credit", "Credit: NightMoon", true));

            Menu.AddToMainMenu();

            if (Menu.Item("DrawStackQPerma", true).GetValue<bool>())
            {
                Menu.Item("StackQ", true).Permashow(true, "Stack Q Active", Color.MediumSlateBlue);
            }

            if (Menu.Item("DrawAutoQPerma", true).GetValue<bool>())
            {
                Menu.Item("AutoQ", true).Permashow(true, "Auto Q Active", Color.Orange);
            }

            if (Menu.Item("DrawComboEQPerma", true).GetValue<bool>())
            {
                Menu.Item("ComboEQFlash", true).Permashow(true, "Combo EQFlash Active", Color.Pink);
            }

            if (Menu.Item("DrawRStatusPerma", true).GetValue<bool>())
            {
                Menu.Item("ComboR", true).Permashow(true, "Combo R Active", Color.PowderBlue);
            }
        }

        private static void ComboEQFlashChanged(object obj, OnValueChangeEventArgs Args)
        {
            Menu.Item("ComboEQFlash", true).Permashow(Args.GetNewValue<bool>(), "Combo EQFlash Active", Color.Pink);
        }

        private static void ComboRChanged(object obj, OnValueChangeEventArgs Args)
        {
            Menu.Item("ComboR", true).Permashow(Args.GetNewValue<bool>(), "Combo R Active", Color.PowderBlue);
        }

        private static void AutoQChanged(object obj, OnValueChangeEventArgs Args)
        {
            Menu.Item("AutoQ", true).Permashow(Args.GetNewValue<bool>(), "Auto Q Active", Color.Orange);
        }

        private static void StackQChanged(object obj, OnValueChangeEventArgs Args)
        {
            Menu.Item("StackQ", true).Permashow(Args.GetNewValue<bool>(), "Stack Q Active", Color.MediumSlateBlue);
        }
    }
}