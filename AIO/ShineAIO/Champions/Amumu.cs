using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using ShineCommon;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ShineSharp.Champions
{
    public class Amumu : BaseChamp
    {
        public Amumu()
            : base("Amumu")
        {

        }

        public override void CreateConfigMenu()
        {
            combo = new Menu("Combo", "Combo");
            combo.AddItem(new MenuItem("CUSEQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("CUSEW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("CUSEE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("CQHITCHANCE", "Q Hit Chance").SetValue<StringList>(new StringList(ShineCommon.Utility.HitchanceNameArray, 2)));

            //
            ult = new Menu("R Settings", "rsetting");
            ult.AddItem(new MenuItem("CUSER", "Use R").SetValue(true));
            ult.AddItem(new MenuItem("CUSERHIT", "Use When Enemy Count >=").SetValue<Slider>(new Slider(3, 1, 5)));
            ult.AddItem(new MenuItem("CUSERMETHOD", "R Method").SetValue<StringList>(new StringList(new string[] { "Only If Will Hit >= X Method", "If Will Hit Toggle Selected", "Shine# Smart R" }, 2)));
            ult.AddItem(new MenuItem("DTOGGLER", "Draw Toggle R").SetValue(true));
            //
            combo.AddSubMenu(ult);

            harass = new Menu("Harass", "Harass");
            harass.AddItem(new MenuItem("HUSEQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("HUSEW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("HUSEE", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("HQHITCHANCE", "Q Hit Chance").SetValue<StringList>(new StringList(ShineCommon.Utility.HitchanceNameArray, 2)));
            harass.AddItem(new MenuItem("HMANA", "Min. Mana Percent").SetValue(new Slider(50, 0, 100)));

            laneclear = new Menu("Lane/Jungle Clear", "LaneClear");
            laneclear.AddItem(new MenuItem("LUSEW", "Use W").SetValue(true));
            laneclear.AddItem(new MenuItem("LUSEE", "Use E").SetValue(true));
            laneclear.AddItem(new MenuItem("LMINW", "Min. Minions To W In Range").SetValue(new Slider(3, 1, 12)));
            laneclear.AddItem(new MenuItem("LMANA", "Min. Mana Percent").SetValue(new Slider(50, 0, 100)));

            misc = new Menu("Misc", "Misc");
            misc.AddItem(new MenuItem("MAUTOQIMMO", "Auto Q Immobile Target").SetValue(false));
            misc.AddItem(new MenuItem("MANTIGAPR", "Anti Gap Closer With R").SetValue(false));

            Config.AddSubMenu(combo);
            Config.AddSubMenu(harass);
            Config.AddSubMenu(laneclear);
            Config.AddSubMenu(misc);
            Config.AddToMainMenu();

            BeforeOrbWalking += BeforeOrbWalk;
            BeforeDrawing += BeforeDraw;

            OrbwalkingFunctions[(int)Orbwalking.OrbwalkingMode.Combo] += Combo;
            OrbwalkingFunctions[(int)Orbwalking.OrbwalkingMode.Mixed] += Harass;
            OrbwalkingFunctions[(int)Orbwalking.OrbwalkingMode.LaneClear] += LaneClear;

        }

        public override void SetSpells()
        {
            Spells[Q] = new Spell(SpellSlot.Q, 1050f);
            Spells[Q].SetSkillshot(0.5f, 90f, 2000f, true, SkillshotType.SkillshotLine);

            Spells[W] = new Spell(SpellSlot.W, 300f);

            Spells[E] = new Spell(SpellSlot.E, 325f);

            Spells[R] = new Spell(SpellSlot.R, 540f);
        }

        public void BeforeOrbWalk()
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                return;

            if (!Spells[W].IsActive() || !Spells[W].IsReady())
                return;

            if (HeroManager.Enemies.Count(p => p.Distance(ObjectManager.Player.ServerPosition) <= Spells[W].Range + 50) == 0 && MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells[W].Range + 50, MinionTypes.All, MinionTeam.NotAlly).Count == 0)
                Spells[W].Cast();
            
        }

        public void Combo()
        {
            if (Spells[Q].IsReady() && Config.Item("CUSEQ").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, TargetSelector.DamageType.Magical);
                if (t != null)
                    CastSkillshot(t, Spells[Q], ShineCommon.Utility.HitchanceArray[combo.Item("CQHITCHANCE").GetValue<StringList>().SelectedIndex]);
            }
            
            if (Spells[W].IsReady() && !Spells[W].IsActive() && combo.Item("CUSEW").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(Spells[W].Range, TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[W].Cast();
            }

            if (Spells[E].IsReady() && combo.Item("CUSEE").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(Spells[E].Range, TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[E].Cast();
            }

            if (Spells[R].IsReady() && ult.Item("CUSER").GetValue<bool>())
            {
                //new string[] { "Only If Will Hit >= X Method", "If Will Hit Toggle Selected", "Shine# Smart R" }

                switch (ult.Item("CUSERMETHOD").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                    {
                        if (ObjectManager.Player.CountEnemiesInRange(Spells[R].Range) >= ult.Item("CUSERHIT").GetValue<Slider>().Value)
                            Spells[R].Cast();
                    }
                    break;

                    case 1:
                    {
                        if (TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget.ServerPosition.Distance(ObjectManager.Player.ServerPosition) <= Spells[R].Range)
                            Spells[R].Cast();
                    }
                    break;

                    case 2:
                    {
                        //high = 3, medium = 2, low = 1
                        int prio_sum = 0;
                        var enemies = HeroManager.Enemies.Where(p => p.ServerPosition.Distance(ObjectManager.Player.ServerPosition) <= Spells[R].Range);
                        foreach (var enemy in enemies)
                            prio_sum += ShineCommon.Utility.GetPriority(enemy.ChampionName);

                        if (prio_sum >= 6) //at least 2 important target or 3 medium target (if not both 2 important it will cast whenever at least for 3 champ)
                            Spells[R].Cast();
                    }
                    break;
                }
            }
        }

        public void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Config.Item("HMANA").GetValue<Slider>().Value)
                return;

            if (Spells[Q].IsReady() && Config.Item("HUSEQ").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(Spells[Q].Range, TargetSelector.DamageType.Magical);
                if (t != null)
                    CastSkillshot(t, Spells[Q], ShineCommon.Utility.HitchanceArray[Config.Item("HQHITCHANCE").GetValue<StringList>().SelectedIndex]);
            }

            if (Spells[W].IsReady() && !Spells[W].IsActive() && Config.Item("HUSEW").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(Spells[W].Range, TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[W].Cast();
            }

            if (Spells[E].IsReady() && Config.Item("HUSEE").GetValue<bool>())
            {
                var t = TargetSelector.GetTarget(Spells[E].Range, TargetSelector.DamageType.Magical);
                if (t != null)
                    Spells[E].Cast();
            }
        }

        public void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < Config.Item("LMANA").GetValue<Slider>().Value)
                return;

            if (Spells[W].IsReady() && !Spells[W].IsActive() && laneclear.Item("LUSEW").GetValue<bool>())
            {
                if (MinionManager.GetMinions(Spells[W].Range + 50, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.None).Count >= laneclear.Item("LMINW").GetValue<Slider>().Value)
                    Spells[W].Cast();
                else if (MinionManager.GetMinions(Spells[W].Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.None).Count > 0) //jungle clear
                    Spells[W].Cast();
            }

            if (Spells[E].IsReady() && laneclear.Item("LUSEE").GetValue<bool>())
                if (MinionManager.GetMinions(Spells[E].Range, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.None).Count > 0)
                    Spells[E].Cast();
            
        }

        public void BeforeDraw()
        {
            if (Config.Item("CUSER").GetValue<bool>() && Config.Item("CUSERMETHOD").GetValue<StringList>().SelectedIndex == 1 && ult.Item("DTOGGLER").GetValue<bool>() && TargetSelector.SelectedTarget != null)
            {
                Text.DrawText(null, "Toggle R Target",
                    (int)(TargetSelector.SelectedTarget.HPBarPosition.X + TargetSelector.SelectedTarget.BoundingRadius / 2 - 10),
                    (int)(TargetSelector.SelectedTarget.HPBarPosition.Y - 20), SharpDX.Color.Yellow);
            }
        }

        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Spells[R].IsReady() && gapcloser.End.Distance(ObjectManager.Player.ServerPosition) <= 300 && misc.Item("MANTIGAPR").GetValue<bool>())
                Spells[R].Cast();
        }

        public override void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender.IsEnemy && sender.IsChampion() && ShineCommon.Utility.IsImmobileTarget(sender as AIHeroClient) && Spells[Q].IsInRange(sender) && misc.Item("MAUTOQIMMO").GetValue<bool>())
                if (Spells[Q].IsReady())
                    Spells[Q].Cast(sender.ServerPosition);
        }
    }
}
