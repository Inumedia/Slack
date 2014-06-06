using SlackAPI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Slack
{
    public partial class MessageEntry : UserControl
    {
        public string OwnerUsername;
        public DateTime lastTimeStamp;

        List<Tuple<Label, ChainLabel>> appended;

        private ChainLabel baseText;
        Tuple<Label, ChainLabel> lastMessage;
        ChatInterface controller;

        Font messageFont;
        Font timeStampFont;

        public MessageEntry(ChatInterface controller, string usersname, CachedPictureBox picture, string initialMessage, DateTime timeStamp, Font message, Font timeStampFont)
        {
            this.controller = controller;
            OwnerUsername = usersname;
            InitializeComponent();

            messageFont = message;
            this.timeStampFont = timeStampFont;

            userName.Text = usersname;
            userName.Font = new Font(CustomFonts.Fonts.Families[1], 11, FontStyle.Bold);
            userName.Size = userName.PreferredSize;
            userName.TextAlign = ContentAlignment.MiddleLeft;
            userName.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            userPicture = picture;
            userPicture.Location = new Point(0, 9);
            userPicture.Name = "UserPicture";
            userPicture.Size = new Size(48, 48);
            userPicture.TabIndex = 0;
            userPicture.TabStop = false;
            Controls.Add(userPicture);

            ts.Font = timeStampFont;
            ts.TextAlign = ContentAlignment.MiddleLeft;
            ts.ForeColor = Color.FromArgb(0xba, 0xbb, 0xbf);

            ts.Text = timeStamp.ToString(controller.connected.Client.MySelf.prefs.time24 ? "HH:mm:ss" : "hh:mm:ss");
            ts.Size = ts.PreferredSize;
            ts.Location = new Point(userName.Right, userName.Bottom - ts.PreferredHeight);

            ts.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            baseText.Text = Regex.Replace(initialMessage, "\r\n|\r|\n", "\r\n");
            baseText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            baseText.Font = message;
            baseText.Size = new Size(Width - 70, 10);
            baseText.Multiline = true;
            baseText.WordWrap = true;
            baseText.ReadOnly = true;
            baseText.Location = new Point(userName.Location.X, userName.Bottom + 5);
            baseText.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            baseText.GotFocus += baseText_GotFocus;
            baseText.MouseWheel += baseText_MouseWheel;

            lastMessage = new Tuple<Label,ChainLabel>(ts, baseText);
            lastTimeStamp = timeStamp;

            appended = new List<Tuple<Label, ChainLabel>>();

            this.ClientSize = this.PreferredSize;

            oldY = this.Location.Y;
            oldHeight = this.Height;
            this.SizeChanged += UpdateSize;
        }

        void baseText_MouseWheel(object sender, MouseEventArgs e)
        {
            controller.HandleMouseWheel(e);
        }

        void baseText_GotFocus(object sender, EventArgs e)
        {
            Externals.User32.HideCaret(((Control)sender).Handle);
        }

        int oldY, oldHeight;

        public event Action<MessageEntry> OnSizeChangeCallback;

        void UpdateSize(object o, EventArgs e)
        {
            if (Location.Y != oldY || Height != oldHeight)
                if (OnSizeChangeCallback != null)
                    OnSizeChangeCallback(this);

            oldY = Location.Y;
            oldHeight = Height;

            ApplyMaxSize(Width);
        }

        Tuple<Label, ChainLabel> GetCombo(string text, DateTime timeStamp)
        {
            ChainLabel message = new ChainLabel();

            message.Text = Regex.Replace(text, "\r\n|\r|\n", "\r\n");
            message.BorderStyle = System.Windows.Forms.BorderStyle.None;
            message.Font = messageFont;
            message.Multiline = true;
            message.Size = new Size(Width - 70, 10);
            message.WordWrap = true;
            message.ReadOnly = true;
            message.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            message.BackColor = Color.White;
            message.GotFocus += baseText_GotFocus;
            message.MouseWheel += baseText_MouseWheel;
            Grow(message);

            Label t = new Label();
            t.Text = timeStamp.ToString(controller.connected.Client.MySelf.prefs.time24 ? "HH:mm:ss" : "hh:mm:ss");
            t.Font = timeStampFont;
            t.ForeColor = ts.ForeColor;
            t.Size = new Size(t.PreferredWidth, ts.Font.Height);
            t.TextAlign = ContentAlignment.MiddleLeft;
            t.Anchor = AnchorStyles.Left | AnchorStyles.Top;

            return new Tuple<Label, ChainLabel>(t, message);
        }

        public void AppendMessage(string newMessage, DateTime timeStamp)
        {
            Tuple<Label, ChainLabel> message = GetCombo(newMessage, timeStamp);

            message.Item2.Location = new Point(baseText.Location.X, lastMessage.Item2.Bottom + 5);
            message.Item1.Location = new Point(-5, message.Item2.Location.Y + 3);

            Controls.Add(message.Item1);
            Controls.Add(message.Item2);
            appended.Add(message);

            this.Height += message.Item2.Height + 5;
            lastTimeStamp = timeStamp;
        }

        public void ApplyMaxSize(int maxWidth)
        {
            baseText.Width = maxWidth - 70;
            Grow(baseText);

            Tuple<Label, ChainLabel> last = new Tuple<Label, ChainLabel>(null, baseText);

            foreach (Tuple<Label, ChainLabel> l in appended)
            {
                l.Item2.Location = new Point(last.Item2.Location.X, last.Item2.Bottom + 5);
                l.Item2.Width = maxWidth - 70;
                Grow(l.Item2);

                last = l;
            }

            if (PreferredSize.Height != Height)
                Height = PreferredSize.Height;
        }

        void Grow(TextBox child)
        {
            //Amount of padding to add
            const int padding = 3;
            //get number of lines (first line is 0)
            int numLines = child.GetLineFromCharIndex(child.TextLength) + 1;
            //get border thickness
            int border = child.Height - child.ClientSize.Height;
            //set height (height of one line * number of lines + spacing)
            child.Height = child.Font.Height * numLines + padding + border;
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.userName = new System.Windows.Forms.Label();
            this.ts = new System.Windows.Forms.Label();
            this.baseText = new ChainLabel();
            this.SuspendLayout();
            // 
            // userName
            // 
            this.userName.AutoSize = true;
            this.userName.Location = new System.Drawing.Point(60, 9);
            this.userName.Name = "userName";
            this.userName.Size = new System.Drawing.Size(52, 13);
            this.userName.TabIndex = 1;
            this.userName.Text = "Someone";
            // 
            // ts
            // 
            this.ts.AutoSize = true;
            this.ts.Location = new System.Drawing.Point(118, 9);
            this.ts.Name = "ts";
            this.ts.Size = new System.Drawing.Size(53, 13);
            this.ts.TabIndex = 2;
            this.ts.Text = "Sometime";
            this.ts.Visible = true;
            // 
            // baseText
            // 
            this.baseText.BackColor = System.Drawing.Color.White;
            this.baseText.Location = new System.Drawing.Point(63, 37);
            this.baseText.Multiline = true;
            this.baseText.Name = "baseText";
            this.baseText.ReadOnly = true;
            this.baseText.Size = new System.Drawing.Size(100, 20);
            this.baseText.TabIndex = 3;
            this.Controls.Add(this.baseText);
            this.Controls.Add(this.ts);
            this.Controls.Add(this.userName);
            this.Name = "MessageEntry";
            this.Size = new System.Drawing.Size(526, 182);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox userPicture;
        private Label userName;
        private Label ts;
    }
}