using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ProSeries.Utils.Drawings;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ProSeries.Champions
{
    public class Corki
    {
        internal static Spell Q;
        internal static Spell E;
        internal static Spell R;

        public Corki()
        {
            // Spell usage
            Q = new Spell(SpellSlot.Q, 825f);
            Q.SetSkillshot(0.35f, 250f, 1500f, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 600f);
            E.SetSkillshot(0f, (float)(45 * Math.PI / 180), 1500, false, SkillshotType.SkillshotCone);

            R = new Spell(SpellSlot.R, 1500f);
            R.SetSkillshot(0.2f, 40f, 2000f, true, SkillshotType.SkillshotLine);

            // Drawings
            Circles.Add("Q Range", Q);

            // Menu
            var cMenu = new Menu("Combo", "combo");
            cMenu.AddItem(new MenuItem("combomana", "Minimum mana %")).SetValue(new Slider(5));
            cMenu.AddItem(new MenuItem("usecomboq", "Use Phosphorus Bomb", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecomboe", "Use Gatling Gun", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecombor", "Use Missile Barrage", true).SetValue(true));
            cMenu.AddItem(new MenuItem("usecombo", "Combo (active)")).SetValue(new KeyBind(32, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(cMenu);

            var hMenu = new Menu("Harass", "harass");
            hMenu.AddItem(new MenuItem("harassmana", "Minimum mana %")).SetValue(new Slider(55));
            hMenu.AddItem(new MenuItem("useharassq", "Use Phosphorus Bomb", true).SetValue(true));
            hMenu.AddItem(new MenuItem("useharasse", "Use Gatling Gun", true).SetValue(false));
            hMenu.AddItem(new MenuItem("useharassr", "Use Missile Barrage", true).SetValue(true));
            hMenu.AddItem(new MenuItem("useharass", "Harass (active)")).SetValue(new KeyBind(67, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(hMenu);

            var fMenu = new Menu("Farming", "farming");
            fMenu.AddItem(new MenuItem("clearmana", "Minimum mana %")).SetValue(new Slider(35));
            fMenu.AddItem(new MenuItem("useclearq", "Use Phosphorus Bomb", true).SetValue(false));
            fMenu.AddItem(new MenuItem("usecleare", "Use Gatling Gun", true).SetValue(false));
            fMenu.AddItem(new MenuItem("useclear", "Wave/Jungle (active)")).SetValue(new KeyBind(86, KeyBindType.Press));
            ProSeries.Config.AddSubMenu(fMenu);

            Game.OnUpdate += Game_OnUpdate;
        }

        internal static void Game_OnUpdate(EventArgs args)
        {
            R.Range = ProSeries.Player.Buffs.Any(
                h => h.Name.ToLower().Contains("corkimissilebarragecounterbig")) ? 1500f : 1300f;

            if (ProSeries.CanCombo())
            {
                var qtarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (qtarget.LSIsValidTarget() && Q.LSIsReady())
                {
                    if (ProSeries.Config.Item("usecomboq", true).GetValue<bool>())
                        Q.CastIfHitchanceEquals(qtarget, HitChance.High);
                }

                var etarget = HeroManager.Enemies.FirstOrDefault(h => h.LSIsValidTarget(E.Range));
                if (etarget.LSIsValidTarget() && E.LSIsReady())
                {
                    if (ProSeries.Config.Item("usecomboe", true).GetValue<bool>() &&
                        E.Cast(etarget) == Spell.CastStates.SuccessfullyCasted)
                    {
                        return;
                    }
                }

                var rtarget = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                if (rtarget.LSIsValidTarget() && R.LSIsReady())
                {
                    if (ProSeries.Config.Item("usecombor", true).GetValue<bool>())
                        R.CastIfHitchanceEquals(rtarget, HitChance.High);
                }
            }

            if (ProSeries.CanHarass())
            {
                var qtarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (qtarget.LSIsValidTarget() && Q.LSIsReady() && ProSeries.IsWhiteListed(qtarget))
                {
                    if (ProSeries.Config.Item("useharassq", true).GetValue<bool>())
                        Q.CastIfHitchanceEquals(qtarget, HitChance.High);
                }

                var etarget = HeroManager.Enemies.FirstOrDefault(h => h.LSIsValidTarget(E.Range));
                if (etarget.LSIsValidTarget() && E.LSIsReady() && ProSeries.IsWhiteListed(etarget))
                {
                    if (ProSeries.Config.Item("useharasse", true).GetValue<bool>())
                        if (E.Cast(etarget) == Spell.CastStates.SuccessfullyCasted)
                            return;
                }

                var rtarget = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                if (rtarget.LSIsValidTarget() && R.LSIsReady() && ProSeries.IsWhiteListed(rtarget))
                {
                    if (ProSeries.Config.Item("useharassr", true).GetValue<bool>() && R.Instance.Ammo > 3)
                        R.CastIfHitchanceEquals(rtarget, HitChance.High);
                }
            }

            if (ProSeries.CanClear())
            {
                foreach (var neutral in ProSeries.JungleMobsInRange(650))
                {
                    if (ProSeries.Config.Item("useclearq", true).GetValue<bool>() && Q.LSIsReady())
                        Q.Cast(neutral);
                    if (ProSeries.Config.Item("usecleare", true).GetValue<bool>() && E.LSIsReady())
                        if (E.Cast(neutral) == Spell.CastStates.SuccessfullyCasted)
                            return;
                }

                if (E.LSIsReady() && ProSeries.Config.Item("usecleare", true).GetValue<bool>())
                {
                    if (ObjectManager.Get<Obj_AI_Minion>().Count(h => h.LSIsValidTarget(E.Range) && !h.Name.Contains("Ward")) >= 3)
                        if (E.Cast(ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(h => h.LSIsValidTarget(E.Range))) ==
                            Spell.CastStates.SuccessfullyCasted)
                            return;
                }

                if (Q.LSIsReady() && ProSeries.Config.Item("useclearq", true).GetValue<bool>())
                {
                    var farmLocation = Q.GetCircularFarmLocation(MinionManager.GetMinions(Q.Range));
                    if (farmLocation.MinionsHit >= 3)
                        Q.Cast(farmLocation.Position);
                }
            }
        }
    }
}
