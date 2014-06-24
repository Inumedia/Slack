using SlackAPI;
using Slack.Adapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timing = System.Threading.Timer;

namespace Slack
{
    public partial class ChatInterface : UserControl
    {
        private Panel chatFooter;
        private Panel channelTitle;
        private SelectablePanel chatContent;
        private BorderPanel inputContainer;
        /// <summary>
        /// Refer to Slack.UploadFile
        /// </summary>
        private Button uploadFile;

        public readonly string Id;
        List<MessageEntry> entries;
        public ConnectedInterface connected;
        MessagesAdapter adapter;

        Font messages;
        Font timestamps;

        Dictionary<int, int> sizes;

        bool isTyping;

        public ChatInterface(MessagesAdapter adapter, ConnectedInterface connected)
        {
            this.connected = connected;
            this.adapter = adapter;
            entries = new List<MessageEntry>();
            Id = adapter.GetId();

            messages = new Font(CustomFonts.Fonts.Families[1], 11);
            timestamps = new Font(CustomFonts.Fonts.Families[1], 9);

            InitializeComponent();

            uploadFile.Font = new Font(CustomFonts.Fonts.Families[0], 18);
            textInput.Font = new Font(CustomFonts.Fonts.Families[1], 11);

            string title = adapter.GetTitle();
            string topic = adapter.GetTopic();

            Label titleLabel = new Label();
            titleLabel.Text = title;
            titleLabel.Location = new Point(24, 0);
            titleLabel.Font = new Font(CustomFonts.Fonts.Families[1], 16, FontStyle.Bold);
            titleLabel.Size = new Size(titleLabel.PreferredWidth, 53);
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            channelTitle.Controls.Add(titleLabel);

            sizes = new Dictionary<int, int>();

            MessageHistory history = adapter.GetMessages(null, null, null);

            lastTimeStamp = history.latest;

            bool firstFetch = lastMessage == null;
            foreach (SlackAPI.Message message in history.messages.Reverse())
                AddMessage(message);

            connected.Client.OnMessageReceived += Client_OnMessageReceived;
            //BeginInvoke(new Action(() => chatContent.AutoScrollPosition = new Point(lastMessage.Left, chatContent.DisplayRectangle.Height)));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            chatContent.AutoScrollPosition = new Point(lastMessage == null ? 0 : lastMessage.Left, chatContent.DisplayRectangle.Height);
        }

        void Client_OnMessageReceived(SlackClient.ReceivingMessage obj)
        {
            if (obj.channel.Equals(Id))
                BeginInvoke(new Action(() =>
                {
                    AddMessage(new SlackAPI.Message()
                    {
                        text = obj.text,
                        user = obj.user,
                        ts = obj.ts
                    });
                    
                    //if (((chatContent.VerticalScroll.Value + 1) * 1f / chatContent.VerticalScroll.Maximum * 1f) > .9f)
                    //    chatContent.VerticalScroll.Value = chatContent.VerticalScroll.Maximum;
                }));
        }

        DateTime lastTimeStamp;
        MessageEntry lastMessage;

        void AddMessage(SlackAPI.Message entry)
        {
            if (entry.user == null || entry.text == null) return;

            string username = adapter.GetUserInfo(entry.user).name;

            if (lastMessage != null && lastMessage.OwnerUsername.Equals(username) && entry.ts.Subtract(lastMessage.lastTimeStamp) < TimeSpan.FromMinutes(5))
                lastMessage.AppendMessage(entry.text, entry.ts);
            else
            {
                int lastY = 0;
                if (lastMessage != null) lastY = lastMessage.Bottom;

                lastMessage = new MessageEntry(this, username, adapter.GetUserImage(entry.user), entry.text, entry.ts, messages, timestamps);
                lastMessage.Location = new Point(0, lastY + 10);
                lastMessage.Width = chatContent.Size.Width-16; //TODO: Replace this with anchors and widths in the child panel.
                chatContent.Controls.Add(lastMessage);
            }

            chatContent.AutoScrollPosition = new Point(lastMessage.Left, chatContent.DisplayRectangle.Height);
        }

        internal void HandleMouseWheel(MouseEventArgs e)
        {
            chatContent.HandleMouseWheel(e);// OnMouseWheel(e);
        }

        private TextBox textInput;

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
            this.chatFooter = new Panel();
            this.uploadFile = new Button();
            this.inputContainer = new BorderPanel();
            this.textInput = new TextBox();
            this.channelTitle = new Panel();
            this.chatContent = new SelectablePanel();
            this.chatFooter.SuspendLayout();
            this.inputContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // chatFooter
            // 
            this.chatFooter.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            //this.chatFooter.BorderStyle = BorderStyle.Fixed3D; //
            this.chatFooter.Controls.Add(this.uploadFile);
            this.chatFooter.Controls.Add(this.inputContainer);
            this.chatFooter.Location = new Point(0, 722);
            this.chatFooter.Margin = new Padding(0);
            this.chatFooter.Name = "chatFooter";
            this.chatFooter.Size = new Size(804, 64);
            this.chatFooter.TabIndex = 1;
            // 
            // uploadFile
            // 
            this.uploadFile.FlatStyle = FlatStyle.Flat;
            this.uploadFile.FlatAppearance.BorderColor = Color.FromArgb(0xe0, 0xe0, 0xe0);
            this.uploadFile.Location = new Point(21, 0);
            this.uploadFile.Margin = new Padding(0, 0, 1, 0);
            this.uploadFile.Name = "uploadFile";
            this.uploadFile.Size = new Size(44, 41);
            this.uploadFile.TabIndex = 0;
            this.uploadFile.Text = "";
            this.uploadFile.TextAlign = ContentAlignment.MiddleRight;
            this.uploadFile.UseVisualStyleBackColor = true;
            this.uploadFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
            uploadFile.ForeColor = Color.FromArgb(0xb8, 0xb9, 0xc0);

            uploadFile.MouseEnter += (o, e) =>
            {
                uploadFile.ForeColor = Color.White;
                uploadFile.BackColor = Color.FromArgb(0xeb, 0x4d, 0x5c);
            };

            uploadFile.MouseLeave += (o, e) =>
            {
                uploadFile.ForeColor = Color.FromArgb(0xb8, 0xb9, 0xc0);
                uploadFile.BackColor = Color.White;
            };

            // 
            // inputContainer
            // 
            this.inputContainer.BackColor = Color.Transparent;
            this.inputContainer.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
            //this.inputContainer.BorderStyle = BorderStyle.FixedSingle;
            this.inputContainer.Controls.Add(this.textInput);
            this.inputContainer.Location = new Point(21, 0);
            this.inputContainer.Margin = new Padding(64, 0, 24, 3);
            this.inputContainer.Name = "inputContainer";
            this.inputContainer.Size = new Size(767, 41);
            inputContainer.Click += inputContainer_Click;
            inputContainer.Cursor = Cursor.Current = Cursors.IBeam;
            this.inputContainer.TabIndex = 0;
            // 
            // textInput
            // 
            this.textInput.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            this.textInput.BorderStyle = BorderStyle.None;
            //this.textInput.BorderStyle = BorderStyle.Fixed3D; // 
            this.textInput.Font = new Font(CustomFonts.Fonts.Families[1], 12);
            this.textInput.TextChanged += textInput_TextChanged;
            this.textInput.KeyDown += textInput_KeyDown;
            this.textInput.Multiline = true;
            this.textInput.Location = new Point(52, 10);
            this.textInput.Margin = new Padding(8, 11, 32, 11);
            this.textInput.Name = "textInput";
            this.textInput.Size = new Size(672, textInput.PreferredHeight);
            this.textInput.TabIndex = 1;
            this.textInput.ShortcutsEnabled = true;
            // 
            // channelTitle
            // 
            this.channelTitle.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            this.channelTitle.Location = new Point(0, 0);
            //this.channelTitle.BorderStyle = BorderStyle.Fixed3D; //
            this.channelTitle.Margin = new Padding(0);
            this.channelTitle.Name = "channelTitle";
            this.channelTitle.Size = new Size(804, 53);
            this.channelTitle.TabIndex = 0;
            // 
            // chatContent
            // 
            this.chatContent.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
            this.chatContent.Location = new Point(21, 53);
            this.chatContent.Margin = new Padding(0);
            this.chatContent.Name = "chatContent";
            this.chatContent.Size = new Size(783, 666);
            //this.chatContent.BorderStyle = BorderStyle.Fixed3D; //
            this.chatContent.AutoScroll = true;
            //this.chatContent.HorizontalScroll.Enabled = false;
            //this.chatContent.HorizontalScroll.Visible = false;
            this.chatContent.TabIndex = 2;
            // 
            // ChatInterface
            // 
            this.BackColor = Color.White;
            this.Controls.Add(this.chatContent);
            this.Controls.Add(this.chatFooter);
            this.Controls.Add(this.channelTitle);
            this.Margin = new Padding(0);
            this.Name = "ChatInterface";
            this.Size = new Size(804, 786);
            this.chatFooter.ResumeLayout(false);
            this.inputContainer.ResumeLayout(false);
            this.inputContainer.PerformLayout();
            this.ResumeLayout(false);
        }

        void inputContainer_Click(object sender, EventArgs e)
        {
            textInput.Focus();
        }

        void textInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13 && e.Modifiers == Keys.None)
            {
                //textInput.Enabled = false;
                //inputContainer.Enabled = false;
                connected.SendMessage((r) =>
                {
                    BeginInvoke(new Action(() =>
                    {
                        //textInput.Text = "";
                        textInput.Enabled = true;
                        inputContainer.Enabled = true;
                        AddMessage(new SlackAPI.Message()
                        {
                            text = r.text,
                            user = connected.Client.MySelf.id,
                            ts = r.ts
                        });
                        //chatContent.VerticalScroll.Value = chatContent.VerticalScroll.Maximum;
                    }));
                }, textInput.Text);

                e.Handled = true;
                e.SuppressKeyPress = true;
                textInput.Text = "";
            }

            if (e.KeyValue == 8 && e.Modifiers == Keys.Control)
            {
                //SendKeys.SendWait("^+{LEFT}{BACKSPACE}");
                e.SuppressKeyPress = true;
            }
        }
        
        int previousSize;
        void textInput_SizeChanged(object sender, EventArgs e)
        {
            if (textInput.Width == previousSize)
                return;

            ResizeTextInput();

            previousSize = textInput.Width;
        }

        void textInput_TextChanged(object sender, EventArgs e)
        {
            if(!isTyping && textInput.Text.Length > 0)
            {
                isTyping = true;
                connected.Client.SendTyping(Id);
            }
            else if (isTyping && textInput.Text.Length == 0)
            {
                isTyping = false;
                connected.Client.SendTyping(Id);
            }

            ResizeTextInput();
        }

        void ResizeTextInput()
        {
            const int padding = 3;
            int lineCount = textInput.GetLineFromCharIndex(textInput.TextLength) + 1;
            int border = textInput.Height - textInput.ClientSize.Height;
            int currentHeight = textInput.Font.Height * lineCount + padding + border;

            chatFooter.Height = currentHeight + 44;
            chatFooter.Location = new Point(0, Height - chatFooter.Height);
            chatContent.Height = Height - channelTitle.Height - chatFooter.Height - 3;
            //uploadFile.Height = inputContainer.Height;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            base.OnPaint(e);
        }

        #endregion
    }
}
