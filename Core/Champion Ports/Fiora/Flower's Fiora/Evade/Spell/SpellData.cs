using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Fiora.Evade
{
    using LeagueSharp;

    public class SpellData
    {
        public int _radius;
        public int _range;
        public int RingRadius;
        public int DangerValue;
        public int Delay;
        public int ExtraDuration;
        public int MissileMaxSpeed;
        public int MissileMinSpeed;
        public int MissileSpeed;
        public int Id = -1;
        public int ExtraRange = -1;
        public int MissileAccel = 0;
        public int MultipleNumber = -1;

        public float MultipleAngle;

        public bool AddHitbox;
        public bool Centered;
        public bool Invert;
        public bool FixedRange;
        public bool MissileDelayed;
        public bool MissileFollowsUnit;
        public bool ForceRemove = false;
        public bool FollowCaster = false;
        public bool CanBeRemoved = false;
        public bool DisabledByDefault = false;
        public bool DisableFowDetection = false;
        public bool DontAddExtraDuration;
        public bool DontCheckForDuplicates = false;
        public bool DontCross = false;
        public bool DontRemove = false;
        public bool IsDangerous = false;
        public bool TakeClosestPath = false;

        public string SpellName;
        public string ChampionName;
        public string FromObject = "";
        public string MissileSpellName = "";
        public string SourceObjectName = "";
        public string ToggleParticleName = "";
        public string[] FromObjects = { };
        public string[] ExtraSpellNames = { };
        public string[] ExtraMissileNames = { };

        public SpellSlot Slot;

        public SkillShotType Type;

        public EarlyObjects[] EarlyEvade;

        public CollisionObjectTypes[] CollisionObjects = { };

        public SpellData()
        {
            
        }

        public SpellData(string championName, string spellName, SpellSlot slot, SkillShotType type, int delay, int range,
            int radius, int missileSpeed, bool addHitbox, bool fixedRange, int defaultDangerValue)
        {
            ChampionName = championName;
            SpellName = spellName;
            Slot = slot;
            Type = type;
            Delay = delay;
            Range = range;
            _radius = radius;
            MissileSpeed = missileSpeed;
            AddHitbox = addHitbox;
            FixedRange = fixedRange;
            DangerValue = defaultDangerValue;
        }

        public string MenuItemName => ChampionName + " - " + SpellName;

        public int Radius
        {
            get
            {
                return !AddHitbox
                    ? _radius + Config.SkillShotsExtraRadius
                    : Config.SkillShotsExtraRadius + _radius + (int) ObjectManager.Player.BoundingRadius;
            }
            set { _radius = value; }
        }

        public int RawRadius => _radius;

        public int RawRange => _range;

        public int Range
        {
            get
            {
                return _range +
                       (Type == SkillShotType.SkillshotLine || Type == SkillShotType.SkillshotMissileLine
                           ? Config.SkillShotsExtraRange
                           : 0);
            }
            set { _range = value; }
        }
    }
}
