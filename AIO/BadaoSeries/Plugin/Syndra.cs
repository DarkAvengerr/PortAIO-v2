using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;
using BadaoSeries.CustomOrbwalker;
using Orbwalking = BadaoSeries.CustomOrbwalker.Orbwalking;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoSeries.Plugin
{
    class SyndraOrbs
    {
        public int Key;
        public GameObject Value;
        public SyndraOrbs(int key, GameObject value)
        {
            Key = key;
            Value = value;
        }
    }
    class LineEQs
    {
        public GameObject Key;
        public Vector2 Value;
        public LineEQs(GameObject key, Vector2 value)
        {
            Key = key;
            Value = value;
        }
    }
    class StunableOrbs
    {
        public AIHeroClient Key;
        public GameObject Value;
        public StunableOrbs(AIHeroClient key, GameObject value)
        {
            Key = key;
            Value = value;
        }
    }
    internal class Syndra : AddUI
    {
        private static int qcount, wcount, ecount, spellcount, waitE, w1cast;
        private static List<SyndraOrbs> SyndraOrb = new List<SyndraOrbs>();

        private static List<Obj_AI_Minion> seed
        {
            get { return ObjectManager.Get<Obj_AI_Minion>().Where(i => i.IsAlly && i.Name == "Seed").ToList(); }
        }

        private static GameObject Wobject()
        {
            return
                ObjectManager.Get<GameObject>().FirstOrDefault(
                    obj => obj.Name.Contains("Syndra_Base_W") && obj.Name.Contains("held") && obj.Name.Contains("02"));
        }
        private static GameObject PickableOrb
        {
            get
            {
                var firstOrDefault = SyndraOrb
                    .FirstOrDefault(x => x.Value.Position.To2D().Distance(Player.Position.To2D()) <= 950);
                return firstOrDefault !=
                       null ? firstOrDefault.Value : null;
            }
        }
        private static Obj_AI_Minion PickableMinion
        {
            get
            {
                var firstOrDefault =  ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(
                            x => x.IsEnemy && x.IsValid && x.Position.To2D().Distance(Player.Position.To2D()) <= 950);
                return firstOrDefault;
            }
        }
        private static List<LineEQs> LineEQ
        {
            get
            {
                if (Wobject() == null)
                {
                    return
                        SyndraOrb.Where(x => x.Value.Position.To2D().Distance(Player.Position.To2D()) <= 700)
                            .Select(
                                x =>
                                    new LineEQs(x.Value,
                                        Player.Position.To2D().Extend(x.Value.Position.To2D(), 1100)))
                            .ToList();
                }
                {
                    return
                        SyndraOrb.Where(
                            x =>
                                x.Value.Position.To2D().Distance(Wobject().Position.To2D()) >= 20 &&
                                x.Value.Position.To2D().Distance(Player.Position.To2D()) <= 700)
                            .Select(
                                x =>
                                    new LineEQs(x.Value,
                                        Player.Position.To2D().Extend(x.Value.Position.To2D(), 1100)))
                            .ToList();
                }
            }
        }

        private static List<StunableOrbs> StunAbleOrb
        {
            get
            {
                return (from orb in LineEQ
                        from target in HeroManager.Enemies.Where(a => a.IsValidTarget())
                        where
                            Prediction.GetPrediction(target, Player.Distance(target) / 1600)
                                .UnitPosition.To2D()
                                .Distance(orb.Key.Position.To2D().Extend(orb.Value, -200), orb.Value, true) <=
                            target.BoundingRadius + 70
                        select new StunableOrbs(target, orb.Key)).ToList();
            }
        }

        private static bool CanEQtarget(AIHeroClient target)
        {
            var pred = E.GetPrediction(target);
            if (pred.Hitchance < HitChance.OutOfRange) return false;
            return Player.Position.To2D().Distance(pred.CastPosition) <= 1200;
        }

        private static Vector2 PositionEQtarget(AIHeroClient target)
        {
            var pred1 = E.GetPrediction(target);
            var pred2 = Q.GetPrediction(target);
            if (pred2.Hitchance >= HitChance.Medium &&
                pred2.UnitPosition.To2D().Distance(Player.Position.To2D()) <= E.Range)
                return pred2.UnitPosition.To2D();
            return pred1.Hitchance >= HitChance.OutOfRange
                ? Player.Position.To2D().Extend(pred1.UnitPosition.To2D(), E.Range)
                : new Vector2();
        }

        private static float Qdamage(Obj_AI_Base target)
        {
            return (float)Player.CalcDamage(target, Damage.DamageType.Magical,
                (new double[] { 50, 95, 140, 185, 230 }[Q.Level - 1]
                    + 0.6 * Player.FlatMagicDamageMod)
                * ((Q.Level == 5 && target is AIHeroClient) ? 1.15 : 1));
        }
        private static float Wdamage(Obj_AI_Base target)
        {
            return (float)Player.CalcDamage(target, Damage.DamageType.Magical,
                (new double[] { 80, 120, 160, 200, 240 }[W.Level - 1]
                                    + 0.7 * Player.FlatMagicDamageMod));
        }
        private static float Edamage(Obj_AI_Base target)
        {
            return (float)Player.CalcDamage(target, Damage.DamageType.Magical,
                (new double[] { 70, 115, 160, 205, 250 }[E.Level - 1]
                                    + 0.4 * Player.FlatMagicDamageMod));
        }
        private static float Rdamage(Obj_AI_Base target)
        {
            return (float)Player.CalcDamage(target, Damage.DamageType.Magical,
                                    new double[] { 90, 135, 180 }[R.Level - 1]
                                    + 0.2 * Player.FlatMagicDamageMod);
        }

        private static float SyndraHalfDamage(AIHeroClient target)
        {
            float x = 0;
            if (Player.Mana > Q.Instance.SData.Mana)
            {
                if (Q.IsReady()) x += Qdamage(target);
                if (Player.Mana > Q.Instance.SData.Mana)
                {
                    if (Player.Mana > Q.Instance.SData.Mana + E.Instance.SData.Mana)
                    {
                        if (E.IsReady()) x += Edamage(target);
                        if (Player.Mana > Q.Instance.SData.Mana + E.Instance.SData.Mana + W.Instance.SData.Mana)
                            if (W.IsReady()) x += Wdamage(target);
                    }
                }

            }
            if (LudensEcho.IsReady())
            {
                x = x + (float)Player.CalcDamage(target, Damage.DamageType.Magical, 100 + 0.1 * Player.FlatMagicDamageMod);
            }
            return x;
        }
        private static float SyndraDamage(AIHeroClient target)
        {
            float x = 0;
            if (Player.Mana > Q.Instance.SData.Mana)
            {
                if (Q.IsReady()) x += Qdamage(target);
                if (Player.Mana > Q.Instance.SData.Mana + R.Instance.SData.Mana)
                {
                    if (R.IsReady()) x += Rdamage(target) * (SyndraOrb.Count + 1);
                    if (Player.Mana > Q.Instance.SData.Mana + R.Instance.SData.Mana + E.Instance.SData.Mana)
                    {
                        if (E.IsReady()) x += Edamage(target);
                        if (Player.Mana > Q.Instance.SData.Mana + R.Instance.SData.Mana + E.Instance.SData.Mana + W.Instance.SData.Mana)
                            if (W.IsReady()) x += Wdamage(target);
                    }
                }

            }
            if (LudensEcho.IsReady())
            {
                x = x + (float)Player.CalcDamage(target, Damage.DamageType.Magical, 100 + 0.1 * Player.FlatMagicDamageMod);
            }
            x = x + (float)Player.GetAutoAttackDamage(target, true);
            return x;
        }
        public Syndra()
        {
            Q = new Spell(SpellSlot.Q, 950);
            W = new Spell(SpellSlot.W, 1150);
            E = new Spell(SpellSlot.E, 700); //1100
            R = new Spell(SpellSlot.R, 675);
            Q.SetSkillshot(0.5f, 10, float.MaxValue, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.25f, 10, 1450, false, SkillshotType.SkillshotLine, Player.Position, Player.Position);
            E.SetSkillshot(0.5f, 10, 1600, false, SkillshotType.SkillshotLine);
            Q.DamageType = W.DamageType = E.DamageType = TargetSelector.DamageType.Magical;
            Q.MinHitChance = HitChance.Medium;
            W.MinHitChance = HitChance.Medium;

            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            MainMenu.AddSubMenu(orbwalkerMenu);

            Menu ts = MainMenu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);

            Menu Combo = new Menu("Combo", "Combo");
            {
                Bool(Combo, "Qc", "Q", true);
                Bool(Combo, "Wc", "W", true);
                Bool(Combo, "Ec", "E", true);
                Bool(Combo, "QEc", "QE", true);
                Bool(Combo, "Rc", "R", true);
                Separator(Combo,"Rbc","cast R target:");
                foreach (var hero in HeroManager.Enemies)
                {
                    Bool(Combo, hero.ChampionName + "c", hero.ChampionName, true);
                }
                MainMenu.AddSubMenu(Combo);
            }
            Menu Harass = new Menu("Harass", "Harass");
            {
                Bool(Harass, "Qh", "Q", true);
                Bool(Harass, "Wh", "W", true);
                Bool(Harass, "Eh", "E", true);
                Slider(Harass, "manah", "Min mana", 40, 0, 100);
                MainMenu.AddSubMenu(Harass);
            }
            Menu Auto = new Menu("Auto", "Auto");
            {
                Bool(Auto, "Qa", "Q on target AA + spellcast ", true);
                Bool(Auto, "GapIntera", "Anti-Gap & Interrupt", true);
                Bool(Auto, "killsteala", "KillSteal ", true);
                MainMenu.AddSubMenu(Auto);
            }
            Menu Helper = new Menu("Helper", "Helper");
            {
                Bool(Helper, "enableh", "Enabale", true);
                KeyBind(Helper, "QEh", "QE to mouse", 'G', KeyBindType.Press);
                MainMenu.AddSubMenu(Helper);
            }
            Menu drawMenu = new Menu("Draw", "Draw");
            {
                Bool(drawMenu, "Qd", "Q");
                Bool(drawMenu, "Wd", "W");
                Bool(drawMenu, "Ed", "E");
                Bool(drawMenu, "QEd", "QE");
                Bool(drawMenu, "Rd", "R");
                Bool(drawMenu, "Hpd", "Damage Indicator").ValueChanged+= Syndra_ValueChanged;
                MainMenu.AddSubMenu(drawMenu);
            }


            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            AntiGapcloser.OnEnemyGapcloser += Gapcloser_OnGapCloser;
            Interrupter2.OnInterruptableTarget += InterruptableSpell_OnInterruptableTarget;
            //Orb.OnAction += Orbwalker_OnAction;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = SyndraDamage;
            CustomDamageIndicator.Initialize(SyndraDamage);
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = drawhp;
            CustomDamageIndicator.Enabled = drawhp;
            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
        }
        private static bool castRtarget (AIHeroClient target)
        {
            return MainMenu.Item(target.ChampionName + "c").GetValue<bool>();
        }
        private static bool comboq { get { return MainMenu.Item("Qc").GetValue<bool>(); } }
        private static bool combow { get { return MainMenu.Item("Wc").GetValue<bool>(); } }
        private static bool comboe { get { return MainMenu.Item("Ec").GetValue<bool>(); } }
        private static bool comboqe { get { return MainMenu.Item("QEc").GetValue<bool>(); } }
        private static bool combor { get { return MainMenu.Item("Rc").GetValue<bool>(); } }
        private static bool harassE { get { return MainMenu.Item("Eh").GetValue<bool>(); } }
        private static bool harassq { get { return MainMenu.Item("Qh").GetValue<bool>(); } }
        private static bool harassw { get { return MainMenu.Item("Wh").GetValue<bool>(); } }
        private static bool autoq { get { return MainMenu.Item("Qa").GetValue<bool>(); } }
        private static bool autogapinter { get { return MainMenu.Item("GapIntera").GetValue<bool>(); } }
        private static bool autokillsteal { get { return MainMenu.Item("killsteala").GetValue<bool>(); } }
        private static bool helperenable { get { return MainMenu.Item("enableh").GetValue<bool>(); } }
        private static bool helperqe { get { return MainMenu.Item("QEh").GetValue<KeyBind>().Active; } }
        private static bool drawq { get { return MainMenu.Item("Qd").GetValue<bool>(); } }
        private static bool draww { get { return MainMenu.Item("Wd").GetValue<bool>(); } }
        private static bool drawe { get { return MainMenu.Item("Ed").GetValue<bool>(); } }
        private static bool drawqe { get { return MainMenu.Item("QEd").GetValue<bool>(); } }
        private static bool drawr { get { return MainMenu.Item("Rd").GetValue<bool>(); } }
        private static bool drawhp { get { return MainMenu.Item("Hpd").GetValue<bool>(); } }
        private static int harassmana { get { return MainMenu.Item("manah").GetValue<Slider>().Value; } }
        private void Syndra_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (!Enable) return;
            if (sender != null)
            {
                LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = e.GetNewValue<bool>();
                CustomDamageIndicator.Enabled = e.GetNewValue<bool>();
            }
        }

        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name.ToLower().Contains("syndraq")) qcount = Utils.GameTimeTickCount;
                if (args.SData.Name.ToLower() == "syndraw") w1cast = Utils.GameTimeTickCount;
                if (args.SData.Name.ToLower().Contains("syndrawcast")) wcount = Utils.GameTimeTickCount;
                if (args.SData.Name.ToLower().Contains("syndrae")) ecount = Utils.GameTimeTickCount;
                spellcount = Math.Max(qcount, Math.Max(ecount, wcount));
            }
            if (!(Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed || autoq)) return;
            if (sender is AIHeroClient && sender.IsEnemy &&
                (args.SData.IsAutoAttack() || !args.SData.CanMoveWhileChanneling) &&
                sender.IsValidTarget(Q.Range))
            {
                if (Q.IsReady())
                    Q.Cast(Q.GetPrediction(sender).UnitPosition.To2D());
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Enable)
            {
                LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = false;
                CustomDamageIndicator.Enabled = false;
                return;
            }
            helper();
            killsteal();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                Combo();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && Player.ManaPercent >= harassmana)
                Harass();
        }
        private void helper()
        {
            if (!helperenable) return;
            if (!helperqe) return;
            if (Player.Mana <= Q.Instance.SData.Mana + E.Instance.SData.Mana || !(Q.IsReady() && E.IsReady())) return;
            Q.Cast(Player.Position.Extend(Game.CursorPos, E.Range - 200));
            LeagueSharp.Common.Utility.DelayAction.Add(250, () => E.Cast(Player.Position.Extend(Game.CursorPos, E.Range - 200)));
        }
        private void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!sender.IsMe) return;
            if (Player.Level == 16)
                R = new Spell(SpellSlot.R, 750);
        }
        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //Chat.Print(args.SData.Name);
            if (sender.IsMe)
            {
                if (args.SData.Name.ToLower().Contains("syndraq")) qcount = Utils.GameTimeTickCount;
                if (args.SData.Name.ToLower() == "syndraw") w1cast = Utils.GameTimeTickCount;
                if (args.SData.Name.ToLower().Contains("syndrawcast")) wcount = Utils.GameTimeTickCount;
                if (args.SData.Name.ToLower().Contains("syndrae")) ecount = Utils.GameTimeTickCount;
                spellcount = Math.Max(qcount, Math.Max(ecount, wcount));
            }
            if (!(Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed || autoq)) return;
            if (sender is AIHeroClient && sender.IsEnemy &&
                (args.SData.IsAutoAttack() || !args.SData.CanMoveWhileChanneling) &&
                sender.IsValidTarget(Q.Range))
            {
                if (Q.IsReady())
                    Q.Cast(sender);
            }
        }
        private void InterruptableSpell_OnInterruptableTarget(Obj_AI_Base Sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Enable) return;
            if (Sender.IsEnemy && E.IsReady() && autogapinter)
            {
                if (Sender.IsValidTarget(E.Range)) E.Cast(Sender.Position);
                if (StunAbleOrb.Any())
                {
                    var i = StunAbleOrb.First(x => x.Key.NetworkId == Sender.NetworkId);
                    if (i.Value != null)
                        E.Cast(i.Value.Position.To2D());
                }
            }
        }

        private void Gapcloser_OnGapCloser(ActiveGapcloser gapcloser)
        {
            if (!Enable) return;
            if (gapcloser.Sender.IsEnemy && E.IsReady() && autogapinter)
            {
                if (gapcloser.Sender.IsValidTarget(E.Range)) E.Cast(gapcloser.Sender.Position);
                if (StunAbleOrb.Any())
                {
                    var i = StunAbleOrb.First(x => x.Key.NetworkId == gapcloser.Sender.NetworkId);
                    if (i.Value != null)
                        E.Cast(i.Value.Position.To2D());
                }
            }
        }
        private static void killsteal()
        {
            // killstealQ
            if (Q.IsReady() && Utils.GameTimeTickCount >= spellcount + 1000)
            {
                foreach (
                    var target in
                        HeroManager.Enemies.Where(
                            x => x.IsValidTarget(Q.Range) && !x.IsZombie && Qdamage(x) > x.Health))
                {
                    if (Q.Cast(target) == Spell.CastStates.SuccessfullyCasted)
                        spellcount = Utils.GameTimeTickCount;
                }
            }
            // killstealW
            if (W.IsReady() && Utils.GameTimeTickCount >= spellcount + 1000)
            {
                foreach (
                    var target in
                        HeroManager.Enemies.Where(
                            x => x.IsValidTarget(W.Range) && !x.IsZombie && Wdamage(x) > x.Health))
                {
                    if (W.Instance.Name == "SyndraW")
                    {
                        if (PickableOrb != null || PickableMinion != null)
                        {
                            W.Cast(PickableOrb != null
                                ? PickableOrb.Position.To2D()
                                : PickableMinion.Position.To2D());
                        }
                        LeagueSharp.Common.Utility.DelayAction.Add(500, () =>
                        {
                            W.UpdateSourcePosition(Wobject().Position);
                            W.Cast(target);
                        });
                        spellcount = Utils.GameTimeTickCount + 500;
                    }
                    else
                    {
                        if (Wobject() != null && Utils.GameTimeTickCount >= w1cast + 500)
                        {
                            W.UpdateSourcePosition(Wobject().Position);
                            if (W.Cast(target) == Spell.CastStates.SuccessfullyCasted)
                                spellcount = Utils.GameTimeTickCount;
                        }
                    }
                }
            }
            //killstealE
            if (E.IsReady() && Utils.GameTimeTickCount >= spellcount + 1000)
            {
                foreach (
                    var target in
                        HeroManager.Enemies.Where(
                            x => x.IsValidTarget(E.Range) && !x.IsZombie && Edamage(x) > x.Health))
                {
                    E.Cast(target.Position);
                    spellcount = Utils.GameTimeTickCount;
                }
            }
            //killstealQW
            if (Q.IsReady() && W.IsReady() && Utils.GameTimeTickCount >= spellcount + 1000)
            {
                foreach (
                    var target in
                        HeroManager.Enemies.Where(
                            x => x.IsValidTarget(Q.Range) && !x.IsZombie && Qdamage(x) + Wdamage(x) > x.Health))
                {
                    if (Q.Cast(target) == Spell.CastStates.SuccessfullyCasted)
                    {
                        if (W.Instance.Name == "SyndraW")
                        {
                            if (PickableOrb != null || PickableMinion != null)
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(250, () => W.Cast(PickableOrb != null
                                    ? PickableOrb.Position.To2D()
                                    : PickableMinion.Position.To2D()));
                            }
                            LeagueSharp.Common.Utility.DelayAction.Add(750, () =>
                            {
                                W.UpdateSourcePosition(Wobject().Position);
                                W.Cast(target);
                            });
                            spellcount = Utils.GameTimeTickCount + 750;
                        }
                        else
                        {
                            if (Wobject() != null && Utils.GameTimeTickCount >= w1cast + 500)
                            {
                                W.UpdateSourcePosition(Wobject().Position);
                                LeagueSharp.Common.Utility.DelayAction.Add(250, () => W.Cast(target));
                                spellcount = Utils.GameTimeTickCount + 250;
                            }
                        }
                    }
                }
            }
            //killstealR
            if (R.IsReady() && Utils.GameTimeTickCount >= spellcount + 1000)
            {
                foreach (
                    var target in
                        HeroManager.Enemies.Where(
                            x => castRtarget(x) && x.IsValidTarget(W.Range) && !x.IsZombie && Rdamage(x) * SyndraOrb.Count > x.Health))
                {
                    if (R.Cast(target) == Spell.CastStates.SuccessfullyCasted)
                        spellcount = Utils.GameTimeTickCount;
                }
            }

        }
        private static void Harass()
        {
            if (Utils.GameTimeTickCount > ecount)
            {

                if (Q.IsReady() && harassq)
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                    if (target.IsValidTarget() && !target.IsZombie)
                    {
                        if (Q.Cast(target) == Spell.CastStates.SuccessfullyCasted)
                            ecount = Utils.GameTimeTickCount + 100;
                    }
                }
                if (E.IsReady() && StunAbleOrb.Any() && Utils.GameTimeTickCount >= wcount + 500 && harassE)
                {
                    var targetE = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                    var Orb = StunAbleOrb.Any(x => x.Key == targetE)
                        ? StunAbleOrb.First(x => x.Key == targetE).Value
                        : StunAbleOrb.First().Value;
                    if (Orb != null)
                    {
                        if (E.Cast(Orb.Position.To2D()))
                            ecount = Utils.GameTimeTickCount + 100;
                    }
                }
                if (W.Instance.Name != "SyndraW" && harassw)
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                    if (target.IsValidTarget() && !target.IsZombie)
                    {
                        if (Wobject() != null && Utils.GameTimeTickCount >= w1cast + 250)
                        {
                            W.UpdateSourcePosition(Wobject().Position, Player.Position);
                            W.Cast(target);
                        }
                    }
                }
                if (W.IsReady() && Utils.GameTimeTickCount >= ecount + 500 && harassw)
                {
                    var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                    if (target.IsValidTarget() && !target.IsZombie)
                    {
                        if (W.Instance.Name != "SyndraW")
                        {
                            if (Wobject() != null && Utils.GameTimeTickCount >= w1cast + 250)
                            {
                                W.UpdateSourcePosition(Wobject().Position, Player.Position);
                                W.Cast(target);
                            }
                        }
                        else
                        {

                            if (PickableOrb != null || PickableMinion != null)
                            {
                                if (W.Cast(PickableOrb != null
                                    ? PickableOrb.Position.To2D()
                                    : PickableMinion.Position.To2D()))
                                {
                                    wcount = Utils.GameTimeTickCount + 100;
                                    ecount = Utils.GameTimeTickCount + 100;
                                }
                            }
                        }
                    }
                }

            }
        }
        private static void Combo()
        {
            // Use R
            if (R.IsReady() && combor)
            {
                foreach (
                    var target in
                        HeroManager.Enemies.Where(
                            x =>
                                castRtarget(x) && x.IsValidTarget(W.Range) && !x.IsZombie && SyndraHalfDamage(x) < x.Health &&
                                SyndraDamage(x) > x.Health))
                {
                    R.Cast(target);
                }

            }

            // final cases;
            //else 
            if (Utils.GameTimeTickCount > ecount)
            {
                {
                    if (R.IsReady() && E.IsReady() && combor && comboe)
                    {
                        var target =
                            HeroManager.Enemies.Where(x => castRtarget(x) && x.IsValidTarget() && !x.IsZombie)
                                .OrderByDescending(x => x.Distance(Player.Position))
                                .LastOrDefault();
                        if (target.IsValidTarget(R.Range) && !target.IsZombie)
                        {
                            var count = target.CountEnemiesInRange(400);
                            if (count >= 3)
                            {
                                R.Cast(target);
                                Q.Cast(target);
                                LeagueSharp.Common.Utility.DelayAction.Add(500, () => E.Cast(target.Position));
                                ecount = Utils.GameTimeTickCount + 510;
                                return;
                            }
                        }
                    }
                }
                {
                    if (Q.IsReady() && comboq)
                    {
                        var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                        if (target.IsValidTarget() && !target.IsZombie)
                        {
                            var x = Q.GetPrediction(target).CastPosition;
                            if (Q.Cast(target) == Spell.CastStates.SuccessfullyCasted && E.IsReady()
                                && x.Distance(Player.Position) <= E.Range - 100 && comboe)
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(250, () => E.Cast(x));
                                ecount = Utils.GameTimeTickCount + 350;
                            }
                        }
                    }
                    if (E.IsReady() && StunAbleOrb.Any() && Utils.GameTimeTickCount >= wcount + 500 && comboe)
                    {
                        var targetE = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
                        var Orb = StunAbleOrb.Any(x => x.Key == targetE)
                            ? StunAbleOrb.First(x => x.Key == targetE).Value
                            : StunAbleOrb.First().Value;
                        if (Orb != null)
                        {
                            if(E.Cast(Orb.Position.To2D()))
                                ecount = Utils.GameTimeTickCount + 100;
                        }
                    }
                    if (W.Instance.Name != "SyndraW" && combow)
                    {
                        var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                        if (target.IsValidTarget() && !target.IsZombie)
                        {
                            if (Wobject() != null && Utils.GameTimeTickCount >= w1cast + 250)
                            {
                                W.UpdateSourcePosition(Wobject().Position, Player.Position);
                                W.Cast(target);
                            }
                        }
                    }
                    if (W.IsReady() && Utils.GameTimeTickCount >= ecount + 500 && combow)
                    {
                        var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                        if (target.IsValidTarget() && !target.IsZombie)
                        {
                            if (W.Instance.Name != "SyndraW")
                            {
                                if (Wobject() != null && Utils.GameTimeTickCount >= w1cast + 250)
                                {
                                    W.UpdateSourcePosition(Wobject().Position, Player.Position);
                                    W.Cast(target);
                                }
                            }
                            else
                            {

                                if (PickableOrb != null || PickableMinion != null)
                                {
                                    W.Cast(PickableOrb != null
                                        ? PickableOrb.Position.To2D()
                                        : PickableMinion.Position.To2D());
                                    wcount = Utils.GameTimeTickCount + 100;
                                    ecount = Utils.GameTimeTickCount + 100;
                                }
                            }
                        }
                    }

                    if (Utils.GameTimeTickCount > ecount && E.IsReady() && Q.IsReady() &&
                        Utils.GameTimeTickCount >= wcount + 500 && comboqe &&
                        Player.Mana >= E.Instance.SData.Mana + Q.Instance.SData.Mana)
                    {
                        var target =
                            HeroManager.Enemies.FirstOrDefault(
                                x => x.IsValidTarget() && !x.IsZombie && CanEQtarget(x));
                        if (target.IsValidTarget() && !target.IsZombie)
                        {
                            var pos = PositionEQtarget(target);
                            if (pos.IsValid())
                            {
                                if (Q.Cast(pos))
                                {
                                    if (pos.Distance(Player.Position.To2D()) < E.Range - 200)
                                    {
                                        LeagueSharp.Common.Utility.DelayAction.Add(250, () => E.Cast(pos));
                                        ecount = Utils.GameTimeTickCount + 350;
                                    }
                                    else
                                    {
                                        LeagueSharp.Common.Utility.DelayAction.Add(150, () => E.Cast(pos));
                                        ecount = Utils.GameTimeTickCount + 250;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void OnDelete(GameObject sender, EventArgs args)
        {
            if (!Enable) return;
            //if (sender.Name.Contains("idle"))
            //Chat.Print(sender.Name + " " + sender.Type);
            if (sender.Name.Contains("Syndra_Base_Q_idle.troy") || sender.Name.Contains("Syndra_Base_Q_Lv5_idle.troy"))
            {
                if (seed.Any(x => x.Position.To2D().Distance(sender.Position.To2D()) <= 20))
                {
                    //foreach (var x in SyndraOrb.Where(x => x.Key == sender.NetworkId))
                    //{
                    //    SyndraOrb.Remove(x);
                    //}
                    SyndraOrb.RemoveAll(x => x.Key == sender.NetworkId);
                }
            }
        }

        private void OnCreate(GameObject sender, EventArgs args)
        {
            if (!Enable) return;
            //Chat.Print(sender.Name + " " + sender.Type);
            if (sender.Name.Contains("Syndra_Base_Q_idle.troy") || sender.Name.Contains("Syndra_Base_Q_Lv5_idle.troy"))
            {
                if (seed.Any(x => x.Position.To2D().Distance(sender.Position.To2D()) <= 20))
                {
                    SyndraOrb.Add(new SyndraOrbs(sender.NetworkId, sender));
                }
            }
        }
        private void OnDraw(EventArgs args)
        {
            if (!Enable) return;
            //if (SyndraOrb.Any())
            //    foreach (var z in SyndraOrb)
            //    {
            //        Drawing.DrawCircle(z.Value.Position, 100, Color.Red);
            //    }
            //foreach (var y in x)
            //{
            //    Drawing.DrawCircle(y.Position, 200, Color.Red);
            //}
            //foreach (var obj in GameObjects.AllGameObjects)
            //{
            //    if (obj.Name.Contains("Syndra_Base_W") && obj.Name.Contains("held") && obj.Name.Contains("02"))
            //        Drawing.DrawCircle(obj.Position, 200, Color.Red);
            //}
            if (Player.IsDead)
                return;
            if (drawq)
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Aqua);
            if (draww)
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Aqua);
            if (drawe)
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Aqua);
            if (drawqe)
                Render.Circle.DrawCircle(Player.Position, 1100, Color.Aqua);
            if (drawr)
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Aqua);
        }
    }
}
