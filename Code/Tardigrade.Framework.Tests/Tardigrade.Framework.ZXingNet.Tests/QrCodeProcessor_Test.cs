using Xunit;

namespace Tardigrade.Framework.ZXingNet.Tests
{
    public class QrCodeProcessor_Test
    {
        [Fact]
        public void GenerateAndSave_New_Success()
        {
            // Arrange.
            string data = "https://www.news.com.au";
            string filename = "QR Code.bmp";
            CustomQrCodeProcessor processor = new CustomQrCodeProcessor();

            // Act.
            processor.Generate(data, filename);

            // Assert.
            Assert.True(true);
        }

        [Fact]
        public void GenerateAndScan_New_Success()
        {
            // Arrange.
            string url = "https://www.news.com.au";
            QrCodeProcessor processor = new QrCodeProcessor();

            // Act.
            byte[] image = processor.Generate(url);
            string data = processor.Scan(image);

            // Assert.
            Assert.Equal(url, data);
        }
    }
}