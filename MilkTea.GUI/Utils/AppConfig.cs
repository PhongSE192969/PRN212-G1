using MilkTea.DAL.Models;

namespace MilkTea.GUI.Utils
{
    public static class AppConfig
    {
        public static User? CurrentUser { get; set; }
        
        public static bool IsAdmin => CurrentUser?.Role == "Admin";
        public static bool IsStaff => CurrentUser?.Role == "Staff";
        
        // VAT rate from config
        public static decimal VATRate { get; set; } = 0.10m;
        
        // Company info
        public static string CompanyName { get; set; } = "TeaPOS - Hệ Thống Thu Ngân Trà Sữa";
        public static string CompanyAddress { get; set; } = "FPT University, Hòa Lạc, Hà Nội";
        public static string CompanyPhone { get; set; } = "024-xxxx-xxxx";
    }
}
