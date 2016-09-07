using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheCassiopeia.Commons.Items
{
    class SeraphsEmbrace : IActivateableItem
    {
        public bool UseOnUltimates;
        public int MinHealth;

        public void Initialize(Menu menu, ItemManager itemManager)
        {
            menu.AddMItem("Use on enemy ults", true, (sender, args) => UseOnUltimates = args.GetNewValue<bool>());
            menu.AddMItem("Use when health %  < ", new Slider(40), (sender, args) => MinHealth = args.GetNewValue<Slider>().Value);

            menu.ProcStoredValueChanged<bool>();
            menu.ProcStoredValueChanged<Slider>();

            Spellbook.OnCastSpell += (sender, args) =>
            {
                if (itemManager.IsTickingNow(this) && sender.Owner.IsEnemy && sender.Owner.Type == GameObjectType.AIHeroClient && args.Slot == SpellSlot.R && UseOnUltimates)
                {
                    if (args.Target != null && args.Target.IsValid && args.Target.IsMe)
                    {
                        Use(args.Target as Obj_AI_Base);
                        return;
                    }

                    var halfLineLength = (args.EndPosition - args.StartPosition).Length()/2f;
                    if (ObjectManager.Player.Position.Distance(args.StartPosition) > halfLineLength && ObjectManager.Player.Position.Distance(args.EndPosition) > halfLineLength) return;
                    Use(args.Target as Obj_AI_Base);
                }
            };
        }

        public string GetDisplayName()
        {
            return "Seraphs Embrace";
        }

        public void Update(AIHeroClient target)
        {
            if (ObjectManager.Player.HealthPercent < MinHealth && HeroManager.Enemies.Any(enemy => enemy.IsValidTarget() && enemy.Distance(ObjectManager.Player) < 1500))
                Use(target);
        }

        public void Use(Obj_AI_Base target)
        {
            var itemSpell = ObjectManager.Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "ItemSeraphsEmbrace");
            if (itemSpell != null && itemSpell.IsReady()) ObjectManager.Player.Spellbook.CastSpell(itemSpell.Slot, target);
        }

        public int GetRange()
        {
            return int.MaxValue;
        }

        public TargetSelector.DamageType GetDamageType()
        {
            return TargetSelector.DamageType.True;
        }
    }
}
