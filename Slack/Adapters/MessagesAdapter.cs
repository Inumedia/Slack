using SlackAPI;
using System;
using System.Drawing;

namespace Slack.Adapters
{
    public abstract class MessagesAdapter
    {
        protected ConnectedInterface connected;
        protected SlackClient client;

        public abstract string GetId();
        public abstract string GetTitle();
        public abstract string GetTopic();
        public abstract MessageHistory GetMessages(DateTime? latest = null, DateTime? oldest = null, int? count = null);

        public virtual User GetUserInfo(string userId)
        {
            if (connected.Client.UserLookup.ContainsKey(userId))
                return connected.Client.UserLookup[userId];
            else return null;
        }

        public virtual CachedPictureBox GetUserImage(string userId)
        {
            if (connected.Client.UserLookup.ContainsKey(userId))
                return new CachedPictureBox(connected.Client.UserLookup[userId].profile.image_48);
            else
                return new CachedPictureBox("https://slack.global.ssl.fastly.net/8390/img/avatars/ava_0014-48.png");
        }

        public abstract string GetChannelPrefix();

        public virtual bool IsStarred()
        {
            return connected.Client.starredChannels.Contains(GetId());
        }

        public virtual void MarkRead(DateTime latest)
        {
            client.MarkChannel((m) => { }, GetId(), latest);
        }
    }
}
