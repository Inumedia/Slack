using SlackAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Slack
{
    public partial class EmailLogin : UserControl
    {
        private Label welcome;
        private Label howTo;
        private Label errorMessage;
        private CustomTextbox emailContainer;
        private SlackLoading loadingPictureBox;
        Action<AuthStartResponse> authResponse;
        CustomButton login;
        //private TextBox emailInput;

        public EmailLogin(Action<AuthStartResponse> startResponse)
        {
            InitializeComponent();

            welcome.Font = new Font(CustomFonts.Fonts.Families[1], 24, FontStyle.Bold);
            howTo.Font = new Font(CustomFonts.Fonts.Families[1], howTo.Font.Size);
            authResponse = startResponse;
        }

        protected override void OnResize(EventArgs e)
        {
            welcome.Location = new Point((ClientSize.Width - welcome.Width) / 2, (ClientSize.Height / 2) - (welcome.Height * 2));
            howTo.Location = new Point((ClientSize.Width - howTo.Width) / 2, welcome.Bottom);
            emailContainer.Location = new Point((ClientSize.Width - emailContainer.Width) / 2, ClientSize.Height / 2);
            loadingPictureBox.Location = new Point(emailContainer.Right + 7, (emailContainer.Top + emailContainer.Height / 2) - (loadingPictureBox.Height / 2));
            login.Location = new Point(emailContainer.Left + (emailContainer.Width - login.Width) / 2, emailContainer.Bottom + 7);

            base.OnResize(e);
        }

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        void LoginEmail(string emailAddress)
        {
            BeginInvoke(new Action(() =>
            {
                loadingPictureBox.Visible = true;
                loadingPictureBox.StartAnimation();
            }));

            EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset);
            SlackClient.StartAuth((r) =>
            {
                if (r.ok)
                    authResponse(r);
                wait.Set();
            }, emailAddress);
            wait.WaitOne();
            BeginInvoke(new Action(() =>
            {
                loadingPictureBox.StopAnimation();
                loadingPictureBox.Visible = false;
            }));
        }
        private void InitializeComponent()
        {
            this.welcome = new System.Windows.Forms.Label();
            this.howTo = new Label();
            this.emailContainer = new CustomTextbox(LoginEmail, "Your email address");
            emailContainer.OnError += (s) =>
            {
                loadingPictureBox.StopAnimation();
                loadingPictureBox.Visible = false;
            };
            this.emailContainer.SuspendLayout();
            this.SuspendLayout();
           
            //
            // welcome
            // 
            this.welcome.AutoSize = true;
            this.welcome.Location = new System.Drawing.Point(222, 125);
            this.welcome.Name = "welcome";
            this.welcome.Size = new System.Drawing.Size(55, 13);
            this.welcome.Text = "Welcome!";
            this.welcome.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.welcome.Parent = this;
            this.welcome.BackColor = Color.Transparent;
            //
            // howTo
            //
            this.howTo.AutoSize = true;
            this.howTo.Name = "introHowTo";
            this.howTo.Text = "Please type in your email below and press enter to get started.";
            this.howTo.Size = this.howTo.PreferredSize;
            this.howTo.TextAlign = ContentAlignment.MiddleCenter;
            this.howTo.Parent = this;
            this.howTo.BackColor = Color.Transparent;
            // 
            // emailContainer
            // 
            this.emailContainer.Location = new System.Drawing.Point(143, 183);
            this.emailContainer.Name = "emailContainer";
            this.emailContainer.Size = new System.Drawing.Size(300, 34);
            this.emailContainer.TabIndex = 2;
            this.emailContainer.BorderRadius = 4;

            login = new CustomButton()
            {
                Text = "Retrieve account"
            }; 
            login.Size = login.PreferredSize;
            login.Location = new Point((emailContainer.Width - login.Width) / 2, emailContainer.Bottom + 7);
            login.Click += (o, e) => LoginEmail(emailContainer.Text);
            Controls.Add(login);

            this.loadingPictureBox = new SlackLoading();
            this.loadingPictureBox.Size = new Size(70, 70);
            this.loadingPictureBox.Parent = this;
            this.loadingPictureBox.BackColor = Color.Transparent;
            this.loadingPictureBox.Visible = false;
            this.Controls.Add(loadingPictureBox);
            // 
            // EmailLogin
            // 
            this.Controls.Add(this.emailContainer);
            this.Controls.Add(this.welcome);
            this.Controls.Add(howTo);
            this.Name = "EmailLogin";
            this.Size = new System.Drawing.Size(500, 400);
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.emailContainer.ResumeLayout(false);
            this.emailContainer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        protected override void OnLoad(EventArgs e)
        {
            emailContainer.Select();
            base.OnLoad(e);
        }
    }
}