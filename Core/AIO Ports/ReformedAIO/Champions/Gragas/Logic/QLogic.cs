using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gragas.Logic
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    #endregion

    internal class QLogic
    {
        #region Fields

        public GameObject GragasQ;

        #endregion

        #region Public Methods and Operators

        public bool CanExplodeQ(Obj_AI_Base target)
        {
            return GragasQ != null && target.Distance(GragasQ.Position) <= 250;
        }

        public bool CanThrowQ()
        {
            return GragasQ == null;
        }

        public void Load()
        {
            if (Variable.Spells != null)
            {
                Variable.Spells[SpellSlot.Q].SetSkillshot(0.3f, 110f, 1000f, false, SkillshotType.SkillshotCircle);
                Variable.Spells[SpellSlot.R].SetSkillshot(0.3f, 700f, 1000f, false, SkillshotType.SkillshotCircle);
                Variable.Spells[SpellSlot.E].SetSkillshot(0.15f, 25f, 900f, true, SkillshotType.SkillshotLine);
            }

            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        public Vector3 QPred(AIHeroClient target)
        {
            var pos = Variable.Spells[SpellSlot.Q].GetPrediction(target);

            return pos.CastPosition;
        }

        #endregion

        #region Methods

        private void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Gragas_Base_Q_Ally.troy")
            {
                GragasQ = null;
            }
        }

        private void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Gragas_Base_Q_Ally.troy")
            {
                GragasQ = sender;
            }
        }

        private float QDelay(Obj_AI_Base target)
        {
            var time = target.Distance(Variable.Player) / Variable.Spells[SpellSlot.R].Speed;

            return time + Variable.Spells[SpellSlot.R].Delay;
        }

        #endregion
    }
}