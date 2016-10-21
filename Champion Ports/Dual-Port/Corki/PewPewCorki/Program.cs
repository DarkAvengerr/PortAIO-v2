using System;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using ItemData = LeagueSharp.Common.Data.ItemData;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PewPewCorki
{
    internal class Program
    {
        public const string ChampName = "Corki";
        public static HpBarIndicator Hpi = new HpBarIndicator();
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell RB;
        private static SpellSlot Ignite;
        private static readonly AIHeroClient player = ObjectManager.Player;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (player.ChampionName != ChampName)
                return;

            Notifications.AddNotification("PewPewCorki Loaded!", 5000);
            Notifications.AddNotification("Latest Changes:", 5000);
            Notifications.AddNotification("First Release!", 5000);

            Q = new Spell(SpellSlot.Q, 800);
            Q.SetSkillshot(0.50f, 250f, 1135f, false,
                SkillshotType.SkillshotCircle);


            //RocketJump Settings//Corki Swag   //Note to self, remember to use castposition - 250/-500 depending on distance target to player.Pos
            W = new Spell(SpellSlot.W, 800);
            W.SetSkillshot(0.25f, 450, 1200, false, SkillshotType.SkillshotLine);

            E = new Spell(SpellSlot.E, 600);
            E.SetSkillshot(0, 25f, float.MaxValue, false, SkillshotType.SkillshotCone);


            R = new Spell(SpellSlot.R, 1300);
            R.SetSkillshot(0.25f, 75f, 2000f, true,
                SkillshotType.SkillshotLine);

            RB = new Spell(SpellSlot.R, 1500);
            RB.SetSkillshot(0.25f, 100f, 2000f, true,
                 SkillshotType.SkillshotLine);


            Config = new Menu("PewPewCorki", "Corki", true);
            Orbwalker = new Orbwalking.Orbwalker(Config.AddSubMenu(new Menu("[PewPew]: Orbwalker", "Orbwalker")));
            TargetSelector.AddToMenu(Config.AddSubMenu(new Menu("[PewPew]: Target Selector", "Target Selector")));

            //COMBOMENU

            Config.AddItem(new MenuItem("PewPew", "            PewPew Prediction Settings"));

            Config.AddItem(new MenuItem("HitchanceQ", "[Q] Hitchance").SetValue(
                    new StringList(
                        new[] { HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(), HitChance.VeryHigh.ToString() }, 3)));

            Config.AddItem(new MenuItem("HitchanceR", "[R] Hitchance").SetValue(
                    new StringList(
                        new[] { HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(), HitChance.VeryHigh.ToString() }, 3)));

            var combo = Config.AddSubMenu(new Menu("[PewPew]: Combo Settings", "Combo Settings"));
            var harass = Config.AddSubMenu(new Menu("[PewPew]: Harass Settings", "Harass Settings"));
            var drawing = Config.AddSubMenu(new Menu("[PewPew]: Draw Settings", "Draw"));

            combo.SubMenu("[SBTW] ManaManager").AddItem(new MenuItem("qmana", "[Q] Mana %").SetValue(new Slider(10, 100, 0)));
            combo.SubMenu("[SBTW] ManaManager").AddItem(new MenuItem("wmana", "[W] Mana %").SetValue(new Slider(10, 100, 0)));
            combo.SubMenu("[SBTW] ManaManager").AddItem(new MenuItem("emana", "[E] Mana %").SetValue(new Slider(10, 100, 0)));
            combo.SubMenu("[SBTW] ManaManager").AddItem(new MenuItem("rmana", "[R] Mana %").SetValue(new Slider(15, 100, 0)));


            //Prediction


            combo.SubMenu("[Q] Settings").AddItem(new MenuItem("UseQ", "Use [Q]").SetValue(true));

            combo.SubMenu("[W] Settings").AddItem(new MenuItem("UseWz", "Coming Soon").SetValue(true));
            //combo.SubMenu("[W] Settings").AddItem(new MenuItem("wnear", "Enemy Count").SetValue(new Slider(2, 5, 1)));
            //combo.SubMenu("[W] Settings").AddItem(new MenuItem("whp", "Own HP %").SetValue(new Slider(75, 100, 0)));
            //combo.SubMenu("[W] Settings").AddItem(new MenuItem("wturret", "Don't fly into turret range").SetValue(true));


            combo.SubMenu("[E] Settings").AddItem(new MenuItem("UseE", "Use [E]").SetValue(true));

            combo.SubMenu("[R] Settings").AddItem(new MenuItem("UseR", "Use R").SetValue(true));
            combo.SubMenu("[R] Settings").AddItem(new MenuItem("manualr", "Cast R on your target").SetValue(new KeyBind('R', KeyBindType.Press)));


            combo.SubMenu("Item Settings")
                .AddItem(new MenuItem("useGhostblade", "Use Youmuu's Ghostblade").SetValue(true));
            combo.SubMenu("Item Settings")
                .AddItem(new MenuItem("UseBOTRK", "Use Blade of the Ruined King").SetValue(true));
            combo.SubMenu("Item Settings")
                .AddItem(new MenuItem("eL", "  Enemy HP Percentage").SetValue(new Slider(80, 100, 0)));
            combo.SubMenu("Item Settings")
                .AddItem(new MenuItem("oL", "  Own HP Percentage").SetValue(new Slider(65, 100, 0)));
            combo.SubMenu("Item Settings").AddItem(new MenuItem("UseBilge", "Use Bilgewater Cutlass").SetValue(true));
            combo.SubMenu("Item Settings")
                .AddItem(new MenuItem("HLe", "  Enemy HP Percentage").SetValue(new Slider(80, 100, 0)));
            combo.SubMenu("Summoner Settings").AddItem(new MenuItem("UseIgnite", "Use Ignite").SetValue(true));


            Config.SubMenu("[PewPew]: Killsteal Settings").AddItem(new MenuItem("smartKS", "Use SmartKS").SetValue(true));
            Config.SubMenu("[PewPew]: Killsteal Settings").AddItem(new MenuItem("KSQ", "Use [Q] Killsteal").SetValue(true));
            Config.SubMenu("[PewPew]: Killsteal Settings").AddItem(new MenuItem("KSR", "Use [R] Killsteal").SetValue(true));

            //LANECLEARMENU
            Config.SubMenu("[PewPew]: Laneclear Settings").AddItem(new MenuItem("Lasthitonly", "Only use Abilities to last hit").SetValue(false));
            Config.SubMenu("[PewPew]: Laneclear Settings").AddItem(new MenuItem("laneQ", "Use Q").SetValue(true));
            Config.SubMenu("[PewPew]: Laneclear Settings").SubMenu("[Q] Settings").AddItem(new MenuItem("laneQhit", "Q Hitcount").SetValue(new Slider(3, 10, 0)));
            Config.SubMenu("[PewPew]: Laneclear Settings").AddItem(new MenuItem("laneE", "Use E").SetValue(true));
            Config.SubMenu("[PewPew]: Laneclear Settings").AddItem(new MenuItem("laneR", "Use R").SetValue(true));
            Config.SubMenu("[PewPew]: Laneclear Settings").SubMenu("[R] Settings").AddItem(new MenuItem("laneRhit", "R Hitcount").SetValue(new Slider(2, 10, 0)));
            Config.SubMenu("[PewPew]: Laneclear Settings").SubMenu("[R] Settings").AddItem(new MenuItem("lanerockets", "Use [R] if Rocket Ammo >").SetValue(new Slider(0, 7, 0)));
            Config.SubMenu("[PewPew]: Laneclear Settings").AddItem(new MenuItem("laneclearmana", "Mana Percentage").SetValue(new Slider(30, 100, 0)));


            Config.SubMenu("[PewPew]: Laneclear Settings").AddItem(new MenuItem("playerlevel", "Don't use abilities till level").SetValue(new Slider(12, 18, 0)));

            //JUNGLEFARMMENU
            Config.SubMenu("[PewPew]: Jungle Settings").AddItem(new MenuItem("jungleQ", "Use [Q]").SetValue(true));
            Config.SubMenu("[PewPew]: Jungle Settings").AddItem(new MenuItem("jungleE", "Use [E]").SetValue(true));
            Config.SubMenu("[PewPew]: Jungle Settings").AddItem(new MenuItem("jungleR", "Use [R]").SetValue(true));

            Config.SubMenu("[PewPew]: Jungle Settings").SubMenu("[R] Settings").AddItem(new MenuItem("junglerockets", "Use [R] if Rocket Ammo >").SetValue(new Slider(0, 7, 0)));

            Config.SubMenu("[PewPew]: Jungle Settings").AddItem(new MenuItem("jungleclearmana", "Mana Percentage").SetValue(new Slider(30, 100, 0)));

            drawing.AddItem(new MenuItem("Draw_Disabled", "Disable All Spell Drawings").SetValue(false));
           // drawing.AddItem(new MenuItem("renderwpos", "Draw [W] Cast Position").SetValue(true));
            drawing.AddItem(new MenuItem("drawRstacks", "Draw Rocket Ammunition").SetValue(true));
            drawing.AddItem(new MenuItem("Qdraw", "Draw Q Range").SetValue(new Circle(true, Color.Orange)));
            drawing.AddItem(new MenuItem("Wdraw", "Draw W Range").SetValue(new Circle(true, Color.DarkOrange)));
            drawing.AddItem(new MenuItem("Edraw", "Draw E Range").SetValue(new Circle(true, Color.AntiqueWhite)));
            drawing.AddItem(new MenuItem("Rdraw", "Draw R Range").SetValue(new Circle(true, Color.LawnGreen)));
            drawing.AddItem(new MenuItem("CircleThickness", "Circle Thickness").SetValue(new Slider(7, 30, 0)));

            harass.AddItem(new MenuItem("AutoHarass", "AutoHarass (TOGGLE)").SetValue(new KeyBind('L', KeyBindType.Toggle)));
            harass.AddItem(new MenuItem("harassQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("harassE", "Use E").SetValue(true));

            harass.AddItem(new MenuItem("harassR", "Use R").SetValue(true));
            harass.SubMenu("[R] Settings").AddItem(new MenuItem("harassrockets", "Use [R] if Rocket Ammo >").SetValue(new Slider(2, 7, 0)));

            harass.AddItem(new MenuItem("harassmana", "Mana Percentage").SetValue(new Slider(30, 100, 0)));

            Config.SubMenu("[PewPew]: Misc Settings").AddItem(new MenuItem("DrawD", "Damage Indicator").SetValue(true));

            Config.AddToMainMenu();

            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnEndScene += OnEndScene;
        }

        private static void OnEndScene(EventArgs args)
        {
            if (Config.SubMenu("[PewPew]: Misc Settings").Item("DrawD").GetValue<bool>())
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
            var target = TargetSelector.GetTarget(R.Range * 2, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget())
                return;

            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                qlogic();

            var emana = Config.Item("emana").GetValue<Slider>().Value;

            if (E.IsReady() && Config.Item("UseE").GetValue<bool>()
            && player.ManaPercent >= emana)
                E.Cast(target);


            var wmana = Config.Item("wmana").GetValue<Slider>().Value;

            if (R.IsReady() && target.IsValidTarget(R.Range))
                rlogic();

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                items();

            if (Config.Item("wturret").GetValue<bool>() && target.Position.UnderTurret(true))
                return;
            if (target.HasBuff("deathdefiedbuff"))
                return;
            if (target.HasBuff("KogMawIcathianSurprise", true))
                return;
            if (target.IsInvulnerable)
                return;



            //Experimental needs some more test1ng
            //var wcastPosition = player.ServerPosition.Extend(target.Position, player.ServerPosition.Distance(target.Position) - Orbwalking.GetRealAutoAttackRange(player));


            //if (W.IsReady() && target.IsValidTarget(W.Range)
            //&& Config.Item("UseW").GetValue<bool>()
            ////&& target.Position.CountEnemiesInRange(700) <= Config.Item("wnear").GetValue<Slider>().Value
            //&& player.HealthPercent >= Config.Item("whp").GetValue<Slider>().Value
            //&& player.ManaPercent >= wmana && !IsWall(wcastPosition) && player.Position.Distance(target.Position) > Orbwalking.GetRealAutoAttackRange(player) + 150)

               // W.Cast(wcastPosition);
        }
        private static bool IsWall(Vector3 pos)
        {
            CollisionFlags cFlags = NavMesh.GetCollisionFlags(pos);
            return (cFlags == CollisionFlags.Wall);
        }

        private static int CalcDamage(Obj_AI_Base target)
        {
            //Calculate Combo Damage
            var aa = player.GetAutoAttackDamage(target, true)*(1 + player.Crit);
            var damage = aa*2;
            Ignite = player.GetSpellSlot("summonerdot");

            if (Ignite.IsReady())
                damage += player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            if (Items.HasItem(3153) && Items.CanUseItem(3153))
                damage += player.GetItemDamage(target, Damage.DamageItems.Botrk); //ITEM BOTRK

            if (Items.HasItem(3144) && Items.CanUseItem(3144))
                damage += player.GetItemDamage(target, Damage.DamageItems.Bilgewater); //ITEM BOTRK

            if (Config.Item("UseE").GetValue<bool>()) // edamage
            {
                if (E.IsReady())
                {
                    damage += E.GetDamage(target);
                }
            }

            if (R.IsReady() && Config.Item("UseR").GetValue<bool>()) // rdamage
            {

                damage += R.GetDamage(target);
            }

            if (W.IsReady() && Config.Item("UseW").GetValue<bool>())
            {
                damage += Q.GetDamage(target);
            }
            return (int) damage;
        }

        private static HitChance PewPewPredQ(string name)
        {
            var qpred = Config.Item(name).GetValue<StringList>();
            switch (qpred.SList[qpred.SelectedIndex])
            {
                case "Low":
                    return HitChance.Low;
                case "Medium":
                    return HitChance.Medium;
                case "High":
                    return HitChance.High;
                case "Very High":
                    return HitChance.VeryHigh;
            }
            return HitChance.VeryHigh;
        }

        private static HitChance PewPewPredR(string name)
        {
            var qpred = Config.Item(name).GetValue<StringList>();
            switch (qpred.SList[qpred.SelectedIndex])
            {
                case "Low":
                    return HitChance.Low;
                case "Medium":
                    return HitChance.Medium;
                case "High":
                    return HitChance.High;
                case "Very High":
                    return HitChance.VeryHigh;
            }
            return HitChance.VeryHigh;
        }
        private static void qlogic()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (Q.IsReady() && Config.Item("UseQ").GetValue<bool>() && Q.GetPrediction(target).Hitchance >= PewPewPredQ("HitchanceQ"))
                Q.Cast(target);
        }




        private static void rlogic()
        {
            var rmana = Config.Item("rmana").GetValue<Slider>().Value;
            var target = TargetSelector.GetTarget(RB.Range, TargetSelector.DamageType.Magical);
            var RBR = player.HasBuff("corkimissilebarragecounterbig");
            // var rstacks =
            if (target == null || !target.IsValidTarget())
                return;

            if (Config.Item("manualr").GetValue<KeyBind>().Active && R.IsReady())
                R.Cast(target);

            if (RBR && Config.Item("UseR").GetValue<bool>() && R.IsReady() &&
                R.GetPrediction(target).Hitchance >= PewPewPredR("HitchanceR"))
                RB.Cast(target);
            
            if (Config.Item("UseR").GetValue<bool>() && R.IsReady() && R.GetPrediction(target).Hitchance >= PewPewPredR("HitchanceR") && !RBR &&
                player.ManaPercent >= rmana)
                R.Cast(target);
        }
        private static float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float)player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
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
                && player.HealthPercent <= Config.Item("oL").GetValue<Slider>().Value
                && Config.Item("UseBOTRK").GetValue<bool>())

                botrk.Cast(target);

            if (cutlass.IsReady() && cutlass.IsOwned(player) && cutlass.IsInRange(target) &&
                target.HealthPercent <= Config.Item("HLe").GetValue<Slider>().Value
                && Config.Item("UseBilge").GetValue<bool>())

                cutlass.Cast(target);

            if (Ghost.IsReady() && Ghost.IsOwned(player) && target.IsValidTarget(E.Range)
                && Config.Item("useGhostblade").GetValue<bool>())

                Ghost.Cast();

            if (player.Distance(target) <= 600 && IgniteDamage(target) >= target.Health &&
                Config.Item("UseIgnite").GetValue<bool>())
                player.Spellbook.CastSpell(Ignite, target);
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Laneclear();
                    Jungleclear();
                    break;
            }
            if (Config.Item("AutoHarass").GetValue<KeyBind>().Active)
                harass();
            if (Config.Item("SmartKS").GetValue<bool>())
                killsteal();
        }

        private static void harass()
        {
            var harassmana = Config.Item("harassmana").GetValue<Slider>().Value;
            var target = TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(player), TargetSelector.DamageType.Physical);
            if (target == null)
                return;

            if (E.IsReady()
                && Config.Item("harassE").GetValue<bool>()
                && player.ManaPercent >= harassmana)

                E.Cast(target);

            if (Q.IsReady()
                && Config.Item("harassQ").GetValue<bool>()
                && target.IsValidTarget(Q.Range)
                && player.ManaPercent >= harassmana && Q.GetPrediction(target).Hitchance >= PewPewPredQ("HitchanceQ"))

                Q.Cast(target);


            if (player.Spellbook.GetSpell(SpellSlot.R).Ammo < Config.Item("harassrockets").GetValue<Slider>().Value)
                return;

            if (R.IsReady()
            && Config.Item("harassR").GetValue<bool>()
            && target.IsValidTarget(R.Range)
            && player.ManaPercent >= harassmana && R.GetPrediction(target).Hitchance >= PewPewPredR("HitchanceR"))
                R.Cast(target);
        }

        private static void killsteal()
        {
            foreach (var enemy in
                ObjectManager.Get<AIHeroClient>()
                    .Where(x => x.IsValidTarget(R.Range))
                    .Where(x => !x.IsZombie)
                    .Where(x => !x.IsDead))
            {
                var qdmg = Q.GetDamage(enemy);
                var rdmg = R.GetDamage(enemy);


                if (Config.Item("KSQ").GetValue<bool>() && Q.IsReady() && enemy.Health < qdmg &&
                    enemy.IsValidTarget(Q.Range) && Q.GetPrediction(enemy).Hitchance >= PewPewPredQ("HitchanceQ"))
                    Q.Cast(enemy);

                if (Config.Item("KSR").GetValue<bool>() && R.IsReady() && enemy.Health < rdmg &&
                    enemy.IsValidTarget(R.Range) && R.GetPrediction(enemy).Hitchance >= PewPewPredQ("HitchanceQ"))
                    R.Cast(enemy);

                if (Config.Item("KSQ").GetValue<bool>() && Config.Item("KSR").GetValue<bool>() && Q.IsReady() &&
                    enemy.Health < qdmg + rdmg && enemy.IsValidTarget(Q.Range) &&
                    Q.GetPrediction(enemy).Hitchance >= PewPewPredQ("HitchanceQ")
                    && Q.GetPrediction(enemy).Hitchance >= PewPewPredQ("HitchanceR"))
                {
                    Q.Cast(enemy);
                    R.Cast(enemy);
                }
            }
        }

        private static void Laneclear2()
        {
            if (player.Level < Config.Item("playerlevel").GetValue<Slider>().Value)
                return;
            var lanemana = Config.Item("laneclearmana").GetValue<Slider>().Value;
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + Q.Width);
            var allMinionsR = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, R.Range + R.Width);

            var Qfarmpos = Q.GetCircularFarmLocation(allMinionsQ, Q.Width);
            var Rfarmpos = R.GetCircularFarmLocation(allMinionsR, R.Width);

            if (player.Spellbook.IsAutoAttacking)
                return;

            foreach (var minion in allMinionsQ)
            {
                float predictedHealtMinionq = HealthPrediction.GetHealthPrediction(minion,
                    (int) (Q.Delay + (player.Distance(minion.ServerPosition)/Q.Speed)));

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                 && Qfarmpos.MinionsHit >= Config.Item("laneQhit").GetValue<Slider>().Value && allMinionsQ.Count >= 1
                 && Config.Item("laneQ").GetValue<bool>()
                 && player.ManaPercent >= lanemana && Q.GetDamage(minion) > predictedHealtMinionq && Qfarmpos.Position.Distance(minion.Position) < Q.Width)
                    Q.Cast(Qfarmpos.Position);
            }

            if (player.Spellbook.GetSpell(SpellSlot.R).Ammo < Config.Item("lanerockets").GetValue<Slider>().Value)
                return;

            foreach (var minionR in allMinionsR)
                if (Rfarmpos.MinionsHit >= Config.Item("laneRhit").GetValue<Slider>().Value
                    && allMinionsR.Count >= 1 && Config.Item("laneR").GetValue<bool>()
                    && player.ManaPercent >= lanemana && R.GetPrediction(minionR).Hitchance >= PewPewPredR("HitchanceR") && R.GetDamage(minionR) > minionR.Health 
                    && Rfarmpos.Position.Distance(minionR.Position) < Q.Width)
                    R.Cast(minionR);
        }
        private static void Laneclear()
        {
            if (player.Level < Config.Item("playerlevel").GetValue<Slider>().Value)
                return;
            if (player.Spellbook.IsAutoAttacking)
                return;
            var lanemana = Config.Item("laneclearmana").GetValue<Slider>().Value;
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + Q.Width);
            var allMinionsR = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, R.Range + R.Width);
            var allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range + W.Width - 50);

            var Qfarmpos = Q.GetCircularFarmLocation(allMinionsQ, Q.Width);
            var Efarmpos = E.GetCircularFarmLocation(allMinionsE, E.Width);
            var Rfarmpos = R.GetCircularFarmLocation(allMinionsR, R.Width);

            if (Config.Item("Lasthitonly").GetValue<bool>())
                Laneclear2();

            if (Config.Item("Lasthitonly").GetValue<bool>())
                return;


            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                && Qfarmpos.MinionsHit >= Config.Item("laneQhit").GetValue<Slider>().Value && allMinionsQ.Count >= 1
                && Config.Item("laneQ").GetValue<bool>()
                && player.ManaPercent >= lanemana)

                Q.Cast(Qfarmpos.Position);

               foreach (var minion in allMinionsE)
                   if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                       && minion.IsValidTarget(E.Range) && Efarmpos.MinionsHit >= 2
                       && allMinionsE.Count >= 2 && Config.Item("laneE").GetValue<bool>()
                       && player.ManaPercent >= lanemana)

                       E.Cast(minion);

               if (player.Spellbook.GetSpell(SpellSlot.R).Ammo < Config.Item("lanerockets").GetValue<Slider>().Value)
                   return;

               foreach (var minionR in allMinionsR)
                   if (Rfarmpos.MinionsHit >= Config.Item("laneRhit").GetValue<Slider>().Value
                       && allMinionsR.Count >= 1 && Config.Item("laneR").GetValue<bool>()
                       && player.ManaPercent >= lanemana && R.GetPrediction(minionR).Hitchance >= PewPewPredR("HitchanceR"))
                       R.Cast(minionR);

        }
        private static void Jungleclear()
        {
            var jlanemana = Config.Item("jungleclearmana").GetValue<Slider>().Value;
            var MinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + Q.Width, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var MinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range + E.Width, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var MinionsR = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, R.Range + R.Width, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            var Qfarmpos = Q.GetCircularFarmLocation(MinionsQ, Q.Width);
            var Efarmpos = E.GetCircularFarmLocation(MinionsE, Q.Width);
            var Rfarmpos = R.GetLineFarmLocation(MinionsR, R.Width);


            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                && Qfarmpos.MinionsHit >= 1
                && MinionsE.Count >= 1 && Config.Item("jungleQ").GetValue<bool>()
                && player.ManaPercent >= jlanemana)

                Q.Cast(Qfarmpos.Position);

            foreach (var minion in MinionsE)
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && minion.IsValidTarget(E.Range)
                    && Efarmpos.MinionsHit >= 1
                    && MinionsE.Count >= 1
                    && Config.Item("jungleE").GetValue<bool>()
                    && player.ManaPercent >= jlanemana)

                    E.Cast(minion);

            if (player.Spellbook.GetSpell(SpellSlot.R).Ammo < Config.Item("junglerockets").GetValue<Slider>().Value)
                return;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                && Rfarmpos.MinionsHit >= 1
                 && MinionsE.Count >= 1 && Config.Item("jungleR").GetValue<bool>()
                 && player.ManaPercent >= jlanemana)

                R.Cast(Rfarmpos.Position);
        }

        private static void OnDraw(EventArgs args)
        {
            {

            }

            //Draw Skill Cooldown on Champ

            if (Config.Item("Draw_Disabled").GetValue<bool>())
                return;

            if (Config.Item("Qdraw").GetValue<Circle>().Active)
                if (Q.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Q.IsReady() ? Config.Item("Qdraw").GetValue<Circle>().Color : Color.Red,
                                                        Config.Item("CircleThickness").GetValue<Slider>().Value);


            if (Config.Item("Wdraw").GetValue<Circle>().Active)
                if (W.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, W.IsReady() ? Config.Item("Wdraw").GetValue<Circle>().Color : Color.Red,
                                                        Config.Item("CircleThickness").GetValue<Slider>().Value);

            if (Config.Item("Edraw").GetValue<Circle>().Active)
                if (E.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range - 1,
                        E.IsReady() ? Config.Item("Edraw").GetValue<Circle>().Color : Color.Red,
                                                        Config.Item("CircleThickness").GetValue<Slider>().Value);

            if (Config.Item("Rdraw").GetValue<Circle>().Active)
                if (R.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range - 150,
                        R.IsReady() ? Config.Item("Rdraw").GetValue<Circle>().Color : Color.Red,
                                                        Config.Item("CircleThickness").GetValue<Slider>().Value);

            var pos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            if (Config.Item("AutoHarass").GetValue<KeyBind>().Active)
                Drawing.DrawText(pos.X - 50, pos.Y + 30, System.Drawing.Color.Plum, "AutoHarass Enabled");

            var zpos = Drawing.WorldToScreen(player.Position);
            var rstacks = player.Spellbook.GetSpell(SpellSlot.R).Ammo;
            if (Config.Item("drawRstacks").GetValue<bool>() && R.Level >= 1)
                Drawing.DrawText(zpos.X - 50, zpos.Y + 50, System.Drawing.Color.Gold,
                    "[R] stacks = " + rstacks.ToString());

            //var target = TargetSelector.GetTarget(R.Range * 2,
            //TargetSelector.DamageType.Magical);
            //var wcastPosition = player.ServerPosition.Extend(target.Position, player.ServerPosition.Distance(target.Position) - Orbwalking.GetRealAutoAttackRange(player));

           // if (Config.Item("renderwpos").GetValue<bool>() && player.Position.Distance(target.Position) > Orbwalking.GetRealAutoAttackRange(player) + 150)
                //Render.Circle.DrawCircle(wcastPosition, player.BoundingRadius, Color.DarkBlue, 10);
        }
    }
}
