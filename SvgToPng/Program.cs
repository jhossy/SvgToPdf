using SharpVectors.Converters;
using Svg;
using Svg.Transforms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SvgToPng
{
    class Program
    {
        static void Main(string[] args)
        {
            //1. Create base64 of icon
            Image image = Image.FromFile("../../1352357958_large.png");
            string base64String = ImageToBase64(image, ImageFormat.Png);

            //2. Create SVG : icon + label text 
            XDocument svgXml = CreateBaseXDocument();

            //2.1 add text
            svgXml = AddText(svgXml, new List<string>() { "Jesper", "Hossy", "Blå stue", "12345678" });

            //2.2 add icon
            svgXml = AddIcon(svgXml, base64String);

            //3. Write svg file
            WriteSvgFile(svgXml, "../../label.svg");

            //4. Convert .svg to .png
            using (var stream = new MemoryStream())
            {
                var svgDocument = SvgDocument.Open("../../label.svg");
                var bitmap = svgDocument.Draw(725, 322);
                bitmap.SetResolution(300, 300);
                bitmap.Save("../../label" + DateTime.Now.ToFileTime() + ".png", ImageFormat.Png);
            }
                        
            //var file = "../../label.svg";
            //var converter = new ImageSvgConverter(null);
            //string temporary = file + ".tmp.png";
            //converter.Convert(file, temporary);

            //string convertedFilename = file.Replace(".svg", string.Empty) + ".png";
            //using (Bitmap bitmap = (Bitmap)Image.FromFile(temporary))
            //{
            //    using (Bitmap newBitmap = new Bitmap(bitmap))
            //    {
            //        newBitmap.SetResolution(300, 300);
            //        newBitmap.Save(convertedFilename, ImageFormat.Png);
            //    }
            //}
        }

        public static string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        //<?xml version = "1.0" encoding="utf-8"?>
        //<svg viewBox="0 0 90.7 39.7" version="1.1" xmlns:xlink="http://www.w3.org/1999/xlink" xmlns="http://www.w3.org/2000/svg">
        //  <!-- firkant til tekst -->
        //  <rect x = "19.8" y="0" width="70.9" height="39.7"/>
        //  <!-- outer circle -->
        //  <circle cx = "19.8" cy="19.8" r="19.8"/>
        //  <!-- inner circle -->
        //  <circle cx = "19.8" cy="19.8" r="14.9"/>
        //</svg>
        private static XDocument CreateBaseXDocument()
        {
            XNamespace xmlns = "http://www.w3.org/2000/svg";
            XNamespace xlinkNs = "http://www.w3.org/1999/xlink";

            XDocument svgDoc = new XDocument(
                new XElement(xmlns + "svg", 
                    new XAttribute("viewBox", "0 0 90.7 39.7"), 
                    new XAttribute("version", "1.1"),
                    new XAttribute(XNamespace.Xmlns + "xlink", xlinkNs)
                    ));
            
            //<!-- firkant til tekst -->
            svgDoc.Root.Add(new XComment("firkant til tekst"));

            //<rect x = "19.8" y="0" width="70.9" height="39.7"/>
            svgDoc.Root.Add(new XElement(xmlns + "rect", 
                new XAttribute("x", "19.8"),
                new XAttribute("y", "0"),
                new XAttribute("width", "70.9"),
                new XAttribute("height", "39.7"),
                new XAttribute("fill", "white")));

            //<!-- outer circle -->
            svgDoc.Root.Add(new XComment("outer circle"));

            //<circle cx = "19.8" cy="19.8" r="19.8"/>
            svgDoc.Root.Add(new XElement(xmlns + "circle",
                new XAttribute("cx", "19.8"),
                new XAttribute("cy", "19.8"),
                new XAttribute("r", "19.8"),
                new XAttribute("fill", "white")));

            //<!-- inner circle -->
            //svgDoc.Root.Add(new XComment("inner circle"));

            ////<circle cx = "19.8" cy="19.8" r="14.9"/>
            //svgDoc.Root.Add(new XElement(xmlns + "circle",
            //    new XAttribute("cx", "19.8"),
            //    new XAttribute("cy", "19.8"),
            //    new XAttribute("r", "14.9"),
            //    new XAttribute("fill", "white")));

            return svgDoc;
        }

        // <?xml version = "1.0" encoding="utf-8"?>
        // <svg viewBox = "0 0 90.7 39.7" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
        //  <!--firkant til tekst-->
        //  <rect x = "19.8" y="0" width="70.9" height="39.7" fill="white" />
        //  <text x = "40" y="12" fill="red" font-size="9">this</text>
        //  <text x = "40" y="20" fill="red" font-size="9">is</text>
        //  <text x = "40" y="28" fill="red" font-size="9">a</text>
        //  <text x = "40" y="36" fill="red" font-size="9">test</text>
        //  <!--outer circle-->
        //  <circle cx = "19.8" cy="19.8" r="19.8" fill="white" />
        //  <!--inner circle-->
        //  <circle cx = "19.8" cy="19.8" r="14.9" fill="white"/>
        //</svg>
        private static XDocument AddText(XDocument svg, List<string> texts)
        {
            if(svg == null || !texts.Any() || svg.Descendants("rect").Any())
            {
                return svg;
            }

            XElement rectElement = svg.Descendants().FirstOrDefault(x => x.Name.LocalName == "rect");
            if(rectElement == null)
            {
                throw new NullReferenceException("rect element could not found in SVG");
            }

            XNamespace ns = svg.Root.Name.Namespace;

            List<XElement> textElements = new List<XElement>();

            int yValue = 12;
            foreach (string text in texts)
            {
                //<text x="10" y="5" fill="red">I love SVG!</text>
                textElements.Add(
                    new XElement(ns + "text",
                            new XAttribute("x", "40"),
                            new XAttribute("y", yValue.ToString()),
                            new XAttribute("fill", "blue"),
                            new XAttribute("font-size", "9"), text));

                yValue = yValue + 8;
            }

            rectElement.AddAfterSelf(textElements);

            return svg;
        }

        private static XDocument AddIcon(XDocument svg, string base64EncodedImage)
        {
            if (svg == null || string.IsNullOrEmpty(base64EncodedImage) || svg.Descendants("circle").Any())
            {
                return svg;
            }

            XElement circleElement = svg.Descendants().FirstOrDefault(x => x.Name.LocalName == "circle");
            if (circleElement == null)
            {
                throw new NullReferenceException("circle element could not found in SVG");
            }

            XNamespace ns = svg.Root.Name.Namespace;
            XNamespace xlinkNs = "http://www.w3.org/1999/xlink";

            XElement imageElement = new XElement(ns + "image", 
                new XAttribute("x", 19.8),
                new XAttribute("y", 15.8),
                new XAttribute("width", 10),
                new XAttribute("height", 10),
                new XAttribute(xlinkNs + "href", "data:image/png;base64," + base64EncodedImage));

            circleElement.AddAfterSelf(imageElement);

            return svg;
        }

        private static void WriteSvgFile(XDocument doc, string outputFileName)
        {
            doc.Save(outputFileName);
        }
    }
}
