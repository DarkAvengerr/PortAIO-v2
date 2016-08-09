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
    public class Ashe
    {
        internal static Spell Q;
        internal static Spell W;
        internal static Spell R;

        public Ashe()
        {
            // Load spells
            Q = new Spell(SpellSlot.Q);

            W = new Spell(SpellSlot.W, 1200);
            W.SetSkillshot(0.25f, (float)(4.62f * Math.PI / 180), 1500f, true, SkillshotType.SkillshotCone);

            R = new Spell(SpellSlot.R, 3000);
            R.SetSkillshot(0.25f, 130f, 1600f, false, SkillshotType.SkillshotLine);

            // Drawings
            Circles.Add("W Range", W);

            // Spell usage
            var cMenu = new Menu("Combo", "combo");
            cMenu.AddItem(new MenuItem("combomana", "Minimum mana %")).SetValue(new Slider(5));
            cMenu.AddItem(new MenuItem("usecomboq", "Use Ranger's Focus", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecombow", "Use Volley", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecombor", "Use Crystal Arrow", true).SetValue(true)); 
            cMenu.AddItem(new MenuItem("usecombo", "Combo (active)")).SetValue(new KeyBind(32, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(cMenu);

            var hMenu = new Menu("Harass", "harass");
            hMenu.AddItem(new MenuItem("harassmana", "Minimum mana %")).SetValue(new Slider(55));
            hMenu.AddItem(new MenuItem("useharassq", "Use Ranger's Focus", true).SetValue(true));
            hMenu.AddItem(new MenuItem("useharassw", "Use Volley", true).SetValue(true));
            hMenu.AddItem(new MenuItem("useharass", "Harass (active)")).SetValue(new KeyBind(67, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(hMenu);

            var fMenu = new Menu("Farming", "farming");
            fMenu.AddItem(new MenuItem("clearmana", "Minimum mana %")).SetValue(new Slider(75));
            fMenu.AddItem(new MenuItem("useclear", "Wave/Jungle (active)")).SetValue(new KeyBind(86, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(fMenu);

            var mMenu = new Menu("Misc", "misc");
            mMenu.AddItem(new MenuItem("maxrdist", "Max R distance", true)).SetValue(new Slider(1500, 0, 3000));
            mMenu.AddItem(new MenuItem("usewimm", "Use W on Immobile", true)).SetValue(true);
            mMenu.AddItem(new MenuItem("usewdash", "Use W on Dashing", true)).SetValue(true);
            mMenu.AddItem(new MenuItem("usesemir", "Use Crystal Arrow", true)).SetValue(new KeyBind('T', KeyBindType.Press));
            ProSeries.Config.AddSubMenu(mMenu);

            // Events
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !args.SData.IsAutoAttack())
            {
                return;
            }

            if (!args.Target.IsValid<AIHeroClient>())
            {
                return;
            }

            if (ProSeries.CanHarass() && Q.IsReady())
            {
                var qtarget = ObjectManager.Get<AIHeroClient>().First(x => x.NetworkId == args.Target.NetworkId);
                if (qtarget.IsValidTarget() && ProSeries.IsWhiteListed(qtarget))
                {
                    if (ProSeries.Config.Item("useharassq", true).GetValue<bool>())
                    {
                        foreach (var buff in ProSeries.Player.Buffs)
                        {
                            if (buff.Name == "AsheQ" && buff.Count >= 3)
                                Q.Cast();
                        }
                    }
                }
            }

            if (ProSeries.CanCombo() && Q.IsReady())
            {
                var qtarget = ObjectManager.Get<AIHeroClient>().First(x => x.NetworkId == args.Target.NetworkId);
                if (qtarget.IsValidTarget() && ProSeries.Player.HasBuff("asheqcastready", true))
                {
                    if (ProSeries.Config.Item("usecomboq", true).GetValue<bool>())
                        Q.Cast();
                }
            }
        }


        internal static void Game_OnUpdate(EventArgs args)
        {
            if (ProSeries.CanCombo())
            {        
                var wtarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (wtarget.IsValidTarget() && W.IsReady())
                {
                    if (ProSeries.Config.Item("usecombow", true).GetValue<bool>())
                        W.Cast(wtarget);
                }
            }

            if (ProSeries.CanHarass())
            {
                var wtarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                if (wtarget.IsValidTarget() && W.IsReady() && ProSeries.IsWhiteListed(wtarget))
                {
                    if (ProSeries.Config.Item("useharassw", true).GetValue<bool>())
                        W.Cast(wtarget);
                }
            }

            if (W.IsReady())
            {
                foreach (var target in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget(W.Range)))
                {
                    if (ProSeries.Config.Item("usewimm", true).GetValue<bool>())
                        W.CastIfHitchanceEquals(target, HitChance.Immobile);

                    if (ProSeries.Config.Item("usewdash", true).GetValue<bool>())
                        W.CastIfHitchanceEquals(target, HitChance.Dashing);
                }
            }

            if (ProSeries.CanCombo() || ProSeries.Config.Item("usesemir", true).GetValue<KeyBind>().Active)
            {
                var maxDistance = ProSeries.Config.Item("maxrdist", true).GetValue<Slider>().Value;
                foreach (var target in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget(maxDistance)))
                {
                    var aaDamage = Orbwalking.InAutoAttackRange(target)
                        ? ProSeries.Player.GetAutoAttackDamage(target, true)
                        : 0;

                    if (!target.IsZombie && R.IsReady() &&
                       (ProSeries.Player.GetSpellDamage(target, SpellSlot.R) >= target.Health - aaDamage ||
                        ProSeries.Config.Item("usesemir", true).GetValue<KeyBind>().Active))
                    {
                        var units = new List<Obj_AI_Base>();
                        if (ProSeries.CountInPath(ProSeries.Player.ServerPosition, 
                            target.ServerPosition, R.Width, maxDistance, out units) <= 1)
                            R.Cast(target);
                    }
                }
            }
        }
    }
}
