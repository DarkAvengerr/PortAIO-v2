using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace TheCassiopeia.Commons.Items
{
    class Botrk : IActivateableItem
    {
        private int _maxPlayerHealth;
        private int _minEnemyHealth;


        public void Initialize(Menu menu, ItemManager itemManager)
        {
            menu.AddMItem("Player max HP %", new Slider(80), (sender, args) => _maxPlayerHealth = args.GetNewValue<Slider>().Value);
            menu.AddMItem("Enemy min HP %", new Slider(20), (sender, args) => _minEnemyHealth = args.GetNewValue<Slider>().Value);
            menu.ProcStoredValueChanged<Slider>();
        }

        public string GetDisplayName()
        {
            return "Blade of the Ruined King";
        }

        public void Update(AIHeroClient target)
        {
            if ((target.HealthPercent >= _minEnemyHealth || ObjectManager.Player.HealthPercent < 20) && ObjectManager.Player.HealthPercent <= _maxPlayerHealth && target.Distance(ObjectManager.Player) < 550)
            {
                Use(target);
            }
        }

        public void Use(Obj_AI_Base target)
        {
            var itemSpell = ObjectManager.Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "ItemSwordOfFeastAndFamine");
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
