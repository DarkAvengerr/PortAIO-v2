using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using hYasuo.Extensions;
using hYasuo.Logics;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy;
using LeagueSharp.Common;
namespace hYasuo.Champions
{
    public class Yasuo
    {
        public Yasuo()
        {
            OnLoad();
        }

        private static void OnLoad()
        {
            Spells.Initialize();
            SpellDatabase.InitalizeSpellDatabase();
            Menus.Initialize();

            //Obj_AI_Base.OnProcessSpellCast += YasuoWW.YasuoWindWallProtector;
            // Obj_AI_Base.OnProcessSpellCast += YasuoWW.YasuoTargettedProtector;
            Game.OnUpdate += OnUpdate;

        }

        private static void OnUpdate(EventArgs args)
        {
            switch (Menus.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    OnHarass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    OnClear();
                    OnJungle();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    OnLastHit();
                    break;
            }

            if (Utilities.Enabled("ks.status"))
            {
                var target = TargetSelector.GetTarget(1100f, TargetSelector.DamageType.Physical);

                if (Spells.Q.IsReady() && Utilities.Enabled("q.ks") && target.IsValidTarget(Spells.Q.Range)
                    && !Spells.Q.Empowered() && Spells.Q.GetDamage(target) > target.Health)
                {
                    Spells.Q.Cast(target.Position);
                }

                if (Spells.Q1.IsReady() && Utilities.Enabled("q2.ks") && target.IsValidTarget(Spells.Q1.Range)
                   && Spells.Q.Empowered() && Spells.Q1.GetDamage(target) > target.Health)
                {
                    Spells.Q1.Cast(target.Position);
                }

                if (Spells.E.IsReady() && Utilities.Enabled("e.ks") && target.IsValidTarget(Spells.E.Range)
                    && ObjectManager.Player.CountEnemiesInRange(Utilities.Slider("e.ks.safety.range")) <= 1
                    && Spells.E.GetDamage(target) > target.Health)
                {
                    Spells.E.CastOnUnit(target);
                }

                if (Spells.R.IsReady() && Utilities.Enabled("r.ks") && target.IsValidTarget(Spells.R.Range)
                    && ObjectManager.Player.CountEnemiesInRange(Utilities.Slider("r.ks.safety.range")) <= 1
                    && Spells.R.GetDamage(target) > target.Health)
                {
                    Spells.R.Cast();
                }
            }
            if (Menus.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None && Utilities.Enabled("auto.stack")
                && !Spells.Q.Empowered() && Spells.Q.IsReady())
            {
                foreach (var minions in MinionManager.GetMinions(Spells.Q.Range).Where(x => x.IsValid))
                {
                    Spells.Q.Cast(minions);
                }
            }
        }

        private static void OnLastHit()
        {

            if (Spells.Q.IsReady() && Utilities.Enabled("q.lasthit") && !Spells.Q.Empowered())
            {
                foreach (var minion in MinionManager.GetMinions(Spells.Q.Range)
                    .Where(x => x.IsValid && Spells.Q.GetDamage(x) > x.Health))
                {
                    Spells.Q.Cast(minion.Position);
                }
            }

            if (Spells.E.IsReady() && Utilities.Enabled("e.lasthit"))
            {
                foreach (var minion in MinionManager.GetMinions(Spells.E.Range)
                    .Where(x => x.IsValid && Spells.E.GetDamage(x) > x.Health))
                {
                    if (!YasuoE.GetDashingEnd(minion).To3D().UnderTurret(true)) // turret check for dash end pos e
                    {
                        Spells.E.CastOnUnit(minion);
                    }
                }
            }
        }

        private static void OnClear()
        {

            if (Spells.Q.IsReady() && Utilities.Enabled("q.clear") && !Spells.Q.Empowered())
            {
                var min = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.Q.Range);
                if (Spells.Q.GetLineFarmLocation(min).MinionsHit >= Utilities.Slider("q.hit.x.minion"))
                {
                    Spells.Q.Cast(Spells.Q.GetLineFarmLocation(min).Position);
                }
            }

            if (Spells.Q1.IsReady() && Utilities.Enabled("q2.clear") && Spells.Q.Empowered())
            {
                var min = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.Q1.Range);
                if (Spells.Q1.GetLineFarmLocation(min).MinionsHit >= Utilities.Slider("q2.hit.x.minion"))
                {
                    Spells.Q1.Cast(Spells.Q1.GetLineFarmLocation(min).Position);
                }
            }

            if (Spells.E.IsReady() && Utilities.Enabled("e.clear"))
            {
                var miniondashlist = MinionManager.GetMinions(
                    Spells.E.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth)
                    .Where(YasuoE.IsDashable).ToList();

                YasuoE.DashPos(Game.CursorPos, miniondashlist, true);
            }
        }

        private static void OnJungle()
        {
            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 1100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mobs == null || (mobs.Count == 0))
            {
                return;
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.jungle") &&
                !Spells.Q.Empowered() && mobs[0].IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.Cast(mobs[0].Position);
            }

            if (Spells.Q1.IsReady() && Utilities.Enabled("q2.jungle") &&
                           Spells.Q.Empowered() && mobs[0].IsValidTarget(Spells.Q1.Range))
            {
                Spells.Q1.Cast(mobs[0].Position);
            }

            if (Spells.E.IsReady() && Utilities.Enabled("e.jungle") &&
                mobs[0].IsValidTarget(Spells.E.Range))
            {
                Spells.E.CastOnUnit(mobs[0]);
            }

        }

        private static void OnHarass()
        {
            var target = TargetSelector.GetTarget(1110f, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                if (Spells.Q.IsReady() && !Spells.Q.Empowered()
                && Utilities.Enabled("q.harass") && target.IsValidTarget(Spells.Q.Range))
                {
                    var pred = Spells.Q.GetPrediction(target);
                    if (pred.Hitchance >= Utilities.HikiChance("q.hitchance"))
                    {
                        Spells.Q.Cast(pred.CastPosition);
                    }
                }

                if (Spells.Q1.IsReady() && Spells.Q.Empowered() && Utilities.Enabled("q2.harass")
                    && target.IsValidTarget(Spells.Q1.Range))
                {
                    var pred = Spells.Q1.GetPrediction(target);
                    if (pred.Hitchance >= Utilities.HikiChance("q2.hitchance"))
                    {
                        Spells.Q1.Cast(pred.CastPosition);
                    }
                }
            }
            else
            {
                if (Spells.Q.IsReady() && Utilities.Enabled("q.lasthit") && !Spells.Q.Empowered())
                {
                    foreach (var minion in MinionManager.GetMinions(Spells.Q.Range)
                        .Where(x => x.IsValid && Spells.Q.GetDamage(x) > x.Health))
                    {
                        Spells.Q.Cast(minion.Position);
                    }
                }
            }
        }

        private static void OnCombo()
        {
            var dashlist = ObjectManager.Get<Obj_AI_Base>().Where(o => o.IsValidTarget(Spells.E.Range) && YasuoE.IsDashable(o) && (o.IsChampion() || o.IsMinion)).ToList();
            var target = TargetSelector.GetTarget(1100f, TargetSelector.DamageType.Physical);

            if (Spells.E.IsReady() && Utilities.Enabled("e.combo"))
            {
                YasuoE.DashPos(Game.CursorPos, dashlist, true);
            }

            if (target != null)
            {
                if (Spells.Q.IsReady() && Utilities.Enabled("q.combo") && target.IsValidTarget(Spells.Q.Range)
                    && !Spells.Q.Empowered())
                {
                    var pred = Spells.Q.GetPrediction(target);
                    if (pred.Hitchance >= Utilities.HikiChance("q.hitchance"))
                    {
                        Spells.Q.Cast(pred.CastPosition);
                    }
                }

                if (Spells.Q1.IsReady() && Utilities.Enabled("q2.combo") &&
                    target.IsValidTarget(Spells.Q1.Range) && Spells.Q.Empowered())
                {
                    Chat.Print("succses");
                    var pred = Spells.Q1.GetPrediction(target);
                    if (pred.Hitchance >= Utilities.HikiChance("q2.hitchance"))
                    {
                        Spells.Q1.Cast(pred.CastPosition);
                    }
                }

                if (Spells.R.IsReady() &&
                    ObjectManager.Player.CountEnemiesInRange(Spells.R.Range) >= Utilities.Slider("min.r.count")
                    && Utilities.Enabled("r.combo"))
                {
                    var enemylist = HeroManager.Enemies.Where(x => x.IsValidTarget(Spells.R.Range)
                    && (x.HasBuff("yasuoq3mis") || x.HasBuffOfType(BuffType.Knockup)
                    || x.HasBuffOfType(BuffType.Knockback))).ToList();

                    if (enemylist.Count >= Utilities.Slider("min.r.count")) //knockup count
                    {
                        Spells.R.Cast();
                    }
                }
            }
        }
    }
}


