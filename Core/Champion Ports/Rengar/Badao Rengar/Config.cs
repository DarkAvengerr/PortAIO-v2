using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoRengar
{
    public static class Config
    {
        public static Menu Menu, Modes, Draw, Magnet, Targetting, Combo, Harass, LaneClear, JungleClear, Auto, Assasinate;
        private static int _lastSwitchTick , _lastAssasinateSwitch;
        private static List<string> heroList = new List<string>();
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static void BadaoActivate()
        {
            Variables.Q = new Spell(SpellSlot.Q,625);
            Variables.Q.SetSkillshot(0.25f, 70, float.MaxValue,false, SkillshotType.SkillshotLine);
            Variables.W = new Spell(SpellSlot.W, 300);
            Variables.E = new Spell(SpellSlot.E, 1000);
            Variables.R = new Spell(SpellSlot.R);
            Variables.E.SetSkillshot(0.25f, 70, 1500, true, SkillshotType.SkillshotLine);
            Variables.E.MinHitChance = HitChance.Medium;
            Variables.E2 = new Spell(SpellSlot.E, 1000);
            Variables.E2.SetSkillshot(0.25f, 70, 1500, true, SkillshotType.SkillshotLine);
            Variables.E2.MinHitChance = HitChance.Medium;
            Variables.W.SetSkillshot(0.25f, 500, 2000, false, SkillshotType.SkillshotCircle);
            Variables.W.MinHitChance = HitChance.Medium;
            foreach (var spell in
                        Player.Spellbook.Spells.Where(
                          i =>
                                i.Name.ToLower().Contains("smite") &&
            (i.Slot == SpellSlot.Summoner1 || i.Slot == SpellSlot.Summoner2)))
            {
                Variables.Smite = spell.Slot;
            }



            Menu = new Menu("The Lazy Cat", Player.ChampionName, true);

            var orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Variables.Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);
            var ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);

            // Modes
            Modes = Menu.AddSubMenu(new Menu("Modes", "modes"));

            // Combo
            Combo = Modes.AddSubMenu(new Menu("Combo", "Combo"));
            Variables.ComboSmite = Combo.AddItem(new MenuItem("comboUseSmite", "Use Smite").SetValue(true));
            Variables.ComboMode = Combo.AddItem(new MenuItem("comboMode", "Combo Mode").SetValue(new StringList(new[] { "Q","W","E" }, 0)));
            Variables.ComboSwitchKey = Combo.AddItem(new MenuItem("ComboSwitch", "ComboModeSwitch").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            //Harass
            //Harass = Modes.AddSubMenu(new Menu("Harass", "Harass"));
            //Variables.HarassW = Harass.AddItem(new MenuItem("harassUseW", "Use W").SetValue(true));
            //Variables.HarassE = Harass.AddItem(new MenuItem("harassUseE", "Use E").SetValue(true));

            //Assasinate
            Assasinate = Modes.AddSubMenu(new Menu("Assasinate", "Assasinate"));
            Variables.AssasinateInstruction = Assasinate.AddItem(new MenuItem("AssasinateInstruction", "Only Works On Selected Target"));
            Variables.AssassinateKey = Assasinate.AddItem(new MenuItem("AssassinateKey", "Assassinate Key").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Press)));
            Variables.AssasinateSwitchKey = Assasinate.AddItem(new MenuItem("AssasinateSwitchKey", "Switch Target Key").SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Press)));
            foreach (var hero in HeroManager.Enemies)
            {
                heroList.Add(hero.ChampionName + "(" + hero.Name + ")");
            }
            Variables.AssasinateTarget = Assasinate.AddItem(new MenuItem("AssasinateTarget", "Assasinate Target").SetValue(new StringList(heroList.ToArray())));

            //LaneClear
            LaneClear = Modes.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Variables.LaneQ = LaneClear.AddItem(new MenuItem("laneUseQ", "Use Q").SetValue(true));
            Variables.LaneW = LaneClear.AddItem(new MenuItem("laneUseW", "Use W").SetValue(true));
            Variables.LaneE = LaneClear.AddItem(new MenuItem("laneUseE", "Use E").SetValue(true));
            Variables.LaneTiamat = LaneClear.AddItem(new MenuItem("laneUseTiamat", "Use Tiamat/Hydra").SetValue(true));

            //JungleClear
            JungleClear = Modes.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            Variables.JungQ = JungleClear.AddItem(new MenuItem("jungUseQ", "Use Q").SetValue(true));
            Variables.JungW = JungleClear.AddItem(new MenuItem("jungUseW", "Use W").SetValue(true));
            Variables.JungE = JungleClear.AddItem(new MenuItem("jungUseE", "Use E").SetValue(true));
            Variables.JungTiamat = JungleClear.AddItem(new MenuItem("jungUseTiamat", "Use Tiamat/Hydra").SetValue(true));

            //Auto
            Auto = Modes.AddSubMenu(new Menu("Auto", "Auto"));
            Variables.AutoEInterrupt = Auto.AddItem(new MenuItem("autoEInterrupt", "Interrupt with E").SetValue(true));
            Variables.AutoSmiteKS = Auto.AddItem(new MenuItem("autoSmiteKS", "Smite KS(blue / red)").SetValue(true));
            Variables.AutoESK = Auto.AddItem(new MenuItem("autoEKS", "E Ks").SetValue(true));
            Variables.AutoWKS = Auto.AddItem(new MenuItem("autoWKS", "W Ks").SetValue(true));
            Variables.AutoSmiteSteal = Auto.AddItem(new MenuItem("autoSteal", "Smite steal Drake/Baron").SetValue(true));

            //drawing
            Draw = Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Variables.DrawMode = Draw.AddItem(new MenuItem("drawMode", "Draw Mode").SetValue(true));
            Variables.DrawAssasinate = Draw.AddItem(new MenuItem("DrawAssasinate", "Draw Assasinate Target").SetValue(true));

            //magnet
            Magnet = Menu.AddSubMenu(new Menu("Magnet", "Magnet"));
            Magnet.AddItem(new MenuItem("MagnetInstruction", "Magnet Only Works On Selected Target"));
            Variables.MagnetEnable = Magnet.AddItem(new MenuItem("magnetEnable", "Enable").SetValue(true));
            Variables.MagnetRange = Magnet.AddItem(new MenuItem("magnetRange", "Magnet Range").SetValue(new Slider(300, 150, 500)));

            Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            ComboModeSwitch();
            AssasinateSwitch();
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (Variables.DrawMode.GetValue<bool>())
            {
                var x = Drawing.WorldToScreen(Player.Position);
                Drawing.DrawText(x[0], x[1], Color.White, Variables.ComboMode.GetValue<StringList>().SelectedValue);
            }
        }
        private static void ComboModeSwitch()
        {
            var comboMode = Variables.ComboMode.GetValue<StringList>().SelectedValue;
            var lasttime = Utils.GameTimeTickCount - _lastSwitchTick;
            if (!Variables.ComboSwitchKey.GetValue<KeyBind>().Active ||
                lasttime <= Game.Ping)
            {
                return;
            }
            switch (comboMode)
            {
                case "Q":
                    Variables.ComboMode.SetValue(new StringList(new[] { "Q", "W", "E" }, 1));
                    _lastSwitchTick = Utils.GameTimeTickCount + 300;
                    break;
                case "W":
                    Variables.ComboMode.SetValue(new StringList(new[] { "Q", "W", "E" }, 2));
                    _lastSwitchTick = Utils.GameTimeTickCount + 300;
                    break;
                case "E":
                    Variables.ComboMode.SetValue(new StringList(new[] { "Q", "W", "E" }, 0));
                    _lastSwitchTick = Utils.GameTimeTickCount + 300;
                    break;
            }
        }
        private static void AssasinateSwitch()
        {
            int TargetIndex = Variables.AssasinateTarget.GetValue<StringList>().SelectedIndex;
            int Index = Variables.AssasinateTarget.GetValue<StringList>().SList.Count() - 1;
            var lastTime = Utils.GameTimeTickCount - _lastAssasinateSwitch;
            if (!Variables.AssasinateSwitchKey.GetValue<KeyBind>().Active ||
                lastTime <= Game.Ping)
            {
                return;
            }
            int NextIndex = TargetIndex + 1 > Index ? 0 : TargetIndex + 1;
            Variables.AssasinateTarget.SetValue(new StringList(heroList.ToArray(),NextIndex));
            _lastAssasinateSwitch = Utils.GameTimeTickCount + 300;
        }
    }
}
