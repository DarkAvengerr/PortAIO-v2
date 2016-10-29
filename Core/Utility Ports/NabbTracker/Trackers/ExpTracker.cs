using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace NabbTracker
{
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;

    /// <summary>
    ///     The drawings class.
    /// </summary>
    internal class ExpTracker
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Loads the range drawings.
        /// </summary>
        public static void Initialize()
        {
            Drawing.OnDraw += delegate
                {
                    foreach (var unit in
                        GameObjects.Heroes.Where(
                            e =>
                            e.IsHPBarRendered
                            && (e.IsMe && Vars.Menu["exptracker"]["me"].GetValue<MenuBool>().Value
                                || e.IsEnemy && Vars.Menu["exptracker"]["enemies"].GetValue<MenuBool>().Value
                                || e.IsAlly && !e.IsMe && Vars.Menu["exptracker"]["allies"].GetValue<MenuBool>().Value))
                        )
                    {
                        var actualExp = unit.Experience.XP;
                        var neededExp = 180 + 100 * unit.Level;
                        Vars.ExpX = (int)unit.HPBarPosition.X + Vars.ExpXAdjustment(unit);
                        Vars.ExpY = (int)unit.HPBarPosition.Y + Vars.ExpYAdjustment(unit);
                        if (unit.Level > 1)
                        {
                            actualExp -= (280 + 80 + 100 * unit.Level) / 2 * (unit.Level - 1);
                        }
                        var expPercent = (int)(actualExp / neededExp * 100);
                        if (unit.Level < 18 || (GameObjects.Player.HasBuff("AwesomeBuff") && unit.Level < 30))
                        {
                            Drawing.DrawLine(
                                Vars.ExpX - 76,
                                Vars.ExpY - 5,
                                Vars.ExpX + 56,
                                Vars.ExpY - 5,
                                7,
                                Colors.Convert(Color.Purple));

                            if (expPercent > 0)
                            {
                                Drawing.DrawLine(
                                    Vars.ExpX - 76,
                                    Vars.ExpY - 5,
                                    Vars.ExpX - 76 + (float)(1.32 * expPercent),
                                    Vars.ExpY - 5,
                                    7,
                                    Colors.Convert(Color.Red));
                            }

                            Vars.DisplayTextFont.DrawText(
                                null,
                                expPercent > 0 ? $"{expPercent}%" : "0%",
                                Vars.ExpX - 13,
                                Vars.ExpY - 13,
                                Colors.Convert(SharpDX.Color.Yellow));
                        }
                    }
                };
        }

        #endregion
    }
}