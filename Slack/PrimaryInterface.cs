using SlackAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Slack
{
    public class PrimaryInterface : Form
    {
        AuthStartResponse authStart;
        UserTeamCombo teamSelected;
        string token;

        EmailLogin email;
        TeamList teams;
        PasswordLogin password;
        ConnectedInterface connected;
        public PrimaryInterface()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.favicon;
            this.Text = "Slack";
            this.BackgroundImage = Properties.Resources.background;

            email = new EmailLogin((r) =>
            {
                authStart = r;
                BeginInvoke(new Action(ShowTeams));
            });
            email.Dock = DockStyle.Fill;
            email.BackColor = Color.Transparent;

            Controls.Add(email);
        }

        void ShowTeams()
        {
            teams = null;

            teams = new TeamList(authStart, (u) =>
            {
                teamSelected = u;
                BeginInvoke(new Action(SelectedTeam));
            });

            teams.Dock = DockStyle.Fill;
            teams.BackColor = Color.Transparent;

            Controls.Add(teams);
            email.Visible = false;
        }

        void SelectedTeam()
        {
            password = new PasswordLogin(teamSelected, (p) =>
            {
                token = p;
                AuthSignIn();
            });

            password.Dock = DockStyle.Fill;
            password.BackColor = Color.Transparent;

            Controls.Add(password);
            teams.Visible = false;
        }

        void AuthSignIn()
        {
            EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset);
            SlackSocketClient client = new SlackSocketClient(token);
            client.OnHello += () =>
            {
                wait.Set();
            };
            client.Connect((l) =>
            {

                if (!l.ok) return;

                BeginInvoke(new Action(() =>
                {
                    connected = new ConnectedInterface(client, l);
                    connected.Dock = DockStyle.Fill;

                    Controls.Add(connected);

                    password.Visible = false;
                }));
            });
            wait.WaitOne();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PrimaryInterface
            // 
            //this.AutoScaleDimensions = new SizeF(6F, 13F);
            //this.AutoScaleMode = AutoScaleMode.Font;

            this.BackColor = Color.White;
            this.ClientSize = new Size(1024, 786);
            this.Name = "PrimaryInterface";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }
    }
}
