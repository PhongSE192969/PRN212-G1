using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace MOMO_QR_DANANG.Services
{
    public static class APIRequestHelper
    {
        // Chuyển đổi System.Drawing.Image sang Base64
        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);

                string mimeType = GetMimeType(format);
                return $"data:{mimeType};base64,{base64String}";
            }
        }

        private static string GetMimeType(ImageFormat format)
        {
            if (format.Equals(ImageFormat.Png)) return "image/png";
            if (format.Equals(ImageFormat.Jpeg)) return "image/jpeg";
            return "image/png";
        }

        // Chuyển đổi Base64 (từ API) sang WPF BitmapImage để hiển thị
        public static BitmapImage Base64ToBitmapImage(string base64String)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (var ms = new MemoryStream(imageBytes))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    image.Freeze();
                    return image;
                }
            }
            catch
            {
                return null;
            }
        }

        // Chuyển đổi WPF Image Source sang System.Drawing.Image cho Base64
        public static Image ImageSourceToDrawingImage(ImageSource imageSource)
        {
            if (imageSource is BitmapImage bitmapImage)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    return Image.FromStream(stream);
                }
            }
            return null;
        }
    }

    // --- DATA MODELS ---

    public class APIRequest
    {
        public string accountNo { get; set; }
        public string accountName { get; set; }
        public int acqId { get; set; }
        public int amount { get; set; }
        public string addInfo { get; set; }
        public string logo { get; set; }
        public string format { get; set; }
        public string template { get; set; }
        public string theme { get; set; }
    }

    public class Data
    {
        public int acpId { get; set; }
        public string accountName { get; set; }
        public string qrCode { get; set; }
        public string qrDataURL { get; set; }
    }

    public class ApiResponse
    {
        public string code { get; set; }
        public string desc { get; set; }
        public Data data { get; set; }
    }
}