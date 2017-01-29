using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ConsoleApplication1
{
    class Inkscape
    {
        public static string Path;
        private XDocument _doc;
        static Inkscape()
        {
            string[] possiblePaths = new string[] {
                @"C:\p\simon\tools\Inkscape-0.91-1-win64\inkscape",
                @"C:\Program Files\Inkscape",
                @"C:\Users\sends\Desktop\simon\toos\inkscape",
                null
            };
            foreach (var path in possiblePaths)
            {
                Path = path;
                if (Directory.Exists(Path))
                    break;
            }
            if (Path == null)
                throw new Exception("Inkscape not found!!");
            
            Path = System.IO.Path.Combine(Path, "inkscape.exe");
        }
        public Inkscape(string x)
        {
            StringReader sr = new StringReader(x);
            _doc = XDocument.Load(sr);
        }

        public string GetSvg()
        {
            return _doc.ToStringWithDeclaration();
        }
        public void DisableId(string id)
        {
            XElement root = _doc.Root;
            var xmlns = "{" + root.GetDefaultNamespace().NamespaceName + "}";
            var gElement = root.Descendants(xmlns + "g").Attributes().Where(a => a.Name == "id" && a.Value == id).Select(z => z.Parent).Single();
            var attribute = gElement.Attributes().SingleOrDefault(a => a.Name == "style");
            if (attribute == null)
            {
                attribute = new XAttribute("style", "");
                gElement.Add(attribute);
            }
            attribute.SetValue("opacity:0");
        }

        public void ReplaceTextInFlowPara(string id, string newText)
        {
            XElement root = _doc.Root;
            var xmlns = "{" + root.GetDefaultNamespace().NamespaceName + "}";
            var gElement = root.Descendants(xmlns + "flowPara").Attributes().Where(a => a.Name == "id" && a.Value == id).Select(z => z.Parent).Single();
            gElement.Value = newText;
        }

    
    }
}