using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;using DetuksSharp;
using LeagueSharp.Common;

using EloBuddy; namespace ARAMDetFull.Champions
{
    class Elise : Champion
    {
        private static bool _human;

        private static bool _spider;

        private static Spell _humanQ, _humanW, _humanE, _r, _spiderQ, _spiderW, _spiderE;

        private readonly float[] HumanQcd = { 6, 6, 6, 6, 6 };

        private readonly float[] HumanWcd = { 12, 12, 12, 12, 12 };

        private readonly float[] HumanEcd = { 14, 13, 12, 11, 10 };

        private readonly float[] SpiderQcd = { 6, 6, 6, 6, 6 };

        private readonly float[] SpiderWcd = { 12, 12, 12, 12, 12 };

        private readonly float[] SpiderEcd = { 26, 23, 20, 17, 14 };

        private float _humQcd = 0, _humWcd = 0, _humEcd = 0;

        private float _spidQcd = 0, _spidWcd = 0, _spidEcd = 0;

        private float _humaQcd = 0, _humaWcd = 0, _humaEcd = 0;

        private float _spideQcd = 0, _spideWcd = 0, _spideEcd = 0;

        public Elise()
        {
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPosibleToInterrupt;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                    {
                        new ConditionalItem(ItemId.Sorcerers_Shoes),
                        new ConditionalItem(ItemId.Rabadons_Deathcap),
                        new ConditionalItem(ItemId.Banshees_Veil,ItemId.Randuins_Omen,ItemCondition.ENEMY_AP),
                        new ConditionalItem(ItemId.Void_Staff,ItemId.Zhonyas_Hourglass,ItemCondition.ENEMY_AP),
                        new ConditionalItem(ItemId.Rylais_Crystal_Scepter),
                        new ConditionalItem(ItemId.Liandrys_Torment),
                    },
                startingItems = new List<ItemId>
                    {
                        ItemId.Needlessly_Large_Rod
                    }
            };

        }

        private static void CheckSpells()
        {
            if (player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseHumanQ" ||
                player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseHumanW" ||
                player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseHumanE")
            {
                _human = true;
                _spider = false;
            }

            if (player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseSpiderQCast" ||
                player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseSpiderW" ||
                player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseSpiderEInitial")
            {
                _human = false;
                _spider = true;
            }
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                //Chat.Print("Spell name: " + args.SData.Name.ToString());
                GetCDs(args);
                CheckSpells();
            }
        }
        private float CalculateCd(float time)
        {
            return time + (time * player.PercentCooldownMod);
        }
         private void GetCDs(GameObjectProcessSpellCastEventArgs spell)
        {
            if (_human)
            {
                if (spell.SData.Name == "EliseHumanQ")
                    _humQcd = Game.Time + CalculateCd(HumanQcd[_humanQ.Level-1]);
                if (spell.SData.Name == "EliseHumanW")
                    _humWcd = Game.Time + CalculateCd(HumanWcd[_humanW.Level - 1]);
                if (spell.SData.Name == "EliseHumanE")
                    _humEcd = Game.Time + CalculateCd(HumanEcd[_humanE.Level - 1]);
            }
            else
            {
                if (spell.SData.Name == "EliseSpiderQCast")
                    _spidQcd = Game.Time + CalculateCd(SpiderQcd[_spiderQ.Level - 1]);
                if (spell.SData.Name == "EliseSpiderW")
                    _spidWcd = Game.Time + CalculateCd(SpiderWcd[_spiderW.Level - 1]);
                if (spell.SData.Name == "EliseSpiderEInitial")
                    _spidEcd = Game.Time + CalculateCd(SpiderEcd[_spiderE.Level] - 1);
            }
        }

         private void Cooldowns()
         {
             _humaQcd = ((_humQcd - Game.Time) > 0) ? (_humQcd - Game.Time) : 0;
             _humaWcd = ((_humWcd - Game.Time) > 0) ? (_humWcd - Game.Time) : 0;
             _humaEcd = ((_humEcd - Game.Time) > 0) ? (_humEcd - Game.Time) : 0;
             _spideQcd = ((_spidQcd - Game.Time) > 0) ? (_spidQcd - Game.Time) : 0;
             _spideWcd = ((_spidWcd - Game.Time) > 0) ? (_spidWcd - Game.Time) : 0;
             _spideEcd = ((_spidEcd - Game.Time) > 0) ? (_spidEcd - Game.Time) : 0;
         }

        private void Interrupter_OnPosibleToInterrupt(Obj_AI_Base target, InterruptableSpell spell)
        {
            if (player.LSDistance(target) < _humanE.Range && target != null && _humanE.GetPrediction(target).Hitchance >= HitChance.Low)
            {
                _humanE.Cast(target);
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_spiderE.LSIsReady() && _spider && gapcloser.Sender.LSIsValidTarget(_spiderE.Range))
            {
                _spiderE.Cast(gapcloser.Sender);
            }
            if (_humanE.LSIsReady() && _human && gapcloser.Sender.LSIsValidTarget(_humanE.Range))
            {
                _humanE.Cast(gapcloser.Sender);
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
        }

        public override void useW(Obj_AI_Base target)
        {
        }

        public override void useE(Obj_AI_Base target)
        {
        }


        public override void useR(Obj_AI_Base target)
        {
        }

        public override void useSpells()
        {
            var target = ARAMTargetSelector.getBestTarget(_humanW.Range);
            if (target == null) return; //buffelisecocoon
            CheckSpells();
            var qdmg = player.LSGetSpellDamage(target, SpellSlot.Q);
            var wdmg = player.LSGetSpellDamage(target, SpellSlot.W);
            if (_human)
            {
                if (target.LSDistance(player.Position) < _humanE.Range  && _humanE.LSIsReady())
                {
                    if (_humanE.GetPrediction(target).Hitchance >= HitChance.High)
                    {
                        _humanE.Cast(target);
                    }
                }

                if (player.LSDistance(target) <= _humanQ.Range  && _humanQ.LSIsReady())
                {
                    _humanQ.Cast(target);
                }
                if (player.LSDistance(target) <= _humanW.Range && _humanW.LSIsReady())
                {
                    _humanW.Cast(target);
                }
                if (!_humanQ.LSIsReady() && !_humanW.LSIsReady() && !_humanE.LSIsReady() && _r.LSIsReady())
                {
                    _r.Cast();
                }
                if (!_humanQ.LSIsReady() && !_humanW.LSIsReady() && player.LSDistance(target) <= _spiderQ.Range && _r.LSIsReady())
                {
                    _r.Cast();
                }
            }
            if (!_spider) return;
            if (player.LSDistance(target) <= _spiderQ.Range && _spiderQ.LSIsReady())
            {
                _spiderQ.Cast(target);
            }
            if (player.LSDistance(target) <= 200&& _spiderW.LSIsReady())
            {
                _spiderW.Cast();
            }
            if (player.LSDistance(target) <= _spiderE.Range && player.LSDistance(target) > _spiderQ.Range && _spiderE.LSIsReady() && !_spiderQ.LSIsReady())
            {
                if ((safeGap(target)) || target.LSDistance(ARAMSimulator.fromNex.Position, true) < player.LSDistance(ARAMSimulator.fromNex.Position, true))
                    _spiderE.Cast(target);
            }
            if (player.LSDistance(target) > _spiderQ.Range && !_spiderE.LSIsReady() && _r.LSIsReady() && !_spiderQ.LSIsReady() )
            {
                _r.Cast();
            }
            if (_humanQ.LSIsReady() && _humanW.LSIsReady() && _r.LSIsReady())
            {
                _r.Cast();
            }
            if (_humanQ.LSIsReady() && _humanW.LSIsReady() && _r.LSIsReady() )
            {
                _r.Cast();
            }
            if ((_humanQ.LSIsReady() && qdmg >= target.Health || _humanW.LSIsReady() && wdmg >= target.Health) )
            {
                _r.Cast();
            }
        }

        public override void farm()
        {
            if(player.ManaPercent<40)
                return;
            CheckSpells();
            Cooldowns();
            foreach (var minion in MinionManager.GetMinions(player.ServerPosition, _humanQ.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health))
                if (_human)
                {
                    if ( _humanQ.LSIsReady() && minion.LSIsValidTarget() &&
                        player.LSDistance(minion) <= _humanQ.Range)
                    {
                        _humanQ.Cast(minion);
                    }
                    if (_humanW.LSIsReady() && minion.LSIsValidTarget() &&
                        player.LSDistance(minion) <= _humanW.Range)
                    {
                        _humanW.Cast(minion);
                    }
                    if (_r.LSIsReady())
                    {
                        _r.Cast();
                    }
                }else if (_spider)
                {
                    if (_spiderQ.LSIsReady() && minion.LSIsValidTarget() &&
                        player.LSDistance(minion) <= _spiderQ.Range)
                    {
                        _spiderQ.Cast(minion);
                    }
                    if (_spiderW.LSIsReady() && minion.LSIsValidTarget() &&
                        player.LSDistance(minion) <= 125)
                    {
                        _spiderW.Cast();
                    }
                }
        }

        public override void killSteal()
        {
            base.killSteal();
            Cooldowns();
            CheckSpells();
            var target = ARAMTargetSelector.getBestTarget(_humanQ.Range);
            if(target == null) return;
            var igniteDmg = player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            var qhDmg = player.LSGetSpellDamage(target, SpellSlot.Q);
            var wDmg = player.LSGetSpellDamage(target, SpellSlot.W);

            if (_human)
            {
                if (_humanQ.LSIsReady() && player.LSDistance(target) <= _humanQ.Range && target != null)
                {
                    if (target.Health <= qhDmg)
                    {
                        _humanQ.Cast(target);
                    }
                }
                if (_humanW.LSIsReady() && player.LSDistance(target) <= _humanW.Range && target != null)
                {
                    if (target.Health <= wDmg)
                    {
                        _humanW.Cast(target);
                    }
                }
            }
            if (_spider && _spiderQ.LSIsReady() && player.LSDistance(target) <= _spiderQ.Range && target != null)
            {
                if (target.Health <= qhDmg)
                {
                    _spiderQ.Cast(target);
                }
            }
        }

        public override void setUpSpells()
        {
            //Create the spells
            _humanQ = new Spell(SpellSlot.Q, 625f);
            _humanW = new Spell(SpellSlot.W, 1150f);
            _humanE = new Spell(SpellSlot.E, 1075f);
            _spiderQ = new Spell(SpellSlot.Q, 475f);
            _spiderW = new Spell(SpellSlot.W, 0);
            _spiderE = new Spell(SpellSlot.E, 750f);
            _r = new Spell(SpellSlot.R, 0);

            _humanW.SetSkillshot(0.25f, 100f, 1000, true, SkillshotType.SkillshotLine);
            _humanE.SetSkillshot(0.25f, 55f, 1300, true, SkillshotType.SkillshotLine);
        }
    }
}
