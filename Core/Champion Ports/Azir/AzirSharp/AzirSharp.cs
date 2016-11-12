
/*
 * ToDo:
 * 
 * 
 * check if in any my minions range <done
 * 
 * 
 * 
 * dont W far if Q not rdy
 * 
 * smart laneClear + good calc
 * 
 * smarter E
 * 
 * calc extended w aa range on other    
 * 
 * W+Q ks
 * 
 * 
 * */

using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using DetuksSharp;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using DeathWalker = DetuksSharp.DeathWalker;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace AzirSharp
{
    internal class AzirSharp
    {

        public const string CharName = "Azir";

        public static Menu Config;

        public static HpBarIndicator hpi = new HpBarIndicator();

        public static Vector3 tempTestPos = new Vector3();
        public AzirSharp()
        {
            Console.WriteLine("Azir started");
            /* CallBAcks */
            onLoad();

        }

        private static void onLoad()
        {

            if (ObjectManager.Player.ChampionName != CharName)
                return;

            Chat.Print("Azir - Sharp by DeTuKs");

            try
            {

                Config = new Menu("Azir - Sharp", "Azir", true);
                //Orbwalker
                Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
                DeathWalker.AddToMenu(Config.SubMenu("Orbwalker"));
                //TS
                var TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
                TargetSelector.AddToMenu(TargetSelectorMenu);
                Config.AddSubMenu(TargetSelectorMenu);
                //Combo
                Config.AddSubMenu(new Menu("Combo Sharp", "combo"));
                Config.SubMenu("combo").AddItem(new MenuItem("fullin", "full in combo")).SetValue(new KeyBind('A', KeyBindType.Press, false));
                Config.SubMenu("combo").AddItem(new MenuItem("useQ", "use Q")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useW", "use W")).SetValue(false);
                Config.SubMenu("combo").AddItem(new MenuItem("useE", "use E")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useR", "use R")).SetValue(false);
                Config.SubMenu("combo").AddItem(new MenuItem("fly", "fly to mouse")).SetValue(new KeyBind('T', KeyBindType.Press, false));
                Config.SubMenu("combo").AddItem(new MenuItem("glide", "Inject closest")).SetValue(new KeyBind('Y', KeyBindType.Press, false));

                //LastHit
                // Config.AddSubMenu(new Menu("LastHit Sharp", "lHit"));

                //LaneClear
                // Config.AddSubMenu(new Menu("LaneClear Sharp", "lClear"));

                //Extra
                Config.AddSubMenu(new Menu("Extra Sharp", "extra"));
                Config.SubMenu("extra").AddItem(new MenuItem("wasteR", "dont Waste R")).SetValue(true);
                Config.SubMenu("extra").AddItem(new MenuItem("autoTower", "auto R undet turr")).SetValue(true);
                
                //Drawings
                Config.AddSubMenu(new Menu("Drawings Sharp", "draw"));
                Config.SubMenu("draw").AddItem(new MenuItem("noDraw", "No Drawings")).SetValue(false);
                Config.SubMenu("draw").AddItem(new MenuItem("drawQmax", "draw Q max")).SetValue(true);
                Config.SubMenu("draw").AddItem(new MenuItem("drawW", "draw W")).SetValue(true);
                Config.SubMenu("draw").AddItem(new MenuItem("drawR", "draw R")).SetValue(true);
                Config.SubMenu("draw").AddItem(new MenuItem("drawFullDmg", "draw damage")).SetValue(true);
                Config.SubMenu("draw").AddItem(new MenuItem("drawSoliAA", "draw Solider AA")).SetValue(true);
                Config.SubMenu("draw").AddItem(new MenuItem("drawSoliCtrl", "draw Solider Control")).SetValue(true);

                //Debug
                Config.AddSubMenu(new Menu("Debug", "debug"));
                Config.SubMenu("debug").AddItem(new MenuItem("db_targ", "Debug Target")).SetValue(new KeyBind('U', KeyBindType.Press, false));


                Config.AddToMainMenu();
                Drawing.OnDraw += onDraw;
                Game.OnUpdate += OnGameUpdate;

                GameObject.OnCreate += OnCreateObject;
                GameObject.OnDelete += OnDeleteObject;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;


                Spellbook.OnCastSpell += onCastSpell;

                Drawing.OnEndScene += OnEndScene;

              //  Game.OnGameSendPacket += OnGameSendPacket;
               // Game.OnGameProcessPacket += OnGameProcessPacket;
                DeathWalker.azir = true;
                Azir.setSkillShots();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Chat.Print("Oops. Something went wrong with Azir Sharp");
            }

        }

        private static void onCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (Config.Item("wasteR").GetValue<bool>() && args.Slot == SpellSlot.R &&
                Azir.Player.GetEnemiesInRange(650).Count(ene => ene.IsValid && !ene.IsDead) == 0)
                args.Process = false;

        }

        public static float startTime = 0;
        public static Vector3 startPos = new Vector3();
        public static float endTime = 0;
        public static Vector3 endPos = new Vector3();
        public static bool first = true;


        private static void OnGameUpdate(EventArgs args)
        {
            try
            {

               /* if (Azir.getUsableSoliders().Count != 0)
                {

                    Obj_AI_Minion fir = Azir.getUsableSoliders().First();
                    if (fir.IsMoving)
                    {
                        if (first)
                        {
                            startTime = Game.Time;
                            startPos = fir.ServerPosition;
                            first = false;
                        }

                    }
                    else
                    {
                        if (!first)
                        {
                            endTime = Game.Time;
                            endPos = fir.ServerPosition;
                            float dist = endPos.Distance(startPos);
                            Console.WriteLine(dist/(endTime-startTime));
                            Console.WriteLine(Azir.Player.BoundingRadius);
                            first = true;
                        }
                    }
                }*/

                if (DeathWalker.CurrentMode == DeathWalker.Mode.Combo)
                {
                    AIHeroClient target = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Magical);
                    if(target != null)
                        Azir.doCombo(target);
                }

                if (DeathWalker.CurrentMode == DeathWalker.Mode.Harass)
                {
                    //Azir.doAttack();
                }

                if (DeathWalker.CurrentMode == DeathWalker.Mode.LaneClear)
                {
                    //Azir.doAttack();
                }

                if (Config.Item("fly").GetValue<KeyBind>().Active)
                {
                    Azir.doFlyToMouse(Game.CursorPos);
                }

                if (Config.Item("db_targ").GetValue<KeyBind>().Active)
                {
                    tempTestPos = Game.CursorPos;
                }

                if (Config.Item("glide").GetValue<KeyBind>().Active)
                {
                    AIHeroClient target = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Magical);
                    if(target != null)
                        Azir.doGlideToMouse(target.Position);
                }

                if (Config.Item("autoTower").GetValue<bool>())
                    Azir.autoRunderTower();

                if (Config.Item("fullin").GetValue<KeyBind>().Active)
                {
                    DeathWalker.deathWalk(Game.CursorPos);
                    AIHeroClient target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Magical);
                    if(target != null)
                            Azir.goFullIn(target);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private static void onDraw(EventArgs args)
        {
            if (Config.Item("noDraw").GetValue<bool>())
                return;
            if(Config.Item("drawQmax").GetValue<bool>())
                Render.Circle.DrawCircle(Azir.Player.Position, 1150, (DeathWalker.canAttack()) ? Color.Red : Color.Blue);

            if (Config.Item("drawSoliAA").GetValue<bool>() || Config.Item("drawSoliCtrl").GetValue<bool>())
                foreach (var solid in Azir.MySoldiers)
                {
                    if (solid.IsValid && !solid.IsDead)
                    {
                        if (Config.Item("drawSoliAA").GetValue<bool>())
                            Render.Circle.DrawCircle(solid.Position, 325, Color.Yellow);
                        if (Config.Item("drawSoliCtrl").GetValue<bool>())
                            Render.Circle.DrawCircle(solid.Position, 900, Color.GreenYellow);
                    }
                }

            if (Config.Item("drawW").GetValue<bool>())
                Render.Circle.DrawCircle(Azir.Player.Position, Azir.W.Range, Color.Yellow);

            if (Config.Item("drawR").GetValue<bool>())
            {
                Obj_AI_Base tower =
                    ObjectManager.Get<Obj_AI_Turret>()
                        .Where(tur => tur.IsAlly && tur.Health > 0)
                        .OrderBy(tur => Azir.Player.Distance(tur))
                        .First();
                if (tower != null)
                {
                    var pol = DeathMath.getPolygonOn(Azir.Player.Position.Extend(tower.Position, -125).To2D(),
                        tower.Position.To2D(), 300 + Azir.R.Level * 100, 270);
                    pol.Draw(Color.Yellow);
                }
            }

        }

        private static void OnEndScene(EventArgs args)
        {
            if (Config.Item("drawFullDmg").GetValue<bool>())
                foreach (var enemy in DeathWalker.AllEnemys.Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                {
                    hpi.unit = enemy;
                    hpi.drawDmg(Azir.getFullDmgOn(enemy), Color.Yellow);
                }
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (sender.Name == "AzirSoldier" && sender.IsAlly)
            {
                Obj_AI_Minion myMin = sender as Obj_AI_Minion;
                if (myMin.BaseSkinName == "AzirSoldier")
                    Azir.MySoldiers.Add(myMin);
            }

        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
            int i = 0;
            foreach (var sol in Azir.MySoldiers)
            {
                if (sol.NetworkId == sender.NetworkId)
                {
                    Azir.MySoldiers.RemoveAt(i);
                    return;
                }
                i++;
            }
        }

        public static void OnProcessSpell(Obj_AI_Base obj, GameObjectProcessSpellCastEventArgs arg)
        {
           // if(!obj.IsMe)
          //      return;;

           
        }




    }
}
