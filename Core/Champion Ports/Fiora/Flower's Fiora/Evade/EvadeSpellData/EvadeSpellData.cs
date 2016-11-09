using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Fiora.Evade
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class EvadeSpellData
    {
        public string Name;
        public string CheckSpellName = "";

        public int _dangerLevel;
        public int Delay;
        public int Speed;

        public float MaxRange;

        public bool CanShieldAllies;
        public bool FixedRange;
        public bool Invert;
        public bool IsBlink;
        public bool IsDash;
        public bool IsInvulnerability;
        public bool IsMovementSpeedBuff;
        public bool IsShield;
        public bool IsSpellShield;
        public bool IsSummonerSpell;
        public bool RequiresPreMove;
        public bool SelfCast;

        public SpellSlot Slot;

        public MoveSpeedAmount MoveSpeedTotalAmount;

        public SpellValidTargets[] ValidTargets;

        public delegate float MoveSpeedAmount();

        public EvadeSpellData()
        {
            
        }

        public EvadeSpellData(string name, int dangerLevel)
        {
            Name = name;
            _dangerLevel = dangerLevel;
        }

        public bool IsTargetted => ValidTargets != null;

        public int DangerLevel
            =>
            Config.Menu.Item("DangerLevel" + Name) != null
                ? Config.Menu.Item("DangerLevel" + Name).GetValue<Slider>().Value
                : _dangerLevel;

        public bool Enabled
            => Config.Menu.Item("Enabled" + Name) == null || Config.Menu.Item("Enabled" + Name).GetValue<bool>();

        public bool IsReady()
        {
            return (CheckSpellName == "" || Player.Instance.Spellbook.GetSpell(Slot).Name == CheckSpellName) &&
                   ((IsSummonerSpell && ObjectManager.Player.Spellbook.CanUseSpell(Slot) == SpellState.Ready) ||
                    (!IsSummonerSpell && ObjectManager.Player.Spellbook.CanUseSpell(Slot) == SpellState.Ready));
        }
    }
}