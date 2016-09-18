using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using myWorld.Library.MenuWarpper;
using myWorld.Library.SummonerManager;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld.Library.DamageManager
{
    public enum ScalingType
    {
        AP,
        AD,
        BONUS_AD,
        HEALTH,
        ARMOR,
        MR,
        MAXHEALTH,
        MAXMANA,
    }

    public enum DrawType
    {
        DEFAULT,
        UP,
        DOWN,
        BOX,
    }

    public delegate bool IsCondition(Obj_AI_Base target);
    public class DamageLib
    {
        AIHeroClient source;
        float Magic_damage_m = 1;
        float Physical_damage_m = 1;
        string DrawTextMsg = "After HP";
        Dictionary<int, float> cachedDamage = new Dictionary<int, float>();
        List<string> Combos;

        delegate float ScalingF(float a);
        

        Dictionary<string, ScalingF> ScalingFunc = new Dictionary<string, ScalingF>();
        Dictionary<string, RegistedDamage> sources = new Dictionary<string,RegistedDamage>();

        Menu Menu;

        public DamageLib(AIHeroClient source)
        {
            this.source = source;

            #region Scaling Functions Initialization

            ScalingFunc.Add("AP", (x) => (x * source.TotalMagicalDamage));
            ScalingFunc.Add("AD", (x) => (x * source.TotalAttackDamage));
            ScalingFunc.Add("BONUS_AD", (x) => (x * (source.TotalAttackDamage - source.BaseAttackDamage)));
            ScalingFunc.Add("ARMOR", (x) => (x * source.Armor));
            ScalingFunc.Add("MR", (x) => (x * source.MagicShield));
            ScalingFunc.Add("MAXHEALTH", (x) => (x * source.MaxHealth));
            ScalingFunc.Add("MAXMANA", (x) => (x * source.MaxMana));

            #endregion


            Summoner s_IGNITE = new Summoner("summonerdot", 600);
            RegistDamage("IGNITE", DamageType.True, 0, 0, DamageType.True, ScalingType.AD, 0, delegate(Obj_AI_Base target) { return s_IGNITE.IsReady(); }, delegate(Obj_AI_Base target) { return 50 + 20 * source.Level; });
        }

        public void AddToMenu(Menu menu, List<string> Combo)
        {
            Menu = menu;
            Combos = Combo;

            Menu DLibMenu = new Menu("DamageLib", "DamageLib");
            DLibMenu.AddBool("DLib.DrawPredictedHealth", "Draw damage after combo");
            menu.AddSubMenu(DLibMenu);

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        public void SetText(string text)
        {
            DrawTextMsg = text;
        }

        void Drawing_OnDraw(EventArgs args)
        {
            //throw new NotImplementedException();
            if (Menu.GetBool("DLib.DrawPredictedHealth"))
            {
                foreach(AIHeroClient enemy in HeroManager.Enemies.Where(x => x.IsValid && !x.IsDead && x.IsValidTarget()))
                {
                    float damage = CalcComboDamage(enemy, Combos);
                    Color color = IsKillable(enemy, Combos) ? Color.Red : Color.Aqua;

                    if (damage > 0)
                    {

                        Vector2 pos = enemy.HPBarPosition;

                        float after = Math.Max(0, enemy.Health - damage) / enemy.MaxHealth;
                        float posY = pos.Y + 20f;
                        float posDamageX = pos.X + 12f + 103 * after;
                        float position = pos.X + 12f + 103 * enemy.Health / enemy.MaxHealth;

                        float diff = (position - posDamageX) + 3;

                        float pos1 = pos.X + 8 + (107 * after);

                        for (int i = 0; i < diff; i++)
                        {
                            Drawing.DrawLine(pos1 + i, posY, pos1 + i, posY + 10, 1, color);
                        }
                        Drawing.DrawText(pos1, posY + 20, color, string.Format("{0} : {1} => {2}", DrawTextMsg, Math.Ceiling(enemy.Health), Math.Max(0, Math.Ceiling(enemy.Health - damage))));
                    }
                }
            }
        }

        void Game_OnUpdate(EventArgs args)
        {
            //throw new NotImplementedException();
            if (Menu.GetBool("DLib.DrawPredictedHealth"))
            {
                foreach(Obj_AI_Base enemy in HeroManager.Enemies)
                {
                    cachedDamage[enemy.NetworkId] = CalcComboDamage(enemy, Combos);
                }
            }
        }

        public void RegistDamage(string spellId, DamageType damageType, float baseDamage, float perLevel, DamageType scalingType, ScalingType scalingStat, float percentScaling, Func<Obj_AI_Base, bool> condition, Func<Obj_AI_Base, float> extra)
        {
            sources.Add(spellId, new RegistedDamage(damageType, baseDamage, perLevel, scalingType, scalingStat, percentScaling, condition, extra));
        }

        public void RegistDamage(string spellId, DamageType damageType, float baseDamage, float perLevel, List<DamageType> scalingType, List<ScalingType> scalingStat, List<float> percentScaling, Func<Obj_AI_Base, bool> condition, Func<Obj_AI_Base, float> extra)
        {
            sources.Add(spellId, new RegistedDamage(damageType, baseDamage, perLevel, scalingType, scalingStat, percentScaling, condition, extra));
        }

        public float GetScalingDamage(Obj_AI_Base target, DamageType Type, ScalingType Stat, float percentScaling)
        {
            float amount = ScalingFunc[Stat.ToString()](percentScaling);
            if(Type == DamageType.Magical )
            {
                return (float)Magic_damage_m * (float)source.CalcDamage(target, Damage.DamageType.Magical, amount);
            }
            else if(Type == DamageType.Physical)
            {
                return (float)Physical_damage_m * (float)source.CalcDamage(target, Damage.DamageType.Physical, amount);
            }
            else if(Type == DamageType.True)
            {
                return amount;
            }
            return 0f;
        }

        public float CalcComboDamage(Obj_AI_Base target, List<string> combo )
        {
            float totalDamage = 0;
            foreach(string spell in combo)
            {
                totalDamage += CalcSpellDamage(target, spell);
            }

            Magic_damage_m = 1;
            return totalDamage;
        }

        public float CalcSpellDamage(Obj_AI_Base target, string spell)
        {
            try
            {
                if (spell == string.Empty) return 0f;
                if (sources[spell] == null) return 0f;
                //RegistedDamage spellData = sources[spell];
                //if (spellData.condition == null || !spellData.condition(target)) return 0f;
                //float result = 0;

                //DamageType type = spellData.DamageType;
                //result = GetTureDamage(target, spell, sources[spell]);

                SpellSlot slot = ConvertToSpellSlot(spell);
                if (slot == SpellSlot.Unknown) return 0f;
                else return (float)ObjectManager.Player.GetDamageSpell(target, slot).CalculatedDamage;

                //return result;
            }
            catch(Exception ex)
            {
                return 0f;
            }
        }

        public bool IsKillable(Obj_AI_Base target, List<string> Combos)
        {
            float damage = CalcComboDamage(target, Combos);
            if (target.Health < damage)
            {
                return true;
            }
            return false;
        }

        public float GetTureDamage(Obj_AI_Base target, string spell, RegistedDamage data)
        {
            float baseDamage = data.baseDamage = 0f;
            float perLevel = data.perLevel = 0f;
            List<DamageType> scalingType = data.ScalingType;
            List<ScalingType> scalingStat = data.ScalingStat;
            List<float> percentScaling = data.percentScaling;
            float extra = data.extra(target);
            if (!data.condition(target)) return 0f;

            float ScalingDamage = 0f;

            for (int i = 0; i < scalingStat.Count; i++ )
            {
                ScalingDamage += GetScalingDamage(target, scalingType[i], scalingStat[i], percentScaling[i]);
            }

            if(data.DamageType == DamageType.Magical)
            {
                return (float)Magic_damage_m * (float)source.CalcDamage(target, Damage.DamageType.Magical, baseDamage+ perLevel * GetSpellLevel(spell) + extra) + ScalingDamage;
            }
            else if(data.DamageType == DamageType.Physical)
            {
                return (float)Physical_damage_m * (float)source.CalcDamage(target, Damage.DamageType.Physical, baseDamage+ perLevel * GetSpellLevel(spell) + extra) + ScalingDamage;
            }
            else if(data.DamageType == DamageType.True)
            {
                return baseDamage + perLevel * GetSpellLevel(spell) + extra + ScalingDamage;
            }
            return 0;
        }

        public void ChangeDrawType(string id, DrawType type)
        {
            sources[id].ChangeDrawType(type);
        }

        public int GetSpellLevel(SpellSlot slot)
        {
            return source.Spellbook.GetSpell(slot).Level;
        }

        public int GetSpellLevel(string slot)
        {
            SpellSlot ss = ConvertToSpellSlot(slot);
            if(ss == SpellSlot.Unknown)
            {
                return 0;
            }
            else
            {
                return GetSpellLevel(ss);
            }
        }

        public SpellSlot ConvertToSpellSlot(string slot)
        {
            foreach (SpellSlot str in Enum.GetValues(typeof(SpellSlot)))
            {
                if(str.ToString() == slot)
                {
                    return str;
                }
            }
            return SpellSlot.Unknown;
        }
    }

    public class RegistedDamage
    {
        public DamageType DamageType;
        public float baseDamage;
        public float perLevel;
        public List<DamageType> ScalingType = new List<DamageType>();
        public List<ScalingType> ScalingStat = new List<ScalingType>();
        public List<float> percentScaling = new List<float>();
        public Func<Obj_AI_Base, bool> condition;
        public Func<Obj_AI_Base, float> extra;
        public DrawType DrawType = DrawType.DEFAULT;

        public RegistedDamage(DamageType DamageType, float baseDamage, float perLevel, DamageType ScalingType, ScalingType ScalingStat, float percentScaling, Func<Obj_AI_Base, bool> condition, Func<Obj_AI_Base, float> extra)
        {
            this.DamageType = DamageType;
            this.baseDamage = baseDamage;
            this.ScalingType.Add(ScalingType);
            this.ScalingStat.Add(ScalingStat);
            this.percentScaling.Add(percentScaling);
            this.condition = condition;
            this.extra = extra;
        }

        public RegistedDamage(DamageType DamageType, float baseDamage, float perLevel, List<DamageType> ScalingType, List<ScalingType> ScalingStat, List<float> percentScaling, Func<Obj_AI_Base, bool> condition, Func<Obj_AI_Base, float> extra)
        {
            this.DamageType = DamageType;
            this.baseDamage = baseDamage;
            this.ScalingType.AddRange(ScalingType);
            this.ScalingStat.AddRange(ScalingStat);
            this.percentScaling.AddRange(percentScaling);
            this.condition = condition;
            this.extra = extra;
        }

        public void ChangeDrawType(DrawType type)
        {
            this.DrawType = type;
        }
    }
}
