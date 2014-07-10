using System;
using System.Windows.Forms;

namespace Slack
{
    public class KeyboardShortcuts
    {
        public static void PreviousChannel(ChatInterface chat, int keyValue, Keys modifiers)
        {

        }

        public static void NextChannel(ChatInterface chat, int keyValue, Keys modifiers)
        {

        }

        public static void PreviousChannelUnread(ChatInterface chat, int keyValue, Keys modifiers)
        {
           
        }
        public static void NextChannelUnread(ChatInterface chat, int keyValue, Keys modifiers)
        {

        }
        public static void MarkRead(ChatInterface chat, int keyValue, Keys modifiers)
        {

        }
        public static void MarkAllRead(ChatInterface chat, int keyValue, Keys modifiers)
        {

        }
        public static void ReprintLastCommand(ChatInterface chat, int keyValue, Keys modifiers)
        {

        }
        /// <summary>
        /// Will likely handle all auto completes. Names, channels, and emojis.
        /// </summary>
        public static void AutoComplete(ChatInterface chat, int keyValue, Keys modifiers)
        {

        }
        public static void HighlightToBeginningOfLine(ChatInterface chat, int keyValue, Keys modifiers)
        {

        }
        public static void HighlightToEndOfLine(ChatInterface chat, int keyValue, Keys modifiers)
        {

        }
        /// <summary>
        /// Will handle both forms of creating new snippets from pastes.
        /// </summary>
        public static void NewSnippet(ChatInterface chat, int keyValue, Keys modifiers)
        {

        }
        public static void ToggleFlexpane(ChatInterface chat, int keyValue, Keys modifiers)
        {

        }
        public static void KeyboardShortcutsOverlay(ChatInterface chat, int keyValue, Keys modifiers)
        {

        }
        public static void QuickSwitcher(ChatInterface chat, int keyValue, Keys modifiers)
        {

        }
        public static void EditLastMessage(ChatInterface chat, int keyValue, Keys modifiers)
        {

        }
        public static void ScrollThroughMessages(ChatInterface chat, int keyValue, Keys modifiers)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class Shortcut : Attribute
    {
        public Shortcut(int keyValue, Keys modifiers)
        {

        }
    }
}
