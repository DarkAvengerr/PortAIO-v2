using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hJhin.Extensions;
using hJhin.Modes;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace hJhin.Champions
{
    public class Jhin
    {
        public Jhin()
        {
            JhinOnLoad();
        }

        private static Orbwalker Orbwalker
        {
            get { return Variables.Orbwalker; }
        }

        private static void JhinOnLoad()
        {
            Notifications.Add(new Notification("hJhin - (click and read)", 
                "Jhin is well syncronized with scripting mechanisms. I developed this assembly to increase your ingame " +
                "performance with Jhin. With this assembly taking a control of your game is inevitable." +
                " Take a step in enjoy the smooth work. Thanks @Southpaw"));

            Spells.Initialize();
            Config.ExecuteMenu();

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    Combo.Execute();
                    break;

                case OrbwalkingMode.LaneClear:
                    Jungle.Execute();
                    Clear.Execute();
                    break;

                case OrbwalkingMode.Hybrid:
                    Harass.Execute();
                    break;
            }

            if (ObjectManager.Player.IsActive(Spells.R))
            {
                Orbwalker.AttackState = false;
                Orbwalker.MovementState = false;
                Orbwalker.Enabled = false;
            }
            else if (!ObjectManager.Player.IsActive(Spells.R))
            {
                Orbwalker.AttackState = true;
                Orbwalker.MovementState = true;
                Orbwalker.Enabled = true;
            }

            if (Config.SemiManualUlt.Active && !ObjectManager.Player.IsActive(Spells.R))
            {
                Orbwalker.Move(Game.CursorPos);
            }

            if (Config.SemiManualUlt.Active && Spells.R.IsReady())
            {
                Ultimate.Execute();
            }
           

            if (Config.Menu["activator.settings"]["use.qss"] && (Items.HasItem((int)ItemId.Quicksilver_Sash) && Items.CanUseItem((int)ItemId.Quicksilver_Sash) ||
                Items.CanUseItem(3139) && Items.HasItem(3137)))
            {
                Qss.ExecuteQss();
            }

            if (Config.Menu["misc.settings"]["auto.orb.buy"] && ObjectManager.Player.Level >= Config.Menu["misc.settings"]["orb.level"]
                && !Items.HasItem((int)ItemId.Farsight_Alteration))
            {
                Shop.BuyItem(ItemId.Farsight_Alteration);
            }
        }
    }
}
