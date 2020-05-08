namespace Tardigrade.Framework.Services.QrCodes
{
    /// <summary>
    /// Interface for operations associated with QR codes.
    /// </summary>
    public interface IQrCodeProcessor
    {
        /// <summary>
        /// Extract the data contained within the QR code image.
        /// </summary>
        /// <param name="image">QR code image.</param>
        /// <returns>Data contained within the QR code image.</returns>
        /// <exception cref="System.ArgumentException">image is not of an appropriate format.</exception>
        /// <exception cref="System.ArgumentNullException">image is null or empty.</exception>
        string Scan(byte[] image);

        /// <summary>
        /// Generate a QR code image containing the specified data.
        /// </summary>
        /// <param name="data">Data for inclusion in the QR code image.</param>
        /// <returns>QR code image.</returns>
        /// <exception cref="System.ArgumentNullException">data is null or empty.</exception>
        byte[] Generate(string data);
    }
}