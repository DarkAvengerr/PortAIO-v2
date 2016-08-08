using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ProSeries.Utils.Drawings;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ProSeries.Champions
{
    public class Sivir
    {
        internal static Spell QCombo;
        internal static Spell QHarass;
        internal static Spell W;
        internal static Spell E;

        public Sivir()
        {
            QCombo = new Spell(SpellSlot.Q, 1250);
            QCombo.SetSkillshot(0.25f, 90f, 1350f, false, SkillshotType.SkillshotLine);

            QHarass = new Spell(SpellSlot.Q, 1000);
            QHarass.SetSkillshot(0.25f, 90f, 1350f, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);

            // Drawings
            Circles.Add("Q Range", QCombo);

            // Spell usage
            var cMenu = new Menu("Combo", "combo");
            cMenu.AddItem(new MenuItem("combomana", "Minimum mana %")).SetValue(new Slider(5));
            cMenu.AddItem(new MenuItem("usecomboq", "Use Boomerang", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecombow", "Use Ricochet", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecomboe", "Use Spell Shield (Semi)", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecombo", "Combo (active)")).SetValue(new KeyBind(32, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(cMenu);

            var hMenu = new Menu("Harass", "harass");
            hMenu.AddItem(new MenuItem("harassmana", "Minimum mana %")).SetValue(new Slider(55));
            hMenu.AddItem(new MenuItem("useharassq", "Use Boomerang", true).SetValue(true));
            hMenu.AddItem(new MenuItem("useharassw", "Use Ricochet", true).SetValue(true));
            hMenu.AddItem(new MenuItem("useharass", "Harass (active)")).SetValue(new KeyBind(67, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(hMenu);

            var wMenu = new Menu("Farming", "farming");
            wMenu.AddItem(new MenuItem("clearmana", "Minimum mana %")).SetValue(new Slider(35));
            wMenu.AddItem(new MenuItem("useclearw", "Use Ricochet", true).SetValue(true));
            wMenu.AddItem(new MenuItem("useclear", "Wave/Jungle (active)")).SetValue(new KeyBind(86, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(wMenu);

            var mMenu = new Menu("Misc", "misc");
            mMenu.AddItem(new MenuItem("useqimm", "Use Q on Immobile", true)).SetValue(true);
            mMenu.AddItem(new MenuItem("useqdash", "Use Q on Dashing", true)).SetValue(true);
            ProSeries.Config.AddSubMenu(mMenu);

            // Events
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += Orbwalking_OnAfterAttack;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsValid<AIHeroClient>())
            {
                return;
            }

            if (!ProSeries.Config.Item("usecomboe", true).GetValue<bool>())
            {
                return;
            }

            if (!sender.IsEnemy)
            {
                return;
            }

            if (args.Target == null || !args.Target.IsValid || !args.Target.IsMe)
            {
                return;
            }

            if (args.SData.LSIsAutoAttack())
            {
                return;
            }

            //Delay the Cast a bit to make it look more human
            LeagueSharp.Common.Utility.DelayAction.Add(100, () => E.Cast());
        }

        private static void Orbwalking_OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsValid || !unit.IsMe)
            {
                return;
            }

            if (ProSeries.CanCombo())
            {
                if (ProSeries.Config.Item("usecombow", true).GetValue<bool>() &&
                    target.IsValid<AIHeroClient>())
                {
                    W.Cast();
                }
            }

            if (ProSeries.CanHarass())
            {
                if (ProSeries.Config.Item("useharassw", true).GetValue<bool>() &&
                    target.IsValid<AIHeroClient>() && ProSeries.IsWhiteListed((AIHeroClient) target))
                {
                    W.Cast();
                }
            }

            if (ProSeries.CanClear())
            {
                if (ProSeries.Config.Item("useclearw", true).GetValue<bool>() &&
                    target.IsValid<Obj_AI_Minion>())
                {
                    //W.Cast();
                }
            }
        }

        internal static void Game_OnGameUpdate(EventArgs args)
        {
            if (ProSeries.CanCombo())
            {
                var target = TargetSelector.GetTarget(QCombo.Range, TargetSelector.DamageType.Physical);
                if (target.LSIsValidTarget() && ProSeries.Config.Item("usecomboq", true).GetValue<bool>())
                {
                    CastQ(false);
                }
            }

            if (ProSeries.CanHarass())
            {
                var target = TargetSelector.GetTarget(QHarass.Range, TargetSelector.DamageType.Physical);
                if (target.LSIsValidTarget() && ProSeries.IsWhiteListed(target))
                {
                    if (ProSeries.Config.Item("useharassq", true).GetValue<bool>())
                    {
                        CastQ(false);
                    }
                }
            }

            if (QCombo.LSIsReady())
            {
                foreach (var target in ObjectManager.Get<AIHeroClient>().Where(h => h.LSIsValidTarget(QCombo.Range)))
                {
                    if (ProSeries.Config.Item("useqimm", true).GetValue<bool>())
                        QCombo.CastIfHitchanceEquals(target, HitChance.Immobile);

                    if (ProSeries.Config.Item("useqdash", true).GetValue<bool>())
                        QCombo.CastIfHitchanceEquals(target, HitChance.Dashing);
                }
            }
        }

        internal static void CastQ(bool harass)
        {
            var spell = harass ? QHarass : QCombo;

            if (!spell.LSIsReady())
            {
                return;
            }

            var target = TargetSelector.GetTarget(spell.Range, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                spell.Cast(target);
            }
        }
    }
}
