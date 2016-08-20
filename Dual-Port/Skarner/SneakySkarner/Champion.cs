using System;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SneakySkarner
{
    class Champion
    {
        protected static Menu Config;
        protected static Spell Q, W, E, R;
        protected static Orbwalking.Orbwalker Orbwalker;
        protected static AIHeroClient Player;

        public static SpellDataInst Qdata = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q);
        public static SpellDataInst Wdata = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W);
        public static SpellDataInst Edata = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E);
        public static SpellDataInst Rdata = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R);

        public Champion()
        {
            Game_OnGameLoad();
            Drawing.OnDraw += Drawing_OnDraw;

            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Game.OnUpdate += Game_OnUpdate;
        }

        protected virtual void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //throw new NotImplementedException();
        }

        protected virtual void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            //throw new NotImplementedException();
        }

        protected virtual void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            //throw new NotImplementedException();
        }

        protected virtual void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            //throw new NotImplementedException();
        }

        protected virtual void Game_OnGameLoad()
        {
            //throw new NotImplementedException();
        }

        protected virtual void Game_OnUpdate(EventArgs args)
        {
            //throw new NotImplementedException();
        }

        protected virtual void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            //throw new NotImplementedException();
        }

        protected virtual void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            //throw new NotImplementedException();
        }

        protected virtual void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            //throw new NotImplementedException();
        }

        protected virtual void Drawing_OnDraw(EventArgs args)
        {
            //throw new NotImplementedException();
        }

        protected static float GetComboDamage(AIHeroClient hero)
        {
            return 0f;
        }

        
    }
}
