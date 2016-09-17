using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheTwitch.Commons.ComboSystem
{
    public class ComboProvider
    {
        protected List<Skill> Skills;
        public AIHeroClient Target;
        public readonly Orbwalking.Orbwalker Orbwalker;
        public float TargetRange;
        public TargetSelector.DamageType DamageType;
        public bool AntiGapcloser = true;
        public bool Interrupter = true;
        public Dictionary<string, bool> GapcloserCancel = new Dictionary<string, bool>();
        public readonly Dictionary<string, List<InterruptableSpell>> InterruptableSpells = new Dictionary<string, List<InterruptableSpell>>();
        private readonly List<Tuple<Skill, Action>> _queuedCasts = new List<Tuple<Skill, Action>>(); //Todo: check if properly working
        private bool _cancelSpellUpdates;
        private readonly Dictionary<int, float> _marks = new Dictionary<int, float>();
        private bool _autoLevelSpells;
        private string _autoLevelSpellsSkillOrder;
        private string _autoLevelSpellsMaxOrder;

        // ReSharper disable InconsistentNaming
        public enum SpellOrder { RQWE, RQEW, RQEEW, RWQE, RWEQ, REQW, REWQ }
        // ReSharper restore InconsistentNaming

        private bool _drawingsEnabled;
        private Circle _targetDrawing;
        private Circle _deadDrawing;
        private bool _autoLevelNotOne;

        public class InterruptableSpell
        {
            public SpellSlot Slot;
            public InterruptableDangerLevel DangerLevel;
            public bool MovementInterrupts;
            public bool FireEvent = true;

            public InterruptableSpell(SpellSlot slot, InterruptableDangerLevel danger, bool movementInterrupts)
            {
                Slot = slot;
                DangerLevel = danger;
                MovementInterrupts = movementInterrupts;
            }
        }

        /// <summary>
        /// Represents a "combo" and it's logic. Manages skill logic.
        /// </summary>
        public ComboProvider(float targetSelectorRange, IEnumerable<Skill> skills, Orbwalking.Orbwalker orbwalker)
        {
            Skills = skills as List<Skill> ?? skills.ToList();
            DamageType = Skills.Count(spell => spell.DamageType == TargetSelector.DamageType.Magical) > Skills.Count(spell => spell.DamageType == TargetSelector.DamageType.Physical) ?
                TargetSelector.DamageType.Magical : TargetSelector.DamageType.Physical;
            TargetRange = targetSelectorRange;
            Orbwalker = orbwalker;

            LeagueSharp.Common.AntiGapcloser.Spells.ForEach(spell =>
            {
                var champ = HeroManager.Enemies.FirstOrDefault(enemy => enemy.ChampionName.Equals(spell.ChampionName, StringComparison.InvariantCultureIgnoreCase));
                if (champ != null && !GapcloserCancel.ContainsKey(champ.ChampionName))
                    GapcloserCancel.Add(champ.ChampionName, true);
            });
            LeagueSharp.Common.AntiGapcloser.OnEnemyGapcloser += OnGapcloser;
            InitInterruptable();
            Interrupter2.OnInterruptableTarget += OnInterrupter;
            Drawing.OnDraw += _ =>
            {
                foreach (var skill in Skills)
                {
                    skill.Draw();
                }

                if (!_drawingsEnabled) return;

                if (_targetDrawing.Active && Target.IsValidTarget())
                    Render.Circle.DrawCircle(Target.Position, 100, _targetDrawing.Color);

                if (_deadDrawing.Active)
                    foreach (var enemy in HeroManager.Enemies)
                        if (enemy.IsValidTarget(TargetRange) && ShouldBeDead(enemy))
                            Render.Circle.DrawCircle(enemy.Position, 200, _deadDrawing.Color);

                //foreach (var enemy in ObjectManager.Get<Obj_AI_Base>())
                //    if (enemy.IsValidTarget() && ShouldBeDead(enemy))
                //        Render.Circle.DrawCircle(enemy.Position, 200, _deadDrawing.Color);

            };

            Spellbook.OnCastSpell += (sender, args) =>
            {
                if (!sender.Owner.IsMe) return;

                for (int i = 0; i < _queuedCasts.Count; i++)
                {
                    if (_queuedCasts[i].Item1.Slot == args.Slot)
                    {
                        _queuedCasts.RemoveAt(i);
                        break;
                    }
                }
            };
        }

        /// <summary>
        /// Represents a "combo" and it's logic. Manages skill logic.
        /// </summary>
        public ComboProvider(float targetSelectorRange, Orbwalking.Orbwalker orbwalker, params Skill[] skills)
            : this(targetSelectorRange, skills.ToList(), orbwalker) { }

        #region Menu creators
        public void CreateBasicMenu(Menu comboMenu, Menu harassMenu, Menu laneclearMenu, Menu antiGapcloserMenu, Menu interrupterMenu, Menu manamanagerMenu, Menu summonerMenu, Menu itemMenu, Menu drawingMenu, bool laneclearHarassSwitch = true /*bool healmanager = true,*/)
        {
            if (comboMenu != null)
            {
                CreateComboMenu(comboMenu);
            }

            if (harassMenu != null)
            {
                CreateHarassMenu(harassMenu);
            }

            if (laneclearMenu != null)
            {
                CreateLaneclearMenu(laneclearMenu, laneclearHarassSwitch);
            }

            if (antiGapcloserMenu != null)
            {
                var gapcloserSpells = new Menu("Enemies", "Gapcloser.Enemies");
                AddGapclosersToMenu(gapcloserSpells);
                antiGapcloserMenu.AddSubMenu(gapcloserSpells);
                antiGapcloserMenu.AddMItem("Enabled", true, (sender, args) => AntiGapcloser = args.GetNewValue<bool>());
            }

            if (interrupterMenu != null)
            {
                var spellMenu = new Menu("Spells", "Interrupter.Spells");
                AddInterruptablesToMenu(spellMenu);
                interrupterMenu.AddSubMenu(spellMenu);
                interrupterMenu.AddMItem("Enabled", true, (sender, args) => Interrupter = args.GetNewValue<bool>());
            }

            if (manamanagerMenu != null)
            {
                ManaManager.Initialize(manamanagerMenu);
            }

            if (summonerMenu != null)
            {
                new SummonerManager().Attach(summonerMenu, this);
            }

            if (itemMenu != null)
            {
                new ItemManager().Attach(itemMenu, this);
            }

            if (drawingMenu != null)
                CreateDrawingMenu(drawingMenu);
        }

        public void CreateComboMenu(Menu comboMenu, params SpellSlot[] forbiddenSlots)
        {
            foreach (var skill in Skills)
            {
                Skill currentSkill = skill;
                if (forbiddenSlots.Contains(currentSkill.Slot)) continue;
                comboMenu.AddMItem("Use " + skill.Slot, skill.ComboEnabled, (sender, args) => SetEnabled(currentSkill, Orbwalking.OrbwalkingMode.Combo, args.GetNewValue<bool>()));
                if (skill.IsSkillshot)
                    comboMenu.AddMItem(skill.Slot + " Hitchance", new StringList(new[] { "Low", "Medium", "High", "VeryHigh" }), (sender, args) => currentSkill.SetMinComboHitchance(args.GetNewValue<StringList>().SelectedValue));
            }
            comboMenu.ProcStoredValueChanged<bool>();
            comboMenu.ProcStoredValueChanged<StringList>();
        }

        public void CreateHarassMenu(Menu harassMenu, params SpellSlot[] forbiddenSlots)
        {
            foreach (var skill in Skills)
            {
                Skill currentSkill = skill;
                if (forbiddenSlots.Contains(currentSkill.Slot) || currentSkill.Slot == SpellSlot.R) continue;
                harassMenu.AddMItem("Use " + skill.Slot, skill.HarassEnabled, (sender, args) => SetEnabled(currentSkill, Orbwalking.OrbwalkingMode.Mixed, args.GetNewValue<bool>()));
                if (skill.IsSkillshot)
                    harassMenu.AddMItem(skill.Slot + " Hitchance", new StringList(new[] { "Low", "Medium", "High", "VeryHigh" }) { SelectedIndex = 3 }, (sender, args) => currentSkill.SetMinHarassHitchance(args.GetNewValue<StringList>().SelectedValue));
            }
            harassMenu.ProcStoredValueChanged<bool>();
            harassMenu.ProcStoredValueChanged<StringList>();
        }

        public void CreateLaneclearMenu(Menu laneclearMenu, bool harassSwitch = true, params SpellSlot[] forbiddenSlots)
        {
            foreach (var skill in Skills)
            {
                Skill currentSkill = skill;
                if (forbiddenSlots.Contains(currentSkill.Slot) || currentSkill.Slot == SpellSlot.R) continue;
                laneclearMenu.AddMItem("Use " + skill.Slot, skill.LaneclearEnabled, (sender, args) => SetEnabled(currentSkill, Orbwalking.OrbwalkingMode.LaneClear, args.GetNewValue<bool>()));
            }
            if (harassSwitch) laneclearMenu.AddMItem("Use mixed mode instead if enemy near", false, (sender, args) => GetSkills().ToList().ForEach(skill => skill.SwitchClearToHarassOnTarget = args.GetNewValue<bool>()));

            laneclearMenu.ProcStoredValueChanged<bool>();
        }

        public void CreateDrawingMenu(Menu drawingMenu)
        {
            _drawingsEnabled = true;
            drawingMenu.AddMItem("Target", new Circle(true, Color.FromArgb(150, Color.OrangeRed)), (sender, args) => _targetDrawing = args.GetNewValue<Circle>()).ProcStoredValueChanged<Circle>();
            drawingMenu.AddMItem("Draw 100% dead enemies", new Circle(true, Color.FromArgb(150, Color.LightGreen)), (sender, args) => _deadDrawing = args.GetNewValue<Circle>()).ProcStoredValueChanged<Circle>();
            drawingMenu.AddMItem("Damage indicator", new Circle(true, Color.FromArgb(150, Color.Goldenrod)), (sender, args) =>
            {
                DamageIndicator.Enabled = args.GetNewValue<Circle>().Active;
                DamageIndicator.Fill = true;
                DamageIndicator.FillColor = Color.FromArgb(100, args.GetNewValue<Circle>().Color);
                DamageIndicator.Color = Color.FromArgb(200, DamageIndicator.FillColor);
                DamageIndicator.DamageToUnit = GetComboDamage;
            }).ProcStoredValueChanged<Circle>();
        }

        public void CreateAutoLevelMenu(Menu autoLevelMenu, SpellOrder skillOrder, SpellOrder maxOrder)
        {
            autoLevelMenu.AddMItem("Enabled", true, (sender, args) => _autoLevelSpells = args.GetNewValue<bool>());
            var possibleItems = Enum.GetValues(typeof(SpellOrder)).Cast<SpellOrder>().Select(item => String.Join<char>("-", item.ToString())).ToArray();

            autoLevelMenu.AddMItem("Don't level at level 1", false, (sender, args) => _autoLevelNotOne = args.GetNewValue<bool>());
            autoLevelMenu.AddMItem("Skill (start) order", new StringList(possibleItems, Array.FindIndex(possibleItems, item => item == (String.Join<char>("-", skillOrder.ToString())))), (sender, args) => _autoLevelSpellsSkillOrder = args.GetNewValue<StringList>().SelectedValue);
            autoLevelMenu.AddMItem("Skill max order", new StringList(possibleItems, Array.FindIndex(possibleItems, item => item == (String.Join<char>("-", maxOrder.ToString())))), (sender, args) => _autoLevelSpellsMaxOrder = args.GetNewValue<StringList>().SelectedValue);
            autoLevelMenu.ProcStoredValueChanged<bool>();
            autoLevelMenu.ProcStoredValueChanged<StringList>();
            Obj_AI_Base.OnLevelUp += OnLevelUp;
            OnLevelUp(ObjectManager.Player, null);
        }


        private void OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!sender.IsMe || _autoLevelNotOne && ObjectManager.Player.Level == 1 || !_autoLevelSpells) return;

            var skillOrder = _autoLevelSpellsSkillOrder.Split('-').Select(item => item.ToEnum<SpellSlot>());
            var maxOrder = _autoLevelSpellsMaxOrder.Split('-').Select(item => item.ToEnum<SpellSlot>());

            foreach (var spellSlot in skillOrder)
                if (ObjectManager.Player.Spellbook.GetSpell(spellSlot).Level < skillOrder.Count(slot => slot == spellSlot))
                    ObjectManager.Player.Spellbook.LevelSpell(spellSlot);

            foreach (var spellSlot in maxOrder)
                ObjectManager.Player.Spellbook.LevelSpell(spellSlot);
        }



        public void AddGapclosersToMenu(Menu menu)
        {
            GapcloserCancel.Keys.ToList().ForEach(item => menu.AddMItem(item, true, (sender, args) => GapcloserCancel[item] = args.GetNewValue<bool>()));
        }

        public void AddInterruptablesToMenu(Menu menu)
        {
            InterruptableSpells.ToList().ForEach(pair => pair.Value.ForEach(champSpell => menu.AddMItem(pair.Key + ": " + champSpell.Slot.ToString(), champSpell.FireEvent, (sender, args) => champSpell.FireEvent = args.GetNewValue<bool>())));
        }
        #endregion

        #region Interruptables
        private void InitInterruptable()
        {
            //Interrupter2 contains the spells, but they are private. Can't use reflection cause of sandbox. GG WP
            RegisterInterruptableSpell("Caitlyn", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.High, true));
            RegisterInterruptableSpell("FiddleSticks", new InterruptableSpell(SpellSlot.W, InterruptableDangerLevel.Medium, true));
            RegisterInterruptableSpell("FiddleSticks", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.High, true));
            RegisterInterruptableSpell("Galio", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.High, true));
            RegisterInterruptableSpell("Janna", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.Low, true));
            RegisterInterruptableSpell("Karthus", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.High, true));
            RegisterInterruptableSpell("Katarina", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.High, true));
            RegisterInterruptableSpell("Lucian", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.High, false));
            RegisterInterruptableSpell("Malzahar", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.High, true));
            RegisterInterruptableSpell("MasterYi", new InterruptableSpell(SpellSlot.W, InterruptableDangerLevel.Low, true));
            RegisterInterruptableSpell("MissFortune", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.High, true));
            RegisterInterruptableSpell("Nunu", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.High, true));
            RegisterInterruptableSpell("Pantheon", new InterruptableSpell(SpellSlot.E, InterruptableDangerLevel.Low, true));
            RegisterInterruptableSpell("Pantheon", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.High, true));
            RegisterInterruptableSpell("RekSai", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.High, true));
            RegisterInterruptableSpell("Sion", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.Low, true));
            RegisterInterruptableSpell("Shen", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.Low, true));
            RegisterInterruptableSpell("TwistedFate", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.Medium, true));
            RegisterInterruptableSpell("Urgot", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.High, true));
            RegisterInterruptableSpell("Velkoz", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.High, true));
            RegisterInterruptableSpell("Warwick", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.High, true));
            RegisterInterruptableSpell("Xerath", new InterruptableSpell(SpellSlot.R, InterruptableDangerLevel.High, true));
            RegisterInterruptableSpell("Varus", new InterruptableSpell(SpellSlot.Q, InterruptableDangerLevel.Low, false));
        }

        private void RegisterInterruptableSpell(string name, InterruptableSpell spell)
        {
            if (!InterruptableSpells.ContainsKey(name))
                InterruptableSpells.Add(name, new List<InterruptableSpell>());
            if (HeroManager.Enemies.Any(enemy => enemy.ChampionName == name))
                InterruptableSpells[name].Add(spell);
        }

        private void OnInterrupter(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Interrupter || sender.IsAlly) return;
            var interruptableSpell = InterruptableSpells[sender.ChampionName].FirstOrDefault(interruptable => interruptable.Slot == sender.Spellbook.ActiveSpellSlot || (sender.Spellbook.ActiveSpellSlot == SpellSlot.Unknown && interruptable.DangerLevel.ToString() == args.DangerLevel.ToString()));
            if (interruptableSpell == null || !interruptableSpell.FireEvent)
                return;

            foreach (var skill in Skills)
                skill.Interruptable(this, sender, interruptableSpell, args.EndTime);
        }

        private void OnGapcloser(ActiveGapcloser gapcloser)
        {
            //            Chat.Print("try " + gapcloser.Sender.ChampionName + " have: " + GapcloserCancel.FirstOrDefault().Key);

            if (!AntiGapcloser || !GapcloserCancel[gapcloser.Sender.ChampionName] || gapcloser.Sender.IsAlly) return;
            foreach (var skill in Skills)
                skill.Gapcloser(this, gapcloser);
        }
        #endregion

        #region Core routines
        /// <summary>
        /// Call to initialize all stuffs. If skills access the menu, this should be called after the menu creation
        /// </summary>
        public virtual void Initialize()
        {
            Skills.ForEach(skill => skill.Initialize(this));
        }

        /// <summary>
        /// Call to initialize all stuffs. If skills access the menu, this should be called after the menu creation
        /// </summary>
        public virtual void Initialize(TargetSelector.DamageType damageType)
        {
            DamageType = damageType;
            Initialize();
        }

        protected virtual AIHeroClient SelectTarget()
        {
            return TargetSelector.GetTarget(TargetRange, DamageType);
        }

        public void Update()
        {
            OnUpdate(Orbwalker.ActiveMode);
        }

        protected virtual void OnUpdate(Orbwalking.OrbwalkingMode mode)
        {
            //Console.WriteLine(mode);
            try
            {
                Target = SelectTarget();
            }
            catch
            {
                if (Game.Time % 1f < 0.05f)
                    Console.WriteLine("[TheNinow.ComboSystem] Error during custom target selection");
                Target = TargetSelector.GetTarget(TargetRange, DamageType);
            }


            for (int i = 0; i < _queuedCasts.Count; i++)
            {
                if (_queuedCasts[i].Item1.HasBeenCast())
                    _queuedCasts.RemoveAt(i);
                else
                {
                    try
                    {
                        _queuedCasts[i].Item2();
                    }
                    catch
                    {
                        _queuedCasts.RemoveAt(i);
                    }
                    break;
                }
            }


            if (!ObjectManager.Player.Spellbook.IsCastingSpell)
            {
                Skills.Sort(); //Checked: this is not expensive
                foreach (var item in Skills)
                {
                    item.Update(mode, this, Target);
                    if (_cancelSpellUpdates)
                    {
                        _cancelSpellUpdates = false;
                        break;
                    }
                }
            }

        }
        #endregion

        #region API
        public void CancelSpellUpdates()
        {
            _cancelSpellUpdates = true;
        }

        /// <summary>
        /// Note: Do not use autoattacks as additionalSpellDamage!
        /// </summary>
        /// <param name="target"></param>
        /// <param name="additionalSpellDamage"></param>
        /// <returns></returns>
        public virtual bool ShouldBeDead(Obj_AI_Base target, float additionalSpellDamage = 0f)
        {
            var healthPred = HealthPrediction.GetHealthPrediction(target, 1000);
            return healthPred - (target.GetRemainingIgniteDamage() + additionalSpellDamage) <= 0;
        }

        /// <summary>
        /// Estimates the damage the combo could do in it's current state
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        public virtual float GetComboDamage(AIHeroClient enemy)
        {
            return Skills.Sum(skill => skill.ComboEnabled ? skill.GetDamage(enemy) : 0);
        }

        public void RemoveTopQueuedCast()
        {
            if (_queuedCasts.Count > 0) _queuedCasts.RemoveAt(_queuedCasts.Count - 1);
        }

        /// <summary>
        /// Adds a cast to the cast-queue. The added castActions will be cast in the same order as they were added
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="castAction"></param>
        public void AddQueuedCast(Skill skill, Action castAction)
        {
            if (_queuedCasts.Any(t => t.Item1 == skill)) return;
            _queuedCasts.Add(new Tuple<Skill, Action>(skill, castAction));
        }

        public void SetEnabled(Skill skill, Orbwalking.OrbwalkingMode mode, bool enabled)
        {
            skill.SetEnabled(mode, enabled);
        }

        public void SetEnabled<T>(Orbwalking.OrbwalkingMode mode, bool enabled) where T : Skill
        {
            foreach (var skill in Skills.Where(skill => skill.GetType() == typeof(T)))
            {
                skill.SetEnabled(mode, enabled);
            }
        }

        /// <summary>
        /// Returns the first skill of type Ts
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSkill<T>() where T : Skill
        {
            return (T)Skills.FirstOrDefault(skill => skill is T);
        }

        /// <summary>
        /// returns all skills
        /// </summary>
        /// <returns></returns>
        public Skill[] GetSkills()
        {
            return Skills.ToArray();
        }

        public void SetMarked(GameObject obj, float time = 1f)
        {
            _marks[obj.NetworkId] = Game.Time + time;
        }

        public bool IsMarked(GameObject obj)
        {
            return _marks.ContainsKey(obj.NetworkId) && _marks[obj.NetworkId] > Game.Time;
        }
        #endregion
    }
}
