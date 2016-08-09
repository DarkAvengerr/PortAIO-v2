using DZLib.Logging;
using iSeriesReborn.Utility;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.External.Activator.Items
{
    class _Youmuu : ISRItem
    {
        public void OnLoad()
        {
            Orbwalking.AfterAttack += AfterAttack;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        public void BuildMenu(Menu RootMenu)
        {
            var itemMenu = new Menu("Youmuu's Ghostblade", "iseriesr.activator.offensive.youmuu");
            {
                itemMenu.AddItem(new MenuItem("iseriesr.activator.offensive.youmuu.combo", "Use In Combo")).SetValue(true);
                itemMenu.AddItem(new MenuItem("iseriesr.activator.offensive.youmuu.mixed", "Use In Harass")).SetValue(false);
                itemMenu.AddItem(new MenuItem("iseriesr.activator.offensive.youmuu.always", "Use Always")).SetValue(false);
                RootMenu.AddSubMenu(itemMenu);
            }
        }

        public ISRItemType GetItemType()
        {
            return ISRItemType.Offensive;
        }

        public bool ShouldRun()
        {
            return LeagueSharp.Common.Items.HasItem(3142) && LeagueSharp.Common.Items.CanUseItem(3142);
        }

        public void Run()
        {
            
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (ShouldRun() && ObjectManager.Player.ChampionName == "Twitch")
            {
                if (args.Slot == SpellSlot.R)
                {
                    LeagueSharp.Common.Items.UseItem(3142);
                }
            }
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (ShouldRun())
            {
                if (!(target is AIHeroClient))
                {
                    return;
                }

                var TargetHero = (AIHeroClient) target;

                var currentMenuItem =
                    Variables.Menu.Item(
                        $"iseriesr.activator.offensive.youmuu.{Variables.Orbwalker.ActiveMode.ToString().ToLower()}");
                var currentValue = currentMenuItem != null ? currentMenuItem.GetValue<bool>() : false;


                if (currentValue || MenuExtensions.GetItemValue<bool>("iseriesr.activator.offensive.youmuu.always"))
                {
                    if (TargetHero.IsValidTarget(ObjectManager.Player.AttackRange + 65f + 65f + 150f))
                    {
                        LeagueSharp.Common.Items.UseItem(3142);
                    }
                }
            }
        }
    }
}
