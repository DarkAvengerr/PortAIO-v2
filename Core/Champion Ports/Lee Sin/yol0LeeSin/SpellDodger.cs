using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace yol0LeeSin
{
    class SpellDodger
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static Spell _W = new Spell(SpellSlot.W, 700);
        private static Dictionary<string, int> SpellDictionary = new Dictionary<string, int>();
        private static Menu _menu;

        public static void Initialize(Menu attachToMenu)
        {
            _menu = attachToMenu;
            SpellDictionary.Add("CurseoftheSadMummy", 550);
            SpellDictionary.Add("GragasR", 375);
            SpellDictionary.Add("UFSlash", 300);
            SpellDictionary.Add("FizzMarinerDoom", 120);
            SpellDictionary.Add("LeonaSolarFlare", 300);
            attachToMenu.AddItem(new MenuItem("dodgeEnabled", "Enable Dodge with W").SetValue(true));
            attachToMenu.AddSubMenu(new Menu("Spells", "Spells"));
            foreach (var enemy in HeroManager.Enemies)
            {
                switch(enemy.ChampionName)
                {
                    case "Amumu": _menu.SubMenu("Spells").AddItem(new MenuItem("CurseoftheSadMummy", "Amumu - Curse of the Sad Mummy").SetValue(true)); break;
                    case "Gragas": _menu.SubMenu("Spells").AddItem(new MenuItem("GragasR", "Gragas - Explosive Cask").SetValue(true)); break;
                    case "Malphite": _menu.SubMenu("Spells").AddItem(new MenuItem("UFSlash", "Malphite - Unstoppable Force").SetValue(true)); break;
                    case "Fizz": _menu.SubMenu("Spells").AddItem(new MenuItem("FizzMarinerDoom", "Fizz - Chum the Waters").SetValue(true)); break;
                    case "Leona": _menu.SubMenu("Spells").AddItem(new MenuItem("LeonaSolarFlare", "Leona - Solar Flare").SetValue(true)); break;
                    default: break;
                }
            }
            Obj_AI_Base.OnProcessSpellCast += DodgeSpell;
        }

        public static void DodgeSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!_menu.Item("dodgeEnabled").GetValue<bool>())
                return;

            if (SpellDictionary.ContainsKey(args.SData.Name))
            {
                if (sender.IsEnemy && Player.Distance(args.End) < SpellDictionary[args.SData.Name] && _menu.SubMenu("Spells").Item(args.SData.Name).GetValue<bool>())
                {

                    foreach (var ally in HeroManager.Allies)
                    {
                        if (Player.Distance(ally.Position) <= 700 && ally.Distance(args.End) > SpellDictionary[args.SData.Name] && !ally.IsMe)
                        {
                            _W.CastOnUnit(ally);
                            return;
                        }
                    }

                    foreach (var minion in MinionManager.GetMinions(700, MinionTypes.All, MinionTeam.Ally))
                    {
                        if (minion.Distance(args.End) > SpellDictionary[args.SData.Name])
                        {
                            _W.CastOnUnit(minion);
                            return;
                        }
                    }

                    foreach (var ward in ObjectManager.Get<Obj_AI_Minion>().Where(obj => (obj.Name.Contains("Ward") || obj.Name.Contains("ward") || obj.Name.Contains("Trinket")) && obj.IsAlly && Player.Distance(obj.Position) <= 700))
                    {
                        if (ward.Distance(args.End) > SpellDictionary[args.SData.Name])
                        {
                            _W.CastOnUnit(ward);
                            return;
                        }
                    }
                }
            }
        }
    }
}

