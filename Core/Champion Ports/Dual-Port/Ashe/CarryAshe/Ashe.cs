using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CarryAshe
{
    internal enum Spells
    {
        Q,
        W,
        E,
        R
    }

    internal class Ashe
    {

        #region Attributes

        /// <summary>
        /// Menu handler
        /// </summary>
        private AsheMenu _menu;

        /// <summary>
        /// Drawing handler
        /// </summary>
        private AsheDrawings _drawings;

        /// <summary>
        /// Ignite Slot
        /// </summary>
        private SpellSlot _ignite;

        /// <summary>
        /// Spell Dictionary
        /// </summary>
        private Dictionary<Spells, Spell> _spells = new Dictionary<Spells, Spell>()
        {
            { Spells.Q, new Spell(SpellSlot.Q) },
            { Spells.W, new Spell(SpellSlot.W, 1200) },
            { Spells.E, new Spell(SpellSlot.E) },
            { Spells.R, new Spell(SpellSlot.R) }
        };
        #endregion

        #region Properties

        /// <summary>
        /// Getter for L#Menu
        /// </summary>
        public Menu Menu { get { return _menu.Menu; } }

        /// <summary>
        /// Getter for Program.Orbwalker
        /// </summary>


        public int QStacks { get { return Player.GetBuffCount("AsheQ") + Player.GetBuffCount("AsheQCastReady"); } }

        public bool IsQMaxStacked { get { return Player.HasBuff("AsheQCastReady"); } }
        #endregion

        #region Helpers

        /// <summary>
        /// Getter for current script version
        /// </summary>
        public String ScriptVersion { get { return typeof(Ashe).Assembly.GetName().Version.ToString(); } }

        /// <summary>
        /// Getter for the Player
        /// </summary>
        public AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }


        public double TimeToHit(AttackableUnit target)
        {
            return Player.Distance(target, false) / Orbwalking.GetMyProjectileSpeed();
        }

        public double Time(AttackableUnit target)
        {
            return this.TimeToHit(target) + Player.AttackCastDelay;
        }

        #endregion



        #region Constructor
        /// <summary>
        /// Ctor
        /// </summary>
        public Ashe()
        {
            _menu = new AsheMenu(this);

            _drawings = new AsheDrawings(this);

            _spells[Spells.W].SetSkillshot(0.5f, 100, 900, true, SkillshotType.SkillshotCone);
            _spells[Spells.R].SetSkillshot(0.5f, 100, 1600, false, SkillshotType.SkillshotCone);
        }
        #endregion

        #region Functions

        internal Spell GetSpell(Spells spell)
        {
            return _spells[spell];
        }

        /// <summary>
        /// OnLoad Callback
        /// </summary>
        /// <param name="args"></param>
        public void OnLoad(EventArgs args)
        {
            Console.WriteLine("loadin");

            if (Player.ChampionName != "Ashe")
                return;
            _menu.Initialize();
            _drawings.Initialize();

            Game.OnUpdate += onGameUpdate;
            Drawing.OnDraw += this._drawings.Drawing_OnDraw;
            Notifications.AddNotification(String.Format("{0} by Romesti v{1}", this.GetNamespace(), ScriptVersion), 1000);
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
        }

        void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var menuItemQ = _menu.GetKeyForMode("UseQ",Program.Orbwalker.ActiveMode);
            Console.WriteLine("Mode : " + menuItemQ);
            if (menuItemQ == null) return;
            Console.WriteLine("not null");
            var useQ = menuItemQ.GetValue<bool>();

            if(!useQ) return;

            if (useQ && !chechForUlt(Spells.Q) && this.IsQMaxStacked && _spells[Spells.Q].IsReady())
            {
                _spells[Spells.Q].Cast();
            }
        }



        private void onGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            switch (Program.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Mixed();
                    break;
            }

            AutoR();

        }

        #region Helpers
        private bool chechForUlt(Spells spellslot)
        {
            var menuItem = _menu.GetKeyForMode("SaveR", Program.Orbwalker.ActiveMode);
            var saveR = menuItem == null ? false : menuItem.GetValue<bool>();
            return _spells[Spells.R].IsReady() && saveR && Player.Mana - 50 < 100;
        }
        #endregion

        #region Main Behaviors
        public void Combo(AIHeroClient target = null)
        {
            target = target ?? TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65, TargetSelector.DamageType.Physical);
            var useQ = false;// Menu.GetItemEndKey("UseQ").GetValue<bool>();
            var useW = Menu.GetItemEndKey("UseW").GetValue<bool>();
            var useWMana = Menu.GetItemEndKey("UseWMana").GetValue<Slider>().Value;
            var useR = Menu.GetItemEndKey("UseR").GetValue<bool>();
            var saveR = Menu.GetItemEndKey("SaveR").GetValue<bool>();

            if (target == null || !target.IsValid)
                return;


            if (useQ && !chechForUlt(Spells.Q) && this.IsQMaxStacked && _spells[Spells.Q].IsReady())
            {
                _spells[Spells.Q].Cast();
            }
            if (useW && !chechForUlt(Spells.W) && Player.ManaPercent > useWMana && _spells[Spells.W].IsReady())
                _spells[Spells.W].CastIfHitchanceEquals(target, _menu.ComboHitChance);

            if (useR && _spells[Spells.R].IsReady())
            {
                _spells[Spells.R].CastIfHitchanceEquals(target, _menu.ComboHitChance);
            }

        }

        public void Mixed(AIHeroClient target = null)
        {
            target = target ?? TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(null) + 65, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValid)
                return;



            var useW = Menu.GetItemEndKey("UseW").GetValue<bool>();
            var useQ = false;// Menu.GetItemEndKey("UseQ").GetValue<bool>();
            var manaThreshold = Menu.GetItemEndKey("ManaThreshold").GetValue<Slider>().Value;

            if (Player.ManaPercent < manaThreshold)
                return;

            if (useQ && this.IsQMaxStacked && _spells[Spells.Q].IsReady())
            {
                _spells[Spells.Q].Cast();
            }

            if (useW && _spells[Spells.W].IsReady())
                _spells[Spells.W].CastIfHitchanceEquals(target, _menu.ComboHitChance);

        }

        public void LaneClear()
        {
            var minions = MinionManager.GetMinions(
                            ObjectManager.Player.ServerPosition,
                                    _spells[Spells.W].Range, MinionTypes.All,
                                    MinionTeam.Enemy,
                                    MinionOrderTypes.MaxHealth)
                            .Where(minion => minion.IsValidTarget(_spells[Spells.W].Range));

            var useQ = false;// Menu.GetItemEndKey("UseQ", "Clear").GetValue<bool>();
            var useW = Menu.GetItemEndKey("UseW", "Clear").GetValue<bool>();

            if (minions.FirstOrDefault() == null) return;

            if (useQ && this.IsQMaxStacked && _spells[Spells.Q].IsReady() && minions.Count(m => Program.Orbwalker.InAutoAttackRange(m)) > 1)
            {
                _spells[Spells.Q].Cast();
            }

            if (useW && _spells[Spells.W].IsReady() && minions.Count() > 3)
            {
                _spells[Spells.W].Cast(minions.Last(), false);
            }
        }

        public void JungleClear()
        {
            var minions = MinionManager.GetMinions(
            ObjectManager.Player.ServerPosition, _spells[Spells.W].Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            var useQ = false;// Menu.GetItemEndKey("UseQ", "Clear").GetValue<bool>();
            var useW = Menu.GetItemEndKey("UseW", "Clear").GetValue<bool>();


            var wMinion = minions.FindAll(minion => minion.IsValidTarget(_spells[Spells.W].Range)).FirstOrDefault();

            if (wMinion == null) return;

            if (useQ && this.IsQMaxStacked && _spells[Spells.Q].IsReady())
            {
                _spells[Spells.Q].Cast();
            }

            if (useW && _spells[Spells.W].IsReady())
            {
                _spells[Spells.W].Cast(wMinion);
            }

        }
        #endregion

        #region Advanced Behaviors
        public void AutoR()
        {
            var autoR = Menu.GetItemEndKey("Toggle", "Misc.AutoR").GetValue<KeyBind>().Active;
            if (!autoR) return;
            var range = Menu.GetItemEndKey("Range", "Misc.AutoR").GetValue<Slider>().Value;
            var hitchance = Menu.GetItemEndKey("Hitchance", "Misc.AutoR").GetHitchance();
            var target = TargetSelector.GetTarget(range, TargetSelector.DamageType.Physical, false);

            if (target == null) return;

            if (_spells[Spells.R].IsReady())
            {
                _spells[Spells.R].CastIfHitchanceEquals(target, hitchance);
            }
        }

        private void Kite(Obj_AI_Base target)
        {
            Program.Orbwalker.ForceTarget(target);
        }

        public void KiteBehavior()
        {
            var enemies = HeroManager.Enemies
                                .Where(enemy => Orbwalking.InAutoAttackRange(enemy));

            var target = enemies.FirstOrDefault(enemy => !(enemy.HasBuff("")));

        }
        #endregion

        #region ComboDamage

        public float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (_spells[Spells.Q].IsReady())
            {
                damage += _spells[Spells.Q].GetDamage(enemy);
            }

            if (_spells[Spells.W].IsReady())
            {
                damage += _spells[Spells.W].GetDamage(enemy);
            }

            if (_spells[Spells.E].IsReady())
            {
                damage += _spells[Spells.E].GetDamage(enemy);
            }

            if (_spells[Spells.R].IsReady())
            {
                damage += _spells[Spells.R].GetDamage(enemy);
            }

            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
            {
                damage += (float)Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }

            return damage;
        }

        #endregion


        #endregion

    }
}
