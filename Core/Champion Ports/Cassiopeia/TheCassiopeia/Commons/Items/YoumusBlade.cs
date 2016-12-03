using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace TheCassiopeia.Commons.Items
{
    class YoumusBlade : IActivateableItem
    {
        private int _minEnemyHealth;
        private bool _onlyTwitchUlt;

        public void Initialize(Menu menu, ItemManager itemManager)
        {
            menu.AddMItem("Enemy min HP %", new Slider(20), (sender, args) => _minEnemyHealth = args.GetNewValue<Slider>().Value).ProcStoredValueChanged<Slider>();
            if (ObjectManager.Player.ChampionName == "Twitch")
            {
                menu.AddMItem("Only in Twitch ult", true, (sender, args) => _onlyTwitchUlt = args.GetNewValue<bool>()).ProcStoredValueChanged<bool>();
            }
        }

        public string GetDisplayName()
        {
            return "Youmus Ghostblade";
        }

        public void Update(AIHeroClient target)
        {
            if (target.HealthPercent >= _minEnemyHealth && (!_onlyTwitchUlt || ObjectManager.Player.HasBuff("TwitchFullAutomatic")))
            {
                Use(target);
            }
        }

        public void Use(Obj_AI_Base target)
        {
            var itemSpell = ObjectManager.Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "YoumusBlade");
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
