using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using TheEkko.Commons;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheEkko.ComboSystem
{
    class ComboProvider
    {
        protected List<Skill> Skills;
        protected Skill TotalControl { get; private set; }
        private bool _totalControl;
        private bool _cancelUpdate;
        private Orbwalking.Orbwalker _orbwalker;
        private IMainContext _context;
        public AIHeroClient Target { get; private set; }
        public float TargetRange;
        public TargetSelector.DamageType DamageType;
        public bool AntiGapcloser = true;
        public Dictionary<string, bool> GapcloserCancel = new Dictionary<string, bool>();
        public readonly Dictionary<string, List<InterruptableSpell>> InterruptableSpells = new Dictionary<string, List<InterruptableSpell>>();

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
        public ComboProvider(List<Skill> skills, float range)
        {
            Skills = skills;
            DamageType = skills.Count(spell => spell.Spell.DamageType == TargetSelector.DamageType.Magical) > skills.Count(spell => spell.Spell.DamageType == TargetSelector.DamageType.Physical) ?
                TargetSelector.DamageType.Magical : TargetSelector.DamageType.Physical;
            TargetRange = range;//skills.Max(spell => spell.Spell.Range);

            LeagueSharp.Common.AntiGapcloser.Spells.ForEach(spell =>
            {
                var champ = HeroManager.Enemies.FirstOrDefault(enemy => enemy.ChampionName.Equals(spell.ChampionName, StringComparison.InvariantCultureIgnoreCase));
                if (champ != null && !GapcloserCancel.ContainsKey(champ.ChampionName))
                {
                    GapcloserCancel.Add(champ.ChampionName, true);

                }
            });
            LeagueSharp.Common.AntiGapcloser.OnEnemyGapcloser += OnGapcloser;
            InitInterruptable();
            Interrupter2.OnInterruptableTarget += OnInterrupter;

        }

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
            var interruptableSpell = InterruptableSpells[sender.ChampionName].FirstOrDefault(interruptable => interruptable.Slot == sender.Spellbook.ActiveSpellSlot || (sender.Spellbook.ActiveSpellSlot == SpellSlot.Unknown && interruptable.DangerLevel.ToString() == args.DangerLevel.ToString()));
            if (interruptableSpell == null || !interruptableSpell.FireEvent)
                return;

            foreach (var skill in Skills)
                skill.Interruptable(_context, this, sender, interruptableSpell);
        }

        public void AddGapclosersToMenu(Menu menu)
        {
            GapcloserCancel.Keys.ToList().ForEach(item => menu.AddMItem(item, true, (sender, args) => GapcloserCancel[item] = args.GetNewValue<bool>()));
        }

        public void AddInterruptablesToMenu(Menu menu)
        {
            InterruptableSpells.ToList().ForEach(pair => pair.Value.ForEach(champSpell => menu.AddMItem(pair.Key + ": " + champSpell.Slot.ToString(), champSpell.FireEvent, (sender, args) => champSpell.FireEvent = args.GetNewValue<bool>())));
        }

        private void OnGapcloser(ActiveGapcloser gapcloser)
        {
            //            Chat.Print("try " + gapcloser.Sender.ChampionName + " have: " + GapcloserCancel.FirstOrDefault().Key);

            if ((!AntiGapcloser) || !GapcloserCancel[gapcloser.Sender.ChampionName]) return;
            foreach (var skill in Skills)
                skill.Gapcloser(_context, this, gapcloser);
        }

        /// <summary>
        /// call to init all stuffs. Menu has to exist at that time
        /// </summary>
        /// <param name="context"></param>
        public virtual void Initialize(IMainContext context)
        {
            Skills.ForEach(skill => skill.Initialize(context, this));
            _context = context;
            _orbwalker = context.GetOrbwalker();
        }

        public float GetComboDamage(AIHeroClient enemy)
        {
            return Skills.Sum(skill => skill.ComboEnabled ? skill.GetDamage(enemy) : 0);
        }

        public AIHeroClient GetTarget()
        {
            return Target;
        }

        /// <summary>
        /// override in sub class to add champion combo logic. for example Garen has a fixed combo, but wants to do W not in order, but when he gets damage.
        /// In this example you would override Update and have a seperate logic for W instead of adding it to the skill routines.
        /// </summary>
        /// <param name="context"></param>
        public virtual void Update(IMainContext context)
        {
            Target = TargetSelector.GetTarget(TargetRange, DamageType);
            IgniteManager.Update(context, Target);
            Skills.Sort(); //Todo: check if expensive. Will do that event-based otherwise

            if (_totalControl)
            {
                TotalControl.Update(_orbwalker.ActiveMode, context, this, Target);

                if (!TotalControl.NeedsControl())
                {
                    TotalControl.TryTerminate(_context);
                    _totalControl = false;
                    Update(context);
                    return;
                }

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var skill in Skills.Where(item => item.GetPriority() > TotalControl.GetPriority()))
                {
                    skill.Update(_orbwalker.ActiveMode, context, this, Target);
                }
            }
            else
            {
                foreach (var item in Skills)
                {
                    if (_cancelUpdate)
                    {
                        _cancelUpdate = false;
                        return;
                    }
                    item.Update(_orbwalker.ActiveMode, context, this, Target);
                }
            }

        }

        public bool GrabControl(Skill skill)
        {
            if (_totalControl && TotalControl == skill)
                return true;
            if (_totalControl && TotalControl.GetPriority() < skill.GetPriority())
            {
                TotalControl.TryTerminate(_context);
                TotalControl = skill;
                _cancelUpdate = true;
                return true;
            }
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var currentSkill in Skills)
            {
                if (skill != currentSkill && currentSkill.NeedsControl() && currentSkill.GetEnabled(_context.GetOrbwalker().ActiveMode) && currentSkill.GetPriority() >= skill.GetPriority())
                {
                    return false;
                }
            }
            _totalControl = true;
            TotalControl = skill;
            _cancelUpdate = true;
            return true;
        }

        public void SetEnabled<T>(Orbwalking.OrbwalkingMode mode, bool enabled) where T : Skill
        {
            foreach (var skill in Skills.Where(skill => skill.GetType() == typeof(T)))
            {
                skill.SetEnabled(mode, enabled);
            }
        }

        public T GetSkill<T>() where T : Skill
        {
            return (T)Skills.FirstOrDefault(skill => skill is T);
        }

        public Skill[] GetSkills()
        {
            return Skills.ToArray();
        }
    }
}
