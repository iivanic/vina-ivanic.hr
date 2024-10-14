using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using PdfSharpCore.Fonts;

namespace Nop.Plugin.Payments.OpcaUplatnicaCroatia.Hub3a
{
    //This implementation is obviously not very good --> Though it should be enough for everyone to implement their own.
    public class FontResolver : IFontResolver {
        public string DefaultFontName => "OpenSans";

        public byte[] GetFont (string faceName) {
            using (var ms = new MemoryStream ()) {
                var embeddedProvider = new EmbeddedFileProvider (Assembly.GetExecutingAssembly ());
                var fs = embeddedProvider.GetFileInfo ("Hub3a.Fonts." + faceName).CreateReadStream ();
                fs.CopyTo (ms);
                ms.Position = 0;
                return ms.ToArray ();
            }
        }
        public FontResolverInfo ResolveTypeface (string familyName, bool isBold, bool isItalic) {
            if (familyName.Equals ("OpenSans", StringComparison.CurrentCultureIgnoreCase)) {
                if (isBold && isItalic) {
                    return new FontResolverInfo ("OpenSans-BoldItalic.ttf");
                } else if (isBold) {
                    return new FontResolverInfo ("OpenSans-Bold.ttf");
                } else if (isItalic) {
                    return new FontResolverInfo ("OpenSans-Italic.ttf");
                } else {
                    return new FontResolverInfo ("OpenSans-Regular.ttf");
                }
            }
            return null;
        }

    }
}