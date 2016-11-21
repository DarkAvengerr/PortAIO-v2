#region Copyright � 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Summoners/CoreSum.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Activator.Summoners
{
    public class CoreSum
    {
        internal virtual string Name { get; set; }
        internal virtual string DisplayName { get; set; }
        internal virtual string[] ExtraNames { get; set; }
        internal virtual float Range { get; set; }
        internal virtual bool Needed { get; set; }
        internal virtual int Duration { get; set; }
        internal virtual int DefaultMP { get; set; }
        internal virtual int DefaultHP { get; set; }
        internal virtual int Priority { get; set; }

        public Menu Menu { get; private set; }
        public Menu Parent => Menu.Parent;
        public SpellSlot Slot => Player.GetSpellSlot(Name);
        public AIHeroClient Player => ObjectManager.Player;

        public bool ComboActive
            => Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active ||
               Orbwalking.Orbwalker.Instances.Any(x => x.ActiveMode == Orbwalking.OrbwalkingMode.Combo);

        public IEnumerable<Priority> PriorityList
            =>
                Lists.Priorities.Values.Where(ii => ii.Needed())
                    .OrderByDescending(ii => ii.Menu().Item("prior" + ii.Name()).GetValue<Slider>().Value);

        public CoreSum CreateMenu(Menu root)
        {
            try
            {
                Menu = new Menu(DisplayName, "m" + Name);

                if (!Name.Contains("smite") && !Name.Contains("teleport"))
                {
                    Menu.AddItem(new MenuItem("use" + Name, "Use " + DisplayName)).SetValue(true).Permashow();
                    Menu.AddItem(new MenuItem("prior" + Name, DisplayName + " Priority"))
                        .SetValue(new Slider(Priority, 1, 7))
                        .SetTooltip("The Priority " + DisplayName + " Will Activate Over Something Else (7 = Highest)");
                }

                AttachMenu(Menu);

                root.AddSubMenu(Menu);
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("<font color=\"#FFF280\">Exception thrown at CoreSum.CreateMenu: </font>: " + e.Message);
            }

            return this;
        }

        public bool IsReady()
        {
            return Player.GetSpellSlot(Name).IsReady() || 
                ExtraNames.Any(exname => Player.GetSpellSlot(exname).IsReady());
        }

        public string[] HumanizerEx = { "summonerexhaust", "summonerdot", "summonersmite" };
        public string[] PriorityEx = {"summonersmite", "summonerteleport", "summonerdot" };

        public void UseSpell(bool combo = false)
        {
            if (combo && !ComboActive)
            {
                return;
            }

            if (IsReady())
            {
                LeagueSharp.Common.Utility.DelayAction.Add(80 - Priority * 10, () => Needed = true);
                LeagueSharp.Common.Utility.DelayAction.Add(1000 + Duration, () => Needed = false);
            }

            if (PriorityList.Any() && Name == PriorityList.First().Name() || PriorityEx.Any(ex => Name.Equals(ex)))
            {
                if (HumanizerEx.Any(ex => Name.Equals(ex)))
                {
                    if (Player.HasBuffOfType(BuffType.Invulnerability) ||
                        Player.HasBuffOfType(BuffType.Invisibility))
                        return;

                    if (Player.GetSpell(Slot).State == SpellState.Ready)
                    {
                        Player.Spellbook.CastSpell(Slot);
                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                        Activator.LastUsedDuration = Duration;
                    }
                }
                else if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration)
                {
                    if (Player.HasBuffOfType(BuffType.Invulnerability) ||
                        Player.HasBuffOfType(BuffType.Invisibility))
                        return;

                    if (Player.GetSpell(Slot).State == SpellState.Ready)
                    {
                        Player.Spellbook.CastSpell(Slot);
                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                        Activator.LastUsedDuration = Duration;
                    }
                }
            }
        }

        public void UseSpellOn(Obj_AI_Base target, bool combo = false)
        {
            if (combo && !ComboActive)
            {
                return;
            }

            if (IsReady())
            {
                LeagueSharp.Common.Utility.DelayAction.Add(80 - Priority * 10, () => Needed = true);
                LeagueSharp.Common.Utility.DelayAction.Add(1000 + Duration, () => Needed = false);
            }

            if (PriorityList.Any() && Name == PriorityList.First().Name() || PriorityEx.Any(ex => Name.Equals(ex)))
            {
                if (HumanizerEx.Any(ex => Name.Equals(ex)))
                {
                    if (Player.HasBuffOfType(BuffType.Invulnerability) ||
                        Player.HasBuffOfType(BuffType.Invisibility))
                        return;

                    if (Player.GetSpell(Slot).State == SpellState.Ready)
                    {
                        Player.Spellbook.CastSpell(Slot, target);
                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                        Activator.LastUsedDuration = Duration;
                    }
                }
                else if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration)
                {
                    if (Player.HasBuffOfType(BuffType.Invulnerability) ||
                        Player.HasBuffOfType(BuffType.Invisibility))
                        return;

                    if (Player.GetSpell(Slot).State == SpellState.Ready)
                    {
                        Player.Spellbook.CastSpell(Slot, target);
                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                        Activator.LastUsedDuration = Duration;
                    }
                }
            }
        }

        public virtual void OnDraw(EventArgs args)
        {

        }

        public virtual void OnTick(EventArgs args)
        {

        }

        public virtual void AttachMenu(Menu menu)
        {
            
        }
    }
}
