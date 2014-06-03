using SlackAPI;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace SlackInterface
{
    public class PasswordLogin : Panel
    {
        UserTeamCombo combo;

        Label title;
        CustomTextbox passwordInput;
        CustomButton signin;
        Action<string> onPasswordAuth;
        SlackLoading loadingAnimation;

        public PasswordLogin(UserTeamCombo userTeam, Action<string> OnPasswordAuthSuccess)
        {
            combo = userTeam;
            onPasswordAuth = OnPasswordAuthSuccess;

            title = new Label()
            {
                Text = string.Format("Enter your password for {0}", userTeam.team),
                Font = new Font(CustomFonts.Fonts.Families[1], 20)
            };
            title.Size = title.PreferredSize;

            passwordInput = new CustomTextbox(ProcessAuth, "Password");
            passwordInput.BorderRadius = 4;
            passwordInput.Size = new Size(300, 34);
            passwordInput.OnError += (s) =>
            {
                BeginInvoke(new Action(() =>
                {
                    loadingAnimation.StopAnimation();
                    loadingAnimation.Visible = false;
                }));
            };
            passwordInput.PreCallback += () => signin.Enabled = false;

            signin = new CustomButton()
            {
                Text = "Sign in"
            };
            signin.Size = signin.PreferredSize;

            signin.Click += (o, e) =>
            {
                passwordInput.PressEnter();
            };

            loadingAnimation = new SlackLoading();
            loadingAnimation.Size = new Size(70, 70);
            loadingAnimation.Visible = false;

            passwordInput.PasswordChar = '*';
            Controls.Add(passwordInput);
            Controls.Add(loadingAnimation);
            Controls.Add(title);
            Controls.Add(signin);

            DoResize();
        }

        void ProcessAuth(string password)
        {
            BeginInvoke(new Action(() =>
            {
                loadingAnimation.Visible = true;
                loadingAnimation.StartAnimation();
            }));

            EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset);

            Slack.AuthSignin((s) =>
            {
                if (!s.ok)
                {
                    wait.Set();
                    return;

                }

                try
                {
                    onPasswordAuth(s.token);
                }
                finally
                {
                    wait.Set();
                }
            }, combo.user_id, combo.team_id, password);
            wait.WaitOne();

            BeginInvoke(new Action(() =>
            {
                loadingAnimation.Visible = false;
                loadingAnimation.StopAnimation();

                signin.Enabled = true;
            }));
        }

        void DoResize()
        {
            title.Location = new Point((ClientSize.Width - title.Width) / 2, (ClientSize.Height / 2) - (title.Height * 2));
            passwordInput.Location = new Point((ClientSize.Width - passwordInput.Width) / 2, ClientSize.Height / 2);
            signin.Location = new Point((ClientSize.Width - signin.Width) / 2, passwordInput.Bottom + 7);
            loadingAnimation.Location = new Point(passwordInput.Right + 7, (passwordInput.Top + passwordInput.Height / 2) - (loadingAnimation.Height / 2));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            DoResize();
        }
    }
}
