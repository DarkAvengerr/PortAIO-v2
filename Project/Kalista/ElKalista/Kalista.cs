using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using LeagueSharp.Common.Data;
using Collision = LeagueSharp.Common.Collision;
using ItemData = LeagueSharp.Common.Data.ItemData;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElKalista
{
    internal enum Spells
    {
        Q,
        W,
        E,
        R
    }

    internal class Kalista
    {
        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static String ScriptVersion { get { return typeof(Kalista).Assembly.GetName().Version.ToString(); } }
        public static Orbwalking.Orbwalker Orbwalker;
        private static AIHeroClient ConnectedAlly;
        private static Dictionary<float, float> _incomingDamage = new Dictionary<float, float>();
        private static Dictionary<float, float> _instantDamage = new Dictionary<float, float>();
        public static float IncomingDamage
        {
            get { return _incomingDamage.Sum(e => e.Value) + _instantDamage.Sum(e => e.Value); }
        }

        public static Dictionary<Spells, Spell> spells = new Dictionary<Spells, Spell>()
        {
            { Spells.Q, new Spell(SpellSlot.Q, 1180) },
            { Spells.W, new Spell(SpellSlot.W, 5200) },
            { Spells.E, new Spell(SpellSlot.E, 1000) },
            { Spells.R, new Spell(SpellSlot.R, 1400) }
        };

        #region autoW
        /*private static void AutoW()
        {
            var useW = ElKalistaMenu._menu.Item("ElKalista.misc.autow").GetValue<bool>();
            if (useW)
            {
                if (spells[Spells.W].IsReady() && Player.ManaPercent > 80 && ObjectManager.Player.Distance(SummonersRift.River.Dragon) <= spells[Spells.W].Range && !Player.IsAttackingPlayer && !Player.IsMoving)
                {
                    spells[Spells.W].Cast(SummonersRift.River.Dragon, true);
                }
                if (spells[Spells.W].IsReady() && Player.ManaPercent > 80 && ObjectManager.Player.Distance(SummonersRift.River.Baron) <= spells[Spells.W].Range && !Player.IsAttackingPlayer && !Player.IsMoving)
                {
                    spells[Spells.W].Cast(SummonersRift.River.Baron, true);
                }
                if (spells[Spells.W].IsReady() && Player.ManaPercent > 80 && ObjectManager.Player.Distance(SummonersRift.Jungle.Blue_BlueBuff) <= spells[Spells.W].Range && Player.Team == GameObjectTeam.Chaos && !Player.IsAttackingPlayer && !Player.IsMoving)
                {
                    spells[Spells.W].Cast(SummonersRift.Jungle.Blue_BlueBuff, true);
                }
                if (spells[Spells.W].IsReady() && Player.ManaPercent > 80 && ObjectManager.Player.Distance(SummonersRift.Jungle.Blue_BlueBuff) <= spells[Spells.W].Range && Player.Team == GameObjectTeam.Chaos && !Player.IsAttackingPlayer && !Player.IsMoving)
                {
                    spells[Spells.W].Cast(SummonersRift.Jungle.Blue_RedBuff, true);
                }
                if (spells[Spells.W].IsReady() && Player.ManaPercent > 80 && ObjectManager.Player.Distance(SummonersRift.Jungle.Blue_BlueBuff) <= spells[Spells.W].Range && Player.Team == GameObjectTeam.Order && !Player.IsAttackingPlayer && !Player.IsMoving)
                {
                    spells[Spells.W].Cast(SummonersRift.Jungle.Red_BlueBuff, true);
                }
                if (spells[Spells.W].IsReady() && Player.ManaPercent > 80 && ObjectManager.Player.Distance(SummonersRift.Jungle.Blue_BlueBuff) <= spells[Spells.W].Range && Player.Team == GameObjectTeam.Order)
                {
                    spells[Spells.W].Cast(SummonersRift.Jungle.Red_RedBuff, true);
                }
            }
        }*/
        #endregion

        #region hitchance

        private static HitChance CustomHitChance
        {
            get { return GetHitchance(); }
        }

        private static HitChance GetHitchance()
        {
            switch (ElKalistaMenu._menu.Item("ElKalista.hitChance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.Medium;
            }
        }

        #endregion

        #region Gameloaded 

        public static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Kalista") return;
           
            Console.WriteLine("Injected");

            Notifications.AddNotification(String.Format("ElKalista by jQuery v{0}", ScriptVersion), 10000);
            spells[Spells.Q].SetSkillshot(0.25f, 40f, 1700f, true, SkillshotType.SkillshotLine);

            ElKalistaMenu.Initialize();
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawings.Drawing_OnDraw;
            Spellbook.OnCastSpell += OnCastSpell;
            Orbwalking.OnNonKillableMinion += Orbwalking_OnNonKillableMinion;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        #endregion

        #region OnGameUpdate

        private static void OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Physical);

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }
          
            KsMode();
            JungleStealMode();
            SaveMode();
            SemiUltMode();
            AutoCastEMode();
            //AutoW();
        }

        #endregion

        #region SuperSeCrEtSeTtInGs

        private static void AutoCastEMode()
        {
            var target = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Physical);

            if (target == null || !target.IsValidTarget())
                return;

            var useE = ElKalistaMenu._menu.Item("ElKalista.E.Auto").GetValue<bool>();
            var useEStacks = ElKalistaMenu._menu.Item("ElKalista.E.Stacks").GetValue<Slider>().Value;

            if (spells[Spells.E].IsReady() && useE && target.Buffs.Count(buf => buf.Name == "kalistaexpungemarker") >= useEStacks)
            {
                if (target.IsRendKillable())
                {
                    spells[Spells.E].Cast();
                }
            }
        }

        private static void SemiUltMode()
        {
            var manualUlt = ElKalistaMenu._menu.Item("ElKalista.SemiR").GetValue<KeyBind>().Active;

            if (spells[Spells.R].IsReady() && manualUlt)
            {
                spells[Spells.R].Cast();
            }
        }

        private static void SaveMode()
        {
            if (Player.IsRecalling() || Player.InFountain())
                return;

            var save = ElKalistaMenu._menu.Item("ElKalista.misc.save").GetValue<bool>();
            var allyHp = ElKalistaMenu._menu.Item("ElKalista.misc.allyhp").GetValue<Slider>().Value;
            var kaliscrank = ElKalistaMenu._menu.Item("ElKalista.misc.kaliscrank").GetValue<bool>();

            if (save)
            {
                if (ConnectedAlly == null)
                {
                    foreach (var cAlly in from ally in ObjectManager.Get<AIHeroClient>().Where(b => b.IsAlly && !b.IsDead && !b.IsMe) where Player.Distance(ally) < spells[Spells.R].Range from buff in ally.Buffs where ally.HasBuff("kalistacoopstrikeally") select ally)
                    {
                        ConnectedAlly = cAlly;
                        break;
                    }
                }
                else
                {
                    if (ConnectedAlly.HealthPercent < allyHp && ConnectedAlly.CountEnemiesInRange(500) > 0)
                    {
                        spells[Spells.R].Cast();
                    }
                    else
                    {
                        foreach (var unit in ObjectManager.Get<AIHeroClient>().Where(h => h.IsEnemy && h.IsHPBarRendered && ConnectedAlly.Distance(h.Position) > 800))
                        {
                            // Get buffs
                            for (int i = 0; i < unit.Buffs.Count(); i++)
                            {
                                // Check if the Soulbound is in a good range
                                var enemy = HeroManager.Enemies.Where(x => ConnectedAlly.Distance(unit.Position) > 800);
                                // Check if the Soulbound is a Blitzcrank
                                // Check if the enemy is hooked
                                // Check if target was far enough for ult
                                if (ConnectedAlly.ChampionName == "Blitzcrank" && unit.Buffs[i].Name == "rocketgrab2" && unit.Buffs[i].IsActive && enemy.Count() > 0 && kaliscrank)
                                {
                                    spells[Spells.R].Cast();
                                }
                            }
                        }
                    }
                }
            }
        }

        //credits to hellsing
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy)
            {
                if (ConnectedAlly != null && spells[Spells.R].IsReady())
                {
                    if ((!(sender is AIHeroClient) || args.SData.IsAutoAttack()) && args.Target != null && args.Target.NetworkId == ConnectedAlly.NetworkId)
                    {
                        _incomingDamage.Add(ConnectedAlly.ServerPosition.Distance(sender.ServerPosition) / args.SData.MissileSpeed + Game.Time, (float)sender.GetAutoAttackDamage(ConnectedAlly));
                    }
                    else if (sender is AIHeroClient)
                    {
                        var attacker = (AIHeroClient)sender;
                        var slot = attacker.GetSpellSlot(args.SData.Name);

                        if (slot != SpellSlot.Unknown)
                        {
                            if (slot == attacker.GetSpellSlot("SummonerDot") && args.Target != null && args.Target.NetworkId == ConnectedAlly.NetworkId)
                            {
                                _instantDamage.Add(Game.Time + 2, (float)attacker.GetSummonerSpellDamage(ConnectedAlly, Damage.SummonerSpell.Ignite));
                            }
                            else if (slot.HasFlag(SpellSlot.Q | SpellSlot.W | SpellSlot.E | SpellSlot.R) &&
                                ((args.Target != null && args.Target.NetworkId == ConnectedAlly.NetworkId) ||
                                args.End.Distance(ConnectedAlly.ServerPosition) < Math.Pow(args.SData.LineWidth, 2)))
                            {
                                _instantDamage.Add(Game.Time + 2, (float)attacker.GetSpellDamage(ConnectedAlly, slot));
                            }
                        }
                    }
                }
            }

            if (sender.IsMe)
            {
                if (args.SData.Name == "KalistaExpungeWrapper")
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(250, Orbwalking.ResetAutoAttackTimer);
                }
            }
        }

        private static void KsMode()
        {
            var target =
                HeroManager.Enemies.FirstOrDefault(
                    x =>
                        !x.HasBuffOfType(BuffType.Invulnerability) && !x.HasBuffOfType(BuffType.SpellShield) && !x.HasBuff("Undying Rage") &&
                        spells[Spells.E].CanCast(x) && (x.Health + (x.HPRegenRate / 2)) 
                        <= spells[Spells.E].GetDamage(x));

            if (spells[Spells.E].IsReady() && spells[Spells.E].CanCast(target))
            {
                spells[Spells.E].Cast(true);
            }
        }

        private static void JungleStealMode()
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Player.IsRecalling())
                return;

            var useJsm = ElKalistaMenu._menu.Item("ElKalista.misc.junglesteal").GetValue<bool>();

            if (!useJsm)
                return;

            var jungleCreep = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.E].Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (jungleCreep.Count == 0)
                return;

            var eCreep = jungleCreep.First();
            if (!(spells[Spells.E].GetDamage(eCreep) > eCreep.Health + eCreep.HPRegenRate))
                return;

            if (spells[Spells.E].CanCast(eCreep))
            {
                spells[Spells.E].Cast(eCreep);
            }
        }

        #endregion

        #region itemusage

        private static void Items(Obj_AI_Base target)
        {
            var botrk = ItemData.Blade_of_the_Ruined_King.GetItem();
            var ghost = ItemData.Youmuus_Ghostblade.GetItem();
            var cutlass = ItemData.Bilgewater_Cutlass.GetItem();

            var useYoumuu = ElKalistaMenu._menu.Item("ElKalista.Items.Youmuu").GetValue<bool>();
            var useCutlass = ElKalistaMenu._menu.Item("ElKalista.Items.Cutlass").GetValue<bool>();
            var useBlade = ElKalistaMenu._menu.Item("ElKalista.Items.Blade").GetValue<bool>();

            var useBladeEhp = ElKalistaMenu._menu.Item("ElKalista.Items.Blade.EnemyEHP").GetValue<Slider>().Value;
            var useBladeMhp = ElKalistaMenu._menu.Item("ElKalista.Items.Blade.EnemyMHP").GetValue<Slider>().Value;

            if (botrk.IsReady() && botrk.IsOwned(Player) && botrk.IsInRange(target) &&
                target.HealthPercent <= useBladeEhp && useBlade)
            {
                botrk.Cast(target);
            }

            if (botrk.IsReady() && botrk.IsOwned(Player) && botrk.IsInRange(target) &&
                Player.HealthPercent <= useBladeMhp && useBlade)
            {
                botrk.Cast(target);
            }

            if (cutlass.IsReady() && cutlass.IsOwned(Player) && cutlass.IsInRange(target) &&
                target.HealthPercent <= useBladeEhp && useCutlass)
            {
                cutlass.Cast(target);
            }

            if (ghost.IsReady() && ghost.IsOwned(Player) && target.IsValidTarget(spells[Spells.Q].Range) && useYoumuu)
            {
                ghost.Cast();
            }
        }

        #endregion

        #region Harass

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(spells[Spells.E].Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget() || !Orbwalking.CanMove(1) || Player.ManaPercent < ElKalistaMenu._menu.Item("ElKalista.minmanaharass").GetValue<Slider>().Value)
                return;

            var harassQ = ElKalistaMenu._menu.Item("ElKalista.Harass.Q").GetValue<bool>();
            var harassE = ElKalistaMenu._menu.Item("ElKalista.Harass.E").GetValue<bool>();

            if (harassQ && spells[Spells.Q].IsReady())
            {
                if (spells[Spells.Q].GetPrediction(target).Hitchance >= CustomHitChance && !Player.Spellbook.IsAutoAttacking && !Player.IsDashing())
                {
                    spells[Spells.Q].Cast(target);
                }
            }

            if (!harassE || !spells[Spells.E].IsReady())
            {
                return;
            }
            var etarget = HeroManager.Enemies.Where(x => x.HasRendBuff()).OrderBy(x => x.Distance(Player, true)).FirstOrDefault();
            if (etarget == null || !(target.Distance(Player, true) < Math.Pow(spells[Spells.E].Range + 200, 2)))
            {
                return;
            }
            if (ObjectManager.Get<Obj_AI_Minion>().Any(m => m.IsRendKillable() && spells[Spells.E].IsInRange(m)))
            {
                spells[Spells.E].Cast();
            }
        }

        #endregion

        #region Combo

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget() || !Orbwalking.CanMove(1))
                return;

            Items(target);

            var comboQ = ElKalistaMenu._menu.Item("ElKalista.Combo.Q").GetValue<bool>();
            var comboE = ElKalistaMenu._menu.Item("ElKalista.Combo.E").GetValue<bool>();
            var comboEDisable = ElKalistaMenu._menu.Item("ElKalista.Combo.Disable.E").GetValue<bool>();
            var comboQmana = ElKalistaMenu._menu.Item("ElKalista.Combo.Q.Mana").GetValue<Slider>().Value;

            if (comboQ && spells[Spells.Q].IsReady())
            {
                if (Player.ManaPercent < comboQmana)
                    return;
                var qtarget = TargetSelector.GetTarget(spells[Spells.Q].Range, TargetSelector.DamageType.Physical);

                if (spells[Spells.Q].CanCast(qtarget) && spells[Spells.Q].GetPrediction(qtarget).Hitchance >= CustomHitChance && !Player.Spellbook.IsAutoAttacking && !Player.IsDashing())
                    spells[Spells.Q].Cast(qtarget);
            }


            var useE = ElKalistaMenu._menu.Item("ElKalista.ComboE.Auto").GetValue<bool>();
            if (useE && comboE && spells[Spells.E].IsReady())
            {
                if (spells[Spells.E].IsInRange(target)
                  && !target.HasBuffOfType(BuffType.Invulnerability) 
                  && !target.HasBuffOfType(BuffType.SpellShield)
                  && !target.HasBuff("Undying Rage"))  
                {
                    if (target.IsRendKillable())
                    {
                        spells[Spells.E].Cast(true);
                    }
                }
                else
                {
                    if (comboEDisable)
                    {
                        if (target.IsRendKillable()  && !target.HasBuffOfType(BuffType.Invulnerability)  && !target.HasBuffOfType(BuffType.SpellShield))
                        {
                            spells[Spells.E].Cast(true);
                        }               
                    }
                }
            }
        }

        #endregion

        #region Laneclear

        private static void JungleClear()
        {
            if (!Orbwalking.CanMove(1) || Player.ManaPercent < ElKalistaMenu._menu.Item("minmanaclear").GetValue<Slider>().Value)
                return;

            var useQ = ElKalistaMenu._menu.Item("useQFarmJungle").GetValue<bool>();
            var useE = ElKalistaMenu._menu.Item("useEFarmJungle").GetValue<bool>();

            var minions = MinionManager.GetMinions(
                Player.ServerPosition, spells[Spells.Q].Range, MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (minions.Count < 0)
                return;

            foreach (var minion in minions)
            {
                if (spells[Spells.Q].IsReady() && useQ && !Player.IsDashing())
                {
                    spells[Spells.Q].Cast(minion.ServerPosition);
                }
                var mAA = 0;
                if (spells[Spells.E].IsReady() && useE &&
                    minions[0].Health + minions[0].HPRegenRate / 2 < spells[Spells.E].GetDamage(minion))
                {
                    mAA = 1;
                    if (mAA == 1)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                        mAA = 2;
                        if (mAA == 2){
                            spells[Spells.E].Cast();
                        }
                    }
                }
            }
        }

        private static List<Obj_AI_Base> QGetCollisionMinions(AIHeroClient source, Vector3 targetposition)
        {
            var input = new PredictionInput
            {
                Unit = source,
                Radius = spells[Spells.Q].Width,
                Delay = spells[Spells.Q].Delay,
                Speed = spells[Spells.Q].Speed,
            };

            input.CollisionObjects[0] = CollisionableObjects.Minions;
            return Collision.GetCollision(new List<Vector3> { targetposition }, input).OrderBy(obj => obj.Distance(source, false)).ToList();
        }

        private static void LaneClear()
        {
            var useQ = ElKalistaMenu._menu.Item("useQFarm").GetValue<bool>();
            var useE = ElKalistaMenu._menu.Item("useQFarm").GetValue<bool>();
            var countMinions = ElKalistaMenu._menu.Item("ElKalista.Count.Minions").GetValue<Slider>().Value;
            var countMinionsE = ElKalistaMenu._menu.Item("ElKalista.Count.Minions.E").GetValue<Slider>().Value;

            if (!Orbwalking.CanMove(1) || Player.ManaPercent < ElKalistaMenu._menu.Item("minmanaclear").GetValue<Slider>().Value)
                return;

            var minions = MinionManager.GetMinions(Player.ServerPosition, spells[Spells.E].Range);

            if (minions.Count <= 0)
            {
                return;
            }

            if (spells[Spells.Q].IsReady() && useQ)
            {
                foreach (var minion in minions.Where(x => x.Health <= spells[Spells.Q].GetDamage(x)))
                {
                    var killcount = 0;

                    foreach (
                        var colminion in
                            QGetCollisionMinions(
                                Player, Player.ServerPosition.Extend(minion.ServerPosition, spells[Spells.Q].Range)))
                    {
                        if (colminion.Health <= spells[Spells.Q].GetDamage(colminion))
                        {
                            killcount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (killcount >= countMinions && !Player.Spellbook.IsAutoAttacking && !Player.IsDashing() && !Player.Spellbook.IsAutoAttacking)
                    {
                        spells[Spells.Q].Cast(minion.ServerPosition); 
                        break;
                    }
                }
            }

            if (!useE || !spells[Spells.E].IsReady())
            {
                return;
            }

            var minionkillcount = minions.Count(x => spells[Spells.E].CanCast(x) && x.Health <= spells[Spells.E].GetDamage(x));

            if (minionkillcount >= countMinionsE)
            {
                spells[Spells.E].Cast();
            }
        }

        #endregion

        #region OnNonKillableMinion
        static void Orbwalking_OnNonKillableMinion(AttackableUnit minion)
        {
            var useE = ElKalistaMenu._menu.Item("ElKalista.misc.lasthithelper").GetValue<bool>();

            var minions = minion as Obj_AI_Base;
            if (useE && minions.IsRendKillable() && minions != null)
            {
                spells[Spells.E].Cast();
            }
        }
        #endregion

        #region OnCastSpell
        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && Player.IsDashing() && args.Slot == SpellSlot.Q)
            {
                args.Process = false;
            }         
        }
        #endregion
    }
}
