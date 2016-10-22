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
using System.Linq;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; namespace Activator.Spells
{
    public class CoreSpell
    {
        internal virtual string Name { get; set; }
        internal virtual string DisplayName { get; set; }
        internal virtual float Range { get; set; }
        internal virtual MenuType[] Category { get; set; }
        internal virtual int DefaultMP { get; set; }
        internal virtual int DefaultHP { get; set; }

        public Menu Menu { get; private set; }
        public Menu Parent => Menu.Parent;
        public AIHeroClient Player => ObjectManager.Player;

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

                if (Category.Any(t => t == MenuType.Stealth))
                    Menu.AddItem(new MenuItem("Stealth" + Name + "pct", "Use on Stealth")).SetValue(true);

                if (Category.Any(t => t == MenuType.SlowRemoval))
                    Menu.AddItem(new MenuItem("use" + Name + "sr", "Use on Slows")).SetValue(true);

                if (Category.Any(t => t == MenuType.EnemyLowHP)) 
                    Menu.AddItem(new MenuItem("enemylowhp" + Name + "pct", "Use on Enemy HP % <="))
                        .SetValue(new Slider(DefaultHP))
                        .SetTooltip("Will Use " + Name + " on Enemy if Their HP % < Value");

                if (Category.Any(t => t == MenuType.SelfLowHP))
                    Menu.AddItem(new MenuItem("selflowhp" + Name + "pct", "Use on Hero HP % <="))
                        .SetValue(new Slider(DefaultHP))
                        .SetTooltip("Will Use " + Name + " When the Income Damage + Hero's HP % < Value");

                if (Category.Any(t => t == MenuType.SelfMuchHP))
                    Menu.AddItem(new MenuItem("selfmuchhp" + Name + "pct", "Use on Hero Dmg Dealt % >="))
                        .SetValue(new Slider(25))
                        .SetTooltip("Will Use " + Name + " When the Hero's Income Damage % > Value");

                if (Category.Any(t => t == MenuType.SelfLowMP))
                    Menu.AddItem(new MenuItem("selflowmp" + Name + "pct", "Use on Hero Mana % <="))
                        .SetValue(new Slider(DefaultMP));

                if (Category.Any(t => t == MenuType.SelfCount))
                    Menu.AddItem(new MenuItem("selfcount" + Name, "Use on # Near Hero >="))
                        .SetValue(new Slider(3, 1, 5));

                if (Category.Any(t => t == MenuType.SelfMinMP))
                    Menu.AddItem(new MenuItem("selfminmp" + Name + "pct", "Minimum Mana/Energy %")).SetValue(new Slider(40));

                if (Category.Any(t => t == MenuType.SelfMinHP))
                    Menu.AddItem(new MenuItem("selfminhp" + Name + "pct", "Minimum HP %")).SetValue(new Slider(40));

                if (Category.Any(t => t == MenuType.SpellShield))
                {
                    Menu.AddItem(new MenuItem("ss" + Name + "all", "Use on Any Spell")).SetValue(false);
                    Menu.AddItem(new MenuItem("ss" + Name + "cc", "Use on Crowd Control")).SetValue(true);
                }

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

        public void CastOnBestTarget(AIHeroClient primary, bool nonhero = false)
        {
            if (LowTarget != null)
            {
                if (!Player.IsRecalling() &&
                    !Player.HasBuffOfType(BuffType.Invisibility) &&
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

        public bool IsReady()
        {
            var ready = Player.GetSpellSlot(Name).IsReady();
            return ready;
        }

        public void UseSpell(bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration)
                {
                    if (!Player.IsRecalling() &&
                        !Player.Spellbook.IsChanneling && 
                        !Player.IsChannelingImportantSpell() &&
                        !Player.Spellbook.IsCastingSpell)
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

        public void UseSpellTowards(Vector3 targetpos, bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration)
                {
                    if (!Player.IsRecalling() &&
                        !Player.Spellbook.IsChanneling &&
                        !Player.IsChannelingImportantSpell() &&
                        !Player.Spellbook.IsCastingSpell)
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

        public void UseSpellOn(Obj_AI_Base target, bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration)
                {
                    if (!Player.IsRecalling() &&
                        !Player.Spellbook.IsChanneling &&
                        !Player.IsChannelingImportantSpell() &&
                        !Player.Spellbook.IsCastingSpell)
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

        public virtual void OnTick(EventArgs args)
        {
        }
    }
}
