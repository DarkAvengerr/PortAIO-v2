using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Core;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Orianna.BallManager
{
    class PetManager
    {
        public Vector3 BallPosition { get; private set; }

        private float LastTick = 0f;

        public void OnLoad()
        {
            BallPosition = ObjectManager.Player.ServerPosition;

            Game.OnUpdate += OnTick;
            Obj_AI_Base.OnSpellCast += OnSCast;
        }

        public void ProcessCommandList(List<Command> commands)
        {
            foreach (var command in commands)
            {
                ProcessCommand(command);
            }
        }

        public void ProcessCommand(Command command)
        {
            switch (command.SpellCommand)
            {
                case Commands.Q:
                    if (command.Where != default(Vector3))
                    {
                        Variables.Spells[SpellSlot.Q].Cast(command.Where);
                    }

                    if (command.Unit != null)
                    {
                        Variables.Spells[SpellSlot.Q].Cast(command.Unit.ServerPosition);
                    }
                    break;
                case Commands.W:
                    Variables.Spells[SpellSlot.W].Cast();
                    break;
                case Commands.E:
                    if (command.Unit.IsValidTarget(float.MaxValue, false))
                    {
                        Variables.Spells[SpellSlot.E].Cast(command.Unit);
                    }
                    break;
                case Commands.R:
                    if (BallPosition.CountEnemiesInRange(300f) > 0)
                    {
                        Variables.Spells[SpellSlot.R].Cast();
                    }
                    break;
            }
        }

        private void OnTick(EventArgs args)
        {
            if (Environment.TickCount - LastTick < 100f)
            {
                return;
            }

            LastTick = Environment.TickCount;

            if (ObjectManager.Player.HasBuff(BallStrings.OriannaSelfShield))
            {
                BallPosition = ObjectManager.Player.ServerPosition;
                return;
            }

            foreach (var ally in HeroManager.Allies)
            {
                if (ally.HasBuff(BallStrings.OriannaShieldAlly))
                {
                    BallPosition = ally.ServerPosition;
                    return;
                }
            }
        }

        private void OnSCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                switch (args.SData.Name)
                {
                    case BallStrings.IzunaCommand:
                        LeagueSharp.Common.Utility.DelayAction.Add((int)(BallPosition.Distance(args.End) / 1.25f - 60 - Game.Ping), () => BallPosition = args.End);
                        BallPosition = Vector3.Zero;
                        break;
                    case BallStrings.RedactCommand:
                        BallPosition = Vector3.Zero;
                        break;
                }
            }
        }
    }
    public enum Commands
    {
        Q, W, E, R
    }

    public class Command
    {
        public Commands SpellCommand { get; set; }

        public Vector3 Where { get; set; }

        public Obj_AI_Base Unit { get; set; }
    }
}
