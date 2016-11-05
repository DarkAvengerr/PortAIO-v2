using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    internal class Insec
    {
        public static Vector3 PosBehindW;
        public static Vector3 PosBehindF;

        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Tristana.Player;
            }
        }

        private static Spell W
        {
            get
            {
                return SkyLv_Tristana.W;
            }
        }

        private static Spell R
        {
            get
            {
                return SkyLv_Tristana.R;
            }
        }
        #endregion

        static Insec()
        {
            //Menu
            SkyLv_Tristana.Menu.SubMenu("Insec").AddItem(new MenuItem("Tristana.InsecSpellUsage", "Insec Spell Usage").SetValue(new StringList(new[] { "Only Flash > R > W", "Only W > R", "Flash > R > W  &  W > R" }, 2)));
            SkyLv_Tristana.Menu.SubMenu("Insec").AddItem(new MenuItem("Tristana.InsecMode", "Mode").SetValue(new StringList(new[] { "Tower - Hero - Current", "Mouse Position", "Current Position" }, 0)));
            SkyLv_Tristana.Menu.SubMenu("Insec").AddItem(new MenuItem("Tristana.MoveWhenInsec", "Move To Mouse Position On Insec KeyPress").SetValue(true));
            SkyLv_Tristana.Menu.SubMenu("Insec").AddItem(new MenuItem("Tristana.UsePacketCastInsec", "Use PacketCast In Insec").SetValue(false));
            SkyLv_Tristana.Menu.SubMenu("Insec").AddItem(new MenuItem("Tristana.InsecKey", "Insec !").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (SkyLv_Tristana.Menu.Item("Tristana.InsecKey").GetValue<KeyBind>().Active)
            {
                if (SkyLv_Tristana.Menu.Item("Tristana.MoveWhenInsec").GetValue<bool>())
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    InsecLogic();
                }

                if (!SkyLv_Tristana.Menu.Item("Tristana.MoveWhenInsec").GetValue<bool>())
                {
                    InsecLogic();
                }
            }
        }

        public static void InsecLogic()
        {
            switch (SkyLv_Tristana.Menu.Item("Tristana.InsecSpellUsage").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    {
                        var PacketCast = SkyLv_Tristana.Menu.Item("Tristana.UsePacketCastInsec").GetValue<bool>();
                        if (TargetSelector.SelectedTarget == null) return;
                        var target = TargetSelector.GetSelectedTarget();

                        if (SkyLv_Tristana.InsecState == false)
                        {
                            PosBehindW = CustomLib.GetBehindPosition(target);
                            PosBehindF = CustomLib.GetBehindPositionExtend(target);
                        }

                        if (Utils.GameTimeTickCount - SkyLv_Tristana.lastInsec > 2500 && SkyLv_Tristana.InsecState == true)
                        {
                            SkyLv_Tristana.InsecState = false;
                            return;
                        }

                        if (SkyLv_Tristana.InsecState == true && Player.Distance(PosBehindW) < Player.Distance(target) && R.IsReady() && W.IsReady() && !SkyLv_Tristana.FlashSlot.IsReady() && Player.Mana >= R.ManaCost + W.ManaCost)
                        {
                            R.Cast(target);
                            SkyLv_Tristana.lastInsec = Utils.GameTimeTickCount;
                            SkyLv_Tristana.InsecState = true;
                            return;
                        }

                        if (SkyLv_Tristana.InsecState == false && Player.Distance(target) < 420 && Player.Distance(PosBehindW) > Player.Distance(target) && SkyLv_Tristana.FlashSlot.IsReady() && R.IsReady() && W.IsReady() && Player.Mana >= R.ManaCost + W.ManaCost)
                        {
                            Player.Spellbook.CastSpell(SkyLv_Tristana.FlashSlot, PosBehindF);
                            SkyLv_Tristana.lastInsec = Utils.GameTimeTickCount;
                            SkyLv_Tristana.InsecState = true;
                            return;
                        }

                        if (SkyLv_Tristana.InsecState == true && !R.IsReady() && !SkyLv_Tristana.FlashSlot.IsReady() && W.IsReady() && Player.Mana >= W.ManaCost)
                        {
                            W.Cast(SkyLv_Tristana.REndPosition);
                            SkyLv_Tristana.lastInsec = Utils.GameTimeTickCount;
                            SkyLv_Tristana.InsecState = false;
                            return;
                        }
                        break;
                    }

                case 1:
                    {
                        var PacketCast = SkyLv_Tristana.Menu.Item("Tristana.UsePacketCastInsec").GetValue<bool>();
                        if (TargetSelector.SelectedTarget == null) return;
                        var target = TargetSelector.GetSelectedTarget();

                        if (SkyLv_Tristana.InsecState == false)
                        {
                            PosBehindW = CustomLib.GetBehindPosition(target);
                        }

                        if (Utils.GameTimeTickCount - SkyLv_Tristana.lastInsec > 2500 && SkyLv_Tristana.InsecState == true)
                        {
                            SkyLv_Tristana.InsecState = false;
                            return;
                        }

                        if (SkyLv_Tristana.InsecState == true && Player.Distance(PosBehindW) < Player.Distance(target) && R.IsReady() && !W.IsReady() && Player.Mana >= R.ManaCost)
                        {
                            R.Cast(target);
                            SkyLv_Tristana.lastInsec = Utils.GameTimeTickCount;
                            SkyLv_Tristana.InsecState = false;
                            return;
                        }

                        if (SkyLv_Tristana.InsecState == false && Player.Distance(PosBehindW) < W.Range && Player.Distance(PosBehindW) > Player.Distance(target) && W.IsReady() && R.IsReady() && Player.Mana >= R.ManaCost + W.ManaCost)
                        {
                            W.Cast(PosBehindW);
                            SkyLv_Tristana.lastInsec = Utils.GameTimeTickCount;
                            SkyLv_Tristana.InsecState = true;
                            return;
                        }
                        break;
                    }

                case 2:
                    {
                        var PacketCast = SkyLv_Tristana.Menu.Item("Tristana.UsePacketCastInsec").GetValue<bool>();
                        if (TargetSelector.SelectedTarget == null) return;
                        var target = TargetSelector.GetSelectedTarget();

                        if (SkyLv_Tristana.InsecState == false)
                        {
                            PosBehindW = CustomLib.GetBehindPosition(target);
                            PosBehindF = CustomLib.GetBehindPositionExtend(target);
                        }

                        if (Utils.GameTimeTickCount - SkyLv_Tristana.lastInsec > 2500 && SkyLv_Tristana.InsecState == true)
                        {
                            SkyLv_Tristana.InsecState = false;
                            return;
                        }

                        if (SkyLv_Tristana.InsecState == true && Player.Distance(PosBehindW) < Player.Distance(target) && R.IsReady() && ((W.IsReady() && Player.Mana >= R.ManaCost + W.ManaCost) || (!W.IsReady() && Player.Mana >= R.ManaCost)) && (!SkyLv_Tristana.FlashSlot.IsReady() || !W.IsReady()))
                        {
                            R.Cast(target);
                            SkyLv_Tristana.lastInsec = Utils.GameTimeTickCount;
                            if (W.IsReady())
                            {
                                SkyLv_Tristana.InsecState = true;
                                return;
                            }

                            if (!W.IsReady())
                            {
                                SkyLv_Tristana.InsecState = false;
                                return;
                            }
                        }

                        if (SkyLv_Tristana.InsecState == false && Player.Distance(target) < 420 && Player.Distance(PosBehindW) > Player.Distance(target) && SkyLv_Tristana.FlashSlot.IsReady() && R.IsReady() && W.IsReady() && Player.Mana >= R.ManaCost + W.ManaCost)
                        {
                            Player.Spellbook.CastSpell(SkyLv_Tristana.FlashSlot, PosBehindF);
                            SkyLv_Tristana.lastInsec = Utils.GameTimeTickCount;
                            SkyLv_Tristana.InsecState = true;
                            return;
                        }

                        if (SkyLv_Tristana.InsecState == false && (Player.Distance(target) > 420 || !SkyLv_Tristana.FlashSlot.IsReady()) && Player.Distance(PosBehindW) < W.Range && Player.Distance(PosBehindW) > Player.Distance(target) && W.IsReady() && R.IsReady() && Player.Mana >= R.ManaCost + W.ManaCost)
                        {
                            W.Cast(PosBehindW);
                            SkyLv_Tristana.lastInsec = Utils.GameTimeTickCount;
                            SkyLv_Tristana.InsecState = true;
                            return;
                        }

                        if (SkyLv_Tristana.InsecState == true && !R.IsReady() && W.IsReady() && Player.Mana >= W.ManaCost)
                        {
                            W.Cast(SkyLv_Tristana.REndPosition);
                            SkyLv_Tristana.lastInsec = Utils.GameTimeTickCount;
                            SkyLv_Tristana.InsecState = false;
                            return;
                        }
                        break;
                    }
            }
        }
    }
}
