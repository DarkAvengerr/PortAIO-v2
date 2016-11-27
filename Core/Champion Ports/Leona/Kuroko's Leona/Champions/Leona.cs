using System;
using System.Linq;
using Kuroko_s_Leona.Extensions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Utilities = Kuroko_s_Leona.Extensions.Utilities;
using Color = System.Drawing.Color;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Kuroko_s_Leona.Champions
{
    internal class Leona
    {
        public Leona()
        {
            LeonaOnLoad();
        }

        private static void LeonaOnLoad()
        {

            Spells.Initialize();
            Menus.Initialize();

            Game.OnUpdate += OnUpdate;
            Drawing.OnEndScene += OnEndScene;
            Obj_AI_Base.OnNewPath += OnNewPath;

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

        private static void OnEndScene(EventArgs args)
        {
            if (Utilities.Enabled("r.range.on.minimap") && Spells.R.Level > 0)
            {
                Utilities.DrawCircle(ObjectManager.Player.Position, Spells.R.Range, Color.Gold,1,30,true);
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
                if (Spells.Q.IsReady() && Utilities.Enabled("q.combo") && target.IsValidTarget(Spells.Q.Range) && !Utilities.IsInvulnerable(target,DamageType.Magical))
                {
                    Spells.Q.Cast();
                }

                if (Spells.W.IsReady() && Utilities.Enabled("w.combo") && target.IsValidTarget(Spells.W.Range))
                {
                    Spells.W.Cast();
                }

                if (Spells.E.IsReady() && Utilities.Enabled("e.combo") && target.IsValidTarget(Spells.E.Range) && !Utilities.IsInvulnerable(target,DamageType.Magical))
                {
                    Spells.E.HikiCast(target, Utilities.HikiChance("hitchance"), "prediction", Menus.Config);
                }

                if (Spells.R.IsReady() && Utilities.Enabled("r.combo") && target.IsValidTarget(Spells.R.Range) && !Utilities.IsInvulnerable(target,DamageType.Magical))
                {
                    if (Utilities.Enabled("r."+target.ChampionName))
                    {
                        Spells.R.HikiCast(target, Utilities.HikiChance("hitchance"), "prediction", Menus.Config);
                    }
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
                    Spells.Q.Cast();
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
                Spells.Q.Cast();
            }

            if (Spells.W.IsReady() && Utilities.Enabled("w.jungle") && mob[0].IsValidTarget(Spells.W.Range))
            {
                Spells.W.Cast();
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

            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.E.Range, MinionTypes.All,MinionTeam.NotAlly);
           
            if (minions == null || minions.Count == 0)
            {
                return;
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.clear"))
            {
                foreach (var min in minions.Where(x=> x.IsValidTarget(Spells.Q.Range) && x.Health < Spells.Q.GetDamage(x)))
                {
                    Spells.Q.Cast();
                }
            }

            if (Spells.W.IsReady() && Utilities.Enabled("w.clear"))
            {
                var minionloc = Spells.W.GetCircularFarmLocation(minions);
                if (minionloc.MinionsHit >= Utilities.Slider("w.min.count"))
                {
                    Spells.W.Cast();
                }
            }

            if (Spells.E.IsReady() && Utilities.Enabled("e.clear"))
            {
                var minionloc = Spells.E.GetLineFarmLocation(minions);
                if (minionloc.MinionsHit >= Utilities.Slider("e.min.count"))
                {
                    Spells.E.Cast(minionloc.Position);
                }
            }
        }

    }
}
