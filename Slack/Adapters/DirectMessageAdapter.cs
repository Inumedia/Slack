using SlackAPI;
using System;
using System.Threading;

namespace Slack.Adapters
{
    public class DirectMessageAdapter : MessagesAdapter
    {
        protected DirectMessageConversation info;

        public DirectMessageAdapter(DirectMessageConversation info, ConnectedInterface connected, SlackClient client)
        {
            this.info = info;
            this.connected = connected;
            this.client = client;
        }

        public override string GetId()
        {
            return info.id;
        }

        public override string GetTitle()
        {
            return GetUserInfo(info.user).name;
        }

        public override string GetTopic()
        {
            return null;
        }

        public override MessageHistory GetMessages(DateTime? latest = null, DateTime? oldest = null, int? count = null)
        {
            MessageHistory history = null;
            EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset);
            client.GetDirectMessageHistory((his) =>
            {
                history = his;
                wait.Set();
            }, info, latest, oldest, count);
            wait.WaitOne();

            return history;
        }

        public override string GetChannelPrefix()
        {
            return "@";
        }
    }
}
