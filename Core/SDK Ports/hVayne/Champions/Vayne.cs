using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using hVayne.Extensions;
using LeagueSharp.SDK.Enumerations;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace hVayne.Champions
{
    public class Vayne
    {
        public Vayne()
        {
            VayneOnLoad();
        }

        private static Orbwalker Orbwalker
        {
            get { return Variables.Orbwalker; }
        }

        private static void VayneOnLoad()
        {
            Notifications.Add(new Notification("hVayne - (click and read)", "Vayne is well syncronized with scripting mechanisms. I developed this assembly to increase your ingame performance with Vayne. With this assembly taking a control of your game is inevitable. Take a step in enjoy the smooth work."));
            
            Spells.ExecuteSpells();
            Config.ExecuteMenu();

            Game.OnUpdate += OnUpdate;
            Orbwalker.OnAction += OnAction;
        }

        private static void OnAction(object sender, OrbwalkingActionArgs e)
        {
            if (e.Type == OrbwalkingType.AfterAttack && e.Target.IsEnemy && 
                e.Target.Type == GameObjectType.AIHeroClient)
            {
                if (Orbwalker.ActiveMode == OrbwalkingMode.Combo && Config.Menu["combo.settings"]["combo.q"] && Spells.Q.IsReady()
                    && e.Target.IsValidTarget(777) && Config.ComboMethod.SelectedValue == "Normal")
                {
                    SpellManager.ExecuteQ(((AIHeroClient)e.Target));
                }

                if (Orbwalker.ActiveMode == OrbwalkingMode.Combo && Config.Menu["combo.settings"]["combo.q"] && Spells.Q.IsReady()
                    && e.Target.IsValidTarget(777) && Config.ComboMethod.SelectedValue == "Burst" &&
                    ((AIHeroClient)e.Target).GetBuffCount("vaynesilvereddebuff") >= 1)
                {
                    SpellManager.ExecuteQ(((AIHeroClient)e.Target));
                }

                if (Orbwalker.ActiveMode == OrbwalkingMode.Combo && Config.Menu["activator.settings"]["use.youmuu"] && Items.HasItem(3142)
                    && Items.CanUseItem(3142) && e.Target.IsValidTarget(ObjectManager.Player.AttackRange))
                {
                    Items.UseItem(3142);
                }

                if (Orbwalker.ActiveMode == OrbwalkingMode.Combo && Config.Menu["activator.settings"]["use.botrk"] && Items.HasItem(3153)
                    && Items.CanUseItem(3153) && e.Target.IsValidTarget(550))
                {
                    if ((((AIHeroClient)e.Target).Health / ((AIHeroClient)e.Target).MaxHealth) < Config.Menu["activator.settings"]["botrk.enemy.hp"] && ((ObjectManager.Player.Health / ObjectManager.Player.MaxHealth) < Config.Menu["activator.settings"]["botrk.vayne.hp"]))
                    {
                        Items.UseItem(3153, ((AIHeroClient)e.Target));
                    }
                }

                if (Orbwalker.ActiveMode == OrbwalkingMode.Hybrid && Config.Menu["harass.settings"]["harass.q"] && Spells.Q.IsReady()
                    && ObjectManager.Player.ManaPercent >= Config.Menu["harass.settings"]["harass.mana"] && e.Target.IsValidTarget(777)
                    && ((AIHeroClient)e.Target).GetBuffCount("vaynesilvereddebuff") >= 1 && Config.HarassMenu.SelectedValue == "2W + Q")
                {
                    SpellManager.ExecuteQ(((AIHeroClient)e.Target));
                }

                
            }

            if (e.Type == OrbwalkingType.AfterAttack && e.Target.Type == GameObjectType.obj_AI_Minion && 
                e.Target.Team == GameObjectTeam.Neutral && ObjectManager.Player.ManaPercent >= Config.Menu["jungle.settings"]["jungle.mana"]
                && Spells.Q.IsReady() && Orbwalker.ActiveMode == OrbwalkingMode.LaneClear)
            {
                Spells.Q.Cast(Game.CursorPos);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    OnCombo();
                    break;
                case OrbwalkingMode.LaneClear:
                    OnJungle();
                    break;
                case OrbwalkingMode.Hybrid:
                    OnHybrid();
                    break;
            }
            if (Config.Menu["activator.settings"]["use.qss"] && (Items.HasItem((int)ItemId.Quicksilver_Sash) && Items.CanUseItem((int)ItemId.Quicksilver_Sash) || 
                Items.CanUseItem(3139) && Items.HasItem(3137)))
            {
                ExecuteQss();
            }
            if (Config.Menu["misc.settings"]["auto.orb.buy"] && ObjectManager.Player.Level >= Config.Menu["misc.settings"]["orb.level"]
                && !Items.HasItem((int)ItemId.Farsight_Alteration))
            {
                Shop.BuyItem(ItemId.Farsight_Alteration);
            }
        }

        private static void ExecuteQss()
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

        private static void OnCombo()
        {
            if (Config.Menu["combo.settings"]["combo.e"] && Spells.E.IsReady())
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(ObjectManager.Player.AttackRange)))
                {
                    if (Config.Menu["condemn.settings"]["condemn." + enemy.ChampionName])
                    {
                        SpellManager.ExecuteE(enemy);
                    }
                }
            }
            if (Config.Menu["combo.settings"]["combo.r"] && Spells.R.IsReady() &&
                ObjectManager.Player.CountEnemyHeroesInRange(ObjectManager.Player.AttackRange) >= Config.Menu["combo.settings"]["combo.r.count"]) // edit this part
            {
                Spells.R.Cast();
            }
        }
        private static void OnHybrid()
        {
            if ( ObjectManager.Player.ManaPercent < Config.Menu["harass.settings"]["harass.mana"])
            {
                return;
            }

            if (Config.Menu["harass.settings"]["harass.e"] && Spells.E.IsReady() && Config.HarassMenu.SelectedValue == "2W + E")
            {
                foreach (var enemy in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(Spells.E.Range) && x.GetBuffCount("vaynesilvereddebuff") >= 2))
                {
                    Spells.E.Cast(enemy);
                }
            }
            
        }

        private static void OnJungle()
        {
            if (Config.Menu["jungle.settings"]["jungle.mana"] < ObjectManager.Player.ManaPercent)
            {
                return;
            }

            if (Config.Menu["jungle.settings"]["jungle.e"] && Spells.E.IsReady())
            {
                foreach (var mob in GameObjects.JungleLarge.Where(x=> x.IsValidTarget(Spells.E.Range)))
                {
                    Condemn.JungleCondemn(mob, Config.PushDistance.Value);
                }
            }
        }
    }
}
