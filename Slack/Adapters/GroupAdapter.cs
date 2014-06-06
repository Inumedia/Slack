using SlackAPI;
using System;
using System.Threading;

namespace Slack.Adapters
{
    public class GroupAdapter : MessagesAdapter
    {
        protected Channel info;
        public GroupAdapter(Channel info, ConnectedInterface connected, SlackClient client)
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
            return info.name;
        }

        public override string GetTopic()
        {
            return info.topic.value;
        }

        public override MessageHistory GetMessages(DateTime? latest = null, DateTime? oldest = null, int? count = null)
        {
            MessageHistory history = null;
            EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset);
            client.GetGroupHistory((his) =>
            {
                history = his;
                wait.Set();
            }, info, latest, oldest, count);
            wait.WaitOne();

            return history;
        }

        public override string GetChannelPrefix()
        {
            return "";
        }
    }
}
