using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp;
using ItemData = LeagueSharp.Common.Data.ItemData;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeRiven
{
    public class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static string R1name = "RivenFengShuiEngine";

        private static string R2name = "RivenIzunaBlade";

        private static Orbwalking.Orbwalker Orbwalker;

        private static Spell Q, W, E, R;

        private static SpellSlot flash = Player.GetSpellSlot("summonerflash");

        private static Menu Menu;

        public static bool waitE, waitQ, waitAA, waitW, waitTiamat, waitR1, waitR2, midAA, canAA, forceQ, forceW, forceT, forceR, waitR, castR, forceEburst, qGap
            , R2style;
        public static int waitQTick, waitR2Tick;
        private static AttackableUnit TTTar = null;

        public static float cE, cQ, cAA, cW, cTiamt, cR1, cR2, Wind, countforce, Rstate, R2countdonw;
        public static int Qstate = 1;

        public static int Windup { get { return Orbwalking.Orbwalker.getWindUp(); } }

        public static void Main()
        {
            OnStart();
        }

        private static void OnStart()
        {
            if (Player.ChampionName != "Riven")
                return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 250);
            R = new Spell(SpellSlot.R, 900);
            R.SetSkillshot(0.25f, 45, 1600, false, SkillshotType.SkillshotCone);
            R.MinHitChance = HitChance.Medium;

            Menu = new Menu("Heaven Strike " + Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));
            spellMenu.AddItem(new MenuItem("RcomboAlways", "RcomboAlways").SetValue(false));
            spellMenu.AddItem(new MenuItem("RcomboKillable", "RcomboKillable").SetValue(true));
            spellMenu.AddItem(new MenuItem("R2comboKS", "R2comboKS").SetValue(true));
            spellMenu.AddItem(new MenuItem("R2comboMaxdmg", "RcomboMaxdmg").SetValue(true));
            spellMenu.AddItem(new MenuItem("R2 Badao Style", "R2 Badao Style").SetValue(true));
            spellMenu.AddItem(new MenuItem("Ecombo", "Ecombo").SetValue(true));
            spellMenu.AddItem(new MenuItem("Q Gap", "Q Gap").SetValue(false));
            spellMenu.AddItem(new MenuItem("Use Q Before Expiry", "Use Q Before Expiry").SetValue(true));
            spellMenu.AddItem(new MenuItem("Q strange Cancel", "Q strange Cancel").SetValue(true));
            spellMenu.AddItem(new MenuItem("Qmode", "Q cast mode").SetValue(new StringList(new[] { "Lock Target", "To Mouse" }, 0)));
            Menu BurstCombo = spellMenu.AddSubMenu(new Menu("Burst Combo", "Burst Combo"));
            //BurstCombo.AddItem(new MenuItem("Burst", "Burst").SetValue(new KeyBind('T', KeyBindType.Press)));
            BurstCombo.AddItem(new MenuItem("Use Flash", "Use Flash").SetValue(false));
            Menu Misc = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            Misc.AddItem(new MenuItem("W interrupt", "W interrupt").SetValue(true));
            Misc.AddItem(new MenuItem("W gapcloser", "W gapcloser").SetValue(true));
            Menu Draw = Menu.AddSubMenu(new Menu("Draw", "Draw"));
            Draw.AddItem(new MenuItem("Draw dmg text", "Draw dmg text").SetValue(true));
            Menu other = Menu.AddSubMenu(new Menu("other", "other"));
            other.AddItem(new MenuItem("Flee", "Flee").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
            other.AddItem(new MenuItem("WallJumpHelper", "WallJumpHelper").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
            //other.AddItem(new MenuItem("FastHarass", "FastHarass").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Press)));
            Menu Clear = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            Clear.AddItem(new MenuItem("Use Tiamat", "Use Tiamat").SetValue(true));
            Clear.AddItem(new MenuItem("Use Q", "Use Q").SetValue(true));
            Clear.AddItem(new MenuItem("Use W", "Use W").SetValue(true));
            Clear.AddItem(new MenuItem("Use E", "Use E").SetValue(true));
            TargetSelector.AddToMenu(ts);
            Menu.AddToMainMenu();

            Drawing.OnDraw += OnDraw;

            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += AfterAttack;
            Orbwalking.OnAttack += OnAttack;
            Obj_AI_Base.OnProcessSpellCast += oncast;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Interrupter2.OnInterruptableTarget += interrupt;
            AntiGapcloser.OnEnemyGapcloser += gapcloser;

        }

        private static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (args.Animation.Contains("c29"))
            {
                if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(280 - Game.Ping, () => EloBuddy.Player.DoEmote(Emote.Dance));
                }
                Qstate = 2;
            }
            else if (args.Animation.Contains("c39"))
            {
                if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None)
                    LeagueSharp.Common.Utility.DelayAction.Add(300 - Game.Ping, () => EloBuddy.Player.DoEmote(Emote.Dance));
                Qstate = 3;
            }
            else if (args.Animation.Contains("c49"))
            {
                if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None)
                    LeagueSharp.Common.Utility.DelayAction.Add(380 - Game.Ping, () => EloBuddy.Player.DoEmote(Emote.Dance));
                Qstate = 1;
            }
            
        }

        private static int Qmode { get { return Menu.Item("Qmode").GetValue<StringList>().SelectedIndex; } }
        private static bool Qstrangecancel { get { return Menu.Item("Q strange Cancel").GetValue<bool>(); } }
        private static bool Rcomboalways { get { return Menu.Item("RcomboAlways").GetValue<bool>(); } }
        private static bool RcomboKillable { get { return Menu.Item("RcomboKillable").GetValue<bool>(); } }
        private static bool R2comboKS { get { return Menu.Item("R2comboKS").GetValue<bool>(); } }
        private static bool R2comboMaxdmg { get { return Menu.Item("R2comboMaxdmg").GetValue<bool>(); } }
        private static bool R2BadaoStyle { get { return Menu.Item("R2 Badao Style").GetValue<bool>(); } }
        private static bool Ecombo { get { return Menu.Item("Ecombo").GetValue<bool>(); } }
        private static bool QGap { get { return Menu.Item("Q Gap").GetValue<bool>(); } }
        private static bool UseQBeforeExpiry { get { return Menu.Item("Use Q Before Expiry").GetValue<bool>(); } }
        private static bool BurstActive { get { return Menu.Item("Burst").GetValue<KeyBind>().Active; } }
        private static bool FlashBurst { get { return Menu.Item("Use Flash").GetValue<bool>(); } }
        private static bool Winterrupt { get { return Menu.Item("W interrupt").GetValue<bool>(); } }
        private static bool Wgapcloser { get { return Menu.Item("W gapcloser").GetValue<bool>(); } }

        private static bool Drawdamage { get { return Menu.Item("Draw dmg text").GetValue<bool>(); } }
        private static bool FleeActive { get { return Menu.Item("Flee").GetValue<KeyBind>().Active; } }
        private static bool WallJumpHelperActive { get { return Menu.Item("WallJumpHelper").GetValue<KeyBind>().Active; } }
        private static bool FastHarassActive { get { return Menu.Item("FastHarass").GetValue<KeyBind>().Active; } }
        private static bool UseTiamatClear { get { return Menu.Item("Use Tiamat").GetValue<bool>(); } }
        private static bool UseQClear { get { return Menu.Item("Use Q").GetValue<bool>(); } }
        private static bool UseWClear { get { return Menu.Item("Use W").GetValue<bool>(); } }
        private static bool UseEClear { get { return Menu.Item("Use E").GetValue<bool>(); } }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            SolvingWaitList();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                Clear();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                Combo();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Burst)
                Burst();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.QuickHarass)
                fastharass();
            if (WallJumpHelperActive)
                walljump();
            if (FleeActive)
                flee();
        }
        public static void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            var target = TargetSelector.GetSelectedTarget();
            if (target != null && target.IsValidTarget() && !target.IsZombie)
                Render.Circle.DrawCircle(target.Position, 150, Color.AliceBlue, 15);
            if (Menu.Item("Draw dmg text").GetValue<bool>())
                foreach (var hero in HeroManager.Enemies)
                {
                    if (hero.IsValidTarget(1500))
                    {
                        var dmg = totaldame(hero) > hero.Health ? 100 : totaldame(hero) * 100 / hero.Health;
                        var dmg1 = Math.Round(dmg);
                        var x = Drawing.WorldToScreen(hero.Position);
                        Color mau = dmg1 == 100 ? Color.Red : Color.Yellow;
                        Drawing.DrawText(x[0], x[1], mau, dmg1.ToString() + " %");
                    }
                }
        }
        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            TTTar = target;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (HasItem())
                {
                    CastItem();
                }
                else if (R2BadaoStyle && R.IsReady() && R.Instance.Name == R2name && Qstate == 3)
                {
                    if (target is Obj_AI_Base)
                    {
                        R.Cast(target as Obj_AI_Base);
                    }
                    if (Q.IsReady())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(150, () => callbackQ(TTTar));
                    }
                }
                else if (W.IsReady() && InWRange(target))
                {
                    W.Cast();
                    if (Q.IsReady())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(150, () => callbackQ(TTTar));
                    }
                }
                else if (Q.IsReady())
                {
                    callbackQ(TTTar);
                }
                else if (E.IsReady() && Ecombo)
                {
                    E.Cast(target.Position);
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (HasItem() && UseTiamatClear)
                {
                    CastItem();
                }
                else if (W.IsReady() && InWRange(target) && UseWClear)
                {
                    W.Cast();
                    if (Q.IsReady() && UseQClear)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(150, () => callbackQ(TTTar));
                    }
                }
                else if (Q.IsReady() && UseQClear)
                {
                    callbackQ(TTTar);
                }
                else if (E.IsReady() && UseEClear)
                {
                    E.Cast(target.Position);
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Burst)
            {
                if (HasItem())
                {
                    CastItem();
                    if (R.IsReady() && R.Instance.Name == R2name)
                    {
                        if (target is AIHeroClient)
                        {
                            callbackR2(TTTar);
                        }
                        if (Q.IsReady())
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add(150, () => callbackQ(TTTar));
                        }
                    }
                    else if(Q.IsReady())
                    {
                        callbackQ(TTTar);
                    }

                }
                else if (R.IsReady() && R.Instance.Name == R2name)
                {
                    if (target is AIHeroClient)
                    {
                        R.Cast(target as AIHeroClient);
                    }
                    if (Q.IsReady())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(150, () => callbackQ(TTTar));
                    }
                }
                else if (W.IsReady() && InWRange(target))
                {
                    W.Cast();
                    if (Q.IsReady())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(150, () => callbackQ(TTTar));
                    }
                }
                else if (Q.IsReady())
                {
                    callbackQ(TTTar);
                }
                else if (E.IsReady() && Ecombo)
                {
                    E.Cast(target.Position);
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.QuickHarass)
            {
                if (HasItem())
                {
                    CastItem();
                }
                else if (W.IsReady() && InWRange(target))
                {
                    W.Cast();
                    if (Q.IsReady())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(150, () => callbackQ(TTTar));
                    }
                }
                else if (Q.IsReady())
                {
                    Q.Cast(target.Position);
                }
                else if (E.IsReady())
                {
                    E.Cast(target.Position);
                }
            }
        }
        public static void interrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && W.IsReady() && sender.IsValidTarget() && !sender.IsZombie && Menu.Item("W interrupt").GetValue<bool>())
            {
                if (sender.IsValidTarget(125 + Player.BoundingRadius + sender.BoundingRadius)) W.Cast();
            }
        }
        public static void gapcloser(ActiveGapcloser gapcloser)
        {
            var target = gapcloser.Sender;
            if (target.IsEnemy && W.IsReady() && target.IsValidTarget() && !target.IsZombie && Menu.Item("W gapcloser").GetValue<bool>())
            {
                if (target.IsValidTarget(125 + Player.BoundingRadius + target.BoundingRadius)) W.Cast();
            }
        }
        private static void oncast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spell = args.SData;

            if (!sender.IsMe)
            {
                return;
            }
            if (spell.Name.Contains("ItemTiamatCleave"))
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.QuickHarass)
                {
                    if (Q.IsReady())
                    {
                        callbackQ(TTTar);
                    }
                }
            }
            if (args.SData.IsAutoAttack())
            {

            }
            if (spell.Name.Contains("RivenTriCleave"))
            {

                waitQ = false;
                Orbwalking.ResetAutoAttackTimer();
                if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(40, () => Reset(40));
                }

                cQ = Utils.GameTimeTickCount;
            }
            if (spell.Name.Contains("RivenMartyr"))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(160 - Game.Ping, () => Chat.Say("/d"));

            }
            if (spell.Name.Contains("RivenFient"))
            {

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Burst)
                {
                    if (R.IsReady() && R.Instance.Name == R1name)
                        LeagueSharp.Common.Utility.DelayAction.Add(150, () => R.Cast());
                }
            }
            if (spell.Name.Contains("RivenFengShuiEngine"))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(140 - Game.Ping, () => Chat.Say("/d"));

            }
            if (spell.Name.Contains("rivenizunablade"))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(140 - Game.Ping, () => Chat.Say("/d"));

            }
        }

        private static void Reset(int t)
        {
            LeagueSharp.Common.Utility.DelayAction.Add(0, () => Orbwalking.ResetAutoAttackTimer());
            for (int i = 10; i < t; i = i + 10)
            {
                if (i - Game.Ping >= 0)
                    LeagueSharp.Common.Utility.DelayAction.Add(i - Game.Ping, () => Cancel());
            }
        }
        private static void Cancel()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Player.Position.Extend(Game.CursorPos, Player.Distance(Game.CursorPos) + 500));
            if (Qstrangecancel)EloBuddy.Player.DoEmote(Emote.Dance);
        }
        private static void OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (ItemData.Youmuus_Ghostblade.GetItem().IsReady())
                    ItemData.Youmuus_Ghostblade.GetItem().Cast();
            }
        }

        private static void Burst()
        {
            var target = TargetSelector.GetSelectedTarget();
            if (target != null && target.IsValidTarget() && !target.IsZombie)
            {
                if (Orbwalking.InAutoAttackRange(target) && Orbwalking.CanMove(Windup + 20) && (!R.IsReady() || (R.IsReady() && R.Instance.Name == R1name)))
                {
                    W.Cast();
                }
                if (Orbwalking.InAutoAttackRange(target) && Orbwalking.CanMove(Windup + 20) && R.IsReady())
                {
                    if (R.IsReady() && R.Instance.Name == R1name) R.Cast();
                    LeagueSharp.Common.Utility.DelayAction.Add(350, () => CastItem());
                    LeagueSharp.Common.Utility.DelayAction.Add(400, () => W.Cast());
                }
                if (!Orbwalking.InAutoAttackRange(target) && Orbwalking.CanMove(Windup + 20) && E.IsReady() && R.IsReady() && Player.Distance(target.Position) <= E.Range + Player.BoundingRadius + target.BoundingRadius)
                {
                    E.Cast(Player.Position.Extend(target.Position, 200));
                    if (R.IsReady() && R.Instance.Name == R1name) R.Cast();
                    LeagueSharp.Common.Utility.DelayAction.Add(350, () => CastItem());
                    LeagueSharp.Common.Utility.DelayAction.Add(400, () => W.Cast());
                }
                if (!Orbwalking.InAutoAttackRange(target) && Orbwalking.CanMove(Windup + 20) && !E.IsReady() && R.IsReady() && !Player.IsDashing()
                    && flash != SpellSlot.Unknown && flash.IsReady() && FlashBurst && Player.Distance(target.Position) <= 425 + Player.BoundingRadius + target.BoundingRadius)
                {
                    if (R.IsReady() && R.Instance.Name == R1name) R.Cast();
                    var x = Player.Distance(target.Position) > 425 ? Player.Position.Extend(target.Position, 425) : target.Position;
                    Player.Spellbook.CastSpell(flash, x);
                    LeagueSharp.Common.Utility.DelayAction.Add(350, () => CastItem());
                    LeagueSharp.Common.Utility.DelayAction.Add(400, () => W.Cast());
                }
                if (!Orbwalking.InAutoAttackRange(target) && Orbwalking.CanMove(Windup + 20) && E.IsReady() && flash != SpellSlot.Unknown && flash.IsReady() && FlashBurst
                    && R.IsReady() && Player.Distance(target.Position) <= E.Range + Player.BoundingRadius + target.BoundingRadius + 425
                    && Player.Distance(target.Position) > Player.BoundingRadius + target.BoundingRadius + 425)
                {
                    if (R.IsReady() && R.Instance.Name == R1name) R.Cast();
                    E.Cast(Player.Position.Extend(target.Position, 200));
                    LeagueSharp.Common.Utility.DelayAction.Add(350,() => Player.Spellbook.CastSpell(flash, target.Position));
                    LeagueSharp.Common.Utility.DelayAction.Add(350, () => CastItem());
                    LeagueSharp.Common.Utility.DelayAction.Add(500, () => W.Cast());
                }
            }
        }

        private static void Combo()
        {
            if (Q.IsReady() && Orbwalking.CanMove(Windup + 20) && QGap && !Player.IsDashing())
            {
                var target = HeroManager.Enemies.Where(x => x.IsValidTarget()).OrderByDescending(x => 1 - x.Distance(Player.Position)).FirstOrDefault();
                if (!Player.IsDashing() && Utils.GameTimeTickCount - cQ >= 1000 && target.IsValidTarget())
                {
                    if (Prediction.GetPrediction(Player, 100).UnitPosition.Distance(target.Position) <= Player.Distance(target.Position))
                        Q.Cast(Game.CursorPos);
                }
            }
            if (W.IsReady() && Orbwalking.CanMove(Windup + 20))
            {
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && InWRange(x));
                if (targets.Any())
                {
                    W.Cast();
                }
            }
            if (E.IsReady() && Orbwalking.CanMove(Windup + 20) && Ecombo)
            {
                var target = TargetSelector.GetTarget(325 + Player.AttackRange + 70, TargetSelector.DamageType.Physical);
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    E.Cast(target.Position);
                }
            }
            if (R.IsReady())
            {
                if (R.Instance.Name == R1name)
                {
                    if (Rcomboalways)
                    {
                        var target = TargetSelector.GetTarget(325 + Player.AttackRange + 70, TargetSelector.DamageType.Physical);
                        if (target.IsValidTarget() && !target.IsZombie && E.IsReady())
                        {
                            R.Cast();
                        }
                        else
                        {
                            var targetR = TargetSelector.GetTarget(200 + Player.BoundingRadius + 70, TargetSelector.DamageType.Physical);
                            if (targetR.IsValidTarget() && !targetR.IsZombie)
                            {
                                R.Cast();
                            }
                        }

                    }
                    if (RcomboKillable)
                    {
                        var targetR = TargetSelector.GetTarget(200 + Player.BoundingRadius + 70, TargetSelector.DamageType.Physical);
                        if (targetR.IsValidTarget() && !targetR.IsZombie && basicdmg(targetR) <= targetR.Health && totaldame(targetR) >= targetR.Health)
                        {
                            R.Cast();
                        }
                        if (targetR.IsValidTarget() && !targetR.IsZombie && Player.CountEnemiesInRange(800) >= 2)
                        {
                            R.Cast();
                        }
                    }
                }
                else if (R.Instance.Name == R2name)
                {
                    if (R2comboKS)
                    {
                        var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && !x.IsZombie);
                        foreach (var target in targets)
                        {
                            if (target.Health < Rdame(target, target.Health))
                                R.Cast(target);
                        }
                    }
                    if (R2comboMaxdmg)
                    {
                        var targets = HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && !x.IsZombie);
                        foreach (var target in targets)
                        {
                            if (target.Health / target.MaxHealth <= 0.25)
                                R.Cast(target);
                        }
                    }
                    if (R2BadaoStyle && !Q.IsReady())
                    {
                        var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                        if (target.IsValidTarget() && !target.IsZombie)
                        {
                            R.Cast(target);
                        }
                    }
                    var targethits = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                    if (targethits.IsValidTarget() && !targethits.IsZombie)
                        R.CastIfWillHit(targethits, 4);

                }
            }
        }
        private static void Clear()
        {
            var targetW = MinionManager.GetMinions(Player.Position, WRange() + 100, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health).FirstOrDefault();
            var targetW2 = MinionManager.GetMinions(Player.Position, WRange() + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (targetW != null && InWRange(targetW) && W.IsReady() && Orbwalking.CanMove(Windup + 20) && UseWClear)
            {
                W.Cast();
            }
            if (targetW2 != null && InWRange(targetW2) && W.IsReady() && Orbwalking.CanMove(Windup + 20) && UseWClear)
            {
                W.Cast();
            }
            if (targetW != null && InWRange(targetW) && E.IsReady() && Orbwalking.CanMove(Windup + 20) && UseEClear)
            {
                E.Cast(targetW.Position);
            }
            if (targetW2 != null && InWRange(targetW2) && E.IsReady() && Orbwalking.CanMove(Windup + 20) && UseEClear)
            {
                E.Cast(targetW2.Position);
            }
        }
        public static void fastharass()
        {
            if (W.IsReady() && Orbwalking.CanMove(Windup + 20))
            {
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && InWRange(x));
                if (targets.Any())
                {
                    W.Cast();
                }
            }
            if (E.IsReady() && Orbwalking.CanMove(Windup + 20))
            {
                var target = TargetSelector.GetTarget(325 + Player.AttackRange + 70, TargetSelector.DamageType.Physical);
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    E.Cast(target.Position);
                }
            }
        }
        private static void SolvingWaitList()
        {
            if (!Q.IsReady(1000)) Qstate = 1;
            if (waitQ == true && TTTar.IsValidTarget())
            {
                //if (Utils.GameTimeTickCount - cQ >= 350 + Player.AttackCastDelay - Game.Ping / 2)
                if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (Qmode == 0 && TTTar != null)
                        Q.Cast(TTTar.Position);
                    else
                        Q.Cast(Game.CursorPos);
                }
                else
                {
                    if (Qmode == 0 && TTTar != null)
                        Q.Cast(TTTar.Position);
                    else
                        Q.Cast(Game.CursorPos);
                }
                if (Environment.TickCount - waitQTick >= 500 + Game.Ping / 2)
                    waitQ = false;
            }
            if (waitR2 == true && TTTar.IsValidTarget())
            {
                R.Cast(TTTar as Obj_AI_Base);
                if (Environment.TickCount - waitQTick >= 500 + Game.Ping / 2)
                    waitQ = false;
            }
            if (Q.IsReady() && UseQBeforeExpiry && !Player.IsRecalling())
            {
                if (Qstate != 1 && Utils.GameTimeTickCount - cQ <= 3800 - Game.Ping / 2 && Utils.GameTimeTickCount - cQ >= 3300 - Game.Ping / 2) { Q.Cast(Game.CursorPos); }
            }
        }
        public static bool HasItem()
        {
            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady() || ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void CastItem()
        {

            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                ItemData.Tiamat_Melee_Only.GetItem().Cast();
            if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
        }

        private static bool InWRange(AttackableUnit target)
        {
            if (Player.HasBuff("RivenFengShuiEngine"))
            {
                return
                    target.BoundingRadius + 200 + Player.BoundingRadius >= Player.Distance(target.Position);
            }
            else
            {
                return
                   target.BoundingRadius + 125 + Player.BoundingRadius >= Player.Distance(target.Position);
            }
        }
        private static float WRange()
        {
            if (Player.HasBuff("RivenFengShuiEngine"))
            {
                return
                    200 + Player.BoundingRadius;
            }
            else
            {
                return
                   125 + Player.BoundingRadius;
            }
        }
        private static void callbackQ(AttackableUnit target)
        {
            waitQ = true;
            TTTar = target;
            waitQTick = Environment.TickCount;
        }
        private static void callbackR2(AttackableUnit target)
        {
            waitR2 = true;
            TTTar = target;
            waitR2Tick = Environment.TickCount;
        }
        public static void checkbuff()
        {
            String temp = "";
            foreach (var buff in Player.Buffs)
            {
                temp += (buff.Name + "(" + buff.Count + ")" + "(" + buff.Type.ToString() + ")" + ", ");
            }
            Chat.Say(temp);
        }
        public static double basicdmg(Obj_AI_Base target)
        {
            if (target != null)
            {
                double dmg = 0;
                double passivenhan = 0;
                if (Player.Level >= 18) { passivenhan = 0.5; }
                else if (Player.Level >= 15) { passivenhan = 0.45; }
                else if (Player.Level >= 12) { passivenhan = 0.4; }
                else if (Player.Level >= 9) { passivenhan = 0.35; }
                else if (Player.Level >= 6) { passivenhan = 0.3; }
                else if (Player.Level >= 3) { passivenhan = 0.25; }
                else { passivenhan = 0.2; }
                if (HasItem()) dmg = dmg + Player.GetAutoAttackDamage(target) * 0.7;
                if (W.IsReady()) dmg = dmg + W.GetDamage(target);
                if (Q.IsReady())
                {
                    var qnhan = 4 - Qstate;
                    dmg = dmg + Q.GetDamage(target) * qnhan + Player.GetAutoAttackDamage(target) * qnhan * (1 + passivenhan);
                }
                dmg = dmg + Player.GetAutoAttackDamage(target) * (1 + passivenhan);
                return dmg;
            }
            else { return 0; }
        }
        public static double totaldame(Obj_AI_Base target)
        {
            if (target != null)
            {
                double dmg = 0;
                double passivenhan = 0;
                if (Player.Level >= 18) { passivenhan = 0.5; }
                else if (Player.Level >= 15) { passivenhan = 0.45; }
                else if (Player.Level >= 12) { passivenhan = 0.4; }
                else if (Player.Level >= 9) { passivenhan = 0.35; }
                else if (Player.Level >= 6) { passivenhan = 0.3; }
                else if (Player.Level >= 3) { passivenhan = 0.25; }
                else { passivenhan = 0.2; }
                if (HasItem()) dmg = dmg + Player.GetAutoAttackDamage(target) * 0.7;
                if (W.IsReady()) dmg = dmg + W.GetDamage(target);
                if (Q.IsReady())
                {
                    var qnhan = 4 - Qstate;
                    dmg = dmg + Q.GetDamage(target) * qnhan + Player.GetAutoAttackDamage(target) * qnhan * (1 + passivenhan);
                }
                dmg = dmg + Player.GetAutoAttackDamage(target) * (1 + passivenhan);
                if (R.IsReady())
                {
                    if (Rstate == 0)
                    {
                        var rdmg = Rdame(target, target.Health - dmg * 1.2);
                        return dmg * 1.2 + rdmg;
                    }
                    else if (Rstate == 1)
                    {
                        var rdmg = Rdame(target, target.Health - dmg);
                        return rdmg + dmg;
                    }
                    else return dmg;
                }
                else return dmg;
            }
            else return 0;
        }
        public static double Rdame(Obj_AI_Base target, double health)
        {
            if (target != null)
            {
                var missinghealth = (target.MaxHealth - health) / target.MaxHealth > 0.75 ? 0.75 : (target.MaxHealth - health) / target.MaxHealth;
                var pluspercent = missinghealth * (8 / 3);
                var rawdmg = new double[] { 80, 120, 160 }[R.Level - 1] + 0.6 * Player.FlatPhysicalDamageMod;
                return Player.CalcDamage(target, Damage.DamageType.Physical, rawdmg * (1 + pluspercent));
            }
            else return 0;
        }

        public static void walljump()
        {
            var x = Player.Position.Extend(Game.CursorPos, 100);
            var y = Player.Position.Extend(Game.CursorPos, 30);
            if (!x.IsWall() && !y.IsWall()) EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, x);
            if (x.IsWall() && !y.IsWall()) EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, y);
            if (Prediction.GetPrediction(Player, 500).UnitPosition.Distance(Player.Position) <= 10) { Q.Cast(Game.CursorPos); }
        }
        public static void flee()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var x = Player.Position.Extend(Game.CursorPos, 300);
            if (Q.IsReady() && !Player.IsDashing()) Q.Cast(Game.CursorPos);
            if (E.IsReady() && !Player.IsDashing()) E.Cast(x);
        }

    }
}
