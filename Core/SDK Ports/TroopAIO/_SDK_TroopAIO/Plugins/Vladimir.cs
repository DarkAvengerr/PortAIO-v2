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
    internal class Vladimir : Program
    {


        public static Spell Q, W, E, R;

        public Vladimir()

        //Summoner Spells
        {
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 700);

            R.SetSkillshot(0.25f, 175, 700, false, SkillshotType.SkillshotCircle);

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

                Combo.Add(new MenuBool("UseWCombo", "Use W ", false));

                Combo.Add(new MenuBool("UseECombo", "Use E", true));

                Combo.Add(new MenuBool("UseRCombo", "Use R", true));

            }

            var Harass = Menu.Add(new Menu("Harass", "Harass"));
            {
                Harass.Add(new MenuBool("HarassUseQ", "Use Q", true));
                Harass.Add(new MenuBool("HarassUseE", "Use E", true));
            }

            var LaneClear = Menu.Add(new Menu("LaneClear", "LaneClear"));
            {
                LaneClear.Add(new MenuBool("useQlc", "Use Q to laneclear", true));
                LaneClear.Add(new MenuBool("useelc", "Use E to laneclear", true));
            }

            var Misc = Menu.Add(new Menu("Misc", "Misc"));
            {
                Misc.Add(new MenuBool("WGapClose", "Use W to anti-gaplcose", true));
            }

            var LastHit = Menu.Add(new Menu("LastHit", "LastHit"));
            {
                LastHit.Add(new MenuBool("qlh", "Use Q to Lasthit", true));
            }

            var Draw = Menu.Add(new Menu("Draw", "Draw"));
            {
                Draw.Add(new MenuBool("DrawQ", "Draw Q Range"));
                Draw.Add(new MenuBool("DrawW", "Draw W Range"));
                Draw.Add(new MenuBool("DrawE", "Draw E Range"));
                Draw.Add(new MenuBool("DrawR", "Draw R Range"));
            }

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Events.OnGapCloser += OnGapCloser;
            Variables.Orbwalker.OnAction += Orbwalker_OnAction;

        }

        private void Orbwalker_OnAction(object sender, OrbwalkingActionArgs args)
        {
            try
            {
                if (args.Type == OrbwalkingType.BeforeAttack && Menu["Key"]["LaneClear"].GetValue<MenuKeyBind>().Active && (args.Target is Obj_AI_Turret || args.Target.Type == GameObjectType.obj_AI_Minion))
                {
                    if (Menu["LaneClear"]["useQlc"].GetValue<MenuBool>() && Q.IsReady())
                    {
                        Q.CastOnUnit(args.Target as Obj_AI_Minion);
                    }
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
                if (Menu["Misc"]["WGapClose"].GetValue<MenuBool>() && W.IsReady())
                {
                    W.Cast();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
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

        private void LastHit()
        {
            if (GameObjects.EnemyMinions.Count() != 0 && (Menu["LastHit"]["qlh"].GetValue<MenuBool>() && Q.IsReady()))
            {
                Obj_AI_Minion minion =
                    GameObjects.EnemyMinions.FirstOrDefault(m => m.Distance(Me) <= Q.Range && m.Health <= Q.GetDamage(m));
                Q.Cast(minion);
            }
        }

        private void Combo()
        {
            var en = Variables.TargetSelector.GetTarget(1400, DamageType.Magical);

            var ComboQ = Menu["Combo"]["UseQCombo"].GetValue<MenuBool>();
            var ComboW = Menu["Combo"]["UseWCombo"].GetValue<MenuBool>();
            var ComboE = Menu["Combo"]["UseECombo"].GetValue<MenuBool>();
            var ComboR = Menu["Combo"]["UseRCombo"].GetValue<MenuBool>();

            if (ComboQ && Q.IsReady() && en.IsValidTarget(Q.Range))
            {
                Q.Cast(en);
            }

            if (ComboE && E.IsReady() && en.IsValidTarget(550))
            {
                E.StartCharging(Game.CursorPos);
            }

            if (ComboR && R.IsReady() && en.IsValidTarget(R.Range))
            {
                var pred = R.GetPrediction(en);
                if (pred.Hitchance >= HitChance.VeryHigh)
                {
                    R.Cast(pred.UnitPosition);
                }

                if (ComboW && W.IsReady() && en.IsValidTarget(W.Range))
                {
                    W.Cast(en);
                }

            }
        }



        private void Harass()
        {
            var HarassTarget = Variables.TargetSelector.GetTarget(750, DamageType.Magical);
            var HarassQ = Menu["Harass"]["HarassUseQ"].GetValue<MenuBool>();
            var HarassE = Menu["Harass"]["HarassUseE"].GetValue<MenuBool>();

            if (HarassQ && Q.IsReady() && HarassTarget.IsValidTarget(Q.Range))
            {
                Q.Cast(HarassTarget);
            }

            if (HarassE && E.IsReady() && HarassTarget.IsValidTarget(550))
            {
                E.StartCharging(Game.CursorPos);
            }
        }

        private void LaneClear()
        {
            try
            {
                if (Menu["Key"]["LaneClear"].GetValue<MenuKeyBind>().Active)
                    if (GameObjects.EnemyMinions.Where(mins => mins.Position.DistanceToPlayer() < E.Range ).Count() >= 2 && (Menu["LaneClear"]["useelc"].GetValue<MenuBool>() && E.IsReady()))
                    {
                        E.Cast();
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            try
            {
                if (Me.IsDead)
                    return;

                if (Q.IsReady() && Menu["Draw"]["DrawQ"].GetValue<MenuBool>())
                    Render.Circle.DrawCircle(Me.Position, Q.Range, System.Drawing.Color.AliceBlue);

                if (W.IsReady() && Menu["Draw"]["DrawW"].GetValue<MenuBool>())
                    Render.Circle.DrawCircle(Me.Position, W.Range, System.Drawing.Color.AliceBlue);

                if (E.IsReady() && Menu["Draw"]["DrawE"].GetValue<MenuBool>())
                    Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.AliceBlue);

                if (R.IsReady() && Menu["Draw"]["DrawR"].GetValue<MenuBool>())
                    Render.Circle.DrawCircle(Me.Position, E.Range, System.Drawing.Color.AliceBlue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
