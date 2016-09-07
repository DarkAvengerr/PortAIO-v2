using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.SDK;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace hJhin.Extensions
{
    class Qss
    {
        public static void ExecuteQss()
        {
            if (Config.Menu["activator.settings"]["qss.charm"] && ObjectManager.Player.HasBuffOfType(BuffType.Charm))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Config.Menu["activator.settings"]["qss.snare"] && ObjectManager.Player.HasBuffOfType(BuffType.Snare))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Config.Menu["activator.settings"]["qss.polymorph"] && ObjectManager.Player.HasBuffOfType(BuffType.Polymorph))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Config.Menu["activator.settings"]["qss.stun"] && ObjectManager.Player.HasBuffOfType(BuffType.Stun))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Config.Menu["activator.settings"]["qss.suppression"] && ObjectManager.Player.HasBuffOfType(BuffType.Suppression))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }
            if (Config.Menu["activator.settings"]["qss.taunt"] && ObjectManager.Player.HasBuffOfType(BuffType.Taunt))
            {
                if (Items.CanUseItem((int)ItemId.Quicksilver_Sash) && Items.HasItem((int)ItemId.Quicksilver_Sash))
                {
                    Items.UseItem((int)ItemId.Quicksilver_Sash);
                }
                if (Items.CanUseItem((int)ItemId.Mercurial_Scimitar) && Items.HasItem((int)ItemId.Mercurial_Scimitar))
                {
                    Items.UseItem((int)ItemId.Mercurial_Scimitar);
                }
            }

        }
    }
}
