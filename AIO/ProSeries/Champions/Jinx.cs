using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ProSeries.Utils.Drawings;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ProSeries.Champions
{
    public class Jinx
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell E;
        internal static Spell R;

        internal static float RocketRange;

        public Jinx()
        {
            Q = new Spell(SpellSlot.Q, 600);

            W = new Spell(SpellSlot.W, 1500f);
            W.SetSkillshot(0.6f, 60f, 3300f, true, SkillshotType.SkillshotLine);

            E = new Spell(SpellSlot.E, 900f);
            E.SetSkillshot(1.0f, 1f, 1750f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 2000);
            R.SetSkillshot(0.6f, 140f, 1700f, false, SkillshotType.SkillshotLine);

            //Drawings
            Circles.Add("W Range", W);

            //Spell usage.
            var cMenu = new Menu("Combo", "combo");
            cMenu.AddItem(new MenuItem("combomana", "Minimum mana %")).SetValue(new Slider(5));
            cMenu.AddItem(new MenuItem("usecomboq", "Use Switcheroo", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecombow", "Use Zap", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecomboe", "Use Flame Chompers", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecombor", "Use Mega Death Rocket", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecombo", "Combo (active)")).SetValue(new KeyBind(32, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(cMenu);

            var hMenu = new Menu("Harass", "harass");
            hMenu.AddItem(new MenuItem("harassmana", "Minimum mana %")).SetValue(new Slider(55));
            hMenu.AddItem(new MenuItem("useharassq", "Use Switcheroo", true).SetValue(true));
            hMenu.AddItem(new MenuItem("useharassw", "Use Zap", true).SetValue(true));
            hMenu.AddItem(new MenuItem("useharass", "Harass (active)")).SetValue(new KeyBind(67, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(hMenu);

            var wMenu = new Menu("Farming", "farming");
            wMenu.AddItem(new MenuItem("clearmana", "Minimum mana %")).SetValue(new Slider(75));
            wMenu.AddItem(new MenuItem("useclearq", "Use Switcheroo", true).SetValue(true));
            wMenu.AddItem(new MenuItem("clearqmin", "Minimum minion count", true)).SetValue(new Slider(3, 2, 6));
            wMenu.AddItem(new MenuItem("useclear", "Wave/Jungle (active)")).SetValue(new KeyBind(86, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(wMenu);

            var mMenu = new Menu("Misc", "misc");
            mMenu.AddItem(new MenuItem("maxrdist", "Max R distance", true)).SetValue(new Slider(1500, 0, 3000));
            mMenu.AddItem(new MenuItem("useeimm", "Use E on Immobile", true)).SetValue(true);
            mMenu.AddItem(new MenuItem("useedash", "Use E on Dashing", true)).SetValue(true);
            ProSeries.Config.AddSubMenu(mMenu);

            //Events
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += OrbwalkingOnAfterAttack;
        }

        private static void OrbwalkingOnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsValid || !unit.IsMe)
            {
                return;
            }

            if (!target.IsValid<AIHeroClient>())
            {
                return;
            }

            var targetAsHero = target as AIHeroClient;

            // was using W too close, will work with rockets after attack though
            if (targetAsHero.LSDistance(ProSeries.Player.ServerPosition) > 525)
            {
                if (ProSeries.Player.LSGetSpellDamage(targetAsHero, SpellSlot.W) / W.Delay >
                    ProSeries.Player.LSGetAutoAttackDamage(targetAsHero, true) * (1 / ProSeries.Player.AttackDelay))
                {
                    W.Cast(targetAsHero);
                }
            }
        }

        private static Obj_AI_Base GetCenterMinion()
        {
            var minions = MinionManager.GetMinions(525 + RocketRange);
            var centerlocation =
                MinionManager.GetBestCircularFarmLocation(minions.Select(x => x.Position.LSTo2D()).ToList(), 250,
                    525 + RocketRange);
;
            return centerlocation.MinionsHit >= ProSeries.Config.Item("clearqmin", true).GetValue<Slider>().Value
                ? MinionManager.GetMinions(1000).OrderBy(x => x.LSDistance(centerlocation.Position)).FirstOrDefault()
                : null;
        }

        private static float GetRDamage(AIHeroClient target)
        {
            if (target == null)
            {
                return 0f;
            }

            var units = new List<Obj_AI_Base>();
            var maxdist = ProSeries.Player.LSDistance(target.ServerPosition) > 750;
            var maxrdist = ProSeries.Config.Item("maxrdist", true).GetValue<Slider>().Value;

            // impact physical damage
            var idmg = R.LSIsReady() &&
                       ProSeries.CountInPath(ProSeries.Player.ServerPosition, target.ServerPosition, R.Width + 50,
                           (maxrdist * 2), out units) <= 1
                ? (maxdist ? R.GetDamage(target, 1) : R.GetDamage(target, 0))
                : 0;

            // explosion damage
            var edmg = R.LSIsReady() &&
                       ProSeries.CountInPath(ProSeries.Player.ServerPosition, target.ServerPosition, R.Width + 50,
                           (maxrdist * 2), out units) > 1 &&
                            target.LSDistance(units.OrderBy(x => x.LSDistance(ProSeries.Player.ServerPosition))
                                .First(t => t.NetworkId != target.NetworkId).ServerPosition) <= R.Width + 100 // explosion radius? :^)
                ? (maxdist
                    ? (float) // maximum explosion dmage
                        (ProSeries.Player.CalcDamage(target, Damage.DamageType.Physical,
                            new double[] {160, 224, 288}[R.Level - 1] +
                            new double[] {20, 24, 28}[R.Level - 1] / 100 * (target.MaxHealth - target.Health) +
                            0.8 * ProSeries.Player.FlatPhysicalDamageMod))
                    : (float) // minimum explosion damage
                        (ProSeries.Player.CalcDamage(target, Damage.DamageType.Physical,
                            new double[] {20, 28, 36}[R.Level - 1] +
                            new double[] {20, 24, 28}[R.Level - 1] / 100 * (target.MaxHealth - target.Health) +
                            0.08 * ProSeries.Player.FlatPhysicalDamageMod)))
                : 0;

            return idmg + edmg;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            RocketRange = new[] { 75, 75, 100, 125, 150, 175 }[Q.Level];
            var minigunOut = ProSeries.Player.GetSpell(SpellSlot.Q).ToggleState == 1;

            if (ProSeries.CanCombo())
            {
                if (!minigunOut && ProSeries.Config.Item("usecomboq", true).GetValue<bool>())
                {
                    if (ProSeries.Player.ManaPercent < 35 &&
                        HeroManager.Enemies.Any(
                            i => i.LSIsValidTarget(590 + RocketRange + 10) &&
                                 ProSeries.Player.LSGetAutoAttackDamage(i, true) * 3 < i.Health))
                    {
                        Q.Cast();
                    }
                }

                var qtarget = TargetSelector.GetTarget(525 + RocketRange + 250, TargetSelector.DamageType.Physical);          
                if (qtarget.LSIsValidTarget() && Q.LSIsReady())
                {
                    if (ProSeries.Config.Item("usecomboq", true).GetValue<bool>())
                    {
                        if (minigunOut && (ProSeries.Player.ManaPercent > 35 ||
                            ProSeries.Player.LSGetAutoAttackDamage(qtarget, true) * 3 > qtarget.Health) &&
                            qtarget.LSDistance(ProSeries.Player.ServerPosition) > 590)
                            Q.Cast();

                        if (!minigunOut && ProSeries.Player.ManaPercent < 35)
                            Q.Cast();

                        if (!minigunOut && qtarget.LSDistance(ProSeries.Player.ServerPosition) <= 590)
                            Q.Cast();      
                    }
                }

                var wtarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (wtarget.LSIsValidTarget() && W.LSIsReady())
                {
                    if (ProSeries.Config.Item("usecombow", true).GetValue<bool>())
                    {
                        if (wtarget.LSDistance(ProSeries.Player.ServerPosition) > 525)
                        {
                            if (ProSeries.Player.LSGetAutoAttackDamage(wtarget, true) * 2 < wtarget.Health)
                                W.CastIfHitchanceEquals(wtarget, HitChance.High);
                        }
                    }
                }

                if (ProSeries.Config.Item("usecomboe", true).GetValue<bool>() && E.LSIsReady())
                {
                    foreach (var target in ObjectManager.Get<AIHeroClient>().Where(h => h.LSIsValidTarget(400) && h.IsMelee()))
                    {
                        E.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                    }
                }
            }

            if (ProSeries.CanHarass())
            {
                var qtarget = TargetSelector.GetTarget(525 + RocketRange + 250, TargetSelector.DamageType.Physical);
                if (!qtarget.LSIsValidTarget() && !minigunOut)
                {
                    if (ProSeries.Config.Item("useharassq", true).GetValue<bool>())
                        Q.Cast();
                }

                if (qtarget.LSIsValidTarget() && Q.LSIsReady() && ProSeries.IsWhiteListed(qtarget))
                {
                    if (ProSeries.Config.Item("useharassq", true).GetValue<bool>())
                    {
                        if (!minigunOut && ProSeries.Player.ManaPercent < 20)
                            Q.Cast();   

                        if (minigunOut && ProSeries.Player.ManaPercent > 20)
                            if (qtarget.LSDistance(ProSeries.Player.ServerPosition) > 590)
                                Q.Cast();

                        if (!minigunOut && qtarget.LSDistance(ProSeries.Player.ServerPosition) <= 590) 
                            Q.Cast();
                    }
                }

                var wtarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (wtarget.LSIsValidTarget() && W.LSIsReady() && ProSeries.IsWhiteListed(wtarget))
                {
                    if (ProSeries.Config.Item("useharassw", true).GetValue<bool>())
                        W.CastIfHitchanceEquals(wtarget, HitChance.High);
                }
            }

            if (ProSeries.CanClear() && Q.LSIsReady())
            {
                if (ProSeries.Config.Item("useclearq", true).GetValue<bool>())
                {
                    if (GetCenterMinion().LSIsValidTarget())
                    {
                        if (minigunOut)
                            Q.Cast();

                        if (!minigunOut)
                            ProSeries.Orbwalker.ForceTarget(GetCenterMinion());
                    }

                    else
                    {
                        if (!minigunOut)
                            Q.Cast();
                    }
                }
            }

            if (ProSeries.Player.GetSpell(SpellSlot.Q).ToggleState == 2 && Q.LSIsReady())
            {
                if (ProSeries.Config.Item("useclearq", true).GetValue<bool>())
                {
                    if (ProSeries.Config.Item("useclear").GetValue<KeyBind>().Active)
                    {
                        if (ProSeries.Player.ManaPercent <=
                            ProSeries.Config.Item("clearmana").GetValue<Slider>().Value)
                        {
                            Q.Cast();
                        }
                    }
                }
            }

            if (E.LSIsReady())
            {
                foreach (var target in ObjectManager.Get<AIHeroClient>().Where(h => h.LSIsValidTarget(E.Range)))
                {
                    if (ProSeries.Config.Item("useeimm", true).GetValue<bool>())
                        E.CastIfHitchanceEquals(target, HitChance.Immobile);

                    if (ProSeries.Config.Item("useedash", true).GetValue<bool>())
                        E.CastIfHitchanceEquals(target, HitChance.Dashing);
                }
            }

            if (ProSeries.Config.Item("usecombor", true).GetValue<bool>())
            {
                var maxDistance = ProSeries.Config.Item("maxrdist", true).GetValue<Slider>().Value;

                foreach (var target in ObjectManager.Get<AIHeroClient>().Where(h => h.LSIsValidTarget(maxDistance)))
                {
                    var canr = !TargetSelector.IsInvulnerable(target, TargetSelector.DamageType.Physical);

                    var aaDamage = Orbwalking.InAutoAttackRange(target)
                        ? ProSeries.Player.LSGetAutoAttackDamage(target, true)
                        : 0;

                    if (target.Health - aaDamage <= GetRDamage(target) && canr)
                    {
                        R.Cast(target);
                    }
                }
            }
        }
    }
}