using SlackAPI;
using Slack.Adapters;
using Slack.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RegularExpressions = System.Text.RegularExpressions;

namespace Slack
{
    public class ConnectedInterface : UserControl
    {
        public SlackClient Client;

        CachedPictureBox userPicture;

        Panel teamTitle;
        Panel profileTitle;
        SlackChannelList channelList;
        Label username;
        Label TeamName;

        Dictionary<string, ChatInterface> chats;
        ChatInterface activeChat;

        public ConnectedInterface(SlackClient connectedClient, LoginResponse loginDetails)
        {
            Client = connectedClient;
            chats = new Dictionary<string, ChatInterface>();

            InitializeComponent();

            channelList = new SlackChannelList(this);
            channelList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            channelList.AutoScroll = true;
            channelList.BackColor = Color.FromArgb(0x4D, 0x39, 0x4B);
            channelList.Location = new Point(0, 52); //53
            channelList.Margin = new Padding(0);
            channelList.Name = "channelList";
            channelList.Size = new Size(220, 671);
            channelList.TabIndex = 2;
            channelList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Controls.Add(this.channelList);
            channelList.Flush();

            username = new Label()
            {
                Text = Client.MySelf.name,
                Location = new Point(64, 10),
                Size = new Size(120, 24),
                ForeColor = Color.White,
                Font = new Font(CustomFonts.Fonts.Families[1], 14, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomLeft
            };

            userPicture = new CachedPictureBox(Client.MyData.profile.image_48);
            userPicture.Location = new Point(8, 8);
            userPicture.Size = new Size(48, 48);

            TeamName = new Label()
            {
                Text = Client.MyTeam.name,
                Size = new Size(188, 53),
                Location = new Point(16, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.White,
                Font = new Font(CustomFonts.Fonts.Families[1], 14, FontStyle.Bold)
            };

            profileTitle.Controls.Add(username);
            teamTitle.Controls.Add(TeamName);
            profileTitle.Controls.Add(userPicture);

            //Default to general channel.
            SelectChannel(Client.Channels[0].id);

            this.Invalidate();
        }

        public void ChangeChannel(string channelId)
        {
            if (activeChat != null)
            {
                if (activeChat.Id.Equals(channelId))
                    return;

                activeChat.Hide();
            }

            if(chats.ContainsKey(channelId))
            {
                chats[channelId].Show();
                activeChat = chats[channelId];
            }
            else
            {
                MessagesAdapter adapter = null;
                switch (channelId[0])
                {
                    case 'D':
                        adapter = new DirectMessageAdapter(Client.DirectMessages.Find((c) => c.id == channelId), this, Client);
                        break;
                    case 'C':
                        adapter = new ChannelAdapter(Client.ChannelLookup[channelId], this, Client);
                        break;
                    case 'G':
                        adapter = new GroupAdapter(Client.GroupLookup[channelId], this, Client);
                        break;
                }

                if (adapter != null)
                {
                    activeChat = new ChatInterface(adapter, this);
                    activeChat.Location = new Point(220, 0);
                    activeChat.Width = Width - 220;
                    activeChat.Height = Height;
                    activeChat.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;
                    chats.Add(channelId, activeChat);
                    Controls.Add(activeChat);
                }
                else if (activeChat != null)
                    activeChat.Show();
            }
        }

        public void SelectChannel(string channelId)
        {
            channelList.SelectChannel(channelId);
        }

        public void SendMessage(Action<SlackClient.ReceivingMessage> callback, string text, string channelId = null)
        {
            Client.SendMessage(callback, channelId ?? activeChat.Id, text);
            //client.SendMessage(callback, channelId ?? activeChat.Id, text);
        }

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.profileTitle = new System.Windows.Forms.Panel();
            this.teamTitle = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // profileTitle
            // 
            this.profileTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.profileTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(49)))), ((int)(((byte)(60)))));
            this.profileTitle.Location = new System.Drawing.Point(0, 722);
            this.profileTitle.Margin = new System.Windows.Forms.Padding(0);
            this.profileTitle.Name = "profileTitle";
            this.profileTitle.Size = new System.Drawing.Size(220, 64);
            this.profileTitle.TabIndex = 1;
            // 
            // teamTitle
            // 
            this.teamTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(49)))), ((int)(((byte)(60)))));
            this.teamTitle.Location = new System.Drawing.Point(0, 0);
            this.teamTitle.Margin = new System.Windows.Forms.Padding(0);
            this.teamTitle.Name = "teamTitle";
            this.teamTitle.Size = new System.Drawing.Size(220, 53);
            this.teamTitle.TabIndex = 0;
            // 
            // ConnectedInterface
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.profileTitle);
            this.Controls.Add(this.teamTitle);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ConnectedInterface";
            this.Size = new System.Drawing.Size(1024, 786);
            this.ResumeLayout(false);

        }

        public string LookupName(string lookup)
        {
            if (lookup.Contains("<") || lookup.Contains(">"))
            {
                List<Tuple<int, int, string>> replaces = new List<Tuple<int, int, string>>();
                RegularExpressions.MatchCollection matches = RegularExpressions.Regex.Matches(lookup, "<(.*)>");
                foreach (RegularExpressions.Match m in matches)
                {
                    bool first = true;
                    foreach (RegularExpressions.Group g in m.Groups)
                    {
                        if (first) { first = false; continue; }
                        string replacement = LookupName(g.Value);
                        replaces.Add(new Tuple<int, int, string>(g.Index, g.Length, replacement));
                    }
                }

                //Reverse.
                replaces.Sort((a, b) => b.Item1 - a.Item1);
                for (int i = 0; i < replaces.Count; ++i)
                    lookup = lookup.Remove(replaces[i].Item1, replaces[i].Item2).Insert(replaces[i].Item1, replaces[i].Item3);

                return lookup;
            }

            if (lookup.Contains("|"))
            {
                return lookup.Substring(lookup.IndexOf('|') + 1, lookup.Length - lookup.IndexOf('>', lookup.IndexOf('|')));
            }

            if (lookup.StartsWith("#C") || lookup.StartsWith("C"))
            {
                string id = lookup.StartsWith("#") ? lookup.Substring(1) : lookup;
                Channel ch = Client.Channels.Find((c) => c.id.Equals(id));
                if (ch != null) lookup = ch.name;
            }

            if (lookup.StartsWith("@U") || lookup.StartsWith("U"))
            {
                string id = lookup.StartsWith("#") ? lookup.Substring(1) : lookup;
                User us = Client.Users.Find((u) => u.id.Equals(id));
                if (us != null) lookup = us.name;
            }

            if (lookup.StartsWith("G"))
            {
                Channel gr = Client.Groups.Find((g) => g.id.Equals(lookup));
                if (gr != null) lookup = gr.name;
            }
            
            if(lookup.StartsWith("D"))

            if (lookup.StartsWith("!"))
            {
                //TODO: Special commands (<!stuff>, <!everyone>, etc.)
                //Don't even know what to do here yet.  :<
            }

            return lookup;
        }

        #endregion
    }
}
