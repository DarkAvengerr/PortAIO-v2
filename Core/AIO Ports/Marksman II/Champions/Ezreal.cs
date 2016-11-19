#region
using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Marksman.Common;

#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Marksman.Champions
{
    using Marksman.Utils;

    internal class Ezreal : Champion
    {
        public static Spell Q, W, E, R;

        private int _lastFarmQKill;

        private static bool haveIceBorn = false;

        public Ezreal()
        {
            Q = new Spell(SpellSlot.Q, 1190);
            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 920);
            W.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);

            E = new Spell(SpellSlot.E, 475);

            R = new Spell(SpellSlot.R, 5000);
            R.SetSkillshot(1f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = true;


            //Obj_AI_Base.OnBuffGain += (sender, args) =>
            //{
            //    //if (sender.IsMe)
            //    //Chat.Print(args.Buff.Name);
            //};

            //foreach (var e in HeroManager.Enemies)
            //{
            //    var nButton = new CheckBoxButtonBeta
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
            //    nButton.Initialize();
            //}

            Utils.PrintMessage("Ezreal loaded");
        }


        public override void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            //var t = target as AIHeroClient;
            //if (t != null && (ComboActive || HarassActive) && unit.IsMe && !t.HasKindredUltiBuff())
            //{
            //    var useQ = GetValue<bool>("UseQ" + (ComboActive ? "C" : "H"));
            //    var useW = GetValue<bool>("UseW" + (ComboActive ? "C" : "H"));

            //    if (Q.IsReady() && useQ)
            //    {
            //        Q.CastIfHitchanceGreaterOrEqual(t);
            //    }
            //    else if (W.IsReady() && useW)
            //    {
            //        W.CastIfHitchanceGreaterOrEqual(t);
            //    }
            //}
        }

        public override void Drawing_OnDraw(EventArgs args)
        {
            if (ComboActive && GetValue<bool>("UseEC") && E.IsReady())
            {
                var nRange = ObjectManager.Player.HealthPercent < 25 ? 550 : 450;
                var nSum =
                    HeroManager.Enemies.Where(e => e.IsValidTarget(nRange) && e.IsFacing(ObjectManager.Player))
                        .Sum(e => e.HealthPercent);
                if (nSum > ObjectManager.Player.HealthPercent)
                {
                    var nResult = HeroManager.Enemies.FirstOrDefault(e => e.IsValidTarget(nRange));
                    if (nResult != null)
                    {
                        var nPosition = ObjectManager.Player.Position.Extend(nResult.Position, -E.Range);
                        E.Cast(nPosition);
                    }
                }
                Render.Circle.DrawCircle(ObjectManager.Player.Position, nRange, Color.DarkSalmon);
            }

            var t = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Magical);
            if (t != null)
            {
                var x = ObjectManager.Player.Position.Extend(t.Position, -E.Range);
                //var x = t.Position.Extend(ObjectManager.Player.Position, -E.Range);
                Render.Circle.DrawCircle(x, 105f, Color.DarkSalmon);

            }
            foreach (
                var enemy in
                    HeroManager.Enemies.Where(
                        enemy => R.IsReady() && enemy.IsValidTarget() && R.GetDamage(enemy) > enemy.Health))
            {
                Marksman.Common.CommonGeometry.DrawBox(new Vector2(Drawing.Width*0.43f, Drawing.Height*0.80f), 185, 18,
                    Color.FromArgb(242, 255, 236, 6), 1,
                    System.Drawing.Color.Black);
                Marksman.Common.CommonGeometry.DrawText(Marksman.Common.CommonGeometry.Text,
                    "Killable enemy with ultimate: " + enemy.ChampionName, Drawing.Width*0.435f,
                    Drawing.Height*0.803f, SharpDX.Color.Black);
            }

            Spell[] spellList = {Q, W};
            foreach (var spell in spellList)
            {
                var menuItem = GetValue<Circle>("Draw" + spell.Slot);
                if (menuItem.Active)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
            }

            var drawRMin = Program.Config.SubMenu("Combo").Item("DrawRMin").GetValue<Circle>();
            if (drawRMin.Active)
            {
                var minRRange = Program.Config.SubMenu("Combo").Item("UseRCMinRange").GetValue<Slider>().Value;
                Render.Circle.DrawCircle(ObjectManager.Player.Position, minRRange, drawRMin.Color, 2);
            }

            var drawRMax = Program.Config.SubMenu("Combo").Item("DrawRMax").GetValue<Circle>();
            if (drawRMax.Active)
            {
                var maxRRange = Program.Config.SubMenu("Combo").Item("UseRCMaxRange").GetValue<Slider>().Value;
                Render.Circle.DrawCircle(ObjectManager.Player.Position, maxRRange, drawRMax.Color, 2);
            }
        }

        public override void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (GetValue<bool>("Misc.E.Antigapcloser"))
                if (gapcloser.End.Distance(ObjectManager.Player.Position) <= 200)
                    if (gapcloser.Sender.IsValidTarget())
                        if (gapcloser.Sender.ChampionName.ToLowerInvariant() != "masteryi")
                            if (E.IsReady())
                                E.Cast(ObjectManager.Player.Position.Extend(gapcloser.Sender.Position, -E.Range));
        }

        public override void DrawingOnEndScene(EventArgs args)
        {
            if (GetValue<bool>("PingDH"))
            {
                var i = 0;
                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            enemy => R.IsReady() && enemy.IsValidTarget() && R.GetDamage(enemy) > enemy.Health))
                {
                    //Chat.Print(HeroManager.Enemies[i].ChampionName);
                    float a1 = (i + 1)*0.025f;

                    CommonGeometry.DrawBox(
                        new Vector2(Drawing.Width*0.43f, Drawing.Height*(0.700f + (float) a1)), 150, 18,
                        Color.FromArgb(170, 255, 0, 0), 1, System.Drawing.Color.Black);

                    CommonGeometry.Text.DrawTextCentered(HeroManager.Enemies[i].ChampionName + " Killable with R",
                        (int) (Drawing.Width*0.475f), (int) (Drawing.Height*(0.803f + a1 - 0.093f)), SharpDX.Color.Wheat);

                    i += 1;
                }
            }
        }

        public override void GameOnUpdate(EventArgs args)
        {
            haveIceBorn = ObjectManager.Player.InventoryItems.Any(i => i.Id == ItemId.Iceborn_Gauntlet);

            if (GetValue<bool>("ChargeR.Enable") && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                var rCooldown = GetValue<Slider>("ChargeR.Cooldown").Value;
                var rMinMana = GetValue<Slider>("ChargeR.MinMana").Value;

                if (ObjectManager.Player.ManaPercent >= rMinMana && R.Cooldown >= rCooldown)
                {
                    var vMinions = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range);
                    foreach (var hit in from minions in vMinions
                        select Q.GetPrediction(minions)
                        into qP
                        let hit = qP.CastPosition.Extend(ObjectManager.Player.Position, -140)
                        where qP.Hitchance >= Q.GetHitchance()
                        select hit)
                    {
                        Q.Cast(hit);
                    }
                }
            }

            if (GetValue<bool>("PingCH"))
            {
                foreach (var enemy in
                    HeroManager.Enemies.Where(
                        enemy =>
                            R.IsReady() && enemy.IsValidTarget() && R.GetDamage(enemy) > enemy.Health
                            && enemy.Distance(ObjectManager.Player) > Q.Range))
                {
                    Utils.MPing.Ping(enemy.Position.To2D());
                }
            }

            AIHeroClient t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            var toggleQ = Program.Config.Item("UseQTH").GetValue<KeyBind>().Active;
            var toggleW = Program.Config.Item("UseWTH").GetValue<KeyBind>().Active;
            if ((toggleQ || toggleW) && t.IsValidTarget(Q.Range) && ToggleActive)
            {
                if (Q.IsReady() && toggleQ)
                {
                    if (ObjectManager.Player.HasBuff("Recall")) return;

                    var useQt = (Program.Config.Item("DontQToggleHarass" + t.ChampionName) != null
                                 && Program.Config.Item("DontQToggleHarass" + t.ChampionName).GetValue<bool>() == false);
                    if (useQt)
                    {
                        Q.CastIfHitchanceGreaterOrEqual(t);
                    }
                }

                if (W.IsReady() && t.IsValidTarget(W.Range) && toggleW)
                {
                    if (ObjectManager.Player.HasBuff("Recall")) return;
                    var useWt = (Program.Config.Item("DontWToggleHarass" + t.ChampionName) != null
                                 && Program.Config.Item("DontWToggleHarass" + t.ChampionName).GetValue<bool>() == false);
                    if (useWt)
                    {
                        W.Cast(t);
                    }
                }
            }

            if (ComboActive || HarassActive)
            {
                t = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

                var useQ = GetValue<bool>("UseQ" + (ComboActive ? "C" : "H"));
                var useW = GetValue<bool>("UseW" + (ComboActive ? "C" : "H"));
                var useR = Program.Config.SubMenu("Combo").Item("UseRC").GetValue<bool>();

                if (Orbwalking.CanMove(100) && !t.HasKindredUltiBuff())
                {
                    if (useQ && Q.IsReady() && t.IsValidTarget(Q.Range))
                    {
                        Q.CastIfHitchanceGreaterOrEqual(t);
                    }

                    if (useW && W.IsReady() && t.IsValidTarget(W.Range))
                    {
                        W.Cast(t);
                    }

                    if (R.IsReady() && useR)
                    {
                        var maxRRange = Program.Config.SubMenu("Combo").Item("UseRCMaxRange").GetValue<Slider>().Value;
                        var minRRange = Program.Config.SubMenu("Combo").Item("UseRCMinRange").GetValue<Slider>().Value;

                        if (Q.IsReady() && t.IsValidTarget(Q.Range) && Q.GetPrediction(t).CollisionObjects.Count == 0
                            && t.Health < ObjectManager.Player.GetSpellDamage(t, SpellSlot.Q)) return;

                        if (t.IsValidTarget() && ObjectManager.Player.Distance(t) >= minRRange
                            && ObjectManager.Player.Distance(t) <= maxRRange
                            && t.Health <= ObjectManager.Player.GetSpellDamage(t, SpellSlot.R) &&
                            t.Health > ObjectManager.Player.TotalAttackDamage*2)
                        {
                            R.Cast(t);
                        }
                    }
                }
            }

            if (R.IsReady() && GetValue<KeyBind>("CastR").Active)
            {
                var target = HeroManager.Enemies.OrderBy(e => e.HealthPercent).FirstOrDefault();
                if (target != null)
                    R.Cast(target);

                //t = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                //if (t.IsValidTarget()) R.Cast(t);
            }
        }

        public override void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!args.Unit.IsMe)
            {
                return;
            }

            if ((LaneClearActive ||
                 LastHitActive) && args.Target is Obj_AI_Minion)
            {
                args.Process = args.Target.NetworkId != _lastFarmQKill;
            }
        }

        public override bool LaneClearMenu(Menu menuLane)
        {
            var qSubMenu = new Menu("Q Farm", "Lane.QFarm");
            {
                qSubMenu.AddItem(new MenuItem("Lane.UseQ" + Id, "Q: Everytime").SetValue(true)).SetFontStyle(FontStyle.Regular, Q.MenuColor());
                qSubMenu.AddItem(new MenuItem("Lane.UseQ.AARange" + Id, "Q: Auto of AA Range").SetValue(true)).SetFontStyle(FontStyle.Regular, Q.MenuColor());
                qSubMenu.AddItem(new MenuItem("Lane.Q.HeatlhPrediction" + Id, "Q: Health Prediciton").SetValue(true)).SetFontStyle(FontStyle.Regular, Q.MenuColor());
            }
            menuLane.AddSubMenu(qSubMenu);
            return true;
        }

        public override void ExecuteLane()
        {

            if (!Q.IsReady()) {
                return;
            }

            if (!GetValue<bool>("Lane.UseQ") && !GetValue<bool>("Lane.UseQ.AARange") && !GetValue<bool>("Lane.UseQ.HeatlhPrediction")) {
                return;
            }

            var vMinions = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range);

            if (GetValue<bool>("Lane.UseQ")) {
                foreach (var minions in
                    vMinions.Where(
                                   minions => minions.Health < ObjectManager.Player.GetSpellDamage(minions, SpellSlot.Q))) {
                    var qP = Q.GetPrediction(minions);
                    var hit = qP.CastPosition.Extend(ObjectManager.Player.Position, -140);
                    if (qP.Hitchance >= HitChance.High) Q.Cast(hit);
                }
            }

            if (GetValue<bool>("Lane.UseQ.AARange")) {
                foreach (var minions in
                    vMinions.Where(
                                   minions =>
                                   minions.Health < ObjectManager.Player.GetSpellDamage(minions, SpellSlot.Q) &&
                                   !minions.IsValidTarget(Orbwalking.GetRealAutoAttackRange(null) + 65))) {
                    var qP = Q.GetPrediction(minions);
                    var hit = qP.CastPosition.Extend(ObjectManager.Player.Position, -140);
                    if (qP.Hitchance >= HitChance.High) Q.Cast(hit);
                }
            }

            if (GetValue<bool>("Lane.Q.HeatlhPrediction")) {
                foreach (var n in vMinions) {
                    var minion =
                        MinionManager.GetMinions(Q.Range)
                                     .Where(
                                            m =>
                                            Q.GetDamage(m) > m.Health ||
                                            HealthPrediction.LaneClearHealthPrediction(
                                                                                       m, (int) (ObjectManager.Player.AttackDelay * 1000), (int) (Q.Delay * 1000)) >
                                            ObjectManager.Player.GetAutoAttackDamage(m) * 1.5f)
                                     .OrderBy(m => !Orbwalking.InAutoAttackRange(m))
                                     .ThenBy(m => m.Health)
                                     .FirstOrDefault();
                    if (minion != null) {
                        Q.Cast(minion);
                        if (Q.GetDamage(minion) > minion.Health) {
                            _lastFarmQKill = minion.NetworkId;
                        }
                    }
                }
            }
        }

        public override void ExecuteJungle()
        {
            if (GetValue<KeyBind>("Jungle.Steal").Active) {
                if (R.IsReady()) {
                    var blue =
                        MinionManager.GetMinions(5000, MinionTypes.All, MinionTeam.Neutral)
                                     .FirstOrDefault(
                                                     m =>
                                                     HealthPrediction.GetHealthPrediction(m,
                                                                                          (int) (R.Delay + (int) (1000 * ObjectManager.Player.Distance(m) / R.Speed))) <=
                                                     R.GetDamage(m) &&
                                                     m.IsValidTarget(R.Range) &&
                                                     m.CharData.BaseSkinName == "SRU_Blue" && m.IsValidTarget(4000) &&
                                                     HealthPrediction.GetHealthPrediction(m,
                                                                                          (int) (R.Delay + (int) (1000 * ObjectManager.Player.Distance(m) / R.Speed))) >
                                                     m.CountEnemiesInRange(1000) * 70);
                    if (blue != null) {

                        var pred = R.GetPrediction(blue);
                        if (pred.Hitchance >= HitChance.High) {
                            R.Cast(pred.CastPosition);
                        }
                    }

                    var red =
                        MinionManager.GetMinions(5000, MinionTypes.All, MinionTeam.Neutral)
                                     .FirstOrDefault(
                                                     m =>
                                                     HealthPrediction.GetHealthPrediction(m,
                                                                                          (int) (R.Delay + (int) (1000 * ObjectManager.Player.Distance(m) / R.Speed))) <=
                                                     R.GetDamage(m) &&
                                                     m.IsValidTarget(R.Range) &&
                                                     m.CharData.BaseSkinName == "SRU_Red" && m.IsValidTarget(4000) &&
                                                     HealthPrediction.GetHealthPrediction(m,
                                                                                          (int) (R.Delay + (int) (1000 * ObjectManager.Player.Distance(m) / R.Speed))) >
                                                     m.CountEnemiesInRange(1000) * 70);
                    if (red != null) {

                        var pred = R.GetPrediction(red);
                        if (pred.Hitchance >= HitChance.High) {
                            R.Cast(pred.CastPosition);
                        }
                    }

                    var dragon =
                        MinionManager.GetMinions(5000, MinionTypes.All, MinionTeam.Neutral)
                                     .FirstOrDefault(
                                                     m =>
                                                     HealthPrediction.GetHealthPrediction(m,
                                                                                          (int) (R.Delay + (int) (1000 * ObjectManager.Player.Distance(m) / R.Speed))) <=
                                                     R.GetDamage(m) &&
                                                     m.IsValidTarget(R.Range) &&
                                                     m.CharData.BaseSkinName.Contains("Dragon") && m.IsValidTarget(4000) &&
                                                     HealthPrediction.GetHealthPrediction(m,
                                                                                          (int) (R.Delay + (int) (1000 * ObjectManager.Player.Distance(m) / R.Speed))) >
                                                     m.CountEnemiesInRange(1000) * 70);
                    if (dragon != null) {

                        var pred = R.GetPrediction(dragon);
                        if (pred.Hitchance >= HitChance.High) {
                            R.Cast(pred.CastPosition);
                        }
                    }

                    var baron =
                        MinionManager.GetMinions(5000, MinionTypes.All, MinionTeam.Neutral)
                                     .FirstOrDefault(
                                                     m =>
                                                     HealthPrediction.GetHealthPrediction(m,
                                                                                          (int) (R.Delay + (int) (1000 * ObjectManager.Player.Distance(m) / R.Speed))) <=
                                                     R.GetDamage(m) &&
                                                     m.IsValidTarget(R.Range) &&
                                                     m.CharData.BaseSkinName.Contains("Dragon") && m.IsValidTarget(4000) &&
                                                     HealthPrediction.GetHealthPrediction(m,
                                                                                          (int) (R.Delay + (int) (1000 * ObjectManager.Player.Distance(m) / R.Speed))) >
                                                     m.CountEnemiesInRange(1000) * 70);
                    if (baron != null) {
                        var pred = R.GetPrediction(baron);
                        if (pred.Hitchance >= HitChance.High) {
                            R.Cast(pred.CastPosition);
                        }
                    }
                }
            }

            if (!Q.IsReady() || GetValue<StringList>("Jungle.Q").SelectedIndex == 0) {
                return;
            }



            var jungleMobs = Utils.GetMobs(Q.Range, Utils.MobTypes.All);

            if (jungleMobs != null) {
                if (haveIceBorn) {
                    Q.Cast(jungleMobs);
                }
                else {
                    switch (GetValue<StringList>("Jungle.Q").SelectedIndex) {
                        case 1: {
                            Q.Cast(jungleMobs);
                            break;
                        }
                        case 2: {
                            jungleMobs = Utils.GetMobs(Q.Range, Utils.MobTypes.BigBoys);
                            if (jungleMobs != null) {
                                Q.Cast(jungleMobs);
                            }
                            break;
                        }
                    }
                }
            }
        }

        private static float GetComboDamage(AIHeroClient t)
        {
            var fComboDamage = 0f;

            if (Q.IsReady()) fComboDamage += (float) ObjectManager.Player.GetSpellDamage(t, SpellSlot.Q);

            if (W.IsReady()) fComboDamage += (float) ObjectManager.Player.GetSpellDamage(t, SpellSlot.W);

            if (E.IsReady()) fComboDamage += (float) ObjectManager.Player.GetSpellDamage(t, SpellSlot.E);

            if (R.IsReady()) fComboDamage += (float) ObjectManager.Player.GetSpellDamage(t, SpellSlot.R);

            if (ObjectManager.Player.GetSpellSlot("summonerdot") != SpellSlot.Unknown
                && ObjectManager.Player.Spellbook.CanUseSpell(ObjectManager.Player.GetSpellSlot("summonerdot"))
                == SpellState.Ready && ObjectManager.Player.Distance(t) < 550)
                fComboDamage += (float) ObjectManager.Player.GetSummonerSpellDamage(t, Damage.SummonerSpell.Ignite);

            if (Items.CanUseItem(3144) && ObjectManager.Player.Distance(t) < 550)
                fComboDamage += (float) ObjectManager.Player.GetItemDamage(t, Damage.DamageItems.Bilgewater);

            if (Items.CanUseItem(3153) && ObjectManager.Player.Distance(t) < 550)
                fComboDamage += (float) ObjectManager.Player.GetItemDamage(t, Damage.DamageItems.Botrk);

            return fComboDamage;
        }

        public override bool ComboMenu(Menu config)
        {
            config.AddItem(new MenuItem("UseQC" + Id, "Q:").SetValue(true));
            config.AddItem(new MenuItem("UseWC" + Id, "W:").SetValue(true));
            config.AddItem(new MenuItem("UseEC" + Id, "E:").SetValue(true));

            var xRMenu = new Menu("R", "ComboR");
            {
                xRMenu.AddItem(new MenuItem("UseRC", "Use").SetValue(true));
                xRMenu.AddItem(new MenuItem("UseRCMinRange", "Min. Range").SetValue(new Slider(200, 200, 1000)));
                xRMenu.AddItem(new MenuItem("UseRCMaxRange", "Max. Range").SetValue(new Slider(1500, 500, 2000)));
                xRMenu.AddItem(new MenuItem("DrawRMin", "Draw Min. R Range").SetValue(new Circle(true, Color.DarkRed)));
                xRMenu.AddItem(
                               new MenuItem("DrawRMax", "Draw Max. R Range").SetValue(new Circle(true, Color.DarkMagenta)));

                config.AddSubMenu(xRMenu);
            }
            return true;
        }

        public override bool HarassMenu(Menu config)
        {
            var qSubMenu = new Menu("Q:", "Harass.Q");
            {
                qSubMenu.AddItem(new MenuItem("UseQH" + Id, "Use:").SetValue(true));
                qSubMenu.AddItem(new MenuItem("UseQTH", "Toggle:").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Toggle))).Permashow(true, "Marksman | Toggle Q");
                qSubMenu.AddSubMenu(new Menu("Don't Toggle:", "DontQToggleHarass"));
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team)) {
                    qSubMenu.SubMenu("DontQToggleHarass").AddItem(new MenuItem("DontQToggleHarass" + enemy.ChampionName, enemy.ChampionName).SetValue(false));
                }
                config.AddSubMenu(qSubMenu);
            }

            var wSubMenu = new Menu("W:", "Harass.W");
            {
                wSubMenu.AddItem(new MenuItem("UseWH" + Id, "Use:").SetValue(true));
                wSubMenu.AddItem(new MenuItem("UseWTH", "Toggle:").SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Toggle))).Permashow(true, "Marksman | Toggle W");
                wSubMenu.AddSubMenu(new Menu("Don't Toggle:", "DontWToggleHarass"));
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team)) {
                    wSubMenu.SubMenu("DontWToggleHarass").AddItem(new MenuItem("DontWToggleHarass" + enemy.ChampionName, enemy.ChampionName).SetValue(false));
                }
                config.AddSubMenu(wSubMenu);
            }

            return true;
        }

        public override bool MiscMenu(Menu config)
        {
            config.AddItem(new MenuItem("Misc.E.Antigapcloser" + Id, "E: Antigapcloser").SetValue(true));

            config.AddItem(new MenuItem("ChargeR.Enable" + Id, "Charge R with Q").SetValue(false).SetFontStyle(FontStyle.Regular, SharpDX.Color.GreenYellow));
            config.AddItem(new MenuItem("ChargeR.Cooldown" + Id, Utils.Tab + "if R cooldown >").SetValue(new Slider(20, 10, 120)));
            config.AddItem(new MenuItem("ChargeR.MinMana" + Id, Utils.Tab + "And Min. Mana > %").SetValue(new Slider(50, 0, 100)));

            config.AddItem(new MenuItem("CastR" + Id, "Cast R (2000 Range)").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            config.AddItem(new MenuItem("PingCH" + Id, "Ping Killable Enemy with R").SetValue(false));
            config.AddItem(new MenuItem("PingDH" + Id, "Draw Killable Enemy with R").SetValue(false));
            return true;
        }

        public override bool DrawingMenu(Menu config)
        {
            config.AddItem(new MenuItem("DrawQ" + Id, "Q range").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            config.AddItem(new MenuItem("DrawW" + Id, "W range").SetValue(new Circle(false, Color.FromArgb(100, 255, 255, 255))));
            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Damage After Combo").SetValue(true);

            config.AddItem(dmgAfterComboItem);
            return true;
        }


        public override bool JungleClearMenu(Menu menuJungle)
        {
            menuJungle.AddItem(new MenuItem("Jungle.Q" + Id, "Use Q").SetValue(new StringList(new[] {"Off", "On", "Just big Monsters"}, 1)))
                  .SetFontStyle(FontStyle.Regular, Q.MenuColor());
            menuJungle.AddItem(new MenuItem("Jungle.Steal" + Id, "Jungle Steal").SetValue(new KeyBind("U".ToCharArray()[0], KeyBindType.Toggle)))
                  .Permashow(true, "Marksman | Jungle Steal");
            return true;
        }

        public override void PermaActive()
        {

        }
    }
}