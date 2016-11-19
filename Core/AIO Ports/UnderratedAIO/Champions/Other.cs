using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using UnderratedAIO.Helpers;


using EloBuddy; 
using LeagueSharp.Common; 
 namespace UnderratedAIO.Champions
{
    internal class Other
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static AutoLeveler autoLeveler;
        public static readonly AIHeroClient player = ObjectManager.Player;

        public Other()
        {
            InitMenu();
            ////Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Underrated AIO Common</font>");
            Jungle.setSmiteSlot();
            Game.OnUpdate += Game_OnGameUpdate;
            Console.WriteLine(ObjectManager.Player.ChampionName);
        }


        private void Game_OnGameUpdate(EventArgs args)
        {
            if (FpsBalancer.CheckCounter())
            {
                return;
            }
            if (config.Item("Enabledcomm").GetValue<bool>())
            {
                switch (orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        Combo();
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        break;
                    case Orbwalking.OrbwalkingMode.LastHit:
                        break;
                    default:
                        break;
                }
            }
        }

        private void Combo()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(900, TargetSelector.DamageType.Physical);
            if (target == null)
            {
                return;
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config);
            }
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            if (config.Item("useIgnite").GetValue<bool>() && ignitedmg > target.Health && hasIgnite &&
                player.Distance(target) > Orbwalking.GetRealAutoAttackRange(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
        }

        private void InitMenu()
        {
            config = new Menu("UnderratedAIO", "UnderratedAIO", true);
            // Target Selector
            Menu menuTS = new Menu("Selector", "tselect");
            TargetSelector.AddToMenu(menuTS);
            config.AddSubMenu(menuTS);
            // Orbwalker
            Menu menuOrb = new Menu("Orbwalker", "orbwalker");
            orbwalker = new Orbwalking.Orbwalker(menuOrb);
            config.AddSubMenu(menuOrb);
            Menu menuC = new Menu("Combo ", "csettings");
            menuC = ItemHandler.addItemOptons(menuC);
            menuC.AddItem(new MenuItem("useIgnite", "Ignite")).SetValue(true);
            config.AddSubMenu(menuC);
            Menu menuM = new Menu("Misc ", "Msettings");
            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);
            config.AddItem(new MenuItem("Enabledcomm", "Enable Utilies")).SetValue(true);
            config.AddItem(new MenuItem("Enabledorb", "Enable OrbWalker", true)).SetValue(false);
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }
}