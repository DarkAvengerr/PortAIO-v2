using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
using LeagueSharp.Common; 
namespace DarkMage
{
    class SyndraModes : Modes
    {
        public SyndraModes()
        {
        }
        public override void Combo(SyndraCore core)
        {
            var useQ = core.GetMenu.GetMenu.Item("CQ").GetValue<bool>();
            var useW = core.GetMenu.GetMenu.Item("CW").GetValue<bool>();
            var useE = core.GetMenu.GetMenu.Item("CE").GetValue<bool>();
            var useR = core.GetMenu.GetMenu.Item("CR").GetValue<bool>();
            var qeRange = core.GetSpells.GetQ.Range + 500;
            var qeTarget = TargetSelector.GetTarget(qeRange, TargetSelector.DamageType.Magical);
            if (qeTarget != null)
            {

                if (!core.GetSpells.castE(qeTarget))
                {
                    if (useQ)
                        core.GetSpells.CastQ();
                    if (useW)
                        core.GetSpells.CastW();
                }
                if (useE)
                {
                    var eTarget = TargetSelector.GetTarget(core.GetSpells.EQ.Range, TargetSelector.DamageType.Magical);
                    if (eTarget != null && core.GetSpells.GetE.IsReady())
                        core.GetSpells.castE(eTarget);
                }

            }
            if (useR)
                core.GetSpells.CastR(core);
            base.Combo(core);
        }
        public override void Harash(SyndraCore core)
        {
            var useQ = core.GetMenu.GetMenu.Item("HQ").GetValue<bool>();
            var useW = core.GetMenu.GetMenu.Item("HW").GetValue<bool>();
            var useE = core.GetMenu.GetMenu.Item("HE").GetValue<bool>();
            if (useQ)
                core.GetSpells.CastQ();
            if (useW)
                core.GetSpells.CastW();
            if (useE)
                core.GetSpells.CastE(core);
            base.Harash(core);
        }
        bool QE,AutoQE;
        public override void Keys(SyndraCore core)
        {
            if (core.GetSpells.GetQ.IsReady() && core.GetSpells.GetE.IsReady())
            {
                QE = false;
                AutoQE = false;
            }
            if (core.GetMenu.GetMenu.Item("RKey").GetValue<KeyBind>().Active)
            {
                if (core.GetSpells.GetR.IsReady())
                {
                    var rTarget = TargetSelector.GetTarget(core.GetSpells.GetR.Range, TargetSelector.DamageType.Magical);
                    if(rTarget!=null)
                    core.GetSpells.GetR.Cast(rTarget);
                }

            }
            if (core.GetMenu.GetMenu.Item("HKey").GetValue<KeyBind>().Active)
            {
               // if (!HeroManager.Player.CanAttack)
               // {
                    core.GetSpells.CastQ();
               // }


            }
            if (core.GetMenu.GetMenu.Item("QEkey").GetValue<KeyBind>().Active)
            {
                if (!QE)
                {
                    var gameCursor = Game.CursorPos;
                    core.GetSpells.GetQ.Cast(core.Hero.Position.Extend(Game.CursorPos, core.GetSpells.GetQ.Range));
                    LeagueSharp.Common.Utility.DelayAction.Add(250 + Game.Ping, () => core.GetSpells.GetE.Cast(gameCursor));
                    QE = true;
                }
            }
            if (core.GetMenu.GetMenu.Item("AUTOQE").GetValue<KeyBind>().Active)
            {
                if (!AutoQE)
                {
                    var qeRange = core.GetSpells.GetQ.Range + 500;
                    var qeTarget = TargetSelector.GetTarget(qeRange, TargetSelector.DamageType.Magical);
                    /*   
                        if (qeTarget != null)
                        {
                            var predpos = Prediction.GetPrediction(qeTarget, 500);
                            if (predpos.UnitPosition.Distance(core.Hero.Position) < qeRange)    
                            {
                                var ballPos = core.Hero.Position.Extend(predpos.UnitPosition, core.GetSpells.GetQ.Range);
                                core.GetSpells.GetQ.Cast(ballPos);
                                LeagueSharp.Common.Utility.DelayAction.Add(250 + Game.Ping, () => core.GetSpells.GetE.Cast(ballPos));
                                AutoQE = true;
                            }
                        }*/
                    if (qeTarget != null)
                    {
                        core.GetSpells.TryBallE(qeTarget);
                    }
                }
            }
            base.Keys(core);
        }
        public override void LastHit(SyndraCore core)
        {

            //Last hit with q when u cant kill minion with aa.
            base.LastHit(core);
        }
        public override void Laneclear(SyndraCore core)
        {
            var useQ = core.GetMenu.GetMenu.Item("LQ").GetValue<bool>();
            var useW = core.GetMenu.GetMenu.Item("LW").GetValue<bool>();
            var miniumMana = core.GetMenu.GetMenu.Item("LM").GetValue<Slider>().Value;
            if (core.Hero.ManaPercent < miniumMana) return;
            if (useQ)
            {
                var minionQ =
    MinionManager.GetMinions(
                    core.Hero.Position,
                     core.GetSpells.GetQ.Range,
                     MinionTypes.All,
                     MinionTeam.Enemy,
                     MinionOrderTypes.MaxHealth);
                if (minionQ != null)
                {
                    var QfarmPos = core.GetSpells.GetQ.GetCircularFarmLocation(minionQ);
                    if (QfarmPos.Position.IsValid())
                        if (QfarmPos.MinionsHit >= 2)
                        {
                            core.GetSpells.GetQ.Cast(QfarmPos.Position);
                        }
                }
            }
            if (useW)
            {
                var minionW =
MinionManager.GetMinions(
    core.Hero.Position,
     core.GetSpells.GetW.Range,
     MinionTypes.All,
     MinionTeam.Enemy,
     MinionOrderTypes.MaxHealth);
                if (minionW != null)
                {
                    var WfarmPos = core.GetSpells.GetQ.GetCircularFarmLocation(minionW);
                    if (WfarmPos.Position.IsValid())
                    {
                        if (WfarmPos.MinionsHit >= 3)
                        {
                            core.GetSpells.CastWToPos(WfarmPos.Position);
                        }
                    }
                }
            }
            base.Laneclear(core);
        }

        public override void Jungleclear(SyndraCore core)
        {
            var useQ = core.GetMenu.GetMenu.Item("JQ").GetValue<bool>();
            var useW = core.GetMenu.GetMenu.Item("JW").GetValue<bool>();
            var useE = core.GetMenu.GetMenu.Item("JE").GetValue<bool>();
            var miniumMana = core.GetMenu.GetMenu.Item("JM").GetValue<Slider>().Value;
            if (core.Hero.ManaPercent < miniumMana) return;
            if (useQ)
            {
                var minionQ =
                    MinionManager.GetMinions(
                        core.Hero.Position,
                        core.GetSpells.GetQ.Range,
                        MinionTypes.All,
                        MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);
                if (minionQ != null)
                {
                    var QfarmPos = core.GetSpells.GetQ.GetCircularFarmLocation(minionQ);
                    if (QfarmPos.Position.IsValid())
                        core.GetSpells.GetQ.Cast(QfarmPos.Position);

                }
            }
            if (useW)
            {
                var minionW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, core.GetSpells.GetW.Range, MinionTypes.All, MinionTeam.NotAlly);
                var minionss = minionW.Any(x => x.Team == GameObjectTeam.Neutral);
                if (minionss)
                {
                    var WfarmPos = core.GetSpells.GetQ.GetCircularFarmLocation(minionW);
                    if (WfarmPos.Position.IsValid())
                    {
                        core.GetSpells.CastWToPos(WfarmPos.Position);

                    }
                    else
                    {
                        core.GetSpells.CastWToPos(Game.CursorPos.To2D());
                    }
                }
            }
            if (useE)
            {
                var minionE =
                    MinionManager.GetMinions(
                        core.Hero.Position,
                        core.GetSpells.GetQ.Range,
                        MinionTypes.All,
                        MinionTeam.Neutral,
                        MinionOrderTypes.MaxHealth);


                if (minionE != null)
                {
                    var EfarmPos = core.GetSpells.GetQ.GetCircularFarmLocation(minionE);
                    if (EfarmPos.Position.IsValid())
                    {
                        core.GetSpells.GetE.Cast(EfarmPos.Position);
                    }
                }
            }
            base.Jungleclear(core);
        }
    }
        
        }
    

