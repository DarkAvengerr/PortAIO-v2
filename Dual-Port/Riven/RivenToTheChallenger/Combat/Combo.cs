using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using RivenToTheChallenger.Spells;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RivenToTheChallenger.Combat
{
    internal class Combo
    {
        private static readonly BuffType[] FollowUpBuffs =
        {
            BuffType.Suppression, BuffType.Stun, BuffType.Knockup,
            BuffType.Charm, BuffType.Polymorph, BuffType.Fear, BuffType.Knockback
        };

        private readonly Config config;
        private bool waitAA;

        public Combo(Config config)
        {
            this.config = config;
            WindSlash.OnR1Casted += OnR1Casted;
            WindSlash.OnR2Casted += OnR2Casted;
            Obj_AI_Base.OnBuffLose += OnBuffLose;
            BrokenWings.OnPlayAnimation += BrokenWingsOnPlayAnimation;
            Game.OnUpdate += GameOnOnUpdate;
        }

        private bool ComboActive => config.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo;
        private float GapCloseRange => BrokenWings.TotalRange + KiBurst.W.Range;

        private void BrokenWingsOnPlayAnimation(object sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!ComboActive)
            {
                return;
            }
            var delay = 0;
            switch (args.Animation)
            {
                case "Spell1a":
                    delay = 340;
                    break;

                case "Spell1b":
                    delay = 340;
                    break;

                case "Spell1c":
                    delay = 475;
                    break;
                default:
                    return;
            }
            var target = TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player),
                TargetSelector.DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }
            waitAA = true;
            LeagueSharp.Common.Utility.DelayAction.Add(delay, () =>
            {
                Orbwalking.ResetAutoAttackTimer();
                var castedHydra = false;

                #region HydraUsage

                if (config["combo.spells.usehydra"].GetValue<bool>())
                {
                    if (Activator.Ravenous_Hydra_Melee_Only.IsOwned())
                    {
                        if (Activator.Ravenous_Hydra_Melee_Only.IsInRange(target))
                        {
                            castedHydra = Activator.Ravenous_Hydra_Melee_Only.Cast();
                        }
                    }
                    else if (Activator.Tiamat_Melee_Only.IsOwned())
                    {
                        if (Activator.Tiamat_Melee_Only.IsInRange(target))
                        {
                            castedHydra = Activator.Tiamat_Melee_Only.Cast();
                        }
                    }
                }

                #endregion

                if (!castedHydra)
                {
                    EloBuddy.Player.DoEmote(Emote.Dance);
                }
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            });
            LeagueSharp.Common.Utility.DelayAction.Add((int) (400 + 1000/(ObjectManager.Player.AttackSpeedMod*3.75)),
                //Trust the broscience :^ )
                () => { waitAA = false; });
        }

        //Alternative: OnBuffAdd with DelayAction(probably better solution so you don't give them time to react)
        //But this might also be needed for example if the buff gets cleaned or Malzahar R stops being casted
        private void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (!ProcessBuff(sender, args.Buff))
            {
                return;
            }
            if (ObjectManager.Player.Distance(sender,true) > KiBurst.W.WidthSqr)
            {
                return;
            }
            KiBurst.W.Cast();

        }

        private bool ProcessBuff(Obj_AI_Base sender, BuffInstance buffInstance)
        {
            if (sender.IsAlly || !KiBurst.W.IsReady() ||  !ComboActive || !config["combo.spells.usew"].GetValue<bool>())
            {
                return false;
            }
            return FollowUpBuffs.Any(t => t == buffInstance.Type);
        }

        private void GameOnOnUpdate(EventArgs args)
        {
            if (!ComboActive || waitAA)
            {
                return;
            }
            var target =
                TargetSelector.GetTarget(
                    Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + BrokenWings.TotalRange,
                    TargetSelector.DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }
            BrokenWings.Cast(target);
        }

        private void OnR1Casted()
        {
            if (!ComboActive)
            {
                return;
            }
            if (KiBurst.W.IsReady() && config["combo.spells.usew"].GetValue<bool>())
            {
                var target = KiBurst.W.GetTarget();
                if (target.IsValidTarget())
                {
                    KiBurst.W.Cast();
                }
                if (config["combo.spells.useygb"].GetValue<bool>())
                {
                    Activator.Youmuus_Ghostblade.Cast();
                }
            }
            else if (BrokenWings.Q.IsReady() && config["combo.spells.useq"].GetValue<bool>())
            {
                var target = BrokenWings.Q.GetTarget();
                if (target.IsValidTarget())
                {
                    BrokenWings.Q.Cast(target);
                }
            }
        }

        private void OnR2Casted()
        {
            if (!ComboActive)
            {
                return;
            }
            if (!BrokenWings.Q.IsReady() || !config["combo.spells.useq"].GetValue<bool>())
            {
                return;
            }
            var target = BrokenWings.Q.GetTarget();
            if (target.IsValidTarget())
            {
                BrokenWings.Q.Cast(target);
            }
        }
    }
}