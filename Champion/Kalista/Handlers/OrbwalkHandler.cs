using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using S_Plus_Class_Kalista.Libaries;
using Damage = S_Plus_Class_Kalista.Libaries.Damage;
using EloBuddy;

namespace S_Plus_Class_Kalista.Handlers
{
    internal class OrbwalkHandler : Core
    {
        private const string _MenuNameBase = ".Orbwalker.Mode Menu";
        private const string _MenuItemBase = "Orbwalker.Mode.";

        public static void Load()
        {
            //SMenu.AddSubMenu(new Menu(".LukeSkywalker", ".LukeSkywalker"));
            //LukeOrbwalker = new LukeSkywalker.Orbwalker(SMenu.SubMenu(".LukeSkywalker"));
            //Game.OnUpdate += OnUpdate;
            //SMenu.AddSubMenu(_Menu());

            SMenu.AddSubMenu(new Menu(".Orbwalker", ".Orbwalker"));
            CommonOrbwalker = new Orbwalking.Orbwalker(SMenu.SubMenu(".CommonOrbwalker"));
            Orbwalking.OnNonKillableMinion += RendCheck.CheckNonKillables;
            Game.OnUpdate += OnUpdate;
            SMenu.AddSubMenu(_Menu());
        }

        private static void OnUpdate(EventArgs args)
        {
            HandleMode();
        }
        private static Menu _Menu()
        {
            var menu = new Menu(_MenuNameBase, "lukeskywalkerModeMenu");
            menu.AddItem(new MenuItem(_MenuItemBase + "Boolean.MinonOrbwalk", "Use Minion Combo-Walk v2(BETA)").SetValue(true));
            var subMenuCombo = new Menu(".Combo", "comboMenu");
            subMenuCombo.AddItem(new MenuItem(_MenuItemBase + "Combo.Boolean.UseQ", "Use Q").SetValue(true));
            subMenuCombo.AddItem(
                new MenuItem(_MenuItemBase + "Combo.Boolean.UseQ.Reset", "Use Q AA reset(Safe Exploit)").SetValue(false));
            subMenuCombo.AddItem(
                new MenuItem(_MenuItemBase + "Combo.Boolean.UseQ.Prediction", "Q prediction").SetValue(
                    new StringList(new[] {"Very High", "High", "Dashing"})));
            //subMenuCombo.AddItem(new MenuItem(_MenuItemBase + "Combo.Boolean.Rend.KillEnemies", "Use Rend to Kill Enemies").SetValue(false));

            var subMenuMixed = new Menu(".Mixed", "mixedMenu");
            subMenuMixed.AddItem(new MenuItem(_MenuItemBase + "Mixed.Boolean.UseQ", "Use Q").SetValue(true));
            subMenuMixed.AddItem(
                new MenuItem(_MenuItemBase + "Mixed.Boolean.UseQ.Reset", "Use Q AA reset(Exploit)").SetValue(false));
            subMenuMixed.AddItem(
                new MenuItem(_MenuItemBase + "Mixed.Boolean.UseQ.Prediction", "Q prediction").SetValue(
                    new StringList(new[] {"Very High", "High", "Dashing"})));
            subMenuMixed.AddItem(new MenuItem(_MenuItemBase + "Mixed.Boolean.Rend", "Use Rend on stacks").SetValue(false));
            subMenuMixed.AddItem(
                new MenuItem(_MenuItemBase + "Mixed.Boolean.Rend.Stacks", ">> Required E stacks").SetValue(new Slider(
                    4, 2, 15)));


            var subMenuClear = new Menu(".Clear", "clearMenu");
            //subMenuClear.AddItem(
            //    new MenuItem(_MenuItemBase + "Clear.Boolean.UseQ.Minions", "Use Q to kill minions").SetValue(true));
            subMenuClear.AddItem(new MenuItem(_MenuItemBase + "Clear.Boolean.Rend.Minions", "Rend Minions").SetValue(true));
            subMenuClear.AddItem(
                new MenuItem(_MenuItemBase + "Clear.Boolean.Rend.Minions.Killed", ">> Required minions killed").SetValue
                    (new Slider(2, 1, 5)));
            //subMenuClear.AddItem(
            //    new MenuItem(_MenuItemBase + "Clear.Boolean.UseQ.Prediction", "Q prediction").SetValue(
            //        new StringList(new[] {"Very High", "High", "Dashing"})));
            //subMenuClear.AddItem(
            //    new MenuItem(_MenuItemBase + "Clear.Boolean.Rend.Minions", "Use Rend to kill minions").SetValue(false));
            //subMenuClear.AddItem(
            //    new MenuItem(_MenuItemBase + "Clear.Boolean.Rend.Minions.Killed", ">> Required minions killed").SetValue
            //        (new Slider(4, 2, 15)));


            menu.AddSubMenu(subMenuCombo);
            menu.AddSubMenu(subMenuMixed);
            menu.AddSubMenu(subMenuClear);
            return menu;
        }
    
        private static void OrbWalkMinions()
        {
            if (!SMenu.Item(_MenuItemBase + "Boolean.MinonOrbwalk").GetValue<bool>()) return;
            if (CommonOrbwalker.GetTarget() != null) return;

            var enemiesHero = HeroManager.Enemies.Where(x => x.IsEnemy && ObjectManager.Player.LSDistance(x) <= Orbwalking.GetRealAutoAttackRange(x));
            if (enemiesHero.Any()) return; // There is a champion we can attack

            var target = TargetSelector.GetTarget(Champion.E.Range * 1.2f, TargetSelector.DamageType.Physical); // Champion in E range

            var autoMinions = MinionManager.GetMinions(Player.Position, Orbwalking.GetRealAutoAttackRange(Player), MinionTypes.All, MinionTeam.NotAlly);
            if (!autoMinions.Any()) return;

            if (target != null && target.GetBuffCount("kalistaexpungemarker") <= 0)// There is a target with a rend stack
            {

                if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return;
                var rendMinions = MinionManager.GetMinions(Player.ServerPosition, Champion.E.Range);
                var count = rendMinions.Count(minion => minion.Health <= Damage.DamageCalc.CalculateRendDamage(minion) && minion.IsValid);

                if (count > 0) // Use Rend
                {
                    Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
                    Champion.E.Cast();
                    return;
                }
            }

            //if no other event occurs
            foreach (var minion in autoMinions)
            {
                if (Vector3.Distance(ObjectManager.Player.ServerPosition, minion.Position) > Orbwalking.GetRealAutoAttackRange(Player) + 50) continue;
                if (minion.CharData.BaseSkinName == "gangplankbarrel") continue;
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                return;
            }

        }

        private static void HandleMode()
        {
            switch (CommonOrbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    OrbWalkMinions();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Mixed();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    //LastHit();
                    break;
            }
        }


        private static HitChance GetHitChance(int index)
        {
            switch (index)
            {
                case 0:
                    return HitChance.VeryHigh;
                case 1:
                    return HitChance.High;
                case 2:
                    return HitChance.Dashing;
            }
            return HitChance.VeryHigh;
        }

        private static void LaneClear()
        {

            if (SMenu.Item(_MenuItemBase + "Clear.Boolean.Rend.Minions").GetValue<bool>() && Champion.E.LSIsReady())
            {
                if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay"))
                {
                    var minions = MinionManager.GetMinions(Player.ServerPosition, Champion.E.Range);
                    var count =
                        minions.Count(
                            minion => minion.Health <= Damage.DamageCalc.CalculateRendDamage(minion) && minion.IsValid);

                    if (SMenu.Item(_MenuItemBase + "Clear.Boolean.Rend.Minions.Killed").GetValue<Slider>().Value > count)
                    {
                        if (true)//ManaHandler.UseModeE())
                        {
                            Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
                            Champion.E.Cast();
                        }
                    }
                }
            }
        }
        private static void Mixed()
        {

            if (SMenu.Item(_MenuItemBase + "Mixed.Boolean.UseQ").GetValue<bool>())
            {
                if (true)//ManaHandler.UseModeQ())
                {
                    var target = TargetSelector.GetTarget(Champion.Q.Range, TargetSelector.DamageType.Physical);
                    var predictionPosition = Champion.Q.GetPrediction(target);
                    var collisionObjects = predictionPosition.CollisionObjects;
                    if (0 >= collisionObjects.Count && predictionPosition.Hitchance >=
                        GetHitChance(
                            SMenu.Item(_MenuItemBase + "Mixed.Boolean.UseQ.Prediction")
                                .GetValue<StringList>()
                                .SelectedIndex))
                    {
                        if (SMenu.Item(_MenuItemBase + "Mixed.Boolean.UseQ.Reset").GetValue<bool>())
                        {
                            if (Player.Spellbook.IsAutoAttacking || Player.LSIsDashing())
                                Champion.Q.Cast(predictionPosition.CastPosition);
                        }

                        else if (!Player.Spellbook.IsAutoAttacking && !Player.LSIsDashing())
                            Champion.Q.Cast(predictionPosition.CastPosition);
                    }
                }

            }
            if (SMenu.Item(_MenuItemBase + "Mixed.Boolean.Rend").GetValue<bool>())
            {
                if (true)//ManaHandler.UseModeE())
                {
                    foreach (var stacks in from target in HeroManager.Enemies
                        where target.IsValid
                        where target.LSIsValidTarget(Champion.E.Range)
                        where !Damage.DamageCalc.CheckNoDamageBuffs(target)
                        select target.GetBuffCount("kalistaexpungemarker")
                        into stacks
                        where stacks >= SMenu.Item(_MenuItemBase + "Mixed.Boolean.Rend.Stacks").GetValue<Slider>().Value
                        select stacks)
                    {
                        if (!Humanizer.Limiter.CheckDelay($"{Humanizer.DelayItemBase}Slider.RendDelay")) return;
                        Humanizer.Limiter.UseTick($"{Humanizer.DelayItemBase}Slider.RendDelay");
                    }
                }
            }

        }
        private static void Combo()
        {
            
            if (SMenu.Item(_MenuItemBase + "Combo.Boolean.UseQ").GetValue<bool>())
            {
                if (true)//ManaHandler.UseModeQ())
                {
                    var target = TargetSelector.GetTarget(Champion.Q.Range, TargetSelector.DamageType.Physical);
                    var predictionPosition = Champion.Q.GetPrediction(target);
                    var collisionObjects = predictionPosition.CollisionObjects;
                    if (0 >= collisionObjects.Count
                        && predictionPosition.Hitchance >=
                        GetHitChance(
                            SMenu.Item(_MenuItemBase + "Combo.Boolean.UseQ.Prediction")
                                .GetValue<StringList>()
                                .SelectedIndex))
                    {
                        if (SMenu.Item(_MenuItemBase + "Combo.Boolean.UseQ.Reset").GetValue<bool>())
                        {
                            if (Player.Spellbook.IsAutoAttacking || Player.LSIsDashing())
                                Champion.Q.Cast(predictionPosition.CastPosition);
                        }

                        else if (!Player.Spellbook.IsAutoAttacking && !Player.LSIsDashing())
                            Champion.Q.Cast(predictionPosition.CastPosition);
                    }
                }
            }

        }
    }
}