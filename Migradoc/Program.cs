using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace Migradoc
{
    class Program
    {
        static void Main(string[] args)
        {
            Document document = CreateDocument();
            //Document document = CreateDocumentFromPng();

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = document;            
            renderer.RenderDocument();

            // Save the document...
            string filename = "HelloMigraDoc_png" + DateTime.Now.ToFileTime() + ".pdf";
            renderer.PdfDocument.Save(filename);

            //PNG
            CreatePngFromPdf(document, renderer);

            // ...and start a viewer.
            Process.Start(filename);
        }

        static void CreatePngFromPdf(Document document, PdfDocumentRenderer renderer)
        {
            int page = 1;

            // Reuse the renderer from the preview
            DocumentRenderer renderer2 = new DocumentRenderer(document);
            renderer2.PrepareDocument();

            PageInfo info = renderer.DocumentRenderer.FormattedDocument.GetPageInfo(page);

            // Create an image
            int dpi = 150;
            int dx, dy;
            if (info.Orientation == PdfSharp.PageOrientation.Portrait)
            {
                dx = (int)(info.Width.Inch * dpi);
                dy = (int)(info.Height.Inch * dpi);
            }
            else
            {
                dx = (int)(info.Height.Inch * dpi);
                dy = (int)(info.Width.Inch * dpi);
            }

            //Create image object from the PDF
            //UGLY HACK: divide width by 3 to create only 1 label
            Image image = new Bitmap(dx / 3, dy, PixelFormat.Format32bppRgb);

            // Create a Graphics object for the image and scale it for drawing with 72 dpi
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(System.Drawing.Color.White);
            float scale = dpi / 72f;
            graphics.ScaleTransform(scale, scale);

            // Create an XGraphics object and render the page
            XGraphics gfx = XGraphics.FromGraphics(graphics, new XSize(info.Width.Point, info.Height.Point));
            renderer2.RenderPage(gfx, page);
            gfx.Dispose();
            image.Save("test.png", ImageFormat.Png);
        }
               
        /// <summary>
        /// Creates an absolutely minimalistic document.
        /// </summary>
        static Document CreateDocument()
        {
            // Create a new MigraDoc document
            Document document = new Document();
            document.DefaultPageSetup.PageHeight = Unit.FromCentimeter(1.6);
            document.DefaultPageSetup.PageWidth = Unit.FromCentimeter(10);

            document.DefaultPageSetup.TopMargin = Unit.FromCentimeter(0.1);
            document.DefaultPageSetup.RightMargin = Unit.FromCentimeter(0.2);
            document.DefaultPageSetup.BottomMargin = Unit.FromCentimeter(0.1);
            document.DefaultPageSetup.LeftMargin = Unit.FromCentimeter(0.2);
            
            //Add styling
            Styles.DefineStyles(document);
            
            // Add a section to the document
            Section section = document.AddSection();
            
            string labelText = string.Format("Testing 123 {0} \u26c4 \u26fd {0} \u260e \u0040", Environment.NewLine);
            
            Table table = new Table();
            table.Borders.Width = 0.75;

            for (int i = 0; i < 3; i++)
            {
                Column column = table.AddColumn("1.2cm");
                column.Format.Alignment = ParagraphAlignment.Center;
                Column column2 = table.AddColumn("2.1cm");
                column2.Format.Alignment = ParagraphAlignment.Center;
            }

            Row row = table.AddRow();

            for (int i = 0; i < 3; i++)
            {
                int tmp = i * 2;

                //circle image
                //Image image = new Image("../../circle-outline-512.png");
                //image.WrapFormat.Style = WrapStyle.Through;
                //image.ScaleHeight = 1.7;
                //image.LockAspectRatio = true;
                //image.Width = "0.7cm";

                //row.Cells[tmp].Add(image);

                //icon image
                MigraDoc.DocumentObjectModel.Shapes.Image iconImage = new MigraDoc.DocumentObjectModel.Shapes.Image("../../1352357958_large.png");
                iconImage.ScaleHeight = 0.3;
                iconImage.LockAspectRatio = true;
                //iconImage.Resolution = 200;
                //row.Cells[tmp].Add(iconImage);

                row.Cells[tmp].VerticalAlignment = VerticalAlignment.Center;
                row.Cells[tmp].Format.Alignment = ParagraphAlignment.Center;

                Paragraph imgParagraph = new Paragraph();
                //imgParagraph.Add(image);
                imgParagraph.Add(iconImage);

                row.Cells[tmp].Add(imgParagraph);

                //label text
                row.Cells[tmp + 1].AddParagraph(labelText);
                row.Cells[tmp + 1].VerticalAlignment = VerticalAlignment.Center;
            }
            
            document.LastSection.Add(table);
            
            return document;
        }

        static Document CreateDocumentFromPng()
        {
            // Create a new MigraDoc document
            Document document = new Document();
            document.DefaultPageSetup.PageHeight = Unit.FromCentimeter(1.6);
            document.DefaultPageSetup.PageWidth = Unit.FromCentimeter(10);

            document.DefaultPageSetup.TopMargin = Unit.FromCentimeter(0.1);
            document.DefaultPageSetup.RightMargin = Unit.FromCentimeter(0.2);
            document.DefaultPageSetup.BottomMargin = Unit.FromCentimeter(0.1);
            document.DefaultPageSetup.LeftMargin = Unit.FromCentimeter(0.2);

            document.AddSection();
            document.LastSection.AddImage("../../label131022004747822985.png");

            return document;
        }
    }
}
