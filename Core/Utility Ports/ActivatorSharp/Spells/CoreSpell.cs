#region Copyright © 2015 Kurisu Solutions
// All rights are reserved. Transmission or reproduction in part or whole,
// any form or by any means, mechanical, electronical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Document:	Spells/CoreSpell.cs
// Date:		22/09/2015
// Author:		Robin Kurisu
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Activator.Spells
{
    public class CoreSpell
    {
        internal virtual string Name { get; set; }
        internal virtual string DisplayName { get; set; }
        internal virtual float Range { get; set; }
        internal virtual bool Needed { get; set; }
        internal virtual MenuType[] Category { get; set; }
        internal virtual int DefaultMP { get; set; }
        internal virtual int DefaultHP { get; set; }
        internal virtual int Priority { get; set; }

        public Menu Menu { get; private set; }
        public Menu Parent => Menu.Parent;
        public AIHeroClient Player => ObjectManager.Player;

        public IEnumerable<Priority> PriorityList
            =>
                Lists.Priorities.Values.Where(ii => ii.Needed() && ii.Ready())
                    .OrderByDescending(ii => ii.Menu().Item("prior" + ii.Name()).GetValue<Slider>().Value);

        public AIHeroClient LowTarget
        {
            get
            {
                return ObjectManager.Get<AIHeroClient>()
                    .Where(x => x.IsValidTarget(Range))
                    .OrderBy(ene => ene.Health/ene.MaxHealth*100).First();
            }
        }

        public CoreSpell CreateMenu(Menu root)
        {
            try
            {
                if (Player.GetSpellSlot(Name) == SpellSlot.Unknown)
                    return null;

                Menu = new Menu(DisplayName, "m" + Name);
                Menu.AddItem(new MenuItem("use" + Name, "Use " + DisplayName)).SetValue(false).Permashow();

                Menu.AddItem(new MenuItem("prior" + Name, DisplayName + " -> Priority"))
                    .SetValue(new Slider(Priority, 1, 7))
                    .SetTooltip("The Priority " + DisplayName + " Will Activate Over Something Else (7 = Highest)");

                if (Category.Any(t => t == MenuType.Stealth))
                    Menu.AddItem(new MenuItem("Stealth" + Name + "pct", "Use on Stealth")).SetValue(true);

                if (Category.Any(t => t == MenuType.SlowRemoval))
                    Menu.AddItem(new MenuItem("use" + Name + "sr", "Use on Slows")).SetValue(true);

                if (Category.Any(t => t == MenuType.EnemyLowHP)) 
                    Menu.AddItem(new MenuItem("enemylowhp" + Name + "pct", "Use on Enemy HP (%) <="))
                        .SetValue(new Slider(DefaultHP))
                        .SetTooltip("Will Use " + Name + " on Enemy if Their HP (%) < Value");

                if (Category.Any(t => t == MenuType.SelfLowHP))
                    Menu.AddItem(new MenuItem("selflowhp" + Name + "pct", "Use on Hero HP (%) <="))
                        .SetValue(new Slider(DefaultHP))
                        .SetTooltip("Will Use " + Name + " When the Income Damage + Hero's HP (%) < Value");

                if (Category.Any(t => t == MenuType.SelfMuchHP))
                {
                    int spellcount = Priority > 6 ? 5 : (Priority <= 6 && Priority > 3 ? 4 : 5);
                    int dangercount = Priority > 6 ? 4 : (Priority <= 6 && Priority > 3 ? 3 : 4);
                    int crowdcontrolcount = 3;

                    Menu.AddItem(new MenuItem("manyspell" + Name, "Use if (#) of Spell Hit Hero >=")).SetValue(new Slider(spellcount, 1, 10));
                    Menu.AddItem(new MenuItem("manydanger" + Name, "Use if (#) of Danger Hit Hero >=")).SetValue(new Slider(dangercount, 1, 10));
                    Menu.AddItem(new MenuItem("manycrowdcontrol" + Name, "Use if (#) of Crowd Control Hit Hero >=")).SetValue(new Slider(crowdcontrolcount, 1, 10));
                }

                if (Category.Any(t => t == MenuType.SelfLowMP))
                    Menu.AddItem(new MenuItem("selflowmp" + Name + "pct", "Use on Hero Mana (%) <="))
                        .SetValue(new Slider(DefaultMP));

                if (Category.Any(t => t == MenuType.SelfCount))
                    Menu.AddItem(new MenuItem("selfcount" + Name, "Use on (#) Near Hero >="))
                        .SetValue(new Slider(3, 1, 5));

                if (Category.Any(t => t == MenuType.SelfMinMP))
                    Menu.AddItem(new MenuItem("selfminmp" + Name + "pct", "Minimum Mana/Energy (%)")).SetValue(new Slider(40));

                if (Category.Any(t => t == MenuType.SelfMinHP))
                    Menu.AddItem(new MenuItem("selfminhp" + Name + "pct", "Minimum HP (%)")).SetValue(new Slider(40));

                if (Category.Any(t => t == MenuType.Zhonyas))
                {
                    Menu.AddItem(new MenuItem("use" + Name + "norm", "Use on Dangerous (Spells)"))
                        .SetTooltip("Not recommended to enable on spells with long cooldowns.").SetValue(false);
                    Menu.AddItem(new MenuItem("use" + Name + "ulti", "Use on Dangerous (Ultimates Only)")).SetValue(true);
                }

                if (Category.Any(t => t == MenuType.ActiveCheck))
                    Menu.AddItem(new MenuItem("mode" + Name, "Mode: "))
                        .SetValue(new StringList(new[] { "Always", "Combo" }));

                root.AddSubMenu(Menu);
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Chat.Print("<font color=\"#FFF280\">Exception thrown at CoreSpell.CreateMenu: </font>: " + e.Message);
            }

            return this;
        }

        public bool IsReady()
        {
            var ready = Player.GetSpellSlot(Name).IsReady();
            return ready;
        }

        public void CastOnBestTarget(AIHeroClient primary, bool nonhero = false)
        {
            if (IsReady())
            {
                LeagueSharp.Common.Utility.DelayAction.Add(80 - Priority * 10, () => Needed = true);
                LeagueSharp.Common.Utility.DelayAction.Add(1000, () => Needed = false);
            }

            if (LowTarget != null)
            {
                if (!Player.IsRecalling() &&
                    !Player.HasBuffOfType(BuffType.Invisibility) &&
                    !Player.HasBuffOfType(BuffType.Invulnerability) &&
                    !Player.Spellbook.IsChanneling &&
                    !Player.IsChannelingImportantSpell())
                {
                    if (Player.Spellbook.CastSpell(Player.GetSpellSlot(Name), LowTarget))
                    {
                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                        Activator.LastUsedDuration = 100;
                    }
                }
            }
        }

        public void UseSpell(bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (IsReady())
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(80 - Priority * 10, () => Needed = true);
                    LeagueSharp.Common.Utility.DelayAction.Add(1000, () => Needed = false);
                }

                if (PriorityList.Any() && Name == PriorityList.First().Name())
                {
                    if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration)
                    {
                        if (!Player.IsRecalling() &&
                            !Player.Spellbook.IsChanneling &&
                            !Player.IsChannelingImportantSpell() &&
                            !Player.Spellbook.IsCastingSpell)
                        {
                            if (!Player.HasBuffOfType(BuffType.Invisibility) && !Player.HasBuffOfType(BuffType.Invulnerability))
                            {
                                if (Player.Spellbook.CastSpell(Player.GetSpellSlot(Name)))
                                {
                                    Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                                    Activator.LastUsedDuration = 100;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void UseSpellTo(Vector3 targetpos, bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (IsReady())
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(80 - Priority * 10, () => Needed = true);
                    LeagueSharp.Common.Utility.DelayAction.Add(1000, () => Needed = false);
                }

                if (PriorityList.Any() && Name == PriorityList.First().Name())
                {
                    if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration)
                    {
                        if (!Player.IsRecalling() &&
                            !Player.Spellbook.IsChanneling &&
                            !Player.IsChannelingImportantSpell() &&
                            !Player.Spellbook.IsCastingSpell)
                        {
                            if (Player.Distance(targetpos) <= Range)
                            {
                                if (!Player.HasBuffOfType(BuffType.Invisibility) && !Player.HasBuffOfType(BuffType.Invulnerability))
                                {
                                    if (Player.Spellbook.CastSpell(Player.GetSpellSlot(Name), targetpos))
                                    {
                                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                                        Activator.LastUsedDuration = 100;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void UseSpellOn(Obj_AI_Base target, bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (IsReady())
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(80 - Priority * 10, () => Needed = true);
                    LeagueSharp.Common.Utility.DelayAction.Add(1000, () => Needed = false);
                }

                if (PriorityList.Any() && Name == PriorityList.First().Name())
                {
                    if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration)
                    {
                        if (!Player.IsRecalling() &&
                            !Player.Spellbook.IsChanneling &&
                            !Player.IsChannelingImportantSpell() &&
                            !Player.Spellbook.IsCastingSpell)
                        {
                            if (Player.Distance(target.Position) <= Range)
                            {
                                if (!Player.HasBuffOfType(BuffType.Invisibility) && !Player.HasBuffOfType(BuffType.Invulnerability))
                                {
                                    if (Player.Spellbook.CastSpell(Player.GetSpellSlot(Name), target))
                                    {
                                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                                        Activator.LastUsedDuration = 100;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool ShouldUseOnMany(Base.Champion hero)
        {
            var alliesFacingMe =
                Activator.Allies().Where(x => !x.Player.IsMe && x.Player.Distance(Player.ServerPosition) <= 1000)
                    .Count(x => x.Player.IsFacing(Player));

            var hostileFacingMe =
                Activator.Heroes.Where(x => x.Player.IsEnemy && x.Player.Distance(Player.ServerPosition) <= 1000)
                    .Count(x => x.Player.IsFacing(Player));

            if (Priority > 6)
            {
                if (hero.SpellCount >= Menu.Item("manyspell" + Name).GetValue<Slider>().Value)
                {
                    if (alliesFacingMe > 0 || hostileFacingMe == 1)
                        return true;
                }
                if (hero.DangerCount >= Menu.Item("manydanger" + Name).GetValue<Slider>().Value)
                {
                    if (alliesFacingMe > 0 || hostileFacingMe == 1)
                        return true;
                }

                if (hero.CrowdControlCount >= Menu.Item("manycrowdcontrol" + Name).GetValue<Slider>().Value)
                {
                    if (alliesFacingMe > 0 || hostileFacingMe == 1)
                        return true;
                }
            }
            else
            {
                if (hero.SpellCount >= Menu.Item("manyspell" + Name).GetValue<Slider>().Value)
                {
                    if (hostileFacingMe > 1)
                        return true;
                }

                if (hero.DangerCount >= Menu.Item("manydanger" + Name).GetValue<Slider>().Value)
                {
                    if (hostileFacingMe > 1)
                        return true;
                }

                if (hero.CrowdControlCount >= Menu.Item("manycrowdcontrol" + Name).GetValue<Slider>().Value)
                {
                    if (hostileFacingMe > 1)
                        return true;
                }
            }

            return false;
        }

        public virtual void OnTick(EventArgs args)
        {
        }
    }
}
