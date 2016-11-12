using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BloodMoonAkali
{
    internal class Program
    {
        public const string ChampName = "Akali";
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q, W, E, R;
        private static SpellSlot Ignite;
        public static HpBarIndicator Hpi = new HpBarIndicator();
        private static readonly AIHeroClient Player = ObjectManager.Player;

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != ChampName)
                return;

            Notifications.AddNotification("Bloodmoon Akali Loaded!", 1000);

            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 300);
            R = new Spell(SpellSlot.R, 700);

            Q.SetTargetted(Q.Instance.SData.SpellCastTime, Q.Instance.SData.MissileSpeed);
            E.SetSkillshot(E.Instance.SData.SpellCastTime, E.Instance.SData.LineWidth, E.Instance.SData.MissileSpeed,false, SkillshotType.SkillshotCircle);

            Config = new Menu("BloodMoonAkali", "Akali", true);
            Orbwalker = new Orbwalking.Orbwalker(Config.AddSubMenu(new Menu("[Orbwalker]", "Orbwalker")));
            TargetSelector.AddToMenu(Config.AddSubMenu(new Menu("[Target Selector]", "Target Selector")));

            var combo = Config.AddSubMenu(new Menu("[Combo Settings]", "Combo Settings"));
            var harass = Config.AddSubMenu(new Menu("[Harass Settings]", "Harass Settings"));
            var killsteal = Config.AddSubMenu(new Menu("[Killsteal Settings]", "Killsteal Settings"));
            var laneclear = Config.AddSubMenu(new Menu("[Laneclear Settings]", "Laneclear Settings"));
            var lasthit = Config.AddSubMenu(new Menu("[Lasthit Settings]", "Lasthit Settings"));
            var jungleclear = Config.AddSubMenu(new Menu("[Jungle Settings]", "Jungle Settings"));
            var misc = Config.AddSubMenu(new Menu("[Misc Settings]", "Misc Settings"));
            var drawing = Config.AddSubMenu(new Menu("[Draw Settings]", "Draw Settings"));

            //Advanced Settings //[E] Settings

            //[W] Settings
            combo.SubMenu("[Advanced Features W]").AddItem(new MenuItem("WHP", "Use [W] on < % HP ").SetValue(false));
            combo.SubMenu("[Advanced Features W]").AddItem(new MenuItem("WHPslider", "% HP").SetValue(new Slider(35, 100, 0)));
            combo.SubMenu("[Advanced Features W]").AddItem(new MenuItem("WHE", "Use [W] X amount of enemies ").SetValue(false));
            combo.SubMenu("[Advanced Features W]").AddItem(new MenuItem("WHEslider", "Enemy Count").SetValue(new Slider(3, 5, 1)));

            //[R] Settings
            //combo.SubMenu("[Advanced Features R]").AddItem(new MenuItem("Rkill", "Only use [R] if Killable").SetValue(true));
            combo.SubMenu("[Advanced Features R]").AddItem(new MenuItem("rendposcheck", "Use [R] End Position Check").SetValue(true));
            combo.SubMenu("[Advanced Features R]").AddItem(new MenuItem("rturret", "Don't R into Turret Range if HP below %").SetValue(false));
            combo.SubMenu("[Advanced Features R]").AddItem(new MenuItem("rturrethp", "% HP").SetValue(new Slider(70, 100, 0)));
            combo.SubMenu("[Advanced Features R]").AddItem(new MenuItem("rturrete", "Don't R into Turret Range if target HP above %").SetValue(false));
            combo.SubMenu("[Advanced Features R]").AddItem(new MenuItem("rturrethpe", "% HP").SetValue(new Slider(40, 100, 0)));
            //combo.SubMenu("[Advanced Features R]").AddItem(new MenuItem("Rcheck", "Don't [R] into X amount of enemies").SetValue(false));
            //combo.SubMenu("[Advanced Features R]").AddItem(new MenuItem("eslider", "Enemy Count").SetValue(new Slider(3, 5, 0)));
            combo.SubMenu("[Advanced Features R]").AddItem(new MenuItem("miniongapclose", "Use Minion to gapclose? [Requires 2 R stacks]").SetValue(true));
            combo.SubMenu("[Advanced Features R]").AddItem(new MenuItem("herogapclose", "Use Enemy Champion to gapclose? [Requires 2 R stacks]").SetValue(true));
            //combo.SubMenu("[Advanced Features R]").AddItem(new MenuItem("rangeRslider", "[R] gapclose Range").SetValue(new Slider(350, 600, 0)));

            //ITEMS
            combo.SubMenu("[Item Settings]").AddItem(new MenuItem("UseItems", "Use Offensive Items").SetValue(true));
            combo.SubMenu("[Item Settings]").AddItem(new MenuItem("UseBOTRK", "Use Blade of the Ruined King").SetValue(true));
            combo.SubMenu("[Item Settings]").AddItem(new MenuItem("eL", "  Enemy HP Percentage").SetValue(new Slider(80, 100, 0)));
            combo.SubMenu("[Item Settings]").AddItem(new MenuItem("oL", "  Own HP Percentage").SetValue(new Slider(65, 100, 0)));
            combo.SubMenu("[Item Settings]").AddItem(new MenuItem("UseHEX", "Use Hextech Gunblade").SetValue(true));
            combo.SubMenu("[Item Settings]").AddItem(new MenuItem("eH", "  Enemy HP Percentage").SetValue(new Slider(80, 100, 0)));
            combo.SubMenu("[Item Settings]").AddItem(new MenuItem("UseBILGE", "Use Bilgewater Cutlass").SetValue(true));
            combo.SubMenu("[Item Settings]").AddItem(new MenuItem("HLe", "  Enemy HP Percentage").SetValue(new Slider(80, 100, 0)));
            combo.SubMenu("[Item Settings]").AddItem(new MenuItem("UseTIAMAT", "Use Tiamat").SetValue(true));
            combo.SubMenu("[Item Settings]").AddItem(new MenuItem("UseHYDRA", "Use Hydra").SetValue(true));

            //Combo Settings
            combo.AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("UseW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("UseR", "Use R").SetValue(true));
            combo.AddItem(new MenuItem("UseIgnite", "Use Ignite").SetValue(true));

            //HARASS
            harass.AddItem(new MenuItem("HarassToggle", "Harass Toggle").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Toggle)));
            harass.AddItem(new MenuItem("HarassQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("HarassQEnergy", " % Energy").SetValue(new Slider(50, 100, 0)));
            harass.AddItem(new MenuItem("HarassE", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("HarassEEnergy", " % Energy").SetValue(new Slider(50, 100, 0)));

            //LASTHIT
            lasthit.AddItem(new MenuItem("LastHitQ", "Lasthit with Q").SetValue(true));
            lasthit.AddItem(new MenuItem("LastHitE", "Lasthit with E").SetValue(true));
            lasthit.AddItem(new MenuItem("LastHitQA", "Calculate Q Mark damage in LastHit").SetValue(true));

            //LANECLEAR
            laneclear.AddItem(new MenuItem("LaneClearQ", "Laneclear with Q").SetValue(true));
            laneclear.AddItem(new MenuItem("LaneClearE", "Laneclear with E").SetValue(false));
            laneclear.AddItem(new MenuItem("LaneClearCount", "Minion HitCount").SetValue(new Slider(3, 10, 0)));
            laneclear.AddItem(new MenuItem("LaneClearEnergy", "% Energy").SetValue(new Slider(50, 100, 0)));
            laneclear.AddItem(new MenuItem("LaneClearOnlyQE", "Only use Abilities if minion is killable").SetValue(true));
            laneclear.AddItem(new MenuItem("LaneClearQA", "Calculate Q Mark damage in LaneClear").SetValue(true));           

            jungleclear.AddItem(new MenuItem("JungleClearQ", "Jungleclear with Q").SetValue(true));
            jungleclear.AddItem(new MenuItem("JungleClearE", "Jungleclear with E").SetValue(true));

            //KILLSTEAL
            killsteal.AddItem(new MenuItem("KS", "Use Smart Killsteal").SetValue(true));
            killsteal.AddItem(new MenuItem("KSQ", "Use Q").SetValue(true));
            killsteal.AddItem(new MenuItem("KSE", "Use E").SetValue(true));
            killsteal.AddItem(new MenuItem("KSR", "Use R").SetValue(false));
            killsteal.AddItem(new MenuItem("KSIngite", "Use Ingite").SetValue(true));

            //DRAWING
            drawing.AddItem(new MenuItem("Draw_Disabled", "Disable All Spell Drawings").SetValue(false));
            drawing.SubMenu("Spell Drawings").AddItem(new MenuItem("Qdraw", "Draw Q Range").SetValue(new Circle(true, System.Drawing.Color.IndianRed)));
            drawing.SubMenu("Spell Drawings").AddItem(new MenuItem("Wdraw", "Draw W Range").SetValue(new Circle(true, System.Drawing.Color.IndianRed)));
            drawing.SubMenu("Spell Drawings").AddItem(new MenuItem("Edraw", "Draw E Range").SetValue(new Circle(true, System.Drawing.Color.IndianRed)));
            drawing.SubMenu("Spell Drawings").AddItem(new MenuItem("Rdraw", "Draw R Range").SetValue(new Circle(true, System.Drawing.Color.IndianRed)));
            //drawing.SubMenu("Spell Drawings").AddItem(new MenuItem("RGdraw", "Draw R Gapclose Range").SetValue(new Circle(true, System.Drawing.Color.IndianRed)));
            drawing.SubMenu("Spell Drawings").AddItem(new MenuItem("CircleThickness", "Circle Thickness").SetValue(new Slider(7, 30, 0)));
            drawing.SubMenu("Misc Drawings").AddItem(new MenuItem("drawdmg", "Draw Damage on Enemy HPbar").SetValue(true));
            drawing.SubMenu("Misc Drawings").AddItem(new MenuItem("drawpotential", "Draw Potential Combo DMG").SetValue(true));
            drawing.SubMenu("Misc Drawings").AddItem(new MenuItem("drawRstacks", "Draw R stacks").SetValue(true));
            drawing.SubMenu("Misc Drawings").AddItem(new MenuItem("rlines", "Draw R Gapclose Lines").SetValue(true));
            drawing.SubMenu("Misc Drawings").AddItem(new MenuItem("drawRend", "Draw R End Position").SetValue(true));

            //MISC
            misc.AddItem(new MenuItem("AntiGapW", "Anti-Gapcloser [W]").SetValue(true));
            //misc.SubMenu("Trinket Settings").AddItem(new MenuItem("AutoRedTrinket", "Auto Buy Sweeper").SetValue(false));
            //misc.SubMenu("Trinket Settings").AddItem(new MenuItem("AutoRedTrinketLevel", "Buy Sweeper at level").SetValue(new Slider(6, 18, 1)));
            //misc.SubMenu("Trinket Settings").AddItem(new MenuItem("AutoRedTrinketUpgrade", "Auto upgrade sweeper (Oracle Lens 250 Gold)").SetValue(false));
            //misc.SubMenu("Trinket Settings").AddItem(new MenuItem("AutoRedTrinketUpgradeLevel", "Upgrade sweeper at level").SetValue(new Slider(9, 18, 9)));


            Config.AddToMainMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += OrbwalkingAfterAttack;
          //Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
            Drawing.OnDraw += Gapcloserdraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;


        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Player.IsDead || gapcloser.Sender.IsInvulnerable)
                return;

            var targetpos = Drawing.WorldToScreen(gapcloser.Sender.Position);
            if (gapcloser.Sender.IsValidTarget(Q.Range * 2) && Config.Item("AntiGap").GetValue<bool>())
            {
                Render.Circle.DrawCircle(gapcloser.Sender.Position, gapcloser.Sender.BoundingRadius, System.Drawing.Color.DeepPink);
                Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, System.Drawing.Color.MediumPurple, "GAPCLOSER!");
            }

            if (W.IsReady() && gapcloser.End.Distance(Player.Position) <= 300 &&
                Config.Item("AntiGapW").GetValue<bool>())
                W.Cast();

        }

        private static void OnEndScene(EventArgs args)
        {
            if (Config.Item("drawdmg").GetValue<bool>())
            {
                foreach (var enemy in
                    ObjectManager.Get<AIHeroClient>().Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                {
                    Hpi.unit = enemy;
                    Hpi.drawDmg(CalcDamage(enemy), System.Drawing.Color.IndianRed);

                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {        


            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    //IF PLAYER WHOULD ENTER COMBO ORBWALKER MODE IT WILL USE SAID COMBO
                    Clogic();
                    WLogic();
                    Elogic();
                    Rlogic();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    Lasthit();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    Jungleclear();
                    break;
            }
            if (Config.Item("HarassToggle").IsActive())
            {
                Harass();
            }
            if (Config.Item("KS").GetValue<bool>())
            {
                SmartKS();
            }
            if (Config.Item("AntiZ").GetValue<bool>() ||
                Config.Item("AntiC").GetValue<bool>())
            {
                OnReceiveBuff();
            }
            //if (Config.Item("AutoRedTrinket").GetValue<bool>())
            //{
            //    AutoRedTrinket();
           // }
            //if (Config.Item("AutoRedTrinketUpgrade").GetValue<bool>())
            //{
             //   AutoRedTrinketUpgrade();
            //}
            var zpos = Drawing.WorldToScreen(Player.Position);
            var rstacks = Player.Buffs.Find(buff => buff.Name == "AkaliShadowDance").Count;
            if (Config.Item("drawRstacks").GetValue<bool>())
                Drawing.DrawText(zpos.X - 50, zpos.Y + 50, System.Drawing.Color.HotPink,
                    "[R] stacks = " + rstacks.ToString());

        }

        //IGNITEDAMAGE
        private static float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static int CalcDamage(Obj_AI_Base target)
        {
            //Calculate Combo Damage
            var aa = Player.GetAutoAttackDamage(target, true);
            var damage = aa;
            var markdmg = Player.CalcDamage(target, Damage.DamageType.Magical,
                (45 + 35 * Q.Level + 0.5 * Player.FlatMagicDamageMod));
            if (Items.HasItem(3153) && Items.CanUseItem(3153))
                damage += Player.GetItemDamage(target, Damage.DamageItems.Botrk); //Botrk
            if (Items.HasItem(3077) && Items.CanUseItem(3077))
                damage += Player.GetItemDamage(target, Damage.DamageItems.Tiamat); //Tiamat
            if (Items.HasItem(3144) && Items.CanUseItem(3144))
                damage += Player.GetItemDamage(target, Damage.DamageItems.Bilgewater); //Bigle
            if (Items.HasItem(3074) && Items.CanUseItem(3074))
                damage += Player.GetItemDamage(target, Damage.DamageItems.Hydra); //Hydra
            if (Items.HasItem(3144) && Items.CanUseItem(3146))
                damage += Player.GetItemDamage(target, Damage.DamageItems.Hexgun); //Hexblade


            if (E.Level == 1 && E.IsReady() || E.Level == 2 && E.IsReady())
                damage += E.GetDamage(target);

            if (E.Level == 3 && E.IsReady() || E.Level == 4 && E.IsReady())
                damage += E.GetDamage(target) * 2;

            if (E.Level == 5 && E.IsReady())
                damage += E.GetDamage(target) * 3;

            if (R.IsReady()) // rdamage          
                damage += R.GetDamage(target);

            if (target.HasBuff("AkaliMota"))
                damage += markdmg;

            if (Q.IsReady() && !target.HasBuff("AkaliMota"))
                damage += Q.GetDamage(target) + markdmg;

            if (Q.IsReady() && target.HasBuff("AkaliMota"))
                damage += Q.GetDamage(target);

            if (Ignite.IsReady())
                damage += Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            return (int)damage;
        }

        private static int PotentialDmg(AIHeroClient target)
        {
            var aa = Player.GetAutoAttackDamage(target, true);
            var damage = aa;
            var markdmg = Player.CalcDamage(target, Damage.DamageType.Magical,
                (45 + 35 * Q.Level + 0.5 * Player.FlatMagicDamageMod));

            var sheen = LeagueSharp.Common.Data.ItemData.Sheen.GetItem();
            var lichbane = LeagueSharp.Common.Data.ItemData.Lich_Bane.GetItem();
            var triforce = LeagueSharp.Common.Data.ItemData.Trinity_Force.GetItem();

            var sheendmg = Player.CalcDamage(target, Damage.DamageType.Physical,
                (Player.BaseAttackDamage));

            var lichbanedmg = Player.CalcDamage(target, Damage.DamageType.Magical,
                (Player.BaseAttackDamage * 0.75 + 0.5 * Player.FlatMagicDamageMod));

            var triforcedmg = Player.CalcDamage(target, Damage.DamageType.Physical,
                (Player.BaseAttackDamage) * 2);

            if (sheen.IsReady() && !Player.HasBuff("sheen"))
                damage += sheendmg;

            if (Player.HasBuff("sheen"))
                damage += sheendmg;

            if (lichbane.IsReady() && !Player.HasBuff("lichbane"))
                damage += lichbanedmg;

            if (Player.HasBuff("lichbane"))
                damage += lichbanedmg;

            if (triforce.IsReady() && !Player.HasBuff("sheen"))
                damage += triforcedmg;

            if (Player.HasBuff("sheen"))
                damage += triforcedmg;
            
            if (Items.HasItem(3153) && Items.CanUseItem(3153))
                damage += Player.GetItemDamage(target, Damage.DamageItems.Botrk); //Botrk
            if (Items.HasItem(3077) && Items.CanUseItem(3077))
                damage += Player.GetItemDamage(target, Damage.DamageItems.Tiamat); //Tiamat
            if (Items.HasItem(3144) && Items.CanUseItem(3144))
                damage += Player.GetItemDamage(target, Damage.DamageItems.Bilgewater); //Bigle
            if (Items.HasItem(3074) && Items.CanUseItem(3074))
                damage += Player.GetItemDamage(target, Damage.DamageItems.Hydra); //Hydra
            if (Items.HasItem(3144) && Items.CanUseItem(3146))
                damage += Player.GetItemDamage(target, Damage.DamageItems.Hexgun); //Hexblade


            if (E.Level == 1 || E.Level == 2)
                damage += E.GetDamage(target);

            if (Player.TotalAttackDamage > 180)
                damage += aa*3;

            if (E.Level == 3 || E.Level == 4)
                damage += E.GetDamage(target) * 2;

            if (E.Level == 5)
                damage += E.GetDamage(target) * 3;

            if (R.Level >= 1 ) // rdamage          
                damage += R.GetDamage(target);

            if (Q.Level >= 1 && !target.HasBuff("AkaliMota"))
                damage += Q.GetDamage(target) + markdmg;

            if (Q.Level >= 1 && target.HasBuff("AkaliMota"))
                damage += Q.GetDamage(target);


            if (Ignite.IsReady())
                damage += Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);


            return (int)damage;



        }
        private static int ActualDMG(AIHeroClient target)
        {
            var aa = Player.GetAutoAttackDamage(target, true);
            var damage = aa;
            var markdmg = Player.CalcDamage(target, Damage.DamageType.Magical,
                (45 + 35 * Q.Level + 0.5 * Player.FlatMagicDamageMod) + aa);


            if (Items.HasItem(3153) && Items.CanUseItem(3153))
                damage += Player.GetItemDamage(target, Damage.DamageItems.Botrk); //Botrk
            if (Items.HasItem(3077) && Items.CanUseItem(3077))
                damage += Player.GetItemDamage(target, Damage.DamageItems.Tiamat); //Tiamat
            if (Items.HasItem(3144) && Items.CanUseItem(3144))
                damage += Player.GetItemDamage(target, Damage.DamageItems.Bilgewater); //Bigle
            if (Items.HasItem(3074) && Items.CanUseItem(3074))
                damage += Player.GetItemDamage(target, Damage.DamageItems.Hydra); //Hydra
            if (Items.HasItem(3144) && Items.CanUseItem(3146))
                damage += Player.GetItemDamage(target, Damage.DamageItems.Hexgun); //Hexblade

            if (E.IsReady() && target.IsValidTarget(E.Range) && !R.IsReady())
                damage += E.GetDamage(target);

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && !R.IsReady())
                damage += Q.GetDamage(target);

            if (target.HasBuff("AkaliMota"))
                damage += markdmg;

            if (Ignite.IsReady())
                damage += Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

            if (target.Distance(Player.Position) > Orbwalking.GetRealAutoAttackRange(Player))
                damage += -aa;

            //R+Q+E DMG ETC
            if (R.IsReady() && target.IsValidTarget(R.Range) && !E.IsReady() && !Q.IsReady()) // rdamage          
                damage += R.GetDamage(target);

            //RE
            if (R.IsReady() && target.IsValidTarget(R.Range) && E.IsReady() && !Q.IsReady()) // rdamage          
                damage += R.GetDamage(target) + E.GetDamage(target);

            //RQM
            if (R.IsReady() && target.IsValidTarget(R.Range) && !E.IsReady() && Q.IsReady() && !target.HasBuff("AkaliMota")) // rdamage          
                damage += R.GetDamage(target) + Q.GetDamage(target) + markdmg;

            //RQ
            if (R.IsReady() && target.IsValidTarget(R.Range) && E.IsReady() && Q.IsReady() && target.HasBuff("AkaliMota")) // rdamage          
                damage += R.GetDamage(target) + Q.GetDamage(target);

            //REQM
            if (R.IsReady() && target.IsValidTarget(R.Range) && E.IsReady() && Q.IsReady() && !target.HasBuff("AkaliMota")) // rdamage          
                damage += R.GetDamage(target) + E.GetDamage(target) + Q.GetDamage(target) + markdmg;

            //REQ
            if (R.IsReady() && target.IsValidTarget(R.Range) && E.IsReady() && Q.IsReady() && target.HasBuff("AkaliMota")) // rdamage          
                damage += R.GetDamage(target) + E.GetDamage(target) + Q.GetDamage(target);

            // Q & E Real DMG

            return (int)damage;



        }
        private static void Lasthit()
        {
            foreach (var minion in
                ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget() && minion.IsEnemy &&
                                                                   minion.Distance(Player.ServerPosition) <= Q.Range))
            {
                var aa = Player.GetAutoAttackDamage(minion, true);
                var damage = aa;
                var markdmg = Player.CalcDamage(minion, Damage.DamageType.Magical,
                    (45 + 35 * Q.Level + 0.5 * Player.FlatMagicDamageMod) + aa - 15);
                var MinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);

                float predictedHealtMinionq = HealthPrediction.GetHealthPrediction(minion,
                    (int)(R.Delay + (Player.Distance(minion.ServerPosition) / Q.Speed)));


                if (minion.HasBuff("AkaliMota")
                    && minion.Health < markdmg)
                {
                    Orbwalker.ForceTarget(minion);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, minion);
                }
    
                if (minion.Health <= aa + 20 && minion.HasBuff("AkaliMota") && E.IsReady() && minion.Distance(Player.Position) < Orbwalking.GetRealAutoAttackRange(Player))
                    return;
               
                if (minion.HasBuff("AkaliMota") && Config.Item("LastHitQA").GetValue<bool>() && minion.Health <= markdmg
                    && minion.Distance(Player.Position) <= Orbwalking.GetRealAutoAttackRange(Player) && E.IsReady())
                    return;
              
                if (minion.Distance(Player.Position) < Orbwalking.GetRealAutoAttackRange(Player) && Q.IsReady()
                    && Config.Item("LastHitQ").GetValue<bool>() && minion.Health <= aa + 8)
                    return;
            
                if (minion.Distance(Player.Position) < Orbwalking.GetRealAutoAttackRange(Player) && E.IsReady()
                    && Config.Item("LastHitE").GetValue<bool>() && minion.Health <= aa + 8)
                    return;
                
                if (Config.Item("LastHitQ").GetValue<bool>() && predictedHealtMinionq < Q.GetDamage(minion) + 5 && Q.IsReady())
                    Q.Cast(minion);
                
                if (Config.Item("LastHitE").GetValue<bool>() && E.IsReady() && minion.Health < E.GetDamage(minion))
                    E.Cast(minion);
                
                if (Config.Item("LastHitQA").GetValue<bool>())
                {
                    if (Config.Item("LastHitQA").GetValue<bool>() && minion.Health < Q.GetDamage(minion) + markdmg
                        && minion.Distance(Player.Position) < Orbwalking.GetRealAutoAttackRange(Player) && Q.IsReady())
                        Q.Cast(minion);

                    if (minion.HasBuff("AkaliMota")
                        && minion.Health < markdmg && minion.Position.Distance(Player.Position) < Orbwalking.GetRealAutoAttackRange(Player))
                    {
                        Orbwalker.ForceTarget(minion);           
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, minion);
                    }
                }
            }
        }
        private static void LaneClear()
        {
            if (Config.Item("LaneClearOnlyQE").GetValue<bool>())
            {
                foreach (var minion in
               ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget() && minion.IsEnemy &&
                                                                  minion.Distance(Player.ServerPosition) <= Q.Range))
                {
                    var MinionsAll = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, R.Range*2);
                    if (MinionsAll.Count == 0)
                        return;                   

                    var aa = Player.GetAutoAttackDamage(minion, true);
                    var damage = aa;
                    var markdmg = Player.CalcDamage(minion, Damage.DamageType.Magical,
                        (45 + 35 * Q.Level + 0.5 * Player.FlatMagicDamageMod) + aa - 15);
                    var MinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);
                    var laneE = Config.Item("LaneClearEnergy").GetValue<Slider>().Value;

                    float predictedHealtMinionq = HealthPrediction.GetHealthPrediction(minion,
                     (int)(R.Delay + (Player.Distance(minion.ServerPosition) / Q.Speed)));

                    if (minion.Team == GameObjectTeam.Neutral)
                        return;

                    if (Player.Spellbook.IsAutoAttacking)
                        return;

                    if (minion.HasBuff("AkaliMota")
                        && minion.Health < markdmg)
                    {
                        Orbwalker.ForceTarget(minion);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, minion);
                    }
                  
                    if (minion.Health < aa && Q.IsReady() && Config.Item("LaneClearQ").GetValue<bool>()
                        && minion.Distance(Player.Position) < Orbwalking.GetRealAutoAttackRange(Player))
                        return;
                   
                    if (minion.Health <= markdmg && minion.HasBuff("AkaliMota") && E.IsReady() && minion.IsValidTarget(E.Range))
                        return;
                
                    if (minion.Health <= aa && minion.Distance(Player.Position) < Orbwalking.GetRealAutoAttackRange(Player) + 25
                        && E.IsReady() && Config.Item("LaneClearE").GetValue<bool>())
                        return;
              
                    if (Config.Item("LaneClearQ").GetValue<bool>() && Config.Item("LaneClearOnlyQE").GetValue<bool>()
                        && predictedHealtMinionq < Q.GetDamage(minion) + 5 && Q.IsReady())
                        Q.Cast(minion);

                 
                    if (Config.Item("LaneClearQA").GetValue<bool>())
                    {
                        if (Config.Item("LaneClearQA").GetValue<bool>() && Config.Item("LaneClearOnlyQE").GetValue<bool>()
                            && minion.Health < Q.GetDamage(minion) + markdmg
                            && minion.Distance(Player.Position) < Orbwalking.GetRealAutoAttackRange(Player) && Q.IsReady())
                            Q.Cast(minion);

                        if (Config.Item("LaneClearQA").GetValue<bool>() && Config.Item("LaneClearOnlyQE").GetValue<bool>()
                            && minion.HasBuff("AkaliMota") && minion.Health < markdmg)
                        {
                            Orbwalker.ForceTarget(minion);
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, minion);
                        }
                    }
                 
                    if (Config.Item("LaneClearE").GetValue<bool>() && Config.Item("LaneClearOnlyQE").GetValue<bool>()
                        && E.IsReady() && minion.Health < E.GetDamage(minion)
                        && MinionsE.Count  >= Config.Item("LaneClearCount").GetValue<Slider>().Value)
                        E.Cast(minion);
                }
            }

            if (Config.Item("LaneClearOnlyQE").GetValue<bool>())
                return;

            foreach (var minion in
           ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget() && minion.IsEnemy &&
                                                              minion.Distance(Player.ServerPosition) <= Q.Range))
            {
                var aa = Player.GetAutoAttackDamage(minion, true);
                var markdmg = Player.CalcDamage(minion, Damage.DamageType.Magical,
                    (45 + 35 * Q.Level + 0.5 * Player.FlatMagicDamageMod) + aa);
                var MinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);
                var laneE = Config.Item("LaneClearEnergy").GetValue<Slider>().Value;

                float predictedHealtMinionq = HealthPrediction.GetHealthPrediction(minion,
                    (int)(R.Delay + (Player.Distance(minion.ServerPosition) / Q.Speed)));

                if (minion.Team == GameObjectTeam.Neutral)
                    return;

                //Q Logic
                if (minion.HasBuff("AkaliMota") && Q.IsReady() && minion.IsValidTarget(Q.Range))
                    return;

                if (Q.IsReady() && Config.Item("LaneClearQ").GetValue<bool>() && predictedHealtMinionq < Q.GetDamage(minion))
                    Q.Cast(minion);

                if (Q.IsReady() && Config.Item("LaneClearQ").GetValue<bool>() && predictedHealtMinionq < Q.GetDamage(minion))
                    return;
                //Check 1
                if (minion.Health <= aa + 20 && minion.HasBuff("AkaliMota") && E.IsReady() && minion.Distance(Player.Position) < Orbwalking.GetRealAutoAttackRange(Player))
                    return;
                //Check 2
                if (minion.HasBuff("AkaliMota") && Config.Item("LaneClearQA").GetValue<bool>() && minion.Health <= markdmg
                    && minion.Distance(Player.Position) <= Orbwalking.GetRealAutoAttackRange(Player) && E.IsReady())
                    return;
                //Q Cast
                if (Config.Item("LaneClearQ").GetValue<bool>() && Q.IsReady())
                    Q.Cast(minion);
                //Q + Mark
                if (Config.Item("LaneClearQA").GetValue<bool>() && predictedHealtMinionq < Q.GetDamage(minion) + markdmg
                    && minion.Distance(Player.Position) < Orbwalking.GetRealAutoAttackRange(Player) && Q.IsReady())
                    Q.Cast(minion);

                if (Config.Item("LaneClearQA").GetValue<bool>() && minion.HasBuff("AkaliMota")
                    && minion.Health < markdmg)
                {
                    Orbwalker.ForceTarget(minion);
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, minion);
                }
                if (Player.ManaPercent <= laneE)
                    return;
                //E Cast
                if (Config.Item("LaneClearE").GetValue<bool>() && E.IsReady() &&
                    MinionsE.Count >= Config.Item("LaneClearCount").GetValue<Slider>().Value)
                    E.Cast();
            }
        }
        private static bool IsWall(Vector3 pos)
        {
            CollisionFlags cFlags = NavMesh.GetCollisionFlags(pos);
            return (cFlags == CollisionFlags.Wall);
        }
        private static void Jungleclear()
        {
            var MinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range,
                MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var MinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range,
                MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            foreach (var minion in MinionsQ)
                if (Config.Item("JungleClearQ").GetValue<bool>() && Q.IsReady() && MinionsE.Count >= 1)
                    Q.Cast(minion);

            if (Config.Item("JungleClearE").GetValue<bool>() && E.IsReady() && MinionsE.Count >= 1)
                E.Cast();
        }

        private static void SmartKS()
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>()
                .Where(x => x.IsValidTarget(R.Range + 150))
                .Where(x => !x.IsZombie)
                .Where(x => !x.IsDead))
            {
                Ignite = Player.GetSpellSlot("summonerdot");
                var qdmg = Q.GetDamage(enemy);
                var edmg = E.GetDamage(enemy);

                if (IgniteDamage(enemy) >= enemy.Health && Ignite.IsReady()
                    && Config.Item("KSIgnite").GetValue<bool>()
                    && Player.Distance(enemy.Position) <= 600)

                    Player.Spellbook.CastSpell(Ignite, enemy);

                if (enemy.Health < qdmg &&
                    Config.Item("KSQ").GetValue<bool>())

                    Q.Cast(enemy);

                if (enemy.Health < edmg && Config.Item("KSE").GetValue<bool>())
                    E.Cast(enemy);

                if (enemy.Health < edmg + qdmg
                    && Config.Item("KSE").GetValue<bool>() && Config.Item("KSE").GetValue<bool>())
                {
                    E.Cast(enemy);
                    Q.Cast(enemy);
                }

                if (enemy.Health < edmg + qdmg + IgniteDamage(enemy) && Ignite.IsReady()
                    && Config.Item("KSE").GetValue<bool>() && Config.Item("KSE").GetValue<bool>())
                {
                    E.Cast(enemy);
                    Q.Cast(enemy);
                }

                if (enemy.Health < ActualDMG(enemy) && R.IsReady()
                    && Config.Item("KSR").GetValue<bool>())
                {
                    R.Cast(enemy);
                }


            }
        }

        private static void IgniteLogic()
        {

            var target = TargetSelector.GetTarget(R.Range * 2 + 150, TargetSelector.DamageType.Magical);
            var rstacks = Player.Buffs.Find(buff => buff.Name == "AkaliShadowDance").Count;
            var markdmg = Player.CalcDamage(target, Damage.DamageType.Magical,
                (45 + 35 * Q.Level + 0.5 * Player.FlatMagicDamageMod));
            var RendPos = Player.ServerPosition.Extend(target.Position, Player.ServerPosition.Distance(target.Position) + 175);
            Ignite = Player.GetSpellSlot("summonerdot");
            if (!Config.Item("UseIgnite").GetValue<bool>())
                return;

            //WHEN NOT TO IGNITE
            if (Player.Distance(target.Position) <= E.Range && E.IsReady() && target.Health < E.GetDamage(target))
                return;
            if (Player.Distance(target.Position) <= Q.Range && Q.IsReady() && target.Health < Q.GetDamage(target))
                return;
            if (Player.Distance(target.Position) <= Orbwalking.GetRealAutoAttackRange(Player) && target.Health < Player.GetAutoAttackDamage(target))
                return;
            if (Player.Distance(target.Position) <= Orbwalking.GetRealAutoAttackRange(Player) && target.Health < Player.GetAutoAttackDamage(target) + markdmg && target.HasBuff("AkaliMota"))
                return;

            //WHEN TO IGNITE
            if (Player.Distance(target.Position) <= E.Range && Ignite.IsReady() && PotentialDmg(target) > target.Health && Player.ManaPercent > 50 && rstacks >= 1 && !RendPos.UnderTurret(true))
                Player.Spellbook.CastSpell(Ignite, target);

            if (Player.Distance(target.Position) <= Q.Range && Ignite.IsReady() && Q.GetDamage(target) + E.GetDamage(target) + IgniteDamage(target) > target.Health && Player.ManaPercent > 50 && rstacks >= 1 && !RendPos.UnderTurret(true) && Q.IsReady() && E.IsReady())
                Player.Spellbook.CastSpell(Ignite, target);

            if (Player.Distance(target.Position) <= 600 && Ignite.IsReady() && IgniteDamage(target) > target.Health)
                Player.Spellbook.CastSpell(Ignite, target);

        }
        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var HarassQE = Config.Item("HarassQEnergy").GetValue<Slider>().Value;
            var HarassEE = Config.Item("HarassEEnergy").GetValue<Slider>().Value;

            if (!target.IsValidTarget() || target == null)
                return;
            if (Player.IsAttackingPlayer)
                return;
            if (Player.ManaPercent <= HarassQE)
                return;
            if (Player.ManaPercent <= HarassEE)
                return;
  
            if (Config.Item("HarassQ").GetValue<bool>() && Q.IsReady())
                Q.Cast(target);
            if (Config.Item("HarassE").GetValue<bool>() && E.IsReady()
                && target.IsValidTarget(E.Range))
                E.Cast(target);

        }
        private static void Rlogic()
        {
            var target = TargetSelector.GetTarget(R.Range * 2 + 150, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
                return;
            var DRendPos = Player.ServerPosition.Extend(target.Position, Player.ServerPosition.Distance(target.Position) + 175);
            Ignite = Player.GetSpellSlot("summonerdot");

            Item();
            IgniteLogic();


            //TURRET CHECK          
                //if (target.CountEnemiesInRange(1200) > Config.Item("eslider").GetValue<Slider>().Value &&
                  //  Config.Item("Rcheck").GetValue<bool>())
                   // return;

                if (DRendPos.UnderTurret(true) && Config.Item("rturrete").GetValue<bool>() &&
                     target.HealthPercent > Config.Item("rturrethpe").GetValue<Slider>().Value)
                     return;
                if (DRendPos.UnderTurret(true) && Config.Item("rturret").GetValue<bool>() &&
                     Player.HealthPercent < Config.Item("rturrethp").GetValue<Slider>().Value)
                     return;


                //When not to use R

                if (Player.Distance(target.ServerPosition) < Orbwalking.GetRealAutoAttackRange(Player) && target.HasBuff("AkaliMota"))        
                    return;
                if (Player.Distance(target.ServerPosition) < E.Range && E.IsReady())
                    return;
                if (Player.Distance(target.ServerPosition) < Q.Range && Q.IsReady())
                    return;

            if (target.IsFacing(Player) && target.IsMoving && !target.HasBuff("AkaliMota") && target.Health > R.GetDamage(target) && target.Health > PotentialDmg(target) && Config.Item("endposcheck").GetValue<bool>())
                return;


                //When to use R
            if (Config.Item("UseR").GetValue<bool>() && DRendPos.Distance(target.Position) < 180)
            {
                R.Cast(target, true);
                Gapcloser();
            }


        }
        public static Obj_AI_Base GetFarthestMinion(Vector3 playerpos, Vector3 enemypos)
        {
            return ObjectManager.Get<Obj_AI_Base>().Where(x => x.Distance(playerpos) < x.Distance(enemypos) && !x.IsInvulnerable && x.IsValidTarget(R.Range)).OrderBy(m => m.Distance(playerpos)).LastOrDefault();

        }
        private static void Gapcloserdraw(EventArgs args)
        {
            var rstacks = Player.Buffs.Find(buff => buff.Name == "AkaliShadowDance").Count;
            var target = TargetSelector.GetTarget(R.Range * 2 + 300, TargetSelector.DamageType.Magical);

            if (target.Position.Distance(Player.Position) < R.Range + 75)
                return;

            foreach (var minion in
                ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget() && minion.IsEnemy &&
                                                                   minion.Distance(Player.ServerPosition) <=
                                                                   R.Range * 2))
            {
                if (Config.Item("rlines").GetValue<bool>() && rstacks > 1 &&
                    minion.Distance(target.Position) < R.Range && Player.Distance(target.Position) >= R.Range && minion.IsValidTarget(R.Range))
                {
                    var rdraw = new Geometry.Polygon.Line(Player.Position, GetFarthestMinion(minion.Position, target.Position).Position);
                    var rmdraw = new Geometry.Polygon.Line(GetFarthestMinion(minion.Position, target.Position).Position, target.Position);
                    rdraw.Draw(Color.White, 1);
                    rmdraw.Draw(Color.White, 1);
                }

                foreach (var h in
                    ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget() && h.IsEnemy &&
                                                                h.Distance(Player.ServerPosition) <=
                                                                R.Range * 2))
                {
                    if (Config.Item("rlines").GetValue<bool>() && rstacks > 1 &&
                        Player.Distance(target.Position) >= R.Range && h.Distance(target.Position) < R.Range && h.IsValidTarget(R.Range))
                    {
                        var rdraw = new Geometry.Polygon.Line(Player.Position, h.Position);
                        var rmdraw = new Geometry.Polygon.Line(h.Position, target.Position);
                        rdraw.Draw(Color.White, 1);
                        rmdraw.Draw(Color.White, 1);
                    }
                }
            }
        }
        private static void Gapcloser()
        {
            var rstacks = Player.Buffs.Find(buff => buff.Name == "AkaliShadowDance").Count;
            var target = TargetSelector.GetTarget(R.Range * 2 + 300, TargetSelector.DamageType.Magical);

            if (target.Position.Distance(Player.Position) < R.Range + 50)
                return;
            
                if (Config.Item("miniongapclose").GetValue<bool>() && rstacks > 1 &&
                    GetFarthestMinion(Player.Position, target.Position).Distance(target.Position) < R.Range && Player.Distance(target.Position) >= R.Range)
                    R.Cast(GetFarthestMinion(Player.Position, target.Position));
            
            foreach (var h in
                ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget() && h.IsEnemy &&
                                                            h.Distance(Player.ServerPosition) <=
                                                            R.Range * 2 + 100))
            {
                if (Config.Item("herogapclose").GetValue<bool>() && rstacks > 1 &&
                    Player.Distance(target.Position) >= R.Range &&
                    h.Distance(target.Position) < R.Range)
                    R.Cast(h);
            }
        }

        private static void Elogic()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
                return;
            if (E.IsReady() && Player.Distance(target.ServerPosition) >= Orbwalking.GetRealAutoAttackRange(Player) &&
                Config.Item("UseE").GetValue<bool>() && target.IsValidTarget(E.Range))
                E.Cast(target);

            //DONT USE E CANCEL -> IF KILLABLE
            if (E.IsReady() && target.Health < E.GetDamage(target) && target.Distance(Player.Position) < E.Width)
                E.Cast(target, true);
            Item();
            IgniteLogic();
        }

        private static void Item()
        {
            Ignite = Player.GetSpellSlot("summonerdot");
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            var cutlass = LeagueSharp.Common.Data.ItemData.Bilgewater_Cutlass.GetItem();
            var botrk = LeagueSharp.Common.Data.ItemData.Blade_of_the_Ruined_King.GetItem();
            var hextech = LeagueSharp.Common.Data.ItemData.Hextech_Gunblade.GetItem();
            var sweeper = LeagueSharp.Common.Data.ItemData.Sweeping_Lens_Trinket.GetItem();
            if (target == null || !target.IsValidTarget())
                return;
            if (botrk.IsReady() && botrk.IsOwned(Player) && botrk.IsInRange(target)
            && target.HealthPercent <= Config.Item("eL").GetValue<Slider>().Value
            && Config.Item("UseBOTRK").GetValue<bool>())

                botrk.Cast(target);

            if (botrk.IsReady() && botrk.IsOwned(Player) && botrk.IsInRange(target)
                && Player.HealthPercent <= Config.Item("oL").GetValue<Slider>().Value
                && Config.Item("UseBOTRK").GetValue<bool>())

                botrk.Cast(target);

            if (cutlass.IsReady() && cutlass.IsOwned(Player) && cutlass.IsInRange(target) &&
                target.HealthPercent <= Config.Item("HLe").GetValue<Slider>().Value
                && Config.Item("UseBILGE").GetValue<bool>())

                cutlass.Cast(target);

            if (hextech.IsReady() && hextech.IsOwned(Player) && hextech.IsInRange(target) &&
                target.HealthPercent <= Config.Item("eH").GetValue<Slider>().Value
                && Config.Item("UseHEX").GetValue<bool>())

                hextech.Cast(target);

            if (Config.Item("UseIgnite").GetValue<bool>() && target.Health < IgniteDamage(target))
                Player.Spellbook.CastSpell(Ignite, target);
        }
        //COMBOLOGIC
        private static void Clogic()
        {
            Ignite = Player.GetSpellSlot("summonerdot");
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
                return;
            var markdmg = Player.CalcDamage(target, Damage.DamageType.Magical,
                (45 + 35 * Q.Level + 0.5 * Player.FlatMagicDamageMod));
            //Q BASIC CAST
            if (Q.IsReady() && target.IsValidTarget() && Config.Item("UseQ").GetValue<bool>())
                Q.Cast(target, true);

            //Q-E-IGNITE FINISH
            if (Config.Item("UseQ").GetValue<bool>() && Config.Item("UseE").GetValue<bool>() &&
                Config.Item("UseIgnite").GetValue<bool>()
                && Q.IsReady() && E.IsReady() &&
                target.Health < Q.GetDamage(target) + E.GetDamage(target) + IgniteDamage(target) + markdmg
                && target.Distance(Player.Position) <= Orbwalking.GetRealAutoAttackRange(Player))
            {
                Q.Cast(target, true);
                Player.Spellbook.CastSpell(Ignite, target);
            }

        }
        //WLOGIC
        private static void WLogic()
        {                 
            foreach (var enemy in ObjectManager.Get<AIHeroClient>()
                .Where(x => x.IsValidTarget(Q.Range))
                .Where(x => !x.IsZombie)
                .Where(x => !x.IsDead))
            {
                if (Config.Item("UseW").GetValue<bool>())
                {
                    if (Player.HealthPercent <= Config.Item("WHPslider").GetValue<Slider>().Value && Config.Item("WHP").GetValue<bool>())

                        W.Cast(Player);

                    if (enemy.CountEnemiesInRange(700) >= Config.Item("WHEslider").GetValue<Slider>().Value && Config.Item("WHE").GetValue<bool>())

                        W.Cast(Player);

                }              
            }           
        }

        private static void OnReceiveBuff()
        {

            if (Player.HasBuff("zedulttargetmark") && Config.Item("Antiz").GetValue<bool>() || 
               (Player.HasBuff("caitlynaceinthehole") && Config.Item("AntiC").GetValue<bool>()))
                W.Cast(Player);

        }
        private static void OrbwalkingAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            //E USAGE (LOWCD BETTER AFTER AA)
            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;

            if (unit.IsMe && !Q.IsReady() && Config.Item("UseE").GetValue<bool>() && target.IsValidTarget(E.Range))
                E.Cast();


            var tiamat = LeagueSharp.Common.Data.ItemData.Tiamat_Melee_Only.GetItem();
            var hydra = LeagueSharp.Common.Data.ItemData.Ravenous_Hydra_Melee_Only.GetItem();

            if (unit.IsMe && hydra.IsReady() && Config.Item("UseHYDRA").GetValue<bool>() && target.IsValidTarget(hydra.Range))
                hydra.Cast();

            if (unit.IsMe && tiamat.IsReady() && Config.Item("UseTIAMAT").GetValue<bool>() && target.IsValidTarget(tiamat.Range))
                tiamat.Cast();
        }

        private static void OnDraw(EventArgs args)
        {
            var pos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            if (Config.Item("HarassToggle").GetValue<KeyBind>().Active)
                Drawing.DrawText(pos.X - 50, pos.Y + 30, System.Drawing.Color.Plum, "AutoHarass Enabled");

            if (Config.Item("Draw_Disabled").GetValue<bool>())
                return;

            //DRAW SPELL RANGES
            if (Config.Item("Qdraw").GetValue<Circle>().Active)
                if (Q.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range,
                        Q.IsReady() ? Config.Item("Qdraw").GetValue<Circle>().Color : System.Drawing.Color.Red,
                                                    Config.Item("CircleThickness").GetValue<Slider>().Value); //Thickness

            if (Config.Item("Wdraw").GetValue<Circle>().Active)
                if (W.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range,
                        W.IsReady() ? Config.Item("Wdraw").GetValue<Circle>().Color : System.Drawing.Color.Red,
                                                    Config.Item("CircleThickness").GetValue<Slider>().Value); //Thickness

            if (Config.Item("Edraw").GetValue<Circle>().Active)
                if (E.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range,
                        E.IsReady() ? Config.Item("Edraw").GetValue<Circle>().Color : System.Drawing.Color.Red,
                                                    Config.Item("CircleThickness").GetValue<Slider>().Value); //Thickness

            if (Config.Item("Rdraw").GetValue<Circle>().Active)
                if (R.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range,
                        R.IsReady() ? Config.Item("Rdraw").GetValue<Circle>().Color : System.Drawing.Color.Red,
                                                        Config.Item("CircleThickness").GetValue<Slider>().Value); //Thickness

            if (Config.Item("RGdraw").GetValue<Circle>().Active)
                if (R.Level >= 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Config.Item("rangeRslider").GetValue<Slider>().Value,
                        R.IsReady() ? Config.Item("RGdraw").GetValue<Circle>().Color : System.Drawing.Color.Red,
                                                        Config.Item("CircleThickness").GetValue<Slider>().Value); //Thickness

            //Draw orbwalker target
            var orbwalkert = Orbwalker.GetTarget();
            if (orbwalkert.IsValidTarget(R.Range))
                Render.Circle.DrawCircle(orbwalkert.Position, 50, System.Drawing.Color.IndianRed, 7);


            //Draw Killable minion AA
            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsValidTarget() && x.IsEnemy
                                                                                 && x.Distance(Player.ServerPosition) <
                                                                                 E.Range &&
                                                                                 x.Health < Player.GetAutoAttackDamage(x)))
            {
                Render.Circle.DrawCircle(minion.Position, 50, System.Drawing.Color.IndianRed, 10);
            }

            //DRAW LINES BETWEEN MINION AND TARGET

            //DRAW POTENTIAL DMG IN NUMBERS
            foreach (var enemy in
                ObjectManager.Get<AIHeroClient>().Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
            {
                var epos = Drawing.WorldToScreen(enemy.Position);
                if (Config.Item("drawpotential").GetValue<bool>())
                    Drawing.DrawText(epos.X - 50, epos.Y + 50, System.Drawing.Color.Gold,
                        "Potential DMG = " + PotentialDmg(enemy).ToString());
              //  if (Config.Item("drawreal").GetValue<bool>())
                 //   Drawing.DrawText(epos.X - 50, epos.Y + 75, System.Drawing.Color.Gold,
                   //     "Actual DMG = " + ActualDMG(enemy).ToString());
            }
            var target = TargetSelector.GetTarget(R.Range * 2 + 500, TargetSelector.DamageType.Magical);
            var DRendPos = Player.ServerPosition.Extend(target.Position, Player.ServerPosition.Distance(target.Position) + 175);

            if (target.IsMoving && target.IsFacing(Player))
                DRendPos = Player.ServerPosition.Extend(target.Position, Player.ServerPosition.Distance(target.Position) + 200);
            if (target.IsMoving && !target.IsFacing(Player))
                DRendPos = Player.ServerPosition.Extend(target.Position, Player.ServerPosition.Distance(target.Position) + 75);
            if (!target.IsMoving)
                DRendPos = Player.ServerPosition.Extend(target.Position, Player.ServerPosition.Distance(target.Position) + 150);
            if (target.HasBuff("AkaliMota") && !target.IsMoving)
                DRendPos = Player.ServerPosition.Extend(target.Position, Player.ServerPosition.Distance(target.Position) + 100);
            if (target.HasBuff("AkaliMota") && target.IsMoving && !target.IsFacing(Player))
                DRendPos = Player.ServerPosition.Extend(target.Position, Player.ServerPosition.Distance(target.Position) + 50);
            if (target.HasBuff("AkaliMota") && !target.IsFacing(Player) && target.IsMoving)
                DRendPos = Player.ServerPosition.Extend(target.Position, Player.ServerPosition.Distance(target.Position) + 100);

            if (Config.Item("drawRend").GetValue<bool>())
                Render.Circle.DrawCircle(DRendPos, 15, System.Drawing.Color.Green, 20);


            //END
        }

    }
}


