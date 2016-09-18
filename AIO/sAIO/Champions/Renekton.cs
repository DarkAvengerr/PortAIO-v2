using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using sAIO.Core;

using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace sAIO.Champions
{
    public class Renekton : Helper
    {
        static bool didE;
        static int lastW; 
        public Renekton()
        {
            Renekton_OnGameLoad();
        }

        static void Renekton_OnGameLoad()
        {
            

            Q = new Spell(SpellSlot.Q, 325f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 450f);
            E2 = new Spell(SpellSlot.E, 450f);
            R = new Spell(SpellSlot.R);



            Tiamat = new Items.Item((int)ItemId.Tiamat_Melee_Only, 420);
            Hydra = new Items.Item((int)ItemId.Ravenous_Hydra_Melee_Only, 420);

            Menu comboMenu = new Menu("Combo", "sRenekton.Combo");
            menu.AddSubMenu(comboMenu);
            CreateMenuBool("sRenekton.Combo", "Combo.Q", "Use Q", true);
            CreateMenuBool("sRenekton.Combo", "Combo.W", "Use W", true);
            CreateMenuBool("sRenekton.Combo", "Combo.E", "Use E", true);
            CreateMenuBool("sRenekton.Combo", "Combo.R", "Use R", true);
            CreateMenuBool("sRenekton.Combo", "Combo.E2", "E2 to nearest turret", true);

            Menu harassMenu = new Menu("Harass", "sRenekton.Harass");
            menu.AddSubMenu(harassMenu);
            CreateMenuBool("sRenekton.Harass", "Harass.Q", "Use Q", true);
            CreateMenuBool("sRenekton.Harass", "Harass.W", "Use W", true);
            CreateMenuBool("sRenekton.Harass", "Harass.E", "Use E", true);
            CreateMenuBool("sRenekton.Harass", "Harass.E2", "E2 to nearest turret", true);


            Menu gapMenu = new Menu("Gap Closer", "sRenekton.GapCloser");
            menu.AddSubMenu(gapMenu);
            CreateMenuBool("sRenekton.GapCloser", "GapCloser.W", "Use W on gap closer", true);

            Menu drawMenu = new Menu("Drawing", "sRenekton.Drawing");
            menu.AddSubMenu(drawMenu);
            CreateMenuBool("sRenekton.Drawing", "Drawing.Q", "Draw Q range", true);            
            CreateMenuBool("sRenekton.Drawing", "Drawing.E", "Draw E range", true);

            Menu farmMenu = new Menu("Farm", "sRenekton.Farm");
            menu.AddSubMenu(farmMenu);
            CreateMenuBool("sRenekton.Farm", "Farm.Q", "Use Q", true);
            CreateMenuBool("sRenekton.Farm", "Farm.W", "Use W", true);
            CreateMenuBool("sRenekton.Farm", "Farm.E", "Use E", true);

            menu.AddSubMenu(new Menu("Auto Q", "AutoQ"));
            CreateMenuBool("AutoQ", "AutoQ.Enable", "Enable", true);
            CreateMenuSlider("AutoQ", "AutoQ.Health", "Health Percent", 1, 20, 100);

            menu.AddToMainMenu();                    
                      
            
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            

            Chat.Print("sAIO: " + player.ChampionName + " loaded");

        }

        static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo | orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                var useW = GetValueMenuBool("Combo.W");
                var useWh = GetValueMenuBool("Harass.W");

                if((useW || useWh) && W.IsReady() && target.IsEnemy && target.IsValidTarget(W.Range))
                {
                    W.Cast();
                    lastW = Environment.TickCount;
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target);
                }

                if (Tiamat.IsInRange(target.Position) && Tiamat.IsReady() && Tiamat.IsOwned())
                {
                    Tiamat.Cast();
                }

                if (Hydra.IsInRange(target.Position) && Hydra.IsReady() && Hydra.IsOwned())
                {
                    Hydra.Cast();
                }
            }
        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
            {
                return;
            }

            if (W.IsReady() && gapcloser.Sender.IsValidTarget(W.Range))
            {
                W.CastOnUnit(gapcloser.Sender); 
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            var drawQ = GetValueMenuBool("Drawing.Q");
            var drawE = GetValueMenuBool("Drawing.E");

            if (drawQ)
            {
                Drawing.DrawCircle(player.Position, Q.Range, System.Drawing.Color.Blue);
            }
                      
            if (drawE)
            {
                Drawing.DrawCircle(player.Position, E.Range, System.Drawing.Color.Green);
            }
        }

        static void Game_OnUpdate(EventArgs args)
        {
            switch(orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    Farm();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClean();
                    break;
            }

            AutoQ();
        }

        static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == E.Instance.SData.Name)
                    didE = true;

                if (args.SData.Name == Q.Instance.SData.Name || args.SData.Name == W.Instance.SData.Name || args.SData.Name == R.Instance.SData.Name)
                    didE = false;

                if (args.SData.Name == W.Instance.SData.Name)
                {
                    Orbwalking.ResetAutoAttackTimer();
                }

                if (args.SData.Name == Q.Instance.SData.Name)
                {
                    Orbwalking.ResetAutoAttackTimer();
                }
            }
        }

        static void Combo()
        {
            var useQ = GetValueMenuBool("Combo.Q");
            var useE = GetValueMenuBool("Combo.E");
            var useR = GetValueMenuBool("Combo.R");
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (E.IsReady() && target.IsValidTarget(E.Range) && useE)
            {
                CastE(target);
            }

            if (useR && R.IsReady())
            {
                R.Cast();
            }

           

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && Environment.TickCount - lastW >= 1300 && useQ)
            {
                Q.Cast();
            }

            /*if (E2.IsReady() && target.IsValidTarget(E2.Range) && useE)
            {
                E2.Cast(target.Position);
            }*/
        }
        static void Harass()
        {
            var useQ = GetValueMenuBool("Harass.Q");
            var useE = GetValueMenuBool("Harass.E");
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            if (E.IsReady() && target.IsValidTarget(E.Range) && useE)
            {
                CastE(target);
            }

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && Environment.TickCount - lastW >= 1300 && useQ)
            {
                Q.Cast();
            }

            /*if (E2.IsReady() && target.IsValidTarget(E2.Range) && useE)
            {
                E2.Cast(target.Position);
            }*/
        }

        static void Farm()
        {
            foreach(var minion in MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Enemy))
            {
                var useQ = GetValueMenuBool("Farm.Q");
                var useW = GetValueMenuBool("Farm.W");
                var useE = GetValueMenuBool("Farm.E");

                var Qdmg=Q.GetDamage(minion);
                var Wdmg=W.GetDamage(minion);
                var Edmg=E.GetDamage(minion);

                if (minion.Health < Qdmg && useQ && Q.IsReady() && Q.IsInRange(minion))
                {
                    Q.Cast();
                }

                if (minion.Health < Wdmg && useQ && W.IsReady() && W.IsInRange(minion))
                {
                    W.Cast();
                }

                if (minion.Health < Edmg && useQ && E.IsReady() && E.IsInRange(minion))
                {
                    E.Cast(minion.Position);
                }
            }
        }

        static void LaneClean()
        {
            foreach (var minion in MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Enemy))
            {               
                var Qdmg = Q.GetDamage(minion);
                var Wdmg = W.GetDamage(minion);
                var Edmg = E.GetDamage(minion);

                if (minion.Health < Qdmg && Q.IsReady() &&Q.IsInRange(minion))
                {
                    Q.Cast();
                }

                if (minion.Health < Wdmg && W.IsReady() && W.IsInRange(minion))
                {
                    W.Cast();
                }

                if (minion.Health < Edmg && E.IsReady() && E.IsInRange(minion))
                {
                    E.Cast(minion.Position);
                }
            }
        }

       

        
        static void CastE(AIHeroClient target)
        {
            

            if (player.Distance(target.Position) <= E.Range && target != null)
            {
                if(!didE)
                    E.Cast(target.ServerPosition);
                
                if (GetValueMenuBool("Combo.E2") || GetValueMenuBool("Harass.E2"))
                {
                    var nearestTurret = ObjectManager.Get<Obj_AI_Turret>().Where(t => t.Team == player.Team && !t.IsDead && t.Distance(player.Position) < 2000).FirstOrDefault();

                    if (E.IsReady() && player.HasBuff("renektonsliceanddicedelay") && nearestTurret != null && didE)
                        E2.Cast(nearestTurret.Position);
                }
            }              

            else
            {
                var minion = MinionManager.GetMinions(E.Range).Where(m => m.Distance(target.Position) < E2.Range).FirstOrDefault();

                if(minion != null)
                {
                    E.Cast(minion.Position);
                    
                }
                   
                if (player.HasBuff("renektonsliceanddicedelay"))
                    E2.Cast(target.Position);
            }
        }
        private static void AutoQ()
        {
            if (!player.HasBuff("renektonrageready") || orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                return;

            var healthPercent = GetValueMenuSlider("AutoQ.Health");

            if (player.HealthPercent <= healthPercent)
            {
                var minion = MinionManager.GetMinions(Q.Range).FirstOrDefault();

                if (player.Distance(minion.Position) < Q.Range && Q.IsReady() && GetValueMenuBool("AutoQ.Enable"))
                {
                    Q.Cast();
                }
            }
        }

    }
}
