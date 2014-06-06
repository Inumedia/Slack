using System;
using System.Windows.Forms;
using System.Drawing;

namespace Slack
{
    class SlackLoading : PictureBox
    {
        public const int Delay = 20;
        Image[] images;
        System.Threading.Timer controller;
        public SlackLoading()
        {
            images = new Image[]{
              Properties.Resources.icon_anim_load_1,
              Properties.Resources.icon_anim_load_2,
              Properties.Resources.icon_anim_load_3,
              Properties.Resources.icon_anim_load_4,
              Properties.Resources.icon_anim_load_5,
              Properties.Resources.icon_anim_load_6,
              Properties.Resources.icon_anim_load_7,
              Properties.Resources.icon_anim_load_8,
              Properties.Resources.icon_anim_load_9,
              Properties.Resources.icon_anim_load_10,
              Properties.Resources.icon_anim_load_11,
              Properties.Resources.icon_anim_load_12,
              Properties.Resources.icon_anim_load_13,
              Properties.Resources.icon_anim_load_14,
              Properties.Resources.icon_anim_load_15,
              Properties.Resources.icon_anim_load_16,
              Properties.Resources.icon_anim_load_17,
              Properties.Resources.icon_anim_load_18,
              Properties.Resources.icon_anim_load_19,
              Properties.Resources.icon_anim_load_20,
              Properties.Resources.icon_anim_load_21,
              Properties.Resources.icon_anim_load_22,
              Properties.Resources.icon_anim_load_23,
              Properties.Resources.icon_anim_load_24,
              Properties.Resources.icon_anim_load_25,
              Properties.Resources.icon_anim_load_26,
              Properties.Resources.icon_anim_load_27,
              Properties.Resources.icon_anim_load_28,
              Properties.Resources.icon_anim_load_29,
              Properties.Resources.icon_anim_load_30,
              Properties.Resources.icon_anim_load_31,
              Properties.Resources.icon_anim_load_32,
              Properties.Resources.icon_anim_load_33,
              Properties.Resources.icon_anim_load_34,
              Properties.Resources.icon_anim_load_35,
              Properties.Resources.icon_anim_load_36,
              Properties.Resources.icon_anim_load_37,
              Properties.Resources.icon_anim_load_38,
              Properties.Resources.icon_anim_load_39,
              Properties.Resources.icon_anim_load_40,
              Properties.Resources.icon_anim_load_41,
              Properties.Resources.icon_anim_load_42,
              Properties.Resources.icon_anim_load_43,
              Properties.Resources.icon_anim_load_44,
              Properties.Resources.icon_anim_load_45,
              Properties.Resources.icon_anim_load_46,
              Properties.Resources.icon_anim_load_47
            };

            Image = images[0];
        }

        int currentImage = 0;
        void Animate(object s)
        {
            BeginInvoke(new Action(IncrementImage));
        }

        void IncrementImage()
        {
            ++currentImage;
            if (currentImage == images.Length)
                currentImage = 0;
            Image = images[currentImage];
        }

        public void StartAnimation()
        {
            currentImage = 0;
            Image = images[0];
            controller = new System.Threading.Timer(Animate, null, Delay, Delay);
        }

        public void StopAnimation()
        {
            if (controller != null)
            {
                controller.Dispose();
                controller = null;
            }
        }
    }
}
