using System;

namespace Tardigrade.Framework.ZXingNet.Tests
{
    internal class CustomQrCodeProcessor : QrCodeProcessor
    {
        public CustomQrCodeProcessor()
        {
            Writer.Options.Height = 250;
            Writer.Options.Width = 250;
        }

        public void Generate(string data, string filename)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            Writer.Write(data).Write(filename);
        }
    }
}