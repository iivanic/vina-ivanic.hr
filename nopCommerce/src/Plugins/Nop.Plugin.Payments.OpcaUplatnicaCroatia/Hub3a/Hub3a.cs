
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

namespace Nop.Plugin.Payments.OpcaUplatnicaCroatia.Hub3a
{
    //Generira PDF417 2D BAR-KOD prema HUB3a standardu.

    public class Hub3a: IDisposable
    {
        public enum Hub3aPNGSize
        {
            default_size,
            double_size,
            triple_size,
            quadruple_size,
            quintuple_size
        }
        int ERROR_CORRECTION_LEVEL = 2;
        #region props
        /// <summary>
        /// Max length 8
        /// </summary>
        public string Zaglavlje { get; set; } = "HRVHUB30";
        /// <summary>
        /// Max length 3
        /// </summary>
        public string ValutaPlacanja { get; set; } = "EUR";
        /// <summary>
        /// Max length 15
        /// </summary>
        private string iznos;
        public string Iznos
        {
            get
            {
                return iznos;
            }
            set
            {
                iznos = value;
                if (!string.IsNullOrEmpty(iznos))
                {
                    if (iznos.Length < 15)
                        iznos = iznos.PadLeft(15, '0');
                    if (iznos.Length > 15)
                        iznos = iznos.Substring(iznos.Length - 15);
                }
            }
        }
        /// <summary>
        /// Max length 30
        /// </summary>
        public string PlatiteljPrvaLinija { get; set; } = "PLATITELJ";
        /// <summary>
        /// Max length 27
        /// </summary>
        public string PlatiteljDrugaLinija { get; set; } = "PLATITELJ ADRESA";
        /// <summary>
        /// Max length 27
        /// </summary>
        public string PlatiteljTrecaLinija { get; set; } = "PLATITELJ MJESTO";
        /// <summary>
        /// Max length 25
        /// </summary>
        public string PrimateljPrvaLinija { get; set; } = "PRIMATELJ";
        /// <summary>
        /// Max length 25
        /// </summary>
        public string PrimateljDrugaLinija { get; set; } = "PRIMATELJ ADRESA";
        /// <summary>
        /// Max length 27
        /// </summary>
        public string PrimateljTrecaLinija { get; set; } = "PRIMATELJ MJESTO";
        /// <summary>
        /// Max length 21
        /// </summary>
        public string PrimateljIBAN { get; set; } = "HR0000000000000000000";
        /// <summary>
        /// Max length 4
        /// </summary>
        public string Model { get; set; } = "HR99";
        /// <summary>
        /// Max length 22
        /// </summary>
        public string PozivNaBroj { get; set; } = "000000";
        /// <summary>
        /// Max length 4
        /// </summary>
        public string SifraNamjene { get; set; } = "COST";
        /// <summary>
        /// Max length 35
        /// </summary>
        public string Opis { get; set; } = "Troškovi";
        #endregion
        public Hub3a()
        {
            if (PdfSharpCore.Fonts.GlobalFontSettings.FontResolver == null)
                PdfSharpCore.Fonts.GlobalFontSettings.FontResolver = new FontResolver();
            if (PdfSharpCore.Fonts.GlobalFontSettings.FontResolver.GetType() != typeof(FontResolver))
                PdfSharpCore.Fonts.GlobalFontSettings.FontResolver = new FontResolver();


            iznos = "000000000000000";
        }
        public Hub3a(string barCodeText) : this()
        {

            string[] t = barCodeText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Zaglavlje = $"{t[0]}";
            ValutaPlacanja = $"{t[1]}";
            Iznos = $"{t[2]}";
            PlatiteljPrvaLinija = $"{t[3]}";
            PlatiteljDrugaLinija = $"{t[4]}";
            PlatiteljTrecaLinija = $"{t[5]}";
            PrimateljPrvaLinija = $"{t[6]}";
            PrimateljDrugaLinija = $"{t[7]}";
            PrimateljTrecaLinija = $"{t[8]}";
            PrimateljIBAN = $"{t[9]}";
            Model = $"{t[10]}";
            PozivNaBroj = $"{t[11]}";
            SifraNamjene = $"{t[12]}";
            Opis = $"{t[13]}";
        }
        public string GetBarCodeText()
        {

            return Zaglavlje + "\n" +
                ValutaPlacanja + "\n" +
                Iznos + "\n" +
                PlatiteljPrvaLinija + "\n" +
                PlatiteljDrugaLinija + "\n" +
                PlatiteljTrecaLinija + "\n" +
                PrimateljPrvaLinija + "\n" +
                PrimateljDrugaLinija + "\n" +
                PrimateljTrecaLinija + "\n" +
                PrimateljIBAN + "\n" +
                Model + "\n" +
                PozivNaBroj + "\n" +
                SifraNamjene + "\n" +
                Opis + "\n";
        }
        public async void DajPDFUplatnicu(string pdfFilePath)
        {
            if (System.IO.File.Exists(pdfFilePath))
                System.IO.File.Delete(pdfFilePath);

            using (FileStream outputFileStream = new FileStream(pdfFilePath, FileMode.Create))
            {
                await DajPDFUplatnicu().CopyToAsync(outputFileStream);
            }

        }
        public Stream DajPDFUplatnicu()
        {
            Stream ret = new MemoryStream();

            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
            var reader = embeddedProvider.GetFileInfo("Hub3a.Pdf.hub-3a.pdf").CreateReadStream();

            //File dimentions - Width = 17 inches, Height - 11 inches (Tabloid Format)
            PdfDocument pdfDocument = PdfReader.Open(reader, PdfDocumentOpenMode.Modify);

            PdfPage page = pdfDocument.Pages[0];

            var gfx = XGraphics.FromPdfPage(page);
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode);
            // no options to embed fonts in PDFSharpCore, seems to work automaticallz

            var font = new XFont("OpenSans", 10, XFontStyle.Bold, options);

            var pomX = -1;
            var pomY = -1.1;
            gfx.DrawString(PlatiteljPrvaLinija, font, XBrushes.Black, new XRect(40 + pomX, 45 + pomY, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString(PlatiteljDrugaLinija, font, XBrushes.Black, new XRect(40 + pomX, 60 + pomY, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString(PlatiteljTrecaLinija, font, XBrushes.Black, new XRect(40 + pomX, 75 + pomY, page.Width, page.Height), XStringFormats.TopLeft);

            gfx.DrawString(PrimateljPrvaLinija, font, XBrushes.Black, new XRect(40 + pomX, 120 + pomY, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString(PrimateljDrugaLinija, font, XBrushes.Black, new XRect(40 + pomX, 135 + pomY, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString(PrimateljTrecaLinija, font, XBrushes.Black, new XRect(40 + pomX, 150 + pomY, page.Width, page.Height), XStringFormats.TopLeft);

            int cnt = 0;
            foreach (char c in ValutaPlacanja.ToCharArray())
            {
                gfx.DrawString(ValutaPlacanja.Substring(cnt, 1), font, XBrushes.Black, new XRect(239 + pomX + (cnt * 9.56), 37 + pomY, page.Width, page.Height), XStringFormats.TopLeft);
                cnt++;

            }

            cnt = 0;
            foreach (char c in Iznos.ToCharArray())
            {
                gfx.DrawString(Iznos.Substring(cnt, 1), font, XBrushes.Black, new XRect(296 + pomX + (cnt * 9.56), 37 + pomY, page.Width, page.Height), XStringFormats.TopLeft);
                cnt++;

            }

            cnt = 0;
            foreach (char c in PrimateljIBAN.ToCharArray())
            {
                gfx.DrawString(PrimateljIBAN.Substring(cnt, 1), font, XBrushes.Black, new XRect(239 + pomX + (cnt * 9.56), 94 + pomY, page.Width, page.Height), XStringFormats.TopLeft);
                cnt++;

            }

            cnt = 0;
            foreach (char c in Model.ToCharArray())
            {
                gfx.DrawString(Model.Substring(cnt, 1), font, XBrushes.Black, new XRect(172 + pomX + (cnt * 9.56), 117 + pomY, page.Width, page.Height), XStringFormats.TopLeft);
                cnt++;

            }

            cnt = 0;
            foreach (char c in PozivNaBroj.ToCharArray())
            {
                gfx.DrawString(PozivNaBroj.Substring(cnt, 1), font, XBrushes.Black, new XRect(230 + pomX + (cnt * 9.56), 117 + pomY, page.Width, page.Height), XStringFormats.TopLeft);
                cnt++;

            }

            cnt = 0;
            foreach (char c in SifraNamjene.ToCharArray())
            {
                gfx.DrawString(SifraNamjene.Substring(cnt, 1), font, XBrushes.Black, new XRect(172 + pomX + (cnt * 9.56), 143 + pomY, page.Width, page.Height), XStringFormats.TopLeft);
                cnt++;

            }

            gfx.DrawString(Opis, font, XBrushes.Black, new XRect(258 + pomX, 132 + pomY, page.Width, page.Height), XStringFormats.TopLeft);

            var fontSmall = new XFont("OpenSans", 8, XFontStyle.Bold, options);

            gfx.DrawString($"{ValutaPlacanja}={int.Parse(Iznos) / 100f}", fontSmall, XBrushes.Black, new XRect(458 + pomX, 41 + pomY, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString($"{PlatiteljPrvaLinija}", fontSmall, XBrushes.Black, new XRect(458 + pomX, 58 + pomY, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString($"{PrimateljIBAN}", fontSmall, XBrushes.Black, new XRect(458 + pomX, 98 + pomY, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString($"{Model} {PozivNaBroj}", fontSmall, XBrushes.Black, new XRect(458 + pomX, 120.5 + pomY, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString($"{Opis}", fontSmall, XBrushes.Black, new XRect(458 + pomX, 139 + pomY, page.Width, page.Height), XStringFormats.TopLeft);

            //  bar code
            Stream bc = DajBarKodPNG(Hub3aPNGSize.quadruple_size);
            bc.Position = 0; //Fix bug PdfSharpCore 
            XImage image = XImage.FromStream(() => bc);
            gfx.DrawImage(image, 38f, 180f, 163, 163 / (256 / 60));

            // --------
            pdfDocument.Save(ret);
            return ret;
        }
        public Stream DajBarKodPNG(Hub3aPNGSize pngSize = Hub3aPNGSize.default_size)
        {
            Stream ret = new MemoryStream();

            var pixelData = DajBarKodPixelData();

            var bgColor = SixLabors.ImageSharp.PixelFormats.Rgba32.ParseHex("#FFFFFF");

            var image = Image.LoadPixelData<SixLabors.ImageSharp.PixelFormats.Rgba32>(pixelData.Pixels, pixelData.Width, pixelData.Height);

            int width = image.Width;
            int height = image.Height;
            int k = 0;
            switch (pngSize)
            {
                case Hub3aPNGSize.default_size:
                    k = 1;
                    break;
                case Hub3aPNGSize.double_size:
                    k = 2;
                    break;
                case Hub3aPNGSize.triple_size:
                    k = 3;
                    break;
                case Hub3aPNGSize.quadruple_size:
                    k = 4;
                    break;
                case Hub3aPNGSize.quintuple_size:
                    k = 5;
                    break;

            }
            //scale image
            image.Mutate(x => x.Resize(width*k, height*k, KnownResamplers.Box));

            image.SaveAsPng(ret, new PngEncoder()
            {
                BitDepth = PngBitDepth.Bit1,
                ColorType = PngColorType.Grayscale
            });
            ret.Position = 0; ; //Fix bug PdfSharpCore 
            return ret;
        }

        public async void DajBarKodPNG(string barCodePNGFilePath, Hub3aPNGSize pngSize = Hub3aPNGSize.default_size)
        {
            if (System.IO.File.Exists(barCodePNGFilePath))
                System.IO.File.Delete(barCodePNGFilePath);

            using (FileStream outputFileStream = new FileStream(barCodePNGFilePath, FileMode.Create))
            {
                await DajBarKodPNG(pngSize).CopyToAsync(outputFileStream);
            }
        }
        private ZXing.Rendering.PixelData DajBarKodPixelData()
        {

            var barcodeWriterPixelData = new ZXing.BarcodeWriterPixelData
            {
                Format = BarcodeFormat.PDF_417,
                Options = new EncodingOptions
                {
                    Margin = 0
                },
                Renderer = new PixelDataRenderer
                {
                    Foreground = new PixelDataRenderer.Color(unchecked((int)0xFF000000)),
                    Background = new PixelDataRenderer.Color(unchecked((int)0xFFFFFFFF)),

                }
            };
            barcodeWriterPixelData.Options.Hints[EncodeHintType.ERROR_CORRECTION] = ERROR_CORRECTION_LEVEL;
            barcodeWriterPixelData.Options.Hints[EncodeHintType.PDF417_AUTO_ECI] = true;
           
            return barcodeWriterPixelData.Write(GetBarCodeText());

        }
        public string DajBarKodSVG()
        {

            var barcodeWriter = new ZXing.BarcodeWriterSvg
            {
                Format = BarcodeFormat.PDF_417,
                Options = new EncodingOptions
                {
                    Margin = 0
                }

            };
            barcodeWriter.Options.Hints[EncodeHintType.ERROR_CORRECTION] = ERROR_CORRECTION_LEVEL;
            ZXing.Rendering.SvgRenderer.SvgImage ret = barcodeWriter.Write(GetBarCodeText());

            return ret.Content;

        }
        public async void DajBarKodSVG(string barCodeSVGFilePath)
        {
            if (System.IO.File.Exists(barCodeSVGFilePath))
                System.IO.File.Delete(barCodeSVGFilePath);

            await File.WriteAllTextAsync(barCodeSVGFilePath, DajBarKodSVG());

        }

        public void Dispose()
        {
            
        }
    }
}