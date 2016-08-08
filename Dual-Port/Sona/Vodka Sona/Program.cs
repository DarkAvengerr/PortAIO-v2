using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;



using EloBuddy; 
 using LeagueSharp.Common; 
 namespace VodkaSona
{
    class Program
    {
        private static String championName = "Sona";
        public static AIHeroClient Player;
        private static Menu _menu;
        private static Orbwalking.Orbwalker _orbwalker;
        private static Spell Q, W, E, R;
        static Items.Item HealthPot;
        static Items.Item ManaPot;
        static SpellSlot IgniteSlot;


        public static void Game_OnLoad()
        {
            {
                Player = ObjectManager.Player;
                if (Player.ChampionName != "Sona") return;

                Q = new Spell(SpellSlot.Q, 850, TargetSelector.DamageType.Magical);
                W = new Spell(SpellSlot.W, 1000);
                E = new Spell(SpellSlot.E, 350);
                R = new Spell(SpellSlot.R, 1000, TargetSelector.DamageType.Magical);

                R.SetSkillshot(0.5f, 125, 3000f, false, SkillshotType.SkillshotLine);
            }

            _menu = new Menu("Vodka Sona", "vodka.sona", true);
            var orbwalkerMenu = new Menu(("Orbwalker"), "vodka.sona.orbwalker");
            _orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            _menu.AddSubMenu(orbwalkerMenu);

            Menu targetSelector = new Menu("Target Selector","vodka.sona.ts");
            TargetSelector.AddToMenu(targetSelector);
            _menu.AddSubMenu(targetSelector);



            var comboMenu = new Menu("Combo", "vodka.sona.combo");
            {
                comboMenu.AddItem(new MenuItem("vodka.sona.combo.useq", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("vodka.sona.combo.usew", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("vodka.sona.combo.usee", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("vodka.sona.combo.user", "Use R").SetValue(true));
                comboMenu.AddItem(new MenuItem("vodka.sona.combo.presR", "if ult will hit").SetValue(new Slider(2, 1, 5)));
            }
            _menu.AddSubMenu(comboMenu);

            var harrassMenu = new Menu("Harass", "vodka.sona.harrass");
            {
                harrassMenu.AddItem(new MenuItem("vodka.sona.harrassuseq", "Use Q").SetValue(true));
                harrassMenu.AddItem(new MenuItem("vodka.sona.harrassusew", "Use W").SetValue(true));
                harrassMenu.AddItem(new MenuItem("vodka.sona.harrassusee", "Use E").SetValue(true));

            }
            _menu.AddSubMenu(harrassMenu);

            var fleeMenu = new Menu("Flee", "vodka.sona.flee");
            {
                fleeMenu.AddItem(new MenuItem("vodka.sona.flee.fleekey", "FLEEEEEEEEEE! ").SetValue(new KeyBind('A', KeyBindType.Press)));
                fleeMenu.AddItem(new MenuItem("vodka.sona.flee.usew", "Use W").SetValue(true));
                fleeMenu.AddItem(new MenuItem("vodka.sona.flee.usee", "Use E").SetValue(true));

            }
            _menu.AddSubMenu(fleeMenu);

            var misc = new Menu("Misc", "vodka.sona.misc");
            {
                misc.AddItem(new MenuItem("vodka.sona.misc.healpro", "heal mates with x % health").SetValue(new Slider(50, 1)));
                misc.AddItem(new MenuItem("vodka.sona.misc.healmate", "heal only with x mates around").SetValue(new Slider(1, 0, 4)));
                misc.AddItem(new MenuItem("vodka.sona.misc.panic", "panic ult! ").SetValue(new KeyBind('T', KeyBindType.Press)));
                misc.AddItem(new MenuItem("vodka.sona.misc.packets", "use packets").SetValue(true));
                misc.AddItem(new MenuItem("vodka.sona.misc.exhaust", "use exhaust").SetValue(true));
            }
            _menu.AddSubMenu(misc);

            var drawingMenu = new Menu("Drawing", "vodka.sona.drawing");
            drawingMenu.AddItem(new MenuItem("DrawQ", "Draw Q range").SetValue(new Circle(true, Color.Aqua, Q.Range)));
            drawingMenu.AddItem(new MenuItem("DrawW", "Draw W range").SetValue(new Circle(true, Color.SpringGreen, W.Range)));
            drawingMenu.AddItem(new MenuItem("DrawE", "Draw E range").SetValue(new Circle(true, Color.SlateBlue, E.Range)));
            drawingMenu.AddItem(new MenuItem("DrawR", "Draw R range").SetValue(new Circle(true, Color.Red, R.Range)));
            _menu.AddSubMenu(drawingMenu);
            _menu.AddToMainMenu();

            Interrupter.OnPossibleToInterrupt += Interrupter_OnPossibleToInterrupt;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            ShowNotification("Vodka Sona - Loaded", 3000);

        }

        static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, InterruptableSpell spell)
        {
            if (!unit.IsValid || unit.IsDead || !unit.IsTargetable || unit.IsStunned) return;
            if (R.LSIsReady() && R.IsInRange(unit.Position) && spell.DangerLevel >= InterruptableDangerLevel.High)
            {
                R.Cast(unit.Position, true);
                return;
            }
            else
            {
                if (!_menu.Item("vodka.sona.misc.exhaust").GetValue<bool>()) return;
                if (unit.LSDistance(Player.Position) > 600) return;
                if (Player.LSGetSpellSlot("SummonerExhaust") != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(Player.LSGetSpellSlot("SummonerExhaust")) == SpellState.Ready)
                    Player.Spellbook.CastSpell(Player.LSGetSpellSlot("SummonerExhaust"), unit);
                if ((W.LSIsReady() && GetPassiveCount() == 2) || (Player.LSHasBuff("sonapassiveattack") && Player.LastCastedSpellName() == "SonaW" && W.LSIsReady() || (Player.LSHasBuff("sonapassiveattack") && W.LSIsReady())))
                {
                    W.Cast();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, unit);
                }
            }
        }
        
        // thanks to royal sona
        static int GetPassiveCount()
        {
            foreach (BuffInstance buff in Player.Buffs)
                if (buff.Name == "sonapassivecount") return buff.Count;
            return 0;
        }

        public static void ShowNotification(string message, int duration = -1, bool dispose = true)
        {
            Notifications.AddNotification(new Notification(message, duration, dispose));
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem1 = _menu.Item("DrawQ").GetValue<Circle>();
            var menuItem2 = _menu.Item("DrawE").GetValue<Circle>();
            var menuItem3 = _menu.Item("DrawW").GetValue<Circle>();
            var menuItem4 = _menu.Item("DrawR").GetValue<Circle>();

            if (menuItem1.Active && Q.LSIsReady()) Render.Circle.DrawCircle(Player.Position, Q.Range, Color.SpringGreen);
            if (menuItem2.Active && E.LSIsReady()) Render.Circle.DrawCircle(Player.Position, E.Range, Color.Crimson);
            if (menuItem3.Active && W.LSIsReady()) Render.Circle.DrawCircle(Player.Position, W.Range, Color.Aqua);
            if (menuItem4.Active && R.LSIsReady()) Render.Circle.DrawCircle(Player.Position, R.Range, Color.Firebrick);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (_menu.Item("vodka.sona.misc.panic").GetValue<KeyBind>().Active)
            {
                R.Cast(R.GetPrediction(TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical)).CastPosition, _menu.Item("vodka.sona.misc.packets").GetValue<bool>());
            }

            if (_menu.Item("vodka.sona.flee.fleekey").GetValue<KeyBind>().Active)
            {
                if (_menu.Item("vodka.sona.flee.usew").GetValue<bool>()) W.Cast();
                if (_menu.Item("vodka.sona.flee.usee").GetValue<bool>()) E.Cast();
            }
                

            switch (_orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    break;
            }
        }

        // laneclear not needed
        private static void Laneclear()
        {
            if (E.LSIsReady() && _menu.Item("vodka.sona.combo.usee").GetValue<bool>())
            {
                
            }
        }

        // da real combo intelligent
        private static void Combo()
        {
            bool vQ = Q.LSIsReady() && _menu.Item("vodka.sona.combo.useq").GetValue<bool>();
            bool vW = W.LSIsReady() && _menu.Item("vodka.sona.combo.usew").GetValue<bool>();
            bool vE = E.LSIsReady() && _menu.Item("vodka.sona.combo.usee").GetValue<bool>();
            bool vR = R.LSIsReady() && _menu.Item("vodka.sona.combo.user").GetValue<bool>();

            AIHeroClient tsQ = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            AIHeroClient tsR = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            if (vR && AlliesInRange(W.Range) >= 1 && tsR != null)
            {
                R.CastIfWillHit(tsR, _menu.Item("vodka.sona.combo.presR").GetValue<Slider>().Value, _menu.Item("vodka.sona.misc.packets").GetValue<bool>());
            }

            if (vQ && tsQ != null && Vector3.Distance(Player.Position, tsQ.Position) <= Q.Range)
            {
                Q.Cast();
            }

            if (vE)
            {
                UseESmart(TargetSelector.GetTarget(1700, TargetSelector.DamageType.Magical));
            }

            if (vW)
            {
                UseWSmart(_menu.Item("vodka.sona.misc.healpro").GetValue<Slider>().Value, _menu.Item("vodka.sona.misc.healmate").GetValue<Slider>().Value);
            }  
            
        }

        //Ty DEKTUS, copypasted as fuck :P
        public static void UseESmart(Obj_AI_Base target)
        {
            try
            {

                if (target.Path.Length == 0 || !target.IsMoving)
                    return;
                Vector2 nextEnemPath = target.Path[0].LSTo2D();
                var dist = Player.Position.LSTo2D().LSDistance(target.Position.LSTo2D());
                var distToNext = nextEnemPath.LSDistance(Player.Position.LSTo2D());
                if (distToNext <= dist)
                    return;
                var msDif = Player.MoveSpeed - target.MoveSpeed;
                if (msDif <= 0 && !Orbwalking.InAutoAttackRange(target))
                    E.Cast();

                var reachIn = dist/msDif;
                if (reachIn > 3)
                    E.Cast();
            }
            catch
            {
                
            }

        }

        static void UseWSmart(int percent, int count)
        {
            AIHeroClient ally = ImportantAllyInRange(W.Range);
            double wHeal = (10 + 20 * W.Level + .2 * Player.FlatMagicDamageMod) * (1 + (Player.Health / Player.MaxHealth) / 2);
            int allies = AlliesInRange(W.Range);

            if (allies >= count && (ally.Health / ally.MaxHealth) * 100 <= percent)
                W.Cast();
            if (allies < 2 && _menu.Item("vodka.sona.combo.usew").GetValue<bool>())
                if (_menu.Item("vodka.sona.combo.usew").GetValue<bool>() && Player.MaxHealth - Player.Health > wHeal)
                    W.Cast();
                else if ((Player.Health / Player.MaxHealth) * 100 <= percent) W.Cast(); ;
        }

        static AIHeroClient ImportantAllyInRange(float range)
        {
            float lastHealth = 9000f;
            AIHeroClient temp = new AIHeroClient();
            foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>())
                if (hero.IsAlly && !hero.IsMe && !hero.IsDead && Vector3.Distance(Player.Position, hero.Position) <= range && hero.Health < lastHealth)
                {
                    lastHealth = hero.Health;
                    temp = hero;
                }
            return temp;
        }

        static int AlliesInRange(float range)
        {
            int count = 0;
            foreach (AIHeroClient hero in ObjectManager.Get<AIHeroClient>())
                if (hero.IsAlly && !hero.IsMe && Vector3.Distance(Player.Position, hero.Position) <= range) count++;
            return count;
        }

    }
}