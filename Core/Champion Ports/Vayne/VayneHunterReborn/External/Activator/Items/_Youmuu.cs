using LeagueSharp;
using LeagueSharp.Common;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.MenuUtility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace VayneHunter_Reborn.External.Activator.Items
{
    class _Youmuu : IVHRItem
    {
        public void OnLoad()
        {
            Orbwalking.AfterAttack += AfterAttack;
        }

        public void BuildMenu(Menu RootMenu)
        {
            var itemMenu = new Menu("Youmuu's Ghostblade", "dz191.vhr.activator.offensive.youmuu");
            {
                itemMenu.AddItem(new MenuItem("dz191.vhr.activator.offensive.youmuu.combo", "Use In Combo")).SetValue(true);
                itemMenu.AddItem(new MenuItem("dz191.vhr.activator.offensive.youmuu.mixed", "Use In Harass")).SetValue(false);
                itemMenu.AddItem(new MenuItem("dz191.vhr.activator.offensive.youmuu.always", "Use Always")).SetValue(false);
                RootMenu.AddSubMenu(itemMenu);
            }
        }

        public IVHRItemType GetItemType()
        {
            return IVHRItemType.Offensive;
        }

        public bool ShouldRun()
        {
            return GetItemObject().IsReady();
        }

        public void Run(){}

        public int GetItemId()
        {
            return 3142;
        }

        public float GetItemRange()
        {
            return ObjectManager.Player.AttackRange + 65f + 65f + 150f;
        }

        public LeagueSharp.Common.Items.Item GetItemObject()
        {
            return new LeagueSharp.Common.Items.Item(GetItemId(), GetItemRange());
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
                        $"dz191.vhr.activator.offensive.youmuu.{Variables.Orbwalker.ActiveMode.ToString().ToLower()}");
                var currentValue = currentMenuItem?.GetValue<bool>() ?? false;


                if (currentValue || MenuExtensions.GetItemValue<bool>("dz191.vhr.activator.offensive.youmuu.always"))
                {
                    if (TargetHero.IsValidTarget(GetItemRange()))
                    {
                        GetItemObject().Cast();
                    }
                }
            }
        }
    }
}
