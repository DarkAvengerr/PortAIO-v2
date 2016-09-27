using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using TheTwitch.Commons;
using TheTwitch.Commons.ComboSystem;
using TheTwitch.Commons.Debug;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheTwitch
{
    class Twitch
    {
        private Orbwalking.Orbwalker _orbwalker;

        public void Load()
        {
            if (ObjectManager.Player.ChampionName != "Twitch")
                return;

            //DevAssistant.Init();

            var notification = new Notification("The Twitch loaded", 3) { TextColor = new SharpDX.ColorBGRA(0, 255, 0, 255), BorderColor = new SharpDX.ColorBGRA(144, 238, 144, 255) };
            Notifications.AddNotification(notification);

            var mainMenu = CreateMenu("The Twitch", true);
            var orbwalkerMenu = CreateMenu("Orbwalker", mainMenu);
            var comboMenu = CreateMenu("Combo", mainMenu);
            var harassMenu = CreateMenu("Harass", mainMenu);
            var laneclearMenu = CreateMenu("Laneclear", mainMenu);
            var miscMenu = CreateMenu("Misc", mainMenu);
            var antigapcloserMenu = CreateMenu("Anti gapcloser", mainMenu);
            var itemMenu = CreateMenu("Items", mainMenu);
            var summonerMenu = CreateMenu("Summoners", mainMenu);
            var autoLevelSpells = mainMenu.CreateSubmenu("Auto level spells");
            var manamanagerMenu = CreateMenu("Manamanager", mainMenu);
            var drawingMenu = CreateMenu("Drawing", mainMenu);


            _orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            var combo = new TwitchCombo(1000, _orbwalker, new TwitchQ(SpellSlot.Q), new TwitchW(SpellSlot.W), new TwitchE(SpellSlot.E), new TwitchR(SpellSlot.R));

            combo.CreateBasicMenu(comboMenu, harassMenu, null, antigapcloserMenu, null, manamanagerMenu, summonerMenu, itemMenu, drawingMenu);
            combo.CreateLaneclearMenu(laneclearMenu, true, SpellSlot.Q);
            combo.CreateAutoLevelMenu(autoLevelSpells, ComboProvider.SpellOrder.REWQ, ComboProvider.SpellOrder.REQW);

            comboMenu.Item("Combo.UseQ").SetTooltip("(for attackspeed, may cancel autos when stealthing!)");

            comboMenu.AddMItem("Min Enemies near for R", new Slider(Math.Min(2, HeroManager.Enemies.Count), 1, Math.Max(2, HeroManager.Enemies.Count)), (sender, args) => combo.GetSkill<TwitchR>().MinEnemies = args.GetNewValue<Slider>().Value);
            comboMenu.AddMItem("E at full stacks", true, (sender, args) => combo.GetSkill<TwitchE>().AlwaysExecuteAtFullStacks = args.GetNewValue<bool>());
            comboMenu.AddMItem("Custom E calculation", true, (sender, args) => combo.GetSkill<TwitchE>().CustomCalculation = args.GetNewValue<bool>());
            comboMenu.AddMItem("Only W after if >= X stacks", new Slider(0, 0, 6), (sender, args) => combo.GetSkill<TwitchW>().ComboAfterStacks = args.GetNewValue<Slider>().Value).SetTooltip("W dmg scales on stacks");
            comboMenu.AddMItem("Do not W when low on mana", true, (sender, args) => combo.GetSkill<TwitchW>().NoCastWhenLowMana = args.GetNewValue<bool>());
            comboMenu.ProcStoredValueChanged<Slider>();
            comboMenu.ProcStoredValueChanged<bool>();

            harassMenu.AddMItem("E after trade if >= X stacks", new Slider(3, 1, 6), (sender, args) => combo.GetSkill<TwitchE>().HarassActivateWhenLeaving = args.GetNewValue<Slider>().Value);
            harassMenu.AddMItem("Only W after if >= X stacks", new Slider(0, 0, 6), (sender, args) => combo.GetSkill<TwitchW>().HarassAfterStacks = args.GetNewValue<Slider>().Value);
            harassMenu.ProcStoredValueChanged<Slider>();

            antigapcloserMenu.AddMItem("Uses W if enabled");

            laneclearMenu.AddMItem("Min W targets", new Slider(4, 1, 8), (sender, args) => combo.GetSkill<TwitchW>().MinFarmMinions = args.GetNewValue<Slider>().Value);
            laneclearMenu.AddMItem("Min E kills", new Slider(3, 1, 8), (sender, args) => combo.GetSkill<TwitchE>().MinFarmMinions = args.GetNewValue<Slider>().Value);
            laneclearMenu.AddMItem("Min E targets", new Slider(6, 1, 16), (sender, args) => combo.GetSkill<TwitchE>().MinFarmDamageMinions = args.GetNewValue<Slider>().Value);
            laneclearMenu.AddMItem("(Uses E if kills OR targets are here)").FontColor = new ColorBGRA(0, 255, 255, 255);
            laneclearMenu.ProcStoredValueChanged<Slider>();

            miscMenu.AddMItem("E Killsteal", true, (sender, args) => combo.GetSkill<TwitchE>().Killsteal = args.GetNewValue<bool>());
            miscMenu.AddMItem("E farm assist", false, (sender, args) => combo.GetSkill<TwitchE>().FarmAssist = args.GetNewValue<bool>());
            miscMenu.AddMItem("W AOE prediction", true, (sender, args) => combo.GetSkill<TwitchW>().IsAreaOfEffect = args.GetNewValue<bool>()).SetTooltip("Will try to hit multiple targets, but has worse hitchance");
            combo.GetSkill<TwitchQ>().StealthRecall = miscMenu.AddMItem("Stealh recall", new KeyBind(66, KeyBindType.Press));
            miscMenu.AddMItem("Don't W during R", false, (sender, args) => combo.GetSkill<TwitchW>().NotDuringR = args.GetNewValue<bool>());
            miscMenu.AddMItem("Auto buy blue trinket", true, (sender, args) => combo.AutoBuyBlueTrinket = args.GetNewValue<bool>());
            miscMenu.AddMItem("Blue trinket when level:", new Slider(6, 1, 18), (sender, args) => combo.BlueTrinketLevel = args.GetNewValue<Slider>().Value);
            miscMenu.ProcStoredValueChanged<bool>();
            miscMenu.ProcStoredValueChanged<Slider>();

            //drawingMenu.AddMItem("Draw Q Range", new Circle(true, Color.Gray), (sender, args) => combo.GetSkill<TwitchQ>().DrawRange = args.GetNewValue<Circle>());
            combo.GetSkill<TwitchQ>().DrawRange = new Circle(true, Color.Gray);

            drawingMenu.AddMItem("Draw W Range", new Circle(false, Color.LightGreen), (sender, args) => combo.GetSkill<TwitchW>().DrawRange = args.GetNewValue<Circle>());
            drawingMenu.AddMItem("Draw E Range", new Circle(true, Color.DarkGreen), (sender, args) => combo.GetSkill<TwitchE>().DrawRange = args.GetNewValue<Circle>());
            drawingMenu.AddMItem("Draw R Range", new Circle(false, Color.Goldenrod), (sender, args) => combo.GetSkill<TwitchR>().DrawRange = args.GetNewValue<Circle>());
            drawingMenu.ProcStoredValueChanged<Circle>();

            combo.Initialize();
            mainMenu.AddToMainMenu();

            Game.OnUpdate += _ =>
            {
                combo.Update();

            };
        }


        private Menu CreateMenu(string name, Menu menu)
        {
            var newMenu = new Menu(name, name);
            menu.AddSubMenu(newMenu);
            return newMenu;
        }

        private Menu CreateMenu(string name, bool root = false)
        {
            return new Menu(name, name, root);
        }
    }
}
