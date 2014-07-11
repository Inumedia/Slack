using SlackAPI;
using SlackAPI.WebSocketMessages;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Slack
{
    public class SlackChannelList : Panel
    {
        public ConnectedInterface parent;

        TitledList starred;
        TitledList channels;
        TitledList users;
        TitledList groups;
        int margin = 16;
        int x = 0;

        List<Tuple<string, Label>> userLabels;

        public SlackChannelList(ConnectedInterface connected)
        {
            parent = connected;

            starred = new TitledList(
                "STARRED",
                connected, (c) => GetLabel(parent.Client.starredChannels[c]), 
                new Label()
                {
                    Text = "\uF005",
                    Font = new Font(CustomFonts.Fonts.Families[0], 10),
                    ForeColor = Color.FromArgb(0xab, 0x9b, 0xa9)
                }, 
                -2, 
                (i) => SelectCallback(parent.Client.starredChannels[i]), 
                (i) => true
            );

            channels = new TitledList(
                "CHANNELS", 
                connected, 
                (i) => GetLabel(parent.Client.Channels[i].id), 
                null, 
                0, 
                (i) => SelectCallback(connected.Client.Channels[i].id), 
                (i) => true
            );

            users = new TitledList(
                "DIRECT MESSAGES",
                connected, 
                (i) => GetUserLabel(connected.Client.UserLookup[connected.Client.DirectMessages[i].user]),
                null,
                0,
                (i) => SelectCallback(connected.Client.DirectMessages[i].id),
                (i) => connected.Client.Users[i].presence.Equals("active", StringComparison.InvariantCultureIgnoreCase)
            );

            groups = new TitledList(
                "PRIVATE GROUPS", 
                connected, (i) => GetLabel(connected.Client.Groups[i].id),
                null, 
                0, 
                (i) => SelectCallback(connected.Client.Groups[i].id), 
                (i) => true
            );

            Controls.AddRange(new Control[] { starred, channels, users, groups });

            FlushDimensions();

            this.AutoScroll = true;

            parent.Client.BindCallback<PresenceChange>((r) =>
                BeginInvoke(new Action(UpdatePresences))
            );
        }

        void UpdatePresences()
        {
            if (userLabels != null)
                foreach (Tuple<string, Label> l in userLabels)
                {
                    string userId = l.Item1;
                    User user = parent.Client.Users.Find((c) => c.id.Equals(userId, StringComparison.InvariantCultureIgnoreCase));
                    bool isActive = user.presence.Equals("active", StringComparison.InvariantCultureIgnoreCase);
                    if (isActive)
                        l.Item2.ForeColor = Color.FromArgb(0x4a, 0x83, 0x7d);
                    else
                        l.Item2.ForeColor = Color.FromArgb(0x62, 0x53, 0x61);
                }
        }

        public void SelectFirstChannel()
        {
            //TODO: Possible bug?
            //Flush();
            Invoke(new Action(() => channels.SelectIndex(0)));
        }

        public void SelectChannel(string channelId)
        {
            string name = parent.LookupName(channelId);

            Label sel;
            MouseEventHandler handler = null;
            if (parent.Client.starredChannels.Contains(channelId))
            {
                sel = starred.entries.First((c) => c.Text.Equals(name));
                handler = starred.clickCallbacks[sel];
            }
            else
            {
                switch (channelId[0])
                {
                    case 'G':
                        sel = groups.entries.First((c) => c.Text.Equals(name));
                        handler = groups.clickCallbacks[sel];
                        break;
                    case 'C':
                        sel = channels.entries.First((c) => c.Text.Equals(name));
                        handler = channels.clickCallbacks[sel];
                        break;
                    case 'U':
                        sel = users.entries.First((c) => c.Text.Equals(name));
                        handler = users.clickCallbacks[sel];
                        break;
                }
            }
            if (handler != null)
                handler(this, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));
        }

        void SelectCallback(string channelId)
        {
            starred.ClearSet();
            channels.ClearSet();
            users.ClearSet();
            groups.ClearSet();

            parent.ChangeChannel(channelId);
        }

        Tuple<int, Label> GetLabel(string channelId)
        {
            Label t = null;
            int offset = 0;
            switch (channelId[0])
            {
                case 'U':
                    return GetUserLabel(parent.Client.UserLookup[channelId]);
                case 'G':
                    t = new Label();
                    t.Font = new Font(CustomFonts.Fonts.Families[0], 9);
                    t.Location = new Point(-7, 0);
                    t.Text = "\uF023";
                    t.Margin = new Padding(0);
                    break;
                default:
                    t = new Label();
                    t.Font = new Font(CustomFonts.Fonts.Families[1], 10);
                    t.Location = new Point(-4, 0);
                    t.Margin = new Padding(0);
                    t.Text = "#"; //For channel only, but just incase there's other kinds of IDs.
                    offset = -2;
                    break;
            }

            t.Name = channelId;
            t.Size = t.PreferredSize;

            return new Tuple<int,Label>(offset, t);
        }

        Tuple<int, Label> GetUserLabel(User u)
        {
            Label l;
            if (u.IsSlackBot)
            {
                l = new Label()
                {
                    Text = "\uF004",
                    Location = new Point(-5, 0),
                    Font = new Font(CustomFonts.Fonts.Families[0], 8),
                    ForeColor = Color.FromArgb(0x4a, 0x83, 0x7d),
                    Name = u.id,
                };
                l.Size = new Size(20, l.PreferredHeight);
                userLabels.Add(new Tuple<string, Label>(u.id, l));

                return new Tuple<int, Label>(-3, l);
            }
            else
            {
                l = new Label()
                {
                    Text = "\u25CF",
                    Location = new Point(0, 0),
                    Font = new Font(CustomFonts.Fonts.Families[1], 16),
                    Name = u.id,
                };
                l.Size = new Size(18, l.PreferredHeight);
                userLabels.Add(new Tuple<string, Label>(u.id, l));

                return new Tuple<int, Label>(-6, l);
            }
        }

        public void Flush()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(Flush));
                return;
            }

            userLabels = new List<Tuple<string, Label>>();

            groups.SetEntries(parent.Client.Groups.Select((g) => parent.Client.starredChannels.Contains(g.id) ? null : g.name).ToArray());
            channels.SetEntries(parent.Client.Channels.Select((g) => parent.Client.starredChannels.Contains(g.id) ? null : g.name).ToArray());
            users.SetEntries(parent.Client.DirectMessages.Select((g) => parent.Client.starredChannels.Contains(g.id) ? null : parent.LookupName(g.user)).ToArray());
            starred.SetEntries(parent.Client.starredChannels.Select((g) => parent.LookupName(g)).ToArray());

            FlushDimensions();
            UpdatePresences();
        }

        public void FlushDimensions()
        {
            int previousBottom = 0;

            previousBottom = SetHeight(starred, previousBottom, margin);
            previousBottom = SetHeight(channels, previousBottom, margin);
            previousBottom = SetHeight(users, previousBottom, margin);
            previousBottom = SetHeight(groups, previousBottom, margin);
        }

        int SetHeight(TitledList c, int previous, int margin)
        {
            if (c.entries == null || c.entries.Length == 0)
                c.Visible = false;
            else
            {
                c.Height = c.PreferredSize.Height;
                c.Location = new Point(x, previous + margin);
                c.Visible = true;
                return c.Bottom + margin;
            }
            return previous;
        }
    }
}