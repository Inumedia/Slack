using SlackAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SlackInterface
{
    public class TeamList : SelectablePanel
    {
        UserTeamCombo[] userTeamCombos;
        Action<UserTeamCombo> teamSelected;
        List<CustomButton> teamButtons;
        Font buttonFont;
        public TeamList(AuthStartResponse authStart, Action<UserTeamCombo> onSelectTeam)
        {
            userTeamCombos = authStart.users;
            teamSelected = onSelectTeam;
            buttonFont = new Font(CustomFonts.Fonts.Families[1], 20);
            teamButtons = new List<CustomButton>();

            IEnumerable<CustomButton> buttons = userTeamCombos.Select((c) => CreateTeamButton(c));

            int maxWidth = buttons.Max((c) => c.Width);
            Width = maxWidth;

            int i = 0;
            foreach (CustomButton b in buttons)
            {
                b.Width = maxWidth;
                Controls.Add(b);
                teamButtons.Add(b);
            }
        }

        public CustomButton CreateTeamButton(UserTeamCombo u)
        {
            CustomButton ret = new CustomButton()
            {
                Text = string.Format("Sign in on {0}", u.team)
            };

            ret.Font = buttonFont;
            ret.Size = ret.PreferredSize;
            //ret.FlatStyle = FlatStyle.Flat;
            //ret.BackColor = ret.FlatAppearance.MouseOverBackColor = ret.FlatAppearance.MouseDownBackColor = ret.FlatAppearance.CheckedBackColor = Color.FromArgb(0x00,0x99,0xcc);
            //ret.ForeColor = Color.White;
            ret.TabStop = false;
            //ret.FlatAppearance.BorderSize = 0;
            //ret.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
            ret.Click += (o, e) => teamSelected(u);

            return ret;
        }

        protected override void OnResize(EventArgs eventargs)
        {
            int midWay = teamButtons.Count / 2;
            for (int i = 0; i < teamButtons.Count; ++i)
                teamButtons[i].Location = new Point((Width - teamButtons[i].Width) / 2, (Height - ((i - (midWay - 1)) * teamButtons[i].Height)) / 2);
            
            base.OnResize(eventargs);
        }
    }
}
