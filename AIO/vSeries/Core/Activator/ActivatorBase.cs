using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using vSupport_Series.Champions;
using vSupport_Series.Core.Database;
using vSupport_Series.Core.Plugins;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace vSupport_Series.Core.Activator
{

    class ActivatorBase
    {
        private static Menu _config;
        private static readonly int MikaelItemId = ItemData.Mikaels_Crucible.Id;
        private static readonly float MikaelRange = ItemData.Mikaels_Crucible.Range;
        private static readonly int MountainId = ItemData.Face_of_the_Mountain.Id;
        private static readonly float MountainRange = ItemData.Face_of_the_Mountain.Range;
        private static readonly int SolariItemId = ItemData.Locket_of_the_Iron_Solari.Id;
        private const float SolariRange = 600;

        public ActivatorBase()
        {
            ActivatorBaseOnLoad();
        }

        public static void ActivatorBaseOnLoad()
        {

            _config = new Menu("vSupport Series: Activator", "vSupport Series: Activator", true);
            {
                var mikaelmenu = new Menu(":: Mikael (Activator)", ":: Mikael (Activator)");
                {
                    mikaelmenu.AddItem(new MenuItem("mikael", "Enabled ?").SetValue(true));

                    var specialbuffs = new Menu(":: Special Debuffs", ":: Special Debuffs");
                    {
                        specialbuffs.AddItem(new MenuItem("mikael.ignite", "Ignite").SetValue(true));
                        specialbuffs.AddItem(new MenuItem("mikael.exhaust", "Exhaust").SetValue(true));
                        specialbuffs.AddItem(new MenuItem("mikael.zed", "Zed (R)").SetValue(true));
                        specialbuffs.AddItem(new MenuItem("mikael.fizz", "Fizz (R)").SetValue(true));
                        specialbuffs.AddItem(new MenuItem("mikael.malzahar", "Malzahar (R)").SetValue(true));
                        specialbuffs.AddItem(new MenuItem("mikael.vladimir", "Vladimir (R)").SetValue(true));
                        mikaelmenu.AddSubMenu(specialbuffs);
                    }

                    var buffmenu = new Menu(":: Debuff List", ":: Debuff List");
                    {
                        buffmenu.AddItem(new MenuItem("mikael.charm", "Charm").SetValue(true));
                        buffmenu.AddItem(new MenuItem("mikael.snare", "Snare").SetValue(true));
                        buffmenu.AddItem(new MenuItem("mikael.polymorph", "Polymorph").SetValue(true));
                        buffmenu.AddItem(new MenuItem("mikael.stun", "Stun").SetValue(true));
                        buffmenu.AddItem(new MenuItem("mikael.suppression", "Suppression").SetValue(true));
                        buffmenu.AddItem(new MenuItem("mikael.taunt", "Taunt").SetValue(true));
                        mikaelmenu.AddSubMenu(buffmenu);
                    }

                    var whitelist = new Menu(":: Whitelist", ":: Whitelist");
                    {
                        foreach (var ally in HeroManager.Allies)
                        {
                            whitelist.AddItem(new MenuItem("mikael." + ally.ChampionName, "(Mikael) : " + ally.ChampionName).SetValue(Helper.HighChamps.Contains(ally.ChampionName)));
                        }
                        mikaelmenu.AddSubMenu(whitelist);
                    }
                    _config.AddSubMenu(mikaelmenu);
                }

                /*
                
                 var mountainmenu = new Menu(":: Face of Mountain (Activator)", ":: Face of Mountain (Activator)");
                 {
                     mountainmenu.AddItem(new MenuItem("mountain", "Enabled ?").SetValue(true));

                     var evademenux = new Menu(":: Protectable Skillshots", ":: Protectable Skillshots");
                     {
                         foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.EvadeableSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsSkillshot)))
                         {
                             evademenux.AddItem(new MenuItem(string.Format("mountain.protect.{0}", spell.SpellName), string.Format("{0} ({1})", spell.ChampionName, spell.Slot)).SetValue(true));
                         }
                         mountainmenu.AddSubMenu(evademenux);
                     }
                     var targettedmenu = new Menu(":: Protectable Targetted Spells", ":: Protectable Targetted Spells");
                     {
                         foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.TargetedSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsTargeted)))
                         {
                             targettedmenu.AddItem(new MenuItem(string.Format("mountain.protect.targetted.{0}", spell.SpellName), string.Format("{0} ({1})", spell.ChampionName, spell.Slot)).SetValue(true));
                         }
                         mountainmenu.AddSubMenu(targettedmenu);
                     }

                     var whitelist = new Menu(":: Whitelist", ":: Whitelist");
                     {
                         foreach (var ally in HeroManager.Allies)
                         {
                             whitelist.AddItem(new MenuItem("mountain." + ally.ChampionName, "(Mountain) : " + ally.ChampionName).SetValue(Helper.HighChamps.Contains(ally.ChampionName)));
                         }
                         mountainmenu.AddSubMenu(whitelist);
                     }
                     _config.AddSubMenu(mountainmenu);
                 }
                  */

                /*solari menu*/
                /*var solarimenu = new Menu(":: Iron Solari (Activator)", ":: Iron Solari (Activator)");
                {
                    solarimenu.AddItem(new MenuItem("solari", "Enabled ?").SetValue(true));

                    var evademenu = new Menu(":: Protectable Skillshots", ":: Protectable Skillshots");
                    {
                        foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.EvadeableSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsSkillshot)))
                        {
                            evademenu.AddItem(new MenuItem(string.Format("solari.protect.{0}", spell.SpellName), string.Format("{0} ({1})", spell.ChampionName, spell.Slot)).SetValue(true));
                        }
                        solarimenu.AddSubMenu(evademenu);
                    }
                    var targettedmenu = new Menu(":: Protectable Targetted Spells", ":: Protectable Targetted Spells");
                    {
                        foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.TargetedSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsTargeted)))
                        {
                            targettedmenu.AddItem(new MenuItem(string.Format("solari.protect.targetted.{0}", spell.SpellName), string.Format("{0} ({1})", spell.ChampionName, spell.Slot)).SetValue(true));
                        }
                        solarimenu.AddSubMenu(targettedmenu);
                    }

                    var whitelist = new Menu(":: Whitelist", ":: Whitelist");
                    {
                        foreach (var ally in HeroManager.Allies)
                        {
                            whitelist.AddItem(new MenuItem("solari." + ally.ChampionName, "(Solari) : " + ally.ChampionName).SetValue(HighChamps.Contains(ally.ChampionName)));
                        }
                        solarimenu.AddSubMenu(whitelist);
                    }
                    _config.AddSubMenu(solarimenu);
                }
            
                */
                _config.AddToMainMenu();
            }



            Chat.Print("<font color='#ff3232'>vSupport Series: </font><font color='#d4d4d4'>Activator loaded!</font>");
            /*Game.OnUpdate += ActivatorBaseOnUpdate;
            Obj_AI_Base.OnSpellCast += ActivatorBaseOnProcessSpellCast;*/
        }

        private static void ActivatorBaseOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            /*mountain protect from skillshots*/
            if (sender is AIHeroClient && args.Target.IsAlly && args.Target.Type == GameObjectType.AIHeroClient
                && !args.SData.IsAutoAttack() && ((_config.Item("mountain.protect." + args.SData.Name).GetValue<bool>() && _config.Item("mountain.protect." + args.SData.Name) != null))
                && sender.IsEnemy && sender.GetSpellDamage(((AIHeroClient)args.Target), args.SData.Name) > ((AIHeroClient)args.Target).Health && Items.HasItem(MountainId) && Items.CanUseItem(MountainId)
                && ((AIHeroClient)args.Target).Distance(ObjectManager.Player.Position) < MountainRange && !((AIHeroClient)args.Target).IsDead)
            {
                Items.UseItem(MountainId, ((AIHeroClient)args.Target));
            }

            /*mountain protect from targetted spells*/
            if (sender is AIHeroClient && args.Target.IsAlly && args.Target.Type == GameObjectType.AIHeroClient
                && !args.SData.IsAutoAttack() && ((_config.Item("mountain.protect.targetted." + args.SData.Name).GetValue<bool>() && _config.Item("mountain.protect.targetted." + args.SData.Name) != null))
                && sender.IsEnemy && sender.GetSpellDamage(((AIHeroClient)args.Target), args.SData.Name) > ((AIHeroClient)args.Target).Health && Items.HasItem(MountainId) && Items.CanUseItem(MountainId)
                && ((AIHeroClient)args.Target).Distance(ObjectManager.Player.Position) < MountainRange && !((AIHeroClient)args.Target).IsDead && args.SData.TargettingType == SpellDataTargetType.Unit)
            {
                Items.UseItem(MountainId, ((AIHeroClient)args.Target));
            }

            /*iron solari protect from skillshots*/
            if (sender is AIHeroClient && args.Target.IsAlly && args.Target.Type == GameObjectType.AIHeroClient
                && !args.SData.IsAutoAttack() && ((_config.Item("solari.protect." + args.SData.Name).GetValue<bool>() && _config.Item("solari.protect." + args.SData.Name) != null))
                && sender.IsEnemy && sender.GetSpellDamage(((AIHeroClient)args.Target), args.SData.Name) > ((AIHeroClient)args.Target).Health && Items.HasItem(SolariItemId) && Items.CanUseItem(SolariItemId)
                && ((AIHeroClient)args.Target).Distance(ObjectManager.Player.Position) < SolariRange && !((AIHeroClient)args.Target).IsDead)
            {
                Items.UseItem(SolariItemId, ((AIHeroClient)args.Target));
            }
            /*iron solari protect from targetted spells*/
            if (sender is AIHeroClient && args.Target.IsAlly && args.Target.Type == GameObjectType.AIHeroClient
                && !args.SData.IsAutoAttack() && ((_config.Item("solari.protect.targetted." + args.SData.Name).GetValue<bool>() && _config.Item("solari.protect.targetted." + args.SData.Name) != null))
                && sender.IsEnemy && sender.GetSpellDamage(((AIHeroClient)args.Target), args.SData.Name) > ((AIHeroClient)args.Target).Health && Items.HasItem(SolariItemId) && Items.CanUseItem(SolariItemId)
                && ((AIHeroClient)args.Target).Distance(ObjectManager.Player.Position) < SolariRange && !((AIHeroClient)args.Target).IsDead && args.SData.TargettingType == SpellDataTargetType.Unit)
            {
                Items.UseItem(SolariItemId, ((AIHeroClient)args.Target));
            }
        }
        private static void ActivatorBaseOnUpdate(EventArgs args)
        {
            if (Items.HasItem(MikaelItemId) && Items.CanUseItem(MikaelItemId) && Helper.MenuCheck("mikael", _config))
            {
                foreach (var ally in HeroManager.Allies.Where(x => x.IsValidTarget(MikaelRange) && Helper.MenuCheck("mikael." + x.ChampionName, _config)))
                {
                    if (ally.HasBuff("summonerexhaust") && Helper.MenuCheck("mikael.exhaust", _config))
                    {
                        Items.UseItem(MikaelItemId, ally);
                    }
                    else if (ally.HasBuff("summonerdot") && Helper.MenuCheck("mikael.ignite", _config))
                    {
                        Items.UseItem(MikaelItemId, ally);
                    }
                    else if (ally.HasBuff("zedulttargetmark") && Helper.MenuCheck("mikael.zed", _config))
                    {
                        Items.UseItem(MikaelItemId, ally);
                    }
                    else if (ally.HasBuff("FizzMarinerDoom") && Helper.MenuCheck("mikael.fizz", _config))
                    {
                        Items.UseItem(MikaelItemId, ally);
                    }
                    else if (ally.HasBuff("AlZaharNetherGrasp") && Helper.MenuCheck("mikael.malzahar", _config))
                    {
                        Items.UseItem(MikaelItemId, ally);
                    }
                    else if (ally.HasBuff("VladimirHemoplague") && Helper.MenuCheck("mikael.vladimir", _config))
                    {
                        Items.UseItem(MikaelItemId, ally);
                    }
                    else if (ally.HasBuff("summonerexhaust") && Helper.MenuCheck("mikael.exhaust", _config))
                    {
                        Items.UseItem(MikaelItemId, ally);
                    }
                    /*classical buffs*/
                    else if (ally.HasBuffOfType(BuffType.Charm) && Helper.MenuCheck("mikael.charm", _config))
                    {
                        Items.UseItem(MikaelItemId, ally);
                    }
                    else if (ally.HasBuffOfType(BuffType.Snare) && Helper.MenuCheck("mikael.snare", _config))
                    {
                        Items.UseItem(MikaelItemId, ally);
                    }
                    else if (ally.HasBuffOfType(BuffType.Polymorph) && Helper.MenuCheck("mikael.polymorph", _config))
                    {
                        Items.UseItem(MikaelItemId, ally);
                    }
                    else if (ally.HasBuffOfType(BuffType.Stun) && Helper.MenuCheck("mikael.stun", _config))
                    {
                        Items.UseItem(MikaelItemId, ally);
                    }
                    else if (ally.HasBuffOfType(BuffType.Suppression) && Helper.MenuCheck("mikael.suppression", _config))
                    {
                        Items.UseItem(MikaelItemId, ally);
                    }
                    else if (ally.HasBuffOfType(BuffType.Taunt) && Helper.MenuCheck("mikael.taunt", _config))
                    {
                        Items.UseItem(MikaelItemId, ally);
                    }
                }
            }
            else
            {
                Console.WriteLine("There is no Mikael :roto2:");
            }
        }


    }

}
