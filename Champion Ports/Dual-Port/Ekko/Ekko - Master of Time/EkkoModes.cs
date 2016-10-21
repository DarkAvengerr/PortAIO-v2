using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Ekko_master_of_time
{
    internal class EkkoModes : Modes
    {
        private Core core;

        public EkkoModes(Core core)
        {
            this.core = core;
        }

        public override void Jungleclear(Core ekko)
        {
            var jungleQ = ekko.Menu.GetMenu.Item("JQ").GetValue<bool>();
            var jungleW = ekko.Menu.GetMenu.Item("JW").GetValue<bool>();
            var jungleE = ekko.Menu.GetMenu.Item("JE").GetValue<bool>();
            var minion = MinionManager.GetMinions(ekko.Spells.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (minion != null)
            {
                if (jungleQ)
                {
                    ekko.Spells.Q.Cast(minion);

                }
                if (jungleW)
                {
                    ekko.Spells.W.Cast(minion.Position);
                }
                if (jungleE)
                {
                    ekko.Spells.E.Cast(minion.Position);
                }
            }
        }

        public override void Laneclear(Core ekko)
        {
            var laneQ = ekko.Menu.GetMenu.Item("LQ").GetValue<bool>();
            var laneE = ekko.Menu.GetMenu.Item("LE").GetValue<bool>();
            var minion = MinionManager.GetMinions(ekko.Spells.Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (minion != null)
            {
                if (laneQ)
                {
                    ekko.Spells.Q.Cast(minion);

                }
         
                if (laneE)
                {
                    ekko.Spells.E.Cast(minion.Position);
                }
            }
        }
        public override void Harash(Core ekko)
        {
            var comboQ = ekko.Menu.GetMenu.Item("HQ").GetValue<bool>();
            var comboW = ekko.Menu.GetMenu.Item("HW").GetValue<bool>();
            var comboE = ekko.Menu.GetMenu.Item("HE").GetValue<bool>();
            var target = TargetSelector.GetTarget(ekko.Spells.Q.Range, TargetSelector.DamageType.Magical);
            if (target != null)
            {
                if (comboQ && ekko.Spells.Q.IsReady())
                {
                    var pred=ekko.Spells.Q.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.VeryHigh)
                    {
                        ekko.Spells.Q.Cast(pred.CastPosition);
                    }
                    if (target.Distance(HeroManager.Player) <= 300)
                    {
                        var pos = HeroManager.Player.ServerPosition.Extend(target.ServerPosition, 200);
                            ekko.Spells.Q.Cast(pos);
                    }
                }
                if (comboE&&ekko.Spells.E.IsReady())
                {
                    
                    ekko.Spells.E.Cast(target.Position);
                }
                if (comboW && ekko.Spells.W.IsReady())
                {
                    if (target.HasBuffOfType(BuffType.Slow) || target.HasBuffOfType(BuffType.Stun) ||
                     target.HasBuffOfType(BuffType.Taunt))
                    {
                        var pos = Prediction.GetPrediction(target, 1000f);
                            ekko.Spells.W.Cast(pos.CastPosition);
                    }

                }
            }
        }

        public override void Combo(Core ekko)
        {
            var comboQ = ekko.Menu.GetMenu.Item("CQ").GetValue<bool>();
            var comboW = ekko.Menu.GetMenu.Item("CW").GetValue<bool>();
            var comboE = ekko.Menu.GetMenu.Item("CE").GetValue<bool>();
            var comboR = ekko.Menu.GetMenu.Item("CR").GetValue<bool>();
            var comboRH = ekko.Menu.GetMenu.Item("CHR").GetValue<Slider>().Value;
            var target = TargetSelector.GetTarget(ekko.Spells.Q.Range, TargetSelector.DamageType.Magical);
            if (target != null)
            {
                // e+q cast
                
                if (target.Distance(HeroManager.Player) <= 300)
                {
                    var pos = HeroManager.Player.ServerPosition.Extend(target.ServerPosition, 200);
                    if (comboQ)
                        ekko.Spells.Q.Cast(pos);
                }
                if(comboE)
                ekko.Spells.E.Cast(target.Position);


                //Wcast only if buffed.
                if (target.HasBuffOfType(BuffType.Slow) || target.HasBuffOfType(BuffType.Stun) ||
                    target.HasBuffOfType(BuffType.Taunt))
                {
                    var pos = Prediction.GetPrediction(target, 1000f);
                    if(comboW)
                    ekko.Spells.W.Cast(pos.CastPosition);
                }
                if (target.UnderAllyTurret())
                {
                    if (core.EkkoUlti().Position.Distance(target.Position) <= ekko.Spells.R.Range)
                    {
                        var towerAggro=riskCheck.GetTowerDamage(target);
                        if (
                            riskCheck.GetDamageInput(new List<Spell> {core.Spells.Q, core.Spells.E, core.Spells.R},
                                target) +towerAggro>= target.Health)
                        {
                            if (comboR)
                                core.Spells.R.Cast();
                        }
                    }
                }
                else
                {
                    if(ekko.EkkoUlti().Position.Distance(target.ServerPosition)<=ekko.Spells.R.Range)
                    if (
                         riskCheck.GetDamageInput(new List<Spell> { core.Spells.Q, core.Spells.E, core.Spells.R },
                             target) >= target.Health)
                    {
                        
                        if (comboR)
                            core.Spells.R.Cast();
                    }
                }
            }
            else
            {
                castRNearAlgorithm(core);
            }
            RKs(ekko);
            if (core.Hero.HealthPercent <= comboRH)
            {
                core.Spells.R.Cast();
            }
            var useR = ekko.Menu.GetMenu.Item("CAR").GetValue<Slider>().Value;
            if (riskCheck.WillHitEnemys(ekko.EkkoUlti().Position, (int) ekko.Spells.R.Range, useR))
            {
                core.Spells.R.Cast();
            }
          //  if(riskCheck.unkilleable())
        }
        public void castRNearAlgorithm(Core ekkoCore)
        {
            //dates
            var ekkoPos = ekkoCore.EkkoUlti().Position;
            var Erange = 700;
            //   matable por e + q + r o q+r
            //damage input
            // foreach()
            bool doIt=false;
            foreach (AIHeroClient target in HeroManager.Enemies)
            {
                if (target.Distance(ekkoPos) <= Erange)
                {
                    double eqComboDamage = 0;
                    if (ekkoCore.Spells.Q.IsReady()) eqComboDamage += ekkoCore.Spells.Q.GetDamage(target);
                    if (ekkoCore.Spells.E.IsReady()) eqComboDamage += ekkoCore.Spells.E.GetDamage(target);
                    eqComboDamage += ekkoCore.Hero.GetAutoAttackDamage(target, true);
                    if (target.Health <= eqComboDamage)
                    {
                        doIt = true;
                        break;
                    }
                    if (target.Distance(ekkoPos) <= ekkoCore.Spells.R.Range - 25)
                    {
                        double reqComboDamage = 0;
                        if (ekkoCore.Spells.Q.IsReady()) reqComboDamage += ekkoCore.Spells.Q.GetDamage(target);
                        if (ekkoCore.Spells.E.IsReady()) reqComboDamage += ekkoCore.Spells.E.GetDamage(target);
                        if (ekkoCore.Spells.R.IsReady()) reqComboDamage += ekkoCore.Spells.R.GetDamage(target);
                        if (target.Health <= reqComboDamage)
                        {
                            doIt = true;
                            break;
                        }
                    }
                }
            }
            if(riskCheck.RiskChecker(ekkoPos,Erange+300))
                {
                    if (doIt)
                    {
                        ekkoCore.Spells.R.Cast();
                    }
                }
  
        }
        public void RKs(Core ekko)
        {
            var KillstealR = ekko.Menu.GetMenu.Item("CRK").GetValue<bool>();
            if(KillstealR)
            foreach (AIHeroClient enemy in HeroManager.Enemies)
            {
                if(!enemy.IsDead)
                if (enemy.Distance(ekko.EkkoUlti().Position) <= ekko.Spells.R.Range-25)
                {
                    if (ekko.Spells.R.IsKillable(enemy))
                    {
                            if(riskCheck.RiskChecker(ekko.EkkoUlti().Position,(int)ekko.Spells.R.Range+300))
                        ekko.Spells.R.Cast();
                    }
                }
            }
        }
    }
}