using Azir_Free_elo_Machine;
using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Azir_Creator_of_Elo
{
    internal class AzirModes : Modes
    {
        public readonly JumpLogic _jump;
        private Insec _insec;

                            
        public AzirModes(AzirMain azir)
        {
            _jump = new JumpLogic(azir);
            _insec = new Insec(azir);
        }

        public override void Update(AzirMain azir)
        {

            base.Update(azir);
            if (azir.Menu.GetMenu.Item("fleekey").GetValue<KeyBind>().Active)
            {
                azir.Orbwalk(Game.CursorPos);
                Jump(azir);
            }

            RAllyTower(azir);
        }



        private void RAllyTower(AzirMain azir)
        {

            var useR = azir.Menu.GetMenu.Item("ARUT").GetValue<bool>();
            if (useR)
            {
                var target = TargetSelector.GetTarget(1100, TargetSelector.DamageType.Magical);
                if (target != null)
                {

                    if (azir.Hero.Distance(target) < 220)
                    {

                        var tower =
                            ObjectManager.Get<Obj_AI_Turret>().FirstOrDefault(it => it.IsAlly && it.IsValidTarget(1000));
                        if (tower != null)
                        {
                            azir.Spells.R.Cast(tower.Position);
                        }
                    }
                }
            }
        }

        public void Jump(AzirMain azir)
        {
            _jump.updateLogic(Game.CursorPos);
        }

        public override void Harash(AzirMain azir)
        {
            var wCount = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Ammo;
            var useQ = azir.Menu.GetMenu.Item("HQ").GetValue<bool>();
            var useW = azir.Menu.GetMenu.Item("HW").GetValue<bool>();
            var savew = azir.Menu.GetMenu.Item("HW2").GetValue<bool>();
            base.Harash(azir);
            var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
            if (target != null)
            {
                if (target.Distance(azir.Hero.ServerPosition) < 450)
                {
                    var pred = azir.Spells.W.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.Medium)
                    {
                        if (savew && (wCount == 1))
                        {

                        }
                        else
                        {
                            if (useW)
                                if (azir.Spells.W.IsReady())
                                    azir.Spells.W.Cast(pred.CastPosition);
                        }
                    }
                }
                else
                {
                    if (!savew || (wCount != 1))
                    {
                        if (useW)
                            azir.Spells.W.Cast(azir.Hero.Position.Extend(target.ServerPosition, 450));
                    }

                }
                var checksQ = azir.SoldierManager.ChecksToCastQ(azir, target);
                if (checksQ)
                    StaticSpells.CastQ(azir, target, useQ);

            }
        }

        public override void Laneclear(AzirMain azir)
        {
            var useQ = azir.Menu.GetMenu.Item("LQ").GetValue<bool>();
            var useW = azir.Menu.GetMenu.Item("LW").GetValue<bool>();
            var minToW = azir.Menu.GetMenu.Item("LWM").GetValue<Slider>().Value;
            base.Laneclear(azir);
     
            // wpart
           var  minionW =
   MinionManager.GetMinions(
                           azir.Hero.Position,
                            azir.Spells.W.Range,
                            MinionTypes.All,
                            MinionTeam.NotAlly,
                            MinionOrderTypes.MaxHealth);
            if (minionW != null&&useW)
            {
                var wFarmLocation = azir.Spells.W.GetCircularFarmLocation(minionW,
                    315);
                if (wFarmLocation.MinionsHit >= minToW)
                {
                    var closestSoldier = azir.SoldierManager.getClosestSolider(wFarmLocation.Position.To3D());
                    if (closestSoldier == null)
                    {
                        azir.Spells.W.Cast(wFarmLocation.Position);
                    }
                    else if (wFarmLocation.Position.Distance(closestSoldier) >= 300)
                    {
                        azir.Spells.W.Cast(wFarmLocation.Position);
                    }
           
                }
            }
           var minionQ =
MinionManager.GetMinions(
                      azir.Hero.Position,
                       azir.Spells.Q.Range,
                       MinionTypes.All,
                       MinionTeam.NotAlly,
                       MinionOrderTypes.MaxHealth);
            if (minionQ == null || !useQ || !azir.SoldierManager.CheckQCastAtLaneClear(minionQ, azir)) return;
            {
                var wFarmLocation = azir.Spells.Q.GetCircularFarmLocation(minionW,
                    315);
                foreach (var objAiBase in   minionQ)
                {
                    var minion = (Obj_AI_Minion) objAiBase;
                    var closestSoldier = azir.SoldierManager.getClosestSolider(minion.ServerPosition);
                    if(closestSoldier!=null)
                        if (minion.Distance(closestSoldier) > 315)
                        {
                            azir.Spells.Q.Cast(minion.Position);
                            break;
                        }
                }
            }
        }
        
        public override void Jungleclear(AzirMain azir)
        {
            var useW = azir.Menu.GetMenu.Item("JQ").GetValue<bool>();
            var useQ = azir.Menu.GetMenu.Item("JW").GetValue<bool>();
            base.Jungleclear(azir);
            var minionW =
MinionManager.GetMinions(
                   azir.Hero.Position,
                    azir.Spells.W.Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);
            if (minionW != null && useW)
            {
                var wFarmLocation = azir.Spells.W.GetCircularFarmLocation(minionW,
                    315);
                if (wFarmLocation.MinionsHit>=1)
                {
                    azir.Spells.W.Cast(wFarmLocation.Position);
                }
            }
            var minionQ =
MinionManager.GetMinions(
                      azir.Hero.Position,
                       azir.Spells.Q.Range,
                       MinionTypes.All,
                       MinionTeam.Neutral,
                       MinionOrderTypes.MaxHealth);
            if (minionQ == null || !useQ || !azir.SoldierManager.CheckQCastAtLaneClear(minionQ, azir)) return;
            {
                var wFarmLocation = azir.Spells.Q.GetCircularFarmLocation(minionW,
                    315);
                foreach (var objAiBase in minionQ)
                {
                    var minion = (Obj_AI_Minion) objAiBase;
                    var closestSoldier = azir.SoldierManager.getClosestSolider(minion.ServerPosition);
                    if (closestSoldier == null) continue;
                    if (!(minion.Distance(closestSoldier) > 315)) continue;
                    azir.Spells.Q.Cast(minion.Position);
                    break;
                }
            }
        }
        

        public override void Combo(AzirMain azir)
        {

            var useQ = azir.Menu.GetMenu.Item("CQ").GetValue<bool>();
            var useW = azir.Menu.GetMenu.Item("CW").GetValue<bool>();
            base.Combo(azir);
            var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
            if (target == null) return;

            if (target.Distance(azir.Hero.ServerPosition) < 450)
            {
                if (target.IsRunningOfYou())
                {
                    var pos = Prediction.GetPrediction(target, 0.5f).UnitPosition;
                    azir.Spells.W.Cast(pos);
                }
                else
                {
                    var pred = azir.Spells.W.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.VeryHigh)
                    {
                        if (useW)
                            azir.Spells.W.Cast(pred.CastPosition);
                    }
                }
            }
            else
            {
                if (azir.Spells.Q.Level > 0 && azir.Spells.Q.IsReady())
                    if (useW)
                        if (target.Distance(HeroManager.Player) <= 750)
                            azir.Spells.W.Cast(azir.Hero.Position.Extend(target.ServerPosition, 450));
            }
            //Qc casting
            var checksQ = azir.SoldierManager.ChecksToCastQ(azir, target);
            if (checksQ)
            {
                StaticSpells.CastQ(azir, target, useQ);
            }




            else if (azir.Spells.R.IsKillable(target))
            {
                if (!azir.Menu.GetMenu.Item("CR").GetValue<bool>()) return;
                if (!(target.Health < azir.Spells.R.GetDamage(target))) return;
                var pred = azir.Spells.R.GetPrediction(target);
                if (pred.Hitchance >= HitChance.High)
                {

                    azir.Spells.R.Cast(pred.CastPosition);
                }
                //      azir.Spells.R.Cast(target);
            }
        }


    }
}
