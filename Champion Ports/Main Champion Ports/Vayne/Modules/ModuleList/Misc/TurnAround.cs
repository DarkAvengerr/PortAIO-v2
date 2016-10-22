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
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            EloBuddy.Player.OnIssueOrder += OnIssueOrder;
        }

        private void OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (this.ShouldGetExecuted())
            {
                if (sender != null && sender.IsMe)
                {
                    switch (args.Order)
                    {
                        case GameObjectOrder.MoveTo:
                            LastMovementCommandIssued = args.TargetPosition;
                            if (LastCommandTick >= Game.Time)
                            {
                                args.Process = false;
                            }
                            break;
                    }
                }
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (ShouldGetExecuted())
            {
                if (!sender.IsValidTarget() || !(sender is AIHeroClient) || !ObjectManager.Player.IsValidTarget(float.MaxValue, false))
                {
                    return;
                }

                var currentSpell =
                    spells.FirstOrDefault(
                        m =>
                        {
                            var objAiHero = sender as AIHeroClient;
                            return objAiHero != null && (m.SpellName.ToLowerInvariant().Equals(args.SData.Name.ToLowerInvariant()) &&
                                                              m.ChampName.ToLowerInvariant().Equals(objAiHero.ChampionName));
                        });
                if (currentSpell != null)
                {
                    if ((currentSpell.IsTargetted && args.Target.NetworkId == ObjectManager.Player.NetworkId)
                    || ObjectManager.Player.Distance(sender.ServerPosition) + 65f <= currentSpell.Range)
                    {
                        EloBuddy.Player.IssueOrder(
                            GameObjectOrder.MoveTo,
                            sender.ServerPosition.Extend(ObjectManager.Player.ServerPosition,
                                sender.ServerPosition.Distance(ObjectManager.Player.ServerPosition)
                                + (currentSpell.FullTurn ? 130 : -130)));

                        LastCommandTick = Game.Time + currentSpell.CastDelay + 0.25f;

                        LeagueSharp.Common.Utility.DelayAction.Add((int)((currentSpell.CastDelay + 0.20f) * 1000), () => EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, LastMovementCommandIssued));
                    }
                }

            }
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
