using Xunit;

namespace Tardigrade.Framework.ZXingNet.Tests
{
    public class QrCodeProcessorTest
    {
        [Fact]
        public void GenerateAndSave_New_Success()
        {
            // Arrange.
            var data = "https://www.news.com.au";
            var filename = "QR Code.bmp";
            var processor = new CustomQrCodeProcessor();

            // Act.
            processor.Generate(data, filename);

            // Assert.
            Assert.True(true);
        }

        [Fact]
        public void GenerateAndScan_New_Success()
        {
            // Arrange.
            var url = "https://www.news.com.au";
            var processor = new QrCodeProcessor();

            // Act.
            byte[] image = processor.Generate(url);
            string data = processor.Scan(image);

            // Assert.
            Assert.Equal(url, data);
        }
    }
}