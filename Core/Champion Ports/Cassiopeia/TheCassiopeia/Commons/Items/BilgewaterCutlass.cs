using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheCassiopeia.Commons.Items
{
    class BilgewaterCutlass : IActivateableItem
    {
        private int _minEnemyHealth;


        public void Initialize(Menu menu, ItemManager itemManager)
        {
            menu.AddMItem("Enemy min HP %", new Slider(20), (sender, args) => _minEnemyHealth = args.GetNewValue<Slider>().Value).ProcStoredValueChanged<Slider>();
        }

        public string GetDisplayName()
        {
            return "Bigewater Cutlass";
        }

        public void Update(AIHeroClient target)
        {
            if (target.HealthPercent >= _minEnemyHealth && target.Distance(ObjectManager.Player) < 550)
            {
                Use(target);
            }
        }

        public void Use(Obj_AI_Base target)
        {
            var itemSpell = ObjectManager.Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "BilgewaterCutlass");
            if (itemSpell != null && itemSpell.IsReady()) ObjectManager.Player.Spellbook.CastSpell(itemSpell.Slot, target);
        }

        public int GetRange()
        {
            return 600;
        }

        public TargetSelector.DamageType GetDamageType()
        {
            return TargetSelector.DamageType.Magical;
        }
    }
}
