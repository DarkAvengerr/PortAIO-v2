using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;
using BadaoSeries.CustomOrbwalker;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoSeries.Plugin
{
    internal class TwistedFate : AddUI
    {
        private static int cardtick, yellowtick;
        public static bool helpergold, helperblue, helperred;
        private static bool isobvious { get { return Utils.GameTimeTickCount - cardtick <= 500; } }
        private static bool IsPickingCard { get { return Player.HasBuff("pickacard_tracker"); } }
        private static bool CanUseR2 { get { return R.IsReady() && Player.HasBuff("destiny_marker"); } }
        private static bool CanUseR1 { get { return R.IsReady() && !Player.HasBuff("destiny_marker"); } }
        private static bool PickACard { get { return W.Instance.Name == "PickACard"; } }
        private static bool GoldCard { get { return W.Instance.Name == "goldcardlock"; } }
        private static bool BlueCard { get { return W.Instance.Name == "bluecardlock"; } }
        private static bool RedCard { get { return W.Instance.Name == "redcardlock"; } }
        private static bool HasBlue { get { return Player.HasBuff("bluecardpreattack"); } }
        private static bool HasRed { get { return Player.HasBuff("redcardpreattack"); } }
        private static bool HasGold { get { return Player.HasBuff("goldcardpreattack"); } }
        private static string HasACard
        {
            get
            {
                if (Player.HasBuff("bluecardpreattack"))
                    return "blue";
                if (Player.HasBuff("goldcardpreattack"))
                    return "gold";
                if (Player.HasBuff("redcardpreattack"))
                    return "red";
                return "none";
            }
        }
        public TwistedFate()
        {
            Q = new Spell(SpellSlot.Q, 1400);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);
            Q.SetSkillshot(0.25f, 40, 1000, false, SkillshotType.SkillshotLine);
            Q.DamageType = W.DamageType = E.DamageType = TargetSelector.DamageType.Magical;
            Q.MinHitChance = HitChance.High;

            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            MainMenu.AddSubMenu(orbwalkerMenu);
            
            Menu ts = MainMenu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);

            Menu Combo = new Menu("Combo", "Combo");
            {
                Bool(Combo, "Qc", "Q", true);
                Bool(Combo, "Qafterattackc", "Q after attack", true);
                Bool(Combo, "Qimmobilec", "Q on immobile", true);
                Slider(Combo, "Qhitc", "Q if will hit", 2, 1, 3);
                Bool(Combo, "Wc", "W", true);
                Bool(Combo, "pickgoldc", "Pick gold card while using R", true);
                Bool(Combo, "dontpickyellow1stc", "don't pick gold at 1st turn", false);
                MainMenu.AddSubMenu(Combo);
            }
            Menu Harass = new Menu("Harass", "Harass");
            {
                Bool(Harass, "Qh", "Q", true);
                Bool(Harass, "Qafterattackh", "Q after attack", true);
                Bool(Harass, "Qimmobileh", "Q on immobile", true);
                Slider(Harass, "Qhith", "Q if will hit", 2, 1, 3);
                Bool(Harass, "Wh", "W", true);
                List(Harass, "Wcolorh", "W card type", new[] { "blue", "red", "gold" });
                Slider(Harass, "manah", "Min mana", 40, 0, 100);
                MainMenu.AddSubMenu(Harass);
            }
            Menu Clear = new Menu("Clear", "Clear");
            {
                Bool(Clear, "Qj", "Q", true);
                Slider(Clear, "Qhitj", "Q if will hit", 2, 1, 3);
                Bool(Clear, "Wj", "W", true);
                List(Clear, "Wcolorj", "W card type", new[] { "blue", "red" });
                Slider(Clear, "wmanaj", "mana only W blue", 0, 0, 100);
                Slider(Clear, "manaj", "Min mana", 40, 0, 100);
                MainMenu.AddSubMenu(Clear);
            }
            Menu Auto = new Menu("Auto", "Auto");
            {
                Bool(Auto, "throwyellowa", "gapclose + interrupt: throw gold card", true);
                Bool(Auto, "killsteala", "KillSteal Q", true);
                MainMenu.AddSubMenu(Auto);
            }
            Menu Helper = new Menu("Helper", "Pick card Helper");
            {
                Bool(Helper, "enableh", "Enabale", true);
                KeyBind(Helper, "pickyellowh", "Pick Yellow", 'W', KeyBindType.Toggle);
                KeyBind(Helper, "pickblueh", "Pick Blue", 'G', KeyBindType.Toggle);
                KeyBind(Helper, "pickredh", "Pick Red", 'H', KeyBindType.Toggle);
                MainMenu.AddSubMenu(Helper);
            }
            Menu drawMenu = new Menu("Draw", "Draw");
            {
                Bool(drawMenu, "Qd", "Q");
                Bool(drawMenu, "Rd", "R");
                Bool(drawMenu, "Hpd", "Damage Indicator").ValueChanged += TwistedFate_ValueChanged;
                MainMenu.AddSubMenu(drawMenu);
            }
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            //Obj_AI_Base.OnSpellCast += OnProcessSpellCast;
            //GameObject.OnCreate += OnCreate;
            AntiGapcloser.OnEnemyGapcloser += Gapcloser_OnGapCloser;
            Interrupter2.OnInterruptableTarget += InterruptableSpell_OnInterruptableTarget;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Orbwalking.OnAttack += Orbwalking_OnAttack;
            //LeagueSharp.Common.Utility.HpBar//DamageIndicator.DamageToUnit = TwistedFateDamage;
            //LeagueSharp.Common.Utility.HpBar//DamageIndicator.Enabled = drawhp;
            //Custom//DamageIndicator.Initialize(TwistedFateDamage);
            //Custom//DamageIndicator.Enabled = drawhp;
        }


        private void InterruptableSpell_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Enable)
                return;
            if (sender.IsEnemy && Orbwalking.InAutoAttackRange(sender))
            {
                if (HasGold)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, sender);
                }
            }
        }

        private void Gapcloser_OnGapCloser(ActiveGapcloser gapcloser)
        {
            if (!Enable)
                return;
            if (gapcloser.Sender.IsEnemy && Orbwalking.InAutoAttackRange(gapcloser.Sender))
            {
                if (HasGold)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, gapcloser.Sender);
                }
            }
        }

        private void TwistedFate_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (!Enable) return;
            if (sender != null)
            {
                //LeagueSharp.Common.Utility.HpBar//DamageIndicator.Enabled = e.GetNewValue<bool>();
                //Custom//DamageIndicator.Enabled = e.GetNewValue<bool>();
            }
        }

        private static bool dontbeobvious { get { return MainMenu.Item("dontpickyellow1stc").GetValue<bool>(); } }
        private static bool comboq { get { return MainMenu.Item("Qc").GetValue<bool>(); } }
        private static bool comboqafterattack { get { return  MainMenu.Item("Qafterattackc").GetValue<bool>(); } }
        private static bool comboqimmobile { get { return  MainMenu.Item("Qimmobilec").GetValue<bool>(); } }
        private static int comboqhit { get { return  MainMenu.Item("Qhitc").GetValue<Slider>().Value; } }
        private static bool combow { get { return  MainMenu.Item("Wc").GetValue<bool>(); } }
        private static bool combopickgold { get { return  MainMenu.Item("pickgoldc").GetValue<bool>(); } }
        private static bool harassq { get { return  MainMenu.Item("Qh").GetValue<bool>(); } }
        private static bool harassqafterattack { get { return  MainMenu.Item("Qafterattackh").GetValue<bool>(); } }
        private static bool harassqimmobile { get { return  MainMenu.Item("Qimmobileh").GetValue<bool>(); } }
        private static int harassqhit { get { return  MainMenu.Item("Qhith").GetValue<Slider>().Value; } }
        private static bool harassw { get { return  MainMenu.Item("Wh").GetValue<bool>(); } }
        private static int harasswcolor { get { return  MainMenu.Item("Wcolorh").GetValue<StringList>().SelectedIndex; } }
        private static int harassmana { get { return  MainMenu.Item("manah").GetValue<Slider>().Value; } }
        private static bool clearq { get { return  MainMenu.Item("Qj").GetValue<bool>(); } }
        private static int clearqhit { get { return  MainMenu.Item("Qhitj").GetValue<Slider>().Value; } }
        private static bool clearw { get { return  MainMenu.Item("Wj").GetValue<bool>(); } }
        private static int clearwcolor { get { return  MainMenu.Item("Wcolorj").GetValue<StringList>().SelectedIndex; } }
        private static int clearwmana { get { return  MainMenu.Item("wmanaj").GetValue<Slider>().Value; } }
        private static int clearmana { get { return  MainMenu.Item("manaj").GetValue<Slider>().Value; } }
        private static bool autothrowyellow { get { return  MainMenu.Item("throwyellowa").GetValue<bool>(); } }
        private static bool autokillsteal { get { return  MainMenu.Item("killsteala").GetValue<bool>(); } }
        private static bool helperenable { get { return  MainMenu.Item("enableh").GetValue<bool>(); } }
        private static bool helperpickyellow
        {
            get { return  MainMenu.Item("pickyellowh").GetValue<KeyBind>().Active; }
            set
            {
                var key = MainMenu.Item("pickyellowh").GetValue<KeyBind>().Key;
                var type = MainMenu.Item("pickyellowh").GetValue<KeyBind>().Type;
                MainMenu.Item("pickyellowh").SetValue<KeyBind>(new KeyBind(key, KeyBindType.Toggle, value));
            }
        }
        private static bool helperpickblue
        {
            get { return  MainMenu.Item("pickblueh").GetValue<KeyBind>().Active; }
            set
            {
                var key = MainMenu.Item("pickblueh").GetValue<KeyBind>().Key;
                var type = MainMenu.Item("pickblueh").GetValue<KeyBind>().Type;
                MainMenu.Item("pickblueh").SetValue<KeyBind>(new KeyBind(key, KeyBindType.Toggle, value));
            }
        }
        private static bool helperpickred
        {
            get { return  MainMenu.Item("pickredh").GetValue<KeyBind>().Active; }
            set
            {
                var key = MainMenu.Item("pickredh").GetValue<KeyBind>().Key;
                var type = MainMenu.Item("pickredh").GetValue<KeyBind>().Type;
                MainMenu.Item("pickredh").SetValue<KeyBind>(new KeyBind(key, KeyBindType.Toggle, value));
            }
        }
        private static bool drawq { get { return  MainMenu.Item("Qd").GetValue<bool>(); } }
        private static bool drawr { get { return  MainMenu.Item("Rd").GetValue<bool>(); } }
        private static bool drawhp { get { return  MainMenu.Item("Hpd").GetValue<bool>(); } }
        private static void AutoHelper()
        {
            if (autokillsteal && Q.IsReady())
            {
                foreach (var x in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && Player.GetSpellDamage(x, SpellSlot.Q) > x.Health))
                {
                    Q.Cast(x);
                }
            }
            if (helperenable)
            {
                if (helperpickblue || helperpickred || helperpickyellow)
                {
                    //EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo,Game.CursorPos);
                    if (!IsPickingCard && PickACard && Utils.GameTimeTickCount - cardtick >= 500)
                    {
                        cardtick = Utils.GameTimeTickCount;
                        W.Cast();
                    }
                    if (helperpickyellow && GoldCard) W.Cast();
                    if (helperpickblue && BlueCard) W.Cast();
                    if (helperpickred && RedCard) W.Cast();
                }
                if (HasGold)
                    helperpickyellow = false;
                if (HasBlue)
                    helperpickblue = false;
                if (HasRed)
                    helperpickred = false;
            }
            if (combow && Player.HasBuff("destiny_marker") && combopickgold && W.IsReady())
            {
                if (!IsPickingCard && PickACard && Utils.GameTimeTickCount - cardtick >= 500)
                {
                    cardtick = Utils.GameTimeTickCount;
                    W.Cast();
                }
                if (IsPickingCard && GoldCard)
                    W.Cast();
            }
        }
        private void Drawing_OnEndScene(EventArgs args)
        {
            if (!Enable)
                return;
            if (Player.IsDead)
                return;
            if (drawr)
            {
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, 5500, Color.Aqua,5,30,true);
            }
        }
        private void OnDraw(EventArgs args)
        {
            if (!Enable)
                return;
            if (Player.IsDead)
                return;
            if (drawq)
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Aqua);
        }
        private void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!Enable)
                return;
            var mode = new Orbwalking.OrbwalkingMode[] { Orbwalking.OrbwalkingMode.Mixed, Orbwalking.OrbwalkingMode.Combo };
            if (IsPickingCard && mode.Contains(Orbwalker.ActiveMode)) args.Process = false;
            else if (HasACard != "none" && !HeroManager.Enemies.Contains(args.Target) && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                args.Process = false;
                var target = TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(Player), TargetSelector.DamageType.Magical);
                if (target.IsValidTarget() && !target.IsZombie)
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }
            else if (HasACard != "none" && HasRed && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                args.Process = false;
                IDictionary<Obj_AI_Minion, int> creeps = new Dictionary<Obj_AI_Minion, int>();
                foreach (var x in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Team != Player.Team && x.Team != GameObjectTeam.Neutral  && Orbwalking.InAutoAttackRange(x)))
                {
                    creeps.Add(x, ObjectManager.Get<Obj_AI_Minion>().Count(y => y.Team != Player.Team && y.Team != GameObjectTeam.Neutral && y.IsValidTarget() && y.Distance(x.Position) <= 300));
                }
                foreach (var x in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Team == GameObjectTeam.Neutral && Orbwalking.InAutoAttackRange(x)))
                {
                    creeps.Add(x, ObjectManager.Get<Obj_AI_Minion>().Count(y => y.Team == GameObjectTeam.Neutral && y.IsValidTarget() && y.Distance(x.Position) <= 300));
                }
                var minion = creeps.OrderByDescending(x => x.Value).FirstOrDefault();
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion.Key);
            }
        }
        private void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!Enable)
                return;
            if (target.IsValidTarget() && (target as AIHeroClient) != null && (target as AIHeroClient).IsValid)
            {
                if (Utils.GameTimeTickCount - yellowtick <= 1500)
                    return;
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && comboqafterattack)
                {
                    if (target.IsValidTarget() && !target.IsZombie)
                    {
                        var Target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                        if (Target.IsValidTarget() && !Target.IsZombie)
                            Q.Cast(Q.GetPrediction(Target).CastPosition);
                    }
                }
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && harassqafterattack && Player.ManaPercent >= harassmana)
                {
                    if (target.IsValidTarget() && !target.IsZombie)
                    {
                        var Target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                        if (Target.IsValidTarget() && !Target.IsZombie)
                            Q.Cast(Q.GetPrediction(Target).CastPosition);
                    }
                }
            }

        }
        private void Orbwalking_OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!Enable)
                return;
            if (HasGold)
                yellowtick = Utils.GameTimeTickCount;
        }
        private void OnUpdate(EventArgs args)
        {
            if (!Enable)
            {
                //LeagueSharp.Common.Utility.HpBar//DamageIndicator.Enabled = false;
                //Custom//DamageIndicator.Enabled = false;
                return;
            }
            AutoHelper();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                Combo();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && Player.ManaPercent >= harassmana)
                Harass();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                Clear();
        }
        private static void Combo()
        {
            if (Q.IsReady() && comboq)
            {
                if (comboqimmobile)
                {
                    foreach (var x in HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie))
                        Q.CastIfHitchanceEquals(x, HitChance.Immobile);
                }
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                    if (target.IsValidTarget() && !target.IsZombie)
                        Q.CastIfWillHit(target, comboqhit);
                }
            }
            if (combow && W.IsReady())
            {
                var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    if (!IsPickingCard && PickACard && Utils.GameTimeTickCount - cardtick >= 500)
                    {
                        cardtick = Utils.GameTimeTickCount;
                        W.Cast();
                    }
                    if (IsPickingCard)
                    {
                        if (Player.Mana >= Q.Instance.SData.Mana)
                        {
                            if (GoldCard && !(dontbeobvious && isobvious))
                                W.Cast();
                        }
                        else if (HeroManager.Allies.Any(x => x.IsValidTarget(800, false)))
                        {
                            if (GoldCard && !(dontbeobvious && isobvious))
                                W.Cast();
                        }
                        else if (BlueCard)
                        {
                            W.Cast();
                        }
                    }
                }
            }
        }
        private static void Harass()
        {
            if (Q.IsReady() && harassq && Player.ManaPercent >= clearmana)
            {
                if (harassqimmobile)
                {
                    foreach (var x in HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie))
                        Q.CastIfHitchanceEquals(x, HitChance.Immobile);
                }
                {
                    var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                    Q.CastIfWillHit(target, harassqhit);
                }
            }
            if (harassw && W.IsReady())
            {
                var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    if (!IsPickingCard && PickACard && Utils.GameTimeTickCount - cardtick >= 500 && Player.ManaPercent >= clearmana)
                    {
                        cardtick = Utils.GameTimeTickCount;
                        W.Cast();
                    }
                    if (IsPickingCard)
                    {
                        switch (harasswcolor)
                        {
                            case 0:
                                if (BlueCard)
                                    W.Cast();
                                break;
                            case 1:
                                if (RedCard)
                                    W.Cast();
                                break;
                            case 2:
                                if (GoldCard)
                                    W.Cast();
                                break;
                        }
                    }
                }
            }
        }
        private static void Clear()
        {
            if (Q.IsReady() && clearq && Player.ManaPercent >= clearmana)
            {
                var farm = Q.GetLineFarmLocation(MinionManager.GetMinions(Q.Range, MinionTypes.All));
                if (farm.MinionsHit >= clearqhit)
                    Q.Cast(farm.Position);
            }
            if (W.IsReady() && clearw)
            {
                var target = ObjectManager.Get<Obj_AI_Minion>().Where(x => x.Team != Player.Team && x.IsValidTarget() && Orbwalking.InAutoAttackRange(x));
                if (target.Any())
                {
                    if (!IsPickingCard && PickACard && Utils.GameTimeTickCount - cardtick >= 500 && Player.ManaPercent >= clearmana)
                    {
                        cardtick = Utils.GameTimeTickCount;
                        W.Cast();
                    }
                    if (IsPickingCard)
                    {
                        if (clearwmana > Player.Mana * 100 / Player.MaxMana)
                        {
                            if (BlueCard)
                                W.Cast();
                        }
                        else
                        {
                            switch (clearwcolor)
                            {
                                case 0:
                                    if (BlueCard)
                                        W.Cast();
                                    break;
                                case 1:
                                    if (RedCard)
                                        W.Cast();
                                    break;
                            }
                        }
                    }
                }
            }
        }
        private static float TwistedFateDamage(AIHeroClient target)
        {
            var Qdamage = (float)Player.GetSpellDamage(target, Q.Slot);
            var Wdamage = (float)Player.GetSpellDamage(target, W.Slot);
            float x = 0;
            if ((W.IsReady() || HasACard != "none") && Q.IsReady())
            {
                if ((Player.Mana >= Q.Instance.SData.Mana + W.Instance.SData.Mana) || (Player.Mana >= Q.Instance.SData.Mana && HasACard != "none"))
                {
                    x = x + Qdamage + Wdamage;
                }
                else if (Player.Mana >= Q.Instance.SData.Mana)
                {
                    x = x + Qdamage;
                }
                else if (Player.Mana >= W.Instance.SData.Mana || HasACard != "none")
                {
                    x = x + Wdamage;
                }
            }
            else if (Q.IsReady())
            {
                x = x + Qdamage;
            }
            else if (W.IsReady() || HasACard != "none")
            {
                x = x + Wdamage;
            }
            if (LichBane.IsReady())
            {
                x = x + (float)Player.CalcDamage(target, Damage.DamageType.Magical, 0.75 * Player.BaseAttackDamage + 0.5 * Player.FlatMagicDamageMod);
            }
            else if (TrinityForce.IsReady())
            {
                x = x + (float)Player.CalcDamage(target, Damage.DamageType.Magical, 2 * Player.BaseAttackDamage);
            }
            else if (IcebornGauntlet.IsReady())
            {
                x = x + (float)Player.CalcDamage(target, Damage.DamageType.Magical, 1.25 * Player.BaseAttackDamage);
            }
            else if (Sheen.IsReady())
            {
                x = x + (float)Player.CalcDamage(target, Damage.DamageType.Magical, 1 * Player.BaseAttackDamage);
            }
            if (LudensEcho.IsReady())
            {
                x = x + (float)Player.CalcDamage(target, Damage.DamageType.Magical, 100 + 0.1 * Player.FlatMagicDamageMod);
            }
            x = x + (float)Player.GetAutoAttackDamage(target, true);
            return x;
        }
        private static void checkbuff()
        {
            var temp = Player.Buffs.Aggregate("", (current, buff) => current + ("( " + buff.Name + " , " + buff.Count + " )"));
            if (temp != null)
            Chat.Print(temp);
        }
    }
}
