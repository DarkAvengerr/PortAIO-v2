using System;
using Activator.Base;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Activator.Items.Offensives
{
    class _3043 : CoreItem
    {
        internal override int Id => 3043;
        internal override int Priority => 5;
        internal override string Name => "Muramana";
        internal override string DisplayName => "Muramana";
        internal override int Duration => 100;
        internal override float Range => float.MaxValue;
        internal override MenuType[] Category => new[] { MenuType.SelfMinMP, MenuType.ActiveCheck };
        internal override MapType[] Maps => new[] { MapType.TwistedTreeline, MapType.CrystalScar };
        internal override int DefaultHP => 95;
        internal override int DefaultMP => 35;


        private bool muramana;
        public override void OnTick(EventArgs args)
        {
            return;

            if (muramana)
            {
                if (Player.Mana/Player.MaxMana * 100 <
                    Menu.Item("selfminmp" + Name + "pct").GetValue<Slider>().Value)
                    return;

                if (Menu.Item("mode" + Name).GetValue<StringList>().SelectedIndex != 1 ||
                    Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
                {
                    var manamune = Player.GetSpellSlot("Muramana");
                    if (manamune != SpellSlot.Unknown && !Player.HasBuff("Muramana"))
                    {       
                        Player.Spellbook.CastSpell(manamune);
                        LeagueSharp.Common.Utility.DelayAction.Add(500, () => muramana = false);
                    }
                }
            }

            if (!muramana && !Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                var manamune = Player.GetSpellSlot("Muramana");
                if (manamune != SpellSlot.Unknown && Player.HasBuff("Muramana"))
                {
                    Player.Spellbook.CastSpell(manamune);
                }
            }
        }

        private void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !IsReady() || Game.Version.Contains("6.4"))
            {
                return;
            }

            var spellslot = Player.GetSpellSlot(args.SData.Name);

            if (spellslot == SpellSlot.Q || spellslot == SpellSlot.W ||
                spellslot == SpellSlot.E || spellslot == SpellSlot.R)
            {
                muramana = true;
            }

            if (!args.SData.IsAutoAttack())
            {
                return;
            }

            if (Activator.Origin.Item("usecombo").GetValue<KeyBind>().Active)
            {
                muramana = true;
            }

            else if (args.Target.Type == GameObjectType.AIHeroClient)
            {
                muramana = true;
            }

            else
            {
                LeagueSharp.Common.Utility.DelayAction.Add(
                    650 + (int) ((args.SData.CastFrame / 30) * 1000), () => muramana = false);
            }
        }
    }
}
