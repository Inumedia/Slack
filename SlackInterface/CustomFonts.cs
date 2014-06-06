using System;
using System.Drawing.Text;
using System.IO;

namespace SlackInterface
{
    public static class CustomFonts
    {
        public static PrivateFontCollection Fonts;
        static CustomFonts()
        {
            Fonts = new PrivateFontCollection();
            Fonts.AddFontFile(Path.Combine(Environment.CurrentDirectory, "Fonts", "FontAwesome.ttf"));
            Fonts.AddFontFile(Path.Combine(Environment.CurrentDirectory, "Fonts", "Lato-Bol.ttf"));
            Fonts.AddFontFile(Path.Combine(Environment.CurrentDirectory, "Fonts", "Lato-Reg.ttf"));
            Fonts.AddFontFile(Path.Combine(Environment.CurrentDirectory, "Fonts", "Lato-RegIta.ttf"));
        }
    }
}
