using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event.OrbwalkingModes
{
    #region

    using System;
    using System.Linq;

    using Core;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Menus;

    using Orbwalking = Orbwalking;

    #endregion

    internal class AfterAuto : Core
    {
        #region Public Methods and Operators

        // Jungle, Combo etc.
        public static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(args.SData.Name))
            {
                return;
            }

            var a = HeroManager.Enemies.Where(x => x.IsValidTarget(Player.AttackRange + 360));

            var targets = a as AIHeroClient[] ?? a.ToArray();

            foreach (var target in targets)
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if (Spells.Q.IsReady())
                    {
                        CastQ(target);
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    if (Qstack == 2)
                    {
                        CastQ(target);
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.FastHarass)
                {
                    if (Spells.Q.IsReady())
                    {
                        CastQ(target);
                    }

                    if (!Spells.Q.IsReady())
                    {
                        CastW(target);
                    }
                }

                if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Burst) return;

                if (Spells.R.IsReady() && Spells.R.Instance.Name == IsSecondR)
                {
                    var pred = Spells.R.GetPrediction(target);

                    if (pred.Hitchance < HitChance.High)
                    {
                        return;
                    }

                    Spells.R.Cast(pred.CastPosition);
                }

                else if (Spells.Q.IsReady())
                {
                    CastQ(target);
                }
            }

            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
            {
                return;
            }

            if (args.Target is Obj_AI_Minion)
            {
                if (MenuConfig.LaneEnemy && ObjectManager.Player.CountEnemiesInRange(1500) > 0)
                {
                    return;
                }

                var minions = MinionManager.GetMinions(Player.AttackRange + 360).Where(x => x != null);
                    // Redundant af?? whatever

                foreach (var m in minions)
                {
                    if (!MenuConfig.LaneQ || m.UnderTurret(true))
                    {
                        return;
                    }

                    if (Spells.Q.IsReady())
                    {
                        CastQ(m);
                    }
                }

                var mobs = MinionManager.GetMinions(Player.Position, 360f, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.None); // Cheese lvl 1, Ex. Small Gromp 

                if (mobs == null)
                {
                    return;
                }

                foreach (var m in mobs)
                {
                    if (MenuConfig.JnglQ && Spells.Q.IsReady())
                    {
                        CastQ(m);
                    }

                    if (!Spells.W.IsReady() || !MenuConfig.JnglW || Player.HasBuff("RivenFeint"))
                    {
                        return;
                    }

                    CastW(m);
                }
            }

            if (!Spells.Q.IsReady() || !MenuConfig.LaneQ)
            {
                return;
            }

            var nexus = args.Target as Obj_HQ;

            if (nexus != null && nexus.IsValid)
            {
                CastQ(nexus);
            }

            var inhib = args.Target as Obj_BarracksDampener;

            if (inhib != null && inhib.IsValid)
            {
                CastQ(inhib);
            }

            var turret = args.Target as Obj_AI_Turret;

            if (turret == null || !turret.IsValid)
            {
                return;
            }

           CastQ(turret);
        }

        #endregion
    }
}