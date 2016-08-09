using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ProSeries.Utils.Drawings;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ProSeries.Champions
{
    public class _Twitch
    {
        internal static Spell W;
        internal static Spell E;

        // Outdated - Twitch Plugin
        public _Twitch()
        {
            W = new Spell(SpellSlot.W, 950);
            W.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 1200);

            // Drawings
            Circles.Add("E Range", E);

            // Spell usage
            var cMenu = new Menu("Combo", "combo");
            cMenu.AddItem(new MenuItem("combomana", "Minimum mana %")).SetValue(new Slider(5));
            cMenu.AddItem(new MenuItem("usecombow", "Use Venom Cask", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecomboe", "Use Contaminate", true).SetValue(true)); 
            cMenu.AddItem(new MenuItem("usecombo", "Combo (active)")).SetValue(new KeyBind(32, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(cMenu);

            var hMenu = new Menu("Harass", "harass");
            hMenu.AddItem(new MenuItem("harassmana", "Minimum mana %")).SetValue(new Slider(55));
            hMenu.AddItem(new MenuItem("useharassw", "Use Venom Cask", true).SetValue(true));
            hMenu.AddItem(new MenuItem("useharasse", "Use Contaminate", true).SetValue(true)); 
            hMenu.AddItem(new MenuItem("useharass", "Harass (active)")).SetValue(new KeyBind(67, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(hMenu);

            var fMenu = new Menu("Farming", "farming");
            fMenu.AddItem(new MenuItem("clearmana", "Minimum mana %")).SetValue(new Slider(35));
            fMenu.AddItem(new MenuItem("usecleare", "Use Contaminate (Smart)", true).SetValue(true));
            fMenu.AddItem(new MenuItem("useclear", "Wave/Jungle (active)")).SetValue(new KeyBind(86, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(fMenu);

            var mMenu = new Menu("Misc", "misc");
            mMenu.AddItem(new MenuItem("usewimm", "Use W on Immobile", true)).SetValue(true);
            mMenu.AddItem(new MenuItem("usewdash", "Use W on Dashing", true)).SetValue(true);
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
            if (ProSeries.Player.GetSpellDamage(targetAsHero, SpellSlot.W) / W.Delay >
                ProSeries.Player.GetAutoAttackDamage(targetAsHero, true) * (1 / ProSeries.Player.AttackDelay))
            {
                W.Cast(targetAsHero);
            }
        }

        internal static void Game_OnUpdate(EventArgs args)
        {
            if (ProSeries.CanCombo())
            {
                var etarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (etarget.IsValidTarget() && E.IsReady())
                {
                    foreach (var buff in etarget.Buffs)
                    {
                        if (buff.Name == "twitchdeadlyvenom" && buff.Count == 6 && 
                            ProSeries.Config.Item("usecomboe", true).GetValue<bool>())
                        {
                            E.Cast();
                        }
                    }
                }

                var wtarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (wtarget.IsValidTarget() && W.IsReady())
                {
                    if (ProSeries.Config.Item("usecombow", true).GetValue<bool>())
                        W.Cast(wtarget);
                }
            }

            if (ProSeries.CanHarass())
            {
                var etarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (etarget.IsValidTarget() && E.IsReady() && ProSeries.IsWhiteListed(etarget))
                {
                    foreach (var buff in etarget.Buffs)
                    {
                        if (buff.Name == "twitchdeadlyvenom" && buff.Count == 3 &&
                            ProSeries.Config.Item("useharasse", true).GetValue<bool>())
                        {
                            E.Cast();
                        }
                    }
                }

                var wtarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (wtarget.IsValidTarget() && W.IsReady() && ProSeries.IsWhiteListed(wtarget))
                {
                    if (ProSeries.Config.Item("useharassw", true).GetValue<bool>())
                        W.Cast(wtarget);
                }
            }

            if (ProSeries.CanClear() && E.IsReady())
            {
                var minionList = MinionManager.GetMinions(E.Range).Where(m => m.HasBuff("twitchdeadlyvenom", true));
                if (minionList.Count(m => m.Health <= ProSeries.Player.GetSpellDamage(m, SpellSlot.E)) >= 3)
                {
                    E.Cast();
                }
            }

            if (W.IsReady() && !ProSeries.Player.HasBuff("twitchhideinshadows", true))
            {
                foreach (var target in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget(W.Range)))
                {
                    if (ProSeries.Config.Item("usewimm", true).GetValue<bool>())
                        W.CastIfHitchanceEquals(target, HitChance.Immobile);

                    if (ProSeries.Config.Item("usewdash", true).GetValue<bool>())
                        W.CastIfHitchanceEquals(target, HitChance.Dashing);
                }
            }
        }
    }
}
