using System.IO;
using System.Text;
using System.Xml;

namespace Tardigrade.Framework.Serialization.Xml.Writers
{
    /// <summary>
    /// Custom XmlTextWriter that omits the XML declaration and converts Boolean attributes to 0 (false) or 1 (true).
    /// </summary>
    public class BoolToIntWriter : XmlTextWriter
    {
        private const string FalseString = "0";
        private const string TrueString = "1";

        /// <inheritdoc/>
        public BoolToIntWriter(Stream w, Encoding encoding) : base(w, encoding)
        {
        }

        /// <inheritdoc/>
        public BoolToIntWriter(string filename, Encoding encoding) : base(filename, encoding)
        {
        }

        /// <inheritdoc/>
        public BoolToIntWriter(TextWriter w) : base(w)
        {
        }

        /// <summary>
        /// Override the base implementation and do nothing as this will omit the XML declaration.
        /// </summary>
        public override void WriteStartDocument()
        {
        }

        /// <summary>
        /// Override the base implementation to convert Boolean attributes to 0 (false) or 1 (true).
        /// </summary>
        /// <param name="text">Text associated with the attribute.</param>
        public override void WriteString(string text)
        {
            if (bool.TryParse(text, out bool value))
            {
                base.WriteString(value ? TrueString : FalseString);
            }
            else
            {
                base.WriteString(text);
            }
        }
    }
}