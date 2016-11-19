using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_Fiora.Evade
{
    using LeagueSharp;
    using LeagueSharp.Common;

    public class EvadeSpellData
    {
        public delegate float MoveSpeedAmount();

        public bool CanShieldAllies;
        public string CheckSpellName = "";
        public int Delay;
        public bool IsDash;
        public bool IsInvulnerability;
        public bool IsMovementSpeedBuff;
        public bool IsShield;
        public bool IsSpellShield;
        public float Range;
        public string Name;
        public bool SelfCast;
        public SpellSlot Slot;
        public int Speed;
        public int _dangerLevel;

        public EvadeSpellData() { }

        public EvadeSpellData(string name, SpellSlot slot, float range, int delay, int speed, int dangerLevel,
            bool isSpellShield = false)
        {
            Name = name;
            Slot = slot;
            Range = range;
            Delay = delay;
            Speed = speed;
            _dangerLevel = dangerLevel;
            IsSpellShield = isSpellShield;
        }

        public int DangerLevel
        {
            get
            {
                return EvadeManager.Menu.Item("DangerLevel" + Name, true) != null
                ? EvadeManager.Menu.Item("DangerLevel" + Name, true).GetValue<Slider>().Value
                : _dangerLevel;
            }
            set { _dangerLevel = value; }
        }

        public bool Enabled
            =>
            EvadeManager.Menu.Item("Enabled" + Name, true) == null ||
            EvadeManager.Menu.Item("Enabled" + Name, true).GetValue<bool>();

        public bool IsReady()
        {
            return (CheckSpellName == "" || ObjectManager.Player.Spellbook.GetSpell(Slot).Name == CheckSpellName) &&
                   ObjectManager.Player.Spellbook.CanUseSpell(Slot) == SpellState.Ready;
        }
    }
}