using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanAnnie
{
    using System;

    using KoreanAnnie.Common;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Annie : ICommonChampion
    {
        #region Constants

        private const string MenuDisplay = "Korean Annie";

        #endregion

        #region Fields

        public Func<bool> CanFarm;

        public Func<bool> CheckStun;

        public Func<string, bool> GetParamBool;

        public Func<string, bool> GetParamKeyBind;

        public Func<string, int> GetParamSlider;

        public Func<string, string> ParamName;

        public Func<bool> SaveStun;

        public Action<string, bool> SetValueBool;

        #endregion

        #region Constructors and Destructors

        public Annie()
        {
            Player = ObjectManager.Player;
            MainMenu = new CommonMenu(MenuDisplay, true);
            Orbwalker = MainMenu.Orbwalker;
            AnnieCustomMenu.Load(MainMenu);

            LoadLambdaExpressions();

            Spells = new CommonSpells(this);
            AnnieSpells.Load(this);

            Buttons = new AnnieButtons(this);
            AnnieOrbwalker = new AnnieCore(this);

            Draws = new AnnieDrawings(this);
            DrawDamage = new CommonDamageDrawing(this);
            DisableAA = new CommonDisableAA(this);
            ForceUltimate = new CommonForceUltimate(this);
            UltimateRange = Spells.R.Range;
            ForceUltimate.ForceUltimate = AnnieOrbwalker.Ultimate;
            DrawDamage.AmountOfDamage = Spells.MaxComboDamage;
            DrawDamage.Active = true;

            Tibbers = new AnnieTibbers(this);

            Obj_AI_Base.OnProcessSpellCast += EAgainstEnemyAA;
            Interrupter2.OnInterruptableTarget += InterruptDangerousSpells;
            AntiGapcloser.OnEnemyGapcloser += StunGapCloser;
            Game.OnUpdate += StackE;
            Obj_AI_Base.OnLevelUp += EvolveUltimate;
        }

        #endregion

        #region Public Properties

        public AnnieCore AnnieOrbwalker { get; set; }
        public AnnieButtons Buttons { get; set; }
        public CommonDisableAA DisableAA { get; set; }
        public CommonDamageDrawing DrawDamage { get; set; }
        public AnnieDrawings Draws { get; set; }
        public CommonForceUltimate ForceUltimate { get; set; }
        public CommonMenu MainMenu { get; set; }
        public Orbwalking.Orbwalker Orbwalker { get; set; }
        public AIHeroClient Player { get; set; }
        public CommonSpells Spells { get; set; }
        public AnnieTibbers Tibbers { get; set; }
        public float UltimateRange { get; set; }

        #endregion

        #region Methods

        private void LoadLambdaExpressions()
        {
            ParamName = s => KoreanUtils.ParamName(MainMenu, s);
            GetParamBool = s => KoreanUtils.GetParamBool(MainMenu, s);
            SetValueBool = (s, b) => KoreanUtils.SetValueBool(MainMenu, s, b);
            GetParamSlider = s => KoreanUtils.GetParamSlider(MainMenu, s);
            GetParamKeyBind = s => KoreanUtils.GetParamKeyBind(MainMenu, s);
            CanFarm =
                () =>
                (!GetParamBool("supportmode"))
                || ((GetParamBool("supportmode")) && (Player.CountAlliesInRange(1500f) == 1));
            CheckStun = () => Player.HasBuff("pyromania_particle");
            SaveStun = () => (CheckStun() && (GetParamBool("savestunforcombo")));
        }

        private void EvolveUltimate(Obj_AI_Base sender, EventArgs args)
        {
            if (sender.IsMe)
            {
                sender.Spellbook.EvolveSpell(SpellSlot.R);
            }
        }

        private void EAgainstEnemyAA(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if ((GetParamBool("useeagainstaa")) && (!sender.IsMe) && (sender.IsEnemy) && (sender is AIHeroClient)
                && (args.Target != null) && (args.Target.IsMe) && (Player.Distance(args.End) < 440)
                && (args.SData.Name.ToLowerInvariant().Contains("attack")))
            {
                Spells.E.Cast();
            }
        }

        private void InterruptDangerousSpells(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if ((GetParamBool("interruptspells")) && (CheckStun()) && (sender.IsEnemy)
                && (args.DangerLevel > Interrupter2.DangerLevel.Medium))
            {
                if ((Spells.Q.IsReady()) && (sender.IsValidTarget(Spells.Q.Range)))
                {
                    Spells.Q.Cast(sender);
                }
                if ((Spells.W.IsReady()) && (sender.IsValidTarget(Spells.W.Range)))
                {
                    Spells.W.Cast(sender);
                }
            }
        }

        private void StackE(EventArgs args)
        {
            if (!Player.IsRecalling() && !CheckStun() && GetParamBool("useetostack")
                && Player.ManaPercent > GetParamSlider("manalimitforstacking") && Spells.E.IsReady())
            {
                LeagueSharp.Common.Utility.DelayAction.Add(100, () => Spells.E.Cast());
            }
        }

        private void StunGapCloser(ActiveGapcloser gapcloser)
        {
            if (GetParamBool("antigapcloser") && (CheckStun()))
            {
                if ((Spells.Q.IsReady()) && (gapcloser.Sender.IsValidTarget(Spells.Q.Range)))
                {
                    Spells.Q.Cast(gapcloser.Sender);
                }
                else if ((Spells.W.IsReady()) && (gapcloser.Sender.IsValidTarget(Spells.W.Range)))
                {
                    Spells.W.Cast(gapcloser.Sender);
                }
            }
        }

        #endregion
    }
}