using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanAnnie
{
    using KoreanAnnie.Common;
    using PortAIO.Properties;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal class AnnieButtons
    {
        #region Fields

        private readonly Annie annie;

        #endregion

        #region Constructors and Destructors

        public AnnieButtons(Annie annie)
        {
            this.annie = annie;

            ShowEasyButton = (KoreanUtils.GetParamBool(annie.MainMenu, "showeeasybutton"));

            int posX = KoreanUtils.GetParamInt(annie.MainMenu, "easybuttonpositionx");
            posX = (posX == 0) ? 50 : posX;
            int posY = KoreanUtils.GetParamInt(annie.MainMenu, "easybuttonpositiony");
            posY = (posY == 0) ? 50 : posY;
            Vector2 pos = new Vector2(posX, posY);

            StunButtonOn = new Render.Sprite(Resources.StunON, pos);
            StunButtonOn.Scale = new Vector2(0.90f, 0.90f);
            StunButtonOn.Add();

            StunButtonOff = new Render.Sprite(Resources.StunOFF, pos);
            StunButtonOff.Scale = new Vector2(0.90f, 0.90f);
            StunButtonOff.Add();

            if (ShowEasyButton)
            {
                StunButtonOn.Visible = KoreanUtils.GetParamBool(annie.MainMenu, "savestunforcombo");
                StunButtonOff.Visible = !StunButtonOn.Visible;
            }

            KoreanUtils.GetParam(annie.MainMenu, "savestunforcombo").ValueChanged += SaveStunForComboValueChanged;
            Game.OnWndProc += ButtonControl;
        }

        #endregion

        #region Properties

        private bool ShowEasyButton { get; set; }
        private Render.Sprite StunButtonOff { get; set; }
        private Render.Sprite StunButtonOn { get; set; }

        #endregion

        #region Methods

        private static bool MouseOnButton(Render.Sprite button)
        {
            Vector2 pos = Utils.GetCursorPos();

            return ((pos.X >= button.Position.X) && pos.X <= (button.Position.X + button.Width)
                    && pos.Y >= button.Position.Y && pos.Y <= (button.Position.Y + button.Height));
        }

        private void ButtonControl(WndEventArgs args)
        {
            ShowEasyButton = (KoreanUtils.GetParamBool(annie.MainMenu, "showeeasybutton"));

            if (ShowEasyButton)
            {
                if ((args.Msg == (uint)WindowsMessages.WM_LBUTTONUP)
                    && (MouseOnButton(StunButtonOn) || MouseOnButton(StunButtonOff)))
                {
                    if (StunButtonOn.Visible)
                    {
                        KoreanUtils.SetValueBool(annie.MainMenu, "savestunforcombo", false);
                    }
                    else if (StunButtonOff.Visible)
                    {
                        KoreanUtils.SetValueBool(annie.MainMenu, "savestunforcombo", true);
                    }
                }
                else if ((args.Msg == (uint)WindowsMessages.WM_MOUSEMOVE) && (args.WParam == 5)
                         && (MouseOnButton(StunButtonOn) || MouseOnButton(StunButtonOff)))
                {
                    MoveButtons(
                        new Vector2(Utils.GetCursorPos().X - (StunButtonOn.Width / 2), Utils.GetCursorPos().Y - 10));
                }

                StunButtonOn.Visible = KoreanUtils.GetParamBool(annie.MainMenu, "savestunforcombo");
                StunButtonOff.Visible = !StunButtonOn.Visible;
            }
            else
            {
                StunButtonOff.Visible = false;
                StunButtonOn.Visible = false;
            }
        }

        private void MoveButtons(Vector2 newPosition)
        {
            StunButtonOn.Position = newPosition;
            StunButtonOn.PositionUpdate = () => newPosition;
            StunButtonOn.PositionUpdate = null;

            StunButtonOff.Position = newPosition;
            StunButtonOff.PositionUpdate = () => newPosition;
            StunButtonOff.PositionUpdate = null;

            KoreanUtils.SetValueInt(annie.MainMenu, "easybuttonpositionx", (int)StunButtonOn.Position.X);
            KoreanUtils.SetValueInt(annie.MainMenu, "easybuttonpositiony", (int)StunButtonOn.Position.Y);
        }

        private void SaveStunForComboValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (ShowEasyButton)
            {
                StunButtonOn.Visible = e.GetNewValue<bool>();
                StunButtonOff.Visible = !StunButtonOn.Visible;
            }
        }

        #endregion
    }
}