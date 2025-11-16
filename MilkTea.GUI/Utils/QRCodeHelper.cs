using System.Windows.Media.Imaging;
using System.IO;
using QRCoder;

namespace MilkTea.GUI.Utils
{
    public static class QRCodeHelper
    {
        public static BitmapImage GenerateQRCode(string data, int pixelsPerModule = 20)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrCodeData);
            using var qrCodeImage = qrCode.GetGraphic(pixelsPerModule);
            
            using var memory = new MemoryStream();
            qrCodeImage.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            memory.Position = 0;
            
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            
            return bitmapImage;
        }
        
        public static string GeneratePaymentData(int invoiceId, decimal amount)
        {
            // Format: INVOICE_ID|AMOUNT|TIMESTAMP
            return $"{invoiceId}|{amount}|{DateTime.Now:yyyyMMddHHmmss}";
        }
    }
}
