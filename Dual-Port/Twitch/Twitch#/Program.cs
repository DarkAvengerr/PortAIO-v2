using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TwitchSharp
{
    public class Program
    {
        private static Menu _config;
        private static Orbwalking.Orbwalker _orbwalker;

        private static Spell _w;
        private static Spell _e;

        private static AIHeroClient Player { get { return ObjectManager.Player; } }



        private static float GetDamage(AIHeroClient target)
        {
            return _e.GetDamage(target);
        }
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            //Verify Champion
            if (Player.ChampionName != "Twitch")
                return;

            //Spells
            _w = new Spell(SpellSlot.W, 950);
            _w.SetSkillshot(0.25f, 120f, 1400f, false, SkillshotType.SkillshotCircle);
            _e = new Spell(SpellSlot.E, 1200);

            //Menu instance
            _config = new Menu("Twitch", "Twitch", true);

            //Orbwalker
            _orbwalker = new Orbwalking.Orbwalker(_config.SubMenu("Orbwalking"));

            //Targetsleector
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            _config.AddSubMenu(targetSelectorMenu);

            //Combo
            _config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));

            //Misc
            _config.SubMenu("Misc").AddItem(new MenuItem("blueTrinket", "Buy Blue Trinket on Level 6").SetValue(true));
            _config.SubMenu("Misc").AddItem(new MenuItem("EKillsteal", "Killsteal with E").SetValue(true));
            _config.SubMenu("Misc").AddItem(new MenuItem("EDamage", "E damage on healthbar").SetValue(new Circle(true, Color.Green)));
            _config.SubMenu("Misc").AddItem(new MenuItem("Emobs", "Kill mobs with E").SetValue(new StringList(new [] { "Baron + Dragon + Siege Minion", "Baron + Dragon", "None" })));

            //Attach to root
            _config.AddToMainMenu();

            // Enable E damage indicators
            CustomDamageIndicator.Initialize(GetDamage);

            //Listen to events
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            // E damage on healthbar
            CustomDamageIndicator.DrawingColor = _config.Item("EDamage").GetValue<Circle>().Color;
            CustomDamageIndicator.Enabled = _config.Item("EDamage").GetValue<Circle>().Active;

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (_e.IsReady())
            {
                //Killsteal with E
                if (_config.Item("EKillsteal").GetValue<bool>())
                {

                    foreach (
                        var enemy in
                            ObjectManager.Get<AIHeroClient>()
                                .Where(enemy => enemy.IsValidTarget(_e.Range) && _e.IsKillable(enemy))
                        )
                    {
                        _e.Cast();
                    }
                }

                //Kill large monsters
                if (_orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                {
                    var minions = MinionManager.GetMinions(_e.Range, MinionTypes.All, MinionTeam.NotAlly);
                    foreach (var m in minions)
                    {
                        switch (_config.Item("Emobs").GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                if ((m.BaseSkinName.Contains("MinionSiege") || m.BaseSkinName.Contains("Dragon") || m.BaseSkinName.Contains("Baron")) && _e.IsKillable(m))
                                {
                                    _e.Cast();
                                }
                                break;

                            case 1:
                                if ((m.BaseSkinName.Contains("Dragon") || m.BaseSkinName.Contains("Baron")) && _e.IsKillable(m))
                                {
                                    _e.Cast();
                                }
                                break;

                            case 2:
                                return;
                                break;
                        }
                    }
                }
            }   

            //Combo/Items
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var target = TargetSelector.GetTarget(_w.Range, TargetSelector.DamageType.Physical);

                //Use W
                if (_config.Item("UseWCombo").GetValue<bool>())
                {
                    if (target.IsValidTarget(_w.Range) && _w.CanCast(target))
                    {
                        _w.Cast(target);
                    }
                }

                //Use Botrk
                if (target != null && target.Type == Player.Type &&
                    target.ServerPosition.Distance(Player.ServerPosition) < 450)
                {
                    var hasCutGlass = Items.HasItem(3144);
                    var hasBotrk = Items.HasItem(3153);

                    if (hasBotrk || hasCutGlass)
                    {
                        var itemId = hasCutGlass ? 3144 : 3153;
                        var damage = Player.GetItemDamage(target, Damage.DamageItems.Botrk);
                        if (hasCutGlass || Player.Health + damage < Player.MaxHealth)
                            Items.UseItem(itemId, target);
                    }
                }

                //Use Youmus
                if (target != null && target.Type == Player.Type && Orbwalking.InAutoAttackRange(target))
                {
                    Items.UseItem(3142);
                }
            }

            //Auto buy blue trinket
            if (_config.Item("blueTrinket").GetValue<bool>() && Player.Level >= 6 && Player.InShop() && !(Items.HasItem(3342) || Items.HasItem(3363)))
            {
                Shop.BuyItem(ItemId.Farsight_Alteration);
            }
        }
    }
}
