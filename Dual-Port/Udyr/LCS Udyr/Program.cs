using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace LCS_Udyr
{
    class Program
    {
        public static Orbwalking.Orbwalker Orbwalker;
        public static Menu Config;
        public static AIHeroClient Udyr = ObjectManager.Player;
        public static Spell Q,W,E,R;

        public static void OnGameLoad()
        {
            Q = new Spell(SpellSlot.Q, Udyr.AttackRange);
            W = new Spell(SpellSlot.W, Udyr.AttackRange);
            E = new Spell(SpellSlot.E, Udyr.AttackRange);
            R = new Spell(SpellSlot.R, Udyr.AttackRange);

            Config = new Menu("LCS Series: Udyr", "LCS Series: Udyr", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu(":: Orbwalker Settings"));

                var comboMenu = new Menu(":: Combo Settings", ":: Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("q.combo", "Use (Q)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("w.combo", "Use (W)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("e.combo", "Use (E)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("r.combo", "Use (R)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("e.stun",  "Force (E)").SetValue(true)).SetTooltip("uses e for stun enemies");
                    comboMenu.AddItem(new MenuItem("combo.type", "Combo Type").SetValue(new StringList(new[] { "Phoenix -> Bear", "Bear -> Phoenix" })));
                    Config.AddSubMenu(comboMenu);
                }
                var jungleMenu = new Menu(":: Jungle Settings", ":: Jungle Settings");
                {
                    jungleMenu.AddItem(new MenuItem("q.jungle", "Use (Q)").SetValue(true));
                    jungleMenu.AddItem(new MenuItem("w.jungle", "Use (W)").SetValue(true));
                    jungleMenu.AddItem(new MenuItem("e.jungle", "Use (E)").SetValue(true));
                    jungleMenu.AddItem(new MenuItem("r.jungle", "Use (R)").SetValue(true));
                    jungleMenu.AddItem(new MenuItem("jungle.mana", "Min. Mana").SetValue(new Slider(50, 1, 99))).SetTooltip("manage your mana.");
                    Config.AddSubMenu(jungleMenu);
                }
                var clearMenu = new Menu(":: Clear Settings", ":: Clear Settings");
                {
                    clearMenu.AddItem(new MenuItem("q.clear", "Use (Q)").SetValue(true));
                    clearMenu.AddItem(new MenuItem("w.clear", "Use (W)").SetValue(true));
                    clearMenu.AddItem(new MenuItem("e.clear", "Use (E)").SetValue(false));
                    clearMenu.AddItem(new MenuItem("r.clear", "Use (R)").SetValue(true));
                    clearMenu.AddItem(new MenuItem("clear.mana", "Min. Mana").SetValue(new Slider(50, 1, 99))).SetTooltip("manage your mana.");
                    Config.AddSubMenu(clearMenu);
                }

                var miscMenu = new Menu(":: Misc Settings", ":: Misc Settings");
                {
                    CustomizableAntiGapcloser.AddToMenu(miscMenu.SubMenu("Customizable Antigapcloser"));
                    miscMenu.AddItem(new MenuItem("e.interrupter", "Use (E) for Interrupt").SetValue(true));
                    miscMenu.SubMenu("Customizable Antigapcloser").AddItem(new MenuItem("xxx", "                     General Settings").SetFontStyle(FontStyle.Bold));
                    miscMenu.SubMenu("Customizable Antigapcloser").AddItem(new MenuItem("e.antigapcloser", "Use (E) for Anti-Gapcloser").SetValue(true));
                    Config.AddSubMenu(miscMenu);
                }
                Config.AddToMainMenu();
            }

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            CustomizableAntiGapcloser.OnEnemyCustomGapcloser += OnEnemyCustomGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2OnOnInterruptableTarget;
        }

        private static void Interrupter2OnOnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && (sender.IsCastingInterruptableSpell() || sender.IsChannelingImportantSpell())
                && sender.LSIsValidTarget(E.Range) && Config.Item("e.interrupter").GetValue<bool>())
            {
                E.Cast();
            }
        }

        private static void OnEnemyCustomGapcloser(CActiveCGapcloser cGapcloser)
        {
            if (cGapcloser.Sender.IsEnemy && cGapcloser.End.LSDistance(Udyr.Position) < 100 && !cGapcloser.Sender.IsDead && E.LSIsReady()
                && Config.Item("e.antigapcloser").GetValue<bool>())
            {
                E.Cast();
            }
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is AIHeroClient && args.Target.IsValid
                && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                switch (Config.Item("combo.q.type").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        
                        if (Q.LSIsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                            && ((AIHeroClient)args.Target).LSIsValidTarget(Udyr.AttackRange) && !Udyr.IsTiger()
                            && Config.Item("q.combo").GetValue<bool>() && !R.LSIsReady())
                        {
                            Q.Cast();
                        }
                        if (W.LSIsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                            && ((AIHeroClient)args.Target).LSIsValidTarget(Udyr.AttackRange) && !Udyr.IsTurtle()
                            && Config.Item("w.combo").GetValue<bool>() && !R.LSIsReady())
                        {
                            W.Cast();
                        }
                        if (E.LSIsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                            && ((AIHeroClient)args.Target).LSIsValidTarget(Udyr.AttackRange) && !Udyr.IsBear()
                            && Config.Item("e.combo").GetValue<bool>() && !Config.Item("e.stun").GetValue<bool>()
                            && !R.LSIsReady())
                        {
                            E.Cast();
                        }
                        if (R.LSIsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                            && ((AIHeroClient)args.Target).LSIsValidTarget(Udyr.AttackRange) && !Udyr.IsPhoenix()
                            && Config.Item("r.combo").GetValue<bool>())
                        {
                            R.Cast();
                        }   

                        break;
                    case 1:
                         if (Q.LSIsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                            && ((AIHeroClient)args.Target).LSIsValidTarget(Udyr.AttackRange) && !Udyr.IsTiger()
                            && Config.Item("q.combo").GetValue<bool>() && !E.LSIsReady())
                        {
                            Q.Cast();
                        }
                        if (W.LSIsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                            && ((AIHeroClient)args.Target).LSIsValidTarget(Udyr.AttackRange) && !Udyr.IsTurtle()
                            && Config.Item("w.combo").GetValue<bool>() && !E.LSIsReady())
                        {
                            W.Cast();
                        }
                        if (E.LSIsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                            && ((AIHeroClient)args.Target).LSIsValidTarget(Udyr.AttackRange) && !Udyr.IsBear()
                            && Config.Item("e.combo").GetValue<bool>() && !Config.Item("e.stun").GetValue<bool>())
                        {
                            E.Cast();
                        }
                        if (R.LSIsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                            && ((AIHeroClient)args.Target).LSIsValidTarget(Udyr.AttackRange) && !Udyr.IsPhoenix()
                            && Config.Item("r.combo").GetValue<bool>() && !E.LSIsReady())
                        {
                            R.Cast();
                        }  
                        break;
                }
            }

            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is Obj_AI_Minion && 
                args.Target.IsValid && ((Obj_AI_Minion)args.Target).Team == GameObjectTeam.Neutral &&
                Udyr.ManaPercent >= Config.Item("jungle.mana").GetValue<Slider>().Value)
            {
                if (Q.LSIsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear 
                    && ((Obj_AI_Minion)args.Target).LSIsValidTarget(Udyr.AttackRange) && !Udyr.IsTiger()
                    && Config.Item("q.jungle").GetValue<bool>())
                {
                    Q.Cast();
                }
                if (W.LSIsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear 
                    && ((Obj_AI_Minion)args.Target).LSIsValidTarget(Udyr.AttackRange) && !Udyr.IsTurtle()
                    && Config.Item("w.jungle").GetValue<bool>() && ObjectManager.Player.Buffs.Any(buff => buff.Name != "UdyrPhoenixStance"))
                {
                    W.Cast();
                }
                if (E.LSIsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear 
                    && ((Obj_AI_Minion)args.Target).LSIsValidTarget(Udyr.AttackRange) && !Udyr.IsBear()
                    && Config.Item("e.jungle").GetValue<bool>())
                {
                    E.Cast();
                }
                if (R.LSIsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                    && ((Obj_AI_Minion)args.Target).LSIsValidTarget(Udyr.AttackRange) &&
                    ObjectManager.Player.Buffs.Any(buff => buff.Name != "UdyrPhoenixStance") && !Udyr.IsPhoenix()
                    && Config.Item("r.jungle").GetValue<bool>())
                {
                    R.Cast();
                }
            }

            if (sender.IsMe && Orbwalking.IsAutoAttack(args.SData.Name) && args.Target is Obj_AI_Minion &&
                args.Target.IsValid && ((Obj_AI_Minion)args.Target).IsEnemy && Udyr.ManaPercent >= Config.Item("clear.mana").GetValue<Slider>().Value)
            {
                if (Q.LSIsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                    && ((Obj_AI_Minion)args.Target).LSIsValidTarget(Udyr.AttackRange) && !Udyr.IsTiger()
                    && Config.Item("q.clear").GetValue<bool>())
                {
                    Q.Cast();
                }
                if (W.LSIsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                    && ((Obj_AI_Minion)args.Target).LSIsValidTarget(Udyr.AttackRange) && !Udyr.IsTurtle()
                    && Config.Item("w.clear").GetValue<bool>() && ObjectManager.Player.Buffs.Any(buff => buff.Name != "UdyrPhoenixStance"))
                {
                    W.Cast();
                }
                if (E.LSIsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                    && ((Obj_AI_Minion)args.Target).LSIsValidTarget(Udyr.AttackRange) && !Udyr.IsBear()
                    && Config.Item("e.clear").GetValue<bool>())
                {
                    E.Cast();
                }
                if (R.LSIsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                    && ((Obj_AI_Minion)args.Target).LSIsValidTarget(Udyr.AttackRange) &&
                    ObjectManager.Player.Buffs.Any(buff => buff.Name != "UdyrPhoenixStance") && !Udyr.IsPhoenix()
                    && Config.Item("r.clear").GetValue<bool>())
                {
                    R.Cast();
                }
            }
 
        }

        private static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
            }
        }

        private static void Combo()
        {
            if (E.LSIsReady() && Config.Item("e.stun").GetValue<bool>() && Config.Item("e.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x=> x.LSIsValidTarget(E.Range * 2) && !Udyr.IsBear()
                    && !x.HasBearPassive()))
                {
                    E.Cast();
                }
            }
        }

    }
}
