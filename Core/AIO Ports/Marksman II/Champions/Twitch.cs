#region

using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Marksman.Utils;
using ItemData = LeagueSharp.Common.Data.ItemData;
using Orbwalking = LeagueSharp.Common.Orbwalking;

#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Marksman.Champions
{
    internal class Twitch : Champion
    {
        public static Spell W, E, R;

        public Twitch()
        {
            W = new Spell(SpellSlot.W, 950);
            W.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 1200);
            R = new Spell(SpellSlot.R, 1000);

             //LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
             //LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = true;
            Utils.Utils.PrintMessage("Twitch");
        }

        public static AIHeroClient GetEMarkedEnemy
            =>
                ObjectManager.Get<AIHeroClient>()
                    .Where(
                        enemy =>
                            !enemy.IsDead &&
                            enemy.IsValidTarget(E.Range))
                    .FirstOrDefault(enemy => enemy.Buffs.Any(buff => buff.DisplayName == "TristanaEChargeSound"));

        public override void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var t = target as AIHeroClient;
            if (t == null || (!ComboActive && !HarassActive) || !unit.IsMe)
                return;

            var useW = GetValue<bool>("UseW" + (ComboActive ? "C" : "H"));

            if (useW && W.IsReady())
                W.Cast(t, false, true);
        }

        public override void DrawingOnEndScene(EventArgs args)
        {
            //foreach (var e in HeroManager.Enemies.Where(e => e.IsValidTarget(E.Range +200) && e.HasBuff("TwitchDeadlyVenom")))
            //{
            //    foreach (var b in e.Buffs.Where(b => b.Name == "TwitchDeadlyVenom"))
            //    {

            //    }
            //}
            //var enemy = GetEMarkedEnemy;
            //if (enemy != null)
            //{
            //    for (int i = 1; i < 5; i++)
            //    {
            //        Marksman.Common.CommonGeometry.DrawBox(new Vector2(enemy.HPBarPosition.X + 10 + (i * 17), enemy.HPBarPosition.Y - 15), 15, 6, System.Drawing.Color.Transparent, 1, System.Drawing.Color.Black);
            //    }

            //    var eCount = GetEMarkedCount;
            //    for (int i = 1; i <= eCount; i++)
            //    {
            //        Marksman.Common.CommonGeometry.DrawBox(new Vector2(enemy.HPBarPosition.X + 11 + (i * 17), enemy.HPBarPosition.Y - 14), 13, 5, System.Drawing.Color.Red, 0, System.Drawing.Color.Black);
            //    }
            //}
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            Spell[] spellList = {W, R};
            foreach (var spell in spellList)
            {
                var menuItem = GetValue<Circle>("Draw" + spell.Slot);
                if (menuItem.Active)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range,
                        spell.IsReady() ? menuItem.Color : Color.LightSlateGray);
                }
            }
        }

        public override void GameOnUpdate(EventArgs args)
        {
            if (Orbwalking.CanMove(100) && (ComboActive || HarassActive))
            {
                var useW = GetValue<bool>("UseW" + (ComboActive ? "C" : "H"));
                var useE = GetValue<bool>("UseE" + (ComboActive ? "C" : "H"));

                var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (!t.IsValidTarget() || t.HasKindredUltiBuff())
                {
                    return;
                }

                if (useW && W.IsReady() && t.IsValidTarget(W.Range))
                {
                    W.Cast(t, false, true);
                }

                if (ObjectManager.Get<AIHeroClient>().Find(e1 => e1.IsValidTarget(E.Range) && E.IsKillable(e1)) != null)
                {
                    E.Cast();
                }

                var nShouldCastE = (t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65) && E.IsKillable(t)) ||
                                   (!t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65) &&
                                    t.GetBuffCount("TwitchDeadlyVenom") == 6);

                if (E.IsReady() && nShouldCastE)
                {
                    E.Cast();
                }
                //if (useE && E.IsReady() && t.GetBuffCount("TwitchDeadlyVenom") == 6)
                //{
                //    E.Cast();
                //}
            }

            if (ComboActive)
            {
                var useR = GetValue<StringList>("Combo.UseR").SelectedIndex;

                if (useR != 0 && R.IsReady() && ObjectManager.Player.CountEnemiesInRange(R.Range - 100) >= useR)
                {
                    R.Cast();
                }
            }

            if (GetValue<bool>("UseEM") && E.IsReady())
            {
                foreach (
                    var hero in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                hero =>
                                    hero.IsValidTarget(E.Range) &&
                                    (ObjectManager.Player.GetSpellDamage(hero, SpellSlot.E) - 10 > hero.Health)))
                {
                    E.Cast();
                }
            }
        }

        public override void ExecuteLane()
        {
            //var prepareMinions = Program.Config.Item("PrepareMinionsE.Lane").GetValue<StringList>().SelectedIndex;
            //if (prepareMinions != 0)
            //{
            //    List<Obj_AI_Minion> list = new List<Obj_AI_Minion>();

            //    IEnumerable<Obj_AI_Minion> minions =
            //        from m in
            //            ObjectManager.Get<Obj_AI_Minion>()
            //                .Where(
            //                    m =>
            //                        m.Health > ObjectManager.Player.TotalAttackDamage &&
            //                        m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))
            //        select m;

            //    var objAiMinions = minions as Obj_AI_Minion[] ?? minions.ToArray();
            //    foreach (var m in objAiMinions)
            //    {
            //        if (m.GetBuffCount(twitchEBuffName) > 0)
            //        {
            //            list.Add(m);
            //        }
            //        else
            //        {
            //            list.Remove(m);
            //        }
            //    }

            //    foreach (var l in objAiMinions.Except(list).ToList())
            //    {
            //        Program.ChampionClass.Orbwalker.ForceTarget(l);
            //    }
            //}
        }

        public override void ExecuteJungle()
        {
            var jungleWValue = Program.Config.Item("UseW.Jungle").GetValue<StringList>().SelectedIndex;
            if (W.IsReady() && jungleWValue != 0)
            {
                var jungleMobs = Utils.Utils.GetMobs(W.Range,
                    jungleWValue != 3 ? Utils.Utils.MobTypes.All : Utils.Utils.MobTypes.BigBoys,
                    jungleWValue != 3 ? jungleWValue : 1);

                if (jungleMobs != null)
                {
                    W.Cast(jungleMobs);
                }
            }

            if (E.IsReady() && Program.Config.Item("UseE.Jungle").GetValue<StringList>().SelectedIndex != 0)
            {
                var jungleMobs = Utils.Utils.GetMobs(E.Range,
                    Program.Config.Item("UseE.Jungle").GetValue<StringList>().SelectedIndex == 1
                        ? Utils.Utils.MobTypes.All
                        : Utils.Utils.MobTypes.BigBoys);

                if (jungleMobs != null && E.CanCast(jungleMobs) && jungleMobs.Health <= E.GetDamage(jungleMobs) + 20)
                {
                    E.Cast();
                }
            }
        }


        private static float GetComboDamage(AIHeroClient t)
        {
            var fComboDamage = 0f;

            if (E.IsReady())
                fComboDamage += E.GetDamage(t);

            if (ObjectManager.Player.GetSpellSlot("summonerdot") != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell(ObjectManager.Player.GetSpellSlot("summonerdot")) ==
                SpellState.Ready && ObjectManager.Player.Distance(t) < 550)
                fComboDamage += (float) ObjectManager.Player.GetSummonerSpellDamage(t, Damage.SummonerSpell.Ignite);

            if (Items.CanUseItem(3144) && ObjectManager.Player.Distance(t) < 550)
                fComboDamage += (float) ObjectManager.Player.GetItemDamage(t, Damage.DamageItems.Bilgewater);

            if (Items.CanUseItem(3153) && ObjectManager.Player.Distance(t) < 550)
                fComboDamage += (float) ObjectManager.Player.GetItemDamage(t, Damage.DamageItems.Botrk);

            return fComboDamage;
        }

        public override bool ComboMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseWC" + Id, "W:").SetValue(true));
            config.AddItem(new MenuItem("UseEC" + Id, "E:").SetValue(true));

            var e = new string[5];
            e[0] = "Off";
            for (var i = 1; i < 5; i++)
            {
                e[i] = "Enemy Marked >= " + i;
            }
            config.AddItem(new MenuItem("Combo.DontWasteE" + Id, "Don't Waste E:").SetValue(new StringList(e, 0)));


            var sl = new string[5];
            sl[0] = "Off";
            for (var i = 1; i < 5; i++)
            {
                sl[i] = "Enemy Count >= " + i;
            }

            config.AddItem(new MenuItem("Combo.UseR" + Id, "R:").SetValue(new StringList(sl, 3)));
            config.AddItem(new MenuItem("Combo.UseYoumuu" + Id, "Use Youmuu With R:").SetValue(true));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseWH" + Id, "Use W").SetValue(false));
            config.AddItem(new MenuItem("UseEH" + Id, "Use E at max Stacks").SetValue(false));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.AddItem(new MenuItem("DrawW" + Id, "W:").SetValue(new Circle(true, Color.Wheat)));
            config.AddItem(new MenuItem("DrawR" + Id, "R:").SetValue(new Circle(true, Color.Coral)));

            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Damage After Combo").SetValue(true);
            config.AddItem(dmgAfterComboItem);

            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseEM" + Id, "Use E KS").SetValue(true));
            return true;
        }

        public override bool LaneClearMenu(Menu menuLane)
        {
            menuLane.AddItem(
                new MenuItem("PrepareMinionsE.Lane", "Prepare Minions for E").SetValue(
                    new StringList(new[] {"Off", "Everytime", "Just Under Ally Turret"}, 2)));

            var strW = new string[6];
            strW[0] = "Off";

            for (var i = 1; i < 6; i++)
            {
                strW[i] = "If Could Infect Minion Count>= " + i;
            }

            menuLane.AddItem(new MenuItem("UseW.Lane", "Use W:").SetValue(new StringList(strW, 0)));


            var strE = new string[6];
            strE[0] = "Off";

            for (var i = 1; i < 6; i++)
            {
                strE[i] = "Minion Count >= " + i;
            }

            menuLane.AddItem(new MenuItem("UseE.Lane", "Use E:").SetValue(new StringList(strE, 0)));
            return true;
        }

        public override bool JungleClearMenu(Menu menuJungle)
        {
            var strW = new string[4];
            strW[0] = "Off";
            strW[3] = "Just big Monsters";

            for (var i = 1; i < 3; i++)
            {
                strW[i] = "If Could Infect Mobs Count>= " + i;
            }

            menuJungle.AddItem(new MenuItem("UseW.Jungle", "Use W:").SetValue(new StringList(strW, 3)));

            //config.AddItem(new MenuItem("UseW.Jungle", "Use W").SetValue(new StringList(new[] { "Off", "On", "Just big Monsters" }, 2)));
            menuJungle.AddItem(
                new MenuItem("UseE.Jungle", "Use E").SetValue(new StringList(new[] {"Off", "On", "Just big Monsters"}, 2)));

            return true;
        }

        public override void PermaActive()
        {
            if (GetValue<bool>("Combo.UseYoumuu"))
            {
                if (ObjectManager.Player.HasBuff("TwitchFullAutomatic"))
                {
                    var iYoumuu = ItemData.Youmuus_Ghostblade.GetItem();

                    if (iYoumuu != null && iYoumuu.IsReady())
                    {
                        iYoumuu.Cast();
                    }
                }
            }

            var dontWasteE = GetValue<StringList>("Combo.DontWasteE").SelectedIndex;
            if (dontWasteE != 0)
            {
                if (ObjectManager.Player.CountEnemiesInRange(Orbwalking.GetRealAutoAttackRange(null) + 65) > 0)
                {
                    return;
                }

                foreach (var enemy in HeroManager.Enemies
                    .Where(
                        e => e.IsValidTarget(E.Range) && !e.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 130))
                    .Where(e => e.GetBuffCount("TwitchDeadlyVenom") >= dontWasteE))
                {
                    foreach (var buffs in enemy.Buffs.Where(b => b.Name == "TwitchDeadlyVenom"))
                    {
                        if (Game.Time > buffs.EndTime - 200)
                        {
                            E.Cast();
                        }
                    }
                }
            }
        }
    }
}