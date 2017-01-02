#region

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SebbyLib;

#endregion

using EloBuddy; 
using LeagueSharp.Common; 
namespace SyndraRevamped
{
    using System.Drawing;

    using Color = SharpDX.Color;

    internal static class Program
    {
        public const string ChampionName = "Syndra";

        //Orbwalker instance
        public static Orbwalking.Orbwalker Orbwalker;

        //Spells
        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;

        public static Spell W;

        public static Spell E;

        public static Spell Eq;

        public static Spell R;

        public static SpellSlot IgniteSlot;

        public static AssassinManager AssassinManager;

        //Menu
        public static Menu Config, DrawMenu;

        private static int qeComboT;

        private static int weComboT;

        public static AIHeroClient Player;

        public static Vector2 _yasuoWallCastedPos;

        public static GameObject _yasuoWall;

        public static int _wallCastT;

        // ReSharper disable once UnusedParameter.Local
        public static void Main()
        {
            Game_OnGameLoad(new EventArgs());
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;

            if (Player.BaseSkinName != ChampionName) return;

            //Create the spells
            Q = new Spell(SpellSlot.Q, 800);
            W = new Spell(SpellSlot.W, 925);
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 675);
            Eq = new Spell(SpellSlot.Q, Q.Range + 450);

            IgniteSlot = Player.GetSpellSlot("SummonerDot");

            Q.SetSkillshot(0.5f, 130f, 2000f, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(0.25f, 140f, 1600f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, (float)(45 * 0.5), 2500f, false, SkillshotType.SkillshotCircle);
            Eq.SetSkillshot(0.900f, 70f, 2100f, false, SkillshotType.SkillshotCircle);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            //Create the menu
            Config = new Menu(ChampionName, ChampionName, true).SetFontStyle(FontStyle.Regular, Color.GreenYellow);

            //Orbwalker submenu
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

           // AssassinManager = new AssassinManager();
            //AssassinManager.Initialize();

            //Initialize the orbwalker and add it to the menu as submenu.
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //var menuKeys = new Menu("Keys", "Keys").SetFontStyle(FontStyle.Regular, Color.Aqua);
            Config.AddSubMenu(new Menu("Keys", "Keys").SetFontStyle(FontStyle.Regular, Color.Aqua));
            {
                Config.SubMenu("Keys").AddItem(
                    new MenuItem("Key.Combo", "Combo!").SetValue(
                        new KeyBind(Config.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)))
                    .SetFontStyle(FontStyle.Regular, Color.GreenYellow);
                Config.SubMenu("Keys").AddItem(
                    new MenuItem("Key.Harass", "Harass!").SetValue(
                        new KeyBind(Config.Item("Farm").GetValue<KeyBind>().Key, KeyBindType.Press)))
                    .SetFontStyle(FontStyle.Regular, Color.Coral);
                Config.SubMenu("Keys").AddItem(
                    new MenuItem("Key.HarassT", "Harass (toggle)!").SetValue(
                        new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)))
                    .SetFontStyle(FontStyle.Regular, Color.Coral)
                    .Permashow(true, "Syndra | Toggle Harass", Color.Aqua);
                Config.SubMenu("Keys").AddItem(
                    new MenuItem("Key.Lane", "Lane Clear!").SetValue(
                        new KeyBind(Config.Item("LaneClear").GetValue<KeyBind>().Key, KeyBindType.Press)))
                    .SetFontStyle(FontStyle.Regular, Color.DarkKhaki);
                Config.SubMenu("Keys").AddItem(
                    new MenuItem("Key.Jungle", "Jungle Farm!").SetValue(
                        new KeyBind(Config.Item("LaneClear").GetValue<KeyBind>().Key, KeyBindType.Press)))
                    .SetFontStyle(FontStyle.Regular, Color.DarkKhaki);
                Config.SubMenu("Keys").AddItem(
                    new MenuItem("Key.InstantQE", "Instant Q-E to Enemy").SetValue(
                        new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
                //Config.AddSubMenu(menuKeys);
            }

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            {
                Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
                Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
                Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
                Config.SubMenu("Combo").AddItem(new MenuItem("UseQECombo", "Use QE").SetValue(true));
                Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R").SetValue(true));
                Config.SubMenu("Combo").AddItem(new MenuItem("UseIgniteCombo", "Use Ignite").SetValue(true));
                //Config.AddSubMenu(menuCombo);
            }

            Config.AddSubMenu(new Menu("Harass", "Harass"));
            {
                Config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "Use Q").SetValue(true));

                Config.SubMenu("Harass").AddItem(new MenuItem("UseWHarass", "Use W").SetValue(false));
                Config.SubMenu("Harass").AddItem(new MenuItem("UseEHarass", "Use E").SetValue(false));
                Config.SubMenu("Harass").AddItem(new MenuItem("UseQEHarass", "Use QE").SetValue(false));
                Config.SubMenu("Harass").AddItem(
                    new MenuItem("Harass.Mana", "Don't harass if mana < %").SetValue(new Slider(0)));
                //Config.AddSubMenu(menuHarass);
            }

            Config.AddSubMenu(new Menu("Lane Farm", "Farm"));
            {
                Config.SubMenu("Farm").AddItem(new MenuItem("EnabledFarm", "Enable! (On/Off: Mouse Scroll)").SetValue(true))
                    .Permashow(true, "Syndra | Farm Mode Active", Color.Aqua);
                Config.SubMenu("Farm").AddItem(
                    new MenuItem("UseQFarm", "Use Q").SetValue(
                        new StringList(new[] { "Freeze", "LaneClear", "Both", "No" }, 2)));
                Config.SubMenu("Farm").AddItem(
                    new MenuItem("UseWFarm", "Use W").SetValue(
                        new StringList(new[] { "Freeze", "LaneClear", "Both", "No" }, 1)));
                Config.SubMenu("Farm").AddItem(
                    new MenuItem("UseEFarm", "Use E").SetValue(
                        new StringList(new[] { "Freeze", "LaneClear", "Both", "No" }, 3)));
                Config.SubMenu("Farm").AddItem(
                    new MenuItem("FreezeActive", "Freeze!").SetValue(
                        new KeyBind(Config.Item("Farm").GetValue<KeyBind>().Key, KeyBindType.Press)));
                Config.SubMenu("Farm").AddItem(new MenuItem("Lane.Mana", "Don't harass if mana < %").SetValue(new Slider(0)));
                //Config.AddSubMenu(menuFarm);
            }

            Config.AddSubMenu(new Menu("Jungle Farm", "JungleFarm"));
            {
                Config.SubMenu("JungleFarm").AddItem(new MenuItem("UseQJFarm", "Use Q").SetValue(true));
                Config.SubMenu("JungleFarm").AddItem(new MenuItem("UseWJFarm", "Use W").SetValue(true));
                Config.SubMenu("JungleFarm").AddItem(new MenuItem("UseEJFarm", "Use E").SetValue(true));
                //Config.AddSubMenu(menuJungle);
            }
            Config.AddSubMenu(new Menu("[R] Settings", "Rsettings"));
            {
                Config.SubMenu("Rsettings").AddSubMenu(new Menu("Dont [R] if it can be killed with", "DontRw"));
                Config.SubMenu("Rsettings").SubMenu("DontRw").AddItem(new MenuItem("DontRwParam", "Damage From").SetValue(new StringList(new[] { "All", "Either one", "None" })));
                Config.SubMenu("Rsettings").SubMenu("DontRw").AddItem(new MenuItem("DontRwQ", "[Q]").SetValue(true));
                Config.SubMenu("Rsettings").SubMenu("DontRw").AddItem(new MenuItem("DontRwW", "[W]").SetValue(true));
                Config.SubMenu("Rsettings").SubMenu("DontRw").AddItem(new MenuItem("DontRwE", "[E]").SetValue(true));
                Config.SubMenu("Rsettings").SubMenu("DontRw").AddItem(new MenuItem("DontRwA", "[AA]").SetValue(true));

                Config.SubMenu("Rsettings").AddSubMenu(new Menu("Dont use R on", "DontUlt"));
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                    Config.SubMenu("Rsettings").SubMenu("DontUlt").AddItem(new MenuItem("DontUlt" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(false));
                Config.SubMenu("Rsettings").AddSubMenu(new Menu("Buff Check (Don't Ult)", "DontRbuff"));
                Config.SubMenu("Rsettings").SubMenu("DontRbuff").AddItem(new MenuItem("DontRbuffUndying", "Trynda's Ult").SetValue(true));
                Config.SubMenu("Rsettings").SubMenu("DontRbuff").AddItem(new MenuItem("DontRbuffJudicator", "Kayle's Ult").SetValue(true));
                Config.SubMenu("Rsettings").SubMenu("DontRbuff").AddItem(new MenuItem("DontRbuffAlistar", "Zilean's Ult").SetValue(true));
                Config.SubMenu("Rsettings").SubMenu("DontRbuff").AddItem(new MenuItem("DontRbuffZilean", "Alistar's Ult").SetValue(true));
                Config.SubMenu("Rsettings").SubMenu("DontRbuff").AddItem(new MenuItem("DontRbuffZac", "Zac's Passive").SetValue(true));
                Config.SubMenu("Rsettings").SubMenu("DontRbuff").AddItem(new MenuItem("DontRbuffAttrox", "Attrox's Passive").SetValue(true));
                Config.SubMenu("Rsettings").SubMenu("DontRbuff").AddItem(new MenuItem("DontRbuffSivir", "Sivir's Spell Shield").SetValue(true));
                Config.SubMenu("Rsettings").SubMenu("DontRbuff").AddItem(new MenuItem("DontRbuffMorgana", "Morgana's Black Shield").SetValue(true));
                Config.SubMenu("Rsettings").AddSubMenu(new Menu("OverKill target by %", "okR"));
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                    Config.SubMenu("Rsettings").SubMenu("okR").AddItem(new MenuItem("okR" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(new Slider(0)));
                //Config.AddSubMenu(Menu);
            }

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            {
                Config.SubMenu("Misc").AddItem(new MenuItem("InterruptSpells", "Interrupt spells").SetValue(true));
                Config.SubMenu("Misc").AddItem(new MenuItem("YasuoWall", "Yasuo Windwall Check").SetValue(true));
                Config.SubMenu("Misc").AddItem(
                    new MenuItem("CastQE", "QE closest to cursor").SetValue(
                        new KeyBind('T', KeyBindType.Press)));
                
                //Config.AddSubMenu(menuMisc);
            }

           


            DrawMenu = new Menu("Drawings", "Drawings");
            {
                DrawMenu.AddItem(
                    new MenuItem("QRange", "Q range").SetValue(
                        new Circle(false, System.Drawing.Color.FromArgb(100, 255, 0, 255))));
                DrawMenu.AddItem(
                    new MenuItem("WRange", "W range").SetValue(
                        new Circle(true, System.Drawing.Color.FromArgb(100, 255, 0, 255))));
                DrawMenu.AddItem(
                    new MenuItem("ERange", "E range").SetValue(
                        new Circle(false, System.Drawing.Color.FromArgb(100, 255, 0, 255))));
                DrawMenu.AddItem(
                    new MenuItem("RRange", "R range").SetValue(
                        new Circle(false, System.Drawing.Color.FromArgb(100, 255, 0, 255))));
                DrawMenu.AddItem(
                    new MenuItem("QERange", "QE range").SetValue(
                        new Circle(true, System.Drawing.Color.FromArgb(100, 255, 0, 255))));

                var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Draw Damage After Combo").SetValue(true);
                //LeagueSharp.Common.Utility.HpBar//DamageIndicator.DamageToUnit = GetComboDamage;
                //LeagueSharp.Common.Utility.HpBar//DamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
                dmgAfterComboItem.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        //LeagueSharp.Common.Utility.HpBar//DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };

                DrawMenu.AddItem(dmgAfterComboItem);
                ManaBarIndicator.Initialize();
                Config.AddSubMenu(DrawMenu);
            }
            Config.AddToMainMenu();

            //Add the events we are going to use:
            Game.OnUpdate += Game_OnGameUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;

            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

            Drawing.OnDraw += Drawing_OnDraw;
            Chat.Print("<font size='30'>Syndra</font> <font color='#b756c5'>Updated by LordZEDith</font>");
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != 0x20a) return;

            if (ObjectManager.Player.InShop() || ObjectManager.Player.InFountain()) return;

            Config.SubMenu("Farm")
                .Item("EnabledFarm")
                .SetValue(!Config.SubMenu("Farm").Item("EnabledFarm").GetValue<bool>());
        }

        private static void Interrupter2_OnInterruptableTarget(
            AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Config.Item("InterruptSpells").GetValue<bool>()) return;

            if (Player.Distance(sender) < E.Range && E.IsReady())
            {
                Q.Cast(sender.ServerPosition);
                E.Cast(sender.ServerPosition);
            }
            else if (Player.Distance(sender) < Eq.Range && E.IsReady() && Q.IsReady())
            {
                UseQe(sender);
            }
        }

        // ReSharper disable once InconsistentNaming
        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Config.Item("Key.Combo").GetValue<KeyBind>().Active)
            {
                args.Process = !(Q.IsReady() || W.IsReady());
            }
        }

        private static void InstantQe2Enemy()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var t = TargetSelector.GetTarget(Eq.Range, TargetSelector.DamageType.Magical);
            if (t.IsValidTarget() && E.IsReady() && Q.IsReady())
            {
                UseQe(t);
            }
        }

        private static void Combo()
        {
            UseSpells(
                Config.Item("UseQCombo").GetValue<bool>(),
                Config.Item("UseWCombo").GetValue<bool>(),
                Config.Item("UseECombo").GetValue<bool>(),
                Config.Item("UseRCombo").GetValue<bool>(),
                Config.Item("UseQECombo").GetValue<bool>(),
                Config.Item("UseIgniteCombo").GetValue<bool>(),
                false);
        }

        private static void Harass()
        {
            if (Player.ManaPercent < Config.Item("Harass.Mana").GetValue<Slider>().Value)
            {
                return;
            }

            UseSpells(
                Config.Item("UseQHarass").GetValue<bool>(),
                Config.Item("UseWHarass").GetValue<bool>(),
                Config.Item("UseEHarass").GetValue<bool>(),
                false,
                Config.Item("UseQEHarass").GetValue<bool>(),
                false,
                true);
        }

        private static void UseE(Obj_AI_Base enemy)
        {
            foreach (var orb in OrbManager.GetOrbs(true))
                if (Player.Distance(orb) < E.Range + 100)
                {
                    var startPoint = orb.To2D().Extend(Player.ServerPosition.To2D(), 100);
                    var endPoint = Player.ServerPosition.To2D()
                        .Extend(orb.To2D(), Player.Distance(orb) > 200 ? 1300 : 1000);
                    Eq.Delay = E.Delay + Player.Distance(orb) / E.Speed;
                    Eq.From = orb;
                    var enemyPred = Eq.GetPrediction(enemy);
                    if (enemyPred.Hitchance >= HitChance.High
                        && enemyPred.UnitPosition.To2D().Distance(startPoint, endPoint, false)
                        < Eq.Width + enemy.BoundingRadius)
                    {
                        E.Cast(orb, true);
                        W.LastCastAttemptT = Utils.TickCount;
                        return;
                    }
                }
        }

        private static void UseQe(Obj_AI_Base enemy)
        {
            Eq.Delay = E.Delay + Q.Range / E.Speed;
            Eq.From = Player.ServerPosition.To2D().Extend(enemy.ServerPosition.To2D(), Q.Range).To3D();

            var prediction = Eq.GetPrediction(enemy);
            if (prediction.Hitchance >= HitChance.High)
            {
                Q.Cast(Player.ServerPosition.To2D().Extend(prediction.CastPosition.To2D(), Q.Range - 100));
                qeComboT = Utils.TickCount;
                W.LastCastAttemptT = Utils.TickCount;
            }
        }

        private static Vector3 GetGrabableObjectPos(bool onlyOrbs)
        {
            if (!onlyOrbs)
            {
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget(W.Range)))
                {
                    return minion.ServerPosition;
                }
            }
            return OrbManager.GetOrbToGrab((int)W.Range);
        }

        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            var damage = 0d;
            damage += Q.IsReady(420) ? Q.GetDamage(enemy) : 0;
            damage += W.IsReady() ? W.GetDamage(enemy) : 0;
            damage += E.IsReady() ? E.GetDamage(enemy) : 0;
            
            if (IgniteSlot != SpellSlot.Unknown && ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                damage += ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }

            if (R.IsReady())
            {
                var rTarget = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                damage += Math.Min(7, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo) * Player.GetSpellDamage(enemy, SpellSlot.R, 1);
                var useR = (Config.Item("DontUlt" + enemy.BaseSkinName) != null
                        && Config.Item("DontUlt" + enemy.BaseSkinName).GetValue<bool>() == false);
                var okR = Config.Item("okR" + enemy.BaseSkinName).GetValue<Slider>().Value * .01 + 1;
                if (DetectCollision(enemy) && useR && ObjectManager.Player.Distance(enemy, true) <= Math.Pow(R.Range, 2) &&
                    (GetRDamage(enemy)) > enemy.Health * okR &&
                    RCheck(enemy))
                {
                    if (
                        !(ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.Q) > enemy.Health &&
                          ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).CooldownExpires - Game.Time < 2 &&
                          ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).CooldownExpires - Game.Time >= 0 && enemy.IsStunned) &&
                        Environment.TickCount - Q.LastCastAttemptT > 500 + Game.Ping &&
                        overkillcheckv2(rTarget) <= rTarget.Health && ObjectManager.Player.HealthPercent >= 35)
                    {
                        R.CastOnUnit(enemy);
                        R.LastCastAttemptT = Environment.TickCount;
                    }

                }
            }
            return (float)damage;
        }
        public static bool RCheck(Obj_AI_Base enemy)
        {
            double aa = 0;
            if (Config.Item("DontRwA").GetValue<bool>()) aa = ObjectManager.Player.GetAutoAttackDamage(enemy);
            //Menu check
            if (Config.Item("DontRwParam").GetValue<StringList>().SelectedIndex == 2) return true;

            //If can be killed by all the skills that are checked
            if (Config.Item("DontRwParam").GetValue<StringList>().SelectedIndex == 0 &&
                GetComboDamage(enemy, Config.Item("DontRwQ").GetValue<bool>(),
                Config.Item("DontRwW").GetValue<bool>(),
                Config.Item("DontRwE").GetValue<bool>(), false) + aa >= enemy.Health) return false;
            //If can be killed by either any of the skills
            if (Config.Item("DontRwParam").GetValue<StringList>().SelectedIndex == 1 &&
                (GetComboDamage(enemy, Config.Item("DontRwQ").GetValue<bool>(), false, false, false) >=
                 enemy.Health ||
                 GetComboDamage(enemy, Config.Item("DontRwW").GetValue<bool>(), false, false, false) >=
                 enemy.Health ||
                 GetComboDamage(enemy, Config.Item("DontRwE").GetValue<bool>(), false, false, false) >=
                 enemy.Health || aa >= enemy.Health)) return false;

            //Check last cast times
            return Environment.TickCount - Q.LastCastAttemptT > 600 + Game.Ping &&
                   Environment.TickCount - E.LastCastAttemptT > 600 + Game.Ping &&
                   Environment.TickCount - W.LastCastAttemptT > 600 + Game.Ping;
        }

        private static void UseSpells(bool useQ, bool useW, bool useE, bool useR, bool useQe,
            bool useIgnite, bool isHarass)
        {
            var qTarget = TargetSelector.GetTarget(Q.Range + (isHarass ? Q.Width / 3 : Q.Width), TargetSelector.DamageType.Magical);
            var wTarget = TargetSelector.GetTarget(W.Range + W.Width, TargetSelector.DamageType.Magical);
            var rTarget = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            var qeTarget = TargetSelector.GetTarget(Eq.Range, TargetSelector.DamageType.Magical);
            var comboDamage = rTarget != null ? GetComboDamage(rTarget) : 0;

            //Q
            if (qTarget != null && useQ)
            {
                Q.Cast(qTarget, false, true);
            }

            //E
            if (Utils.TickCount - W.LastCastAttemptT > Game.Ping + 150 && E.IsReady() && useE)
            {
                foreach (var enemy in HeroManager.Enemies)
                {
                    if (enemy.IsValidTarget(Eq.Range))
                    {
                        UseE(enemy);
                    }
                }
            }
                

            //W
            if (useW)
            {
                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1 && W.IsReady() && qeTarget != null)
                {
                    var gObjectPos = GetGrabableObjectPos(wTarget == null);

                    if (gObjectPos.To2D().IsValid() && Utils.TickCount - W.LastCastAttemptT > Game.Ping + 300
                        && Utils.TickCount - E.LastCastAttemptT > Game.Ping + 600)
                    {
                        W.Cast(gObjectPos);
                        W.LastCastAttemptT = Utils.TickCount;
                    }
                }
                else if (wTarget != null && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 1 && W.IsReady()
                         && Utils.TickCount - W.LastCastAttemptT > Game.Ping + 100)
                {
                    if (OrbManager.WObject(false) != null)
                    {
                        W.From = OrbManager.WObject(false).ServerPosition;
                        W.Cast(wTarget, false, true);
                    }
                }
            }


            if (rTarget != null && useR)
            {
                useR = (Config.Item("DontUlt" + rTarget.BaseSkinName) != null
                        && Config.Item("DontUlt" + rTarget.BaseSkinName).GetValue<bool>() == false);
            }
                

            if (rTarget != null && useR && R.IsReady() && comboDamage > rTarget.Health && !rTarget.IsZombie && !Q.IsReady())
            {
                R.Cast(rTarget);
            }

            //Ignite
            if (rTarget != null && useIgnite && IgniteSlot != SpellSlot.Unknown
                && ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (comboDamage > rTarget.Health)
                {
                    ObjectManager.Player.Spellbook.CastSpell(IgniteSlot, rTarget);
                }
            }

            //QE
            if (wTarget == null && qeTarget != null && Q.IsReady() && E.IsReady() && useQe )
            {
                UseQe(qeTarget);
            }

            //WE
            if (wTarget == null && qeTarget != null && E.IsReady() && useE && OrbManager.WObject(true) != null)
            {
                Eq.Delay = E.Delay + Q.Range / W.Speed;
                Eq.From = Player.ServerPosition.To2D().Extend(qeTarget.ServerPosition.To2D(), Q.Range).To3D();
                var prediction = Eq.GetPrediction(qeTarget);
                if (prediction.Hitchance >= HitChance.High)
                {
                    W.Cast(Player.ServerPosition.To2D().Extend(prediction.CastPosition.To2D(), Q.Range - 100));
                    weComboT = Utils.TickCount;
                }
            }
        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (Utils.TickCount - qeComboT < 500 && args.SData.Name.Equals("SyndraQ", StringComparison.InvariantCultureIgnoreCase))
            {
                W.LastCastAttemptT = Utils.TickCount + 400;
                E.Cast(args.End, true);
            }

            if (Utils.TickCount - weComboT < 500
                && (args.SData.Name.Equals("SyndraW", StringComparison.InvariantCultureIgnoreCase) || args.SData.Name.Equals("SyndraWCast", StringComparison.InvariantCultureIgnoreCase)))
            {
                W.LastCastAttemptT = Utils.TickCount + 400;
                E.Cast(args.End, true);
            }
        }

        private static void Farm(bool laneClear)
        {
            if (!Config.Item("EnabledFarm").GetValue<bool>())
            {
                return;
            }

            if (Player.ManaPercent < Config.Item("Lane.Mana").GetValue<Slider>().Value)
            {
                return;
            }
            if (!Orbwalking.CanMove(40))
            {
                return;
            }

            var rangedMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + Q.Width + 30, MinionTypes.Ranged);
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + Q.Width + 30);
            var rangedMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width + 30, MinionTypes.Ranged);
            var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width + 30);

            var useQi = Config.Item("UseQFarm").GetValue<StringList>().SelectedIndex;
            var useWi = Config.Item("UseWFarm").GetValue<StringList>().SelectedIndex;
            var useQ = (laneClear && (useQi == 1 || useQi == 2)) || (!laneClear && (useQi == 0 || useQi == 2));
            var useW = (laneClear && (useWi == 1 || useWi == 2)) || (!laneClear && (useWi == 0 || useWi == 2));

            if (useQ && Q.IsReady())
                if (laneClear)
                {
                    var fl1 = Q.GetCircularFarmLocation(rangedMinionsQ, Q.Width);
                    var fl2 = Q.GetCircularFarmLocation(allMinionsQ, Q.Width);

                    if (fl1.MinionsHit >= 3)
                    {
                        Q.Cast(fl1.Position);
                    }

                    else if (fl2.MinionsHit >= 2 || allMinionsQ.Count == 1)
                    {
                        Q.Cast(fl2.Position);
                    }
                }
                else
                {
                    foreach (
                        var minion in
                            allMinionsQ.Where(
                                minion =>
                                !Orbwalking.InAutoAttackRange(minion) && minion.Health < 0.75 * Q.GetDamage(minion)))
                    {
                        Q.Cast(minion);
                    }
                }

            if (useW && W.IsReady() && allMinionsW.Count > 3)
            {
                if (laneClear)
                {
                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                    {
                        //WObject
                        var gObjectPos = GetGrabableObjectPos(false);

                        if (gObjectPos.To2D().IsValid() && Utils.TickCount - W.LastCastAttemptT > Game.Ping + 150)
                        {
                            W.Cast(gObjectPos);
                        }
                    }
                    else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 1)
                    {
                        var fl1 = Q.GetCircularFarmLocation(rangedMinionsW, W.Width);
                        var fl2 = Q.GetCircularFarmLocation(allMinionsW, W.Width);

                        if (fl1.MinionsHit >= 3 && W.IsInRange(fl1.Position.To3D()))
                        {
                            W.Cast(fl1.Position);
                        }

                        else if (fl2.MinionsHit >= 1 && W.IsInRange(fl2.Position.To3D()) && fl1.MinionsHit <= 2)
                        {
                            W.Cast(fl2.Position);
                        }
                    }
                }
            }
        }

        private static void JungleFarm()
        {
            var useQ = Config.Item("UseQJFarm").GetValue<bool>();
            var useW = Config.Item("UseWJFarm").GetValue<bool>();
            var useE = Config.Item("UseEJFarm").GetValue<bool>();

            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range, 
                MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (mobs.Count > 0)
            {
                var mob = mobs[0];

                if (Q.IsReady() && useQ)
                {
                    Q.Cast(mob);
                }

                if (W.IsReady() && useW && Utils.TickCount - Q.LastCastAttemptT > 800)
                {
                    W.Cast(mob);
                }

                if (useE && E.IsReady())
                {
                    E.Cast(mob);
                }
            }
        }
        public static void OnCreate(GameObject obj, EventArgs args)
        {
            if (Player.Distance(obj.Position) > 1500 ||
                !ObjectManager.Get<AIHeroClient>()
                    .Any(h => h.ChampionName == "Yasuo" && h.IsEnemy && h.IsVisible && !h.IsDead)) return;
            //Yasuo Wall
            if (obj.IsValid &&
                System.Text.RegularExpressions.Regex.IsMatch(
                    obj.Name, "_w_windwall.\\.troy",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                _yasuoWall = obj;
            }
        }

        public static void OnDelete(GameObject obj, EventArgs args)
        {
            if (Player.Distance(obj.Position) > 1500 ||
                !ObjectManager.Get<AIHeroClient>()
                    .Any(h => h.ChampionName == "Yasuo" && h.IsEnemy && h.IsVisible && !h.IsDead)) return;
            //Yasuo Wall
            if (obj.IsValid && System.Text.RegularExpressions.Regex.IsMatch(
                obj.Name, "_w_windwall.\\.troy",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                _yasuoWall = null;
            }
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            //Update the R range
            R.Range = R.Level == 3 ? 750 : 675;

            if (Config.Item("CastQE").GetValue<KeyBind>().Active && E.IsReady() && Q.IsReady())
            {
                foreach (
                    var enemy in
                        HeroManager.Enemies
                            .Where(
                                enemy =>
                                enemy.IsValidTarget(Eq.Range) && Game.CursorPos.Distance(enemy.ServerPosition) < 300))
                {
                    UseQe(enemy);
                }
            }

            if (Config.Item("Key.Combo").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            else
            {
                if (Config.Item("Key.Harass").GetValue<KeyBind>().Active
                    || Config.Item("Key.HarassT").GetValue<KeyBind>().Active) Harass();

                var lc = Config.Item("Key.Lane").GetValue<KeyBind>().Active;
                if (lc || Config.Item("FreezeActive").GetValue<KeyBind>().Active) Farm(lc);

                if (Config.Item("Key.Jungle").GetValue<KeyBind>().Active) JungleFarm();
            }

            if (Config.Item("Key.InstantQE").GetValue<KeyBind>().Active)
            {
                InstantQe2Enemy();
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            //Draw the ranges of the spells.
            var menuItem = Config.Item("QERange").GetValue<Circle>();
            if (menuItem.Active) Render.Circle.DrawCircle(Player.Position, Eq.Range, menuItem.Color);

            foreach (var spell in SpellList)
            {
                menuItem = Config.Item(spell.Slot + "Range").GetValue<Circle>();
                if (menuItem.Active)
                {
                    Render.Circle.DrawCircle(Player.Position, spell.Range, menuItem.Color);
                }
            }

            if (OrbManager.WObject(false) != null)
            Render.Circle.DrawCircle(OrbManager.WObject(false).Position, 100, System.Drawing.Color.White);
        }

        public static bool BuffCheck(Obj_AI_Base enemy)
        {
            var buff = 0;
            if (enemy.HasBuff("UndyingRage") && Config.Item("DontRbuffUndying").GetValue<bool>()) buff++;
            if (enemy.HasBuff("JudicatorIntervention") && Config.Item("DontRbuffJudicator").GetValue<bool>()) buff++;
            if (enemy.HasBuff("ZacRebirthReady") && Config.Item("DontRbuffZac").GetValue<bool>()) buff++;
            if (enemy.HasBuff("AttroxPassiveReady") && Config.Item("DontRbuffAttrox").GetValue<bool>()) buff++;
            if (enemy.HasBuff("Spell Shield") && Config.Item("DontRbuffSivir").GetValue<bool>()) buff++;
            if (enemy.HasBuff("Black Shield") && Config.Item("DontRbuffMorgana").GetValue<bool>()) buff++;
            if (enemy.HasBuff("Chrono Shift") && Config.Item("DontRbuffZilean").GetValue<bool>()) buff++;
            if (enemy.HasBuff("Ferocious Howl") && Config.Item("DontRbuffAlistar").GetValue<bool>()) buff++;

            return buff <= 0;
        }
        public static float GetComboDamage(Obj_AI_Base enemy, bool UseQ, bool UseW, bool UseE, bool UseR)
        {
            if (enemy == null)
                return 0f;
            var damage = 0d;
            var combomana = 0d;
            var useR = Config.Item("DontR" + enemy.BaseSkinName) != null &&
                       Config.Item("DontR" + enemy.BaseSkinName).GetValue<bool>() == false;

            //Add R Damage
            if (R.IsReady() && UseR && useR)
            {
                combomana += ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).SData.Mana;
                if (combomana <= ObjectManager.Player.Mana) damage += GetRDamage(enemy);
                else combomana -= ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).SData.Mana;
            }

            //Add Q Damage
            if (Q.IsReady() && UseQ)
            {
                combomana += ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;
                if (combomana <= ObjectManager.Player.Mana) damage += ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.Q);
                else combomana -= ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;
            }

            //Add E Damage
            if (E.IsReady() && UseE)
            {
                combomana += ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).SData.Mana;
                if (combomana <= ObjectManager.Player.Mana) damage += ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.E);
                else combomana -= ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).SData.Mana;
            }

            //Add W Damage
            if (W.IsReady() && UseW)
            {
                combomana += ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
                if (combomana <= ObjectManager.Player.Mana) damage += ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.W);
                else combomana -= ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
            }

            return (float)damage;
        }
        public static float overkillcheckv2(Obj_AI_Base target)
        {
            double dmg = 0;
            if (Q.IsReady())
                dmg += Q.GetDamage(target);
            if (E.IsReady())
                dmg += E.GetDamage(target);
            if (ObjectManager.Player.Distance(target.Position) <= 550)
                dmg += ObjectManager.Player.GetAutoAttackDamage(target);
            if (W.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                W.GetDamage(target);

            return (float)dmg;
        }
        public static double GetRDamage(Obj_AI_Base enemy)
        {
            if (!R.IsReady()) return 0f;
            double damage = 0;
            if (IgniteSlot.IsReady())
                damage += GetIgniteDamage(enemy);
            if (R.IsReady())
                damage += Math.Min(7, ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Ammo) *
                    ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.R, 1); ;
            return damage;
        }

        

        public static float GetIgniteDamage(Obj_AI_Base enemy)
        {
            if (IgniteSlot == SpellSlot.Unknown || ObjectManager.Player.Spellbook.CanUseSpell(IgniteSlot) != SpellState.Ready)
                return 0f;
            return (float)ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
        }
        public static bool DetectCollision(GameObject target)
        {
            if (_yasuoWall == null || !Config.Item("YasuoWall").GetValue<bool>() ||
                !ObjectManager.Get<AIHeroClient>()
                    .Any(h => h.ChampionName == "Yasuo" && h.IsEnemy && h.IsVisible && !h.IsDead)) return true;

            var level = _yasuoWall.Name.Substring(_yasuoWall.Name.Length - 6, 1);
            var wallWidth = (300 + 50 * Convert.ToInt32(level));
            var wallDirection = (_yasuoWall.Position.To2D() - _yasuoWallCastedPos).Normalized().Perpendicular();
            var wallStart = _yasuoWall.Position.To2D() + ((int)(wallWidth / 2)) * wallDirection;
            var wallEnd = wallStart - wallWidth * wallDirection;

            var intersection = wallStart.Intersection(wallEnd, ObjectManager.Player.Position.To2D(), target.Position.To2D());

            return !intersection.Point.IsValid() || !(Environment.TickCount + Game.Ping + R.Delay - _wallCastT < 4000);

        }
    }
}