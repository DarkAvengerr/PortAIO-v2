using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event.OrbwalkingModes
{
    #region

    using System.Linq;

    using Core;

    using LeagueSharp;
    using LeagueSharp.Common;

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
                if (target.HasBuff("FioraW") && Qstack == 3)
                {
                    return;
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if (Spells.Q.IsReady())
                    {
                        Usables.CastYoumoo();
                        BackgroundData.CastQ(target);
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    if (Qstack == 2)
                    {
                        BackgroundData.CastQ(target);
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.QuickHarass)
                {
                    if (Spells.Q.IsReady() && Qstack == 1)
                    {
                        BackgroundData.CastQ(target);
                    }

                    if (Spells.W.IsReady() && BackgroundData.InRange(target))
                    {
                        BackgroundData.CastW(target);
                        BackgroundData.DoubleCastQ(target);
                    }
                }

                if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Burst) return;

                if (Spells.Q.IsReady())
                {
                    BackgroundData.CastQ(target);
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

                var minions = MinionManager.GetMinions(Player.AttackRange + 360);

                foreach (var m in minions)
                {
                    if (!MenuConfig.LaneQ || (m.UnderTurret(true) && ObjectManager.Player.CountEnemiesInRange(1500) >= 1))
                    {
                        return;
                    }

                    if (Spells.Q.IsReady())
                    {
                        BackgroundData.CastQ(m);
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
                        BackgroundData.CastQ(m);
                    }

                    if (!Spells.W.IsReady() || !MenuConfig.JnglW || Player.HasBuff("RivenFeint") || !BackgroundData.InRange(m))
                    {
                        return;
                    }

                    BackgroundData.CastW(m);
                }
            }

            if (!Spells.Q.IsReady() || !MenuConfig.LaneQ)
            {
                return;
            }

            var nexus = args.Target as Obj_HQ;

            if (nexus != null && nexus.IsValid)
            {
                IsGameObject = true;
                Spells.Q.Cast(nexus.Position - 500);
            }

            var inhib = args.Target as Obj_BarracksDampener;

            if (inhib != null && inhib.IsValid)
            {
                IsGameObject = true;
                Spells.Q.Cast(inhib.Position - 250);
            }

            var turret = args.Target as Obj_AI_Turret;

            if (turret == null || !turret.IsValid)
            {
                return;
            }

            IsGameObject = true;
            Spells.Q.Cast(turret.Position - 250);
        }

        #endregion
    }
}