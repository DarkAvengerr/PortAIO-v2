using System;
using System.Linq;
using hCamille.Extensions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Utilities = hCamille.Extensions.Utilities;

using EloBuddy; 
using LeagueSharp.Common; 
namespace hCamille.Champions
{
    class Camille
    {

        public Camille()
        {
            Spells.Initializer();
            Menus.Initializer();

            Game.OnUpdate += CamilleOnUpdate;
            Obj_AI_Base.OnSpellCast += CamilleOnSpellCast;

        }

        private void CamilleOnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.IsAutoAttack() && Menus.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                && Menus.Config.Item("q.mode").GetValue<StringList>().SelectedIndex == 0 && Spells.Q.IsReady() && Utilities.Enabled("q.combo"))
            {
                Spells.Q.Cast();
            }
        }

        private void CamilleOnUpdate(EventArgs args)
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



        private static void OnCombo()
        {

            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                if (Spells.Q.IsReady() && Utilities.Enabled("q.combo") && target.IsValidTarget(ObjectManager.Player.AttackRange)
                    && Menus.Config.Item("q.mode").GetValue<StringList>().SelectedIndex == 1)
                {
                    Spells.Q.Cast();
                }

                if (Spells.W.IsReady() && Utilities.Enabled("w.combo") && target.IsValidTarget(Spells.W.Range))
                {
                    var pred = Spells.W.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.Medium)
                    {
                        Spells.W.Cast(pred.CastPosition);
                    }
                }

                if (Spells.E.IsReady() && Utilities.Enabled("e.combo") && target.IsValidTarget(Spells.E.Range))
                {
                    var polygon = new Geometry.Polygon.Circle(target.Position, 600).Points.FirstOrDefault(x => NavMesh.GetCollisionFlags(x.X, x.Y).HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(x.X, x.Y).HasFlag(CollisionFlags.Building));

                    if (new Vector2(polygon.X, polygon.Y).Distance(ObjectManager.Player.Position) < Spells.E.Range)
                    {
                        Spells.E.Cast(new Vector2(polygon.X, polygon.Y));
                    }
                }

                if (Spells.R.IsReady() && Utilities.Enabled("r.combo") && target.IsValidTarget(Spells.R.Range))
                {
                    if (target.HealthPercent < Utilities.Slider("enemy.health.percent") && Utilities.Enabled("r." + target.ChampionName))
                    {
                        Spells.R.CastOnUnit(target);
                    }
                }
            }
            
        }

        private static void OnMixed()
        {
            var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                if (Spells.Q.IsReady() && Utilities.Enabled("q.combo") && target.IsValidTarget(ObjectManager.Player.AttackRange))
                {
                    Spells.Q.Cast();
                }

                if (Spells.W.IsReady() && Utilities.Enabled("w.combo") && target.IsValidTarget(Spells.W.Range))
                {
                    var pred = Spells.W.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        Spells.W.Cast(pred.CastPosition);
                    }
                }
            }
        }

        private static void OnJungle()
        {
            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mob == null || mob.Count == 0)
            {
                return;
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.jungle"))
            {
                Spells.Q.Cast();
            }

            if (Spells.W.IsReady() && Utilities.Enabled("w.jungle"))
            {
                Spells.W.Cast(mob[0].Position);
            }
            
        }

        private static void OnClear()
        {
            if (Spells.Q.IsReady() && Utilities.Enabled("q.clear"))
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.W.Range, MinionTypes.All,
                MinionTeam.NotAlly);

                var minioncount = Spells.W.GetLineFarmLocation(minions);
                if (minions == null || minions.Count == 0)
                {
                    return;
                }

                if (minioncount.MinionsHit >= Utilities.Slider("min.count"))
                {
                    Spells.W.Cast(minioncount.Position);
                }
            }
        }
    }
}
