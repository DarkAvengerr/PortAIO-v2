using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheTwitch.Commons.Items
{
    class RavenousHydra : IActivateableItem
    {
        private bool _afterAttack;
        private bool _justAttacked;

        public void Initialize(Menu menu, ItemManager itemManager)
        {
            menu.AddMItem("After attack", true, (sender, args) => _afterAttack = args.GetNewValue<bool>());
            menu.ProcStoredValueChanged<bool>();
            Orbwalking.AfterAttack += (sender, args) => _justAttacked = true;
        }

        public string GetDisplayName()
        {
            return "Ravenous Hydra / Tiamat";
        }

        public void Update(AIHeroClient target)
        {
            if ((!_afterAttack || _justAttacked) && ObjectManager.Player.CountEnemiesInRange(400) > 0 || ObjectManager.Player.ChampionName == "Garen" && ObjectManager.Player.HasBuff("GarenE"))
            {
                if (ObjectManager.Player.ChampionName == "Garen")
                {
                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).GetState() == SpellState.Ready)
                        return;
                }
                Use(target);
            }
            _justAttacked = false;
        }

        public void Use(Obj_AI_Base target)
        {
            var itemSpell = ObjectManager.Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "ItemTiamatCleave");
            if (itemSpell != null && itemSpell.GetState() == SpellState.Ready) ObjectManager.Player.Spellbook.CastSpell(itemSpell.Slot, target);
        }

        public int GetRange()
        {
            return 400;
        }

        public TargetSelector.DamageType GetDamageType()
        {
            return TargetSelector.DamageType.Physical;
        }
    }
}
