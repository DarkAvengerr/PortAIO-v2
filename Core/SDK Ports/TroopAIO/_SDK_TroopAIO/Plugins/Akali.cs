using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.UI;
using LeagueSharp.SDK.Utils;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Menu = LeagueSharp.SDK.UI.Menu;


using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace _SDK_TroopAIO.Plugins






{
    internal class Akali : Program
    {


        public static Spell Q, E, R;
        internal string comb = "   ";
        private static Items.Item hextech;
        private static Items.Item cutlass;


        public Akali()

        //Summs
        {
            Q = new Spell(SpellSlot.Q, 600f);
            E = new Spell(SpellSlot.E, 310f);
            R = new Spell(SpellSlot.R, 700f);

            var Key = Menu.Add(new Menu("Key", "Key"));
            {
                Key.Add(new MenuKeyBind("Combo", "Combo", System.Windows.Forms.Keys.Space, KeyBindType.Press));
                Key.Add(new MenuKeyBind("Harass", "Harass", System.Windows.Forms.Keys.C, KeyBindType.Press));
                Key.Add(new MenuKeyBind("LaneClear", "LaneClear", System.Windows.Forms.Keys.V, KeyBindType.Press));
                Key.Add(new MenuKeyBind("LastHit", "LastHit", System.Windows.Forms.Keys.X, KeyBindType.Press));
            }

            var Combo = Menu.Add(new Menu("Combo", "Combo"));
            {

                Combo.Add(new MenuBool("UseQCombo", "Use Q", true));

                Combo.Add(new MenuBool("UseECombo", "Use E", true));

                Combo.Add(new MenuBool("UseRCombo", "Use R", true));

            }

            var Harass = Menu.Add(new Menu("Harass", "Harass"));
            {
                Harass.Add(new MenuBool("HarassUseQ", "Use Q to Harass", true));
            }

            var Misc = Menu.Add(new Menu("Misc", "Misc"));
            {
                Misc.Add(new MenuBool("RGapClose", "Use R to anti-gaplcose", true));
            }

            var LastHit = Menu.Add(new Menu("LastHit", "LastHit"));
            {
                LastHit.Add(new MenuBool("qlh", "Use Q to Lasthit", true));
            }

            var LaneClear = Menu.Add(new Menu("LaneClear", "LaneClear"));
            {
                LaneClear.Add(new MenuBool("useelc", "Use E to laneclear", true));
            }

            var Draw = Menu.Add(new Menu("Draw", "Draw"));
            {
                Draw.Add(new MenuBool("DrawQ", "Draw Q Range"));
                Draw.Add(new MenuBool("DrawE", "Draw E Range"));
                Draw.Add(new MenuBool("DrawR", "Draw R Range"));
            }
            hextech = new Items.Item(3146, 700);
            cutlass = new Items.Item(3144, 450);

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Events.OnGapCloser += OnGapCloser;
            Variables.Orbwalker.OnAction += Orbwalker_OnAction;

        }


        private void Game_OnUpdate(EventArgs args)
        {
            try
            {
                if (Me.IsDead)
                    return;

                if (Menu["Key"]["Combo"].GetValue<MenuKeyBind>().Active)
                {
                    Combo();
                }

                if (Menu["Key"]["Harass"].GetValue<MenuKeyBind>().Active)
                {
                    Harass();
                }

                if (Menu["Key"]["LastHit"].GetValue<MenuKeyBind>().Active)
                {
                    LastHit();
                }

                if (Menu["Key"]["LaneClear"].GetValue<MenuKeyBind>().Active)
                {
                    LaneClear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void Combo()
        {
            {
                var en = Variables.TargetSelector.GetTarget(700, DamageType.Magical);

                var ComboQ = Menu["Combo"]["UseQCombo"].GetValue<MenuBool>();
                var ComboR = Menu["Combo"]["UseRCombo"].GetValue<MenuBool>();

                if (en != null && Me.Distance(en) <= hextech.Range)
                {
                    hextech.Cast(en);
                }
                if (en != null && Me.Distance(en) <= cutlass.Range)
                {
                    cutlass.Cast(en);
                }


                if (ComboQ && Q.IsReady() && en.IsValidTarget(Q.Range))
                {
                    Q.Cast(en);
                }

                if (ComboR && R.IsReady() && en.IsValidTarget(R.Range))
                {
                    if (!en.IsUnderEnemyTurret())
                    {
                        R.Cast(en);
                    }
                }
            }
        }

        private void Harass()
        {
            var HT = Variables.TargetSelector.GetTarget(700, DamageType.Magical);
            var HarassQ = Menu["Harass"]["HarassUseQ"].GetValue<MenuBool>();

            if (HarassQ && Q.IsReady() && HT.IsValidTarget(Q.Range))
            {
                Q.Cast(HT);
            }

        }

        private void LastHit()
        {
            if (GameObjects.EnemyMinions.Count() != 0 && (Menu["LastHit"]["qlh"].GetValue<MenuBool>() && Q.IsReady()))
            {
                Obj_AI_Minion minion =
                    GameObjects.EnemyMinions.FirstOrDefault(m => m.Distance(Me) <= Q.Range && m.Health <= Q.GetDamage(m));
                Q.Cast(minion);
            }
        }


        private void Drawing_OnDraw(EventArgs args)
        {
            try
            {
                if (Me.IsDead)
                    return;

                if (Q.IsReady() && Menu["Draw"]["DrawQ"].GetValue<MenuBool>())
                    Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.DeepPink);

                if (E.IsReady() && Menu["Draw"]["DrawE"].GetValue<MenuBool>())
                    Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.AliceBlue);

                if (R.IsReady() && Menu["Draw"]["DrawR"].GetValue<MenuBool>())
                    Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.Goldenrod);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }



        private void Orbwalker_OnAction(object sender, OrbwalkingActionArgs args)
        {
            try
            {
                if (args.Type == OrbwalkingType.OnAttack && Menu["Key"]["Combo"].GetValue<MenuKeyBind>().Active && (args.Target is AIHeroClient))
                {
                    if (Menu["Combo"]["UseECombo"].GetValue<MenuBool>() && E.IsReady())
                    {
                        E.CastOnUnit(args.Target as AIHeroClient);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void LaneClear()
        {
            try
            {
                if (Menu["Key"]["LaneClear"].GetValue<MenuKeyBind>().Active)
                    if (GameObjects.EnemyMinions.Where(mins => mins.Position.DistanceToPlayer() < E.Range).Count() >= 2 && (Menu["LaneClear"]["useelc"].GetValue<MenuBool>() && E.IsReady()))
                    {
                        E.Cast();
                    }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void OnGapCloser(object oSender, Events.GapCloserEventArgs args)
        {
            try
            {
                var RGc = Variables.TargetSelector.GetTarget(700, DamageType.Magical);
                if (!RGc.IsUnderEnemyTurret() && (Menu["Misc"]["RGapClose"].GetValue<MenuBool>() && R.IsReady()))
                {
                    R.Cast(RGc);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}