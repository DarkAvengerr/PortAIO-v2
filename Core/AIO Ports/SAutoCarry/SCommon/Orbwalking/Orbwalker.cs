using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SCommon;
using SCommon.Database;
using SCommon.Packet;
//typedefs
using TargetSelector = SCommon.TS.TargetSelector;
using EloBuddy;

namespace SCommon.Orbwalking
{
    public class Orbwalker
    {
        /// <summary>
        /// The Orbwalker mode enum.
        /// </summary>
        public enum Mode
        {
            None,
            Combo,
            Mixed,
            LaneClear,
            LastHit,
        }

        private Random m_rnd;
        private int m_lastAATick;
        private int m_lastWindUpEndTick;
        private int m_lastWindUpTime;
        private int m_lastAttackCooldown;
        private int m_lastAttackCompletesAt;
        private int m_lastMoveTick;
        private int m_lastAttackTick;
        private float m_baseAttackSpeed;
        private float m_baseWindUp;
        private bool m_attackInProgress;
        private bool m_rengarAttack;
        private bool m_Attack;
        private bool m_Move;
        private bool m_IslastCastedAA;
        private Vector2 m_lastAttackPos;
        private Vector3 m_orbwalkingPoint;
        private ConfigMenu m_Configuration;
        private bool m_orbwalkEnabled;
        private AttackableUnit m_forcedTarget;
        private bool m_attackReset;
        private AttackableUnit m_lastTarget;
        private Obj_AI_Base m_towerTarget;
        private Obj_AI_Base m_sourceTower;
        private int m_towerAttackTick;
        private bool m_channelingWait;
        private Func<bool> m_fnCanAttack;
        private Func<bool> m_fnCanMove;
        private Func<AttackableUnit, bool> m_fnCanOrbwalkTarget;
        private Func<bool> m_fnShouldWait;

        /// <summary>
        /// The orbwalker constructor
        /// </summary>
        /// <param name="menuToAttach">The menu to attach.</param>
        public Orbwalker(Menu menuToAttach)
        {
            m_rnd = new Random();
            m_lastAATick = 0;
            m_lastWindUpEndTick = 0;
            m_lastMoveTick = 0;
            m_Attack = true;
            m_Move = true;
            m_baseWindUp = 1f / (ObjectManager.Player.AttackCastDelay * ObjectManager.Player.GetAttackSpeed());
            m_baseAttackSpeed = 1f / (ObjectManager.Player.AttackDelay * ObjectManager.Player.GetAttackSpeed());
            m_orbwalkingPoint = Vector3.Zero;
            m_Configuration = new ConfigMenu(this, menuToAttach);
            m_orbwalkEnabled = true;
            m_forcedTarget = null;
            m_lastTarget = null;
            m_fnCanAttack = null;
            m_fnCanMove = null;
            m_fnCanOrbwalkTarget = null;
            m_fnShouldWait = null;

            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnDoCast;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffAdd;
            Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffRemove;
            Obj_AI_Base.OnNewPath += Obj_AI_Base_OnNewPath;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            Spellbook.OnStopCast += Spellbook_OnStopCast;
            Obj_AI_Base.OnDamage += Obj_AI_Base_OnDamage;
            PacketHandler.Register(0x31, PacketHandler_AfterAttack);
            PacketHandler.Register(0x155, PacketHandler_CancelWindup);
            new Drawings(this);
        }

        /// <summary>
        /// Gets Orbwalker's active mode
        /// </summary>
        public Mode ActiveMode
        {
            get
            {
                if (m_Configuration.Combo)
                    return Mode.Combo;

                if (m_Configuration.Harass)
                    return Mode.Mixed;

                if (m_Configuration.LaneClear)
                    return Mode.LaneClear;

                if (m_Configuration.LastHit)
                    return Mode.LastHit;

                return Mode.None;
            }
        }

        /// <summary>
        /// Gets Last Auto Attack Tick
        /// </summary>
        public int LastAATick
        {
            get { return m_lastAATick; }
        }

        /// <summary>
        /// Gets Last WindUp tick
        /// </summary>
        public int LastWindUpEndTick
        {
            get { return m_lastWindUpEndTick; }
        }

        /// <summary>
        /// Gets Last Movement tick
        /// </summary>
        public int LastMoveTick
        {
            get { return m_lastMoveTick; }
        }

        /// <summary>
        /// Gets Configuration menu;
        /// </summary>
        public ConfigMenu Configuration
        {
            get { return m_Configuration; }
        }

        /// <summary>
        /// Gets or sets orbwalking point
        /// </summary>
        public Vector3 OrbwalkingPoint
        {
            get { return m_orbwalkingPoint == Vector3.Zero ? Game.CursorPos : m_orbwalkingPoint; }
            set { m_orbwalkingPoint = value; }
        }

        /// <summary>
        /// Gets or sets orbwalking is enabled
        /// </summary>
        public bool Enabled
        {
            get { return m_orbwalkEnabled; }
            set { m_orbwalkEnabled = value; }
        }

        /// <summary>
        /// Gets or sets forced orbwalk target
        /// </summary>
        public AttackableUnit ForcedTarget
        {
            get { return m_forcedTarget; }
            set { m_forcedTarget = value; }
        }

        /// <summary>
        /// Gets base attack speed value
        /// </summary>
        public float BaseAttackSpeed
        {
            get { return m_baseAttackSpeed; }
        }

        /// <summary>
        /// Gets base windup value
        /// </summary>
        public float BaseWindup
        {
            get { return m_baseWindUp; }
        }

        /// <summary>
        /// Resets auto attack timer
        /// </summary>
        public void ResetAATimer()
        {
            if (m_baseAttackSpeed != 0.5f)
            {
                m_lastAATick = Utils.TickCount - Game.Ping / 2 - m_lastAttackCooldown;
                m_lastAttackTick = 0;
                m_attackReset = true;
                m_attackInProgress = false;
            }
        }

        /// <summary>
        /// Resets orbwalk values
        /// </summary>
        public void ResetOrbwalkValues()
        {
            m_baseAttackSpeed = 0.5f;
        }
        
        /// <summary>
        /// Sets orbwalk values
        /// </summary>
        public void SetOrbwalkValues()
        {
            m_baseWindUp = 1f / (ObjectManager.Player.AttackCastDelay * ObjectManager.Player.GetAttackSpeed());
            m_baseAttackSpeed = 1f / (ObjectManager.Player.AttackDelay * ObjectManager.Player.GetAttackSpeed());
        }

        /// <summary>
        /// Sets attack value
        /// </summary>
        public void SetAttack(bool set)
        {
            m_Attack = set;
        }

        /// <summary>
        /// Sets move value
        /// </summary>
        public void SetMove(bool set)
        {
            m_Move = set;
        }

        /// <summary>
        /// Sets can orbwalk while channeling spell
        /// </summary>
        /// <param name="set">The orbwalker will orbwalk if the set is <c>false</c></param>
        public void SetChannelingWait(bool set)
        {
            m_channelingWait = set;
        }

        /// <summary>
        /// Checks if player can attack
        /// </summary>
        /// <returns>true if can attack</returns>
        public bool CanAttack(int t = 0)
        {
            if (!m_Attack)
                return false;

            if (m_attackReset)
                return true;

            if (m_fnCanAttack != null)
                return m_fnCanAttack();

            return EloBuddy.SDK.Orbwalker.CanAutoAttack;
        }

        /// <summary>
        /// Checks if player can move
        /// </summary>
        /// <returns>true if can move</returns>
        public bool CanMove(int t = 0)
        {
            if (!m_Move)
                return false;

            if (m_fnCanMove != null)
                return m_fnCanMove();

            return EloBuddy.SDK.Orbwalker.CanMove;
        }

        /// <summary>
        /// Checks if player can orbwalk given target
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns>true if can orbwalk target</returns>
        public bool CanOrbwalkTarget(AttackableUnit target)
        {
            if (target == null)
                return false;

            if (m_fnCanOrbwalkTarget != null)
                return m_fnCanOrbwalkTarget(target);

            if (target.IsValidTarget())
            {
                if (target.Type == GameObjectType.AIHeroClient)
                {
                    AIHeroClient hero = target as AIHeroClient;
                    return ObjectManager.Player.Distance(hero.ServerPosition) - hero.BoundingRadius - hero.GetScalingRange() + 20 < Utility.GetAARange();
                }
                else
                    return (target.Type != GameObjectType.obj_AI_Turret || m_Configuration.AttackStructures) && ObjectManager.Player.Distance(target.Position) - target.BoundingRadius + 20 < Utility.GetAARange();
            }
            return false;
        }

        /// <summary>
        /// Checks if player can orbwalk given target in custom range
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="range">Custom range</param>
        /// <returns>true if can orbwalk target</returns>
        public bool CanOrbwalkTarget(AttackableUnit target, float range)
        {
            if (target.IsValidTarget())
            {
                if (target.Type == GameObjectType.AIHeroClient)
                {
                    AIHeroClient hero = target as AIHeroClient;
                    return ObjectManager.Player.Distance(hero.ServerPosition) - hero.BoundingRadius - hero.GetScalingRange() + 10 < range + ObjectManager.Player.BoundingRadius + ObjectManager.Player.GetScalingRange();
                }
                else
                    return ObjectManager.Player.Distance(target.Position) - target.BoundingRadius + 20 < range + ObjectManager.Player.BoundingRadius + ObjectManager.Player.GetScalingRange();
            }
            return false;
        }

        /// <summary>
        /// Checks if player can orbwalk given target from custom position
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="position">Custom position</param>
        /// <returns>true if can orbwalk target</returns>
        public bool CanOrbwalkTarget(AttackableUnit target, Vector3 position)
        {
            if (target.IsValidTarget())
            {
                if (target.Type == GameObjectType.AIHeroClient)
                {
                    AIHeroClient hero = target as AIHeroClient;
                    return position.Distance(hero.ServerPosition) - hero.BoundingRadius - hero.GetScalingRange() < Utility.GetAARange();
                }
                else
                    return position.Distance(target.Position) - target.BoundingRadius < Utility.GetAARange();
            }
            return false;
        }

        /// <summary>
        /// Orbwalk itself
        /// </summary>
        /// <param name="target">Target</param>
        public void Orbwalk(AttackableUnit target)
        {
            Orbwalk(target, OrbwalkingPoint);
        }

        /// <summary>
        /// Orbwalk itself
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="point">Orbwalk point</param>
        public void Orbwalk(AttackableUnit target, Vector3 point)
        {
            if (!m_attackInProgress)
            {
                if (CanOrbwalkTarget(target))
                {
                    if (CanAttack())
                    {
                        BeforeAttackArgs args = Events.FireBeforeAttack(this, target);
                        if (args.Process)
                        {
                            if(!m_Configuration.DisableAA || target.Type != GameObjectType.AIHeroClient)
                                Attack(target);
                        }
                        else
                        {
                            if (EloBuddy.SDK.Orbwalker.CanMove)
                            {
                                if (m_Configuration.DontMoveInRange && target.Type == GameObjectType.AIHeroClient)
                                    return;

                                if ((m_Configuration.LegitMode && !ObjectManager.Player.IsMelee) || !m_Configuration.LegitMode)
                                    Move(point);
                            }
                        }
                    }
                    else if (EloBuddy.SDK.Orbwalker.CanMove)
                    {
                        if (m_Configuration.DontMoveInRange && target.Type == GameObjectType.AIHeroClient)
                            return;

                        if ((m_Configuration.LegitMode && !ObjectManager.Player.IsMelee) || !m_Configuration.LegitMode)
                            Move(point);
                    }
                }
                else
                    Move(point);
            }
        }
       
        /// <summary>
        /// Gets AA Animation Time
        /// </summary>
        /// <returns></returns>
        private float GetAnimationTime()
        {
            return 1f / (ObjectManager.Player.GetAttackSpeed() * m_baseAttackSpeed);
        }

        /// <summary>
        /// Gets AA Windup Time
        /// </summary>
        /// <returns></returns>
        private float GetWindupTime()
        {
            return 1f / (ObjectManager.Player.GetAttackSpeed() * m_baseWindUp) + m_Configuration.ExtraWindup;
        }

        /// <summary>
        /// Orders move hero to given position
        /// </summary>
        /// <param name="pos"></param>
        private void Move(Vector3 pos)
        {
            if (!m_attackInProgress && EloBuddy.SDK.Orbwalker.CanMove && (!CanAttack(60) || EloBuddy.SDK.Orbwalker.CanAutoAttack))
            {
                Vector3 playerPos = ObjectManager.Player.ServerPosition;

                bool holdzone = m_Configuration.DontMoveMouseOver || m_Configuration.HoldAreaRadius != 0;
                var holdzoneRadiusSqr = Math.Max(m_Configuration.HoldAreaRadius * m_Configuration.HoldAreaRadius, ObjectManager.Player.BoundingRadius * ObjectManager.Player.BoundingRadius * 4);
                if (holdzone && playerPos.Distance(pos, true) < holdzoneRadiusSqr)
                {
                    if ((Utils.TickCount + Game.Ping / 2 - m_lastAATick) * 0.6f >= 1000f / (ObjectManager.Player.GetAttackSpeed() * m_baseWindUp))
                        EloBuddy.Player.IssueOrder(GameObjectOrder.Stop, playerPos);
                    m_lastMoveTick = Utils.TickCount + m_rnd.Next(1, 20);
                    return;
                }

                if (ObjectManager.Player.Distance(pos, true) < 22500)
                    pos = playerPos.Extend(pos, (m_rnd.NextFloat(0.6f, 1.01f) + 0.2f) * 400);


                if (m_lastMoveTick + 50 + Math.Min(60, Game.Ping) < Utils.TickCount)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, pos);
                    m_lastMoveTick = Utils.TickCount + m_rnd.Next(1, 20);
                }
            }
        }
        
        /// <summary>
        /// Orders attack hero to given target
        /// </summary>
        /// <param name="target"></param>
        private void Attack(AttackableUnit target)
        {
            if (m_lastAttackTick < Utils.TickCount && !m_attackInProgress)
            {
                m_lastWindUpEndTick = 0;
                m_lastAttackTick = Utils.TickCount + (int)Math.Floor(ObjectManager.Player.AttackDelay * 1000);
                m_lastAATick = Utils.TickCount + Game.Ping;
                m_attackInProgress = true;
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }
        }

        /// <summary>
        /// Magnets the hero to given target
        /// </summary>
        /// <param name="target">The target.</param>
        private void Magnet(AttackableUnit target)
        {
            if (!m_attackInProgress && !CanOrbwalkTarget(target))
            {
                if (ObjectManager.Player.AttackRange <= m_Configuration.StickRange)
                {
                    if (target.IsValidTarget(m_Configuration.StickRange))
                    {
                        /*expermential*/
                        OrbwalkingPoint = target.Position.Extend(ObjectManager.Player.ServerPosition, -(m_rnd.NextFloat(0.6f, 1.01f) + 0.2f) * 400);
                        /*expermential*/
                    }
                    else
                        OrbwalkingPoint = Vector3.Zero;
                }
                else
                    OrbwalkingPoint = Vector3.Zero;
            }
            else
                OrbwalkingPoint = Vector3.Zero;
        }

        /// <summary>
        /// The event which called after an attack
        /// </summary>
        /// <param name="target">The target.</param>
        private void AfterAttack(AttackableUnit target)
        {
            m_lastWindUpEndTick = Utils.TickCount;
            m_attackReset = false;
            m_attackInProgress = false;
            m_lastMoveTick = 0;
            Events.FireAfterAttack(this, target);
        }

        /// <summary>
        /// Gets laneclear target
        /// </summary>
        /// <returns></returns>
        private Obj_AI_Base GetLaneClearTarget()
        {
            foreach (var minion in MinionManager.GetMinions(ObjectManager.Player.AttackRange + 100f).OrderByDescending(p => ObjectManager.Player.GetAutoAttackDamage(p)))
            {
                if (CanOrbwalkTarget(minion))
                {
                    var pred = EloBuddy.SDK.Prediction.Health.GetPrediction(minion, (int)(ObjectManager.Player.AttackDelay * 1000 * 2) + 30);
                    if (pred >= 2 * Damage.AutoAttack.GetDamage(minion, true) || Damage.Prediction.IsLastHitable(minion))
                    {
                        return minion;
                    }
                }
            }
            var mob = GetJungleClearTarget();
            if (mob != null)
                return mob;

            return null;
        }

        /// <summary>
        /// Gets jungleclear target
        /// </summary>
        /// <returns></returns>
        private Obj_AI_Base GetJungleClearTarget()
        {
            Obj_AI_Base mob = null;
            if (Game.MapId == GameMapId.SummonersRift || Game.MapId == GameMapId.TwistedTreeline)
            {
                int mobPrio = 0;
                foreach (var minion in MinionManager.GetMinions(2000, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth))
                {
                    if (CanOrbwalkTarget(minion))
                    {
                        int prio = minion.GetJunglePriority();
                        if (minion.Health < ObjectManager.Player.GetAutoAttackDamage(minion))
                            return minion;
                        else
                        {
                            if (mob == null)
                            {
                                mob = minion;
                                mobPrio = prio;
                            }
                            else if (prio < mobPrio)
                            {
                                mob = minion;
                                mobPrio = prio;
                            }
                        }
                    }
                }
            }
            return mob;
        }

        /// <summary>
        /// Finds the last hit minion
        /// </summary>
        /// <returns></returns>
        private Obj_AI_Base FindKillableMinion()
        {
            if (m_Configuration.SupportMode)
                return null;

            foreach (var minion in MinionManager.GetMinions(ObjectManager.Player.AttackRange + 100f).OrderBy(p => ObjectManager.Player.GetAutoAttackDamage(p)))
            {
                if (CanOrbwalkTarget(minion) && Damage.Prediction.IsLastHitable(minion))
                    return minion;
            }
            return null;
        }

        /// <summary>
        /// Checks if the orbwalker should wait to lasthit
        /// </summary>
        /// <returns><c>true</c> if should wait</returns>
        public bool ShouldWait()
        {
            if (m_towerTarget != null && m_towerTarget.IsValidTarget() && CanOrbwalkTarget(m_towerTarget) && !m_towerTarget.IsSiegeMinion())
                return true;

            var underTurret = ObjectManager.Get<Obj_AI_Turret>().FirstOrDefault(p => p.IsValidTarget() && p.Distance(ObjectManager.Player.ServerPosition) < 950f && p.IsAlly);

            if (underTurret != null)
            {
                return ObjectManager.Get<Obj_AI_Minion>()
                    .Any(
                        minion => minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral &&
                                  MinionManager.IsMinion(minion, false) && !minion.IsSiegeMinion() &&
                                  underTurret.Distance(minion.ServerPosition) < 950f);
                
            }

            if (m_fnShouldWait != null)
                return m_fnShouldWait();

            return
                ObjectManager.Get<Obj_AI_Minion>()
                    .Any(
                        minion =>
                            minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral &&
                            Utility.InAARange(minion) && MinionManager.IsMinion(minion) &&
                            HealthPrediction.LaneClearHealthPrediction(
                                    minion, (int)(ObjectManager.Player.AttackDelay * 1000f * 2f + ObjectManager.Player.AttackCastDelay * 1000f), 30) <=
                                Damage.AutoAttack.GetDamage(minion));

        }

        /// <summary>
        /// Gets orbwalker target
        /// </summary>
        /// <returns></returns>
        public AttackableUnit GetTarget()
        {
            bool wait = false;
            if(ActiveMode == Mode.LaneClear)
                wait = ShouldWait();

            if (ActiveMode == Mode.LaneClear || ActiveMode == Mode.LastHit || ActiveMode == Mode.Mixed)
            {
                //turret farming
                if (m_towerTarget != null && m_sourceTower != null && m_sourceTower.IsValidTarget(float.MaxValue, false) && m_towerTarget.IsValidTarget() && CanOrbwalkTarget(m_towerTarget, ObjectManager.Player.AttackRange + 150f))
                {
                    float health = m_towerTarget.Health - Damage.Prediction.GetPrediction(m_towerTarget, (m_towerTarget.Distance(m_sourceTower.ServerPosition) / m_sourceTower.BasicAttack.MissileSpeed + m_sourceTower.AttackCastDelay) * 1000f);
                    if (Damage.Prediction.IsLastHitable(m_towerTarget))
                        return m_towerTarget;

                    if (m_towerTarget.Health - m_sourceTower.GetAutoAttackDamage(m_towerTarget) * 2f > 0)
                        return null;

                    else if (m_towerTarget.Health - m_sourceTower.GetAutoAttackDamage(m_towerTarget) > 0)
                    {
                        if (m_towerTarget.Health - m_sourceTower.GetAutoAttackDamage(m_towerTarget) - Damage.AutoAttack.GetDamage(m_towerTarget) <= 0)
                            return null;
                        else if (health - m_sourceTower.GetAutoAttackDamage(m_towerTarget) - Damage.AutoAttack.GetDamage(m_towerTarget) * 2f <= 0)
                            return m_towerTarget;
                    }

                    if (m_Configuration.FocusNormalWhileTurret)
                        return FindKillableMinion();

                    if (m_towerTarget.Health - Damage.AutoAttack.GetDamage(m_towerTarget) * 2f <= 0)
                    {
                        float twoAaTime = ObjectManager.Player.AttackDelay + ObjectManager.Player.AttackCastDelay + 2 * (ObjectManager.Player.Distance(m_towerTarget.ServerPosition) / Utility.GetProjectileSpeed());
                        float towerAaTime = m_sourceTower.AttackCastDelay + m_sourceTower.Distance(m_towerTarget.ServerPosition)  / m_sourceTower.BasicAttack.MissileSpeed;
                        if (twoAaTime <= towerAaTime)
                            return m_towerTarget;
                    }

                    return null;
                }
                var killableMinion = FindKillableMinion();
                if (killableMinion != null && !killableMinion.IsDead && killableMinion.IsHPBarRendered && killableMinion.IsHPBarRendered && killableMinion.IsTargetable)
                    return killableMinion;
            }

            if (m_forcedTarget != null && m_forcedTarget.IsValidTarget() && Utility.InAARange(m_forcedTarget) && !m_forcedTarget.IsDead && m_forcedTarget.IsVisible && m_forcedTarget.IsTargetable)
                return m_forcedTarget;

            //buildings
            if (ActiveMode == Mode.LaneClear && m_Configuration.AttackStructures && !wait)
            {
                /* turrets */
                foreach (var turret in
                    ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsValidTarget() && Utility.InAARange(t)))
                {
                    return turret;
                }

                /* inhibitor */
                foreach (var turret in
                    ObjectManager.Get<Obj_BarracksDampener>().Where(t => t.IsValidTarget() && Utility.InAARange(t)))
                {
                    return turret;
                }

                /* nexus */
                foreach (var nexus in
                    ObjectManager.Get<Obj_HQ>().Where(t => t.IsValidTarget() && Utility.InAARange(t)))
                {
                    return nexus;
                }
            }

            //champions
            if (ActiveMode != Mode.LastHit)
            {
                if (ActiveMode == Mode.LaneClear && wait)
                    return null;

                if ((ActiveMode == Mode.LaneClear && !m_Configuration.DontAttackChampWhileLaneClear) || ActiveMode == Mode.Combo || ActiveMode == Mode.Mixed)
                {
                    float range = -1;
                    range = (ObjectManager.Player.IsMelee && m_Configuration.MagnetMelee && m_Configuration.StickRange > ObjectManager.Player.AttackRange) ? m_Configuration.StickRange : -1;
                    if (ObjectManager.Player.CharData.BaseSkinName == "Azir")
                        range = 1000f;
                    var target = TargetSelector.GetTarget(range, LeagueSharp.Common.TargetSelector.DamageType.Physical);
                    if (target != null)
                    {
                        if (!target.IsDead && target.IsHPBarRendered && target.IsHPBarRendered && target.IsTargetable && target.IsValidTarget() && (Utility.InAARange(target) || (ActiveMode != Mode.LaneClear && ObjectManager.Player.IsMelee && m_Configuration.MagnetMelee && target.IsValidTarget(m_Configuration.StickRange))))
                            return target;
                    }
                }
            }

            if (!wait)
            {
                if (ActiveMode == Mode.LaneClear)
                {
                    var minion = GetLaneClearTarget();
                    if (minion != null && minion.IsValidTarget() && Utility.InAARange(minion) && !minion.IsDead && minion.IsHPBarRendered && minion.IsTargetable)
                        return minion;
                }
            }
            return null;
        }


        /// <summary>
        /// Registers the CanAttack function
        /// </summary>
        /// <param name="fn">The function.</param>
        public void RegisterCanAttack(Func<bool> fn)
        {
            m_fnCanAttack = fn;
        }

        /// <summary>
        /// Registers the CanMove function
        /// </summary>
        /// <param name="fn">The function.</param>
        public void RegisterCanMove(Func<bool> fn)
        {
            m_fnCanMove = fn;
        }

        /// <summary>
        /// Registers the CanOrbwalkTarget function
        /// </summary>
        /// <param name="fn">The function.</param>
        public void RegisterCanOrbwalkTarget(Func<AttackableUnit, bool> fn)
        {
            m_fnCanOrbwalkTarget = fn;
        }

        /// <summary>
        /// Registers the ShouldWait function
        /// </summary>
        /// <param name="fn">The function</param>
        public void RegisterShouldWait(Func<bool> fn)
        {
            m_fnShouldWait = fn;
        }

        /// <summary>
        /// Unregisters the CanAttack function
        /// </summary>
        public void UnRegisterCanAttack()
        {
            m_fnCanAttack = null;
        }

        /// <summary>
        /// Unregisters the CanMove function
        /// </summary>
        public void UnRegisterCanMove()
        {
            m_fnCanMove = null;
        }

        /// <summary>
        /// Unregisters the CanOrbwalkTarget function
        /// </summary>
        public void UnRegisterCanOrbwalkTarget()
        {
            m_fnCanOrbwalkTarget = null;
        }

        /// <summary>
        /// Unregisters the ShouldWait function
        /// </summary>
        public void UnRegisterShouldWait()
        {
            m_fnShouldWait = null;
        }

        /// <summary>
        /// Game.OnUpdate event
        /// </summary>
        /// <param name="args">The args.</param>
        private void Game_OnUpdate(EventArgs args)
        {
            if (ActiveMode == Mode.None || (ObjectManager.Player.IsCastingInterruptableSpell(true) && m_channelingWait) || ObjectManager.Player.IsDead)
                return;

            if (CanMove() && m_attackInProgress)
                m_attackInProgress = false;

            var t = GetTarget();
            if (t != null)
                m_lastTarget = t;

            if (ObjectManager.Player.IsMelee && m_Configuration.MagnetMelee && t is AIHeroClient)
                Magnet(t);
            else
                OrbwalkingPoint = Vector3.Zero;

            Orbwalk(t);
        }

        /// <summary>
        /// OnProcessSpellCast event for detect the auto attack and auto attack resets
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (Utility.IsAutoAttack(args.SData.Name))
                {
                    m_IslastCastedAA = true;
                    OnAttackArgs onAttackArgs = Events.FireOnAttack(this, args.Target as AttackableUnit);
                    if (!onAttackArgs.Cancel)
                    {
                        m_lastAATick = Utils.TickCount - Game.Ping / 2;
                        m_lastWindUpTime = (int)Math.Round(sender.AttackCastDelay * 1000);
                        m_lastAttackCooldown = (int)Math.Round(sender.AttackDelay * 1000);
                        m_lastAttackCompletesAt = m_lastAATick + m_lastWindUpTime;
                        m_lastAttackPos = ObjectManager.Player.ServerPosition.To2D();
                        m_attackInProgress = true;
                    }
                    if (m_baseAttackSpeed == 0.5f)
                        SetOrbwalkValues();
                }
                else
                {
                    m_IslastCastedAA = false;
                    if (Utility.IsAutoAttackReset(args.SData.Name))
                    {
                        ResetAATimer();
                    }
                    else if (!Utility.IsAutoAttackReset(args.SData.Name))
                    {
                        if (m_attackInProgress)
                            ResetAATimer();
                    }
                    else if (args.SData.Name == "AspectOfTheCougar")
                    {
                        ResetOrbwalkValues();
                    }
                }
            }
            else
            {
                if (sender.Type == GameObjectType.obj_AI_Turret && args.Target.Type == GameObjectType.obj_AI_Minion && sender.Team == ObjectManager.Player.Team && args.Target.Position.Distance(ObjectManager.Player.ServerPosition) <= 2000)
                {
                    m_towerTarget = args.Target as Obj_AI_Base;
                    m_sourceTower = sender;
                    m_towerAttackTick = Utils.TickCount - Game.Ping / 2;
                }
            }
        }

        /// <summary>
        /// OnNewPath event for the detect rengar leap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Obj_AI_Base_OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.IsMe && args.IsDash && sender.CharData.BaseSkinName == "Rengar")
            {
                Events.FireOnAttack(this, m_lastTarget);
                m_lastAATick = Utils.TickCount - Game.Ping / 2;
                m_lastWindUpTime = (int)(sender.AttackCastDelay * 1000);
                m_lastAttackCooldown = (int)(sender.AttackDelay * 1000);
                m_lastAttackCompletesAt = m_lastAATick + m_lastWindUpTime;
                m_lastAttackPos = ObjectManager.Player.ServerPosition.To2D();
                m_attackInProgress = true;
                m_rengarAttack = true;
                if (m_baseAttackSpeed == 0.5f)
                    SetOrbwalkValues();
            }
        }

        /// <summary>
        /// OnDamage event for detect rengar leap's end
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Obj_AI_Base_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (args.Source.NetworkId == ObjectManager.Player.NetworkId && ObjectManager.Player.CharData.BaseSkinName == "Rengar" && m_rengarAttack)
            {
                AfterAttack(m_lastTarget);
                m_rengarAttack = false;
            }
        }

        /// <summary>
        /// AfterAttack event for detect after attack for heroes which has projectile
        /// </summary>
        /// <param name="data"></param>
        private void PacketHandler_AfterAttack(byte[] data)
        {
            if (BitConverter.ToInt32(data, 2) == ObjectManager.Player.NetworkId && m_IslastCastedAA && m_attackInProgress)
            {
                m_lastAATick = Utils.TickCount - (int)Math.Ceiling(GetWindupTime()) - Game.Ping;
                AfterAttack(m_lastTarget);
            }
        }

        private void PacketHandler_CancelWindup(byte[] data)
        {
            if (BitConverter.ToInt32(data, 2) == ObjectManager.Player.NetworkId)
                ResetAATimer();
        }

        /// <summary>
        /// OnDoCast event for detect after attack for heroes which hasnt projectile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Obj_AI_Base_OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (Utility.IsAutoAttack(args.SData.Name) && (m_attackInProgress || !Utility.HasProjectile()))
                    AfterAttack(args.Target as AttackableUnit);
            }
        }

        /// <summary>
        /// OnBuffRemoveEvent for detect orbwalk value changes
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void Obj_AI_Base_OnBuffRemove(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe)
            {
                string buffname = args.Buff.Name.ToLower();
                if (buffname == "swainmetamorphism" || buffname == "gnartransform")
                    ResetOrbwalkValues();

                if (Data.IsImmobilizeBuff(args.Buff.Type))
                    ResetAATimer();
            }
        }

        /// <summary>
        /// OnBuffAdd for detect orbwalk value changes
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void Obj_AI_Base_OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender.IsMe)
            {
                string buffname = args.Buff.Name.ToLower();
                if (buffname == "jaycestancegun" || buffname == "jaycestancehammer" || buffname == "swainmetamorphism" || buffname == "gnartransform")
                    ResetOrbwalkValues();
            }
        }

        /// <summary>
        /// OnPlayAnimation Event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            //if (sender.IsMe && m_attackInProgress && (args.Animation == "Run" || args.Animation == "Idle"))
            //{
            //    Chat.Print("{0} ({1})", args.Animation, Utils.TickCount);
            //    ResetAATimer();
            //}
        }

        /// <summary>
        /// OnStopCast Event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void Spellbook_OnStopCast(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
        {
            if (sender.IsValid && sender.IsMe && args.DestroyMissile && args.StopAnimation)
                ResetAATimer();
        }
    }
}
