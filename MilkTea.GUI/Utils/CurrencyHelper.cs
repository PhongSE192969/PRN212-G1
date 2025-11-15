using System.Globalization;

namespace MilkTea.GUI.Utils
{
    public static class CurrencyHelper
    {
        private static readonly CultureInfo _vietnameseCulture = new CultureInfo("vi-VN");
        
        public static string FormatVND(decimal amount)
        {
            return amount.ToString("N0", _vietnameseCulture) + " đ";
        }
        
        public static string FormatNumber(decimal number)
        {
            return number.ToString("N0", _vietnameseCulture);
        }
        
        public static decimal ParseVND(string text)
        {
            // Remove currency symbol and parse
            text = text.Replace("đ", "").Replace(",", "").Trim();
            return decimal.TryParse(text, out decimal result) ? result : 0;
        }
    }
}
