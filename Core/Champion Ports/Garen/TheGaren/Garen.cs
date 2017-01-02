using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using TheKalista.Commons;
using TheKalista.Commons.ComboSystem;
using TheKalista.Commons.Debug;
using TheKalista.Commons.Items;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheGaren
{
    class Garen
    {
        private ComboProvider _comboProvider;
        private Circle _drawR, _drawFlashUlt;
        private GarenR _r;
        private SpellDataInst _flash;

        public void Load()
        {
            if (ObjectManager.Player.ChampionName != "Garen")
                return;
            Notifications.AddNotification("The Garen v2 loaded!", 3);

            var mainMenu = new Menu("The Garen", "The Garen", true);
            var orbwalkerMenu = mainMenu.CreateSubmenu("Orbwalker");
            var targetSelectorMenu = mainMenu.CreateSubmenu("Target Selector");
            var comboMenu = mainMenu.CreateSubmenu("Combo");
            var laneClearMenu = mainMenu.CreateSubmenu("Lane Clear");
            var miscMenu = mainMenu.CreateSubmenu("Misc");
            var items = mainMenu.CreateSubmenu("Items");
            var gapcloserMenu = mainMenu.CreateSubmenu("Gapcloser");
            var interrupterMenu = mainMenu.CreateSubmenu("Interrupter");
            var autoLevel = mainMenu.CreateSubmenu("Auto-level Spells");
            var drawingMenu = mainMenu.CreateSubmenu("Drawing");


            var orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            _comboProvider = new ComboProvider(500, new Skill[] { new GarenQ((SpellSlot.Q)), new GarenW((SpellSlot.W)), new GarenE((SpellSlot.E)), new GarenR((SpellSlot.R)) }.ToList(), orbwalker);
            _r = _comboProvider.GetSkill<GarenR>();
            _flash = ObjectManager.Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "summonerflash");
            _comboProvider.CreateBasicMenu(targetSelectorMenu, comboMenu, null, null, null, gapcloserMenu, interrupterMenu, null, false);
            _comboProvider.CreateLaneclearMenu(laneClearMenu, false, SpellSlot.W);
            _comboProvider.GetSkill<GarenE>().ItemManager = _comboProvider.CreateItemsMenu(items, new RavenousHydra(), new BilgewaterCutlass(), new YoumusBlade(), new Botrk());
            _comboProvider.CreateAutoLevelMenu(autoLevel, ComboProvider.SpellOrder.REQW, ComboProvider.SpellOrder.REQW);

            comboMenu.AddMItem("Q after Auto Attack", true, (sender, args) => _comboProvider.GetSkill<GarenQ>().OnlyAfterAuto = args.GetNewValue<bool>());
            comboMenu.AddMItem("E after Auto Attack", true, (sender, args) => _comboProvider.GetSkill<GarenE>().OnlyAfterAuto = args.GetNewValue<bool>());
            comboMenu.AddMItem("Use R to KS", false, (sender, args) => _comboProvider.GetSkill<GarenR>().Killsteal = args.GetNewValue<bool>());
            comboMenu.AddMItem("Q to Get in Range", true, (sender, args) => _comboProvider.GetSkill<GarenQ>().UseWhenOutOfRange = args.GetNewValue<bool>());

            miscMenu.AddMItem("Use W Out of Combo", true, (sender, args) => _comboProvider.GetSkill<GarenW>().UseAlways = args.GetNewValue<bool>());
            miscMenu.AddMItem("Min. Incoming Damage for W in %HP", new Slider(2, 1, 15), (sender, args) => _comboProvider.GetSkill<GarenW>().MinDamagePercent = args.GetNewValue<Slider>().Value);
            miscMenu.AddMItem("Always W Enemy Ults", true, (sender, args) => _comboProvider.GetSkill<GarenW>().UseOnUltimates = args.GetNewValue<bool>());

            gapcloserMenu.AddMItem("(Use W if Enabled)");

            laneClearMenu.AddMItem("E if X Minions", new Slider(1, 1, 8), (sender, args) => _comboProvider.GetSkill<GarenE>().MinFarmMinions = args.GetNewValue<Slider>().Value);
            laneClearMenu.AddMItem("Use Hydra", true, (sender, args) => _comboProvider.GetSkill<GarenE>().UseHydra = args.GetNewValue<bool>());

            drawingMenu.AddMItem("Damage Indicator", new Circle(true, Color.FromArgb(100, Color.Goldenrod)), (sender, args) =>
            {
                //DamageIndicator.DamageToUnit = _comboProvider.GetComboDamage;
                //DamageIndicator.Enabled = args.GetNewValue<Circle>().Active;
                //DamageIndicator.FillColor = args.GetNewValue<Circle>().Color;
                //DamageIndicator.Fill = true;
                //DamageIndicator.Color = Color.FromArgb(255, //DamageIndicator.FillColor);
            });
            drawingMenu.AddMItem("Draw R Range", new Circle(true, Color.Goldenrod), (sender, args) => _drawR = args.GetNewValue<Circle>());
            drawingMenu.AddMItem("Draw Flash-R Indicator", new Circle(true, Color.Red), (sender, args) => _drawFlashUlt = args.GetNewValue<Circle>());
            drawingMenu.AddMItem("Damage Indicator by xSalice / detuks!");

            //mainMenu.AddMItem("Max order: R > E > Q > W! Have fun!");
            mainMenu.AddToMainMenu();
            _comboProvider.Initialize();
            //DevAssistant.Init();
            Game.OnUpdate += Tick;
            Drawing.OnDraw += Draw;
        }

        private void Draw(EventArgs args)
        {
            if (_drawR.Active)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 375, _drawR.Color);
            if (_drawFlashUlt.Active && _r.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(_flash != null && _flash.IsReady() ? 375 + 425 : 375) && _r.IsKillable(enemy)))
                {
                    var screenPos = Drawing.WorldToScreen(enemy.Position);
                    Drawing.DrawText(screenPos.X - 50, screenPos.Y - 50, _drawFlashUlt.Color, "Flash-R Possible!");
                }
            }

            //foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget()))
            //{
            //    var screenPos = Drawing.WorldToScreen(enemy.Position);
            //    Drawing.DrawText(screenPos.X - 50, screenPos.Y - 50, _drawFlashUlt.Color, (enemy.Health - HealthPrediction.GetHealthPrediction(enemy,1)).ToString());
            //}

            //Drawing.DrawText(200, 100, Color.Red, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).GetState().ToString() + " " + (int)ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).GetState());

        }

        private void Tick(EventArgs args)
        {
            _comboProvider.Update();
        }
    }
}
