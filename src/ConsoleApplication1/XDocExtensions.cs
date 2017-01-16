using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ConsoleApplication1
{
    public static class XDocExtensions
    {
        public static string ToStringWithDeclaration(this XDocument doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc");
            }
            StringBuilder builder = new StringBuilder();
            using (TextWriter writer = new EncodingStringWriter(builder, Encoding.UTF8))
            {
                doc.Save(writer);
            }
            return builder.ToString();
        }
    }
    public class EncodingStringWriter : StringWriter
    {
        private readonly Encoding _encoding;

        public EncodingStringWriter(StringBuilder builder, Encoding encoding) : base(builder)
        {
            _encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return _encoding; }
        }
    }

}