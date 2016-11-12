#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Champions;
using Marksman.Common;
using Marksman.Utils;
using SharpDX;
using Color = System.Drawing.Color;


#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Marksman
{
    internal class Program
    {
        public static Menu Config, MenuLane, MenuJungle;

        public static Menu OrbWalking;

        public static Menu MenuActivator;

        public static Marksman.Champions.Champion ChampionClass;

        private static SpellSlot igniteSlot;

        //public static Utils.EarlyEvade EarlyEvade;

        public static Spell Smite;

        public static SpellSlot SmiteSlot = SpellSlot.Unknown;

        private static readonly int[] SmitePurple = {3713, 3726, 3725, 3726, 3723};

        private static readonly int[] SmiteGrey = {3711, 3722, 3721, 3720, 3719};

        private static readonly int[] SmiteRed = {3715, 3718, 3717, 3716, 3714};

        private static readonly int[] SmiteBlue = {3706, 3710, 3709, 3708, 3707};

        public static Menu MenuExtraTools { get; set; }
        public static Menu MenuExtraToolsActivePackets { get; set; }

        private static string Smitetype
        {
            get
            {
                if (SmiteBlue.Any(i => Items.HasItem(i)))
                    return "s5_summonersmiteplayerganker";

                if (SmiteRed.Any(i => Items.HasItem(i)))
                    return "s5_summonersmiteduel";

                if (SmiteGrey.Any(i => Items.HasItem(i)))
                    return "s5_summonersmitequick";

                if (SmitePurple.Any(i => Items.HasItem(i)))
                    return "itemsmiteaoe";

                return "summonersmite";
            }
        }

        public static void Game_OnGameLoad()
        {
            Config = new Menu("Marksman", "Marksman", true).SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow);
            ChampionClass = new Marksman.Champions.Champion();
            CommonGeometry.Init();
            var baseType = ChampionClass.GetType();

            igniteSlot = ObjectManager.Player.GetSpellSlot("summonerdot");

            /* Update this with Activator.CreateInstance or Invoke
               http://stackoverflow.com/questions/801070/dynamically-invoking-any-function-by-passing-function-name-as-string 
               For now stays cancer.
             */
            var championName = ObjectManager.Player.ChampionName.ToLowerInvariant();

            switch (championName)
            {
                case "ashe":
                    ChampionClass = new Ashe();
                    break;
                case "caitlyn":
                    ChampionClass = new Caitlyn();
                    break;
                case "corki":
                    ChampionClass = new Corki();
                    break;
                case "draven":
                    ChampionClass = new Draven();
                    break;
                case "ezreal":
                    ChampionClass = new Ezreal();
                    break;
                case "graves":
                    ChampionClass = new Graves();
                    break;
                case "gnar":
                    ChampionClass = new Marksman.Champions.Gnar();
                    break;
                case "jhin":
                    ChampionClass = new Marksman.Champions.Jhin();
                    break;
                case "jinx":
                    ChampionClass = new Jinx();
                    break;
                case "kalista":
                    ChampionClass = new Kalista();
                    break;
                case "kindred":
                    ChampionClass = new Kindred();
                    break;
                case "kogmaw":
                    ChampionClass = new Kogmaw();
                    break;
                case "lucian":
                    ChampionClass = new Lucian();
                    break;
                case "missfortune":
                    ChampionClass = new MissFortune();
                    break;
                case "quinn":
                    ChampionClass = new Quinn();
                    break;
                case "sivir":
                    ChampionClass = new Sivir();
                    break;
                case "teemo":
                    ChampionClass = new Teemo();
                    break;
                case "tristana":
                    ChampionClass = new Tristana();
                    break;
                case "twitch":
                    ChampionClass = new Twitch();
                    break;
                case "urgot":
                    ChampionClass = new Urgot();
                    break;
                case "vayne":
                    ChampionClass = new Vayne();
                    break;
                case "varus":
                    ChampionClass = new Varus();
                    break;
                default:
                    Chat.Print(ObjectManager.Player.CharData.BaseSkinName + " Doesn't support from Marksman!");
                    break;
            }
            //Config.DisplayName = "Marksman Lite | " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(championName);
            Config.DisplayName = "Marksman II - " + ObjectManager.Player.ChampionName;

            ChampionClass.Id = ObjectManager.Player.CharData.BaseSkinName;
            ChampionClass.Config = Config;
            ChampionClass.MenuLane = MenuLane;
            ChampionClass.MenuJungle = MenuJungle;

            MenuExtraTools = new Menu("Marksman II - Tools", "ExtraTools", true).SetFontStyle(FontStyle.Regular,
                SharpDX.Color.GreenYellow);
            {
                var nMenuExtraToolsPackets = new Menu("Available Tools", "MenuExtraTools.Available");
                nMenuExtraToolsPackets.AddItem(new MenuItem("ExtraTools.Orbwalker", "Orbwalker:")).SetValue(new StringList(new[] {"LeagueSharp Common", "Marksman Orbwalker (With Attack Speed Limiter)"})).SetFontStyle(FontStyle.Regular, SharpDX.Color.Gray);
                nMenuExtraToolsPackets.AddItem(new MenuItem("ExtraTools.Prediction", "Prediction:")).SetValue(new StringList(new[] {"LeagueSharp Common", "SPrediction (Synx)"})).SetFontStyle(FontStyle.Regular, SharpDX.Color.Gray);
                nMenuExtraToolsPackets.AddItem(new MenuItem("ExtraTools.AutoLevel", "Auto Leveller:")).SetValue(true);
                nMenuExtraToolsPackets.AddItem(new MenuItem("ExtraTools.AutoBush", "Auto Bush Ward:")).SetValue(true);
                nMenuExtraToolsPackets.AddItem(new MenuItem("ExtraTools.AutoPink", "Auto Pink Ward:")).SetValue(true).SetTooltip("For rengar / vayne / shaco etc.");
                nMenuExtraToolsPackets.AddItem(new MenuItem("ExtraTools.WarningSpells", "Warning Spells [NEW]:")).SetValue(true).SetTooltip("For Rengar R / Shaco Q etc.").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow);
                nMenuExtraToolsPackets.AddItem(new MenuItem("ExtraTools.Skin", "Skin Manager:")).SetValue(true);
                nMenuExtraToolsPackets.AddItem(new MenuItem("ExtraTools.Emote", "Emote:")).SetValue(true);
                nMenuExtraToolsPackets.AddItem(new MenuItem("ExtraTools.AllySupport", "Ally Support:")).SetValue(true);
                nMenuExtraToolsPackets.AddItem(new MenuItem("ExtraTools.BuffTimer", "Buff Time Manager:")).SetValue(false).SetFontStyle(FontStyle.Regular, SharpDX.Color.Gray);
                nMenuExtraToolsPackets.AddItem(new MenuItem("ExtraTools.Potition", "Potition Manager:")).SetValue(false).SetFontStyle(FontStyle.Regular, SharpDX.Color.Gray);
                nMenuExtraToolsPackets.AddItem(new MenuItem("ExtraTools.Summoners", "Summoner Manager:")).SetValue(false).SetFontStyle(FontStyle.Regular, SharpDX.Color.Gray);
                nMenuExtraToolsPackets.AddItem(new MenuItem("ExtraTools.Tracker", "Tracker:")).SetValue(false).SetFontStyle(FontStyle.Regular, SharpDX.Color.Gray);

                nMenuExtraToolsPackets.AddItem(new MenuItem("ExtraTools.Reload", "Press F5 for Load Extra Tools!")).SetFontStyle(FontStyle.Bold, SharpDX.Color.GreenYellow);

                MenuExtraTools.AddSubMenu(nMenuExtraToolsPackets);

                MenuExtraToolsActivePackets = new Menu("Installed Tools", "MenuExtraTools.Installed").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow);

                MenuExtraTools.AddSubMenu(MenuExtraToolsActivePackets);
            }

            CommonSettings.Init(Config);

            OrbWalking = Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            ChampionClass.Orbwalker = new Orbwalking.Orbwalker(OrbWalking);

            MenuActivator = new Menu("Activator", "Activator").SetFontStyle(FontStyle.Regular, SharpDX.Color.Aqua);
            {
                if (MenuExtraTools.Item("ExtraTools.AutoLevel").GetValue<bool>())
                {
                    CommonAutoLevel.Init(MenuExtraToolsActivePackets);
                }

                if (MenuExtraTools.Item("ExtraTools.AutoPink").GetValue<bool>())
                {
                    CommonAutoPink.Initialize(MenuExtraToolsActivePackets);
                }

                if (MenuExtraTools.Item("ExtraTools.WarningSpells").GetValue<bool>())
                {
                    CommonWarningSpelss.Initialize(MenuExtraToolsActivePackets);
                }

                if (MenuExtraTools.Item("ExtraTools.AutoBush").GetValue<bool>())
                {
                    CommonAutoBush.Init(MenuExtraToolsActivePackets);
                }

                if (MenuExtraTools.Item("ExtraTools.Skin").GetValue<bool>())
                {
                    CommonSkinManager.Init(MenuExtraToolsActivePackets);
                }

                if (MenuExtraTools.Item("ExtraTools.Emote").GetValue<bool>())
                {
                    CommonEmote.Init(MenuExtraToolsActivePackets);
                }

                if (MenuExtraTools.Item("ExtraTools.AllySupport").GetValue<bool>())
                {
                    CommonAlly.Init(MenuExtraToolsActivePackets);
                }

                /* Menu Items */
                var items = MenuActivator.AddSubMenu(new Menu("Items", "Items"));
                items.AddItem(new MenuItem("BOTRK", "BOTRK").SetValue(true));
                items.AddItem(new MenuItem("GHOSTBLADE", "Ghostblade").SetValue(true));
                items.AddItem(new MenuItem("SWORD", "Sword of the Divine").SetValue(true));
                items.AddItem(new MenuItem("MURAMANA", "Muramana").SetValue(true));
                items.AddItem(new MenuItem("UseItemsMode", "Use items on").SetValue(new StringList(new[] {"No", "Mixed mode", "Combo mode", "Both"}, 2)));
            }
            Config.AddSubMenu(MenuActivator);

            // If Champion is supported draw the extra menus
            if (baseType != ChampionClass.GetType())
            {
                SetSmiteSlot();

                var combo = new Menu("Combo", "Combo").SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow);
                if (ChampionClass.ComboMenu(combo))
                {
                    if (SmiteSlot != SpellSlot.Unknown)
                        combo.AddItem(new MenuItem("ComboSmite", "Use Smite").SetValue(true));

                    Config.AddSubMenu(combo);
                }

                var harass = new Menu("Harass", "Harass");
                if (ChampionClass.HarassMenu(harass))
                {
                    harass.AddItem(new MenuItem("HarassMana", "Min. Mana Percent").SetValue(new Slider(50, 100, 0)));
                    Config.AddSubMenu(harass);
                }

                var menuClear = new Menu("Farm / Jungle", "Mode.Clear");
                {
                    MenuLane = new Menu("Lane", "Mode.Lane");
                    {
                        if (ChampionClass.LaneClearMenu(MenuLane))
                        {
                            MenuLane.AddItem(new MenuItem("Lane.Min.Mana", ":: Min. Mana %:").SetValue(new Slider(60, 100, 0)));
                        }
                        menuClear.AddSubMenu(MenuLane);
                    }

                    MenuJungle = new Menu("Jungle", "Mode.Jungle");
                    {
                        if (ChampionClass.JungleClearMenu(MenuJungle))
                        {
                            MenuJungle.AddItem(new MenuItem("Jungle.Min.Mana", ":: Min. Mana %:").SetValue(new Slider(30, 100, 0)));
                            MenuJungle.AddItem(new MenuItem("Jungle.Items", ":: Use Items:").SetValue(new StringList(new[] { "Off", "Use for Baron", "Use for Baron", "Both" }, 3)));
                            menuClear.AddSubMenu(MenuJungle);
                        }
                    }

                    menuClear.AddItem(new MenuItem("Farm.Active", ":: Farm Active!").SetValue(new KeyBind("J".ToCharArray()[0], KeyBindType.Toggle, true))).Permashow(true, "Marksman | Farm", SharpDX.Color.Aqua);
                    menuClear.AddItem(new MenuItem("Farm.Min.Mana.Control", ":: Min. Mana Control!").SetValue(new KeyBind("M".ToCharArray()[0], KeyBindType.Toggle, true))).Permashow(true, "Marksman | Farm Min. Mana Control", SharpDX.Color.Aqua);
                    Config.AddSubMenu(menuClear);
                }


                //var laneclear = new Menu("Lane Mode", "LaneClear");
                //if (ChampionClass.LaneClearMenu(laneclear))
                //{
                //    laneclear.AddItem(new MenuItem("Lane.Enabled", ":: Enable Lane Farm!").SetValue(new KeyBind("L".ToCharArray()[0],KeyBindType.Toggle, true))).Permashow(true, "Marksman | Enable Lane Farm", SharpDX.Color.Aqua);

                //    var minManaMenu = new Menu("Min. Mana Settings", "Lane.MinMana.Title");
                //    {
                //        minManaMenu.AddItem(new MenuItem("LaneMana.Alone", "If I'm Alone %:").SetValue(new Slider(30, 100, 0))).SetFontStyle(FontStyle.Regular, SharpDX.Color.LightSkyBlue);
                //        minManaMenu.AddItem(new MenuItem("LaneMana.Enemy", "If Enemy Close %:").SetValue(new Slider(60, 100, 0))).SetFontStyle(FontStyle.Regular, SharpDX.Color.IndianRed);
                //        laneclear.AddSubMenu(minManaMenu);
                //    }
                //    Config.AddSubMenu(laneclear);
                //}

                //var jungleClear = new Menu("Jungle Mode", "JungleClear");
                //if (ChampionClass.JungleClearMenu(jungleClear))
                //{
                //    var minManaMenu = new Menu("Min. Mana Settings", "Jungle.MinMana.Title");
                //    {
                //        minManaMenu.AddItem(new MenuItem("Jungle.Mana.Ally", "Ally Mobs %:").SetValue(new Slider(50, 100, 0))).SetFontStyle(FontStyle.Regular, SharpDX.Color.LightSkyBlue);
                //        minManaMenu.AddItem(new MenuItem("Jungle.Mana.Enemy", "Enemy Mobs %:").SetValue(new Slider(30, 100, 0))).SetFontStyle(FontStyle.Regular, SharpDX.Color.IndianRed);
                //        minManaMenu.AddItem(new MenuItem("Jungle.Mana.BigBoys", "Baron/Dragon %:").SetValue(new Slider(70, 100, 0))).SetFontStyle(FontStyle.Regular, SharpDX.Color.HotPink);
                //        jungleClear.AddSubMenu(minManaMenu);
                //    }
                //    jungleClear.AddItem(new MenuItem("Jungle.Items", ":: Use Items:").SetValue(new StringList(new[] {"Off", "Use for Baron", "Use for Baron", "Both"}, 3)));
                //    jungleClear.AddItem(new MenuItem("Jungle.Enabled", ":: Enable Jungle Farm!").SetValue(new KeyBind("J".ToCharArray()[0], KeyBindType.Toggle, true))).Permashow(true, "Marksman | Enable Jungle Farm", SharpDX.Color.Aqua);
                //    Config.AddSubMenu(jungleClear);
                //}

                /*----------------------------------------------------------------------------------------------------------*/
                //Obj_AI_Base ally = (from aAllies in HeroManager.Allies
                //    from aSupportedChampions in
                //        new[]
                //        {
                //            "janna", "tahm", "leona", "lulu", "lux", "nami", "shen", "sona", "braum", "bard"
                //        }
                //    where aSupportedChampions == aAllies.ChampionName.ToLower()
                //    select aAllies).FirstOrDefault();

                //if (ally != null)
                //{
                //    var menuAllies = new Menu("Ally Combo", "Ally.Combo").SetFontStyle(FontStyle.Regular, Color.Crimson);
                //    {
                //        AIHeroClient leona = HeroManager.Allies.Find(e => e.ChampionName.ToLower() == "leona");
                //        if (leona != null)
                //        {
                //            var menuLeona = new Menu("Leona", "Leona");
                //            menuLeona.AddItem(new MenuItem("Leona.ComboBuff", "Force Focus Marked Enemy for Bonus Damage").SetValue(true));
                //            menuAllies.AddSubMenu(menuLeona);
                //        }

                //        AIHeroClient Lux = HeroManager.Allies.Find(e => e.ChampionName.ToLower() == "lux");
                //        if (Lux != null)
                //        {
                //            var menuLux = new Menu("Lux", "Lux");
                //            menuLux.AddItem(new MenuItem("Lux.ComboBuff", "Force Focus Marked Enemy for Bonus Damage").SetValue(true));
                //            menuAllies.AddSubMenu(menuLux);
                //        }

                //        AIHeroClient Shen = HeroManager.Allies.Find(e => e.ChampionName.ToLower() == "shen");
                //        if (Shen != null)
                //        {
                //            var menuShen = new Menu("Shen", "Shen");
                //            menuShen.AddItem(new MenuItem("Shen.ComboBuff", "Force Focus Q Marked Enemy Objects for Heal").SetValue(true));
                //            menuShen.AddItem(new MenuItem("Shen.ComboBuff", "Minimum Heal:").SetValue(new Slider(80)));
                //            menuAllies.AddSubMenu(menuShen);
                //        }

                //        AIHeroClient Tahm = HeroManager.Allies.Find(e => e.ChampionName.ToLower() == "Tahm");
                //        if (Tahm != null)
                //        {
                //            var menuTahm = new Menu("Tahm", "Tahm");
                //            menuTahm.AddItem(new MenuItem("Tahm.ComboBuff", "Force Focus Marked Enemy for Stun").SetValue(true));
                //            menuAllies.AddSubMenu(menuTahm);
                //        }

                //        AIHeroClient Sona = HeroManager.Allies.Find(e => e.ChampionName.ToLower() == "Sona");
                //        if (Sona != null)
                //        {
                //            var menuSona = new Menu("Sona", "Sona");
                //            menuSona.AddItem(new MenuItem("Sona.ComboBuff", "Force Focus to Marked Enemy").SetValue(true));
                //            menuAllies.AddSubMenu(menuSona);
                //        }

                //        AIHeroClient Lulu = HeroManager.Allies.Find(e => e.ChampionName.ToLower() == "Lulu");
                //        if (Lulu != null)
                //        {
                //            var menuLulu = new Menu("Lulu", "Lulu");
                //            menuLulu.AddItem(new MenuItem("Lulu.ComboBuff", "Force Focus to Enemy If I have E buff").SetValue(true));
                //            menuAllies.AddSubMenu(menuLulu);
                //        }

                //        AIHeroClient Nami = HeroManager.Allies.Find(e => e.ChampionName.ToLower() == "nami");
                //        if (Nami != null)
                //        {
                //            var menuNami = new Menu("Nami", "Nami");
                //            menuNami.AddItem(new MenuItem("Nami.ComboBuff", "Force Focus to Enemy If I have E Buff").SetValue(true));
                //            menuAllies.AddSubMenu(menuNami);
                //        }
                //    }
                //    Config.AddSubMenu(menuAllies);
                //}
                /*----------------------------------------------------------------------------------------------------------*/

                var misc = new Menu("Misc", "Misc").SetFontStyle(FontStyle.Regular, SharpDX.Color.DarkOrange);
                if (ChampionClass.MiscMenu(misc))
                {
                    misc.AddItem(new MenuItem("Misc.SaveManaForUltimate", "Save Mana for Ultimate").SetValue(false));
                    Config.AddSubMenu(misc);
                }
                /*
                                var extras = new Menu("Extras", "Extras");
                                if (ChampionClass.ExtrasMenu(extras))
                                {
                                    Config.AddSubMenu(extras);
                                }
                 */

                var marksmanDrawings = new Menu("Drawings", "MDrawings");
                Config.AddSubMenu(marksmanDrawings);

                var drawing = new Menu(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(championName), "Drawings").SetFontStyle(FontStyle.Regular, SharpDX.Color.Aquamarine);
                if (ChampionClass.DrawingMenu(drawing))
                {
                    marksmanDrawings.AddSubMenu(drawing);
                }

                var globalDrawings = new Menu("Global", "GDrawings");
                {
                    marksmanDrawings.AddItem(new MenuItem("Draw.TurnOff", "Drawings").SetValue(new StringList(new[]{"Disable", "Enable", "Disable on Combo Mode", "Disable on Lane/Jungle Mode", "Both"}, 1)));
                    globalDrawings.AddItem(new MenuItem("Draw.MinionLastHit", "Minion Last Hit").SetValue(new StringList(new[] {"Off", "On", "Just Out of AA Range Minions"}, 2)));
                    globalDrawings.AddItem(new MenuItem("Draw.KillableEnemy", "Killable Enemy Text").SetValue(false));
                    //GlobalDrawings.AddItem(new MenuItem("Draw.JunglePosition", "Jungle Farm Position").SetValue(new StringList(new[] { "Off", "If I'm Close to Mobs", "If Jungle Clear Active" }, 2)));
                    marksmanDrawings.AddSubMenu(globalDrawings);
                }

                //CreateButtons();
            }

            ChampionClass.MainMenu(Config);
            ChampionClass.ToolsMenu(MenuExtraTools);

            //Evade.Evade.Initiliaze();
            //Config.AddSubMenu(Evade.Config.Menu);
            //var y = new Common.CommonObjectDetector();
            Config.AddToMainMenu();
            MenuExtraTools.AddToMainMenu();

            foreach (var i in Config.Children.SelectMany(GetChildirens))
            {
                i.DisplayName = ":: " + i.DisplayName;
            }

            foreach (var i in MenuExtraTools.Children.SelectMany(GetChildirens))
            {
                i.DisplayName = ":: " + i.DisplayName;
            }


            //CheckAutoWindUp();

            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += eventArgs =>
            {
                //DrawButtons();
                if (Config.Item("Draw.KillableEnemy").GetValue<bool>())
                {

                    foreach (var e in HeroManager.Enemies.Where(e => e.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) * 2)))
                    {
                        var x = (int)Math.Ceiling(e.Health / ObjectManager.Player.GetAutoAttackDamage(e));
                        //Utils.Utils.DrawText(CommonGeometry.Text, $"{x} x AA = Kill", (int)e.HPBarPosition.X + 20, (int)e.HPBarPosition.Y + 17, SharpDX.Color.White);
                        CommonGeometry.Text.DrawTextLeft(string.Format("{0} x AA Kill", x), (int)e.HPBarPosition.X + 5, (int)e.HPBarPosition.Y + 25, SharpDX.Color.White);

                    }
                    //var t = KillableEnemyAa;
                    //if (t.Key != null && t.Key.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 1000) &&
                    //    t.Value > 0)
                    //{
                    //    Utils.Utils.DrawText(CommonGeometry.Text, string.Format("{0} x AA Damage = Kill", t.Value), (int)t.Key.HPBarPosition.X + 30, (int)t.Key.HPBarPosition.Y + 5, SharpDX.Color.White);


                    //}
                }
            };
            Game.OnUpdate += GameOnUpdate;
            Game.OnUpdate += delegate(EventArgs eventArgs) { ChampionClass.GameOnUpdate(eventArgs); };

            Game.OnUpdate += eventArgs =>
            {
                if (ChampionClass.ComboActive) ChampionClass.ExecuteCombo();
                if (ChampionClass.LaneClearActive) ChampionClass.ExecuteLane();
                if (ChampionClass.JungleClearActive) ChampionClass.ExecuteJungle();
                ChampionClass.PermaActive();
            };

            Orbwalking.OnAttack += (unit, target) =>
            {//
               // if (unit.IsMe)
                 //   Chat.Print("Attack");
            };
            Orbwalking.AfterAttack += (unit, target) => { ChampionClass.Orbwalking_AfterAttack(unit, target); };
            Orbwalking.BeforeAttack += (eventArgs) => { ChampionClass.Orbwalking_BeforeAttack(eventArgs); };
            

            GameObject.OnCreate += (sender, eventArgs) => { ChampionClass.OnCreateObject(sender, eventArgs); };
            GameObject.OnDelete += (sender, eventArgs) => { ChampionClass.OnDeleteObject(sender, eventArgs); };

            Drawing.OnEndScene += eventArgs => { ChampionClass.DrawingOnEndScene(eventArgs); };

            Obj_AI_Base.OnBuffGain += (sender, eventArgs) => { ChampionClass.Obj_AI_Base_OnBuffAdd(sender, eventArgs); };
            Obj_AI_Base.OnBuffLose += (sender, eventArgs) => { ChampionClass.Obj_AI_Base_OnBuffLose(sender, eventArgs); };
            Obj_AI_Base.OnProcessSpellCast += (sender, eventArgs) => { ChampionClass.Obj_AI_Base_OnProcessSpellCast(sender, eventArgs); };
            Obj_AI_Base.OnPlayAnimation += (sender, eventArgs) => { ChampionClass.Obj_AI_Base_OnPlayAnimation(sender, eventArgs); };

            AntiGapcloser.OnEnemyGapcloser += (gapcloser) => { ChampionClass.AntiGapcloser_OnEnemyGapcloser(gapcloser); };

            Spellbook.OnCastSpell += (sender, eventArgs) => { ChampionClass.Spellbook_OnCastSpell(sender, eventArgs); };

            Interrupter2.OnInterruptableTarget += (sender, eventArgs) => { ChampionClass.Interrupter2_OnInterruptableTarget(sender, eventArgs); };

            Obj_AI_Base.OnPlayAnimation += (sender, eventArgs) => { ChampionClass.Obj_AI_Base_OnPlayAnimation(sender, eventArgs); };

            Console.Clear();
        }

        private static IEnumerable<Menu> GetChildirens(Menu menu)
        {
            yield return menu;

            foreach (var childChild in menu.Children.SelectMany(GetChildirens))
                yield return childChild;
        }

        private static void CheckAutoWindUp()
        {
            var additional = 0;

            if (Game.Ping >= 100)
            {
                additional = Game.Ping/100*10;
            }
            else if (Game.Ping > 40 && Game.Ping < 100)
            {
                additional = Game.Ping/100*20;
            }
            else if (Game.Ping <= 40)
            {
                additional = +20;
            }
            var windUp = Game.Ping + additional;
            if (windUp < 40)
            {
                windUp = 40;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var turnOffDrawings = Config.Item("Draw.TurnOff").GetValue<StringList>().SelectedIndex;

            if (turnOffDrawings == 0)
            {
                return;
            }

            if ((turnOffDrawings == 2 || turnOffDrawings == 4) &&
                ChampionClass.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            if ((turnOffDrawings == 3 || turnOffDrawings == 4) &&
                (ChampionClass.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit ||
                 ChampionClass.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear))
            {
                return;
            }

            var drawMinionLastHit = Config.Item("Draw.MinionLastHit").GetValue<StringList>().SelectedIndex;
            if (drawMinionLastHit != 0)
            {
                var mx =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(m => !m.IsDead && m.IsEnemy)
                        .Where(m => m.Health <= ObjectManager.Player.TotalAttackDamage);

                if (drawMinionLastHit == 1)
                {
                    mx = mx.Where(m => m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65));
                }
                mx =
                    mx.Where(
                        m =>
                            m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65 + 300) &&
                            m.Distance(ObjectManager.Player.Position) > Orbwalking.GetRealAutoAttackRange(null) + 65);

                foreach (var minion in mx)
                {
                    Render.Circle.DrawCircle(minion.Position, minion.BoundingRadius, Color.GreenYellow, 1);
                }
            }

            ChampionClass?.Drawing_OnDraw(args);
        }

        private static void GameOnUpdate(EventArgs args)
        {
            //Update the combo and harass values.
            ChampionClass.ComboActive = ChampionClass.Config.Item("Orbwalk").GetValue<KeyBind>().Active;

            var vHarassManaPer = Config.Item("HarassMana").GetValue<Slider>().Value;
            ChampionClass.HarassActive = ChampionClass.Config.Item("Farm").GetValue<KeyBind>().Active &&
                                         ObjectManager.Player.ManaPercent >= vHarassManaPer;

            ChampionClass.ToggleActive = ObjectManager.Player.ManaPercent >= vHarassManaPer &&
                                         ChampionClass.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo &&
                                         !ObjectManager.Player.IsRecalling();

            #region LaneClearActive
            ChampionClass.LaneClearActive = ChampionClass.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None &&
                                            ChampionClass.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo &&
                                            Config.Item("Farm.Active").GetValue<KeyBind>().Active &&
                                            ObjectManager.Player.ManaPercent >= (Config.Item("Farm.Min.Mana.Control").GetValue<KeyBind>().Active ? Config.Item("Lane.Min.Mana").GetValue<Slider>().Value : 0);
            #endregion LaneClearActive    

            #region JungleClearActive
            ChampionClass.JungleClearActive = ChampionClass.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear &&
                                              Config.Item("Farm.Active").GetValue<KeyBind>().Active &&
                                              MinionManager.GetMinions(ObjectManager.Player.Position, LeagueSharp.Common.Orbwalking.GetRealAutoAttackRange(null) + 65, MinionTypes.All, MinionTeam.Neutral).Count > 0 &&
                                              ObjectManager.Player.ManaPercent >= (Config.Item("Farm.Min.Mana.Control").GetValue<KeyBind>().Active ? Config.Item("Jungle.Min.Mana").GetValue<Slider>().Value : 0);
            #endregion JungleClearActive

            ChampionClass.LastHitActive = ChampionClass.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit;

            //ChampionClass.JungleClearActive = ChampionClass.Config.Item("LaneClear").GetValue<KeyBind>().Active && ObjectManager.Player.ManaPercent >= Config.Item("Jungle.Mana").GetValue<Slider>().Value;

            //ChampionClass.GameOnUpdate(args);

            UseSummoners();
            var useItemModes = Config.Item("UseItemsMode").GetValue<StringList>().SelectedIndex;

            //Items
            if (
                !((ChampionClass.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo &&
                   (useItemModes == 2 || useItemModes == 3))
                  ||
                  (ChampionClass.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed &&
                   (useItemModes == 1 || useItemModes == 3))))
                return;

            var botrk = Config.Item("BOTRK").GetValue<bool>();
            var ghostblade = Config.Item("GHOSTBLADE").GetValue<bool>();
            var sword = Config.Item("SWORD").GetValue<bool>();
            var muramana = Config.Item("MURAMANA").GetValue<bool>();
            var target = ChampionClass.Orbwalker.GetTarget() as Obj_AI_Base;

            var smiteReady = SmiteSlot != SpellSlot.Unknown &&
                             ObjectManager.Player.Spellbook.CanUseSpell(SmiteSlot) == SpellState.Ready;

            if (smiteReady && ChampionClass.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                Smiteontarget(target as AIHeroClient);

            if (botrk)
            {
                if (target != null && target.Type == ObjectManager.Player.Type &&
                    target.ServerPosition.Distance(ObjectManager.Player.ServerPosition) < 550)
                {
                    var hasCutGlass = Items.HasItem(3144);
                    var hasBotrk = Items.HasItem(3153);

                    if (hasBotrk || hasCutGlass)
                    {
                        var itemId = hasCutGlass ? 3144 : 3153;
                        var damage = ObjectManager.Player.GetItemDamage(target, Damage.DamageItems.Botrk);
                        if (hasCutGlass || ObjectManager.Player.Health + damage < ObjectManager.Player.MaxHealth)
                            Items.UseItem(itemId, target);
                    }
                }
            }

            if (ghostblade && target != null && target.Type == ObjectManager.Player.Type &&
                !ObjectManager.Player.HasBuff("ItemSoTD") /*if Sword of the divine is not active */
                && Orbwalking.InAutoAttackRange(target))
                Items.UseItem(3142);

            if (sword && target != null && target.Type == ObjectManager.Player.Type &&
                !ObjectManager.Player.HasBuff("spectralfury") /*if ghostblade is not active*/
                && Orbwalking.InAutoAttackRange(target))
                Items.UseItem(3131);

            if (muramana && Items.HasItem(3042))
            {
                if (target != null && ChampionClass.ComboActive &&
                    target.Position.Distance(ObjectManager.Player.Position) < 1200)
                {
                    if (!ObjectManager.Player.HasBuff("Muramana"))
                    {
                        Items.UseItem(3042);
                    }
                }
                else
                {
                    if (ObjectManager.Player.HasBuff("Muramana"))
                    {
                        Items.UseItem(3042);
                    }
                }
            }
        }

        public static void UseSummoners()
        {
            if (ChampionClass.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            var t = ChampionClass.Orbwalker.GetTarget() as AIHeroClient;

            if (t != null && igniteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell(igniteSlot) == SpellState.Ready)
            {
                if (ObjectManager.Player.Distance(t) < 650 &&
                    ObjectManager.Player.GetSummonerSpellDamage(t, Damage.SummonerSpell.Ignite) >=
                    t.Health)
                {
                    ObjectManager.Player.Spellbook.CastSpell(igniteSlot, t);
                }
            }
        }


        
        private static void SetSmiteSlot()
        {
            foreach (
                var spell in
                    ObjectManager.Player.Spellbook.Spells.Where(
                        spell => string.Equals(spell.Name, Smitetype, StringComparison.CurrentCultureIgnoreCase)))
            {
                SmiteSlot = spell.Slot;
                Smite = new Spell(SmiteSlot, 700);
            }
        }

        private static void Smiteontarget(AIHeroClient t)
        {
            var useSmite = Config.Item("ComboSmite").GetValue<bool>();
            var itemCheck = SmiteBlue.Any(i => Items.HasItem(i)) || SmiteRed.Any(i => Items.HasItem(i));
            if (itemCheck && useSmite &&
                ObjectManager.Player.Spellbook.CanUseSpell(SmiteSlot) == SpellState.Ready &&
                t.Distance(ObjectManager.Player.Position) < Smite.Range)
            {
                ObjectManager.Player.Spellbook.CastSpell(SmiteSlot, t);
            }
        }

        public static void DrawBox(Vector2 position, int width, int height, Color color, int borderwidth,
            Color borderColor)
        {
            Drawing.DrawLine(position.X, position.Y, position.X + width, position.Y, height, color);

            if (borderwidth > 0)
            {
                Drawing.DrawLine(position.X, position.Y, position.X + width, position.Y, borderwidth, borderColor);
                Drawing.DrawLine(position.X, position.Y + height, position.X + width, position.Y + height, borderwidth,
                    borderColor);
                Drawing.DrawLine(position.X, position.Y + 1, position.X, position.Y + height, borderwidth, borderColor);
                Drawing.DrawLine(position.X + width, position.Y + 1, position.X + width, position.Y + height,
                    borderwidth, borderColor);
            }
        }

        private static KeyValuePair<AIHeroClient, int> KillableEnemyAa
        {
            get
            {
                var x = 0;
                var t = TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) * 2, TargetSelector.DamageType.Physical);
                {
                    if (t.IsValidTarget())
                    {
                        //if (t.Health < ObjectManager.Player.TotalAttackDamage * (1 / ObjectManager.Player.AttackCastDelay > 1400 ? 8 : 4))
                        //{
                            x = (int)Math.Ceiling(t.Health / ObjectManager.Player.GetAutoAttackDamage(t));
                        //}
                        return new KeyValuePair<AIHeroClient, int>(t, x);
                    }
                }
                return new KeyValuePair<AIHeroClient, int>(t, x);
            }
        }
    }
}