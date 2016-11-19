#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using LeagueSharp;
using LeagueSharp.Common;

using Marksman.Utils;
using SharpDX;
using Font = SharpDX.Direct3D9.Font;
using Orbwalking = LeagueSharp.Common.Orbwalking;

#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Marksman.Champions
{
    internal class Tristana : Champion
    {
        public static AIHeroClient Player = ObjectManager.Player;
        public static Spell Q, W, E, R;

        private static Vector3 firstJumpPosition;
        private static int firstJumpTick;


        public Tristana()
        {
            Q = new Spell(SpellSlot.Q, 703);

            W = new Spell(SpellSlot.W, 900);
            W.SetSkillshot(.50f, 250f, 1400f, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 703);
            R = new Spell(SpellSlot.R, 703);

            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = TristanaData.GetComboDamage;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = true;

            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

            //var i = 0;
            //foreach (var e in HeroManager.Enemies)
            //{
            //    nButton[i] = new CheckBoxButtonBeta
            //    {
            //        Name = "btn" + e.ChampionName,
            //        Caption = "Nearby enemy object",
            //        ButtonColor = System.Drawing.Color.GreenYellow,
            //        Font = Common.CommonGeometry.Text,
            //        Height = 21,
            //        Width = 0,
            //        Visible = true,
            //        Enabled = true,
            //        Checked = true
            //    };
            //    nButton[i].Initialize();
            //    i += 1;
            //}
            
            Utils.Utils.PrintMessage("Tristana");
        }

        public override void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.W)
            {
                firstJumpPosition = ObjectManager.Player.Position;
                firstJumpTick = LeagueSharp.Common.Utils.TickCount;
            }
        }

        private static BuffInstance GetECharge(Obj_AI_Base target)
        {
            return target.Buffs.Find(x => x.DisplayName == "TristanaECharge");
        }

        private static bool IsECharged(Obj_AI_Base target)
        {
            return GetECharge(target) != null;
        }


        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.IsReady() && gapcloser.Sender.IsValidTarget(R.Range) && GetValue<bool>("UseRMG"))
            {
                R.CastOnUnit(gapcloser.Sender);
            }
        }

        private AIHeroClient GetTarget
        {
            get
            {
                var nRange = Orbwalking.GetRealAutoAttackRange(null) + 65 >= E.Range ? Orbwalking.GetRealAutoAttackRange(null) + 65 : E.Range;
                var nResult = ObjectManager.Get<AIHeroClient>()
                        .Where(
                            enemy =>
                                !enemy.IsDead &&
                                enemy.IsValidTarget(nRange))
                        .Find(enemy => enemy.Buffs.Any(buff => buff.Name == "tristanaechargesound" || buff.Name == "tristanaecharge"));

                if (nResult != null)
                {
                    return nResult;
                }

                return  TargetSelector.GetTarget(nRange, TargetSelector.DamageType.Physical);
            }
        }
        public override void Interrupter2_OnInterruptableTarget(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && unit.IsValidTarget(R.Range) && GetValue<bool>("UseRMI"))
            {
                R.CastOnUnit(unit);
            }
        }

        public override void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            args.Process = !GetValue<KeyBind>("Combo.Insec").Active;

                if (GetValue<bool>("Misc.UseQ.Inhibitor") && args.Target is Obj_BarracksDampener && Q.IsReady())
            {
                if (((Obj_BarracksDampener) args.Target).Health >= Player.TotalAttackDamage*3)
                {
                    Q.Cast();
                }
            }

            if (GetValue<bool>("Misc.UseQ.Nexus") && args.Target is Obj_HQ && Q.IsReady())
            {
                Q.Cast();
            }

            var unit = args.Target as Obj_AI_Turret;
            if (unit != null)
            {
                if (GetValue<bool>("UseEM") && E.IsReady())
                {
                    if (((Obj_AI_Turret) args.Target).Health >= Player.TotalAttackDamage*3)
                    {
                        E.CastOnUnit(unit);
                    }
                }

                if (GetValue<bool>("Misc.UseQ.Turret") && Q.IsReady())
                {
                    if (((Obj_AI_Turret) args.Target).Health >= Player.TotalAttackDamage*3)
                    {
                        Q.Cast();
                    }
                }
            }

            if (Q.IsReady() && args.Target is AIHeroClient)
            {
                var t = args.Target as AIHeroClient;
                if (t.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) - 65) && ComboActive)
                {
                    Q.Cast();
                }
            }
        }

        public override void GameOnUpdate(EventArgs args)
        {
            E.Range = 630 + (7 * (Player.Level - 1));
            R.Range = 630 + (7 * (Player.Level - 1));

            //DrawButtons();

            //if (!W.IsReady())
            //{
            //    firstJumpPosition = Vector3.Zero;
            //}

            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (GetValue<KeyBind>("Combo.Insec").Active)
            {
                return;
            }

            AIHeroClient t = GetTarget;
            
            if (!t.IsValidTarget())
            {
                return;
            }
            //Console.WriteLine(t.ChampionName + " : " + t.CountEnemiesInRange(600));
            if (GetValue<KeyBind>("UseETH").Active && E.IsReady() && t.IsValidTarget(E.Range) && ToggleActive)
            {
                if (GetValue<bool>("DontEToggleHarass" + t.ChampionName) == false)
                {
                    E.CastOnUnit(t);
                }
            }

            if (ComboActive)
            {
                if (E.CanCast(t))
                {
                    if (GetValue<bool>("DontEToggleHarass" + t.ChampionName) == false)
                    {
                        E.CastOnUnit(t);
                    }

                    if (GetValue<bool>("DontEToggleHarass" + t.ChampionName) == true && t.CountEnemiesInRange(600) == 1)
                    {
                        E.CastOnUnit(t);
                    }
                }

                //if (GetValue<bool>("UseREC") && R.IsReady())
                //{
                //    var enemy = HeroManager.Enemies.Find(e => e.IsValidTarget(R.Range) && e.Health < R.GetDamage(e) + TristanaData.GetEDamage && IsECharged(e));
                //    if (enemy != null)
                //    {
                //        R.CastOnUnit(enemy);
                //    }
                //}
            }
        }

        public override void ExecuteJungle()
        {
            var jungleMobs = Marksman.Utils.Utils.GetMobs(E.Range, Marksman.Utils.Utils.MobTypes.All);

            if (jungleMobs != null)
            {
                if (E.IsReady())
                {
                    switch (GetValue<StringList>("Jungle.UseE").SelectedIndex)
                    {
                        case 1:
                        {
                            E.CastOnUnit(jungleMobs);
                            break;
                        }
                        case 2:
                        {
                            jungleMobs = Utils.Utils.GetMobs(E.Range, Utils.Utils.MobTypes.BigBoys);
                            if (jungleMobs != null)
                            {
                                E.CastOnUnit(jungleMobs);
                            }
                            break;
                        }
                    }
                }

                if (Q.IsReady())
                {
                    var jE = GetValue<StringList>("Jungle.UseQ").SelectedIndex;
                    if (jE != 0)
                    {
                        if (jE == 1)
                        {
                            jungleMobs = Utils.Utils.GetMobs(
                                Orbwalking.GetRealAutoAttackRange(null) + 65,
                                Utils.Utils.MobTypes.BigBoys);
                            if (jungleMobs != null)
                            {
                                Q.Cast();
                            }
                        }
                        else
                        {
                            var totalAa =
                                ObjectManager.Get<Obj_AI_Minion>()
                                    .Where(
                                        m =>
                                            m.Team == GameObjectTeam.Neutral &&
                                            m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 165))
                                    .Sum(mob => (int) mob.Health);

                            totalAa = (int) (totalAa/ObjectManager.Player.TotalAttackDamage);
                            if (totalAa > jE)
                            {
                                Q.Cast();
                            }

                        }
                    }
                }
            }
        }

        public override void ExecuteLane()
        {

            if (E.IsReady())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.Position, E.Range, MinionTypes.All,
                    MinionTeam.Enemy);

                if (minions != null)
                {
                    var eJ = Program.Config.Item("UseE.Lane").GetValue<StringList>().SelectedIndex;
                    if (eJ != 0)
                    {
                        var mE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range + 175,
                            MinionTypes.All);
                        var locW = E.GetCircularFarmLocation(mE, 175);
                        if (locW.MinionsHit >= eJ && E.IsInRange(locW.Position.To3D()))
                        {
                            foreach (
                                var x in
                                    ObjectManager.Get<Obj_AI_Minion>()
                                        .Where(m => m.IsEnemy && !m.IsDead && m.Distance(locW.Position) < 100))
                            {
                                E.CastOnUnit(x);
                            }
                        }
                    }
                }
                if (Q.IsReady())
                {
                    var jE = Program.Config.Item("UseQ.Lane").GetValue<StringList>().SelectedIndex;
                    if (jE != 0)
                    {
                        var totalAa =
                            ObjectManager.Get<Obj_AI_Minion>()
                                .Where(
                                    m =>
                                        m.IsEnemy && !m.IsDead &&
                                        m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null)))
                                .Sum(mob => (int) mob.Health);

                        totalAa = (int) (totalAa/ObjectManager.Player.TotalAttackDamage);
                        if (totalAa > jE)
                        {
                            Q.Cast();
                        }
                    }
                }
            }
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            foreach (var i in Config.Items.Where(e=> e.Name == "DontEToggleHarass"))
            {
                

            }
            if (GetValue<KeyBind>("Combo.Insec").Active && R.IsReady())
            {
                Drawing.DrawText(ObjectManager.Player.HPBarPosition.X + 150, ObjectManager.Player.HPBarPosition.Y + 5, System.Drawing.Color.Red, "Insec: ON");
            }

            Spell[] spellList = {W, E};
            foreach (var spell in spellList)
            {
                var menuItem = GetValue<Circle>("Draw" + spell.Slot);
                if (menuItem.Active)
                    Render.Circle.DrawCircle(Player.Position, spell.Range, menuItem.Color, 1);
            }
        }

        public override bool ComboMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseREC" + Id, "E-R Kill Combo:").SetValue(true));
            config.AddItem(new MenuItem("UseR" + Id, "R:").SetValue(true));
            config.AddItem(new MenuItem("Combo.Insec" + Id, "Insec [W - R]:").SetValue(new KeyBind("G".ToCharArray()[0],KeyBindType.Press)));
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            config.AddSubMenu(new Menu("Don't E Toggle to", "DontEToggleHarass"));
            {
                foreach (var enemy in
                    ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
                {
                    config.SubMenu("DontEToggleHarass")
                        .AddItem(
                            new MenuItem("DontEToggleHarass" + enemy.ChampionName + Id, enemy.ChampionName).SetValue(false));
                }
            }

            config.AddItem(
                new MenuItem("UseETH" + Id, "Use E (Toggle)").SetValue(new KeyBind("H".ToCharArray()[0],
                    KeyBindType.Toggle))).Permashow(true, "Tristana | Toggle E");

            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.AddItem(new MenuItem("DrawW" + Id, "W:").SetValue(new Circle(true, System.Drawing.Color.Beige)));
            config.AddItem(new MenuItem("DrawE" + Id, "E:").SetValue(new Circle(true, System.Drawing.Color.Orange)));

            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Damage After Combo").SetValue(true);
            config.AddItem(dmgAfterComboItem);

            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            var menuMiscQ = new Menu("Q Spell", "MiscQ");
            menuMiscQ.AddItem(new MenuItem("Misc.UseQ.Turret" + Id, "Use Q for Turret").SetValue(true));
            menuMiscQ.AddItem(new MenuItem("Misc.UseQ.Inhibitor" + Id, "Use Q for Inhibitor").SetValue(true));
            menuMiscQ.AddItem(new MenuItem("Misc.UseQ.Nexus" + Id, "Use Q for Nexus").SetValue(true));
            config.AddSubMenu(menuMiscQ);

            var menuMiscW = new Menu("W Spell", "MiscW");
            menuMiscW.AddItem(new MenuItem("ProtectWMana", "[Soon/WIP] Protect my mana for [W] if my Level < ").SetValue(new Slider(8, 2, 18)));
            menuMiscW.AddItem(new MenuItem("UseWM" + Id, "Use W KillSteal").SetValue(false));
            config.AddSubMenu(menuMiscW);

            var menuMiscE = new Menu("E Spell", "MiscE");
            menuMiscE.AddItem(new MenuItem("UseEM" + Id, "Use E for Enemy Turret").SetValue(true));
            config.AddSubMenu(menuMiscE);

            var menuMiscR = new Menu("R Spell", "MiscR");
            {
                menuMiscR.AddItem(new MenuItem("ProtectRMana", "[Soon/WIP] Protect my mana for [R] if my Level < ").SetValue(new Slider(11, 6, 18)));
                menuMiscR.AddItem(new MenuItem("UseRM" + Id, "Use R KillSteal").SetValue(true));
                menuMiscR.AddItem(new MenuItem("UseRMG" + Id, "Use R Gapclosers").SetValue(true));
                menuMiscR.AddItem(new MenuItem("UseRMI" + Id, "Use R Interrupt").SetValue(true));
                config.AddSubMenu(menuMiscR);
            }

            return true;
        }

        public override bool LaneClearMenu(Menu menuLane)
        {
            string[] strQ = new string[7];
            strQ[0] = "Off";

            for (var i = 1; i < 7; i++)
            {
                strQ[i] = "If need to AA more than >= " + i;
            }

            menuLane.AddItem(new MenuItem("UseQ.Lane", "Q:").SetValue(new StringList(strQ, 0))).SetFontStyle(FontStyle.Regular, Q.MenuColor());

            string[] strE = new string[5];
            strE[0] = "Off";

            for (var i = 1; i < 5; i++)
            {
                strE[i] = "Minion Count >= " + i;
            }

            menuLane.AddItem(new MenuItem("UseE.Lane", "E:").SetValue(new StringList(strE, 0))).SetFontStyle(FontStyle.Regular, E.MenuColor());
            ;
            return true;
        }

        public override bool JungleClearMenu(Menu menuJungle)
        {
            string[] strLaneMinCount = new string[8];
            strLaneMinCount[0] = "Off";
            strLaneMinCount[1] = "Just for big Monsters";

            for (var i = 2; i < 8; i++)
            {
                strLaneMinCount[i] = "If need to AA more than >= " + i;
            }

            menuJungle.AddItem(new MenuItem("Jungle.UseQ", "Q:").SetValue(new StringList(strLaneMinCount, 4))).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            menuJungle.AddItem(new MenuItem("Jungle.UseE", "E:").SetValue(new StringList(new[] {"Off", "On", "Just for big Monsters"},1))).SetFontStyle(FontStyle.Regular, E.MenuColor());

            return true;
        }

        public override void DrawingOnEndScene(EventArgs args)
        {
            if (GetValue<KeyBind>("Combo.Insec").Active && R.IsReady())
            {
                if (firstJumpTick + 3000 > LeagueSharp.Common.Utils.TickCount)
                {
                    Render.Circle.DrawCircle(firstJumpPosition, 50f, System.Drawing.Color.Red);
                }

                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Red);


                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                var t = TargetSelector.GetTarget(W.Range * 2, TargetSelector.DamageType.Physical);

                if (t.IsValidTarget())
                {
                    var targetBehind = t.Position.Extend(ObjectManager.Player.Position, -200);
                    if (W.IsReady() && ObjectManager.Player.Distance(targetBehind) <= W.Range)
                    {
                        W.Cast(targetBehind);
                    }
                    if (ObjectManager.Player.Distance(firstJumpPosition) > t.Distance(firstJumpPosition) && ObjectManager.Player.Distance(t.Position) >= t.BoundingRadius)
                    {
                        R.CastOnUnit(t);
                    }
                    Render.Circle.DrawCircle(targetBehind, 50f, System.Drawing.Color.GreenYellow);
                }
            }


            var enemy = TristanaData.GetEMarkedEnemy;
            if (enemy != null)
            {
                for (int i = 1; i < 5; i++)
                {
                    Marksman.Common.CommonGeometry.DrawBox(new Vector2(enemy.HPBarPosition.X + 10 + (i*17), enemy.HPBarPosition.Y - 15), 15, 6, System.Drawing.Color.Transparent, 1, System.Drawing.Color.Black);
                }

                var eCount = TristanaData.GetEMarkedCount;
                for (int i = 1; i <= eCount; i++)
                {
                    Marksman.Common.CommonGeometry.DrawBox(new Vector2(enemy.HPBarPosition.X + 11 + (i*17), enemy.HPBarPosition.Y - 14), 13, 5, System.Drawing.Color.Red, 0, System.Drawing.Color.Black);
                }
            }
        }

        public class TristanaData
        {
            //public static double GetWDamage
            //{
            //    get
            //    {
            //        if (W.IsReady())
            //        {
            //            var wDamage = new double[] {80, 105, 130, 155, 180}[W.Level - 1] + 0.5*Player.FlatMagicDamageMod;
            //            if (GetEMarkedCount > 0 && GetEMarkedCount < 4)
            //            {
            //                return wDamage + (wDamage*GetEMarkedCount*.20);
            //            }
            //            switch (GetEMarkedCount)
            //            {
            //                case 0:
            //                    return wDamage;
            //                case 4:
            //                    return wDamage*2;
            //            }
            //        }
            //        return 0;
            //    }
            //}

            public static double GetEDamage(Obj_AI_Base t)
            {
                if (E.Level == 0)
                {
                    return 0f;
                }

                if (t.Buffs.Count(x => x.DisplayName == "TristanaECharge") == 0)
                {
                    return 0;
                }

                var nMagicDamage = Player.TotalMagicalDamage*0.25 +
                                   new double[] {50f, 75f, 100f, 125f, 150f}[E.Level - 1];

                var nPhysicalDamage = (Player.TotalMagicalDamage*0.50) +
                                      new double[] {60, 70, 80, 90, 100}[E.Level - 1] +
                                      (Player.TotalAttackDamage/100*new double[] {50f, 65f, 80f, 95f, 110f}[E.Level - 1]);

                var nPhysicalDamageBonus = (Player.TotalMagicalDamage*0.15) +
                                           new double[] {18f, 21f, 24, 27, 30f}[E.Level - 1] +
                                           (Player.TotalAttackDamage/100*
                                            new double[] {15f, 19.5f, 24f, 28.5f, 33f}[E.Level - 1]);

                var result = nMagicDamage + nPhysicalDamage + nPhysicalDamageBonus;
                return result;
            }

            public static float GetComboDamage(AIHeroClient t)
            {
                if (!t.IsValidTarget(W.Range * 20))
                {
                    return 0;
                }

                var fComboDamage = 0d;
                
                if (Q.IsReady())
                {
                    var baseAttackSpeed = 0.656 + (0.656 / 100 * (Player.Level - 1) * 1.5);
                    var qExtraAttackSpeed = new double[] { 30, 50, 70, 90, 110 }[Q.Level - 1];
                    var attackDelay = (float) (baseAttackSpeed + (baseAttackSpeed / 100 * qExtraAttackSpeed));
                    attackDelay = (float) Math.Round(attackDelay, 2);

                    attackDelay *= 5;
                    attackDelay *= (float) Math.Floor(Player.TotalAttackDamage);
                    fComboDamage += attackDelay;
                }
                    
                if (E.IsReady())
                {
                    fComboDamage += TristanaData.GetEDamage(t); //E.GetDamage(t);
                }

                if (R.IsReady())
                {
                    fComboDamage += R.GetDamage(t);
                }

                return (float) fComboDamage;
            }

            public static AIHeroClient GetEMarkedEnemy
                =>
                    ObjectManager.Get<AIHeroClient>()
                        .Where(
                            enemy =>
                                !enemy.IsDead &&
                                enemy.IsValidTarget(W.Range + Orbwalking.GetRealAutoAttackRange(Player)))
                        .FirstOrDefault(enemy => enemy.Buffs.Any(buff => buff.Name == "tristanaechargesound"));

            //public static Obj_AI_Base GetEMarkedObjects
            //    =>
            //        ObjectManager.Get<Obj_AI_Base>()
            //            .Where(
            //                enemy =>
            //                    !enemy.IsDead &&
            //                    enemy.IsValidTarget(W.Range + Orbwalking.GetRealAutoAttackRange(Player)))
            //            .FirstOrDefault(enemy => enemy.Buffs.Any(buff => buff.DisplayName == "TristanaEChargeSound"));

            public static int GetEMarkedCount
                =>
                    GetEMarkedEnemy?.Buffs.Where(buff => buff.Name == "tristanaecharge")
                        .Select(xBuff => xBuff.Count)
                        .FirstOrDefault() ?? 0;

            //public static int GetEMarkedObjectsCount
            //    =>
            //        GetEMarkedObjects?.Buffs.Where(buff => buff.DisplayName == "TristanaECharge")
            //            .Select(xBuff => xBuff.Count)
            //            .FirstOrDefault() ?? 0;
        }
    }
}