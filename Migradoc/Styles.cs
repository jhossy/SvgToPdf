using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migradoc
{
    public static class Styles
    {
        public static void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];

            // Because all styles are derived from Normal, the next line changes the
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            //style.Font.Name = "Proxima Nova Rg";
            style.Font.Name = "Segoe UI Emoji";
            style.Font.Size = 8;

            
        }
    }
}
