using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ConsoleApplication1
{
    class Inkscape
    {
        public static string Path = @"C:\Users\sends\Desktop\simon\toos\inkscape";

        static Inkscape()
        {
            if (!Directory.Exists(Path))
            {
                Path = @"C:\p\simon\tools\Inkscape-0.91-1-win64\inkscape";
            }
            Path = System.IO.Path.Combine(Path, "inkscape.exe");
        }

        public static string DisableId(string x, string id)
        {
            using (StringReader sr = new StringReader(x))
            {
                XDocument doc = XDocument.Load(sr);
                XElement root = doc.Root;
                var xmlns = "{" + root.GetDefaultNamespace().NamespaceName + "}";
                var gElement = root.Descendants(xmlns + "g").Attributes().Where(a => a.Name == "id" && a.Value == id).Select(z => z.Parent).Single();
                var attribute = gElement.Attributes().Where(a => a.Name == "style").SingleOrDefault();
                if (attribute == null)
                {
                    attribute = new XAttribute("style", "");
                    gElement.Add(attribute);
                }
                attribute.SetValue("opacity:0");
                return doc.ToStringWithDeclaration();
            }
        }

        internal static string ReplaceTextInFlowPara(string x, string id, string newText)
        {
            using (StringReader sr = new StringReader(x))
            {
                XDocument doc = XDocument.Load(sr);
                XElement root = doc.Root;
                var xmlns = "{" + root.GetDefaultNamespace().NamespaceName + "}";
                var gElement = root.Descendants(xmlns + "flowPara").Attributes().Where(a => a.Name == "id" && a.Value == id).Select(z => z.Parent).Single();
                gElement.Value = newText;
                return doc.ToStringWithDeclaration();
            }
        }
    }
}