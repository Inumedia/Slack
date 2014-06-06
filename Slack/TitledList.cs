using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Slack
{
    public class TitledList : UserControl
    {
        Color selectedColor = Color.FromArgb(0x4c, 0x96, 0x89);
        Action<int> selectCallback;

        int marginSpaceBetweenTitleAndItems = 2;
        public Label[] entries;
        public Label[] entryPrefixes;

        public Dictionary<Label, MouseEventHandler> clickCallbacks;
        Func<int, Tuple<int, Label>> entryPrefixCallback;
        Label titlePrefix;
        Func<int, bool> checkShouldWhiteCallback;

        public TitledList(string title, ConnectedInterface connected, Func<int, Tuple<int, Label>> entryPrefix, Label titlePrefix, int titleOffsetFromPrefix, Action<int> selectCallback, Func<int, bool> checkShouldWhite)
        {
            checkShouldWhiteCallback = checkShouldWhite;

            Controls.Add(titlePrefix);
            InitializeComponent();
            this.title.Text = title;
            this.title.Font = new Font(CustomFonts.Fonts.Families[1], 10);
            this.title.ForeColor = Color.FromArgb(0xab, 0x9b, 0xa9);
            entryPrefixCallback = entryPrefix;
            this.titlePrefix = titlePrefix;
            if (this.titlePrefix != null)
            {
                this.titlePrefix.Location = new Point(0, this.title.Location.Y);
                this.titlePrefix.Size = new Size(this.titlePrefix.PreferredSize.Width + 16, this.title.Height);
                this.titlePrefix.TextAlign = ContentAlignment.MiddleRight;
                this.title.Location = new Point(titlePrefix.Right + titleOffsetFromPrefix, this.title.Location.Y);
            }
            else
                this.title.Location = new Point(16, this.title.Location.Y);
            this.title.Width = 180 - this.title.Location.X;

            this.selectCallback = selectCallback;
        }

        public void SetEntries(string[] data)
        {
            if (entries != null && entries.Length > 0)
                foreach (Label l in entries)
                {
                    Controls.Remove(l);
                    l.Dispose();
                }

            entries = new Label[data.Count((c) => c != null)];
            entryPrefixes = new Label[entries.Length];
            clickCallbacks = new Dictionary<Label, MouseEventHandler>();
            int i = 0;
            for (int k = 0; k < data.Length; ++k)
            {
                if (string.IsNullOrEmpty(data[k])) continue;

                Tuple<int, Label> prefixPair = entryPrefixCallback(i);
                Label prefix = prefixPair.Item2;
                prefix.Location = new Point(prefix.Location.X, title.Location.Y + (title.Size.Height * (i + 1)) + marginSpaceBetweenTitleAndItems);
                prefix.TextAlign = ContentAlignment.MiddleRight;
                if (prefix.ForeColor == Label.DefaultForeColor)
                    prefix.ForeColor = Color.FromArgb(0x62, 0x53, 0x61);
                prefix.Size = new Size(40, title.Size.Height);
                prefix.BackColor = Color.Transparent;

                entries[i] = new Label()
                {
                    Location = new Point(prefix.Right + prefixPair.Item1, prefix.Top),
                    Size = new Size(180-prefix.Right, title.Size.Height),
                    Text = string.Format("{0}", data[k]),
                    Font = title.Font,
                    TextAlign = ContentAlignment.MiddleLeft,
                    ForeColor = title.ForeColor
                };

                prefix.Parent = entries[i];

                entryPrefixes[i] = prefix;

                Controls.Add(entries[i]);
                Controls.Add(entryPrefixes[i]);

                SetCallbacks(entries[i], prefix, i);
                
                ++i;
            }
            this.Height = (entries.Length + 1) * title.Size.Height;
        }

        Tuple<Label, Label> currentlyOver;
        Tuple<Tuple<Label, Color>, Tuple<Label, Color>> selectMemory;

        public void ClearSet()
        {
            if (entries != null)
                for (int i = 0; i < entries.Length; ++i)
                    entries[i].BackColor = entryPrefixes[i].BackColor = Color.Transparent;

            if(currentlyOver != null)
                currentlyOver.Item1.BackColor = currentlyOver.Item2.BackColor = Color.FromArgb(0x3e, 0x31, 0x3c);

            if (selectMemory != null)
            {
                selectMemory.Item1.Item1.ForeColor = selectMemory.Item1.Item2;
                selectMemory.Item2.Item1.ForeColor = selectMemory.Item2.Item2;
                selectMemory = null;
            }
        }

        void SetCallbacks(Label entry, Label prefix, int index)
        {
            EventHandler mouseOver = new EventHandler((o, e) =>
            {
                currentlyOver = new Tuple<Label, Label>(entry, prefix);
                if (entry.BackColor != selectedColor)
                    entry.BackColor = prefix.BackColor = Color.FromArgb(0x3e, 0x31, 0x3c);
                Invalidate();
            });

            EventHandler mouseLeave = new EventHandler((o, e) =>
            {
                if (entry.BackColor != selectedColor)
                    entry.BackColor = prefix.BackColor = Color.Transparent;
                currentlyOver = null;
                Invalidate();
            });

            MouseEventHandler mouseClick = new MouseEventHandler((o, e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    selectCallback(index);
                    entry.BackColor = prefix.BackColor = selectedColor;
                    selectMemory = new Tuple<Tuple<Label, Color>, Tuple<Label, Color>>(new Tuple<Label, Color>(entry, entry.ForeColor), new Tuple<Label, Color>(prefix, prefix.ForeColor));

                    entry.ForeColor = Color.White;

                    //Refer to SlackChannelList.GetUserLabel, I know, it's hackish, but meh.
                    if (checkShouldWhiteCallback == null || checkShouldWhiteCallback(index))
                        prefix.ForeColor = Color.White;
                }
            });

            entry.MouseEnter += mouseOver;
            prefix.MouseEnter += mouseOver;
            entry.MouseLeave += mouseLeave;
            prefix.MouseLeave += mouseLeave;

            entry.MouseClick += mouseClick;
            prefix.MouseClick += mouseClick;

            clickCallbacks.Add(entry, mouseClick);
        }

        internal void SelectIndex(int i)
        {
            selectCallback(i);

            Label entry = entries[i];
            Label prefix = entryPrefixes[i];

            entry.BackColor = prefix.BackColor = selectedColor;
            selectMemory = new Tuple<Tuple<Label, Color>, Tuple<Label, Color>>(new Tuple<Label, Color>(entry, entry.ForeColor), new Tuple<Label, Color>(prefix, prefix.ForeColor));

            entry.ForeColor = Color.White;

            //Refer to SlackChannelList.GetUserLabel, I know, it's hackish, but meh.
            if (!prefix.Name.Equals("#donotwhite"))
                prefix.ForeColor = Color.White;
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
            this.title = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // title
            // 
            //this.title.AutoSize = true;
            this.title.Location = new Point(0, 0);
            this.title.Margin = new Padding(0);
            this.title.TextAlign = ContentAlignment.MiddleLeft;
            this.title.Size = new Size(203, 24);
            this.title.TabIndex = 0;
            this.title.Text = "Title";
            // 
            // TitledList
            // 
            //this.AutoScaleDimensions = new SizeF(6F, 13F);
            //this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.title);
            this.Margin = new Padding(0);
            this.Name = "TitledList";
            this.Size = new Size(200, 220);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label title;
    }
}