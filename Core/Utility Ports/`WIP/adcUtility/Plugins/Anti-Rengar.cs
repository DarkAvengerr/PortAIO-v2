using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace adcUtility.Plugins
{
    public static class Anti_Rengar
    {
        private static Obj_AI_Base adCarry = null;
        private static Spell antiRengarCast;
        public static bool hikiAntiRengar { get; set; }
        
        public static Obj_AI_Base adcAntiRengar
        {
            get
            {
                if (adCarry != null && adCarry.IsValid)
                {
                    return adCarry;
                }
                return null;
            }
        }
        private static Spell antiRengarSpell()
        {
            if(ObjectManager.Player.ChampionName == "Vayne")
            {
                return new Spell(SpellSlot.E, 550);
            }
            else if (ObjectManager.Player.ChampionName == "Tristana")
            {
                return new Spell(SpellSlot.R, 550);
            }
            else if (ObjectManager.Player.ChampionName == "Draven")
            {
                return new Spell(SpellSlot.E, 1100);
            }
            else if (ObjectManager.Player.ChampionName == "Ashe")
            {
                return new Spell(SpellSlot.R, 20000);
            }
            else if (ObjectManager.Player.ChampionName == "Jinx")
            {
                return new Spell(SpellSlot.E, 900f);
            }
            else if (ObjectManager.Player.ChampionName == "Urgot")
            {
                return new Spell(SpellSlot.R, 700);
            }
            return null;
        }

        static Anti_Rengar()
        {
            antiRengarCast = antiRengarSpell();
            GameObject.OnCreate += GameObject_OnCreate;
            adCarry = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.IsMe);
            if (adCarry != null)
            {
                Console.Write(adCarry.CharData.BaseSkinName);
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Vayne" ||
                ObjectManager.Player.ChampionName != "Tristana" ||
                ObjectManager.Player.ChampionName != "Draven" ||
                ObjectManager.Player.ChampionName != "Ashe" ||
                ObjectManager.Player.ChampionName != "Jinx" ||
                ObjectManager.Player.ChampionName != "Urgot")
            {
                return;
            }
            var useSpell = Program.Config.Item("anti.rengar").GetValue<bool>();

            if (useSpell && antiRengarCast.IsReady())
            {
                foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget(antiRengarCast.Range)
                && x.ChampionName == "Rengar"))
                {
                    if (sender.Name == "Rengar_LeapSound.troy" && ObjectManager.Player.Distance(hero.Position) <= antiRengarCast.Range)
                    {
                        antiRengarCast.Cast(hero);
                    }
                }
            }
                
            
            
            
        }
        
    }
}