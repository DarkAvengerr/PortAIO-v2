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
    internal class Evelynn : Program
    {


        public static Spell Q, W, E, R;
        internal string comb = "   ";


        public Evelynn()

        //Summs
        {
            Q = new Spell(SpellSlot.Q, 500);
            W = new Spell(SpellSlot.W, Q.Range);
            E = new Spell(SpellSlot.E, 225f + 2 * 65f);
            R = new Spell(SpellSlot.R, 650f);

            R.SetSkillshot(0.25f, 350f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            var Key = Menu.Add(new Menu("Key", "Key"));
            {
                Key.Add(new MenuKeyBind("Combo", "Combo", System.Windows.Forms.Keys.Space, KeyBindType.Press));
                Key.Add(new MenuKeyBind("Harass", "Harass", System.Windows.Forms.Keys.C, KeyBindType.Press));
                Key.Add(new MenuKeyBind("LaneClear", "LaneClear", System.Windows.Forms.Keys.V, KeyBindType.Press));
            }


            var Combo = Menu.Add(new Menu("Combo", "Combo"));
            {

                Combo.Add(new MenuBool("UseQCombo", "Use Q", true));

                Combo.Add(new MenuBool("UseWCombo", "Use W ", true));

                Combo.Add(new MenuBool("UseECombo", "Use E", true));

                Combo.Add(new MenuBool("UseRCombo", "Use R", true));
            }

            var Harass = Menu.Add(new Menu("Harass", "Harass"));
            {
                Harass.Add(new MenuBool("HarassUseQ", "Use Q", true));
                Harass.Add(new MenuSlider("HarassQMana", comb + "Min Mana Percent", 40, 0, 100));
            }

            var LaneClear = Menu.Add(new Menu("LaneClear", "LaneClear"));
            {
                LaneClear.Add(new MenuSlider("lanejungleclearQmana", "LaneClear Min Mana", 40));
                LaneClear.Add(new MenuBool("useqlc", "Use Q to laneclear + to JungleClear!", true));
                LaneClear.Add(new MenuSlider("lanejungleclearEmana", "LaneClear Min Mana", 40));
                LaneClear.Add(new MenuBool("useelc", "Use E to laneclear + to JungleClear!", true));
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
            Variables.Orbwalker.OnAction += Orbwalker_OnAction;

        }

        private void Orbwalker_OnAction(object sender, OrbwalkingActionArgs args)
        {
            try
            {
                if (Menu["LaneClear"]["useelc"].GetValue<MenuBool>() && Me.ManaPercent >= Menu["LaneClear"]["lanejungleclearEmana"].GetValue<MenuSlider>() && E.IsReady())
                {
                    E.Cast(args.Target as Obj_AI_Minion);
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

        private void LaneClear()
        {
            try
            {
                var m = Q.GetLineFarmLocation(GameObjects.EnemyMinions.Where(min => min.Team != GameObjectTeam.Neutral && min.IsValidTarget(Q.Range)).ToList(), Q.Width);
                if (Menu["Key"]["LaneClear"].GetValue<MenuKeyBind>().Active)
                {

                    if (Menu["LaneClear"]["useqlc"].GetValue<MenuBool>() && Me.ManaPercent >= Menu["LaneClear"]["lanejungleclearQmana"].GetValue<MenuSlider>() && Q.IsReady())
                    {
                        Q.Cast(m.Position);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        private void Combo()
        {
            var en = Variables.TargetSelector.GetTarget(650, DamageType.Physical);


            var ComboQ = Menu["Combo"]["UseQCombo"].GetValue<MenuBool>();
            var ComboW = Menu["Combo"]["UseWCombo"].GetValue<MenuBool>();
            var ComboE = Menu["Combo"]["UseECombo"].GetValue<MenuBool>();
            var ComboR = Menu["Combo"]["UseRCombo"].GetValue<MenuBool>();

            if (ComboQ && Q.IsReady() && en.IsValidTarget(Q.Range))
            {
                    Q.Cast(en);
            }

            if (ComboW && W.IsReady() && en.IsValidTarget(Q.Range))
            {
                W.Cast(en);
            }

            if (ComboE && E.IsReady() && en.IsValidTarget(E.Range))
            {
                E.Cast(en);
            }

            if (ComboR && R.IsReady() && en.IsValidTarget(R.Range))
            {
                var pred = R.GetPrediction(en);
                if (pred.Hitchance >= HitChance.High)
                    R.Cast(pred.UnitPosition);
            }


        }

        private void Harass()
        {
            var He = Variables.TargetSelector.GetTarget(650, DamageType.Physical);

            var HarassQ = Menu["Harass"]["HarassUseQ"].GetValue<MenuBool>();
            var HarassQMana = Menu["Harass"]["HarassQMana"].GetValue<MenuSlider>().Value;

            if (HarassQ && Q.IsReady() && Me.ManaPercent >= HarassQMana && He.IsValidTarget(Q.Range) && Q.CanCast(He))
            {
                    Q.CastOnUnit(He);
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
