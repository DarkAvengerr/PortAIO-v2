using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ProSeries.Utils.Drawings;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ProSeries.Champions
{
    public class Ezreal
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell R;

        public Ezreal()
        {
            // Load spells
            Q = new Spell(SpellSlot.Q, 1190);
            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 800);
            W.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);

            R = new Spell(SpellSlot.R, 2500);
            R.SetSkillshot(1f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            // Drawings
            Circles.Add("Q Range", Q);

            // Spell usage
            var cMenu = new Menu("Combo", "combo");
            cMenu.AddItem(new MenuItem("combomana", "Minimum mana %")).SetValue(new Slider(5));
            cMenu.AddItem(new MenuItem("usecomboq", "Use Mystic Shot", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecombow", "Use Essence Flux", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecombor", "Use Trueshot Barrage", true).SetValue(true));;
            cMenu.AddItem(new MenuItem("usecombo", "Combo (active)")).SetValue(new KeyBind(32, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(cMenu);

            var hMenu = new Menu("Harass", "harass");
            hMenu.AddItem(new MenuItem("harassmana", "Minimum mana %")).SetValue(new Slider(55));
            hMenu.AddItem(new MenuItem("useharassq", "Use Mystic Shot", true).SetValue(true));
            hMenu.AddItem(new MenuItem("useharassw", "Use Essence Flux", true).SetValue(true));
            hMenu.AddItem(new MenuItem("useharass", "Harass (active)")).SetValue(new KeyBind(67, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(hMenu);

            var fMenu = new Menu("Farming", "farming");
            fMenu.AddItem(new MenuItem("clearmana", "Minimum mana %")).SetValue(new Slider(35));
            fMenu.AddItem(new MenuItem("useclearq", "Use Mystic Shot", true).SetValue(true));
            fMenu.AddItem(new MenuItem("useclear", "Wave/Jungle (active)")).SetValue(new KeyBind(86, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(fMenu);

            var mMenu = new Menu("Misc", "misc");
            mMenu.AddItem(new MenuItem("maxrdist", "Max R distance", true)).SetValue(new Slider(1500, 0, 3000));
            mMenu.AddItem(new MenuItem("useqimm", "Use Q on Immobile", true)).SetValue(true);
            mMenu.AddItem(new MenuItem("useqdash", "Use Q on Dashing", true)).SetValue(true);
            ProSeries.Config.AddSubMenu(mMenu);

            // Events
            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        }

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsValid || !unit.IsMe)
            {
                return;
            }

            if (!target.IsValid<AIHeroClient>())
            {
                return;
            }

            var targetAsHero = (AIHeroClient) target;
            if (ProSeries.Player.LSGetSpellDamage(targetAsHero, SpellSlot.Q)/Q.Delay >
                ProSeries.Player.LSGetAutoAttackDamage(targetAsHero, true)*(1/ProSeries.Player.AttackDelay))
            {
                Q.Cast(targetAsHero);
            }
        }

        internal static void Game_OnUpdate(EventArgs args)
        {
            if (ProSeries.CanCombo())
            {
                var qtarget = TargetSelector.GetTargetNoCollision(Q);
                if (qtarget.LSIsValidTarget() && Q.LSIsReady())
                {
                    if (ProSeries.Config.Item("usecomboq", true).GetValue<bool>())
                        Q.Cast(qtarget);
                }

                var wtarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (wtarget.LSIsValidTarget() && W.LSIsReady())
                {
                    if (ProSeries.Config.Item("usecombow", true).GetValue<bool>())
                        W.Cast(wtarget);
                }
            }

            if (ProSeries.CanHarass())
            {
                var qtarget = TargetSelector.GetTargetNoCollision(Q);
                if (qtarget.LSIsValidTarget() && Q.LSIsReady() && ProSeries.IsWhiteListed(qtarget))
                {
                    if (ProSeries.Config.Item("usecomboq", true).GetValue<bool>())
                        Q.Cast(qtarget);
                }

                var wtarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (wtarget.LSIsValidTarget() && W.LSIsReady() && ProSeries.IsWhiteListed(wtarget))
                {
                    if (ProSeries.Config.Item("useharassw", true).GetValue<bool>())
                        W.Cast(wtarget);
                }
            }

            if (Q.LSIsReady())
            {
                foreach (var target in ObjectManager.Get<AIHeroClient>().Where(h => h.LSIsValidTarget(Q.Range)))
                {
                    if (ProSeries.Config.Item("useqimm", true).GetValue<bool>())
                        Q.CastIfHitchanceEquals(target, HitChance.Immobile);

                    if (ProSeries.Config.Item("useqdash", true).GetValue<bool>())
                        Q.CastIfHitchanceEquals(target, HitChance.Dashing);
                }
            }

            if (ProSeries.CanClear())
            {
                foreach (var neutral in ProSeries.JungleMobsInRange(650))
                {
                    if (ProSeries.Config.Item("useclearq", true).GetValue<bool>())
                        Q.Cast(neutral);
                }

                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(m => m.LSIsValidTarget(Q.Range)))
                {
                    if (ProSeries.Player.LSGetSpellDamage(minion, Q.Slot) >= minion.Health &&
                        ProSeries.Config.Item("useclearq", true).GetValue<bool>())
                    {
                        if (!minion.Name.Contains("Ward"))
                            Q.Cast(minion);
                    }
                }
            }

            if (ProSeries.Config.Item("usecombor", true).GetValue<bool>())
            {
                var maxDistance = ProSeries.Config.Item("maxrdist", true).GetValue<Slider>().Value;
                foreach (var target in ObjectManager.Get<AIHeroClient>().Where(h => h.LSIsValidTarget(maxDistance)))
                {
                    var aaDamage = Orbwalking.InAutoAttackRange(target)
                        ? ProSeries.Player.LSGetAutoAttackDamage(target, true)
                        : 0;

                    if (!target.IsZombie &&
                         target.Health - aaDamage <= ProSeries.Player.LSGetSpellDamage(target, SpellSlot.R))
                    {
                        R.Cast(target);
                    }
                }
            }
        }
    }
}
