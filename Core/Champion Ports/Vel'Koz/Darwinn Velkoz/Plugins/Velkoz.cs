using System;
using System.Linq;
using Darwinn_s_velkoz.Extensions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Utilities = Darwinn_s_velkoz.Extensions.Utilities;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Darwinn_s_velkoz.Plugins
{
    internal class Velkoz
    {

        private MissileClient QMissile = null;

        public Velkoz()
        {
            VelkozOnLoad();
        }

        private static void VelkozOnLoad()
        {

            Spells.Initialize();
            Menus.Initialize();

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnNewPath += OnNewPath;

        }

        private void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsValid<MissileClient>() && sender.IsAlly)
            {
                MissileClient missile = (MissileClient)sender;
                if (missile.SData.Name != null && missile.SData.Name == "VelkozQMissile")
                    QMissile = missile;
            }
        }

        private static void OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.IsEnemy && sender is AIHeroClient && args.IsDash && Utilities.Enabled("block.dashes")
                && sender.IsValidTarget(Spells.E.Range) && Spells.E.IsReady())
            {
                var starttick = Utils.TickCount;
                var speed = args.Speed;
                var startpos = sender.ServerPosition.To2D();
                var path = args.Path;
                var forch = path.OrderBy(x => starttick + (int)(1000 * (new Vector3(x.X, x.Y, x.Z).
                    Distance(startpos.To3D()) / speed))).FirstOrDefault();
                {
                    var endpos = new Vector3(forch.X, forch.Y, forch.Z);
                    var endtick = starttick + (int)(1000 * (endpos.Distance(startpos.To3D())
                        / speed));
                    var duration = endtick - starttick;

                    if (duration < starttick)
                    {
                        Spells.E.Cast(endpos);
                    }
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            switch (Menus.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    OnMixed();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    OnJungle();
                    OnClear();
                    break;
            }

        }

        public static void OnCombo()
        {
            var target = TargetSelector.GetTarget(Spells.R.Range, TargetSelector.DamageType.Magical);

            if (target != null)
            {
                if (Spells.Q.IsReady() && Utilities.Enabled("q.combo") && target.IsValidTarget(Spells.Q.Range) && !Utilities.IsInvulnerable(target, DamageType.Magical))
                {
                    Spells.Q.HikiCast(target, Utilities.HikiChance("hitchance"), "prediction", Menus.Config);
                }

                if (Spells.W.IsReady() && Utilities.Enabled("w.combo") && target.IsValidTarget(Spells.W.Range))
                {
                    Spells.W.HikiCast(target, Utilities.HikiChance("hitchance"), "prediction", Menus.Config);
                }

                if (Spells.E.IsReady() && Utilities.Enabled("e.combo") && target.IsValidTarget(Spells.E.Range) && !Utilities.IsInvulnerable(target, DamageType.Magical))
                {
                    Spells.E.HikiCast(target, Utilities.HikiChance("hitchance"), "prediction", Menus.Config);
                }

            }

        }

        private static void OnMixed()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("harassmana"))
            {
                return;
            }

            var target = TargetSelector.GetTarget(Spells.E.Range, TargetSelector.DamageType.Magical);

            if (target != null)
            {
                if (Spells.Q.IsReady() && Utilities.Enabled("q.harass") && target.IsValidTarget(Spells.Q.Range) && !Utilities.IsInvulnerable(target, DamageType.Magical))
                {
                    Spells.Q.HikiCast(target, Utilities.HikiChance("hitchance"), "prediction", Menus.Config);
                }

                if (Spells.E.IsReady() && Utilities.Enabled("e.harass") && target.IsValidTarget(Spells.E.Range) && !Utilities.IsInvulnerable(target, DamageType.Magical))
                {
                    Spells.E.HikiCast(target, Utilities.HikiChance("hitchance"), "prediction", Menus.Config);
                }


            }
        }

        private static void OnJungle()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("junglemana"))
            {
                return;
            }

            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mob == null || mob.Count == 0)
            {
                return;
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.jungle") && mob[0].IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.Cast(mob[0].Position);
            }

            if (Spells.W.IsReady() && Utilities.Enabled("w.jungle") && mob[0].IsValidTarget(Spells.W.Range))
            {
                Spells.W.Cast(mob[0].Position);
            }

            if (Spells.E.IsReady() && Utilities.Enabled("e.jungle") && mob[0].IsValidTarget(Spells.E.Range))
            {
                Spells.E.Cast(mob[0].Position);
            }

        }

        private static void OnClear()
        {

            if (ObjectManager.Player.ManaPercent < Utilities.Slider("clearmana"))
            {
                return;
            }

            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.E.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (minions == null || minions.Count == 0)
            {
                return;
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.clear"))
            {
                var minionloc = Spells.E.GetLineFarmLocation(minions);
                foreach (var min in minions.Where(x => x.IsValidTarget(Spells.Q.Range) && x.Health < Spells.Q.GetDamage(x)))
                {
                    Spells.Q.Cast(minionloc.Position);
                }
            }

            if (Spells.W.IsReady() && Utilities.Enabled("w.clear"))
            {
                var minionloc = Spells.W.GetLineFarmLocation(minions);

                {
                    Spells.W.Cast(minionloc.Position);
                }
            }

            if (Spells.E.IsReady() && Utilities.Enabled("e.clear"))
            {
                var minionloc = Spells.E.GetLineFarmLocation(minions);
                {
                    Spells.E.Cast(minionloc.Position);
                }
            }
        }

    }
}