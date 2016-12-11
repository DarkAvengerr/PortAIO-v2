using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using VayneHunter_Reborn.Modules.ModuleHelpers;
using VayneHunter_Reborn.Utility.MenuUtility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace VayneHunter_Reborn.Modules.ModuleList.Misc
{
    class TurnAround : IModule
    {
        private List<TurnAroundSpell> spells = new List<TurnAroundSpell>()
        {
            new TurnAroundSpell()
            {
                ChampName = "Tryndamere",
                SpellName = "MockingShout",
                Range = 875f,
                CastDelay = 0.60f,
                IsTargetted = true,
                FullTurn = false
            },
            new TurnAroundSpell()
            {
                ChampName = "Cassiopeia",
                SpellName = "CassiopeiaR",
                Range = 900f,
                CastDelay = 0.80f,
                IsTargetted = false,
                FullTurn = true
            }
        };

        private Vector3 LastMovementCommandIssued = Vector3.Zero;

        private float LastCommandTick = 0f;

        public void OnLoad()
        {
        }

        private void OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
        }

        public bool ShouldGetExecuted()
        {
            return (MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.general.turnaround"));
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            
        }
    }

    class TurnAroundSpell
    {
        public string ChampName { get; set; }

        public string SpellName { get; set; }

        public float Range { get; set; }

        public float CastDelay { get; set; }

        public bool IsTargetted { get; set; }

        public bool FullTurn { get; set; }
    }
}
