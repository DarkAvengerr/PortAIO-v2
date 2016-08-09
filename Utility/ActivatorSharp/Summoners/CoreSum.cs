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
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; namespace Activator.Summoners
{
    public class CoreSum
    {
        internal virtual string Name { get; set; }
        internal virtual string DisplayName { get; set; }
        internal virtual string[] ExtraNames { get; set; }
        internal virtual float Range { get; set; }
        internal virtual int Duration { get; set; }
        internal virtual int DefaultMP { get; set; }
        internal virtual int DefaultHP { get; set; }

        public Menu Menu { get; private set; }
        public Menu Parent => Menu.Parent;
        public SpellSlot Slot => Player.GetSpellSlot(Name);
        public AIHeroClient Player => ObjectManager.Player;

        public CoreSum CreateMenu(Menu root)
        {
            try
            {
                Menu = new Menu(DisplayName, "m" + Name);

                if (!Name.Contains("smite") && !Name.Contains("teleport"))
                    Menu.AddItem(new MenuItem("use" + Name, "Use " + DisplayName)).SetValue(true).Permashow();
 
                if (Name == "summonersnowball")
                    Activator.UseEnemyMenu = true;

                if (Name == "summonerheal")
                {
                    Activator.UseAllyMenu = true;
                    Menu.AddItem(new MenuItem("selflowhp" + Name + "pct", "Use on Hero HP % <=")).SetValue(new Slider(20));
                    Menu.AddItem(new MenuItem("selfmuchhp" + Name + "pct", "Use on Hero Dmg Dealt % >=")).SetValue(new Slider(45));
                    Menu.AddItem(new MenuItem("use" + Name + "tower", "Include Tower Damage")).SetValue(false);
                    Menu.AddItem(new MenuItem("mode" + Name, "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }, 1));
                }

                if (Name == "summonerboost")
                {
                    Activator.UseAllyMenu = true;
                    var ccmenu = new Menu(DisplayName + " Buff Types", DisplayName.ToLower() + "cdeb");
                    ccmenu.AddItem(new MenuItem(Name + "cstun", "Stuns")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "ccharm", "Charms")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "ctaunt", "Taunts")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "cflee", "Flee/Fear")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "csnare", "Snares")).SetValue(true);
                    ccmenu.AddItem(new MenuItem(Name + "cexh", "Exhaust")).SetValue(false);
                    ccmenu.AddItem(new MenuItem(Name + "csupp", "Supression")).SetValue(false);
                    ccmenu.AddItem(new MenuItem(Name + "csilence", "Silences")).SetValue(false);
                    ccmenu.AddItem(new MenuItem(Name + "cpolymorph", "Polymorphs")).SetValue(false);
                    ccmenu.AddItem(new MenuItem(Name + "cblind", "Blinds")).SetValue(false);
                    ccmenu.AddItem(new MenuItem(Name + "cslow", "Slows")).SetValue(false);
                    ccmenu.AddItem(new MenuItem(Name + "cpoison", "Poisons")).SetValue(false);
                    Menu.AddSubMenu(ccmenu);

                    Menu.AddItem(new MenuItem("use" + Name + "number", "Min Buffs to Use")).SetValue(new Slider(1, 1, 5))
                        .SetTooltip("Will Only " + DisplayName + " if Your Buff Count is >= Value");
                    Menu.AddItem(new MenuItem("use" + Name + "time", "Min Durration to Use"))
                        .SetValue(new Slider(500, 250, 2000))
                        .SetTooltip("Will not use unless the buff durration (stun, snare, etc) last at least this long (ms, 500 = 0.5 seconds)");
                    Menu.AddItem(new MenuItem("use" + Name + "od", "Use for Dangerous Only")).SetValue(false);
                    Menu.AddItem(new MenuItem("use" + Name + "delay", "Activation Delay (in ms)")).SetValue(new Slider(150, 0, 500));
                    Menu.AddItem(new MenuItem("mode" + Name, "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }));
                }

                if (Name == "summonerdot")
                {
                    Activator.UseEnemyMenu = true;
                    Menu.AddItem(new MenuItem("idmgcheck", "Combo Damage Check %", true)).SetValue(95)
                        .SetValue(new Slider(100, 1, 200)).SetTooltip("Lower if Igniting to early. Increase if opposite.");

                    switch (Player.ChampionName)
                    {
                        case "Ahri":
                            Menu.AddItem(new MenuItem("ii" + Player.ChampionName, Player.ChampionName + ": Check Charm"))
                                .SetValue(false).SetTooltip("Only ignite if target is charmed?");
                            break;
                        case "Cassiopeia":
                            Menu.AddItem(new MenuItem("ii" + Player.ChampionName, Player.ChampionName + ": Check Poison"))
                                .SetValue(false).SetTooltip("Only ignite if target is poisoned?");
                            break;
                        case "Diana":
                            Menu.AddItem(new MenuItem("ii" + Player.ChampionName, Player.ChampionName + ": Check Moonlight?"))
                                .SetValue(false).SetTooltip("Only ignite if target has moonlight debuff?");
                            break;
                    }

                    Menu.AddItem(new MenuItem("itu", "Dont Ignite Near Turret")).SetValue(true);
                    Menu.AddItem(new MenuItem("igtu", "-> Ignore after Level")).SetValue(new Slider(11, 1, 18));
                    Menu.AddItem(new MenuItem("idraw", "Draw Combo Damage %")).SetValue(true);
                    Menu.AddItem(new MenuItem("mode" + Name, "Mode: "))
                        .SetValue(new StringList(new[] { "Killsteal", "Combo" }, 0));
                }

                if (Name == "summonermana")
                {
                    Activator.UseAllyMenu = true;
                    Menu.AddItem(new MenuItem("selflowmp" + Name + "pct", "Minimum Mana % <=")).SetValue(new Slider(40));        
                }

                if (Name == "summonerbarrier")
                {
                    Activator.UseAllyMenu = true;
                    Menu.AddItem(new MenuItem("selflowhp" + Name + "pct", "Use on Hero HP % <=")).SetValue(new Slider(20));
                    Menu.AddItem(new MenuItem("selfmuchhp" + Name + "pct", "Use on Hero Dmg Dealt % >=")).SetValue(new Slider(45));
                    Menu.AddItem(new MenuItem("use" + Name + "ulti", "Use on Dangerous (Ultimates Only)")).SetValue(true);
                    Menu.AddItem(new MenuItem("f" + Name, "-> Force Barrier"))
                        .SetValue(false).SetTooltip("Will force barrier ultimates ignoring HP% & income damage");
                    Menu.AddItem(new MenuItem("use" + Name + "tower", "Include Tower Damage")).SetValue(true);
                    Menu.AddItem(new MenuItem("mode" + Name, "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }, 1));           
                }

                if (Name == "summonerexhaust")
                {
                    Activator.UseEnemyMenu = true;
                    Menu.AddItem(new MenuItem("a" + Name + "pct", "Exhaust on ally HP %")).SetValue(new Slider(35));
                    Menu.AddItem(new MenuItem("e" + Name + "pct", "Exhaust on enemy HP %")).SetValue(new Slider(45));                  
                    Menu.AddItem(new MenuItem("use" + Name + "ulti", "Use on Dangerous (Utimates Only)"))
                        .SetValue(true).SetTooltip("Or spells with \"Force Exhaust\"");
                    Menu.AddItem(new MenuItem("f" + Name, "-> Force Exhaust"))
                       .SetValue(true).SetTooltip("Will force exhaust ultimates ignoring HP% & income damage");
                    Menu.AddItem(new MenuItem("mode" + Name, "Mode: ")).SetValue(new StringList(new[] { "Always", "Combo" }));
                }

                if (Name == "summonersmite")
                {
                    Activator.UseEnemyMenu = true;
                    Menu.AddItem(new MenuItem("usesmite", "Use Smite")).SetValue(new KeyBind('M', KeyBindType.Toggle, true)).Permashow();
                    Menu.AddItem(new MenuItem("smiteskill", "-> Smite + Ability")).SetValue(true);
                    Menu.AddItem(new MenuItem("smitesmall", "Smite Small Camps")).SetValue(true);
                    // Menu.AddItem(new MenuItem("smitekrug", "-> Krug")).SetValue(true);
                    // Menu.AddItem(new MenuItem("smitewolve", "-> Wolves")).SetValue(true);
                    // Menu.AddItem(new MenuItem("smiterazor", "-> Razorbeak")).SetValue(true);
                    Menu.AddItem(new MenuItem("smitelarge", "Smite Large Camps")).SetValue(true);
                    // Menu.AddItem(new MenuItem("smiteblu", "-> Blu")).SetValue(true);
                    // Menu.AddItem(new MenuItem("smitered", "-> Red")).SetValue(true);
                    Menu.AddItem(new MenuItem("smitesuper", "Smite Epic Camps")).SetValue(true);
                    // Menu.AddItem(new MenuItem("smitebaron", "-> Baron")).SetValue(true);
                    // Menu.AddItem(new MenuItem("smitedragon", "-> Dragon")).SetValue(true);
                    // Menu.AddItem(new MenuItem("smiterift", "-> Rift Herald")).SetValue(true);
                    Menu.AddItem(new MenuItem("smitemode", "Smite Enemies: "))
                        .SetValue(new StringList(new[] { "Killsteal", "Combo", "Nope" }, 1));
                    Menu.AddItem(new MenuItem("savesmite", "-> Save a Smite Charge")
                        .SetValue(true).SetTooltip("Will only combo smite if Ammo > 1"));
                    // Menu.AddItem(new MenuItem("savesmite2", "-> Dont Smite Near Camps")
                        // .SetValue(true).SetTooltip("Wont smite enemies near camps"));
                }

                if (Name == "summonerteleport")
                {
                    Activator.UseAllyMenu = true;
                    Menu.AddItem(new MenuItem("telesound", "Enable Sound")).SetValue(false).SetTooltip("Only you can hear this.");
                    Menu.AddItem(new MenuItem("telelowhp2", "Ping Low Health Allies")).SetValue(false);
                    Menu.AddItem(new MenuItem("teleulthp2", "Ping Dangerous Activity")).SetValue(false);
                }

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

        public string[] Excluded = { "summonerexhaust" };

        public void UseSpell(bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (Excluded.Any(ex => Name.Equals(ex)) || // ignore limit
                    Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration)
                {
                    if (Player.GetSpell(Slot).State == SpellState.Ready)
                    {
                        Player.Spellbook.CastSpell(Slot);
                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                        Activator.LastUsedDuration = Name == "summonerexhaust" ? 0: Duration;
                    }
                }
            }
        }

        public void UseSpellOn(Obj_AI_Base target, bool combo = false)
        {
            if (!combo || Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                if (Excluded.Any(ex => Name.Equals(ex)) || // ignore limit
                    Utils.GameTimeTickCount - Activator.LastUsedTimeStamp > Activator.LastUsedDuration) 
                {
                    if (Player.GetSpell(Slot).State == SpellState.Ready)
                    {
                        Player.Spellbook.CastSpell(Slot, target);
                        Activator.LastUsedTimeStamp = Utils.GameTimeTickCount;
                        Activator.LastUsedDuration = Name == "summonerexhaust" ? 0 : Duration;
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
    }
}
