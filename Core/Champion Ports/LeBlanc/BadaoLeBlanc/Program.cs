using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoLeblanc
{
    class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Orbwalking.Orbwalker Orbwalker;

        public static Spell Q, W, E, R;

        public static Menu Menu;

        public static int Rstate, Wstate, Ecol;
        public static string QName = "LeblancQ", WName = "LeblancW", EName = "LeblancE", RName = "LeblancRToggle", QRName = "",
            WRName = "", ERName = "", RRName = "LeblancR", W2Name = "LeblancWReturn", RW2Name = "LeblancRWReturn";

        public static void Main()
        {
            Game_OnGameLoad();
        }

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Leblanc")
                return;

            Q = new Spell(SpellSlot.Q, 710);
            W = new Spell(SpellSlot.W, 750);
            E = new Spell(SpellSlot.E, 950);
            R = new Spell(SpellSlot.R);

            //Q.SetSkillshot(300, 50, 2000, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 70, 1600, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0, 70, 1500, false, SkillshotType.SkillshotLine);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);

            Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));
            spellMenu.AddItem(new MenuItem("Use Q Harass", "Use Q Harass").SetValue(true));
            spellMenu.AddItem(new MenuItem("Use W Harass", "Use W Harass").SetValue(true));
            spellMenu.AddItem(new MenuItem("Use W Back Harass", "Use W Back Harass").SetValue(true));
            spellMenu.AddItem(new MenuItem("Use W Combo", "Use W Combo").SetValue(true));
            spellMenu.AddItem(new MenuItem("Rmode", "R mode").SetValue(new StringList (new string[] { "Q","W","E","Any"},3)));
            spellMenu.AddItem(new MenuItem("force focus selected", "force focus selected").SetValue(false));
            spellMenu.AddItem(new MenuItem("if selected in :", "if selected in :").SetValue(new Slider(1000, 1000, 1500)));
            spellMenu.AddItem(new MenuItem("QE Selected Target", "QE Selected Target").SetValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Press)));

            Menu.AddToMainMenu();

            //Drawing.OnDraw += Drawing_OnDraw;

            Game.OnUpdate += Game_OnGameUpdate;


            Chat.Print("Leblanc by badao updated 10/11/2016!, updated for Lb REWORK!");

            Chat.Print("Visist forum for more information");

            Chat.Print("Leave an upvote in database if you like it <3!");
        }
        public static int Rmode { get{ return Menu.Item("Rmode").GetValue<StringList>().SelectedIndex; } }
        public static bool WgapCombo { get { return Menu.Item("Use W Combo Gap").GetValue<bool>(); } }
        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (Menu.Item("Use Q Harass").GetValue<bool>())
                {
                    useQ();
                }
                if (Menu.Item("Use W Harass").GetValue<bool>())
                {
                    useWH();
                }
                if (Menu.Item("Use W Back Harass").GetValue<bool>())
                {
                    useWBH();
                }

            }
            if (Menu.Item("QE Selected Target").GetValue<KeyBind>().Active)
            {
                useQE();
            }
        }

        public static bool Selected()
        {
            if (!Menu.Item("force focus selected").GetValue<bool>())
            {
                return false;
            }
            else
            {
                var target = TargetSelector.GetSelectedTarget();
                float a = Menu.Item("if selected in :").GetValue<Slider>().Value;
                if (target == null || target.IsDead || target.IsZombie)
                {
                    return false;
                }
                else
                {
                    if (Player.Distance(target.Position) > a)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }

        public static void useQ()
        {
            if (Selected())
            {
                var target = TargetSelector.GetSelectedTarget();
                if (target != null && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }

            }
            else
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (target != null && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }
            }
        }

        public static void useE()
        {
            if (Selected())
            {
                var target = TargetSelector.GetSelectedTarget();
                if (target != null && target.IsValidTarget(E.Range))
                {
                    CastE(target);
                }

            }
            else
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical) ??
                             TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                if (target != null && target.IsValidTarget(E.Range))
                {
                    CastE(target);
                }
            }
        }
        public static void useW()
        {
            if (Menu.Item("Use W Combo").GetValue<bool>())
            {
                if (Selected())
                {
                    var target = TargetSelector.GetSelectedTarget();
                    if (target != null && target.IsValidTarget(W.Range))
                    {
                        CastW(target);
                    }

                }
                else
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                    if (target != null && target.IsValidTarget(W.Range))
                    {
                        CastW(target);
                    }
                }
            }
        }

        public static void useWH()
        {
            if (Selected())
            {
                var target = TargetSelector.GetSelectedTarget();
                if (target != null && target.IsValidTarget(W.Range))
                {
                    CastW(target);
                }

            }
            else
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                if (target != null && target.IsValidTarget(W.Range))
                {
                    CastW(target);
                }
            }
        }

        public static void useWBH()
        {
            if (W.Instance.Name == W2Name)
                W.Cast();
        }

        public static void useR()
        {
            float range = Q.Range;
            if (Rmode == 2)
                range = W.Range;
            if (Rmode == 3)
                range = E.Range;
            if (Selected())
            {
                var target = TargetSelector.GetSelectedTarget();
                if (target != null && target.IsValidTarget(range))
                {
                    CastR(target);
                }

            }
            else
            {
                var target = TargetSelector.GetTarget(range, TargetSelector.DamageType.Magical);
                if (target != null && target.IsValidTarget(range))
                {
                    CastR(target);
                }
            }
        }

        public static void CastR(Obj_AI_Base target)
        {
            if (R.Instance.Name == RName && R.IsReady())
            {
                if (Q.IsReady())
                {
                    return;
                }
                R.Cast();
            }
            if (R.Instance.Name == RRName)
            {
                if (Rmode == 0 && target != null && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }
                if (Rmode == 1 && target != null && target.IsValidTarget(W.Range))
                {
                    CastW(target);
                }
                if (Rmode == 2 && target != null && target.IsValidTarget(E.Range))
                {
                    CastE(target);
                }
                if (Rmode == 3 && target != null && target.IsValidTarget(E.Range))
                {
                    CastE(target);
                    CastW(target);
                    Q.Cast(target);
                }
            }
        }

        public static void CastW(Obj_AI_Base target)
        {
            if (!W.IsReady() || W.Instance.Name == W2Name)
                return;
            var t = Prediction.GetPrediction(target, 400).CastPosition;
            float x = target.MoveSpeed;
            float y = x * 400 / 1000;
            var pos = target.Position;
            if (target.Distance(t) <= y)
            {
                pos = t;
            }
            if (target.Distance(t) > y)
            {
                pos = target.Position.Extend(t, y - 50);
            }
            if (Player.Distance(pos) <= 600)
            {
                W.Cast(pos);
            }
            if (Player.Distance(pos) > 600)
            {
                if (target.Distance(t) > y)
                {
                    var pos2 = target.Position.Extend(t, y);
                    if (Player.Distance(pos2) <= 600)
                    {
                        W.Cast(pos2);
                    }
                    else
                    {
                        var prediction = W.GetPrediction(target);
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            var pos3 = prediction.CastPosition;
                            var pos4 = Player.Position.Extend(pos3, 600);
                            W.Cast(pos4);
                        }
                    }

                }
            }
        }

        public static void CastE(Obj_AI_Base target)
        {
            if (E.IsReady() && !Player.IsDashing())
            {
                E.Cast(target);
            }
        }
        public static void Combo()
        {
            if (R.Instance.Name != RRName || R.Instance.Level < 1)
            {
                useE();
                useQ();
                useW();
            }
            useR();
        }

        public static void useQE()
        {
            Orbwalking.MoveTo(Game.CursorPos);
                var target = TargetSelector.GetSelectedTarget();
                if (target != null && target.IsValidTarget() && !target.IsZombie)
                {
                    if( Player.Distance(target.Position) <= Q.Range)
                    {
                        Q.Cast(target);
                    }
                    if (Player.Distance(target.Position) <= E.Range)
                    {
                        E.Cast(target);
                    }
                } 
        }
    }
}
