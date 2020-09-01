using ImageMagick;
using System;
using System.Collections.Generic;
using Tardigrade.Framework.Services.QrCodes;
using ZXing;

namespace Tardigrade.Framework.ZXingNet
{
    /// <summary>
    /// Implementation of a QR code processor based on the ZXing.Net library.
    /// </summary>
    public class QrCodeProcessor : IQrCodeProcessor
    {
        /// <summary>
        /// QR code scanner.
        /// </summary>
        protected ZXing.Magick.BarcodeReader Reader { get; set; }

        /// <summary>
        /// QR code generator.
        /// </summary>
        protected ZXing.Magick.BarcodeWriter Writer { get; set; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        public QrCodeProcessor()
        {
            Reader = new ZXing.Magick.BarcodeReader
            {
                Options = { PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE } }
            };

            Writer = new ZXing.Magick.BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE
            };
        }

        /// <summary>
        /// <see cref="IQrCodeProcessor.Scan(byte[])"/>
        /// </summary>
        public string Scan(byte[] image)
        {
            if (image == null || image.Length == 0)
            {
                throw new ArgumentNullException(nameof(image));
            }

            string data;

            try
            {
                using (MagickImage magickImage = new MagickImage(image))
                {
                    data = Reader.Decode(magickImage)?.Text;
                }
            }
            catch (MagickException e)
            {
                throw new ArgumentException($"QR code not of a valid format: {e.GetBaseException().Message}", nameof(image), e);
            }

            return data;
        }

        /// <summary>
        /// <see cref="IQrCodeProcessor.Generate(string)"/>
        /// </summary>
        public byte[] Generate(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            byte[] image = Writer.Write(data).ToByteArray();

            return image;
        }
    }
}