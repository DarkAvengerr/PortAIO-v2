using System;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Printing;
using System.Linq;
using LeagueSharp;
using ItemData = LeagueSharp.Common.Data.ItemData;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using Color = System.Drawing.Color;
using JustWukong;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace JustWukong
{
    internal class Program
    {
        public const string ChampName = "MonkeyKing";
        public const string Menuname = "JustWukong";
        public static HpBarIndicator Hpi = new HpBarIndicator();
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q, W, E, R;
        private static SpellSlot _smiteSlot = SpellSlot.Unknown;
        private static Spell _smite;
        //Credits to Kurisu
        private static readonly int[] SmitePurple = { 3713, 3726, 3725, 3726, 3723 };
        private static readonly int[] SmiteGrey = { 3711, 3722, 3721, 3720, 3719 };
        private static readonly int[] SmiteRed = { 3715, 3718, 3717, 3716, 3714 };
        private static readonly int[] SmiteBlue = { 3706, 3710, 3709, 3708, 3707 };
        private static SpellSlot Ignite;
        private static readonly AIHeroClient player = ObjectManager.Player;

        public static void Main()
        {
            OnLoad();

        }

        private static void OnLoad()
        {
            if (player.ChampionName != ChampName)
                return;

            Notifications.AddNotification("JustWukong - [V.1.0.1.0]", 8000);

            //Ability Information - Range - Variables.
            Q = new Spell(SpellSlot.Q, 375f);
            W = new Spell(SpellSlot.W, 0f);
            E = new Spell(SpellSlot.E, 640f);
            R = new Spell(SpellSlot.R, 375f);
           
            SetSmiteSlot();

            Config = new Menu(Menuname, Menuname, true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //Combo
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseW", "Use W").SetValue(false));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseR", "Use R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseS", "Use Smite (Red/Blue)").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Rene", "Min Enemies for R").SetValue(new Slider(2, 1, 5)));

            //Harass
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("hQ", "Use Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("hW", "Use W").SetValue(false));
            Config.SubMenu("Harass").AddItem(new MenuItem("hE", "Use E").SetValue(true));
           // Config.SubMenu("Harass").AddItem(new MenuItem("AutoHarass", "Auto Harass", true).SetValue(new KeyBind("J".ToCharArray()[0], KeyBindType.Toggle)));
           // Config.SubMenu("Harass").AddItem(new MenuItem("aQ", "Use Q for Auto Harass").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("harassmana", "Mana Percentage").SetValue(new Slider(30, 0, 100)));

            //Item
            Config.AddSubMenu(new Menu("Item", "Item"));
            Config.SubMenu("Item").AddItem(new MenuItem("useGhostblade", "Use Youmuu's Ghostblade").SetValue(true));
            Config.SubMenu("Item").AddItem(new MenuItem("UseBOTRK", "Use Blade of the Ruined King").SetValue(true));
            Config.SubMenu("Item").AddItem(new MenuItem("eL", "  Enemy HP Percentage").SetValue(new Slider(80, 0, 100)));
            Config.SubMenu("Item").AddItem(new MenuItem("oL", "  Own HP Percentage").SetValue(new Slider(65, 0, 100)));
            Config.SubMenu("Item").AddItem(new MenuItem("UseBilge", "Use Bilgewater Cutlass").SetValue(true));
            Config.SubMenu("Item")
                .AddItem(new MenuItem("HLe", "  Enemy HP Percentage").SetValue(new Slider(80, 0, 100)));
            Config.SubMenu("Item").AddItem(new MenuItem("UseIgnite", "Use Ignite").SetValue(true));

            //Laneclear
            Config.AddSubMenu(new Menu("Clear", "Clear"));
            Config.SubMenu("Clear").AddItem(new MenuItem("laneQ", "Use Q").SetValue(true));
            Config.SubMenu("Clear").AddItem(new MenuItem("laneE", "Use E").SetValue(true));
            Config.SubMenu("Clear").AddItem(new MenuItem("emin", "Min Minion for E").SetValue(new Slider(2, 1, 5)));
            Config.SubMenu("Clear").AddItem(new MenuItem("laneclearmana", "Mana Percentage").SetValue(new Slider(30, 0, 100)));

            //Draw
            Config.AddSubMenu(new Menu("Draw", "Draw"));
            Config.SubMenu("Draw").AddItem(new MenuItem("Draw_Disabled", "Disable All Spell Drawings").SetValue(false));
            Config.SubMenu("Draw").AddItem(new MenuItem("Qdraw", "Draw Q Range").SetValue(true));
            Config.SubMenu("Draw").AddItem(new MenuItem("Wdraw", "Draw W Range").SetValue(true));
            Config.SubMenu("Draw").AddItem(new MenuItem("Edraw", "Draw E Range").SetValue(true));
            Config.SubMenu("Draw").AddItem(new MenuItem("Rdraw", "Draw R Range").SetValue(true));

            //Flee
            Config.AddSubMenu(new Menu("Flee", "Flee"));
           // Config.SubMenu("Flee").AddItem(new MenuItem("Flee", "Flee Key").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));
          //  Config.SubMenu("Flee").AddItem(new MenuItem("fW", "Flee with W").SetValue(false));
            Config.SubMenu("Flee").AddItem(new MenuItem("fE", "Flee with Q/E [ SOON™ ]"));
            
            //Misc
            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("KsQ", "Killsteal with Q").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("KsE", "Killsteal with E").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("tower", "Auto R Under Tower").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("DrawD", "Damage Indicator").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("interrupt", "Interrupt Spells").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("antigap", "AntiGapCloser").SetValue(true));

            Config.AddToMainMenu();
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Spellbook.OnCastSpell += OnCastSpell;
            Chat.Print(
                "<font color=\"#FFCC00\">JustWukong - <font color=\"#FFFFFF\"> Latest Version Successfully Loaded.</font>");
            Drawing.OnEndScene += OnEndScene;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (R.IsReady() && sender.IsValidTarget(R.Range) && Config.Item("interrupt").GetValue<bool>())
                R.Cast();
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (W.IsReady() && gapcloser.Sender.IsValidTarget(Q.Range) && Config.Item("antigap").GetValue<bool>())
                W.Cast();
        }

        public static string Smitetype()
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
            if (Config.SubMenu("Draw").Item("DrawD").GetValue<bool>())
            {
                foreach (var enemy in
                    ObjectManager.Get<AIHeroClient>().Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                {
                    Hpi.unit = enemy;
                    Hpi.drawDmg(CalcDamage(enemy), Color.Green);
                }
            }
        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E))
            {
                if (player.HasBuff("MonkeyKingSpinToWin"))
                args.Process = false;
            }
        }

        private static void combo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget())
                return;

            if (E.IsReady() && target.IsValidTarget(E.Range) && Config.Item("UseE").GetValue<bool>())
                E.CastOnUnit(target);

            UseSmite(target);

            var enemys = Config.Item("Rene").GetValue<Slider>().Value;
            if (R.IsReady() && Config.Item("UseR").GetValue<bool>() && target.IsValidTarget(R.Range))
                if (Config.Item("Rene").GetValue<Slider>().Value <= enemys)
                R.Cast();
            
            if (Q.IsReady() && Config.Item("UseQ").GetValue<bool>() && target.IsValidTarget(Q.Range))
                Q.Cast();
            
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                items();

            if (W.IsReady() && Config.Item("UseW").GetValue<bool>())
                W.Cast();
        }
        
        private static int CalcDamage(Obj_AI_Base target)
        {
            var aa = player.GetAutoAttackDamage(target, true) * (1 + player.Crit);
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

            if (E.IsReady() && Config.Item("UseE").GetValue<KeyBind>().Active) // edamage
            {

                damage += E.GetDamage(target);
            }

            if (_smite.IsReady() && Config.Item("UseS").GetValue<KeyBind>().Active) // edamage
            {

                damage += GetSmiteDmg();
            }

           return (int)damage;
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float)player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
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
                    Q.Cast();
                }
                var eDmg = player.GetSpellDamage(target, SpellSlot.E);
                if (Config.Item("ksE").GetValue<bool>() && target.IsValidTarget(E.Range) && target.Health <= eDmg)
                {
                    E.CastOnUnit(target);
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

        private static void UnderTower()
        {
            var Target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);

            if (LeagueSharp.Common.Utility.UnderTurret(Target, false) && R.IsReady() && Config.Item("tower").GetValue<bool>())
            {
                R.Cast();
            }
        }
        
        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (player.IsDead || MenuGUI.IsChatOpen || player.IsRecalling())
            {
                return;
            }

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

            Killsteal();
            UnderTower();
            //Flee();
           }
        
        private static void harass()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            var harassmana = Config.Item("harassmana").GetValue<Slider>().Value;
            if (target == null || !target.IsValidTarget())
                return;

            if (E.IsReady() && Config.Item("hE").GetValue<bool>() && target.IsValidTarget(E.Range) &&
                player.ManaPercent >= harassmana)
                E.CastOnUnit(target);

            if (Q.IsReady() && Config.Item("hQ").GetValue<bool>() && target.IsValidTarget(Q.Range) && player.ManaPercent >= harassmana)
                Q.Cast();
        }

        private static void Clear()
        {
            var minionObj = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly,
                MinionOrderTypes.MaxHealth);

            if (!minionObj.Any())
            {
                return;
            }

            if (minionObj.Count > Config.Item("emin").GetValue<Slider>().Value)
            {
                var minions = minionObj[2];
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Config.Item("laneE").GetValue<bool>())
                {
                    E.Cast(minions);
                }
            }
            
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Config.Item("laneQ").GetValue<bool>())
            {
                var pos = Q.GetLineFarmLocation(minionObj.Where(i => Q.IsInRange(i)).ToList());
                if (pos.MinionsHit > 0 && Q.Cast())
                {
                    return;
                }
            }
       
        }

        private static void Flee()
        {
            var farthestMinion = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly,
                MinionOrderTypes.MaxHealth).OrderByDescending(x => x.Distance(ObjectManager.Player.Position));
            var minionObj = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly,
                MinionOrderTypes.MaxHealth);
           
            if (EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos))
            if (W.IsReady() && Config.Item("fW").GetValue<bool>())
            {
                W.Cast(Game.CursorPos);
            }
        }
        
        private static void OnDraw(EventArgs args)
        {
            if (Config.Item("Draw_Disabled").GetValue<bool>())
                return;

            if (Config.Item("Qdraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, Q.Range, System.Drawing.Color.White, 3);
            if (Config.Item("Edraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, E.Range, System.Drawing.Color.White, 3);
            if (Config.Item("Rdraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, R.Range, System.Drawing.Color.White, 3);
        }

        //Credits to metaphorce
        public static void UseSmite(AIHeroClient target)
        {
            var usesmite = Config.Item("UseS").GetValue<bool>();
            var itemscheck = SmiteBlue.Any(i => Items.HasItem(i)) || SmiteRed.Any(i => Items.HasItem(i));
            if (itemscheck && usesmite &&
                ObjectManager.Player.Spellbook.CanUseSpell(_smiteSlot) == SpellState.Ready &&
                target.Distance(player.Position) < _smite.Range)
            {
                ObjectManager.Player.Spellbook.CastSpell(_smiteSlot, target);
            }
        }

        private static void SetSmiteSlot()
        {
            foreach (
                var spell in
                    ObjectManager.Player.Spellbook.Spells.Where(
                        spell => String.Equals(spell.Name, Smitetype(), StringComparison.CurrentCultureIgnoreCase)))
            {
                _smiteSlot = spell.Slot;
                _smite = new Spell(_smiteSlot, 700);
                return;
            }
        }

        private static int GetSmiteDmg()
        {
            int level = player.Level;
            int index = player.Level / 5;
            float[] dmgs = { 370 + 20 * level, 330 + 30 * level, 240 + 40 * level, 100 + 50 * level };
            return (int)dmgs[index];
        }
    }
}
