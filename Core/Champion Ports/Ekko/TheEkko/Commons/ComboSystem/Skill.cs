using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheEkko.ComboSystem
{
    abstract class Skill : IComparable<Skill>
    {
        public bool ComboEnabled = true, LaneclearEnabled = false, HarassEnabled = false; // If this skill should be used in Combo, ...
        private float _castTime;
        private string _castName;
        private Spell _castSpell;
        protected const float SafeCastMaxTime = 0.5f;
        public Spell Spell { get; private set; }
        protected ComboProvider Provider { get; private set; }
        protected IMainContext Context { get; private set; }
        protected bool UseManaManager = true;
        public bool SwitchClearToHarassOnTarget = true;

        protected Skill(Spell spell)
        {
            Spell = spell;
            UseManaManager = spell.Instance.SData.ManaCostArray.MaxOrDefault((value) => value) > 0;
            //Console.WriteLine(spell.Instance.SData.ManaCostArray.MaxOrDefault((value) => value) + " MAX VALUE");
        }

        /// <summary>
        /// The safe cast mechanism is used to set a skill "on cooldown" even though the server hasn't even sent the cooldown update.
        /// Results in less ability spam, (faster execution ?) and it's easier to debug
        /// </summary>
        protected bool SafeCast(Action spellCastAction, string name = "")
        {
            if (string.IsNullOrEmpty(name))
                name = Spell.Instance.Name;
            if (HasBeenSafeCast(name))
                return false;
            _castTime = Game.Time;
            _castName = name;
            _castSpell = Spell;
            spellCastAction();
            return true;
        }

        /// <summary>
        /// The safe cast mechanism is used to set a skill "on cooldown" even though the server hasn't even sent the cooldown update.
        /// Results in less ability spam, (faster execution ?) and it's easier to debug
        /// </summary>
        protected bool SafeCast(Spell spell, Action spellCastAction, string name = "")
        {
            if (string.IsNullOrEmpty(name))
                name = spell.Instance.Name;
            if (HasBeenSafeCast(name))
                return false;
            _castTime = Game.Time;
            _castName = name;
            _castSpell = spell;
            spellCastAction();
            return true;
        }

        /// <summary>
        /// Add Initialisation logic in sub class. Called by ComboProvider.SetActive(skill)
        /// </summary>
        /// <param name="combo"></param>
        public virtual void Initialize(IMainContext context, ComboProvider combo)
        {
            Provider = combo;
            Context = context;
        }

        public virtual void SetEnabled(Orbwalking.OrbwalkingMode mode, bool enabled)
        {
            //Console.WriteLine(GetType().Name+": "+mode+" enabled: "+enabled);
            switch (mode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    ComboEnabled = enabled;
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneclearEnabled = enabled;
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    HarassEnabled = enabled;
                    break;
            }
        }

        public bool GetEnabled(Orbwalking.OrbwalkingMode mode)
        {
            switch (mode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    return ComboEnabled;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    return LaneclearEnabled;
                case Orbwalking.OrbwalkingMode.Mixed:
                    return HarassEnabled;
            }
            return false;
        }


        public virtual void Update(Orbwalking.OrbwalkingMode mode, IMainContext context, ComboProvider combo, AIHeroClient target)
        {
            if (mode == Orbwalking.OrbwalkingMode.None) return;
            if (mode == Orbwalking.OrbwalkingMode.LaneClear && SwitchClearToHarassOnTarget && target != null && HarassEnabled)
                mode = Orbwalking.OrbwalkingMode.Mixed;
            if (UseManaManager && !ManaManager.CanUseMana(mode)) return;
            switch (mode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (ComboEnabled)
                        Combo(context, combo, target);
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    if (LaneclearEnabled)
                        LaneClear(context, combo, target);
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    if (HarassEnabled)
                        Harass(context, combo, target);
                    break;
            }
        }

        public abstract void Cast(AIHeroClient target, bool force = false, HitChance minChance = HitChance.Low);
        public virtual void Combo(IMainContext context, ComboProvider combo, AIHeroClient target)
        {
            Cast(target);
        }
        public virtual void LaneClear(IMainContext context, ComboProvider combo, AIHeroClient target) { }
        public virtual void Harass(IMainContext context, ComboProvider combo, AIHeroClient target)
        {
            Cast(target);
        }
        public virtual void Gapcloser(IMainContext context, ComboProvider combo, ActiveGapcloser gapcloser) { }
        public virtual void Interruptable(IMainContext context, ComboProvider combo, AIHeroClient sender, ComboProvider.InterruptableSpell interruptableSpell) { }

        public virtual float GetDamage(AIHeroClient enemy)
        {
            return Spell.Instance.State == SpellState.Ready ? (float)ObjectManager.Player.GetSpellDamage(enemy, Spell.Slot) : 0f;
        }

        /// <summary>
        /// If this returns true an other skill of the same or lower priority can't grab control. If this skill
        /// has control but this return false, the control will be terminated.
        /// </summary>
        /// <returns></returns>
        public virtual bool NeedsControl() { return false; }

        public abstract int GetPriority();

        /// <summary>
        /// Gets called if some other skill wants total control OR this skill doesn't need control even though it has it.
        /// </summary>
        /// <returns></returns>
        public virtual bool TryTerminate(IMainContext context) { return true; }

        /// <summary>
        /// If the spell seems available. SafeCast compatible
        /// </summary>
        /// <returns></returns>
        public virtual bool CanBeCast()
        {
            return Spell.Instance.State == SpellState.Ready && !HasBeenSafeCast();
        }

        /// <summary>
        /// If the spell as been cast. SafeCast compatible
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasBeenSafeCast(string name)
        {
            //Todo: I think this is nidalee/elise/jayce incompatible
            return (_castName == name && Game.Time - _castTime < SafeCastMaxTime) || (Spell.Instance.State != SpellState.Ready) || (_castSpell != null && Spell.Instance.Name != _castName); //bug: != ready was  == cooldown, had to change cuz bug ... same with method below, and removed _castSpell != null && check before that
        }

        /// <summary>
        /// If the spell as been cast. SafeCast compatible
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasBeenSafeCast()
        {
            //Todo: I think this is nidalee/elise/jayce incompatible
            return (_castName == Spell.Instance.Name && Game.Time - _castTime < SafeCastMaxTime) || (Spell.Instance.State != SpellState.Ready) || (_castSpell != null && Spell.Instance.Name != _castName);
        }

        /// <summary>
        /// If the spell as been cast. SafeCast compatible
        /// </summary>
        /// <param name="name"></param>
        /// <param name="castTime"></param>
        /// <returns></returns>
        public bool IsInSafeCast(string name, float castTime = SafeCastMaxTime)
        {
            return (!string.IsNullOrEmpty(_castName) && _castName != name) || Game.Time - _castTime < castTime;
        }

        /// <summary>
        /// If the spell as been cast. SafeCast compatible
        /// </summary>
        /// <param name="name"></param>
        /// <param name="castTime"></param>
        /// <returns></returns>
        public bool IsInSafeCast(float castTime = SafeCastMaxTime)
        {
            return Game.Time - _castTime < castTime;
        }

        public int CompareTo(Skill obj)
        {
            return obj.GetPriority() - GetPriority();
        }
    }
}
