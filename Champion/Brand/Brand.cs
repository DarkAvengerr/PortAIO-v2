using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using TheBrand.Commons;
using TheBrand.Commons.ComboSystem;
using Color = System.Drawing.Color;
using EloBuddy;

namespace TheBrand
{
    class Brand
    {
        private BrandCombo _comboProvider;
        private Menu _mainMenu;
        private Orbwalking.Orbwalker _orbwalker;
        private MenuItem _drawQ, _drawW, _drawE, _drawR;

        public void Load()
        {
            try
            {
                if (ObjectManager.Player.ChampionName != "Brand")
                    return;

                var notification = new Notification("The Brand loaded", 10000) { TextColor = new SharpDX.ColorBGRA(255, 0, 0, 255), BorderColor = new SharpDX.ColorBGRA(139, 100, 0, 255) };
                Notifications.AddNotification(notification);

                _mainMenu = CreateMenu("The Brand", true);
                var orbwalkerMenu = CreateMenu("Orbwalker", _mainMenu);
                var comboMenu = CreateMenu("Combo", _mainMenu);
                var harassMenu = CreateMenu("Harass", _mainMenu);
                var laneclearMenu = CreateMenu("Laneclear", _mainMenu);
                var manamanagerMenu = CreateMenu("Manamanager", _mainMenu);
                var igniteMenu = CreateMenu("Ignite", _mainMenu);
                var miscMenu = CreateMenu("Misc", _mainMenu);
                var antiGapcloser = CreateMenu("Anti gapcloser", _mainMenu);
                var interrupter = CreateMenu("Interrupter", _mainMenu);
                var autoLevel = CreateMenu("Auto level spells", _mainMenu);
                var drawingMenu = CreateMenu("Drawing", _mainMenu);

                _orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

                _comboProvider = new BrandCombo(1050, _orbwalker, new BrandQ(SpellSlot.Q), new BrandW(SpellSlot.W), new BrandE(SpellSlot.E), new BrandR(SpellSlot.R));

                _comboProvider.CreateBasicMenu(comboMenu, harassMenu, null, antiGapcloser, interrupter, manamanagerMenu, igniteMenu, null, drawingMenu);
                _comboProvider.CreateLaneclearMenu(laneclearMenu, true, SpellSlot.Q, SpellSlot.R);
                _comboProvider.CreateAutoLevelMenu(autoLevel, ComboProvider.SpellOrder.RWQE, ComboProvider.SpellOrder.RWQE);

                var rOptions = CreateMenu("Ult Options", comboMenu);
                rOptions.AddMItem("Bridge R", false, (sender, args) => _comboProvider.GetSkill<BrandR>().UseBridgeUlt = args.GetNewValue<bool>());
                rOptions.AddMItem("", "_");
                rOptions.AddMItem("Risky R", true, (sender, args) => _comboProvider.GetSkill<BrandR>().RiskyUlt = args.GetNewValue<bool>()).SetTooltip("R bounces, may fail");
                rOptions.AddMItem("", "__");
                rOptions.AddMItem("Ult non killable", true, (sender, args) => _comboProvider.GetSkill<BrandR>().UltNonKillable = args.GetNewValue<bool>()).ProcStoredValueChanged<bool>();
                rOptions.AddMItem("when min X targets", new Slider(Math.Min(HeroManager.Enemies.Count, 1), 1, Math.Max(HeroManager.Enemies.Count, 2)), (sender, args) => _comboProvider.GetSkill<BrandR>().MinBounceTargets = args.GetNewValue<Slider>().Value).ProcStoredValueChanged<Slider>();
                rOptions.AddMItem("", "___");
                rOptions.AddMItem("Don't R with", true, (sender, args) => _comboProvider.GetSkill<BrandR>().AntiOverkill = args.GetNewValue<bool>()).ProcStoredValueChanged<bool>();
                rOptions.AddMItem("% Health difference", new Slider(60), (sender, args) => _comboProvider.GetSkill<BrandR>().OverkillPercent = args.GetNewValue<Slider>().Value).ProcStoredValueChanged<Slider>();
                rOptions.AddMItem("Ignore when fleeing", true, (sender, args) => _comboProvider.GetSkill<BrandR>().IgnoreAntiOverkillOnFlee = args.GetNewValue<bool>()).ProcStoredValueChanged<bool>();
                rOptions.ProcStoredValueChanged<bool>();
                rOptions.ProcStoredValueChanged<Slider>();

                laneclearMenu.AddMItem("Min W targets", new Slider(3, 1, 10), (sender, args) => _comboProvider.GetSkill<BrandW>().WaveclearTargets = args.GetNewValue<Slider>().Value).ProcStoredValueChanged<Slider>();

                miscMenu.AddMItem("E on fire-minion", true, (sender, args) => _comboProvider.GetSkill<BrandE>().UseMinions = args.GetNewValue<bool>());
                miscMenu.AddMItem("Try AOE with W", true, (sender, args) => _comboProvider.GetSkill<BrandW>().TryAreaOfEffect = args.GetNewValue<bool>());
                miscMenu.AddMItem("E farm assist", true, (sender, args) => _comboProvider.GetSkill<BrandE>().FarmAssist = args.GetNewValue<bool>());
                miscMenu.AddMItem("E Killsteal", true, (sender, args) => _comboProvider.GetSkill<BrandE>().Killsteal = args.GetNewValue<bool>());
                miscMenu.AddMItem("Only KS in Combo", false, (sender, args) => _comboProvider.GetSkill<BrandE>().KillstealCombo = args.GetNewValue<bool>());
                miscMenu.AddMItem("Force AA in combo", false, (sender, args) => _comboProvider.ForceAutoAttacks = args.GetNewValue<bool>());
                miscMenu.AddMItem("Targetselector range", new Slider((int)_comboProvider.TargetRange, 900, 1500), (sender, args) => { _comboProvider.TargetRange = args.GetNewValue<Slider>().Value; });
                miscMenu.ProcStoredValueChanged<bool>();
                miscMenu.ProcStoredValueChanged<Slider>();

                interrupter.AddMItem("E Usage", true, (sender, args) =>
                {
                    _comboProvider.GetSkill<BrandW>().InterruptE = args.GetNewValue<bool>();
                    _comboProvider.GetSkill<BrandE>().InterruptE = args.GetNewValue<bool>();
                });
                interrupter.AddMItem("W Usage", true, (sender, args) => _comboProvider.GetSkill<BrandW>().InterruptW = args.GetNewValue<bool>());
                interrupter.ProcStoredValueChanged<bool>();

                drawingMenu.AddMItem("Damage indicator", new Circle(true, Color.Yellow), (sender, args) =>
                {
                    DamageIndicator.Enabled = args.GetNewValue<Circle>().Active;
                    DamageIndicator.Fill = true;
                    DamageIndicator.FillColor = Color.FromArgb(100, args.GetNewValue<Circle>().Color);
                    DamageIndicator.Color = Color.FromArgb(200, DamageIndicator.FillColor);
                    DamageIndicator.DamageToUnit = _comboProvider.GetComboDamage;
                }).ProcStoredValueChanged<Circle>();

                drawingMenu.AddMItem("W Prediction", new Circle(false, Color.Red), (sender, args) =>
                {
                    _comboProvider.GetSkill<BrandW>().DrawPredictedW = args.GetNewValue<Circle>().Active;
                    _comboProvider.GetSkill<BrandW>().PredictedWColor = args.GetNewValue<Circle>().Color;
                });

                _drawW = drawingMenu.AddMItem("W Range", new Circle(true, Color.Red));
                _drawQ = drawingMenu.AddMItem("Q Range", new Circle(false, Color.OrangeRed));
                _drawE = drawingMenu.AddMItem("E Range", new Circle(false, Color.Goldenrod));
                _drawR = drawingMenu.AddMItem("R Range", new Circle(false, Color.DarkViolet));

                drawingMenu.ProcStoredValueChanged<Circle>();
                _mainMenu.AddToMainMenu();

                _comboProvider.Initialize();

                Game.OnUpdate += Tick;
                Drawing.OnDraw += Draw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error initialitzing TheBrand: " + ex);
            }
        }


        private void Draw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;

            var q = _drawQ.GetValue<Circle>();
            var w = _drawW.GetValue<Circle>();
            var e = _drawE.GetValue<Circle>();
            var r = _drawR.GetValue<Circle>();

            if (q.Active)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 1050, q.Color);
            if (w.Active)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 900, w.Color);
            if (e.Active)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 650, e.Color);
            if (r.Active)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 750, r.Color);
        }



        private void Tick(EventArgs args)
        {
            _comboProvider.Update();
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
