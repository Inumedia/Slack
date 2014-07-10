using SlackAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace Slack
{
    public class CachedPictureBox : PictureBox
    {
        static Dictionary<string, Image> cachedImages;

        static int caching;
        static LockFreeQueue<CachedPictureBox> cacheQueue;

        internal string Url;

        static CachedPictureBox()
        {
            cachedImages = new Dictionary<string, Image>();
            cacheQueue = new LockFreeQueue<CachedPictureBox>();
        }

        public CachedPictureBox(string url, int width = 48, int height = 48)
        {
            this.Width = width;
            this.Height = height;

            this.Url = url;

            if (cachedImages.ContainsKey(url))
                Image = cachedImages[url];
            else
            {
                cacheQueue.Push(this);
                StartCache();
            }
        }

        static void StartCache()
        {
            if (Interlocked.CompareExchange(ref caching, 1, 0) == 0)
                new Thread(ExecuteCaching).Start();
        }

        static void ExecuteCaching()
        {
            CachedPictureBox currentlyCaching;
            while (cacheQueue.Pop(out currentlyCaching))
            {
                if (cachedImages.ContainsKey(currentlyCaching.Url))
                {
                    currentlyCaching.UpdateCaching();
                    continue;
                }
                Stream data = new Uri(currentlyCaching.Url).DownloadFile();
                cachedImages.Add(currentlyCaching.Url, Image.FromStream(data));
                currentlyCaching.UpdateCaching();
            }

            caching = 0;
        }

        void UpdateCaching()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateCaching));
                return;
            }
            if (cachedImages.ContainsKey(Url))
                this.Image = cachedImages[Url];
        }
    }
}
