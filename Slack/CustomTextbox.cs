using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Slack
{
    public class CustomTextbox : Control
    {
        public event Action PreCallback;
        Action<string> callback;
        public event Action<string> OnError;
        TextBox input;
        BorderPanel inputContainer;
        string customPlaceHolder;
        public int BorderWidth
        {
            get { return inputContainer.BorderWidth; }
            set
            {
                inputContainer.BorderWidth = value;
                UpdateSize();
            }
        }
        public Color BorderColor { get { return inputContainer.BorderColor; } set { inputContainer.BorderColor = value; } }
        public int BorderRadius
        {
            get { return inputContainer.BorderRadius; }
            set
            {
                inputContainer.BorderRadius = value;
                UpdateSize();
            }
        }

        public override Color BackColor
        {
            get
            {
                return input.BackColor;
            }
            set
            {
                input.BackColor = inputContainer.BackColor = value;
            }
        }

        public override Color ForeColor
        {
            get
            {
                return input.ForeColor;
            }
            set
            {
                input.ForeColor = value;
            }
        }

        public override string Text
        {
            get
            {
                return input.Text;
            }
            set
            {
                input.Text = value;
            }
        }

        public char PasswordChar
        {
            get
            {
                return input.PasswordChar;
            }
            set
            {
                input.PasswordChar = value;
            }
        }

        public CustomTextbox(Action<string> callback, string placeHolder = null, int fontSize = 16)
        {
            this.callback = callback;
            customPlaceHolder = placeHolder;

            inputContainer = new BorderPanel();
            inputContainer.Dock = DockStyle.Fill;
            input = new TextBox();
            input.BorderStyle = BorderStyle.None;
            input.Font = new Font(CustomFonts.Fonts.Families[1], fontSize);
            input.HandleCreated += input_HandleCreated;
            input.KeyDown += input_KeyDown;

            inputContainer.BackColor = input.BackColor;
            inputContainer.Size = inputContainer.PreferredSize;
            inputContainer.GotFocus += (o, e) => input.Select();

            inputContainer.Controls.Add(input);
            Controls.Add(inputContainer);
            Size = PreferredSize;
        }

        public void PressEnter()
        {
            input.Enabled = false;
            inputContainer.BackColor = Color.FromArgb(0xF0, 0xF0, 0xF0);
            if (PreCallback != null)
                PreCallback();
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    callback(input.Text);
                }
                catch (Exception ex)
                {
                    if (OnError != null)
                        OnError(ex.Message);
                }
                BeginInvoke(new Action(() =>
                {
                    input.Enabled = true;
                    inputContainer.BackColor = Color.Transparent;
                    input.Text = "";
                }));
            });
        }

        void input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13 && e.Modifiers != Keys.Shift)
            {
                e.SuppressKeyPress = true;
                PressEnter();
            }
        }

        void input_HandleCreated(object sender, EventArgs e)
        {
            input.SetPlaceholder(customPlaceHolder);
        }

        protected override void OnResize(EventArgs e)
        {
            UpdateSize();
            base.OnResize(e);
        }

        void UpdateSize()
        {
            input.Location = new Point(BorderRadius, BorderRadius);
            input.Size = new Size(Width - BorderRadius * 2, Height - BorderRadius * 2);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            input.Select();
        }
    }
}
