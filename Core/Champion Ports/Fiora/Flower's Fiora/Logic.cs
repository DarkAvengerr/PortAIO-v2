using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Fiora
{
    using Common;
    using Manager.Passive;
    using Manager.Events;
    using Manager.Menu;
    using Manager.Spells;
    using LeagueSharp;
    using LeagueSharp.Common;

    public class Logic
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static SpellSlot Ignite = SpellSlot.Unknown;
        public static int SkinID;
        public static Menu Menu;
        public static AIHeroClient Me;
        public static Orbwalking.Orbwalker Orbwalker;
        public static HpBarDraw DrawHpBar = new HpBarDraw();

        internal static void Load()
        {
            Me = ObjectManager.Player;
            SkinID = ObjectManager.Player.SkinId;

            SpellManager.Init();
            MenuManager.Init();
            PassiveManager.Init();
            Evade.EvadeManager.Init();
            Evade.EvadeTargetManager.Init();

            Obj_AI_Base.OnSpellCast += DoCastManager.Init;
            Game.OnUpdate += UpdateManager.Init;
            Drawing.OnDraw += DrawManager.Init;
        }

        internal static void EnbaleSkin(object obj, OnValueChangeEventArgs Args)
        {
            if (!Args.GetNewValue<bool>())
            {
                //ObjectManager.//Player.SetSkin(ObjectManager.Player.ChampionName, SkinID);
            }
        }

        internal static void ItemsUse(bool UseYoumuu = false, bool UseTiamat = false, bool UseHydra = false, bool LaneClear = false)
        {
            if (UseYoumuu && Items.HasItem(3142) && Items.CanUseItem(3142) && Me.CountEnemiesInRange(W.Range) > 0)
            {
                Items.UseItem(3142);
            }

            if (UseTiamat && (Me.CountEnemiesInRange(385f) > 0 || LaneClear) && Items.HasItem(3077) && Items.CanUseItem(3077))
            {
                Items.UseItem(3077);
            }

            if (UseHydra && (Me.CountEnemiesInRange(385f) > 0 || LaneClear))
            {
                if (Items.HasItem(3074) && Items.CanUseItem(3074))
                {
                    Items.UseItem(3074);
                }
                else if (Items.HasItem(3748) && Items.CanUseItem(3748))
                {
                    Items.UseItem(3748);
                }
            }
        }
    }
}
