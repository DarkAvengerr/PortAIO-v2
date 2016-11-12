using System;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Printing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;
using Color = System.Drawing.Color;
using JustMalphite;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace JustMalphite
{
    internal class Program
    {
        public const string ChampName = "Malphite";
        public static HpBarIndicator Hpi = new HpBarIndicator();
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Smite;

        //Credits to Kurisu for Smite Stuff :^)
        public static readonly int[] SmitePurple = {3713, 3726, 3725, 3726, 3723};
        public static readonly int[] SmiteGrey = {3711, 3722, 3721, 3720, 3719};
        public static readonly int[] SmiteRed = {3715, 3718, 3717, 3716, 3714};
        public static readonly int[] SmiteBlue = {3706, 3710, 3709, 3708, 3707};
        private static SpellSlot Ignite;
        private static SpellSlot smiteSlot;
        private static readonly AIHeroClient player = ObjectManager.Player;

        public static void Main()
        {
            OnLoad();

        }

        private static void OnLoad()
        {
            if (player.ChampionName != ChampName)
                return;

            Notifications.AddNotification("JustMalphite - [V.1.0.3.0]", 8000);

            //Ability Information - Range - Variables.
            Q = new Spell(SpellSlot.Q, 625);
            W = new Spell(SpellSlot.W, 125);
            E = new Spell(SpellSlot.E, 375);
            R = new Spell(SpellSlot.R, 1000);
            R.SetSkillshot(0.00f, 160, 700, false, SkillshotType.SkillshotCircle);


            Config = new Menu(player.ChampionName, player.ChampionName, true);

            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //Combo menu:
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseW", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseR", "Use R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("manualr", "Cast R Manual").SetValue(new KeyBind('R', KeyBindType.Press)));
            Config.SubMenu("Combo").AddItem(new MenuItem("RHit", "Cast R if Hit X Enemies").SetValue(new Slider(2, 1, 5)));

            //Harass menu:
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("hQ", "Use Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("hW", "Use W").SetValue(false));
            Config.SubMenu("Harass").AddItem(new MenuItem("hE", "Use E").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("harassmana", "Mana Percentage").SetValue(new Slider(30, 0, 100)));
            Config.SubMenu("Harass").AddItem(new MenuItem("AutoHarass", "Toggle Auto-Harass", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Toggle)));
            Config.SubMenu("Harass").AddItem(new MenuItem("AutoHarass.Q", "Auto-Use Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("AutoHarass.E", "Auto-Use E").SetValue(true));

            //Farming menu:
            Config.AddSubMenu(new Menu("Clear", "Clear"));
            Config.SubMenu("Clear").AddItem(new MenuItem("laneQ", "Use Q").SetValue(false));
            Config.SubMenu("Clear").AddItem(new MenuItem("laneW", "Use W").SetValue(false));
            Config.SubMenu("Clear").AddItem(new MenuItem("laneE", "Use E").SetValue(false));
            Config.SubMenu("Clear").AddItem(new MenuItem("laneclearmana", "Mana Percentage").SetValue(new Slider(30, 0, 100)));

            //KS menu:
            Config.AddSubMenu(new Menu("KS", "KS"));
            Config.SubMenu("KS").AddItem(new MenuItem("ksQ", "Use Q For KS").SetValue(true));
            Config.SubMenu("KS").AddItem(new MenuItem("ksR", "Use R For KS").SetValue(false));

            //Misc Menu:
            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("interrupt", "Use R to Interrupt").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("tower", "Auto R Under Tower").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("antigap", "Anti Gapcloser with Q")).SetValue(true);
            Config.SubMenu("Misc").AddItem(new MenuItem("DrawD", "Damage Indicator").SetValue(true));

            //Drawings menu:
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("Draw_Disabled", "Disable All Spell Drawings").SetValue(false));
            Config.SubMenu("Drawings").AddItem(new MenuItem("Qdraw", "Draw Q Range").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("Wdraw", "Draw W Range").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("Edraw", "Draw E Range").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("Rdraw", "Draw R Range").SetValue(true));

            Config.AddToMainMenu();
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnEndScene += OnEndScene;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && sender.IsValidTarget(R.Range) && Config.Item("interrupt").GetValue<bool>())
                R.CastIfHitchanceEquals(sender, HitChance.High);
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Q.IsReady() && gapcloser.Sender.IsValidTarget(Q.Range) && Config.Item("antigap").GetValue<bool>())
                Q.CastOnUnit(gapcloser.Sender);
        }
        
        public static string GetSmiteType()
        {
            if (SmiteBlue.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmiteplayerganker";
            }
            if (SmiteRed.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmiteduel";
            }
            if (SmiteGrey.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmitequick";
            }
            if (SmitePurple.Any(id => Items.HasItem(id)))
            {
                return "itemsmiteaoe";
            }
            return "summonersmite";
        }

        private static void OnEndScene(EventArgs args)
        {
            if (Config.SubMenu("Misc").Item("DrawD").GetValue<bool>())
            {
                foreach (var enemy in
                    ObjectManager.Get<AIHeroClient>().Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                {
                    Hpi.unit = enemy;
                    Hpi.drawDmg(CalcDamage(enemy), Color.Green);
                }
            }
        }

        private static void combo()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
                return;

            if (Q.IsReady() && Config.Item("UseQ").GetValue<bool>() && target.IsValidTarget(Q.Range))
                Q.Cast(target);

            if (W.IsReady() && target.IsValidTarget(W.Range) && Config.Item("UseW").GetValue<bool>())
                W.Cast();

            if (E.IsReady() && target.IsValidTarget(E.Range) && Config.Item("UseE").GetValue<bool>())
                E.Cast();

            var enemys = Config.Item("RHit").GetValue<Slider>().Value;
            if (R.IsReady() && Config.Item("UseR").GetValue<bool>() && (Config.Item("Rene").GetValue<Slider>().Value <= enemys) && target.IsValidTarget(R.Range))
            {
                var pred = R.GetPrediction(target).Hitchance;
                if (pred >= HitChance.High)
                    R.Cast(target);
            }
            
            if (Config.Item("ManualR").GetValue<bool>() && R.IsReady() && target.IsValidTarget(R.Range))
            {
                var pred = R.GetPrediction(target).Hitchance;
                if (pred >= HitChance.High)
                    R.Cast(target);
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                items();
        }

        private static int CalcDamage(Obj_AI_Base target)
        {
            var aa = player.GetAutoAttackDamage(target, true)*(1 + player.Crit);
            var damage = aa;
            Ignite = player.GetSpellSlot("summonerdot");

            if (Ignite.IsReady())
                damage += player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            if (Items.HasItem(3153) && Items.CanUseItem(3153))
                damage += player.GetItemDamage(target, Damage.DamageItems.Botrk); //ITEM BOTRK

            if (Items.HasItem(3144) && Items.CanUseItem(3144))
                damage += player.GetItemDamage(target, Damage.DamageItems.Bilgewater); //ITEM BOTRK

            if (R.IsReady() && Config.Item("UseR").GetValue<bool>()) // rdamage
            {
                if (R.IsReady())
                {
                    damage += R.GetDamage(target);
                }
            }

            if (Q.IsReady() && Config.Item("UseQ").GetValue<KeyBind>().Active) // qdamage
            {

                damage += Q.GetDamage(target);
            }

            if (W.IsReady() && Config.Item("UseW").GetValue<KeyBind>().Active) // wdamage
            {

                damage += E.GetDamage(target);
            }
            return (int) damage;
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void UnderTower()
        {
            var Target = TargetSelector.GetTarget(R.Range + R.Width, TargetSelector.DamageType.Magical);

            if (LeagueSharp.Common.Utility.UnderTurret(Target, false) && R.IsReady() && Config.Item("tower").GetValue<bool>())
            {
                var pred = R.GetPrediction(Target).Hitchance;
                if (pred >= HitChance.High)
                    R.Cast(Target);
            }
        }

        private static void Killsteal()
        {
            foreach (AIHeroClient target in
                ObjectManager.Get<AIHeroClient>()
                    .Where(
                        hero =>
                            hero.IsValidTarget(Q.Range) && !hero.HasBuffOfType(BuffType.Invulnerability) && hero.IsEnemy)
                )
            {
                var qDmg = player.GetSpellDamage(target, SpellSlot.Q);
                if (Config.Item("ksQ").GetValue<bool>() && target.IsValidTarget(Q.Range) && target.Health <= qDmg)
                {
                    Q.CastOnUnit(target);
                }
                var rDmg = player.GetSpellDamage(target, SpellSlot.R);
                if (Config.Item("ksR").GetValue<bool>() && target.IsValidTarget(R.Range) && target.Health <= rDmg)
                {
                    var pred = R.GetPrediction(target).Hitchance;
                    if (pred >= HitChance.High)
                        R.Cast(target);
                }
            }
        }

        private static void items()
        {
            Ignite = player.GetSpellSlot("summonerdot");
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget())
                return;

            var botrk = ItemData.Blade_of_the_Ruined_King.GetItem();
            var Ghost = ItemData.Youmuus_Ghostblade.GetItem();
            var cutlass = ItemData.Bilgewater_Cutlass.GetItem();

            if (botrk.IsReady() && botrk.IsOwned(player) && botrk.IsInRange(target)
                && target.HealthPercent <= Config.Item("eL").GetValue<Slider>().Value
                && Config.Item("UseBOTRK").GetValue<bool>())

                botrk.Cast(target);

            if (botrk.IsReady() && botrk.IsOwned(player) && botrk.IsInRange(target)
                && target.HealthPercent <= Config.Item("oL").GetValue<Slider>().Value
                && Config.Item("UseBOTRK").GetValue<bool>())

                botrk.Cast(target);

            if (cutlass.IsReady() && cutlass.IsOwned(player) && cutlass.IsInRange(target) &&
                target.HealthPercent <= Config.Item("HLe").GetValue<Slider>().Value
                && Config.Item("UseBilge").GetValue<bool>())

                cutlass.Cast(target);

            if (Ghost.IsReady() && Ghost.IsOwned(player) && target.IsValidTarget(E.Range)
                && Config.Item("useGhostblade").GetValue<bool>())

                Ghost.Cast();

            if (player.Distance(target.Position) <= 600 && IgniteDamage(target) >= target.Health &&
                Config.Item("UseIgnite").GetValue<bool>())
                player.Spellbook.CastSpell(Ignite, target);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            Killsteal();

            if (Config.Item("tower").GetValue<bool>())
                UnderTower();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
            }

            var autoHarass = Config.Item("AutoHarass", true).GetValue<KeyBind>().Active;
            if (autoHarass)
               AutoHarass();
        }

        private static void AutoHarass()
        {
            if (player.IsDead || MenuGUI.IsChatOpen || player.IsRecalling())
            {
                return;
            }

            var qTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var eTarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (qTarget == null || !qTarget.IsValid)
                return;

            var useQ = Config.Item("AutoHarass.Q").GetValue<bool>();
            var useE = Config.Item("AutoHarass.E").GetValue<bool>();
            var playerMana = Config.Item("harassmana").GetValue<Slider>().Value;

            if (player.Mana < playerMana)
                return;

            if (useQ && Q.IsReady() && Q.IsInRange(qTarget))
            {
                Q.CastOnUnit(qTarget);
            }

            if (useE && E.IsReady() && E.IsInRange(eTarget) && eTarget != null)
            {
                E.Cast(eTarget);
            }
        }

        private static void harass()
        {
            var harassmana = Config.Item("harassmana").GetValue<Slider>().Value;
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
                return;

            if (Q.IsReady()
                && Config.Item("hQ").GetValue<bool>()
                && target.IsValidTarget(Q.Range)
                && player.ManaPercent >= harassmana)

                Q.CastOnUnit(target);

            if (W.IsReady()
                && Config.Item("hW").GetValue<bool>()
                && target.IsValidTarget(W.Range)
                && player.ManaPercent >= harassmana)

                W.Cast();

            if (E.IsReady()
                && Config.Item("hE").GetValue<bool>()
                && target.IsValidTarget(E.Range)
                && player.ManaPercent >= harassmana)

                E.Cast(target);
        }

        private static void Clear()
        {
            var minionObj = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly,MinionOrderTypes.MaxHealth);
            var lanemana = Config.Item("laneclearmana").GetValue<Slider>().Value;

            if (!minionObj.Any())
            {
                return;
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Config.Item("laneE").GetValue<bool>()
                && player.ManaPercent >= lanemana &&
                (minionObj.Count > 1 || minionObj.Any(i => i.MaxHealth >= 1200)))
            {
                var pos = E.GetCircularFarmLocation(minionObj);
                if (pos.MinionsHit > 0 && E.Cast(pos.Position))
                {
                    return;
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Config.Item("laneQ").GetValue<bool>()
                && player.ManaPercent >= lanemana)
            {
                var pos = Q.GetLineFarmLocation(minionObj.Where(i => Q.IsInRange(i)).ToList());
                if (pos.MinionsHit > 0 && Q.Cast(pos.Position))
                {
                    return;
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                && Config.Item("laneW").GetValue<bool>()
                && player.ManaPercent >= lanemana)
            {
                var obj = minionObj.Where(i => W.IsInRange(i)).FirstOrDefault(i => i.MaxHealth >= 1200);
                if (obj == null)
                {
                    obj = minionObj.Where(i => W.IsInRange(i)).MinOrDefault(i => i.Health);
                }
                if (obj != null)
                {
                    W.Cast();
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Config.Item("Draw_Disabled").GetValue<bool>())
                return;

            if (Config.Item("Qdraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, Q.Range, System.Drawing.Color.White, 3);
            if (Config.Item("Wdraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, W.Range, System.Drawing.Color.White, 3);
            if (Config.Item("Edraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, E.Range, System.Drawing.Color.White, 3);
            if (Config.Item("Rdraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, R.Range, System.Drawing.Color.White, 3);

            var orbtarget = Orbwalker.GetTarget();
            Render.Circle.DrawCircle(orbtarget.Position, 100, Color.DarkOrange, 10);
        }



        public static void UseSmiteOnChamp(AIHeroClient target)
        {
            if (target.IsValidTarget(E.Range) && smiteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell((smiteSlot)) == SpellState.Ready &&
                (GetSmiteType() == "s5_summonersmiteplayerganker" ||
                 GetSmiteType() == "s5_summonersmiteduel"))
            {
                ObjectManager.Player.Spellbook.CastSpell(smiteSlot, target);
            }
        }

        public static void GetSmiteSlot()
        {
            foreach (
                var spell in
                    ObjectManager.Player.Spellbook.Spells.Where(
                        spell => String.Equals(spell.Name, GetSmiteType(), StringComparison.CurrentCultureIgnoreCase)))
            {
                smiteSlot = spell.Slot;
                Smite = new Spell(smiteSlot, 700);
                return;
            }
        }
    }
}
