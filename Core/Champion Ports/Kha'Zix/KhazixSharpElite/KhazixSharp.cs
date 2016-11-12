using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;

/*
 * ToDo:
 * 
 * Hydra <-- done
 * 
 * overkill <--done
 * 
 * 
 * tower dives
 * 
 * 
 * ult only close
 * 
 * */


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KhazixSharp
{
    internal class KhazixSharp
    {

        public const string CharName = "Khazix";

        public static Menu Config;

        public static HpBarIndicator hpi = new HpBarIndicator();

        public KhazixSharp()
        {
            /* CallBAcks */
            onLoad();

        }

        private static void onLoad()
        {

            Chat.Print("Khazix - Sharp by DeTuKs");

            try
            {

                Config = new Menu("KhazixSharp", "Khazix", true);
                //Orbwalker
                Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
                Khazix.orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));
                //TS
                var TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
                TargetSelector.AddToMenu(TargetSelectorMenu);
                Config.AddSubMenu(TargetSelectorMenu);
                //Combo
                Config.AddSubMenu(new Menu("Combo Sharp", "combo"));
                Config.SubMenu("combo").AddItem(new MenuItem("comboItems", "Use Items")).SetValue(true);

                //LastHit
                Config.AddSubMenu(new Menu("LastHit Sharp", "lHit"));
               
                //LaneClear
                Config.AddSubMenu(new Menu("LaneClear Sharp", "lClear"));
               
                //Harass
                Config.AddSubMenu(new Menu("Harass Sharp", "harass"));
                Config.SubMenu("harass").AddItem(new MenuItem("harassBtn", "Harass Target")).SetValue(new KeyBind('A', KeyBindType.Press, false));

                //Extra
                Config.AddSubMenu(new Menu("Extra Sharp", "extra"));
                

                //Debug
                Config.AddSubMenu(new Menu("Debug", "debug"));
                Config.SubMenu("debug").AddItem(new MenuItem("db_targ", "Debug Target")).SetValue(new KeyBind('T', KeyBindType.Press, false));


                Config.AddToMainMenu();
                Drawing.OnDraw += onDraw;
                Game.OnUpdate += OnGameUpdate;

                Khazix.setSkillshots();
            }
            catch
            {
                Chat.Print("Oops. Something went wrong with KhazixSharp");
            }

        }

        private static void OnGameUpdate(EventArgs args)
        {
            try
            {

                if (Khazix.orbwalker.ActiveMode.ToString() == "Combo")
                {
                    AIHeroClient target = TargetSelector.GetTarget(Khazix.getBestRange(), TargetSelector.DamageType.Physical);

                    Khazix.checkUpdatedSpells();


                    Khazix.doCombo(target);
                    //Console.WriteLine(target.NetworkId);
                }
                if (Config.Item("harassBtn").GetValue<KeyBind>().Active)
                {
                    AIHeroClient target = TargetSelector.GetTarget(Khazix.getBestRange(), TargetSelector.DamageType.Physical);


                    Khazix.doHarass(target);
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        private static void onDraw(EventArgs args)
        {
            foreach (
                            var enemy in
                                ObjectManager.Get<AIHeroClient>()
                                    .Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsHPBarRendered))
            {
                hpi.unit = enemy;
                hpi.drawDmg(Khazix.fullComboDmgOn(enemy), Color.Yellow);
            }
            Drawing.DrawCircle(Khazix.Player.Position, Khazix.Q.Range, Color.Pink);
            Drawing.DrawCircle(Khazix.Player.Position, Khazix.W.Range, Color.Pink);
            Drawing.DrawCircle(Khazix.Player.Position, Khazix.E.Range, Color.Pink);
        }
    }
}
