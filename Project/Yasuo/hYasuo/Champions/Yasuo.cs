using System;
using System.Linq;
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

        public static int HydraID => 3074;
        public static int TiamatID => 3074;

        private static void OnLoad()
        {
            Spells.Initialize();
            SpellDatabase.InitalizeSpellDatabase();
            Menus.Initialize();

            Obj_AI_Base.OnProcessSpellCast += YasuoWW.YasuoWindWallProtector;
            Obj_AI_Base.OnProcessSpellCast += YasuoWW.YasuoTargettedProtector;
            Obj_AI_Base.OnProcessSpellCast += YasuoE.GetTime;
            Obj_AI_Base.OnProcessSpellCast += YasuoHydra;
            Obj_AI_Base.OnProcessSpellCast += YasuoIncomingDamage.IncomingDamage;
            Obj_AI_Base.OnSpellCast += YasuoQReset;

            Game.OnUpdate += OnUpdate;

        }

        private static void YasuoQReset(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || Menus.Config.Item("q.type").GetValue<StringList>().SelectedIndex != 1)
            {
                return;
            }

            if (args.Target.IsEnemy && args.SData.IsAutoAttack() && 
                args.Target.Type == GameObjectType.AIHeroClient)
            {
                if (Spells.Q.IsReady() && Utilities.Enabled("q.combo") && 
                    ((AIHeroClient)args.Target).IsValidTarget(Spells.Q.Range)
                    && !Spells.Q.Empowered())
                {
                    var pred = Spells.Q.GetPrediction(((AIHeroClient)args.Target));
                    if (pred.Hitchance >= Utilities.HikiChance("q.hitchance"))
                    {
                        Spells.Q.Cast(pred.CastPosition);
                    }
                }
            }
        }

        private static void YasuoHydra(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (!args.SData.IsAutoAttack() && (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.E)
                && args.Target.IsEnemy && args.Target.Type == GameObjectType.AIHeroClient)
            {
                UseItemForCancel();
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

            if (Menus.Config.Item("flee.key").GetValue<KeyBind>().Active)
            {
                Orbwalking.Orbwalk(null, Game.CursorPos);
                var dashlist = ObjectManager.Get<Obj_AI_Base>().Where(o => o.IsValidTarget(Spells.E.Range) 
                && YasuoE.IsDashable(o)).ToList();

                if (Spells.E.IsReady())
                {
                    YasuoE.DashPos(Game.CursorPos, dashlist,false);
                }
            }

            if (Menus.Config.Item("toggle.active").GetValue<KeyBind>().Active)
            {
                var target = TargetSelector.GetTarget(Spells.Q3.Range, TargetSelector.DamageType.Physical);

                if (Spells.Q.IsReady() && Menus.Config.Item("q.toggle").GetValue<bool>() 
                    && target.IsValidTarget(Spells.Q.Range) && !Spells.Q.Empowered())
                {
                    Spells.Q.Do(target, Utilities.HikiChance("q.hitchance"), true);
                }

                if (Spells.Q3.IsReady() && Menus.Config.Item("q3.toggle").GetValue<bool>()
                    && target.IsValidTarget(Spells.Q3.Range) && Spells.Q.Empowered())
                {
                    Spells.Q.Do(target, Utilities.HikiChance("q3.hitchance"), true);
                }
            }

            if (Utilities.Enabled("ks.status"))
            {
                var target = TargetSelector.GetTarget(1100f, TargetSelector.DamageType.Physical);

                if (Spells.Q.IsReady() && Utilities.Enabled("q.ks") && target.IsValidTarget(Spells.Q.Range)
                    && !Spells.Q.Empowered() && Spells.Q.GetDamage(target) > target.Health)
                {
                    Spells.Q.Do(target, Utilities.HikiChance("q.hitchance"), true);
                }

                if (Spells.Q3.IsReady() && Utilities.Enabled("q2.ks") && target.IsValidTarget(Spells.Q3.Range)
                   && Spells.Q.Empowered() && Spells.Q3.GetDamage(target) > target.Health)
                {
                    Spells.Q.Do(target, Utilities.HikiChance("q3.hitchance"), true);
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
                foreach (var minions in MinionManager.GetMinions(Spells.Q.Range).
                    Where(x=> x.IsValid && x.Health < Spells.Q.GetDamage(x) && x.IsValidTarget(Spells.Q.Range)))
                {
                    Spells.Q.Do(minions, Utilities.HikiChance("q.hitchance"), true);
                }
            }
        }

        public static string HasCancelableItems()
        {
            if (Items.HasItem(TiamatID))
            {
                return "Tiamat";
            }
            else if (Items.HasItem(HydraID))
            {
                return "Hydra";
            }
            return null;
        }

        public static void UseItemForCancel()
        {
            if (HasCancelableItems() != null && HasCancelableItems() == "Tiamat" && Items.CanUseItem(TiamatID))
            {
                Items.UseItem(TiamatID);
            }
            if (HasCancelableItems() != null && HasCancelableItems() == "Hydra" && Items.CanUseItem(HydraID))
            {
                Items.UseItem(HydraID);
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

            if (Spells.Q3.IsReady() && Utilities.Enabled("q3.clear") && Spells.Q.Empowered())
            {
                var min = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.Q3.Range);
                if (Spells.Q3.GetLineFarmLocation(min).MinionsHit >= Utilities.Slider("q3.hit.x.minion"))
                {
                    Spells.Q3.Cast(Spells.Q3.GetLineFarmLocation(min).Position);
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

            if (Spells.Q3.IsReady() && Utilities.Enabled("q3.jungle") &&
                           Spells.Q.Empowered() && mobs[0].IsValidTarget(Spells.Q3.Range))
            {
                Spells.Q3.Cast(mobs[0].Position);
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
                    Spells.Q.Do(target, Utilities.HikiChance("q.hitchance"), true);
                }

                if (Spells.Q3.IsReady() && Spells.Q.Empowered() && Utilities.Enabled("q3.harass")
                    && target.IsValidTarget(Spells.Q3.Range))
                {
                    Spells.Q.Do(target, Utilities.HikiChance("q3.hitchance"), true);
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

                if (Spells.E.IsReady() && Utilities.Enabled("e.lasthit") && !Spells.Q.Empowered())
                {
                    foreach (var minion in MinionManager.GetMinions(Spells.E.Range)
                            .Where(x => x.IsValid && Spells.E.GetDamage(x) > x.Health))
                    {
                        if (!YasuoE.GetDashingEnd(minion).To3D().UnderTurret(true))
                        {
                            Spells.E.CastOnUnit(minion);
                        }
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
                if (!Utilities.Enabled("disable.e.safety"))
                {
                    if (Utilities.Enabled("eqq.combo"))
                    {
                        YasuoE.DashPos(Game.CursorPos, dashlist, false);
                    }
                    else
                    {
                        if (Game.Time - YasuoE.lastetime >= 1)
                        {
                            YasuoE.DashPos(Game.CursorPos, dashlist, false);
                        }
                    }
                }
                else
                {
                    if (Utilities.Enabled("eqq.combo"))
                    {
                        YasuoE.DashPos(Game.CursorPos, dashlist, true);
                    }
                    else
                    {
                        if (Game.Time - YasuoE.lastetime >= 1)
                        {
                            YasuoE.DashPos(Game.CursorPos, dashlist, true);
                        }
                    }
                }
                
            }

            if (target != null)
            {
                if (Menus.Config.Item("q.type").GetValue<StringList>().SelectedIndex == 0)
                {
                    if (Spells.Q.IsReady() && Utilities.Enabled("q.combo") && target.IsValidTarget(Spells.Q.Range)
                        && !Spells.Q.Empowered())
                    {
                        Spells.Q.Do(target, Utilities.HikiChance("q.hitchance"), true);
                    }
                }

                if (Spells.Q3.IsReady() && Utilities.Enabled("q3.combo") &&  
                    target.IsValidTarget(Spells.Q3.Range) && Spells.Q.Empowered())
                {
                    Spells.Q.Do(target, Utilities.HikiChance("q3.hitchance"), true);
                }
                
                if (Spells.R.IsReady() && 
                    ObjectManager.Player.CountEnemiesInRange(Spells.R.Range) >= Utilities.Slider("min.r.count")
                    && Utilities.Enabled("r.combo"))
                {
                    var enemylist = HeroManager.Enemies.Where(x=> x.IsValidTarget(Spells.R.Range) 
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


