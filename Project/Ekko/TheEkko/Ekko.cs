using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using TheEkko;
using TheEkko.ComboSystem;
using TheEkko.Commons;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheEkko
{
    class Ekko : IMainContext
    {
        private Menu _mainMenu;
        private Orbwalking.Orbwalker _orbwalker;
        private ComboProvider _comboProvider;
        private MenuItem _drawR, _drawQ, _drawQEx;

        public void Load(EventArgs eArgs)
        {
            _comboProvider = new ComboProvider(new Skill[] { new EkkoQ(new Spell(SpellSlot.Q)), new EkkoW(new Spell(SpellSlot.W)), new EkkoE(new Spell(SpellSlot.E)), new EkkoR(new Spell(SpellSlot.R)) }.ToList(), 1000);

            _mainMenu = CreateMenu("The Ekko", true);
            var orbwalkerMenu = CreateMenu("Orbwalker", _mainMenu);
            var targetSelectorMenu = CreateMenu("Target Selector", _mainMenu);
            var comboMenu = CreateMenu("Combo", _mainMenu);
            var harassMenu = CreateMenu("Harass", _mainMenu);
            //var laneClear = CreateMenu("Laneclear", _mainMenu);
            var antiGapcloser = CreateMenu("Anti Gapcloser", _mainMenu);
            ManaManager.Initialize(_mainMenu, "Manamanager", true, false, false);
            IgniteManager.Initialize(_mainMenu);
            var drawingMenu = CreateMenu("Drawing", _mainMenu);
            var miscMenu = CreateMenu("Misc", _mainMenu);

            _orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            TargetSelector.AddToMenu(targetSelectorMenu);

            comboMenu.AddMItem("Use Q", true, (sender, args) => _comboProvider.SetEnabled<EkkoQ>(Orbwalking.OrbwalkingMode.Combo, args.GetNewValue<bool>()));
            comboMenu.AddMItem("Use W", true, (sender, args) => _comboProvider.SetEnabled<EkkoW>(Orbwalking.OrbwalkingMode.Combo, args.GetNewValue<bool>()));
            comboMenu.AddMItem("Use E", true, (sender, args) => _comboProvider.SetEnabled<EkkoE>(Orbwalking.OrbwalkingMode.Combo, args.GetNewValue<bool>()));
            comboMenu.AddMItem("Use R", true, (sender, args) => _comboProvider.SetEnabled<EkkoR>(Orbwalking.OrbwalkingMode.Combo, args.GetNewValue<bool>()));
            comboMenu.ProcStoredValueChanged<bool>();
            comboMenu.AddMItem("Min Ult Enemies", new Slider(3, 1, HeroManager.Enemies.Count));
            comboMenu.AddMItem("Min Ult Health %", new Slider(30));
            comboMenu.AddMItem("Ult for Save", true);

            harassMenu.AddMItem("Use Q", true, (sender, args) => _comboProvider.SetEnabled<EkkoQ>(Orbwalking.OrbwalkingMode.Mixed, args.GetNewValue<bool>()));
            harassMenu.ProcStoredValueChanged<bool>();

            //laneClear.AddMItem("Use Q", true, (sender, args) => _comboProvider.SetEnabled<EkkoQ>(Orbwalking.OrbwalkingMode.LaneClear, args.GetNewValue<bool>()));
            //laneClear.ProcStoredValueChanged<bool>();
            ////laneClear.AddMItem("Min Q Farm", new Slider(4, 1, 10), (sender,args) => _comboProvider.GetSkill<EkkoQ>().MinFarm = args.GetNewValue<Slider>().Value);


            var gapcloserSpells = CreateMenu("Enemies");
            _comboProvider.AddGapclosersToMenu(gapcloserSpells);
            antiGapcloser.AddSubMenu(gapcloserSpells);
            antiGapcloser.AddMItem("W on Gapcloser", true, (sender, args) => _comboProvider.GetSkill<EkkoW>().AntiGapcloser = args.GetNewValue<bool>()).ProcStoredValueChanged<bool>();


            _drawQ = drawingMenu.AddMItem("Draw Q", new Circle(true, Color.OrangeRed));
            _drawQEx = drawingMenu.AddMItem("Draw Q Ex", new Circle(false, Color.Yellow));
            _drawR = drawingMenu.AddMItem("Draw R", new Circle(true, Color.Red));

            drawingMenu.AddMItem("Damage indicator", new Circle(true, Color.Yellow), (sender, args) =>
            {
                DamageIndicator.Enabled = args.GetNewValue<Circle>().Active;
                DamageIndicator.Fill = true;
                DamageIndicator.FillColor = Color.FromArgb(100, args.GetNewValue<Circle>().Color);
                DamageIndicator.Color = Color.FromArgb(200, DamageIndicator.FillColor);
                DamageIndicator.DamageToUnit = _comboProvider.GetComboDamage;
            }).ProcStoredValueChanged<Circle>();

            miscMenu.AddMItem("When clearing harass if enemy near", true, (sender, args) => _comboProvider.GetSkills().ToList().ForEach(skill => skill.SwitchClearToHarassOnTarget = args.GetNewValue<bool>()));

            _mainMenu.AddToMainMenu();


            Game.OnUpdate += Update;
            Drawing.OnDraw += Draw;

            _comboProvider.Initialize(this);
        }

        private void Draw(EventArgs args)
        {
            var drawR = _drawR.GetValue<Circle>();
            var drawQ = _drawQ.GetValue<Circle>();
            var drawQEx = _drawQEx.GetValue<Circle>();

            if (drawR.Active)
            {
                var ekko = ObjectManager.Get<GameObject>().FirstOrDefault(item => item.Name == "Ekko_Base_R_TrailEnd.troy");
                if (ekko != null)
                    Render.Circle.DrawCircle(ekko.Position, 400, drawR.Color);
            }
            if (drawQ.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 700, drawQ.Color);
            }
            if (drawQEx.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 1000, drawQEx.Color);
            }
        }

        private void Update(EventArgs args)
        {
            _comboProvider.Update(this);
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

        public Menu GetRootMenu()
        {
            return _mainMenu;
        }

        public Orbwalking.Orbwalker GetOrbwalker()
        {
            return _orbwalker;
        }
    }
}
