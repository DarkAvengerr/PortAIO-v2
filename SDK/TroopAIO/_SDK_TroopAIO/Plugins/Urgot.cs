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
    internal class Urgot : Program
    {


        public static Spell Q, Q2, W, E, R;
        internal string comb = "   ";

        public Urgot()

        //Summoner Spells
        {
            Q = new Spell(SpellSlot.Q, 950); //Mainly 1000 but it does better with 950)
            Q2 = new Spell(SpellSlot.Q, 1200);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 850);

            Q.SetSkillshot(0.2667f, 60f, 1600f, true, SkillshotType.SkillshotLine);
            Q2.SetSkillshot(0.3f, 60f, 1800f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.2658f, 120f, 1500f, false, SkillshotType.SkillshotCircle);







            //MENU STARTS HERE
            var Key = Menu.Add(new Menu("Key", "Key"));
            {
                Key.Add(new MenuKeyBind("Combo", "Combo", System.Windows.Forms.Keys.Space, KeyBindType.Press));
                Key.Add(new MenuKeyBind("Harass", "Harass", System.Windows.Forms.Keys.C, KeyBindType.Press));
                Key.Add(new MenuKeyBind("LaneClear", "LaneClear", System.Windows.Forms.Keys.V, KeyBindType.Press));
                //Key.Add(new MenuKeyBind("LastHit", "LastHit", System.Windows.Forms.Keys.X, KeyBindType.Press));
            }


            var Combo = Menu.Add(new Menu("Combo", "Combo"));
            {

                Combo.Add(new MenuBool("UseQCombo", "Use Q", true));

                Combo.Add(new MenuBool("UseQ2Combo", "Use Q if enemy has been hit by E", true));

                Combo.Add(new MenuBool("ComboW", "Use W if E has been hit", true));

                Combo.Add(new MenuBool("UseECombo", "Use E", true));

            }

            var Harass = Menu.Add(new Menu("Harass", "Harass"));
            {
                Harass.Add(new MenuBool("HarassUseQ", "Use Q", true));
                Harass.Add(new MenuSlider("HarassQMana", comb + "Min Mana Percent", 40, 0, 100));
            }

            var LaneClear = Menu.Add(new Menu("LaneClear", "LaneClear"));
            {
                LaneClear.Add(new MenuSlider("laneclearmana", "LaneClear Min Mana", 40));
                LaneClear.Add(new MenuBool("useqlc", "Use Q to laneclear", true));
            }

            var Misc = Menu.Add(new Menu("Misc", "Misc"));
            {
                Misc.Add(new MenuBool("RInterrupt", "Use R to Interrupt", true));
            }


            var Draw = Menu.Add(new Menu("Draw", "Draw"));
            {
                Draw.Add(new MenuBool("DrawQ", "Draw Q Range"));
                Draw.Add(new MenuBool("DrawW", "Draw W Range"));
                Draw.Add(new MenuBool("DrawE", "Draw E Range"));
            }


            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Events.OnInterruptableTarget += Events_OnInterruptableTarget;
            Variables.Orbwalker.OnAction += Orbwalker_OnAction;

        }

        private void Orbwalker_OnAction(object sender, OrbwalkingActionArgs args)
        {
            try
            {
                if (args.Type == OrbwalkingType.BeforeAttack && Menu["Key"]["LaneClear"].GetValue<MenuKeyBind>().Active && (args.Target is Obj_AI_Turret || args.Target.Type == GameObjectType.obj_AI_Minion))
                {
                    if (Menu["LaneClear"]["useqlc"].GetValue<MenuBool>() && Me.ManaPercent >= Menu["LaneClear"]["laneclearmana"].GetValue<MenuSlider>() && Q.IsReady())
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
                //if (Menu["Key"]["LaneClear"].GetValue<MenuKeyBind>().Active)
                //{
                //    LaneClear();
                //}


            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //private void LaneClear()
        //{
        //    try
        //    {


        //    }
        //    catch (Exception ex)   //Everything put in OnAction
        //    {
        //        Console.WriteLine(ex);
        //    }
        //}

        private void Combo()
        {
            var enemy = Variables.TargetSelector.GetTarget(1200, DamageType.Physical);


            var ComboQ = Menu["Combo"]["UseQCombo"].GetValue<MenuBool>();
            var ComboQ2 = Menu["Combo"]["UseQ2Combo"].GetValue<MenuBool>();
            var ComboW = Menu["Combo"]["ComboW"].GetValue<MenuBool>();
            var ComboE = Menu["Combo"]["UseECombo"].GetValue<MenuBool>();


            if (ComboE && E.IsReady() && enemy.IsValidTarget(E.Range))
            {
                var pred = E.GetPrediction(enemy);
                if (pred.Hitchance >= HitChance.High)
                    E.Cast(pred.UnitPosition);
            }


            if (ComboQ && Q.IsReady() && enemy.IsValidTarget(Q.Range))
            {
                var pred = Q.GetPrediction(enemy);
                if (pred.Hitchance >= HitChance.High && pred.CollisionObjects.Count == 0)
                    Q.Cast(pred.UnitPosition);
            }



            if (ComboQ2 && Q2.IsReady() && enemy.IsValidTarget(Q2.Range) && HasUrgotEBuff(enemy))
            {
                if (ComboW && W.IsReady())
                    W.Cast();
                Q2.CastOnUnit(enemy);
            }




        }




        private void Harass()
        {
            var e = Variables.TargetSelector.GetTarget(1200, DamageType.Physical);

            var HarassQ = Menu["Harass"]["HarassUseQ"].GetValue<MenuBool>();
            var HarassQMana = Menu["Harass"]["HarassQMana"].GetValue<MenuSlider>().Value;

            if (HarassQ && Q.IsReady() && Me.ManaPercent >= HarassQMana && e.IsValidTarget(Q.Range) && Q.CanCast(e))
            {
                var pred = Q.GetPrediction(e);
                if (pred.Hitchance >= HitChance.High && pred.CollisionObjects.Count == 0)
                    Q.CastOnUnit(e);
            }
        }







        private void Events_OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            try
            {
                var target = args.Sender;

                if (Menu["Misc"]["RInterrupt"].GetValue<MenuBool>() && target.IsEnemy && target.IsValidTarget(R.Range) && R.IsReady())
                    if (!target.IsUnderEnemyTurret())
                    {
                        if (args.DangerLevel >= LeagueSharp.Data.Enumerations.DangerLevel.High || target.IsCastingInterruptableSpell())
                        {
                            R.Cast();
                        }
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static bool HasUrgotEBuff(Obj_AI_Base e)
        {
            return (e.HasBuff("urgotcorrosivedebuff"));
        }
    }
}