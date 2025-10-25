using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaPOS.Infrastructure.Settings
{
    public static class DbConfig
    {
        public static string ConnectionString =>
        ConfigurationManager.ConnectionStrings["TeaPOS"].ConnectionString;
    }
}
