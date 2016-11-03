using EloBuddy; 
 using LeagueSharp.Common; 
 namespace xSaliceResurrected_Rework.Base
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using System;
    using Managers;

    public class Champion : SpellBase
    {
        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;

        protected readonly AIHeroClient Player = ObjectManager.Player;

        protected Champion()
        {
            //Events
            Game.OnUpdate += Game_OnGameUpdateEvent;
            Drawing.OnDraw += Drawing_OnDrawEvent;
            Interrupter2.OnInterruptableTarget += Interrupter_OnPosibleToInterruptEvent;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloserEvent;
            AttackableUnit.OnDamage += ObjAiHeroOnOnDamage;

            GameObject.OnCreate += GameObject_OnCreateEvent;
            GameObject.OnDelete += GameObject_OnDeleteEvent;

            Spellbook.OnUpdateChargeableSpell += Spellbook_OnUpdateChargeableSpellEvent;
            Spellbook.OnCastSpell += SpellbookOnOnCastSpell;
            Spellbook.OnStopCast += SpellbookOnOnStopCast;

            Orbwalking.AfterAttack += AfterAttackEvent;
            Orbwalking.BeforeAttack += BeforeAttackEvent;
            Orbwalking.OnAttack += OnAttack;

            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCastEvent;
            EloBuddy.Player.OnIssueOrder += ObjAiHeroOnOnIssueOrderEvent;
            Obj_AI_Base.OnBuffGain += ObjAiBaseOnOnBuffAdd;
            Obj_AI_Base.OnBuffLose += ObjAiBaseOnOnBuffLose;
        }

        public Champion(bool load)
        {
            if (load)
            {
                GameOnLoad();
            }
        }

        private void GameOnLoad()
        {
            Chat.Print(
                "<font color = \"#FFB6C1\">xSalice's Ressurected AIO</font> by <font color = \"#00FFFF\">xSalice</font>, imsosharp Update, NightMoon Rework!");

            Menu = new Menu("xSalice's " + Player.ChampionName, Player.ChampionName, true);

            Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker"));

            var itemMenu = new Menu("Activator", "Items");
            ItemManager.AddToMenu(itemMenu);
            Menu.AddSubMenu(itemMenu);

            Menu.AddToMainMenu();

            var pluginLoader = new PluginLoader();
        }

        #region 
        private void Drawing_OnDrawEvent(EventArgs args)
        {
            if (Player.IsDead) return;

            Drawing_OnDraw(args);
        }

        protected virtual void Drawing_OnDraw(EventArgs args)
        {
      
        }

        private void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            OnSpellCast(sender, args);
        }

        protected virtual void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            
        }

        private void AntiGapcloser_OnEnemyGapcloserEvent(ActiveGapcloser gapcloser)
        {
            AntiGapcloser_OnEnemyGapcloser(gapcloser);
        }

        protected virtual void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
       
        }

        private void Interrupter_OnPosibleToInterruptEvent(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            Interrupter_OnPosibleToInterrupt(unit, spell);
        }

        protected virtual void Interrupter_OnPosibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {

        }

        private void Game_OnGameUpdateEvent(EventArgs args)
        {
            if (Player.IsDead && Player.ChampionName.ToLower() != "karthus") return;

            Game_OnGameUpdate(args);
        }

        protected virtual void Game_OnGameUpdate(EventArgs args)
        {

        }

        private void GameObject_OnCreateEvent(GameObject sender, EventArgs args)
        {
            GameObject_OnCreate(sender, args);
        }

        protected virtual void GameObject_OnCreate(GameObject sender, EventArgs args)
        {

        }

        private void GameObject_OnDeleteEvent(GameObject sender, EventArgs args)
        {
            GameObject_OnDelete(sender, args);
        }

        protected virtual void GameObject_OnDelete(GameObject sender, EventArgs args)
        {

        }

        private void Obj_AI_Base_OnProcessSpellCastEvent(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {
            Obj_AI_Base_OnProcessSpellCast(unit, args);
        }

        protected virtual void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {

        }

        private void AfterAttackEvent(AttackableUnit unit, AttackableUnit target)
        {
            AfterAttack(unit, target);
        }

        protected virtual void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {

        }

        private void BeforeAttackEvent(Orbwalking.BeforeAttackEventArgs args)
        {
            BeforeAttack(args);
        }

        protected virtual void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
           
        }

        protected virtual void OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            
        }

        private void ObjAiHeroOnOnIssueOrderEvent(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            ObjAiHeroOnOnIssueOrder(sender, args);
        }

        protected virtual void ObjAiHeroOnOnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
           
        }

        private void Spellbook_OnUpdateChargeableSpellEvent(Spellbook sender, SpellbookUpdateChargeableSpellEventArgs args)
        {
            Spellbook_OnUpdateChargeableSpell(sender, args);
        }

        protected virtual void Spellbook_OnUpdateChargeableSpell(Spellbook sender, SpellbookUpdateChargeableSpellEventArgs args)
        {
            
        }

        protected virtual void SpellbookOnOnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            
        }

        protected virtual void SpellbookOnOnStopCast(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
        {
          
        }

        protected virtual void ObjAiBaseOnOnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {

        }

        protected virtual void ObjAiBaseOnOnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {

        }

        protected virtual void ObjAiHeroOnOnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {

        }
        #endregion
    }
}