using MilkTea.DAL.Models;

namespace MilkTea.BLL
{
    public static class AppConfig
    {
        public static decimal VATRate { get; set; } = 0.10m; // 10%
        public static User? CurrentUser { get; set; }
    }
}
