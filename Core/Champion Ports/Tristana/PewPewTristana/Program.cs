using System;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Security.AccessControl;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;

using Color = System.Drawing.Color;

using EloBuddy; 
using LeagueSharp.Common; 
namespace PewPewTristana
{
    internal class Program
    {
        public const string ChampName = "Tristana";
        public static HpBarIndicator Hpi = new HpBarIndicator();
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static int SpellRangeTick;
        private static SpellSlot Ignite;
        private static readonly AIHeroClient player = ObjectManager.Player;

        public static void Main()
        {
            OnLoad(new EventArgs());
        }

        private static void OnLoad(EventArgs args)
        {
            if (player.ChampionName != ChampName)
                return;

            Notifications.AddNotification("PewPewTristana Loaded!", 5000);

            //Ability Information - Range - Variables.
            Q = new Spell(SpellSlot.Q, 585);

            //RocketJump Settings
            W = new Spell(SpellSlot.W, 900);
            W.SetSkillshot(0.25f, 150, 1200, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 630);
            R = new Spell(SpellSlot.R, 630);


            Config = new Menu("PewPewTristana", "Tristana", true);
            Orbwalker = new Orbwalking.Orbwalker(Config.AddSubMenu(new Menu("[PPT]: Orbwalker", "Orbwalker")));
            TargetSelector.AddToMenu(Config.AddSubMenu(new Menu("[PPT]: Target Selector", "Target Selector")));

            //COMBOMENU

            var combo = Config.AddSubMenu(new Menu("[PPT]: Combo Settings", "Combo Settings"));
            var harass = Config.AddSubMenu(new Menu("[PPT]: Harass Settings", "Harass Settings"));
            var drawing = Config.AddSubMenu(new Menu("[PPT]: Draw Settings", "Draw"));


            combo.SubMenu("[Q] Settings").AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            combo.SubMenu("[Q] Settings").AddItem(new MenuItem("QonE", "Use [Q] if target has [E] debuff").SetValue(false));


            combo.SubMenu("[W] Settings").AddItem(new MenuItem("UseW", "Not Supported"));

            combo.SubMenu("[E] Settings").AddItem(new MenuItem("UseE", "Use Explosive Charge").SetValue(true));

            combo.SubMenu("[R] Settings").AddItem(new MenuItem("UseR", "Use R").SetValue(true));
            combo.SubMenu("[R] Settings").AddItem(new MenuItem("UseRE", "Use ER [FINISHER]").SetValue(true));
            combo.SubMenu("[R] Settings").AddItem(new MenuItem("manualr", "Cast R on your target").SetValue(new KeyBind('R', KeyBindType.Press)));



            combo.SubMenu("Item Settings").AddItem(new MenuItem("useGhostblade", "Use Youmuu's Ghostblade").SetValue(true));
            combo.SubMenu("Item Settings").AddItem(new MenuItem("UseBOTRK", "Use Blade of the Ruined King").SetValue(true));
            combo.SubMenu("Item Settings").AddItem(new MenuItem("eL", "  Enemy HP Percentage").SetValue(new Slider(80, 100, 0)));
            combo.SubMenu("Item Settings").AddItem(new MenuItem("oL", "  Own HP Percentage").SetValue(new Slider(65, 100, 0)));
            combo.SubMenu("Item Settings").AddItem(new MenuItem("UseBilge", "Use Bilgewater Cutlass").SetValue(true));
            combo.SubMenu("Item Settings").AddItem(new MenuItem("HLe", "  Enemy HP Percentage").SetValue(new Slider(80, 100, 0)));
            combo.SubMenu("Summoner Settings").AddItem(new MenuItem("UseIgnite", "Use Ignite").SetValue(true));


            //LANECLEARMENU
            Config.SubMenu("[PPT]: Laneclear Settings").AddItem(new MenuItem("laneQ", "Use Q").SetValue(true));
            Config.SubMenu("[PPT]: Laneclear Settings").AddItem(new MenuItem("laneE", "Use E").SetValue(true));
            Config.SubMenu("[PPT]: Laneclear Settings").AddItem(new MenuItem("eturret", "Use E on turrets").SetValue(true));
            Config.SubMenu("[PPT]: Laneclear Settings").AddItem(new MenuItem("laneclearmana", "Mana Percentage").SetValue(new Slider(30, 100, 0)));
            Config.SubMenu("[PPT]: Laneclear Settings").AddItem(new MenuItem("levelclear", "Don't use abilities till level").SetValue(new Slider(8, 18 ,1)));

            //JUNGLEFARMMENU
            Config.SubMenu("[PPT]: Jungle Settings").AddItem(new MenuItem("jungleQ", "Use Q").SetValue(true));
            Config.SubMenu("[PPT]: Jungle Settings").AddItem(new MenuItem("jungleE", "Use E").SetValue(true));
            Config.SubMenu("[PPT]: Jungle Settings").AddItem(new MenuItem("jungleclearmana", "Mana Percentage").SetValue(new Slider(30, 100, 0)));

            drawing.AddItem(new MenuItem("Draw_Disabled", "Disable All Spell Drawings").SetValue(false));
            drawing.SubMenu("Misc Drawings").AddItem(new MenuItem("drawRtoggle", "Draw R finisher toggle").SetValue(true));
            drawing.SubMenu("Misc Drawings").AddItem(new MenuItem("drawtargetcircle", "Draw Orbwalker target circle").SetValue(true));

            drawing.AddItem(new MenuItem("Qdraw", "Draw Q Range").SetValue(new Circle(true, Color.Orange)));
            drawing.AddItem(new MenuItem("Wdraw", "Draw W Range").SetValue(new Circle(true, Color.DarkOrange)));
            drawing.AddItem(new MenuItem("Edraw", "Draw E Range").SetValue(new Circle(true, Color.AntiqueWhite)));
            drawing.AddItem(new MenuItem("Rdraw", "Draw R Range").SetValue(new Circle(true, Color.LawnGreen)));
            drawing.AddItem(new MenuItem("CircleThickness", "Circle Thickness").SetValue(new Slider(1, 30, 0)));

            harass.AddItem(new MenuItem("harassQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("harassE", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("harassmana", "Mana Percentage").SetValue(new Slider(30, 100, 0)));

            drawing.AddItem(new MenuItem("disable.dmg", "Fully Disable Damage Indicator").SetValue(false));
            drawing.AddItem(new MenuItem("dmgdrawer", "[Damage Indicator]:", true).SetValue(new StringList(new[] { "Custom", "Common" }, 1)));

            Config.SubMenu("[PPT]: Misc Settings").AddItem(new MenuItem("interrupt", "Interrupt Spells").SetValue(true));
            Config.SubMenu("[PPT]: Misc Settings").AddItem(new MenuItem("antigap", "Antigapcloser").SetValue(true));
            Config.SubMenu("[PPT]: Misc Settings").AddItem(new MenuItem("AntiRengar", "Anti-Rengar Leap").SetValue(true));
            Config.SubMenu("[PPT]: Misc Settings").AddItem(new MenuItem("AntiKhazix", "Anti-Khazix Leap").SetValue(true));

            Config.AddToMainMenu();

            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnLevelUp += TristRange;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnEndScene += OnEndScene;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapCloser_OnEnemyGapcloser;
            GameObject.OnCreate += GameObject_OnCreate;



        }

        private static void TristRange(Obj_AI_Base sender, EventArgs args)
        {
            var lvl = (7 * (player.Level - 1));
            Q.Range = 605 + lvl;
            E.Range = 635 + lvl;
            R.Range = 635 + lvl;
        }



        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {

            var rengar = HeroManager.Enemies.Find(h => h.ChampionName.Equals("Rengar")); //<---- Credits to Asuna (Couldn't figure out how to cast R to Sender so I looked at his vayne ^^
            if (rengar != null)

                if (sender.Name == ("Rengar_LeapSound.troy") && Config.Item("AntiRengar").GetValue<bool>() &&
                    sender.Position.Distance(player.Position) < R.Range)
                    R.Cast(rengar);

            var khazix = HeroManager.Enemies.Find(h => h.ChampionName.Equals("Khazix"));
            if (khazix != null)

                if (sender.Name == ("Khazix_Base_E_Tar.troy") && Config.Item("AntiKhazix").GetValue<bool>() &&
                   sender.Position.Distance(player.Position) <= 300)
                    R.Cast(khazix);

        }
        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {

            if (R.IsReady() && sender.IsValidTarget(E.Range) && Config.Item("interrupt").GetValue<bool>())
                R.CastOnUnit(sender);
        }

        private static void AntiGapCloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (R.IsReady() && gapcloser.Sender.IsValidTarget(E.Range) && Config.Item("antigap").GetValue<bool>())
                R.CastOnUnit(gapcloser.Sender);
        }

        private static void OnEndScene(EventArgs args)
        {
            if (Config.Item("disable.dmg").GetValue<bool>())
            {
                LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = false;
                return;
            }
            int mode = Config.Item("dmgdrawer", true).GetValue<StringList>().SelectedIndex;
            if (mode == 0)
            {
                foreach (var enemy in
                    ObjectManager.Get<AIHeroClient>().Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                {
                    Hpi.unit = enemy;
                    Hpi.drawDmg(CalcDamage(enemy), Color.Green);
                    LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = false;
                }
            }
            if (mode == 1)
            {
                LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = CalcDamage;
                LeagueSharp.Common.Utility.HpBarDamageIndicator.Color = Color.Aqua;
                LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = true;

            }
        }

        private static void combo()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget())
                return;

            if (R.IsReady() && target.IsValidTarget(R.Range)) rlogic(target);

            var botrk = LeagueSharp.Common.Data.ItemData.Blade_of_the_Ruined_King.GetItem();
            var Ghost = LeagueSharp.Common.Data.ItemData.Youmuus_Ghostblade.GetItem();
            var cutlass = LeagueSharp.Common.Data.ItemData.Bilgewater_Cutlass.GetItem();

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (botrk.IsReady() && botrk.IsInRange(target) || 
                    Ghost.IsReady() && target.IsValidTarget(Q.Range) || 
                    cutlass.IsReady() && cutlass.IsInRange(target)) items();
            }

            if (!Q.IsReady() && !E.IsReady())
                return;

            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                qlogic(target);

            if (E.IsReady() && Config.Item("UseE").GetValue<bool>()) E.Cast(target);

        }
        public static float CalcDamage(Obj_AI_Base target)
        {
                //Calculate Combo Damage
                float damage = (float)player.GetAutoAttackDamage(target, true) * (1 + player.Crit);
                Ignite = player.GetSpellSlot("summonerdot");

                if (Ignite.IsReady())
                    damage += (float)player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);

                if (Items.HasItem(3153) && Items.CanUseItem(3153))
                    damage += (float)player.GetItemDamage(target, Damage.DamageItems.Botrk);

                if (Items.HasItem(3144) && Items.CanUseItem(3144))
                    damage += (float)player.GetItemDamage(target, Damage.DamageItems.Bilgewater);

            if (E.IsReady())
                damage += E.GetDamage(target);

            if (target.HasBuff("tristanaecharge"))
                damage += E.GetDamage(target) * (float)(0.30 * target.Buffs.Find(buff => buff.Name == "tristanaecharge").Count) +1;


            if (R.IsReady()) // rdamage
                    damage += R.GetDamage(target);

                return damage;
            }
            
    

        private static void qlogic(AIHeroClient target)
        {
            if (target == null || !target.IsValid) return;

            if (Config.Item("QonE").GetValue<bool>() && !target.HasBuff("tristanaecharge"))
                return;

            if (Q.IsReady() && Config.Item("UseQ").GetValue<bool>() && target.IsValidTarget(Q.Range))
                Q.Cast(player);
        }




        private static void rlogic(AIHeroClient target)
        {
            if (target == null)
                return;

            if (Config.Item("UseR").GetValue<bool>() && R.IsReady() &&
            R.GetDamage(target) > target.Health)
            {
                R.Cast(target);
            }

            if (Config.Item("manualr").GetValue<KeyBind>().Active && R.IsReady())
                R.Cast(target);

            if (Config.Item("UseRE").GetValue<bool>()
                && R.IsReady()
                && Config.Item("UseR").GetValue<bool>()
                && target.HasBuff("tristanaecharge") 
                && (E.GetDamage(target) * 
                ((0.30 * target.Buffs.Find(buff => buff.Name == "tristanaecharge").Count) + 1) + R.GetDamage(target)) - 2 * target.Level > target.Health)
            {
                R.Cast(target);
            }


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

            var botrk = LeagueSharp.Common.Data.ItemData.Blade_of_the_Ruined_King.GetItem();
            var Ghost = LeagueSharp.Common.Data.ItemData.Youmuus_Ghostblade.GetItem();
            var cutlass = LeagueSharp.Common.Data.ItemData.Bilgewater_Cutlass.GetItem();

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

            if (player.Distance(target) <= 600 && IgniteDamage(target) > target.Health &&
                Config.Item("UseIgnite").GetValue<bool>())
                player.Spellbook.CastSpell(Ignite, target);
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            try
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

            }
            catch
            { }
        }

        private static void harass()
        {
            var harassmana = Config.Item("harassmana").GetValue<Slider>().Value;
            var target = TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(player), TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValid)
                return;


            if (E.IsReady()
                && Config.Item("harassE").GetValue<bool>()
                && target.IsValidTarget(E.Range)
                && player.ManaPercent >= harassmana)

                E.CastOnUnit(target);

            if (Q.IsReady()
                && Config.Item("harassQ").GetValue<bool>()
                && target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(player))
                && player.ManaPercent >= harassmana)

                Q.Cast(player);
        }

        private static void Laneclear()
        {
            var lanemana = Config.Item("laneclearmana").GetValue<Slider>().Value;
            var MinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(player));
            var allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range);
            var AA = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(player));
            var Efarmpos = W.GetCircularFarmLocation(allMinionsE, 200);

            foreach (var turret in
                ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsValidTarget() && player.Distance(t.Position) < Orbwalking.GetRealAutoAttackRange(player) && t != null))
            {
                if (Config.Item("eturret").GetValue<bool>())
                {
                    E.Cast(turret);
                }
            }

            if (player.Level < Config.Item("levelclear").GetValue<Slider>().Value)
                return;


            if (MinionsQ.Count >= 2
                && Config.Item("laneQ").GetValue<bool>()
                && player.ManaPercent >= lanemana)
            {
                Q.Cast(player);
            }

            foreach (var minion in allMinionsE)
            {
                if (minion == null) return;

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                    && minion.IsValidTarget(E.Range) && Efarmpos.MinionsHit > 2
                    && allMinionsE.Count >= 2 && Config.Item("laneE").GetValue<bool>()
                    && player.ManaPercent >= lanemana)

                    E.CastOnUnit(minion);
            }




        }
        private static void Jungleclear()
        {
            var jlanemana = Config.Item("jungleclearmana").GetValue<Slider>().Value;
            var MinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(player), MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var MinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, E.Range + W.Width - 50, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            var Efarmpos = W.GetCircularFarmLocation(MinionsE, W.Width - +100);
            var AA = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(player));

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                && MinionsQ.Count >= 1 && Config.Item("jungleQ").GetValue<bool>()
                && player.ManaPercent >= jlanemana)
                Q.Cast(player);

            foreach (var minion in MinionsE)
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && minion.IsValidTarget(E.Range)
                    && Efarmpos.MinionsHit >= 1
                    && MinionsE.Count >= 1
                    && Config.Item("jungleE").GetValue<bool>()
                    && player.ManaPercent >= jlanemana)

                    E.CastOnUnit(minion);

        }

        private static void OnDraw(EventArgs args)
        {
            {

            }

            //Draw Skill Cooldown on Champ
            var pos = Drawing.WorldToScreen(ObjectManager.Player.Position);

            if (Config.Item("UseR").GetValue<bool>() && Config.Item("drawRtoggle").GetValue<bool>())
                Drawing.DrawText(pos.X - 50, pos.Y + 50, Color.LawnGreen, "[R] Finisher: On");
            else if (Config.Item("drawRtoggle").GetValue<bool>())
                Drawing.DrawText(pos.X - 50, pos.Y + 50, Color.Tomato, "[R] Finisher: Off");

            if (Config.Item("Draw_Disabled").GetValue<bool>())
                return;

           // if (Config.Item("Qdraw").GetValue<Circle>().Active)
              //  if (Q.Level > 0)
                //    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Q.IsReady() ? Config.Item("Qdraw").GetValue<Circle>().Color : Color.Red,
                      //                                  Config.Item("CircleThickness").GetValue<Slider>().Value);
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
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, 550 + 7 * player.Level,
                        E.IsReady() ? Config.Item("Edraw").GetValue<Circle>().Color : Color.Red,
                                                        Config.Item("CircleThickness").GetValue<Slider>().Value);

            if (Config.Item("Rdraw").GetValue<Circle>().Active)
                if (R.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, 550 + 7 * player.Level,
                        R.IsReady() ? Config.Item("Rdraw").GetValue<Circle>().Color : Color.Red,
                                                        Config.Item("CircleThickness").GetValue<Slider>().Value);

            var orbtarget = Orbwalker.GetTarget();
            if (Config.Item("drawtargetcircle").GetValue<bool>() && orbtarget != null)
            Render.Circle.DrawCircle(orbtarget.Position, 100, Color.DarkOrange, 10);

        }
    }
}
