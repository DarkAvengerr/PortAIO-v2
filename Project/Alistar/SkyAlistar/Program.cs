using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;
using System.Drawing;
using System.Runtime.Remoting.Channels;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace AlistarBySky97
{
    class Program
    {
        private static String championName = "Alistar";
        public static AIHeroClient Player;
        private static Menu _Menu;
        private static Menu DrawsManager;
        private static Menu AbilitiesManager;
        private static Menu OrbwalkerMenu;
        private static Orbwalking.Orbwalker Orbwalker;
        private static int Eslidervalue = 0;
        private static AIHeroClient CurrentTarget;
        private static HeroManager Enemies;
        
        
        

        public static Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>()
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q) },
            { SpellSlot.W, new Spell(SpellSlot.W, 650f) },
            { SpellSlot.E, new Spell(SpellSlot.E, 575f) },
            { SpellSlot.R, new Spell(SpellSlot.R) }
        };

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void Game_OnDraw(EventArgs args)
        {
            if (_Menu.Item("AlistarScriptSky.DrawsManager.rangeE").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, spells[SpellSlot.E].Range, System.Drawing.Color.Chartreuse, 1, false);
            }
            if (_Menu.Item("AlistarScriptSky.DrawsManager.rangeW").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, spells[SpellSlot.W].Range, System.Drawing.Color.Red, 1, false);
            }
            if (_Menu.Item("AlistarScriptSky.DrawsManager.QWComboFixer").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, ((spells[SpellSlot.W].Range/100)*_Menu.Item("AlistarScriptSky.AbilitiesManager.QWComboFixer").GetValue<Slider>().Value), System.Drawing.Color.Red, 1, false);
            }
        }


        private static void OnLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.ChampionName != championName)
            {
                return;
            }
            _Menu = new Menu("Alistar Script by Sky97", "AlistarScriptSky", true);
            DrawsManager = new Menu("Drawings settings", "AlistarScriptSky.DrawsManager");
            {
                DrawsManager.AddItem(new MenuItem("AlistarScriptSky.DrawsManager.rangeE", "Display E Range").SetValue(true));
                DrawsManager.AddItem(new MenuItem("AlistarScriptSky.DrawsManager.rangeW", "Display W Range").SetValue(true));
                DrawsManager.AddItem(new MenuItem("AlistarScriptSky.DrawsManager.QWComboFixer", "Display QWComboFixer Range").SetValue(true));
                _Menu.AddSubMenu(DrawsManager);
            }

            AbilitiesManager = new Menu("Manage Abilites and Combos", "AlistarScriptSky.AbilitiesManager");
            {
                AbilitiesManager.AddItem(new MenuItem("AlistarScriptSky.AbilitiesManager.AutoHealAllies", "Auto Heal Allies nearby With Hp below x (in percent)").SetValue(new Slider(0, 0, 100)));
                AbilitiesManager.AddItem(new MenuItem("AlistarScriptSky.AbilitiesManager.QWComboFixer", "Fixes the minimal range for the W jump (in percent)").SetValue(new Slider(0, 0, 100)));
                AbilitiesManager.AddItem(new MenuItem("AlistarScriptSky.AbilitiesManager.Antigapcloser", "Antigapcloser").SetValue(true));
                AbilitiesManager.AddItem(new MenuItem("AlistarScriptSky.AbilitiesManager.Interrupt", "Interrupt").SetValue(true));
                _Menu.AddSubMenu(AbilitiesManager);
            }

            OrbwalkerMenu = new Menu("Orbwalkermenu", "AlistarScriptSky.OrbwalkerMenu");
            {
                Orbwalker = new Orbwalking.Orbwalker(OrbwalkerMenu);
                _Menu.AddSubMenu(OrbwalkerMenu);
            }
            _Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Game_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += OnGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterrupter;
        }

        private static void OnInterrupter(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsValidTarget(spells[SpellSlot.Q].Range)
                && args.DangerLevel >= Interrupter2.DangerLevel.High 
                && spells[SpellSlot.Q].IsReady() 
                && _Menu.Item("AlistarScriptSky.AbilitiesManager.Interrupt").GetValue<bool>())
            {
                spells[SpellSlot.Q].Cast();
            }
                 
        }

        private static void OnGapcloser(ActiveGapcloser gapcloser)
        {
            if (_Menu.Item("AlistarScriptSky.AbilitiesManager.Antigapcloser").GetValue<bool>() &&
                spells[SpellSlot.W].IsReady()
                && gapcloser.Sender.IsValidTarget(400))
            {
                spells[SpellSlot.W].Cast(gapcloser.Sender);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    AlistarQWCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    AlistarE();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:

                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:

                    break;
                default:
                    return;
            }
            

        }



        private static void AlistarE()
        {
            if (ObjectManager.Player.GetAlliesInRange(spells[SpellSlot.E].Range).Any(hero => hero.HealthPercent < _Menu.Item("AlistarScriptSky.AbilitiesManager.AutoHealAllies").GetValue<Slider>().Value))
            {

                if (spells[SpellSlot.E].IsReady())
                {
                    spells[SpellSlot.E].Cast();

                }
            }
        }

        private static void AlistarQWCombo()
        {
            //QW Combo
            if (spells[SpellSlot.W].IsReady() && Player.Mana>=(spells[SpellSlot.W].ManaCost+spells[SpellSlot.Q].ManaCost))
            {
                CurrentTarget = TargetSelector.GetTarget(spells[SpellSlot.W].Range, TargetSelector.DamageType.Magical);
                //QWComboFixer Integration
                if (CurrentTarget.IsValidTarget() && CurrentTarget.Distance(ObjectManager.Player)>= (spells[SpellSlot.W].Range/100)*_Menu.Item("AlistarScriptSky.AbilitiesManager.QWComboFixer").GetValue<Slider>().Value)
                {
                    
                    if (spells[SpellSlot.Q].IsReady())
                    {
                        spells[SpellSlot.W].Cast(CurrentTarget);
                    }
                   
                }
                
            }
            if (!spells[SpellSlot.W].IsReady() && spells[SpellSlot.Q].IsReady())
            {
                if (ObjectManager.Player.CountEnemiesInRange(spells[SpellSlot.Q].Range)>=1)
                {
                    spells[SpellSlot.Q].Cast();
                }
            }
            
            //Save the alistar!
            if (ObjectManager.Player.HealthPercent < 25 && ObjectManager.Player.CountEnemiesInRange(1500f) >= 2)
            {
                if (spells[SpellSlot.R].IsReady())
                {
                    spells[SpellSlot.R].Cast();
                }
            }

        }


    }
}
